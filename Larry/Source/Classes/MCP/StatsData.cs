using System.Collections.Generic;

namespace Larry.Source.Classes.MCP
{
    public class StatsData
    {
        public int current_season { get; set; }
        public string last_used_project { get; set; }
        public int max_island_plots { get; set; }
        public string? homebase_name { get; set; }
        public bool publish_allowed { get; set; }
        public string support_code { get; set; }
        public object? bans { get; set; }
        public Homebase? homebase { get; set; }
        public string? default_hero_squad_id { get; set; }
        public string last_used_plot { get; set; }
        public string creator_name { get; set; }
        public bool use_random_loadout { get; set; }
        public string fromAccountId { get; set; }
        public List<PastSeasons> past_seasons { get; set; }
        public int season_match_boost { get; set; }
        public string? theater_unique_id { get; set; }
        public int? past_lifetime_zones_completed { get; set; }
        public string? last_event_instance_key { get; set; }
        public int? last_zones_completed { get; set; }
        public List<string> loadouts { get; set; }
        public bool mfa_reward_claimed { get; set; }
        public int rested_xp_overflow { get; set; }
        public string current_mtx_platform { get; set; }
        public string last_xp_interaction { get; set; }
        public NodeCosts? node_costs { get; set; }
        public object? twitch { get; set; }
        public List<object>? mode_loadouts { get; set; }
        public MissionAlertRedemptionRecord mission_alert_redemption_record { get; set; }
        public List<object>? node_loadouts { get; set; }
        public Dictionary<string, NamedCounter>? named_counters { get; set; }
        public PlayerLoadout? player_loadout { get; set; }
        public ResearchLevels research_levels { get; set; }
        public ClientSettings client_settings { get; set; }
        public string selected_hero_loadout { get; set; }
        public string latent_xp_marker { get; set; }
        public CollectionBook collection_book { get; set; }
        public QuestManager quest_manager { get; set; }
        public List<CampaignStat> campaign_stats { get; set; }
        public List<GameplayStat> gameplay_stats { get; set; }
        public EventCurrency event_currency { get; set; }
        public int matches_played { get; set; }
        public DailyRewards daily_rewards { get; set; }
        public Dictionary<string, string> quest_completion_session_ids { get; set; }
        public int packs_granted { get; set; }
        public int book_level { get; set; }
        public int season_num { get; set; }
        public int book_xp { get; set; }
        public object creative_dynamic_xp { get; set; }
        public Season season { get; set; }
        public string party_assist_quest { get; set; }
        public string pinned_quest { get; set; }
        public VoteData vote_data { get; set; }
        public int lifetime_wins { get; set; }
        public bool book_purchased { get; set; }
        public int rested_xp_exchange { get; set; }
        public int level { get; set; }
        public int rested_xp { get; set; }
        public int rested_xp_mult { get; set; }
        public int accountLevel { get; set; }
        public int rested_xp_cumulative { get; set; }
        public int xp { get; set; }
        public int battlestars { get; set; }
        public int battlestars_season_total { get; set; }
        public int season_friend_match_boost { get; set; }
        public int active_loadout_index { get; set; }
        public List<object> purchased_bp_offers { get; set; }
        public List<object> purchased_battle_pass_tier_offers { get; set; }
        public string last_match_end_datetime { get; set; }
        public List<object> mtx_purchase_history_copy { get; set; }
        public string last_applied_loadout { get; set; }
        public string favorite_musicpack { get; set; }
        public string banner_icon { get; set; }
        public string favorite_character { get; set; }
        public List<string> favorite_itemwraps { get; set; }
        public string favorite_skydivecontrail { get; set; }
        public string favorite_pickaxe { get; set; }
        public string favorite_glider { get; set; }
        public string favorite_backpack { get; set; }
        public List<string> favorite_dance { get; set; }
        public string favorite_loadingscreen { get; set; }
        public string banner_color { get; set; }
        public object survey_data { get; set; }
        public object personal_offers { get; set; }
        public bool intro_game_played { get; set; }
        public object import_friends_claimed { get; set; }
        public MtxPurchaseHistory mtx_purchase_history { get; set; }
        public List<object> undo_cooldowns { get; set; }
        public string mtx_affiliate_set_time { get; set; }
        public int inventory_limit_bonus { get; set; }
        public string mtx_affiliate { get; set; }
        public string forced_intro_played { get; set; }
        public object weekly_purchases { get; set; }
        public object daily_purchases { get; set; }
        public BanHistory ban_history { get; set; }
        public InAppPurchases in_app_purchases { get; set; }
        public List<object> permissions { get; set; }
        public string undo_timeout { get; set; }
        public object monthly_purchases { get; set; }
        public bool allowed_to_send_gifts { get; set; }
        public bool mfa_enabled { get; set; }
        public bool allowed_to_receive_gifts { get; set; }
        public object gift_history { get; set; }
        public List<Gifts> gifts { get; set; }
        public BanStatus ban_status { get; set; }
        public Dictionary<string, object?> ToDictionary()
        {
            var properties = GetType().GetProperties();
            var dictionary = new Dictionary<string, object?>();

            foreach (var property in properties)
            {
                var value = property.GetValue(this);
                dictionary.Add(property.Name, value);
            }

            return dictionary;
        }
    }
}
