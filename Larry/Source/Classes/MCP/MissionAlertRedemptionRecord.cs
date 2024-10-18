namespace Larry.Source.Classes.MCP
{
    public class MissionAlertRedemptionRecord
    {
        public object lastClaimTimesMap { get; set; }
        public object lastClaimedGuidPerTheater { get; set; }
        public List<MissionAlertClaimData> claimData { get; set; }
    }
}
