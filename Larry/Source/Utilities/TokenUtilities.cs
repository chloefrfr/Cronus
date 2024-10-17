using Larry.Source.Database.Entities;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Jose;
using Newtonsoft.Json;
using Larry.Source.Repositories;

namespace Larry.Source.Utilities
{
    public static class TokenUtilities
    {
        /// <summary>
        /// Creates a JWT token.
        /// </summary>
        /// <param name="clientId">Client ID</param>
        /// <param name="grantType">Grant Type</param>
        /// <param name="user">User object</param>
        /// <param name="type">Token type: "access" or "refresh"</param>
        /// <returns>JWT token as a string</returns>
        private static async Task<string> CreateToken(string clientId, string grantType, User user, string type)
        {
            Config config = Config.GetConfig();

            var payload = new
            {
                App = "fortnite",
                Sub = user.AccountId,
                Dvid = new Random().Next(1, 1000000000),
                Mver = false,
                Clid = clientId,
                Dn = user.Username,
                Am = type == "access" ? grantType : "refresh",
                P = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                Iai = user.AccountId,
                Sec = 1,
                Clsvc = "fortnite",
                T = "s",
                Ic = true,
                Jti = Guid.NewGuid().ToString(),
                CreationDate = DateTime.UtcNow.ToString("o"),
                ExpiresIn = type == "access" ? 4 * 3600 : 14 * 24 * 3600
            };

            var token = JWT.Encode(payload, Encoding.UTF8.GetBytes(config.ClientSecret), JwsAlgorithm.HS256);

            Repository<Tokens> tokensRepository = new Repository<Tokens>(config.ConnectionUrl);

            await tokensRepository.SaveAsync(new Tokens
            {
                Type = type + "token",
                ClientId = clientId,
                GrantType = grantType,
                AccountId = user.AccountId,
                Token = token
            });

            return token;
        }

        /// <summary>
        /// Creates an access token.
        /// </summary>
        /// <param name="clientId">Client ID</param>
        /// <param name="grantType">Grant Type</param>
        /// <param name="user">User object</param>
        /// <returns>Access token as a string</returns>
        public static async Task<string> CreateAccessToken(string clientId, string grantType, User user)
        {
            return await CreateToken(clientId, grantType, user, "access");
        }

        /// <summary>
        /// Creates a refresh token.
        /// </summary>
        /// <param name="clientId">Client ID</param>
        /// <param name="user">User object</param>
        /// <returns>Refresh token as a string</returns>
        public static async Task<string> CreateRefreshToken(string clientId, User user)
        {
            return await CreateToken(clientId, "", user, "refresh");
        }
    }
}
