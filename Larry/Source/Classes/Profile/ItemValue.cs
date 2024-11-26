using Larry.Source.Classes.MCP;
using Newtonsoft.Json;

namespace Larry.Source.Classes.Profile
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class ItemValue
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? item_seen { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<Variants>? variants { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? xp { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? use_count { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? level { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? favorite { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? platform { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? banner_color_template { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? banner_icon_template { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? locker_name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public LockerSlotData? locker_slots_data { get; set; }

        public JObject ToJObjectWithoutNulls()
        {
            var jsonObject = JObject.FromObject(this);

            foreach (var property in jsonObject.Properties())
            {
                if (property.Value.Type == JTokenType.Null)
                {
                    property.Remove();
                }
            }

            return jsonObject;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
