using Larry.Source.Database.Entities;
using Larry.Source.Repositories;
using Larry.Source.Utilities;
using Larry.Source.Utilities.Parsers;
using Microsoft.AspNetCore.Mvc;
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

                default:
                    Logger.Warning($"Missing Grant: {grantType}");
                    return BadRequest(Errors.CreateError((int)HttpStatusCode.BadRequest, Request.Path, "Invalid grant.", timestamp));
            }

            if (user == null)
            {
                return NotFound(Errors.CreateError((int)HttpStatusCode.NotFound, Request.Path, "Failed to find user.", timestamp));
            }

            Repository<Tokens> tokensRepository = new Repository<Tokens>(config.ConnectionUrl);

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
    }
}
