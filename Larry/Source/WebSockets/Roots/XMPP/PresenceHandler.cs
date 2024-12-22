using Fleck;
using Larry;
using Larry.Source;
using Larry.Source.Utilities;
using Larry.Source.WebSockets;
using Larry.Source.WebSockets.Helpers.XMPP;
using Larry.Source.WebSockets.Models;
using Larry.Source.WebSockets.Roots;
using Larry.Source.WebSockets.Roots.XMPP;
using Larry.Source.WebSockets.Services;
using Newtonsoft.Json;
using System.Xml.Linq;

namespace Larry.Source.WebSockets.Roots.XMPP
{
    public class PresenceHandler
    {
        public static async Task HandleAsync(IWebSocketConnection socket, SocketClient client, XElement root)
        {
            bool hasXElement = root.Elements().Any(e => e.Name.LocalName == "x");
            bool hasTypeAttribute = root.Attribute("type") != null;

            if (hasTypeAttribute)
            {
                string rootType = root.Attribute("type")?.Value;
                string to = root.Attribute("to")?.Value;

                Logger.Information($"LOG : [PresenceHandler] - Root Type: {rootType}");

                if (rootType == "unavailable")
                {
                    if (to.EndsWith("@muc.prod.ol.epicgames.com") ||
                        to.Split("/")[0].EndsWith("@muc.prod.ol.epicgames.com"))
                    {
                        var name = to.Split("@")[0];
                        var room = XmppService.XmppMucsDictionary[name];

                        if (room != null)
                        {
                            var rMemberIndex = room.Members.FindIndex(m => m.AccountId == client.AccountId);

                            if (rMemberIndex != -1)
                            {
                                room.Members.RemoveAt(rMemberIndex);
                                XmppService.JoinedMUCs.RemoveAt(XmppService.JoinedMUCs.IndexOf(name));
                            }

                            XNamespace mucUserNs = XNamespace.Get("http://jabber.org/protocol/muc#user");

                            socket.Send(new XElement(XNamespace.Get("jabber:client") + "presence",
                                new XAttribute("to", client.Jid),
                                new XAttribute("from", $"{name}@muc.prod.ol.epicgames.com/{Uri.EscapeDataString(client.DisplayName)}:{client.AccountId}:{client.Resource}"),
                                new XAttribute("xmlns", XNamespace.Get("jabber:client")),
                                new XAttribute("type", "unavailable"),
                                new XElement(mucUserNs + "x",
                                    new XAttribute("xmlns", mucUserNs),
                                    new XElement(mucUserNs + "item",
                                        new XAttribute("nick", $"{name}@muc.prod.ol.epicgames.com/{Uri.EscapeDataString(client.DisplayName)}:{client.AccountId}:{client.Resource}".Replace($"{name}@muc.prod.ol.epicgames.com/", "")),
                                        new XAttribute("jid", client.Jid),
                                        new XAttribute("role", "none")
                                    ),
                                    new XElement(mucUserNs + "status", new XAttribute("code", "110")),
                                    new XElement(mucUserNs + "status", new XAttribute("code", "100")),
                                    new XElement(mucUserNs + "status", new XAttribute("code", "170"))
                                )
                            ).ToString());
                        }
                    }
                }
            }

            if (hasXElement)
            {
                string to = root.Attribute("to")?.Value;

                var name = to.Split("@")[0];

                if (XmppService.XmppMucsDictionary[name] == null)
                {
                    XmppService.XmppMucsDictionary[name] = new MUCInfo();
                }

                if (XmppService.XmppMucsDictionary[name].Members.Any(member => member.AccountId == client.AccountId))
                    return;

                XmppService.XmppMucsDictionary[name].Members.Add(new MUCMember
                {
                    AccountId = client.AccountId,
                });
                XmppService.JoinedMUCs.Add(name);

                socket.Send(new XElement(XNamespace.Get("jabber:client") + "presence",
                    new XAttribute("to", client.Jid),
                    new XAttribute("from", $"{name}@muc.prod.ol.epicgames.com/{client.DisplayName}:{client.AccountId}:{client.Resource}"),
                    new XAttribute("type", "available"),
                    new XElement(XNamespace.Get("http://jabber.org/protocol/muc#user") + "x",
                        new XElement("item",
                            new XAttribute("nick", $"{client.DisplayName}:{client.AccountId}:{client.Resource}"),
                            new XAttribute("jid", client.Jid),
                            new XAttribute("role", "participant"),
                            new XAttribute("affiliation", "none")),
                        new XElement("status",
                            new XAttribute("code", "110")),
                        new XElement("status",
                            new XAttribute("code", "100")),
                        new XElement("status",
                            new XAttribute("code", "170"))
                    )
                ).ToString().Replace(" xmlns=\"\"", ""));

                var matchingClients = XmppService.XmppMucsDictionary[name].Members
                    .Select(member => XmppService.Clients
                        .FirstOrDefault(cl => cl.Value.AccountId == member.AccountId))
                    .ToList();

                foreach (var matchingClient in matchingClients)
                {
                    socket.Send(new XElement(XNamespace.Get("jabber:client") + "presence",
                        new XAttribute("from", $"{name}@muc.prod.ol.epicgames.com/{matchingClient.Value.DisplayName}:{matchingClient.Value.AccountId}:{matchingClient.Value.Resource}"),
                        new XAttribute("to", matchingClient.Value.Jid),
                        new XElement(XNamespace.Get("http://jabber.org/protocol/muc#user") + "x",
                            new XElement("item",
                                new XAttribute("nick", $"{matchingClient.Value.DisplayName}:{matchingClient.Value.AccountId}:{matchingClient.Value.Resource}"),
                                new XAttribute("jid", matchingClient.Value.Jid),
                                new XAttribute("role", "participant"),
                                new XAttribute("affiliation", "none")
                            )
                        )
                    ).ToString().Replace(" xmlns=\"\"", ""));

                    if (client.AccountId == matchingClient.Value.AccountId) return;

                    client.Socket.Send(new XElement(XNamespace.Get("jabber:client") + "presence",
                        new XAttribute("from", $"{name}@muc.prod.ol.epicgames.com/{matchingClient.Value.DisplayName}:{matchingClient.Value.AccountId}:{matchingClient.Value.Resource}"),
                        new XAttribute("to", matchingClient.Value.Jid),
                        new XElement(XNamespace.Get("http://jabber.org/protocol/muc#user") + "x",
                            new XElement("item",
                                new XAttribute("nick", $"{matchingClient.Value.DisplayName}:{matchingClient.Value.AccountId}:{matchingClient.Value.Resource}"),
                                new XAttribute("jid", matchingClient.Value.Jid),
                                new XAttribute("role", "participant"),
                                new XAttribute("affiliation", "none")
                            )
                        )
                    ).ToString().Replace(" xmlns=\"\"", ""));

                    return;
                }
            }

            bool hasStatus = root.Elements().Any(e => e.Name.LocalName == "status");

            if (!hasStatus) return;

            var statusElement = root.Elements().Where(x => x.Name.LocalName == "status").First();
            if (statusElement == null) return;

            object status;
            Console.WriteLine(statusElement.Value.ToString());
            try
            {
                status = JsonConvert.DeserializeObject(statusElement.Value.ToString());
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to parse status: {ex.Message}");
                return;
            }

            bool isAway = root.Elements().ToList().FindIndex(x => x.Name.LocalName == "show") == 1 ? false : true;

            await UpdatePresenceForFriend.UpdateAsync(socket, status, false, isAway);
            await GetUserPresence.GetAsync(false, client.AccountId, client.AccountId);
        }
    }
}
