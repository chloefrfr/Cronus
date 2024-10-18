using System.Text.Json.Serialization;
using Newtonsoft.Json;


namespace Larry.Source.Classes
{
    public class LauncherInfoDTO
    {
        [JsonProperty("appName")]
        public string AppName {  get; set; }
        [JsonProperty("catalogItemId")]
        public string CatalogItemId { get; set; }
        [JsonProperty("namespace")]
        public string Namespace { get; set; }

        public override string ToString()
        {
            return $"AppName: {AppName}, CatalogItemId: {CatalogItemId}, Namespace: {Namespace}";
        }
    }
}
