using Larry.Source.Classes.MCP.RequestBody;
using Larry.Source.Classes.MCP.Response;
using Larry.Source.Classes.Profile;
using Larry.Source.Controllers.Profile.Operations;
using Larry.Source.Database.Entities;
using Larry.Source.Repositories;
using Larry.Source.Responses;
using Larry.Source.Utilities;
using Larry.Source.Utilities.Managers;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Reactive;

namespace Larry.Source.Controllers.Profile
{
    [ApiController]
    [Route("fortnite/api/game/v2/profile")]
    public class ProfileController : Controller
    {
        [HttpPost("{accountId}/client/{operation}")]
        public async Task<IActionResult> ProfileHandler(string accountId, string operation)
        {
            Logger.Information($"Operation => {operation}");
            string timestamp = DateTime.UtcNow.ToString("o");
            string profileId = Request.Query["profileId"];
            int rvn = int.Parse(Request.Query["rvn"]);

            var UserAgent = Request.Headers["User-Agent"];
            
            if (operation == null)
            {
                return BadRequest(Errors.CreateError(400, Request.Path, "Missing parameter 'operation'.", timestamp));
            }

            if (accountId == null || rvn == null || profileId == null)
            {
                return BadRequest(Errors.CreateError(400, Request.Path, "Missing query parameters.", timestamp));
            }

            if (UserAgent.ToString() == null)
            {
                return BadRequest(Errors.CreateError(400, Request.Path, "Missing heaer 'User-Agent'.", timestamp));
            }

            Config config = Config.GetConfig();

            Repository<User> userRepository = new Repository<User>(config.ConnectionUrl);
            Repository<Profiles> profilesRepository = new Repository<Profiles>(config.ConnectionUrl);


            var user = await userRepository.FindByAccountIdAsync(accountId);

            if (user == null)
            {
                return NotFound(Errors.CreateError(404, Request.Path, "Failed to find user.", timestamp));
            }

            var profile = await profilesRepository.FindByProfileIdAndAccountIdAsync(profileId, user.AccountId);


            if (profile == null && profileId == "common_public")
            {
                var profileChanges = new List<object>
                {
                    new
                    {
                        changeType = "fullProfileUpdate",
                        profile = new MCPProfile
                        {
                            created = DateTime.UtcNow.ToString("o"),
                            updated = DateTime.UtcNow.ToString("o"),
                            rvn = 0,
                            wipeNumber = 1,
                            accountId = accountId,
                            profileId = "common_public",
                            version = "no_version",
                            stats = new StatsAttributes(),
                            items = new Dictionary<string, Classes.MCP.ItemDefinition>(),
                            commandRevision = 0
                        }
                    }
                };

                return Ok(MCPResponses.Generate(new Profiles
                {
                    AccountId = accountId,
                    ProfileId = "common_public",
                    Revision = 0
                }, profileChanges, profileId));
            } else if (profile == null && profileId == "creative")
            {
                var profileChanges = new List<object>
                {
                    new
                    {
                        changeType = "fullProfileUpdate",
                        profile = new MCPProfile
                        {
                            created = DateTime.UtcNow.ToString("o"),
                            updated = DateTime.UtcNow.ToString("o"),
                            rvn = 0,
                            wipeNumber = 1,
                            accountId = accountId,
                            profileId = "common_public",
                            version = "no_version",
                            stats = new StatsAttributes(),
                            items = new Dictionary<string, Classes.MCP.ItemDefinition>(),
                            commandRevision = 0
                        }
                    }
                };

                return Ok(MCPResponses.Generate(new Profiles
                {
                    AccountId = accountId,
                    ProfileId = "creative",
                    Revision = 0
                }, profileChanges, profileId));
            }


            if (profile == null)
            {
                Logger.Warning($"Missing profile '{profileId}'");
                return NotFound(Errors.CreateError(404, Request.Path, "Failed to find profile.", timestamp));
            }

            List<object> applyProfileChanges = new List<object>();

            var operations = new Dictionary<string, Func<Task<IActionResult>>>
            {
                { "QueryProfile", async () => Ok(await QueryProfile.Init(user.AccountId, profileId)) },

                { "EquipBattleRoyaleCustomization", async () =>
                    await HandleRequestBody<EquipRequestBody>(body =>
                        EquipBattleRoyaleCustomization.Init(user.AccountId, profileId, body).AsTask()) },

                { "MarkItemSeen", async () =>
                    await HandleRequestBody<MITSRequestBody>(body =>
                        MarkItemSeen.Init(user.AccountId, profileId, body)) },

                { "SetMtxPlatform", async () =>
                    await HandleRequestBody<MtxPlatformBody>(body =>
                        SetMtxPlatform.Init(user.AccountId, profileId, body)) }
            };

            if (operations.TryGetValue(operation, out var func))
            {
                return await func();
            }

            Logger.Warning($"Missing operation: {operation}");
            var response = MCPResponses.Generate(profile, applyProfileChanges, profileId);
            return Ok(response);
        }

        async Task<IActionResult> HandleRequestBody<T>(Func<T, Task<BaseResponse>> action)
        {
            var body = await Request.ReadFromJsonAsync<T>();
            var timestamp = DateTime.UtcNow.ToString("o");
            if (body == null)
            {
                return BadRequest(Errors.CreateError(400, Request.Path, "Invalid body.", timestamp));
            }
            return Ok(await action(body));
        }
    }
}
