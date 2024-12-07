using Fleck;
using System.Net.WebSockets;

namespace Larry.Source.WebSockets.Models
{
    public class SocketClient
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public IWebSocketConnection Socket { get; set; }
        public bool IsLoggedIn { get; set; }
        public bool IsAuthenticated { get; set; }
        public string AccountId { get; set; }
        public string DisplayName { get; set; }
        public string Token { get; set; }
        public string Jid { get; set; }
        public string Resource { get; set; }
    }
}
