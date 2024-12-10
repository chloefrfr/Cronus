using Larry.Source.WebSockets.Models;
using System.Collections.Concurrent;

namespace Larry.Source.WebSockets.Services
{
    public static class XmppService
    {
        /// <summary>
        /// A thread-safe dictionary that holds all connected WebSocket clients, mapped by their account ID.
        /// </summary>
        public static readonly ConcurrentDictionary<string, SocketClient> Clients = new();
        /// <summary>
        /// A dictionary that holds all XMPP MUC information.
        /// </summary>
        public static Dictionary<string, MUCInfo> XmppMucsDictionary { get; } = new Dictionary<string, MUCInfo>();
        /// <summary>
        /// Gets or sets the collection of joined MUCs.
        /// </summary>
        public static List<string> JoinedMUCs { get; } = new List<string>();

        /// <summary>
        /// Adds a new client to the Clients dictionary. If the account ID already exists, the client will not be added.
        /// </summary>
        /// <param name="client">The client to be added to the service.</param>
        public static void AddClient(SocketClient client)
        {
            Clients.TryAdd(client.AccountId, client);
        }
    }
}
