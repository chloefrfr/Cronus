using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Jose;

namespace Larry.Source.Utilities
{
    public class TokenGenerator
    {
        /// <summary>
        /// Generates a JWT using the provided client ID and client secret.
        /// The token is signed using HMAC SHA-256.
        /// </summary>
        /// <param name="clientId">The client ID used for identification.</param>
        /// <param name="clientSecret">The client secret used for signing the token.</param>
        /// <returns>A string representing the generated JWT.</returns>
        public static string GenerateToken(string clientId, string clientSecret)
        {
            byte[] randomBytes128 = new byte[128];
            byte[] randomBytes32 = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes128);
                rng.GetBytes(randomBytes32);
            }

            string p = Convert.ToBase64String(randomBytes128);
            string jti = BitConverter.ToString(randomBytes32).Replace("-", "").ToLowerInvariant();
            var now = DateTime.UtcNow;
            var expirationTime = now.AddMinutes(240);

            var payload = new Dictionary<string, object>
            {
                { "p", p },
                { "clsvc", "fortnite" },
                { "t", "s" },
                { "mver", "false" },
                { "clid", clientId },
                { "ic", "true" },
                { "exp", new DateTimeOffset(expirationTime).ToUnixTimeSeconds() },
                { "am", "client_credentials" },
                { "iat", new DateTimeOffset(now).ToUnixTimeSeconds() },
                { "jti", jti },
                { "creation_date", now.ToString("o") },
                { "expires_in", "1" },
            };

            var token = JWT.Encode(payload, Encoding.UTF8.GetBytes(clientSecret), JwsAlgorithm.HS256);
            return token;
        }

        /// <summary>
        /// Decodes a JWT token and returns the claims as a dictionary.
        /// </summary>
        /// <param name="token">The JWT token to decode.</param>
        /// <param name="clientSecret">The client secret used for signing the token.</param>
        /// <returns>A dictionary containing the claims of the decoded token, or null if decoding fails.</returns>
        public static IDictionary<string, object> DecodeToken(string token, string clientSecret)
        {
            try
            {
                var payload = JWT.Decode<IDictionary<string, object>>(token, Encoding.UTF8.GetBytes(clientSecret), JwsAlgorithm.HS256);
                return payload;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error decoding token: {ex.Message}");
                return null;
            }
        }
    }
}
