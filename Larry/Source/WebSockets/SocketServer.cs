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
using System.Net.Sockets;
using Larry.Source.WebSockets.Roots.XMPP;

namespace Larry.Source.WebSockets
{
    public class SocketServer
    {
        private SocketClient _client = null;

        private static readonly ConcurrentDictionary<string, Delegate> Handlers = new ConcurrentDictionary<string, Delegate>
        {
            ["open"] = new Action<IWebSocketConnection, SocketClient, XElement>(OpenHandler.Handle),
            ["auth"] = new Func<IWebSocketConnection, SocketClient, XElement, Task>(AuthHandler.HandleAsync),
            ["iq"] = new Func<IWebSocketConnection, SocketClient, XElement, Task>(IqHandler.HandleAsync),
            ["presence"] = new Func<IWebSocketConnection, SocketClient, XElement, Task>(PresenceHandler.HandleAsync)
        };

        /// <summary>
        /// Starts the WebSocket server and initializes the event handlers for connection management.
        /// </summary>
        public async Task StartAsync()
        {
            // TODO: Handle The Matchmaker and XMPP at once
            var socketServer = new WebSocketServer("ws://0.0.0.0:443");
            socketServer.Start(server =>
            {
                server.OnOpen = () => OnOpen(server);
                server.OnMessage = async (message) => await OnMessage(server , message);
                server.OnClose = () => OnClose();
            });
        }

        /// <summary>
        /// Called when a new WebSocket connection is opened.
        /// </summary>
        /// <param name="socket">The WebSocket connection instance.</param>
        private void OnOpen(IWebSocketConnection socket)
        {
            Logger.Information("WebSocket connection established");
            _client = new SocketClient
            {
                Socket = socket
            };
        }

        /// <summary>
        /// Called when a WebSocket connection is closed.
        /// </summary>  
        private void OnClose()
        {
            Logger.Information($"Socket connection closed.");
        }

        /// <summary>
        /// Handles incoming WebSocket messages, parses them, and dispatches them to the appropriate handler.
        /// </summary>
        /// <param name="socket">The WebSocket connection instance.</param>
        /// <param name="message">The message received from the client as a string.</param>
        private async Task OnMessage(IWebSocketConnection socket, string message)
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
                    if (handler is Func<IWebSocketConnection, SocketClient, XElement, Task> asyncHandler)
                    {
                        await asyncHandler(socket, _client, xElement);
                    }
                    else if (handler is Action<IWebSocketConnection, SocketClient, XElement> syncHandler)
                    {
                        syncHandler(socket, _client, xElement);
                    }
                    else
                    {
                        Logger.Error($"Handler for '{rootName}' has an unsupported type.");
                    }
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

            bool isValidConnection =
                !_client.IsLoggedIn &&
                _client.IsAuthenticated &&
                !string.IsNullOrEmpty(_client.AccountId) &&
                !string.IsNullOrEmpty(_client.DisplayName) &&
                !string.IsNullOrEmpty(_client.Jid) &&
                !string.IsNullOrEmpty(_client.Resource);

            if (isValidConnection)
            {
                XmppService.AddClient(_client);
                _client.IsLoggedIn = true;

                Logger.Information($"Successfully logged in as '{_client.DisplayName}'");
            }
        }
    }
}
