namespace Larry.Source.Classes.MCP.Response
{
    public class RefundResponse : BaseResponse
    {
        public List<MultiUpdate> MultiUpdate { get; set; } = new List<MultiUpdate>();
    }
}
