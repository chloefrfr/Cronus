using Larry.Source.Utilities;
using Larry.Source.WebSockets.Models;
using Serilog;
using System.Buffers;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using Fleck;
using System.Threading.Channels;
using System.Xml.Linq;
using Larry.Source.WebSockets.Roots;
using System.Collections.Concurrent;
using Larry.Source.WebSockets.Services;
using System.Runtime.CompilerServices;

namespace Larry.Source.WebSockets
{
    public class SocketServer
    {
        private static readonly ConcurrentDictionary<string, Action<IWebSocketConnection, SocketClient, XElement>> Handlers = new ConcurrentDictionary<string, Action<IWebSocketConnection, SocketClient, XElement>>
        {
            ["open"] = OpenHandler.Handle,
        };

        public async Task StartAsync(string[] args)
        {
            var socketServer = new WebSocketServer("ws://0.0.0.0:443");
            SocketClient client = null;
            socketServer.Start(server =>
            {
                server.OnOpen = () => OnOpen();
                server.OnMessage = async (message) => OnMessage(server, client, message);
                server.OnClose = () => OnClose();
            });
        }

        private void OnOpen()
        {
            Logger.Information("WebSocket connection established");
        }

        private void OnClose()
        {
            Logger.Information($"Socket connection closed.");
        }

        private void OnMessage(IWebSocketConnection socket, SocketClient client, string message)
        {
            var xElement = XElement.Parse(message);

            var rootName = xElement?.Name.LocalName;
            if (rootName == null)
            {
                Logger.Warning("XML document does not have a root element.");
                return;
            }

            Logger.Information($"Requested root '{rootName}'");

            if (Handlers.TryGetValue(rootName, out var handler))
            {
                try
                {
                    handler(socket, client, xElement);
                }
                catch (Exception ex)
                {
                    Logger.Error($"Error executing handler for root '{rootName}': {ex.Message}");
                }
            }
            else
            {
                Logger.Warning($"No handler found for root element: {rootName}");
            }
        }
    }
}
