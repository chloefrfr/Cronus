using Larry.Source.Database.Entities;
using Larry.Source.Repositories;
using Larry.Source.Utilities;
using Larry.Source.Utilities.Parsers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Larry.Source.Controllers.OAuth
{
    [ApiController]
    [Route("/account/api/oauth/")]
    public class OAuthController : Controller
    {
        [HttpPost("token")]
        public async Task<IActionResult> AuthToken()
        {
            var tokenHeader = Request.Headers["Authorization"].ToString();
            var timestamp = DateTime.UtcNow.ToString("o"); 
            var userAgent = Request.Headers["User-Agent"].ToString();

            if (string.IsNullOrEmpty(tokenHeader))
            {
                return BadRequest(Errors.CreateError((int)HttpStatusCode.BadRequest, Request.Path, "Invalid Headers.", timestamp));
            }

            var token = tokenHeader.Split(" ");

            if (token.Length != 2)
            {
                return BadRequest(Errors.CreateError((int)HttpStatusCode.BadRequest, Request.Path, "Invalid base64", timestamp));
            }

            Dictionary<string, string> body;

            var bodyParser = new BodyParser();

            try
            {
                body = await bodyParser.Parse(Request);
            }
            catch (Exception)
            {
                return BadRequest(Errors.CreateError((int)HttpStatusCode.BadRequest, Request.Path, "Invalid body.", timestamp));
            }

            Config config = Config.GetConfig();
            Repository<User> userRepository = new Repository<User>(config.ConnectionUrl);

            string grantType = body.ContainsKey("grant_type") ? body["grant_type"] : null;

            string clientId = Encoding.UTF8.GetString(Convert.FromBase64String(token[1])).Split(':')[0];

            if (clientId == null)
            {
                return BadRequest(Errors.CreateError((int)HttpStatusCode.BadRequest, Request.Path, "Invalid clientId", timestamp));
            }

            Repository<Tokens> tokensRepository = new Repository<Tokens>(config.ConnectionUrl);

            User user = null;

            switch (grantType)
            {
                case "password":
                    var username = body["username"];
                    var password = body["password"];

                    if (password == null || username == null)
                    {
                        return NotFound(Errors.CreateError((int)HttpStatusCode.NotFound, Request.Path, "Username or Password is missing.", timestamp));
                    }

                    user = await userRepository.FindByEmailAsync(username);

                    if (user == null)
                    {
                        return NotFound(Errors.CreateError((int)HttpStatusCode.NotFound, Request.Path, "Failed to find user.", timestamp));
                    }

                    if (user.Banned)
                    {
                        return Unauthorized(Errors.CreateError((int)HttpStatusCode.Unauthorized, HttpContext.Request.Path, $"User '{user.Username} is banned.'", timestamp));
                    }

                    var verifiedPassword = PasswordHasher.VerifyPassword(password, user.Password);
                    if (!verifiedPassword)
                    {
                        return NotFound(Errors.CreateError((int)HttpStatusCode.NotFound, Request.Path, "Invalid account credentials.", timestamp));
                    }

                    break;

                case "client_credentials":
                    string access_token = TokenGenerator.GenerateToken(config.ClientId, config.ClientSecret);

                    return Ok(new
                    {
                        access_token = $"eg1~{access_token}",
                        expires_in = 3600,
                        expires_at = DateTime.UtcNow.AddHours(1).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        token_type = "bearer",
                        client_id = clientId,
                        internal_client = true,
                        client_service = "fortnite"
                    });
                case "refresh_token":
                    string refresh_token = body["refresh_token"]?.ToString(); 

                    if (refresh_token == null)
                    {
                        return NotFound(Errors.CreateError((int)HttpStatusCode.NotFound, Request.Path, "Failed to find 'refresh_token'.", timestamp));
                    }

                    refresh_token = refresh_token.Replace("$", "").Replace("eg1~", "");

                    var validToken = await tokensRepository.FindByTokenAndTypeAsync(refresh_token, "refreshtoken");

                    if (validToken == null)
                    {
                        Logger.Error("Failed to find token.");
                        return NotFound(Errors.CreateError((int)HttpStatusCode.NotFound, Request.Path, "Failed to find token.", timestamp));
                    }

                    user = await userRepository.FindByAccountIdAsync(validToken.AccountId);
                    if (user == null)
                    {
                        return NotFound(Errors.CreateError((int)HttpStatusCode.NotFound, Request.Path, "Failed to find user.", timestamp));
                    }

                    if (user.Banned)
                    {
                        return Unauthorized(Errors.CreateError((int)HttpStatusCode.Unauthorized, HttpContext.Request.Path, $"User '{user.Username} is banned.'", timestamp));
                    }

                    access_token = await TokenUtilities.CreateAccessToken(clientId, grantType, user);

                    return Ok(new
                    {
                        access_token = access_token,
                        expires_in = 3600,
                        expires_at = DateTime.UtcNow.AddMonths(1).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        token_type = "bearer",
                        refresh_token = body["refresh_token"],
                        refresh_expires = 86400,
                        refresh_expires_at = DateTime.UtcNow.AddMonths(1).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        account_id = user.AccountId,
                        client_id = clientId,
                        internal_client = true,
                        client_service = "fortnite",
                        diplayName = user.Username,
                        app = "fortnite",
                        in_app_id = user.AccountId,
                        device_id = Request.Headers["X-Epic-Device-Id"],
                    });
                
                default:
                    Logger.Warning($"Missing Grant: {grantType}");
                    return BadRequest(Errors.CreateError((int)HttpStatusCode.BadRequest, Request.Path, "Invalid grant.", timestamp));
            }

            if (user == null)
            {
                return NotFound(Errors.CreateError((int)HttpStatusCode.NotFound, Request.Path, "Failed to find user.", timestamp));
            }


            await tokensRepository.DeleteByTypeAsync(user.AccountId, "accesstoken");
            await tokensRepository.DeleteByTypeAsync(user.AccountId, "refreshtoken");

            string accessToken = await TokenUtilities.CreateAccessToken(clientId, grantType, user);
            string refreshToken = await TokenUtilities.CreateRefreshToken(clientId, user);

            return Ok(new
            {
                access_token = $"eg1~{accessToken}",
                expires_in = 3600,
                expires_at = DateTime.UtcNow.AddHours(1).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                token_type = "bearer",
                account_id = user.AccountId,
                client_id = clientId,
                internal_client = true,
                client_service = "fortnite",
                refresh_token = $"eg1~${ refreshToken}",
                refresh_expires = 86400,
                refresh_expires_at = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                displayName = user.Username,
                app = "fortnite",
                in_app_id = user.AccountId,
                device_id = Request.Headers["X-Epic-Device-Id"],
            });
        }

        [HttpGet("verify")]
        public async Task<IActionResult> VerifyToken()
        {
            string authorizationHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            string timestamp = DateTime.UtcNow.ToString("o");

            if (string.IsNullOrEmpty(authorizationHeader))
            {
                return BadRequest(Errors.CreateError(400, Request.Path, "Authorization header missing.", timestamp));
            }

            const string bearerPrefix = "Bearer ";
            if (!authorizationHeader.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(Errors.CreateError(400, Request.Path, "Invalid token format.", timestamp));
            }

            string token = authorizationHeader.Substring(bearerPrefix.Length).Trim();
            string accessToken = token.Replace("eg1~", string.Empty);

            if (string.IsNullOrEmpty(accessToken))
            {
                return BadRequest(Errors.CreateError(400, Request.Path, "Invalid token.", timestamp));
            }

            Config config = Config.GetConfig();

            var decodedToken = TokenGenerator.DecodeToken(accessToken, config.ClientSecret);
            if (decodedToken == null)
            {
                return BadRequest(Errors.CreateError(400, Request.Path, "Invalid token.", timestamp));
            }

            var accountId = decodedToken.TryGetValue("sub", out var sub) ? sub.ToString() : null;
            if (string.IsNullOrEmpty(accountId))
            {
                return NotFound(Errors.CreateError(404, Request.Path, "Failed to find user.", timestamp));
            }

            Repository<User> userRepository = new Repository<User>(config.ConnectionUrl);
            var user = await userRepository.FindByAccountIdAsync(accountId);
            if (user == null)
            {
                return NotFound(Errors.CreateError(404, Request.Path, "Failed to find user.", timestamp));
            }

            if (!decodedToken.TryGetValue("creationDate", out var creationDateValue) || !DateTime.TryParse(creationDateValue.ToString(), out var creationDate))
            {
                Logger.Error("Invalid or missing creationDate in token.");
                return BadRequest(Errors.CreateError(400, Request.Path, "Invalid or missing creationDate in token.", timestamp));
            }

            if (!decodedToken.TryGetValue("expiresIn", out var expiresInValue) || !int.TryParse(expiresInValue.ToString(), out var expiresIn))
            {
                Logger.Error("Invalid or missing expiresIn in token.");
                return BadRequest(Errors.CreateError(400, Request.Path, "Invalid or missing expiresIn in token.", timestamp));
            }

            var creationDateUnix = ((DateTimeOffset)creationDate).ToUnixTimeSeconds();
            var expiry = creationDate.AddHours(expiresIn);

            return Ok(new
            {
                token,
                session_id = decodedToken["jti"].ToString(),
                token_type = "bearer",
                client_id = decodedToken["clid"].ToString(),
                internal_client = true,
                client_service = "fortnite",
                account_id = user.AccountId,
                expires_in = (int)(expiry - DateTime.UtcNow).TotalSeconds,
                expires_at = expiry.AddHours(8),
                auth_method = decodedToken["am"].ToString(),
                display_name = user.Username,
                app = "fortnite",
                in_app_id = user.AccountId,
                device_id = decodedToken["dvid"].ToString(),
            });
        }
    }
}
