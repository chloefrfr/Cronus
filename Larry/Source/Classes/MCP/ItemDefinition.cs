using Larry.Source.Classes.Profile;

namespace Larry.Source.Classes.MCP
{
    public class ItemDefinition
    {
        public string templateId { get; set; }
        public int quantity { get; set; }
        public ItemValue attributes { get; set; }



        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
        }
    }
}
