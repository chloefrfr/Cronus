﻿using Larry.Source.WebSockets.Services;
using Newtonsoft.Json;
using System.Xml.Linq;

namespace Larry.Source.WebSockets.Helpers.XMPP
{
    public class GetUserPresence
    {
        public static async Task GetAsync(bool offline, string senderId, string receiverId)
        {
            var clients = XmppService.Clients.Values.ToList();
            var sender = clients.FirstOrDefault(c => c.AccountId == senderId);
            var receiver = clients.FirstOrDefault(c => c.AccountId == receiverId);

            if (sender == null || receiver == null) return;

            var presenceType = offline ? "unavailable" : "available";

            var xmlMessage = new XElement(XNamespace.Get("jabber:client") + "presence",
                new XAttribute("to", receiver.Jid),
                new XAttribute("xmlns", XNamespace.Get("jabber:client")),
                new XAttribute("from", sender.Jid),
                new XAttribute("type", presenceType)
            );

            if (sender.LastPresenceUpdate.Away)
            {
                xmlMessage.Add(
                    new XElement("show", "away"),
                    new XElement("status", JsonConvert.SerializeObject(sender.LastPresenceUpdate.Status))
                );
            }
            xmlMessage.Add(
                new XElement("status", JsonConvert.SerializeObject(sender.LastPresenceUpdate.Status))
            );

            receiver.Socket.Send(xmlMessage.ToString().Replace(" xmlns=\"\"", ""));
        }
    }
}
