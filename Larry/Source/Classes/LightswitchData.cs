using System.Text.Json.Serialization;
using Larry.Source.Interfaces;
using Larry.Source.Enums;
using Larry.Source.Utilities.Converters;
using Newtonsoft.Json;


namespace Larry.Source.Classes
{
    public class LightswitchData : ILightswitchData
    {
        [JsonProperty("serviceInstanceId")]
        public string ServiceInstanceId { get; set; }
        
        [JsonProperty("status")]
        [System.Text.Json.Serialization.JsonConverter(typeof(StatusEnumConverter))]
        public StatusEnum Status { get; set; }
        
        [JsonProperty("message")]
        public string Message { get; set; }
        
        [JsonProperty("maintenanceUri")]
        public string MaintenanceUri { get; set; }

        [JsonProperty("overrideCatalogIds")]
        public List<string> OverrideCatalogIds { get; set; }
        
        [JsonProperty("allowedActions")]
        [System.Text.Json.Serialization.JsonConverter(typeof(ActionsEnumListConverter))]
        public List<ActionsEnum> AllowedActions { get; set; }
        
        [JsonProperty("banned")]
        public bool Banned { get; set; }
        
        [JsonProperty("launcherInfoDTO")]
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
