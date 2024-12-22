using Larry.Source.WebSockets.Services;
using Microsoft.AspNetCore.Mvc;

namespace Larry.Source.Controllers.Services
{
    [ApiController]
    public class GlobalClientsController : Controller
    {
        [HttpGet("/api/v1/xmpp/clients")]
        public IActionResult Get()
        {
            var clients = XmppService.Clients;

            return Ok(new
            {
                clients = clients.Select(client => new
                {
                    key = client.Key,              
                    value = new
                    {
                        client.Value.DisplayName,      
                        client.Value.LastPresenceUpdate,       
                        client.Value.AccountId,  
                        client.Value.IsAuthenticated,
                        client.Value.IsLoggedIn
                    }
                }),
                count = clients.Count,             
                timestamp = DateTime.UtcNow       
            });
        }
    }
}
