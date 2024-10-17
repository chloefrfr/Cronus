using System.Text.Json.Serialization;
using Larry.Source.Interfaces;
using Larry.Source.Enums;
using Larry.Source.Utilities.Converters;

namespace Larry.Source.Classes
{
    public class LightswitchData : ILightswitchData
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

        public override string ToString()
        {
            return $"ServiceInstanceId: {ServiceInstanceId}, Status: {Status}, Message: {Message}, " +
               $"OverrideCatalogIds: [{string.Join(", ", OverrideCatalogIds)}], " +
               $"AllowedActions: [{string.Join(", ", AllowedActions)}], Banned: {Banned}, " +
               $"LauncherInfoDTO: {LauncherInfoDTO}";
        }
    }
}
