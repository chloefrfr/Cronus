namespace Larry.Source.Classes.MCP.Response
{
    public class PurchaseResponse : BaseResponse
    {
        public List<object> Notifications { get; set; } = new List<object>();
        public List<MultiUpdate> MultiUpdate { get; set; } = new List<MultiUpdate>();
    }
}
