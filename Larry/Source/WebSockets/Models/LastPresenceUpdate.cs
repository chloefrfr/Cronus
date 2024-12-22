namespace Larry.Source.WebSockets.Models
{
    public class LastPresenceUpdate
    {
        public bool Away { get; set; } = false;
        public object Status { get; set; } = null;
    }
}
