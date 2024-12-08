using Fleck;
using Larry.Source.Database.Entities;
using Larry.Source.Repositories;
using Larry.Source.Utilities;
using Larry.Source.WebSockets.Models;
using System.Linq.Expressions;
using System.Text;
using System.Xml.Linq;
using System.Threading.Tasks;
using Larry.Source.WebSockets.Services;

namespace Larry.Source.WebSockets.Roots
{
    public class AuthHandler
    {
        public static async Task HandleAsync(IWebSocketConnection socket, SocketClient client, XElement root)
        {
            if (root == null || root.Value == null)
            {
                socket.Close();
                return;
            }

            byte[] decodedBytes = Convert.FromBase64String(root.Value);
            string decodedContent = Encoding.UTF8.GetString(decodedBytes);

            string[] authFields = decodedContent.Split('\0');
            if (authFields.Length < 2)
            {
                socket.Close();
                return;
            }

            string accountId = authFields[1];

            Config config = Config.GetConfig();
            var tokenRepository = new Repository<Tokens>(config.ConnectionUrl);
            var userRepository = new Repository<User>(config.ConnectionUrl);

            var accessToken = await tokenRepository.FindTokenByAccountIdAndTypeAsync(accountId, "accesstoken");

            if (XmppService.Clients.Any(c => c.Value.AccountId == accountId))
            {
                socket.Close();
                return;
            }

            var user = await userRepository.FindByAccountIdAsync(accountId);

            if (user == null || user.Banned)
            {
                socket.Send(CreateFailureResponse("not-authorized", "Password not verified"));
                return;
            }

            client.AccountId = user.AccountId;
            if (accessToken != null)
            {
                client.Token = accessToken.Token;
            }
            client.DisplayName = user.Username;
            client.IsAuthenticated = true;

            Logger.Information($"New XMPP Client logged in as {user.Username}");

            socket.Send(new XElement(
                XNamespace.Get("urn:ietf:params:xml:ns:xmpp-sasl") + "success"
            ).ToString());
        }

        private static string CreateSuccessResponse()
        {
            XNamespace ns = XNamespace.Get("urn:ietf:params:xml:ns:xmpp-sasl");

            XDocument doc = new XDocument(
                new XElement(ns + "success")
            );
            return doc.ToString();
        }

        private static string CreateFailureResponse(string condition, string message)
        {
            XNamespace ns = XNamespace.Get("urn:ietf:params:xml:ns:xmpp-sasl");

            XDocument doc = new XDocument(
                new XElement(ns + "failure",
                    new XElement(ns + condition),
                    new XElement(ns + "text",
                        new XAttribute(XNamespace.Xml + "lang", "eng"),
                        message
                    )
                )
            );
            return doc.ToString();
        }
    }
}
