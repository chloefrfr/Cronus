using Larry.Source.Utilities;
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



            return Ok();

        }
    }
}
