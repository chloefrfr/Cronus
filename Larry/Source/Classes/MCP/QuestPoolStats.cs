namespace Larry.Source.Classes.MCP
{
    public class QuestPoolStats
    {
        public string dailyLoginInterval { get; set; }
        public PoolLockouts poolLockouts { get; set; }
        public List<PoolStat> poolStats { get; set; }
    }
}
