using Larry.Source.Database.Entities;
using Larry.Source.Repositories;
using Larry.Source.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Larry.Source.Controllers.Account
{
    [ApiController]
    [Route("/account/api/public/")]
    public class AccountController : Controller
    {
        [HttpGet("account/{accountId}")]
        public async Task<IActionResult> ITellYou(string accountId)
        {
            Response.ContentType = "application/json";
            Config config = Config.GetConfig();
            var userRepository = new Repository<User>(config.ConnectionUrl);

            var user = await userRepository.FindByAccountIdAsync(accountId);
            var timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

            if (user == null)
            {
                return NotFound(Errors.CreateError((int)HttpStatusCode.NotFound, HttpContext.Request.Path, "User not found.", timestamp));
            }

            if (user.Banned)
            {
                return Unauthorized(Errors.CreateError((int)HttpStatusCode.Unauthorized, HttpContext.Request.Path, $"User '{user.Username} is banned.'", timestamp));
            }

            return Ok(new
            {
                id = user.AccountId,
                displayName = user.Username,
                name = user.Username,
                failedLoginAttempts = 0,
                lastLogin = timestamp,
                numberOfDisplayNameChanges = 0,
                ageGroup = "UNKNOWN",
                headless = false,
                country = "US",
                lastName = "",
                links = new object { },
                preferredLanguage = "en",
                canUpdateDisplayName = false,
                tfaEnabled = true,
                emailVerified = true,
                minorVerified = true,
                minorExpected = true,
                minorStatus = "UNKNOWN"
            });
        }

        [HttpGet("account")]
        public async Task<IActionResult> Account()
        {
            Response.ContentType = "application/json";
            string accountIdQuery = Request.Query["accountId"];
            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

            if (accountIdQuery == null)
            {
                return BadRequest(Errors.CreateError((int)HttpStatusCode.BadRequest, HttpContext.Request.Path, "Query Parameter 'accountId' is missing.", timestamp));
            }

            Config config = Config.GetConfig();
            var userRepository = new Repository<User>(config.ConnectionUrl);

            List<object> response = new List<object>();

            if (accountIdQuery.Contains(","))
            {
                string[] accountIds = accountIdQuery.Split(",");

                foreach (var accountId in accountIds)
                {
                    var user = await userRepository.FindByAccountIdAsync(accountId);

                    if (user == null)
                    {
                        return NotFound(Errors.CreateError((int)HttpStatusCode.NotFound, HttpContext.Request.Path, "Failed to find user.", timestamp));
                    }

                    response.Add(new
                    {
                        id = user.AccountId,
                        displayName = user.Username,
                        externalAuth = new object { }
                    });
                }
            } else
            {
                var user = await userRepository.FindByAccountIdAsync(accountIdQuery);

                if (user == null)
                {
                    return NotFound(Errors.CreateError((int)HttpStatusCode.NotFound, HttpContext.Request.Path, "Failed to find user.", timestamp));
                }

                response.Add(new
                {
                    id = user.AccountId,
                    displayName = user.Username,
                    cabinedMode = false,
                    externalAuth = new object { }
                });
            }

            return Ok(response);
        }

        [HttpGet("displayName/{username}")]
        public async Task<IActionResult> FindByDisplayName(string username)
        {
            Response.ContentType = "application/json";
            var timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

            Config config = Config.GetConfig();
            var userRepository = new Repository<User>(config.ConnectionUrl);

            var user = await userRepository.FindByUsernameAsync(username);

            if (user == null)
            {
                return NotFound(Errors.CreateError((int)HttpStatusCode.NotFound, HttpContext.Request.Path, "Failed to find user.", timestamp));
            }

            return Ok(new
            {
                id = user.AccountId,
                displayName = user.Username,
                externalAuths = new object { }
            });
        }
    }
}   
