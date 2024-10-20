using Larry.Source.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;

namespace Larry.Source.Controllers.Storefront
{
    [ApiController]
    [Route("/fortnite/api/storefront/v2/")]
    public class StorefrontController : Controller
    {
        [HttpGet("keychain")]
        public async Task<IActionResult> GetKeychain()
        {
            Response.ContentType = "application/json";

            var keychainFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Memory", "keychain.json");
            if (!System.IO.File.Exists(keychainFile))
            {
                return NotFound();
            }

            var json = System.IO.File.ReadAllText(keychainFile);
            return Content(json);
        }
    }
}
