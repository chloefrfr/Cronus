namespace Larry.Source.Classes.MCP
{
    public class InAppPurchases
    {
        public List<string> receipts { get; set; }
        public List<string> ignoredReceipts { get; set; }
        public Dictionary<string, int> fulfillmentCounts { get; set; }
        public Dictionary<string, RefreshTimer> refreshTimers { get; set; }
        public int version { get; set; }
    }
}
