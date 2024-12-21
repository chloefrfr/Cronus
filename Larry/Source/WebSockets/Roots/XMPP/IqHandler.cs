using Fleck;
using Larry;
using Larry.Source;
using Larry.Source.Database.Entities;
using Larry.Source.Repositories;
using Larry.Source.Utilities;
using Larry.Source.WebSockets;
using Larry.Source.WebSockets.Models;
using Larry.Source.WebSockets.Roots;
using Larry.Source.WebSockets.Roots.XMPP;
using Larry.Source.WebSockets.Services;
using System;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Larry.Source.WebSockets.Roots.XMPP
{
    public class IqHandler
    {
        public static async Task HandleAsync(IWebSocketConnection socket, SocketClient client, XElement root)
        {
            var AttributeId = root.Attribute("id");
            Config config = Config.GetConfig();
            var friendRepository = new Repository<Friends>(config.ConnectionUrl);

            switch (AttributeId!.Value)
            {
                case "_xmpp_bind1":
                    var resource = root.Elements().Where(x => x.Name.LocalName == "bind").First().Elements().First().Value.ToString();
                    client.Resource = resource;
                    client.Jid = $"{client.AccountId}@prod.ol.epicgames.com/{client.Resource}";

                    socket.Send(
                        new XElement(XNamespace.Get("jabber:client") + "iq",
                            new XAttribute("to", socket.ConnectionInfo.Id),
                            new XAttribute("id", "_xmpp_bind1"),
                            new XAttribute("type", "result"),
                            new XElement(XNamespace.Get("urn:ietf:params:xml:ns:xmpp-bind") + "bind",
                                new XElement(XNamespace.Get("urn:ietf:params:xml:ns:xmpp-bind") + "jid", socket.ConnectionInfo.Id)
                            )
                        ).ToString()
                    );
                    break;

                case "_xmpp_session1":
                    socket.Send(new XElement(XNamespace.Get("jabber:client") + "iq",
                        new XAttribute("to", client.Jid),
                        new XAttribute("from", "prod.ol.epicgames.com"),
                        new XAttribute("id", "_xmpp_session1"),
                        new XAttribute("xmlns", "jabber:client"),
                        new XAttribute("type", "result")
                    ).ToString());

                    var user = await friendRepository.FindFriendsByAccountIdAsync(client.AccountId);

                    if (user == null)
                    {
                        Logger.Error($"User '{client.AccountId}' not found.");
                        socket.Close();
                        return;
                    }

                    foreach (var friend in user)
                    {
                        if (friend.Status == "ACCEPTED")
                        {
                            var cl = XmppService.Clients.FirstOrDefault(c => c.Value.AccountId == friend.AccountId);

                            if (cl.Equals(default(KeyValuePair<string, SocketClient>)))
                            {
                                Logger.Error($"Friend with AccountId '{friend.AccountId}' not found.");
                                socket.Close();
                                return;
                            }

                            var presenceXml = new XElement("presence",
                                new XAttribute("to", client.Jid),
                                new XAttribute("xmlns", "jabber:client"),
                                new XAttribute("from", cl.Value.Jid),
                                new XAttribute("type", "available")
                            );

                            if (cl.Value.LastPresenceUpdate.Away)
                            {
                                presenceXml.Add(new XElement("show", "away"));
                                presenceXml.Add(new XElement("status", cl.Value.LastPresenceUpdate.Status));
                            }
                            else
                            {
                                presenceXml.Add(new XElement("status", cl.Value.LastPresenceUpdate.Status));
                            }

                            socket.Send(presenceXml.ToString());
                        }
                        else
                        {
                            Logger.Information("How'd you manage this.");
                        }
                    }
                    break;

                default:
                    Logger.Warning($"Missing attributeId: {AttributeId.Value}");
                    socket.Send(
                        new XElement(XNamespace.Get("jabber:client") + "iq",
                            new XAttribute("to", socket.ConnectionInfo.Id),
                            new XAttribute("from", "prod.ol.epicgames.com"),
                            new XAttribute("id", AttributeId.Value),
                            new XAttribute("type", "result")
                        ).ToString()
                    );
                    break;
            }
        }
    }
}
