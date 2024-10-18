using System.Text.Json.Serialization;

namespace Larry.Source.Classes.MCP
{
    public class Homebase
    {
        [JsonPropertyName("banner_color_id")]
        public string BannerColorId { get; set; }

        [JsonPropertyName("banner_icon_id")]
        public string BannerIconId { get; set; }

        [JsonPropertyName("flag_color")]
        public int FlagColor { get; set; }

        [JsonPropertyName("flag_pattern")]
        public int FlagPattern { get; set; }

        [JsonPropertyName("town_name")]
        public string TownName { get; set; }
    }
}
