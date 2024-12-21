using System.Xml.Linq;
using System.Threading.Tasks;
using Fleck;
using Larry.Source.WebSockets.Models;
using System.Net.WebSockets;

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
            SendOpenMessage(socket, client);
            SendFeaturesMessage(socket, client);
        }

        private static void SendOpenMessage(IWebSocketConnection socket, SocketClient client)
        {
            var openMessage = new XElement(
                XNamespace.Get("urn:ietf:params:xml:ns:xmpp-framing") + "open",
                new XAttribute("from", "prod.ol.epicgames.com"),
                new XAttribute("id", socket.ConnectionInfo.Id),
                new XAttribute("version", "1.0"),
                new XAttribute(XNamespace.Get("xml") + "lang", "en")
            ).ToString();

            socket.Send(openMessage.ToString());
        }

        private static void SendFeaturesMessage(IWebSocketConnection socket, SocketClient client)
        {
            if (client.IsAuthenticated)
            {
                socket.Send(new XElement(
                    XNamespace.Get("http://etherx.jabber.org/streams") + "features",
                    new XElement(XNamespace.Get("urn:xmpp:features:rosterver") + "ver"),
                    new XElement(XNamespace.Get("urn:ietf:params:xml:ns:xmpp-tls") + "starttls"),
                    new XElement(XNamespace.Get("urn:ietf:params:xml:ns:xmpp-bind") + "bind"),
                    new XElement(
                        XNamespace.Get("http://jabber.org/features/compress") + "compression",
                        new XElement("method", "zlib")
                    ),
                    new XElement(XNamespace.Get("urn:ietf:params:xml:ns:xmpp-session") + "session")
                ).ToString());
            }
            else
            {
                socket.Send(new XElement(
                    XNamespace.Get("http://etherx.jabber.org/streams") + "features",
                    new XElement(
                        XNamespace.Get("urn:ietf:params:xml:ns:xmpp-sasl") + "mechanisms",
                        new XElement("mechanism", "PLAIN")
                    ),
                    new XElement(XNamespace.Get("urn:xmpp:features:rosterver") + "ver"),
                    new XElement(XNamespace.Get("urn:ietf:params:xml:ns:xmpp-tls") + "starttls"),
                    new XElement(
                        XNamespace.Get("http://jabber.org/features/compress") + "compression",
                        new XElement("method", "zlib")
                    ),
                    new XElement(XNamespace.Get("http://jabber.org/features/iq-auth") + "auth")
                ).ToString());
            }
        }
    }
}
