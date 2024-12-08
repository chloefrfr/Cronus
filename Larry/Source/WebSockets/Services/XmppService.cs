using Larry.Source.WebSockets.Models;
using System.Collections.Concurrent;

namespace Larry.Source.WebSockets.Services
{
    public static class XmppService
    {
        public static readonly ConcurrentDictionary<string, SocketClient> Clients = new();

        public static void AddClient(SocketClient client)
        {
            Clients.TryAdd(client.AccountId, client);
        }
    }
}
