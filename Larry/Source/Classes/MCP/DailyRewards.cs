namespace Larry.Source.Classes.MCP
{
    public class DailyRewards
    {
        public int nextDefaultReward { get; set; }
        public int totalDaysLoggedIn { get; set; }
        public string lastClaimDate { get; set; }
        public Dictionary<string, AdditionalSchedule> additionalSchedules { get; set; }
    }
}
