namespace Larry.Source.Classes.MCP
{
    public class VoteData
    {
        public string electionId { get; set; }
        public Dictionary<string, object> voteHistory { get; set; }
        public int votesRemaining { get; set; }
        public string lastVoteGranted { get; set; }
    }
}
