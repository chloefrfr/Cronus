using System.Text.Json.Serialization;

namespace Larry.Source.Classes
{
    public class LauncherInfoDTO
    {
        [JsonPropertyName("appName")]
        public string AppName {  get; set; }
        [JsonPropertyName("catalogItemId")]
        public string CatalogItemId { get; set; }
        [JsonPropertyName("namespace")]
        public string Namespace { get; set; }

        public override string ToString()
        {
            return $"AppName: {AppName}, CatalogItemId: {CatalogItemId}, Namespace: {Namespace}";
        }
    }
}
