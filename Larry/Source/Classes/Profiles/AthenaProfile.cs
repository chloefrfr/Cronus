using Larry.Source.Classes.MCP;
using Larry.Source.Classes.Profile;
using Larry.Source.Interfaces;
using Larry.Source.Database.Entities;
using System.Collections.Generic;
using System;
using System.Reflection;
using Newtonsoft.Json;
using Larry.Source.Utilities;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;

namespace Larry.Source.Classes.Profiles
{
    public class AthenaProfile : ProfileBase
    {
        public IProfile Profile { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AthenaProfile"/> class.
        /// </summary>
        /// <param name="accountId">The account ID associated with the profile.</param>
        /// <param name="items">A list of items for the profile.</param>
        /// <param name="attributes">A list of attributes for the profile.</param>
        public AthenaProfile(string accountId, List<Items> items, List<ItemAttributes> attributes) : base(null)
        {
            Profile = CreateProfile(accountId, items, attributes);
        }

        /// <summary>
        /// Creates a profile based on the provided account ID, items, and attributes.
        /// </summary>
        /// <param name="accountId">The account ID associated with the profile.</param>
        /// <param name="items">A list of items for the profile.</param>
        /// <param name="attributes">A list of attributes for the profile.</param>
        /// <returns>An instance of <see cref="IProfile"/> representing the created profile.</returns>
        public IProfile CreateProfile(string accountId, List<Items> items, List<ItemAttributes> attributes)
        {
            var defaultItems = new Dictionary<Guid, ItemDefinition>();
            var favoriteItems = new Dictionary<string, Guid>();

            Logger.Information($"ItemsCount: {items.Count}");

            foreach (var item in items)
            {
                if (item == null || item.Value == null)
                {
                    Logger.Warning($"NULL VALUE FOR {item?.TemplateId}");
                    continue;
                }

                var itemAttributes = System.Text.Json.JsonSerializer.Deserialize<ItemValue>(item.Value.ToString());

                if (itemAttributes == null) continue;

                try
                {
                    dynamic relevantAttributes = new System.Dynamic.ExpandoObject();
                    var deserializedValue = JsonConvert.DeserializeObject<dynamic>(item.Value) ?? new JObject();
                    var attributesDict = new Dictionary<string, object>
                    {
                        { "favorite", deserializedValue.favorite ?? false },
                        { "item_seen", deserializedValue.item_seen ?? false },
                        { "xp", deserializedValue.xp ?? 0 },
                        { "variants", deserializedValue.variants ?? new List<object>() }
                    };

                    defaultItems[Guid.NewGuid()] = new ItemDefinition
                    {
                        templateId = item.TemplateId,
                        quantity = item.Quantity,
                        attributes = MapToItemValue(attributesDict)
                    };

                    if (item.Favorite)
                    {
                        switch (item.TemplateId)
                        {
                            case var id when id.Contains("AthenaCharacter"):
                                favoriteItems["character"] = Guid.NewGuid();
                                break;
                            case var id when id.Contains("AthenaPickaxe"):
                                favoriteItems["pickaxe"] = Guid.NewGuid();
                                break;
                            case var id when id.Contains("AthenaGlider"):
                                favoriteItems["glider"] = Guid.NewGuid();
                                break;
                            case var id when id.Contains("AthenaDance"):
                                favoriteItems["dance"] = Guid.NewGuid();
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"An error occurred while generating default items: {ex.Message}");
                }
            }

            var initialStats = CreateInitialStats(favoriteItems);

            return BuildProfileSkeleton(accountId, defaultItems, initialStats);
        }

        /// <summary>
        /// Maps the provided attributes dictionary to an <see cref="ItemValue"/> object.
        /// </summary>
        /// <param name="attributes">A dictionary of attributes.</param>
        /// <returns>An instance of <see cref="ItemValue"/> representing the mapped attributes.</returns>
        private ItemValue MapToItemValue(Dictionary<string, object> attributes)
        {
            return new ItemValue
            {
                favorite = attributes.TryGetValue("favorite", out var fav) && Convert.ToBoolean(fav),
                item_seen = attributes.TryGetValue("item_seen", out var seen) && Convert.ToBoolean(seen),
                xp = attributes.TryGetValue("xp", out var xp) ? Convert.ToInt32(xp) : 0,
                variants = attributes.TryGetValue("variants", out var variants) && variants is IEnumerable<object> varList
                    ? varList.Select(v => new Variants
                    {
                        channel = (v as dynamic)?.channel ?? string.Empty,
                        owned = (v as dynamic)?.owned ?? new List<string>(),
                        active = (v as dynamic)?.active ?? string.Empty
                    }).ToList()
                    : new List<Variants>()
            };
        }

        /// <summary>
        /// Builds the profile skeleton for the given account ID, default items, and initial stats.
        /// </summary>
        /// <param name="accountId">The account ID associated with the profile.</param>
        /// <param name="defaultItems">A dictionary of default items for the profile.</param>
        /// <param name="initialStats">The initial statistics for the profile.</param>
        /// <returns>An instance of <see cref="IProfile"/> representing the profile skeleton.</returns>
        private IProfile BuildProfileSkeleton(string accountId, Dictionary<Guid, ItemDefinition> defaultItems, StatsAttributes initialStats)
        {
            var cleanedStats = CleanNullAttributes(initialStats);
            return new MCPProfile
            {
                created = DateTime.UtcNow.ToString("o"),
                updated = DateTime.UtcNow.ToString("o"),
                rvn = 0,
                wipeNumber = 1,
                accountId = accountId,
                profileId = "athena",
                version = "no_version",
                stats = cleanedStats,
                items = defaultItems,
                commandRevision = 0,
            };
        }

        /// <summary>
        /// Cleans null attributes from the initial stats.
        /// </summary>
        /// <param name="initialStats">The initial statistics to clean.</param>
        /// <returns>A <see cref="StatsAttributes"/> instance with cleaned attributes.</returns>
        private StatsAttributes CleanNullAttributes(StatsAttributes initialStats)
        {
            var cleanedAttributes = new StatsAttributes
            {
                attributes = new Dictionary<string, object>()
            };

            foreach (var prop in initialStats.attributes.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var value = prop.GetValue(initialStats.attributes);
                if (value != null && !(value is IList<object> list && list.Count == 0))
                {
                    cleanedAttributes.attributes[prop.Name] = value;
                }
            }

            return cleanedAttributes;
        }

        /// <summary>
        /// Creates the initial statistics for the profile based on the favorite items.
        /// </summary>
        /// <param name="favoriteItems">A dictionary of favorite items.</param>
        /// <returns>An instance of <see cref="StatsAttributes"/> representing the initial statistics.</returns>
        private StatsAttributes CreateInitialStats(Dictionary<string, Guid> favoriteItems)
        {
            var attributes = new StatsData
            {
                use_random_loadout = false,
                past_seasons = new List<PastSeasons>(),
                season_match_boost = 0,
                loadouts = new List<string>(),
                mfa_reward_claimed = false,
                rested_xp_overflow = 0,
                current_mtx_platform = "Epic",
                last_xp_interaction = DateTime.UtcNow.ToString("o"),
                quest_manager = new QuestManager
                {
                    dailyLoginInterval = DateTime.MinValue.ToString("o"),
                    dailyQuestRerolls = 1,
                    questPoolStats = new QuestPoolStats()
                },
                book_level = 1,
                season_num = 1,
                book_xp = 0,
                creative_dynamic_xp = new Dictionary<string, int>(),
                season = new Season { numWins = 0, numHighBracket = 0, numLowBracket = 0 },
                lifetime_wins = 0,
                book_purchased = false,
                rested_xp_exchange = 1,
                level = 1,
                rested_xp = 2500,
                rested_xp_mult = 4,
                accountLevel = 1,
                rested_xp_cumulative = 52500,
                xp = 0,
                active_loadout_index = 0,
                favorite_character = favoriteItems.TryGetValue("character", out var characterGuid) ? characterGuid.ToString() : null,
                favorite_pickaxe = favoriteItems.TryGetValue("pickaxe", out var pickaxeGuid) ? pickaxeGuid.ToString() : null,
                favorite_glider = favoriteItems.TryGetValue("glider", out var gliderGuid) ? gliderGuid.ToString() : null,
                favorite_dance = new List<string> { favoriteItems.TryGetValue("dance", out var danceGuid) ? danceGuid.ToString() : string.Empty }
            };

            return new StatsAttributes
            {
                attributes = attributes
            };
        }

        /// <summary>
        /// Gets the profile associated with this instance.
        /// </summary>
        /// <returns>An instance of <see cref="IProfile"/> representing the profile.</returns>
        public override IProfile GetProfile()
        {
            return Profile;
        }
    }
}
