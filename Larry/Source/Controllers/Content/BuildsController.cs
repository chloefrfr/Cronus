using Larry.Source.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Larry.Source.Controllers.Builds
{
    [ApiController]
    [Route("/Builds/Fortnite/Content/CloudDir/")]
    public class BuildsController : Controller
    {
        [HttpGet]
        [Route("{file}")]
        public IActionResult GetFile(string file)
        {
            Response.ContentType = "application/octet-stream";
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Memory", "Assets", file);
            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

            Logger.Information($"Path to file {filePath}");


            if (System.IO.File.Exists(filePath))
            {
                return PhysicalFile(filePath, "application/octet-stream");
            } 
            else
            {
                Logger.Warning($"File '{file}' not found.");
                return NotFound(Errors.CreateError(404, Request.Path, $"File '{file}' not found.", timestamp));
            }
        }

        [HttpGet]
        [Route("{namespace}/{file}")]
        public IActionResult AlsoGetTheFile(string file)
        {
            Response.ContentType = "application/octet-stream";
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Memory", "Assets", file);
            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

            Logger.Information($"Path to file {filePath}");


            if (System.IO.File.Exists(filePath))
            {
                return PhysicalFile(filePath, "application/octet-stream");
            }
            else
            {
                Logger.Warning($"File '{file}' not found.");
                return NotFound(Errors.CreateError(404, Request.Path, $"File '{file}' not found.", timestamp));
            }
        }
    }
}
