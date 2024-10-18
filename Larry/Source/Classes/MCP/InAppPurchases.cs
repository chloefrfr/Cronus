namespace Larry.Source.Classes.MCP
{
    public class InAppPurchases
    {
        public List<string> Receipts { get; set; }
        public List<string> IgnoredReceipts { get; set; }
        public Dictionary<string, int> FulfillmentCounts { get; set; }
        public Dictionary<string, RefreshTimer> RefreshTimers { get; set; }
        public int Version { get; set; }
    }
}
