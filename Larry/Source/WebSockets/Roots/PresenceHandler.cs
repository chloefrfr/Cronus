using Fleck;
using Larry.Source.Utilities;
using Larry.Source.WebSockets.Models;
using Larry.Source.WebSockets.Services;
using System.Xml.Linq;

namespace Larry.Source.WebSockets.Roots
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


            }
        }
    }
}
