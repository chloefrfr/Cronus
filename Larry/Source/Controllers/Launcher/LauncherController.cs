using Larry.Source.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Larry.Source.Controllers.Launcher
{
    [ApiController]
    [Route("/launcher/api/public/")]
    public class LauncherController : Controller
    {
        [HttpGet]
        [Route("distributionpoints")]
        public IActionResult GetDistributionPoints()
        {
            return Ok(new
            {
                distributions = new string[]
                {
                    $"http://127.0.0.1:{HttpContext.Request.Host.Port}",
                    "https://download.epicgames.com/",
                    "https://epicgames-download1.akamaized.net/",
                    "https://fastly-download.epicgames.com/"
                }
            });
        }

        [HttpGet]
        [Route("assets/{platform}/{catalogItemId}/{appName}")]
        public IActionResult GetLauncherAssets(string platform, string catalogItemId, string appName)
        {
            var Label = HttpContext.Request.Query["label"];
            Config config = Config.GetConfig();

            var random = new Random();
            var signature = Guid.NewGuid().ToString();

            return Ok(new
            {
                appName = appName,
                labelName = $"{Label}-{platform}",
                buildVersion = config.CurrentVersion,
                catalogItemId = catalogItemId,
                expires = DateTime.Parse("9999-12-31T23:59:59.999Z", null, System.Globalization.DateTimeStyles.RoundtripKind),
                items = new
                {
                    MANIFEST = new
                    {
                        signature = signature,
                        distribution = $"http://127.0.0.1:{HttpContext.Request.Host.Port}",
                        path = $"Builds/Fortnite/Content/CloudDir/Larry.manifest",
                        additionalDistributions = new string[] { }
                    }
                },
                assetId = appName
            });
        }
    }
}
