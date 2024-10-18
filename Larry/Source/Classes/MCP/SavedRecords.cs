using Newtonsoft.Json;

namespace Larry.Source.Classes.MCP
{
    public class SavedRecords
    {
        [JsonProperty("recordIndex")]
        public int RecordIndex { get; set; }
        [JsonProperty("archiveNumber")]
        public int ArchiveIndex { get; set; }
        [JsonProperty("recordFilename")]
        public string RecordFileName { get; set; }
    }
}
