using Larry.Source.Classes.Cloud;
using Larry.Source.Classes.Cloud.files;
using Larry.Source.Utilities;
using Larry.Source.Utilities.Parsers;
using Microsoft.AspNetCore.Mvc;
using System.Reactive;
using System.Security.Cryptography;

namespace Larry.Source.Controllers.Cloudstorage
{
    [ApiController]
    [Route("/fortnite/api/cloudstorage/")]
    public class CloudstorageController : Controller
    {
        private readonly string _clientSettingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Larry", "ClientSettings");

        public CloudstorageController()
        {
            if (!Directory.Exists(_clientSettingsPath))
            {
                Directory.CreateDirectory(_clientSettingsPath);
            }
        }

        [HttpGet]
        [Route("system")]
        public async Task<IActionResult> GetCloudStorageFilesAsync()
        {
            var userAgent = Request.Headers["User-Agent"].ToString();
            var timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

            var seasonData = UserAgentParser.Parse(userAgent);
            if (seasonData == null)
            {
                Logger.Error($"Invalid User-Agent: {userAgent} at {timestamp}");
                return BadRequest(Errors.CreateError(400, Request.Path, "Invalid User-Agent", timestamp));
            }

            var fileContents = new Dictionary<string, string>
            {
                { "DefaultEngine.ini", Engine.GetDefaultEngine() },
                { "DefaultGame.ini", Game.GetDefaultGame(seasonData.Season) },
                { "DefaultRuntimeOptions.ini", RuntimeOptions.GetDefaultRuntimeOptions() }
            };

            var cloudStorage = new CloudStorage();
            foreach (var (file, content) in fileContents)
            {
                cloudStorage.Files.Add(new CloudStorageFile
                {
                    UniqueFilename = file,
                    Filename = file,
                    Hash = GenerateHash.Generate(content, HashAlgorithmName.SHA1),
                    Hash256 = GenerateHash.Generate(content, HashAlgorithmName.SHA256),
                    Length = content.Length,
                    ContentType = "application/octet-stream",
                    Uploaded = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    StorageType = "S3",
                    DoNotCache = false
                });
            }

            return Ok(cloudStorage.Files);
        }

        [HttpGet("system/{filename}")]
        public async Task<IActionResult> GetFileByNameAsync(string filename)
        {
            var timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

            var userAgent = Request.Headers["User-Agent"].ToString();
            var seasonData = UserAgentParser.Parse(userAgent);

            if (seasonData == null)
            {
                Logger.Error($"Invalid User-Agent: {userAgent} at {timestamp}");
                return BadRequest(Errors.CreateError(400, Request.Path, "Invalid User-Agent", timestamp));
            }

            switch (filename)
            {
                case "DefaultEngine.ini":
                    return Content(Engine.GetDefaultEngine(), "text/plain");

                case "DefaultGame.ini":
                    return Content(Game.GetDefaultGame(seasonData.Season), "text/plain");

                case "DefaultRuntimeOptions.ini":
                    return Content(RuntimeOptions.GetDefaultRuntimeOptions(), "text/plain");

                default:
                    return NotFound(Errors.CreateError(404, Request.Path, $"File '{filename}' not found.", timestamp));
            }
        }

        [HttpGet]
        [Route("user/{accountId}/{file}")]
        public async Task<IActionResult> GetClientSettingsFile(string accountId, string file)
        {
            string clientSettingsFile = Path.Combine(_clientSettingsPath, $"ClientSettings-{accountId}.Sav");
            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

            if (file != "ClientSettings.Sav" || !System.IO.File.Exists(clientSettingsFile))
            {
                return NotFound(Errors.CreateError(404, Request.Path, $"Sorry, we couldn't find a settings file with the filename {file} for the accountId {accountId}", timestamp));
            }

            try
            {
                byte[] data = await System.IO.File.ReadAllBytesAsync(clientSettingsFile);
                return File(data, "application/octet-stream");
            }
            catch (Exception ex)
            {
                return StatusCode(500, Errors.CreateError(500, Request.Path, ex.Message, timestamp));
            }
        }

        [HttpGet]
        [Route("user/{accountId}")]
        public async Task<IActionResult> GetClientSettingsMetadata(string accountId)
        {
            string clientSettingsFile = Path.Combine(_clientSettingsPath, $"ClientSettings-{accountId}.Sav");
            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

            if (System.IO.File.Exists(clientSettingsFile))
            {
                try
                {
                    byte[] fileContent = await System.IO.File.ReadAllBytesAsync(clientSettingsFile);
                    var fileInfo = new FileInfo(clientSettingsFile);
                    string sha1Hash = GenerateHash.GenerateHashByByte(fileContent, SHA1.Create());
                    string sha256Hash = GenerateHash.GenerateHashByByte(fileContent, SHA256.Create());

                    var response = new[]
                    {
                    new
                    {
                        uniqueFilename = "ClientSettings.Sav",
                        filename = "ClientSettings.Sav",
                        hash = sha1Hash,
                        hash256 = sha256Hash,
                        length = fileContent.Length,
                        contentType = "application/octet-stream",
                        uploaded = fileInfo.LastWriteTimeUtc,
                        storageType = "S3",
                        storageIds = new object(),
                        accountId,
                        doNotCache = false
                    }
                };

                    return Ok(response);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, Errors.CreateError(500, Request.Path, ex.Message, timestamp));
                }
            }

            return Ok(Array.Empty<object>());
        }

        [HttpPut]
        [Route("user/{accountId}/{file}")]
        public async Task<IActionResult> UploadClientSettingsFile(string accountId, string file)
        {
            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await Request.Body.CopyToAsync(memoryStream);
                    byte[] body = memoryStream.ToArray();

                    if (body.Length >= 400000)
                    {
                        return StatusCode(403, Errors.CreateError(403, Request.Path, "Raw body is larger than 400,000 bytes.", timestamp));
                    }

                    if (file != "ClientSettings.Sav")
                    {
                        return NotFound(Errors.CreateError(400, Request.Path, "The specified file was not found.", timestamp));
                    }

                    string clientSettingsFile = Path.Combine(_clientSettingsPath, $"ClientSettings-{accountId}.Sav");

                    await System.IO.File.WriteAllBytesAsync(clientSettingsFile, body);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, Errors.CreateError(500, Request.Path, ex.Message, timestamp));
            }
        }
    }
}
