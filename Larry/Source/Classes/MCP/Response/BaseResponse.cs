using Larry.Source.Classes.Profile;

namespace Larry.Source.Classes.MCP.Response
{
    public class BaseResponse
    {
        public int ProfileRevision { get; set; }
        public string ProfileId { get; set; }
        public int ProfileChangesBaseRevision { get; set; }
        public List<object> ProfileChanges { get; set; } = new List<object>();
        public int ProfileCommandRevision { get; set; }
        public string ServerTime { get; set; }
        public int ResponseVersion { get; set; }
    }
}
