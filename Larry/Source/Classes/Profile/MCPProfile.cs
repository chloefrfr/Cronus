using Larry.Source.Classes.MCP;
using Larry.Source.Interfaces;
using Newtonsoft.Json;

namespace Larry.Source.Classes.Profile
{
    public class MCPProfile : IProfile
    {
        public string created { get; set; }
        public string updated { get; set; }
        public int rvn { get; set; }
        public int wipeNumber { get; set; }
        public string accountId { get; set; }
        public string profileId { get; set; }
        public string version { get; set; }
        public StatsAttributes stats { get; set; }
        public Dictionary<Guid, ItemDefinition> items { get; set; }
        public int commandRevision { get; set; }
    }
}
