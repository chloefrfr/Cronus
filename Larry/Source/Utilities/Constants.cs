using Larry.Source.Classes.Event;
using Larry.Source.Interfaces;

namespace Larry.Source.Utilities
{
    public class Constants
    {
        public static DateTime ActiveUntil => new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);

        /// <summary>
        /// Gets the list of season events available in the system.
        /// </summary>
        public static readonly List<SeasonEvent> SeasonEvents = new()
        {
            new SeasonEvent
            {
                SeasonNumber = 3,
                Events = new List<IEvent>
                {
                    new Event { EventType = "EventFlag.Spring2018Phase1", ActiveUntil = ActiveUntil, ActiveSince = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") }
                }
            },
            new SeasonEvent
            {
                SeasonNumber = 4,
                Events = new List<IEvent>
                {
                    new Event { EventType = "EventFlag.Blockbuster2018", ActiveUntil = ActiveUntil, ActiveSince = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                    new Event { EventType = "EventFlag.Blockbuster2018Phase1", ActiveUntil = ActiveUntil, ActiveSince = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") }
                }
            },
            new SeasonEvent
            {
                SeasonNumber = 6,
                Events = new List<IEvent>
                {
                    new Event { EventType = "EventFlag.LTM_Fortnitemares", ActiveUntil = ActiveUntil, ActiveSince = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                    new Event { EventType = "EventFlag.FortnitemaresPhase1", ActiveUntil = ActiveUntil, ActiveSince = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                    new Event { EventType = "EventFlag.LTM_LilKevin", ActiveUntil = ActiveUntil, ActiveSince = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                    new Event { EventType = "EventFlag.LobbySeason6Halloween", ActiveUntil = ActiveUntil, ActiveSince = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") }
                }
            },
            new SeasonEvent
            {
                SeasonNumber = 8,
                Events = new List<IEvent>
                {
                    new Event { EventType = "EventFlag.Spring2019", ActiveUntil = ActiveUntil, ActiveSince = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                    new Event { EventType = "EventFlag.Spring2019.Phase1", ActiveUntil = ActiveUntil, ActiveSince = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                    new Event { EventType = "EventFlag.LTM_Ashton", ActiveUntil = ActiveUntil, ActiveSince = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                    new Event { EventType = "EventFlag.LTM_Goose", ActiveUntil = ActiveUntil, ActiveSince = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                    new Event { EventType = "EventFlag.LTM_HighStakes", ActiveUntil = ActiveUntil, ActiveSince = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                    new Event { EventType = "EventFlag.LTE_BootyBay", ActiveUntil = ActiveUntil, ActiveSince = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                    new Event { EventType = "EventFlag.Spring2019.Phase2", ActiveUntil = ActiveUntil, ActiveSince = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") }
                }
            },
            new SeasonEvent
            {
                SeasonNumber = 10,
                Events = new List<IEvent>
                {
                    new Event { EventType = "EventFlag.Mayday", ActiveUntil = ActiveUntil, ActiveSince = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                    new Event { EventType = "EventFlag.Season10.Phase2", ActiveUntil = ActiveUntil, ActiveSince = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                    new Event { EventType = "EventFlag.Season10.Phase3", ActiveUntil = ActiveUntil, ActiveSince = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                    new Event { EventType = "EventFlag.LTE_BlackMonday", ActiveUntil = ActiveUntil, ActiveSince = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                    new Event { EventType = "EventFlag.LTE_SharpShooter", ActiveUntil = ActiveUntil, ActiveSince = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                    new Event { EventType = "EventFlag.LTE_Fortnitemares2020", ActiveUntil = ActiveUntil, ActiveSince = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") }
                }
            },
            new SeasonEvent
            {
                SeasonNumber = 11,
                Events = new List<IEvent>
                {
                    new Event { EventType = "EventFlag.Fortnitemares2020", ActiveUntil = ActiveUntil, ActiveSince = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                    new Event { EventType = "EventFlag.Halloween2020", ActiveUntil = ActiveUntil, ActiveSince = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                    new Event { EventType = "EventFlag.LobbyHalloween2020", ActiveUntil = ActiveUntil, ActiveSince = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                    new Event { EventType = "EventFlag.LobbySeason11Halloween", ActiveUntil = ActiveUntil, ActiveSince = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") }
                }
            },
            new SeasonEvent
            {
                SeasonNumber = 14,
                Events = new List<IEvent>
                {
                    new Event { EventType = "EventFlag.Fortnitemares2021", ActiveUntil = ActiveUntil, ActiveSince = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                    new Event { EventType = "EventFlag.LobbyHalloween2021", ActiveUntil = ActiveUntil, ActiveSince = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") }
                }
            },
            new SeasonEvent
            {
                SeasonNumber = 15,
                Events = new List<IEvent>
                {
                    new Event { EventType = "EventFlag.Season15", ActiveUntil = ActiveUntil, ActiveSince = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                    new Event { EventType = "EventFlag.Winterfest2021", ActiveUntil = ActiveUntil, ActiveSince = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
                    new Event { EventType = "EventFlag.LobbyWinterfest2021", ActiveUntil = ActiveUntil, ActiveSince = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") }
                }
            },
            new SeasonEvent
            {
                SeasonNumber = 16,
                Events = new List<IEvent>
                {
                    new Event { EventType = "EventFlag.Season16", ActiveUntil = ActiveUntil, ActiveSince = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") }
                }
            },
            new SeasonEvent
            {
                SeasonNumber = 17,
                Events = new List<IEvent>
                {
                    new Event { EventType = "EventFlag.Season17", ActiveUntil = ActiveUntil, ActiveSince = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") }
                }
            },
            new SeasonEvent
            {
                SeasonNumber = 18,
                Events = new List<IEvent>
                {
                    new Event { EventType = "EventFlag.Season18", ActiveUntil = ActiveUntil, ActiveSince = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") }
                }
            },
            new SeasonEvent
            {
                SeasonNumber = 19,
                Events = new List<IEvent>
                {
                    new Event { EventType = "EventFlag.Season19", ActiveUntil = ActiveUntil, ActiveSince = DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") }
                }
            }
        };
    }
}
