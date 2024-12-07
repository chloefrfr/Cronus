using Larry.Source.Utilities;
using System.Collections.Concurrent;
using System.Net;
using System.Net.WebSockets;

namespace Larry.Source.WebSockets
{
    public class SocketRouter
    {
        private readonly ConcurrentDictionary<string, WebSocket> _clients = new();
        //private readonly string _fileDirectory;

        //public SocketRouter(string fileDirectory)
        //{
        //    _fileDirectory = fileDirectory;
        //    Directory.CreateDirectory(_fileDirectory);
        //}

        public async Task StartAsync(string url, CancellationToken cancellationToken = default)
        {
            var listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();

            Logger.Information($"XMPP Server started at {url}");

            // TODO: Add TCP Support for versions below S4

            while (!cancellationToken.IsCancellationRequested)
            {
                var context = await listener.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    var webSocketContext = await context.AcceptWebSocketAsync(null);
                    string clientId = Guid.NewGuid().ToString();

                    _clients.TryAdd(clientId, webSocketContext.WebSocket);
                    Logger.Information($"Client connected: {clientId}");
                } else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
        }
    }
}
