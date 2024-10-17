using Larry.Source.Classes;
using Larry.Source.Enums;
using Larry.Source.Utilities.Converters;
using System.Text.Json.Serialization;

namespace Larry.Source.Interfaces
{
    public interface ILightswitchData
    {
        [JsonPropertyName("serviceInstanceId")]
        public string ServiceInstanceId { get; set; }
        [JsonPropertyName("status")]
        [JsonConverter(typeof(StatusEnumConverter))]
        public StatusEnum Status { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
        [JsonPropertyName("maintenanceUri")]
        public string MaintenanceUri { get; set; }
        [JsonPropertyName("overrideCatalogIds")]
        public List<string> OverrideCatalogIds { get; set; }
        [JsonPropertyName("allowedActions")]
        [JsonConverter(typeof(ActionsEnumListConverter))]
        public List<ActionsEnum> AllowedActions { get; set; }
        [JsonPropertyName("banned")]
        public bool Banned { get; set; }
        [JsonPropertyName("launcherInfoDTO")]
        public LauncherInfoDTO LauncherInfoDTO { get; set; }
    }
}
