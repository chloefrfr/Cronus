using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using Larry.Source.Utilities;
using Larry.Source.Repositories;
using Larry.Source.Database.Entities;
using Larry.Source.Enums;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Larry.Source.Classes;
using Larry.Source.Interfaces;
using System.Text.Json;

namespace Larry.Source.Controllers.Lightswitch
{
    [ApiController]
    [Route("/lightswitch/api/service/")]
    public class Lightswitch : Controller
    {
        [HttpGet("bulk/status")]
        public async Task<IActionResult> BulkStatus()
        {
            string authorization = Request.Headers["Authorization"].ToString()?.Replace("bearer eg1~", "");
            string timestamp = DateTime.UtcNow.ToString("o");

            if (string.IsNullOrEmpty(authorization))
            {
                return BadRequest(Errors.CreateError((int)HttpStatusCode.BadRequest, Request.Path, "Invalid authorization header.", timestamp));
            }

            Config config = Config.GetConfig();


            string accountId = null;
            try
            {
                var decodedToken = TokenGenerator.DecodeToken(authorization, config.ClientSecret);

                if (decodedToken == null)
                {
                    return BadRequest(Errors.CreateError((int)HttpStatusCode.BadRequest, Request.Path, "Failed to decode token.", timestamp));
                }

                accountId = decodedToken["sub"] as string;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error: {ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, Errors.CreateError((int)HttpStatusCode.InternalServerError, Request.Path, $"Internal Server Error: {ex.Message}", timestamp));
            }

            Repository<User> userRepository = new Repository<User>(config.ConnectionUrl);
            User user = await userRepository.FindByAccountIdAsync(accountId);

            if (user == null)
            {
                return NotFound(Errors.CreateError((int)HttpStatusCode.NotFound, Request.Path, "Failed to find user.", timestamp));
            }

            if (user.Banned)
            {
                return Unauthorized(Errors.CreateError((int)HttpStatusCode.Unauthorized, Request.Path, $"User {user.Username} is banned.", timestamp));
            }

            List<ILightswitchData> response = new List<ILightswitchData>
            {
                new LightswitchData
                {
                    ServiceInstanceId = "fortnite",
                    Status = StatusEnum.Up,
                    Message = "Servers are UP!",
                    OverrideCatalogIds = new List<string>
                    {
                        "a7f138b2e51945ffbfdacc1af0541053"
                    },
                    AllowedActions = new List<ActionsEnum>
                    {
                        ActionsEnum.Play,
                        ActionsEnum.Download
                    },
                    Banned = user.Banned,
                    LauncherInfoDTO = new LauncherInfoDTO
                    {
                        AppName = "Fortnite",
                        CatalogItemId = "4fe75bbc5a674f4f9b356b5c90567da5",
                        Namespace = "fn"
                    }
                }
            };

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null, 
            };

            string json = JsonSerializer.Serialize(response, options);

            Console.WriteLine(json);

            return Ok(json);
        }
    }
}
