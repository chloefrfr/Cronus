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
    public class CommonCoreProfile : ProfileBase
    {
        public IProfile Profile { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonCoreProfile"/> class.
        /// </summary>
        /// <param name="accountId">The account ID associated with the profile.</param>
        /// <param name="items">The list of items associated with the profile.</param>
        /// <param name="attributes">The attributes associated with the profile.</param>
        public CommonCoreProfile(string accountId, List<Items> items, List<ItemAttributes> attributes) : base(null)
        {
            Profile = CreateProfile(accountId, items, attributes);
        }

        /// <summary>
        /// Method to create a new profile for CoommonCore using pre-existing items and attributes.
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
            }

            var initialStats = CreateInitialStats(favoriteItems);

            return BuildProfileSkeleton(accountId, defaultItems, initialStats);
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
                created = DateTime.UtcNow.ToString("o"),
                updated = DateTime.UtcNow.ToString("o"),
                rvn = 0,
                wipeNumber = 1,
                accountId = accountId,
                profileId = "common_core",
                version = "no_version",
                stats = initialStats,
                items = defaultItems,
                commandRevision = 0,
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
                attributes = new StatsData
                {
                    survey_data = new object(),
                    personal_offers = new object(),
                    intro_game_played = true,
                    import_friends_claimed = new object(),
                    mtx_purchase_history = new MtxPurchaseHistory
                    {
                        purchases = new List<object>(),
                        refundCredits = 3,
                        refundsUsed = 0,
                    },
                    undo_cooldowns = new List<object>(),
                    mtx_affiliate_set_time = "None",
                    inventory_limit_bonus = 0,
                    current_mtx_platform = "EpicPC",
                    mtx_affiliate = "None",
                    forced_intro_played = "Coconut",
                    weekly_purchases = new object(),
                    daily_purchases = new object(),
                    ban_history = new BanHistory
                    {
                        banCount = new Dictionary<string, int>(),
                        banTier = new Dictionary<string, int>(),
                    },
                    in_app_purchases = new InAppPurchases
                    {
                        receipts = new List<string>(),
                        fulfillmentCounts = new Dictionary<string, int>(),
                        ignoredReceipts = new List<string>(),
                        refreshTimers = new Dictionary<string, RefreshTimer>
                        {
                            { "EpicPurchasingService", new RefreshTimer
                            {
                                NextEntitlementRefresh = DateTime.UtcNow.ToString("o")
                            } }  
                        },
                        version = 1
                    },
                    permissions = new List<object>(),
                    undo_timeout = "min",
                    monthly_purchases = new object(),
                    allowed_to_receive_gifts = true,
                    mfa_enabled = false,
                    allowed_to_send_gifts = true,
                    gift_history = new object(),
                    banner_icon = "",
                    banner_color = "",
                    homebase_name = "",
                    gifts = new List<Gifts>(),
                    bans = new BanStatus
                    {
                        bRequiresUserAck = false,
                        bBanHasStarted = false,
                        banReasons = new List<string>(),
                        banStartTimeUtc =null,
                        banDurationDays = null,
                        additionalInfo = "",
                        exploitProgramName = "",
                        competitiveBanReason = "None"
                    }
                }
            };
        }


        public override IProfile GetProfile()
        {
            return Profile;
        }
    }
}
