using Newtonsoft.Json;

namespace Larry.Source.Classes.MCP
{
    public class PlacedBuildings
    {
        [JsonProperty("buildingTag")]
        public string BuildingTag { get; set; }
        [JsonProperty("placedTag")]
        public string PlacedTag { get; set; }
    }
}
