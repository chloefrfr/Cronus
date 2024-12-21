using Fleck;
using Larry.Source.Database.Entities;
using Larry.Source.Repositories;
using Larry.Source.Utilities;
using Larry.Source.WebSockets.Models;
using Larry.Source.WebSockets.Services;
using System.Xml.Linq;

namespace Larry.Source.WebSockets.Helpers.XMPP
{
    public class UpdatePresenceForFriend
    {
        public static async Task UpdateAsync(IWebSocketConnection socket, string status, bool offline, bool away)
        {
            var senderIndex = XmppService.Clients
                .Select((client, index) => new { Client = client, Index = index })
                .FirstOrDefault(x => x.Client.Value.Socket == socket)?.Index ?? -1;

            var sender = senderIndex != -1 ? XmppService.Clients.ElementAt(senderIndex).Value : null;
            if (sender == null) return;

            Config config = new Config();
            var friendRepository = new Repository<Friends>(config.ConnectionUrl);
            var friends = await friendRepository.FindFriendsByAccountIdAsync(sender.AccountId);

            if (friends == null) return;

            foreach (var friend in friends)
            {
                if (friend.Status == "ACCEPTED")
                {
                    var client = XmppService.Clients
                        .Select(kvp => kvp.Value)
                        .FirstOrDefault(c => c.AccountId == friend.AccountId);

                    if (client == null) continue;

                    var presenceType = offline ? "unavailable" : "available";

                    var xmlMessage = new XElement(XNamespace.Get("jabber:client") + "presence",
                        new XAttribute("to", client.Jid),
                        new XAttribute("xmlns", XNamespace.Get("jabber:client")),
                        new XAttribute("from", sender.Jid),
                        new XAttribute("type", presenceType)
                    );

                    if (sender.LastPresenceUpdate.Away)
                    {
                        xmlMessage.Add(
                            new XElement(XNamespace.Get("jabber:client") + "show", "away"),
                            new XElement(XNamespace.Get("jabber:client") + "status", sender.LastPresenceUpdate.Status)
                        );
                    }
                    else if (!string.IsNullOrEmpty(sender.LastPresenceUpdate.Status))
                    {
                        xmlMessage.Add(
                           new XElement(XNamespace.Get("jabber:client") + "status", sender.LastPresenceUpdate.Status)
                       );
                    }

                    client.Socket.Send(xmlMessage.ToString());
                }
                else
                {
                    Logger.Warning("Skill issue");
                }
            }
        }
    }
}
