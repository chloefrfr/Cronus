﻿using Microsoft.AspNetCore.Mvc;

namespace Larry.Source.Controllers.Version
{
    [ApiController]
    [Route("/fortnite/api/v2/")]
    public class VersionController : Controller
    {
        [HttpGet("versioncheck")]
        public IActionResult check()
        {
            Response.ContentType = "application/json";

            return Ok(new
            {
                type = "NO_UPDATE"
            });
        }

        [HttpGet("versioncheck/{version}")]
        public IActionResult checkByVersion(string version)
        {
            Response.ContentType = "application/json";
            return Ok(new
            {
                type = "NO_UPDATE"
            });
        }
    }
}
