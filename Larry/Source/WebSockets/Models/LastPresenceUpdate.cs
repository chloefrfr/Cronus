namespace Larry.Source.WebSockets.Models
{
    public class LastPresenceUpdate
    {
        public bool Away { get; set; } = false;
        public string Status { get; set; } = string.Empty;
    }
}
