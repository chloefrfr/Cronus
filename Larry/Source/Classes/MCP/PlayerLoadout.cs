namespace Larry.Source.Classes.MCP
{
    public class PlayerLoadout
    {
        public QuickBarRecord primaryQuickBarRecord { get; set; }
        public List<object> pinnedSchematicInstances { get; set; }
        public QuickBarRecord secondaryQuickBarRecord { get; set; }
        public int zonesCompleted { get; set; }
        public bool bPlayerIsNew { get; set; }
    }
}
