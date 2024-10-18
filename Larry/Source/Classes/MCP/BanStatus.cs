using System.Collections.Generic;

namespace Larry.Source.Classes.MCP
{
    public class BanStatus
    {
        public bool bRequiresUserAck { get; set; }
        public bool bBanHasStarted { get; set; }
        public List<string> banReasons { get; set; }
        public string banStartTimeUtc { get; set; }
        public int? banDurationDays { get; set; }
        public string additionalInfo { get; set; }
        public string exploitProgramName { get; set; }
        public string competitiveBanReason { get; set; }
    }
}
