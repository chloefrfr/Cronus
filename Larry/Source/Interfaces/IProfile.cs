using Larry.Source.Classes.MCP;
using Larry.Source.Classes.Profile;
using System;
using System.Collections.Generic;

namespace Larry.Source.Interfaces
{
    public interface IProfile
    {
        string created { get; set; }
        string updated { get; set; }
        int rvn { get; set; }
        int wipeNumber { get; set; }
        string accountId { get; set; }
        string profileId { get; set; }
        string version { get; set; }
        StatsAttributes stats { get; set; }
        Dictionary<Guid, ItemDefinition> items { get; set; }
        int commandRevision { get; set; }
    }
}
