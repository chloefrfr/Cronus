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
using System.Threading.Tasks;
using System.Linq;

namespace Larry.Source.Controllers.Profile
{
    [ApiController]
    [Route("fortnite/api/game/v2/profile")]
    public class ProfileController : Controller
    {
        [HttpPost("{accountId}/client/{operation}")]
        public async Task<IActionResult> ProfileHandler(string accountId, string operation)
        {
            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            string profileId = Request.Query["profileId"];
            int rvn = int.Parse(Request.Query["rvn"]);
            var userAgent = Request.Headers["User-Agent"].ToString();

            if (string.IsNullOrEmpty(operation))
                return BadRequest(Errors.CreateError(400, Request.Path, "Missing parameter 'operation'.", timestamp));
            if (string.IsNullOrEmpty(accountId) || string.IsNullOrEmpty(profileId))
                return BadRequest(Errors.CreateError(400, Request.Path, "Missing query parameters.", timestamp));
            if (string.IsNullOrEmpty(userAgent))
                return BadRequest(Errors.CreateError(400, Request.Path, "Missing header 'User-Agent'.", timestamp));

            var config = Config.GetConfig();
            var userRepository = new Repository<User>(config.ConnectionUrl);
            var profilesRepository = new Repository<Profiles>(config.ConnectionUrl);

            var userTask = userRepository.FindByAccountIdAsync(accountId);
            var profileTask = profilesRepository.FindByProfileIdAndAccountIdAsync(profileId, accountId);
            await Task.WhenAll(userTask, profileTask);

            var user = userTask.Result;
            var profile = profileTask.Result;

            if (user == null)
                return NotFound(Errors.CreateError(404, Request.Path, "Failed to find user.", timestamp));

            if (profile == null && (profileId == "common_public" || profileId == "creative"))
            {
                var profileChanges = new List<object>
                {
                    new
                    {
                        changeType = "fullProfileUpdate",
                        profile = new MCPProfile
                        {
                            created = timestamp,
                            updated = timestamp,
                            rvn = 0,
                            wipeNumber = 1,
                            accountId = accountId,
                            profileId = profileId,
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
                    ProfileId = profileId,
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
                { "QueryProfile", async () => Ok(await QueryProfile.Init(user.AccountId, profileId, userAgent)) },

                { "EquipBattleRoyaleCustomization", async () =>
                    await HandleRequestBody<EquipRequestBody>(body =>
                        EquipBattleRoyaleCustomization.Init(user.AccountId, profileId, body).AsTask()) },
                { "SetCosmeticLockerSlot", async () =>
                    await HandleRequestBody<SetLockerSlotRequestBody>(body =>
                        SetCosmeticLockerSlot.Init(user.AccountId, profileId, body).AsTask()) },

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
            if (operation == "ClientQuestLogin")
                return Ok(MCPResponses.Generate(profile, applyProfileChanges, profileId));
            return NotFound(Errors.CreateError(404, Request.Path, $"Operation '{operation}' not found.", timestamp));
        }

        private async Task<IActionResult> HandleRequestBody<T>(Func<T, Task<BaseResponse>> action)
        {
            var body = await Request.ReadFromJsonAsync<T>();
            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

            if (body == null)
                return BadRequest(Errors.CreateError(400, Request.Path, "Invalid body.", timestamp));

            var response = await action(body);
            return Ok(response);
        }
    }
}
