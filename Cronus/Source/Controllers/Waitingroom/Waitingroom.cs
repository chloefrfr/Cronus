using Microsoft.AspNetCore.Mvc;


namespace Cronus.Source.Controllers.Waitingroom
{
    [ApiController]
    public class Waitingroom : Controller
    {
        [HttpGet("/waitingroom/api/waitingroom")]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
