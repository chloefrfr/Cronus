using Larry.Source.Classes.Event;
using Larry.Source.Interfaces;
using Larry.Source.Utilities.Parsers;

namespace Larry.Source.Utilities.Managers
{
    public class TimelineManager
    {
        private static readonly string CurrentDateTime = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");


        /// <summary>
        /// Creates a timeline of events based on the provided user agent string.
        /// </summary>
        /// <param name="userAgent">The user agent string to parse.</param>
        /// <returns>A task that represents the asynchronous operation, containing the list of events.</returns>
        public static async Task<List<IEvent>> CreateTimelineAsync(string? userAgent)
        {
            var parsedUserAgent = UserAgentParser.Parse(userAgent);
            var events = new List<IEvent>
            {
                CreateEvent($"EventFlag.Season{parsedUserAgent?.Season}", CurrentDateTime),
                CreateEvent($"EventFlag.LobbySeason{parsedUserAgent?.Season}", CurrentDateTime)
            };

            AddSeasonEvents(parsedUserAgent?.Season, events);

            return events;
        }

        /// <summary>
        /// Creates an event with the specified type and active since date.
        /// </summary>
        /// <param name="eventType">The type of the event.</param>
        /// <param name="activeSince">The date and time since the event is active.</param>
        /// <returns>The created event.</returns>
        private static IEvent CreateEvent(string eventType, string activeSince)
        {
            return new Event
            {
                EventType = eventType,
                ActiveUntil = Constants.ActiveUntil,
                ActiveSince = activeSince
            };
        }

        /// <summary>
        /// Adds season-specific events to the provided event list based on the season number.
        /// </summary>
        /// <param name="seasonNumber">The season number.</param>
        /// <param name="events">The collection of events to add to.</param>
        private static void AddSeasonEvents(int? seasonNumber, ICollection<IEvent> events)
        {
            if (seasonNumber.HasValue)
            {
                var seasonEvents = Constants.SeasonEvents
                    .FirstOrDefault(season => season.SeasonNumber == seasonNumber.Value)?.Events;

                if (seasonEvents != null)
                {
                    foreach (var seasonEvent in seasonEvents)
                    {
                        events.Add(new Event
                        {
                            EventType = seasonEvent.EventType,
                            ActiveUntil = seasonEvent.ActiveUntil,
                            ActiveSince = seasonEvent.ActiveSince
                        });
                    }
                }
            }
        }
    }
}
