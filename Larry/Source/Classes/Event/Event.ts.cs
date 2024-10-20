using Larry.Source.Interfaces;

namespace Larry.Source.Classes.Event
{
    public class Event : IEvent
    {
        /// <inheritdoc/>
        public string EventType { get; set; }

        /// <inheritdoc/>
        public DateTime ActiveUntil { get; set; }

        /// <inheritdoc/>
        public string ActiveSince { get; set; }
    }
}
