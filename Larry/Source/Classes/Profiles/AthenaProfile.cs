using Larry.Source.Classes.MCP;
using Larry.Source.Classes.Profile;
using Larry.Source.Interfaces;
using Larry.Source.Database.Entities;
using System.Collections.Generic;
using System;
using System.Reflection;
using Newtonsoft.Json;
using Larry.Source.Utilities;

namespace Larry.Source.Classes.Profiles
{
    public class AthenaProfile : ProfileBase
    {
        public IProfile Profile { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AthenaProfile"/> class.
        /// </summary>
        /// <param name="accountId">The account ID associated with the profile.</param>
        /// <param name="items">The list of items associated with the profile.</param>
        /// <param name="attributes">The attributes associated with the profile.</param>
        public AthenaProfile(string accountId, List<Items> items, List<ItemAttributes> attributes) : base(null)
        {
            Profile = CreateProfile(accountId, items, attributes);
        }

        /// <summary>
        /// Method to create a new profile for Athena using pre-existing items and attributes.
        /// </summary>
        /// <param name="accountId">The account ID associated with the profile.</param>
        /// <param name="items">The list of items associated with the profile.</param>
        /// <param name="attributes">The attributes associated with the profile.</param>
        /// <returns>A fully initialized <see cref="IProfile"/> object.</returns>
        public IProfile CreateProfile(string accountId, List<Items> items, List<ItemAttributes> attributes)
        {
            var defaultItems = new Dictionary<Guid, ItemDefinition>();
            var favoriteItems = new Dictionary<string, Guid>();

            foreach (var item in items)
            {
                var itemId = Guid.NewGuid();

                if (item == null) continue;

                var itemAttributes = System.Text.Json.JsonSerializer.Deserialize<ItemValue>(item.Value.ToString());

                //if (HasAnyNullProperties(itemAttributes)) continue;
                
                if (itemAttributes != null)
                {
                    try
                    {
                        dynamic relevantAttributes = new System.Dynamic.ExpandoObject();
                        var attributesType = itemAttributes.GetType();

                        foreach (var property in attributesType.GetProperties())
                        {
                            var jsonProperty = property.GetCustomAttribute<JsonPropertyAttribute>();
                            var jsonPropertyName = jsonProperty != null ? jsonProperty.PropertyName : property.Name;

                            var deserializedValue = JsonConvert.DeserializeObject<dynamic>(item.Value);

                            //var value = deserializedValue?.GetType().GetProperty(property.Name)?.GetValue(deserializedValue);

                            switch (jsonPropertyName)
                            {
                                case "favorite":
                                    ((IDictionary<string, object>)relevantAttributes)["favorite"] = deserializedValue.favorite;
                                    break;
                                case "item_seen":
                                    ((IDictionary<string, object>)relevantAttributes)["item_seen"] = deserializedValue.item_seen;
                                    break;
                                case "xp":
                                    ((IDictionary<string, object>)relevantAttributes)["xp"] = deserializedValue.xp;
                                    break;
                                case "variants":
                                    ((IDictionary<string, object>)relevantAttributes)["variants"] = deserializedValue.variants;
                                    break;
                            }
                        }

                        defaultItems[itemId] = new ItemDefinition
                        {
                            TemplateId = item.TemplateId,
                            Quantity = item.Quantity,
                            Attributes = relevantAttributes
                        };
                    } catch (Exception ex)
                    {
                        Logger.Error($"An error occured while generating default items: {ex.Message}");
                    }
                }

                if (item.Favorite)
                {
                    switch (item.TemplateId)
                    {
                        case var id when id.Contains("AthenaCharacter"):
                            favoriteItems["character"] = itemId;
                            break;
                        case var id when id.Contains("AthenaPickaxe"):
                            favoriteItems["pickaxe"] = itemId;
                            break;
                        case var id when id.Contains("AthenaGlider"):
                            favoriteItems["glider"] = itemId;
                            break;
                        case var id when id.Contains("AthenaDance"):
                            favoriteItems["dance"] = itemId;
                            break;
                    }
                }
            }

            var initialStats = CreateInitialStats(favoriteItems);

            return BuildProfileSkeleton(accountId, defaultItems, initialStats);
        }

        /// <summary>
        /// Checks if any properties of the specified item are null.
        /// </summary>
        /// <typeparam name="T">The type of the item to check.</typeparam>
        /// <param name="item">The item instance to inspect.</param>
        /// <returns>true if any property is null; otherwise, false.</returns>
        private bool HasAnyNullProperties<T>(T item)
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            return properties.Any(prop => prop.GetValue(item) == null);
        }

        private ItemValue CreateFilteredAttributes(ItemValue itemAttributes)
        {
            var filteredAttributes = new ItemValue();

            foreach (var property in typeof(ItemValue).GetProperties())
            {
                var value = property.GetValue(itemAttributes);
                if (value != null) 
                {
                    property.SetValue(filteredAttributes, value);
                }
            }

            return filteredAttributes;
        }

        /// <summary>
        /// Builds the profile skeleton, initializing all necessary components for a new profile.
        /// </summary>
        /// <param name="accountId">The unique identifier for the account associated with the profile.</param>
        /// <param name="items">A list of items associated with the profile.</param>
        /// <param name="attributes">A list of attributes for the profile items.</param>
        /// <returns>An instance of <see cref="IProfile"/> with the initialized profile data.</returns>
        private IProfile BuildProfileSkeleton(string accountId, Dictionary<Guid, ItemDefinition> defaultItems, StatsAttributes initialStats)
        {
            return new MCPProfile
            {
                Created = DateTime.UtcNow.ToString("o"),
                Updated = DateTime.UtcNow.ToString("o"),
                Rvn = 0,
                WipeNumber = 1,
                AccountId = accountId,
                ProfileId = "athena",
                Version = "no_version",
                Stats = initialStats,
                Items = defaultItems,
                CommandRevision = 0,
            };
        }

        /// <summary>
        /// Creates the initial stats for the profile.
        /// </summary>
        /// <returns>A dictionary containing the initial stats for the profile.</returns>
        private StatsAttributes CreateInitialStats(Dictionary<string, Guid> favoriteItems)
        {
            return new StatsAttributes
            {
                // Why do they do this, why have a attributes object????
                Attributes = new StatsData
                {
                    UseRandomLoadout = false,
                    PastSeasons = new List<PastSeasons>(),
                    SeasonMatchBoost = 0,
                    Loadouts = new List<string>(),
                    MfaRewardClaimed = false,
                    RestedXpOverflow = 0,
                    CurrentMtxPlatform = "Epic",
                    LastXpInteraction = DateTime.UtcNow.ToString("o"),
                    QuestManager = new QuestManager
                    {
                        DailyLoginInterval = DateTime.MinValue.ToString("o"),
                        DailyQuestRerolls = 1
                    },
                    BookLevel = 1,
                    SeasonNum = 13,
                    BookXp = 0,
                    CreativeDynamicXp = new Dictionary<string, int>(),
                    Season = new Season(),
                    LifetimeWins = 0,
                    BookPurchased = false,
                    RestedXpExchange = 1,
                    Level = 1,
                    RestedXp = 2500,
                    RestedXpMult = 4,
                    AccountLevel = 1,
                    RestedXpCumulative = 52500,
                    Xp = 0,
                    ActiveLoadoutIndex = 0,
                    FavoriteCharacter = favoriteItems.ContainsKey("character") ? favoriteItems["character"].ToString() : string.Empty,
                    FavoritePickaxe = favoriteItems.ContainsKey("pickaxe") ? favoriteItems["pickaxe"].ToString() : string.Empty,
                    FavoriteGlider = favoriteItems.ContainsKey("glider") ? favoriteItems["glider"].ToString() : string.Empty,
                    FavoriteDance = new List<string> { favoriteItems.ContainsKey("dance") ? favoriteItems["dance"].ToString() : string.Empty },
                    BannerIcon = "BRSeason01",
                    BannerColor = "DefaultColor1"
                }
            };
        }


        public override IProfile GetProfile()
        {
            return Profile;
        }
    }
}
