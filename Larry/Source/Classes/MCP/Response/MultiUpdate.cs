using Larry.Source.Classes.Profile;

namespace Larry.Source.Classes.MCP.Response
{
    public class MultiUpdate
    {
        public int ProfileRevision { get; set; }
        public ProfileIds ProfileId { get; set; }
        public int ProfileChangesBaseRevision { get; set; }
        public List<object> ProfileChanges { get; set; } = new List<object>();
        public int ProfileCommandRevision { get; set; }
    }
}
