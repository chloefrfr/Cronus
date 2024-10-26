using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Objects.GameplayTags;
using Larry.Source.Classes.Event;
using Larry.Source.Classes.MCP;
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


        /// <summary>
        /// Parses and finds the channel name from the tag text.
        /// </summary>
        /// <param name="tagText">The tag text to parse.</param>
        /// <returns>The found channel name.</returns>
        public static string ParseChannelName(string tagText)
        {
            const string propertyPrefix = "Cosmetics.Variant.Property.";
            const string channelPrefix = "Cosmetics.Variant.Channel.";

            int propertyIndex = tagText.IndexOf(propertyPrefix);
            if (propertyIndex >= 0)
            {
                return tagText.Substring(propertyIndex + propertyPrefix.Length);
            }

            int channelIndex = tagText.IndexOf(channelPrefix);
            if (channelIndex >= 0)
            {
                return tagText.Substring(channelIndex + channelPrefix.Length);
            }

            return string.Empty;
        }

        /// <summary>
        /// Creates a <see cref="Variants"/> instance from an array of option structures based on the specified channel.
        /// </summary>
        /// <param name="options">An array of <see cref="FStructFallback"/> structures representing the variant options.</param>
        /// <param name="channel">The channel name for the variant.</param>
        /// <returns>A <see cref="Variants"/> object with parsed active and owned properties.</returns>
        public static Variants CreateVariantFromOptions(FStructFallback[] options, string channel)
        {
            var variant = new Variants
            {
                channel = channel,
                active = string.Empty,
                owned = new List<string>()
            };

            foreach (var option in options)
            {
                if (option.TryGetValue(out FGameplayTag gameplayTag, "CustomizationVariantTag") &&
                    option.TryGetValue(out bool bIsDefault, "bIsDefault"))
                {
                    var tagName = ParseChannelName(gameplayTag.TagName.PlainText);

                    if (bIsDefault)
                    {
                        variant.active = tagName;
                    }
                    variant.owned.Add(tagName);
                }
            }

            return variant;
        }
    }
}
