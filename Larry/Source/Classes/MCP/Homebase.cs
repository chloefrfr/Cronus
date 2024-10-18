using System.Text.Json.Serialization;
using Newtonsoft.Json;


namespace Larry.Source.Classes.MCP
{
    public class Homebase
    {
        [JsonProperty("banner_color_id")]
        public string BannerColorId { get; set; }

        [JsonProperty("banner_icon_id")]
        public string BannerIconId { get; set; }

        [JsonProperty("flag_color")]
        public int FlagColor { get; set; }

        [JsonProperty("flag_pattern")]
        public int FlagPattern { get; set; }

        [JsonProperty("town_name")]
        public string TownName { get; set; }
    }
}
