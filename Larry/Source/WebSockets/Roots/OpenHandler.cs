using System.Xml.Linq;
using System.Threading.Tasks;
using Fleck;
using Larry.Source.WebSockets.Models;

namespace Larry.Source.WebSockets.Roots
{
    public class OpenHandler
    {
        private static readonly XNamespace JabberNamespace = "urn:ietf:params:xml:ns:xmpp-framing";
        private static readonly XNamespace NsStream = "http://etherx.jabber.org/streams";
        private static readonly XNamespace NsJabber = "jabber:client";
        private static readonly XNamespace NsSasl = "urn:ietf:params:xml:ns:xmpp-sasl";
        private static readonly XNamespace NsRosterVer = "urn:xmpp:features:rosterver";
        private static readonly XNamespace NsTls = "urn:ietf:params:xml:ns:xmpp-tls";
        private static readonly XNamespace NsCompress = "http://jabber.org/features/compress";
        private static readonly XNamespace NsSession = "urn:ietf:params:xml:ns:xmpp-session";
        private static readonly XNamespace NsAuth = "http://jabber.org/features/iq-auth";

        public static void Handle(IWebSocketConnection socket, SocketClient client, XElement root)
        {
            client = new SocketClient
            {
                Socket = socket,
                IsAuthenticated = false,
                Jid = string.Empty
            };

            SendOpenMessage(socket, client);
            SendFeaturesMessage(socket);
        }

        private static void SendOpenMessage(IWebSocketConnection socket, SocketClient client)
        {
            var openMessage = new XElement(JabberNamespace + "open",
                new XAttribute("xmlns", JabberNamespace.NamespaceName),
                new XAttribute("from", "prod.ol.epicgames.com"),
                new XAttribute("id", client.Socket.ConnectionInfo.Id),
                new XAttribute("version", "1.0"),
                new XAttribute(XNamespace.Xml + "lang", "en"));

            socket.Send(openMessage.ToString());
        }

        private static void SendFeaturesMessage(IWebSocketConnection socket)
        {
            var featuresMessage = new XElement(NsStream + "features",
                new XElement(NsSasl + "mechanisms",
                    new XElement("mechanism", "PLAIN")),
                new XElement(NsRosterVer + "ver"),
                new XElement(NsTls + "starttls"),
                new XElement(NsCompress + "compression",
                    new XElement("method", "zlib")),
                new XElement(NsAuth + "auth"));

            socket.Send(featuresMessage.ToString());
        }
    }
}
