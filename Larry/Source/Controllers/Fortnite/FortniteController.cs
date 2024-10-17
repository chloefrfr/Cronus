using Microsoft.AspNetCore.Mvc;

namespace Larry.Source.Controllers.Fortnite
{
    [ApiController]
    [Route("/fortnite/api/game/")]
    public class FortniteController : Controller
    {
        [HttpPost("v2/tryPlayOnPlatform/account/{accountId}")]
        public IActionResult TryPlayOnPlatform(string accountId)
        {
            Response.ContentType = "text/plain";
            return Ok("true");
        }

        [HttpGet("v2/enabled_features")]
        public IActionResult EnabledFeatures()
        {
            return Ok(new object[] { });
        }

    }
}
