using Larry.Source.Classes.MCP;
using Newtonsoft.Json;

namespace Larry.Source.Classes.Profile
{
    public class ItemValue
    {
        public bool item_seen { get; set; }
        public List<Variants> variants { get; set; }
        public int xp { get; set; }
        public int use_count { get; set; }
        public int level { get; set; }
        public bool favorite { get; set; }
        public string? platform { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

    }
}
