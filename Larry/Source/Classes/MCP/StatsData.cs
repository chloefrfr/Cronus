using Newtonsoft.Json;

namespace Larry.Source.Classes.MCP
{
    public class StatsData
    {
        [JsonProperty("current_season")]
        public int CurrentSeason { get; set; }

        [JsonProperty("last_used_project")]
        public string LastUsedProject { get; set; }

        [JsonProperty("max_island_plots")]
        public int MaxIslandPlots { get; set; }

        [JsonProperty("homebase_name")]
        public string? HomebaseName { get; set; }

        [JsonProperty("publish_allowed")]
        public bool PublishAllowed { get; set; }

        [JsonProperty("support_code")]
        public string SupportCode { get; set; }

        [JsonProperty("bans")]
        public object? Bans { get; set; }

        [JsonProperty("homebase")]
        public Homebase? Homebase { get; set; }

        [JsonProperty("default_hero_squad_id")]
        public string? DefaultHeroSquadId { get; set; }

        [JsonProperty("last_used_plot")]
        public string LastUsedPlot { get; set; }

        [JsonProperty("creator_name")]
        public string CreatorName { get; set; }

        [JsonProperty("use_random_loadout")]
        public bool UseRandomLoadout { get; set; }

        [JsonProperty("fromAccountId")]
        public string FromAccountId { get; set; }

        [JsonProperty("past_seasons")]
        public List<PastSeasons> PastSeasons { get; set; }

        [JsonProperty("season_match_boost")]
        public int SeasonMatchBoost { get; set; }

        [JsonProperty("theater_unique_id")]
        public string? TheaterUniqueId { get; set; }

        [JsonProperty("past_lifetime_zones_completed")]
        public int? PastLifetimeZonesCompleted { get; set; }

        [JsonProperty("last_event_instance_key")]
        public string? LastEventInstanceKey { get; set; }

        [JsonProperty("last_zones_completed")]
        public int? LastZonesCompleted { get; set; }

        [JsonProperty("loadouts")]
        public List<string> Loadouts { get; set; }

        [JsonProperty("mfa_reward_claimed")]
        public bool MfaRewardClaimed { get; set; }

        [JsonProperty("rested_xp_overflow")]
        public int RestedXpOverflow { get; set; }

        [JsonProperty("current_mtx_platform")]
        public string CurrentMtxPlatform { get; set; }

        [JsonProperty("last_xp_interaction")]
        public string LastXpInteraction { get; set; }

        [JsonProperty("node_costs")]
        public NodeCosts? NodeCosts { get; set; }

        [JsonProperty("twitch")]
        public object? Twitch { get; set; }

        [JsonProperty("mode_loadouts")]
        public List<object>? ModeLoadouts { get; set; }

        [JsonProperty("mission_alert_redemption_record")]
        public MissionAlertRedemptionRecord MissionAlertRedemptionRecord { get; set; }

        [JsonProperty("node_loadouts")]
        public List<object>? NodeLoadouts { get; set; }

        [JsonProperty("named_counters")]
        public Dictionary<string, NamedCounter>? NamedCounters { get; set; }

        [JsonProperty("player_loadout")]
        public PlayerLoadout? PlayerLoadout { get; set; }

        [JsonProperty("research_levels")]
        public ResearchLevels ResearchLevels { get; set; }

        [JsonProperty("client_settings")]
        public ClientSettings ClientSettings { get; set; }

        [JsonProperty("selected_hero_loadout")]
        public string SelectedHeroLoadout { get; set; }

        [JsonProperty("latent_xp_marker")]
        public string LatentXpMarker { get; set; }

        [JsonProperty("collection_book")]
        public CollectionBook CollectionBook { get; set; }

        [JsonProperty("quest_manager")]
        public QuestManager QuestManager { get; set; }

        [JsonProperty("campaign_stats")]
        public List<CampaignStat> CampaignStats { get; set; }

        [JsonProperty("gameplay_stats")]
        public List<GameplayStat> GameplayStats { get; set; }

        [JsonProperty("event_currency")]
        public EventCurrency EventCurrency { get; set; }

        [JsonProperty("matches_played")]
        public int MatchesPlayed { get; set; }

        [JsonProperty("daily_rewards")]
        public DailyRewards DailyRewards { get; set; }

        [JsonProperty("quest_completion_session_ids")]
        public Dictionary<string, string> QuestCompletionSessionIds { get; set; }

        [JsonProperty("packs_granted")]
        public int PacksGranted { get; set; }

        [JsonProperty("book_level")]
        public int BookLevel { get; set; }

        [JsonProperty("season_num")]
        public int SeasonNum { get; set; }

        [JsonProperty("book_xp")]
        public int BookXp { get; set; }

        [JsonProperty("creative_dynamic_xp")]
        public object CreativeDynamicXp { get; set; }

        [JsonProperty("season")]
        public Season Season { get; set; }

        [JsonProperty("party_assist_quest")]
        public string PartyAssistQuest { get; set; }

        [JsonProperty("pinned_quest")]
        public string PinnedQuest { get; set; }

        [JsonProperty("vote_data")]
        public VoteData VoteData { get; set; }

        [JsonProperty("lifetime_wins")]
        public int LifetimeWins { get; set; }

        [JsonProperty("book_purchased")]
        public bool BookPurchased { get; set; }

        [JsonProperty("rested_xp_exchange")]
        public int RestedXpExchange { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("rested_xp")]
        public int RestedXp { get; set; }

        [JsonProperty("rested_xp_mult")]
        public int RestedXpMult { get; set; }

        [JsonProperty("accountLevel")]
        public int AccountLevel { get; set; }

        [JsonProperty("rested_xp_cumulative")]
        public int RestedXpCumulative { get; set; }

        [JsonProperty("xp")]
        public int Xp { get; set; }

        [JsonProperty("battlestars")]
        public int Battlestars { get; set; }

        [JsonProperty("battlestars_season_total")]
        public int BattlestarsSeasonTotal { get; set; }

        [JsonProperty("season_friend_match_boost")]
        public int SeasonFriendMatchBoost { get; set; }

        [JsonProperty("active_loadout_index")]
        public int ActiveLoadoutIndex { get; set; }

        [JsonProperty("purchased_bp_offers")]
        public List<object> PurchasedBpOffers { get; set; }

        [JsonProperty("purchased_battle_pass_tier_offers")]
        public List<object> PurchasedBattlePassTierOffers { get; set; }

        [JsonProperty("last_match_end_datetime")]
        public string LastMatchEndDatetime { get; set; }

        [JsonProperty("mtx_purchase_history_copy")]
        public List<object> MtxPurchaseHistoryCopy { get; set; }

        [JsonProperty("last_applied_loadout")]
        public string LastAppliedLoadout { get; set; }

        [JsonProperty("favorite_musicpack")]
        public string FavoriteMusicpack { get; set; }

        [JsonProperty("banner_icon")]
        public string BannerIcon { get; set; }

        [JsonProperty("favorite_character")]
        public string FavoriteCharacter { get; set; }

        [JsonProperty("favorite_itemwraps")]
        public List<string> FavoriteItemWraps { get; set; }

        [JsonProperty("favorite_skydivecontrail")]
        public string FavoriteSkydiveContrail { get; set; }

        [JsonProperty("favorite_pickaxe")]
        public string FavoritePickaxe { get; set; }

        [JsonProperty("favorite_glider")]
        public string FavoriteGlider { get; set; }

        [JsonProperty("favorite_backpack")]
        public string FavoriteBackpack { get; set; }

        [JsonProperty("favorite_dance")]
        public List<string> FavoriteDance { get; set; }

        [JsonProperty("favorite_loadingscreen")]
        public string FavoriteLoadingScreen { get; set; }

        [JsonProperty("banner_color")]
        public string BannerColor { get; set; }

        [JsonProperty("survey_data")]
        public object SurveyData { get; set; }

        [JsonProperty("personal_offers")]
        public object PersonalOffers { get; set; }

        [JsonProperty("intro_game_played")]
        public bool IntroGamePlayed { get; set; }

        [JsonProperty("import_friends_claimed")]
        public object ImportFriendsClaimed { get; set; }

        [JsonProperty("mtx_purchase_history")]
        public MtxPurchaseHistory MtxPurchaseHistory { get; set; }

        [JsonProperty("undo_cooldowns")]
        public List<object> UndoCooldowns { get; set; }

        [JsonProperty("mtx_affiliate_set_time")]
        public string MtxAffiliateSetTime { get; set; }

        [JsonProperty("inventory_limit_bonus")]
        public int InventoryLimitBonus { get; set; }

        [JsonProperty("mtx_affiliate")]
        public string MtxAffiliate { get; set; }
        [JsonProperty("forced_intro_played")]
        public string ForcedIntroPlayed { get; set; }
        [JsonProperty("weekly_purchases")]
        public object WeeklyPurchases { get; set; }
        [JsonProperty("daily_purchases")]
        public object DailyPurchases { get; set; }
        [JsonProperty("ban_history")]
        public BanHistory BanHistory { get; set; }
        [JsonProperty("in_app_purchases")]
        public InAppPurchases InAppPurchases { get; set; }
        [JsonProperty("permissions")]
        public List<object> Permissions { get; set; }
        [JsonProperty("undo_timeout")]
        public string UndoTimeout { get; set; }
        [JsonProperty("monthly_purchases")]
        public object MonthlyPurchases { get; set; }
        [JsonProperty("allowed_to_send_gifts")]
        public bool AllowedToSendGifts { get; set; }
        [JsonProperty("mfa_enabled")]
        public bool MfaEnabled { get; set; }
        [JsonProperty("allowed_to_receive_gifts")]
        public bool AllowedToReceiveGifts { get; set; }
        [JsonProperty("gift_history")]
        public object GiftHistory { get; set; }
        [JsonProperty("gifts")]
        public List<Gifts> Gifts { get; set; }
        [JsonProperty("ban_status")]
        public BanStatus BanStatus { get; set; }
    }
}
