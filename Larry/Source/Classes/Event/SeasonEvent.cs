using Larry.Source.Interfaces;

namespace Larry.Source.Classes.Event
{
    public class SeasonEvent
    {
        public int SeasonNumber { get; set; }
        public List<IEvent> Events { get; set; } = new();
    }
}
