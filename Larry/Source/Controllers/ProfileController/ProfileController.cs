using Larry.Source.Utilities;
using Larry.Source.Utilities.Managers;
using Microsoft.AspNetCore.Mvc;

namespace Larry.Source.Controllers.ProfileController
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

            var test = ProfileManager.GetProfileAsync(accountId);

            if (test == null)
            {
                Logger.Error($"Failed to get profile: {profileId}");
                return BadRequest(Errors.CreateError(400, Request.Path, "Failed to get profile.", timestamp));
            }

            return Ok();
        }
    }
}
