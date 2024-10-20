using Larry.Source.Interfaces;
using System.Reflection;

namespace Larry.Source.Classes.Timeline
{
    public class EventState
    {
        public DateTime validFrom { get; set; }
        public List<IEvent> activeEvents { get; set; }
        public EventInfo state { get; set; }
    }
}
