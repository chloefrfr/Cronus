namespace Larry.Source.Classes.Timeline
{
    public class ElectionState
    {
        public string electionId { get; set; }
        public List<string> candidates { get; set; }
        public string electionEnds { get; set; }
        public int numWinners { get; set; }
        public int wipeNumber { get; set; }
        public int winnerStateHours { get; set; }
        public List<string> offers { get; set; }
    }
}
