using Fleck;
using Larry.Source.Utilities;
using Larry.Source.WebSockets.Models;
using System;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Larry.Source.WebSockets.Roots
{
    public class IqHandler
    {
        public static async Task HandleAsync(IWebSocketConnection socket, SocketClient client, XElement root)
        {
            var AttributeId = root.Attribute("id");

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
