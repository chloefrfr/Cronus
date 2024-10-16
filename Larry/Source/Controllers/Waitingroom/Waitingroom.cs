using Microsoft.AspNetCore.Mvc;


namespace Larry.Source.Controllers.Waitingroom
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
