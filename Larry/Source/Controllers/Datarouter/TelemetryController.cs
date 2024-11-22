using Microsoft.AspNetCore.Mvc;

namespace Larry.Source.Controllers.Datarouter
{
    [ApiController]
    [Route("/datarouter/api/v1/public/")]
    public class TelemetryController : Controller
    {
        [HttpPost]
        [Route("data")]
        public IActionResult GetTelemetry()
        {
            return Ok();
        }
    }
}
