using Larry.Source.Classes.MCP;
using Newtonsoft.Json;

namespace Larry.Source.Classes.Profile
{
    public class ItemValue
    {
        [JsonProperty("platform")]
        public string Platform { get; set; }

        [JsonProperty("gender")]
        public string? Gender { get; set; }

        [JsonProperty("personality")]
        public string? Personality { get; set; }

        [JsonProperty("fromAccountId")]
        public string? FromAccountId { get; set; }

        [JsonProperty("time")]
        public string? Time { get; set; }

        [JsonProperty("squad_slot_idx")]
        public int? SquadSlotIndex { get; set; }

        [JsonProperty("userMessage")]
        public string? UserMessage { get; set; }

        [JsonProperty("templateIdHashed")]
        public string? TemplateIdHashed { get; set; }

        [JsonProperty("quest_state")]
        public string? QuestState { get; set; }

        [JsonProperty("portrait")]
        public string? Portrait { get; set; }

        [JsonProperty("building_slot_used")]
        public int? BuildingSlotUsed { get; set; }

        [JsonProperty("set_bonus")]
        public string? SetBonus { get; set; }

        [JsonProperty("sent_new_notification")]
        public bool SentNewNotification { get; set; }

        [JsonProperty("ObjectiveState")]
        public List<ObjectiveState> ObjectiveState { get; set; }

        [JsonProperty("lootList")]
        public List<object>? LootList { get; set; }

        [JsonProperty("alterationDefinitions")]
        public List<object>? AlterationDefinitions { get; set; }

        [JsonProperty("baseClipSize")]
        public int? BaseClipSize { get; set; }

        [JsonProperty("item_seen")]
        public bool ItemSeen { get; set; }

        [JsonProperty("refundsUsed")]
        public int? RefundsUsed { get; set; }

        [JsonProperty("refundCredits")]
        public int? RefundCredits { get; set; }

        [JsonProperty("params")]
        public GiftParameters Params { get; set; }

        [JsonProperty("sectionStates")]
        public List<object>? SectionStates { get; set; }

        [JsonProperty("state")]
        public string? State { get; set; }

        [JsonProperty("cloud_save_info")]
        public CloudSaveInfo? CloudSaveInfo { get; set; }

        [JsonProperty("outpost_core_info")]
        public OutpostCoreInfo? OutpostCoreInfo { get; set; }

        [JsonProperty("tier_progression")]
        public TierProgression? TierProgression { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("max_level_bonus")]
        public int MaxLevelBonus { get; set; }

        [JsonProperty("rnd_sel_cnt")]
        public int RndSelCnt { get; set; }

        [JsonProperty("legacy_alterations")]
        public List<object>? LegacyAlterations { get; set; }

        [JsonProperty("refund_legacy_item")]
        public bool RefundLegacyItem { get; set; }

        [JsonProperty("alterations")]
        public List<object>? Alterations { get; set; }

        [JsonProperty("refundable")]
        public bool? Refundable { get; set; }

        [JsonProperty("alteration_base_rarities")]
        public List<object>? AlterationBaseRarities { get; set; }

        [JsonProperty("homebaseBannerColorId")]
        public string? HomebaseBannerColorId { get; set; }

        [JsonProperty("loadedAmmo")]
        public int? LoadedAmmo { get; set; }

        [JsonProperty("durability")]
        public int? Durability { get; set; }

        [JsonProperty("inventory_overflow_date")]
        public bool? InventoryOverflowDate { get; set; }

        [JsonProperty("itemSource")]
        public string? ItemSource { get; set; }

        [JsonProperty("quest_complete_playsolo")]
        public int? QuestCompletePlaySolo { get; set; }

        [JsonProperty("quest_unlock_researchgadgets")]
        public int? QuestUnlockResearchGadgets { get; set; }

        [JsonProperty("current_mtx_currency")]
        public int? CurrentMtxCurrency { get; set; }

        [JsonProperty("squad_slots_unlocked")]
        public int? SquadSlotsUnlocked { get; set; }

        [JsonProperty("quest_unlock_buildweapons")]
        public int? QuestUnlockBuildWeapons { get; set; }

        [JsonProperty("current_fort_profile_banner")]
        public string? CurrentFortProfileBanner { get; set; }

        [JsonProperty("quest_unlock_defenders")]
        public int? QuestUnlockDefenders { get; set; }

        [JsonProperty("commander_level")]
        public int? CommanderLevel { get; set; }

        [JsonProperty("time_played")]
        public int? TimePlayed { get; set; }

        [JsonProperty("squad_slot_count")]
        public int? SquadSlotCount { get; set; }

        [JsonProperty("quest_unlock_eventquest")]
        public int? QuestUnlockEventQuest { get; set; }

        [JsonProperty("quest_unlock_missiondefender")]
        public int? QuestUnlockMissionDefender { get; set; }

        [JsonProperty("homebaseBannerIconId")]
        public string? HomebaseBannerIconId { get; set; }

        [JsonProperty("squad_slot_count_1")]
        public int? SquadSlotCount1 { get; set; }

        [JsonProperty("accountLevel")]
        public int AccountLevel { get; set; }

        [JsonProperty("squad_slot_count_2")]
        public int? SquadSlotCount2 { get; set; }

        [JsonProperty("daily_rewards_claimed")]
        public int? DailyRewardsClaimed { get; set; }

        [JsonProperty("seasonal_gold")]
        public int? SeasonalGold { get; set; }

        [JsonProperty("quest_unlock_craftweapons")]
        public int? QuestUnlockCraftWeapons { get; set; }

        [JsonProperty("quest_unlock_personalassistant")]
        public int? QuestUnlockPersonalAssistant { get; set; }

        [JsonProperty("quest_unlock_upgradeweapon")]
        public int? QuestUnlockUpgradeWeapon { get; set; }

        [JsonProperty("book_level")]
        public int BookLevel { get; set; }

        [JsonProperty("book_xp")]
        public int BookXp { get; set; }

        [JsonProperty("book_sections_completed")]
        public int? BookSectionsCompleted { get; set; }

        [JsonProperty("book_section_idx")]
        public int? BookSectionIndex { get; set; }

        [JsonProperty("xp_overflow")]
        public int XpOverflow { get; set; }

        [JsonProperty("quest_unlock_missionevent")]
        public int QuestUnlockMissionEvent { get; set; }

        [JsonProperty("purchased_slots")]
        public int PurchasedSlots { get; set; }

        [JsonProperty("squad_slot_count_3")]
        public int SquadSlotCount3 { get; set; }

        [JsonProperty("squad_slot_count_4")]
        public int SquadSlotCount4 { get; set; }

        [JsonProperty("homebaseBannerColorIdBattleRoyal")]
        public string HomebaseBannerColorIdBattleRoyal { get; set; }

        [JsonProperty("currentSeason")]
        public int CurrentSeason { get; set; }

        [JsonProperty("homebaseName")]
        public string HomebaseName { get; set; }

        [JsonProperty("variants")]
        public List<Variants> Variants { get; set; }

        [JsonProperty("xp")]
        public int Xp { get; set; }

        [JsonProperty("purchases")]
        public List<object>? Purchases { get; set; }

        [JsonProperty("undo_cooldowns")]
        public List<object>? UndoCooldowns { get; set; }

        [JsonProperty("mtx_affiliate_set_time")]
        public string? MtxAffiliateSetTime { get; set; }

        [JsonProperty("inventory_limit_bonus")]
        public int? InventoryLimitBonus { get; set; }

        [JsonProperty("mtx_affiliate")]
        public string? MtxAffiliate { get; set; }

        [JsonProperty("forced_intro_played")]
        public string? ForcedIntroPlayed { get; set; }

        [JsonProperty("weekly_purchases")]
        public object? WeeklyPurchases { get; set; }

        [JsonProperty("daily_purchases")]
        public object? DailyPurchases { get; set; }

        [JsonProperty("ban_history")]
        public BanHistory? BanHistory { get; set; }

        [JsonProperty("in_app_purchases")]
        public object? InAppPurchases { get; set; }

        [JsonProperty("permissions")]
        public List<object>? Permissions { get; set; }

        [JsonProperty("undo_timeout")]
        public string? UndoTimeout { get; set; }

        [JsonProperty("monthly_purchases")]
        public object? MonthlyPurchases { get; set; }

        [JsonProperty("allowed_to_send_gifts")]
        public bool? AllowedToSendGifts { get; set; }

        [JsonProperty("granted_bundles")]
        public List<string>? GrantedBundles { get; set; }

        [JsonProperty("has_unlock_by_completion")]
        public bool? HasUnlockByCompletion { get; set; }

        [JsonProperty("num_quests_completed")]
        public int? NumQuestsCompleted { get; set; }

        [JsonProperty("max_allowed_bundle_level")]
        public int? MaxAllowedBundleLevel { get; set; }

        [JsonProperty("num_granted_bundle_quests")]
        public int? NumGrantedBundleQuests { get; set; }

        [JsonProperty("challenge_bundle_schedule_id")]
        public string? ChallengeBundleScheduleId { get; set; }

        [JsonProperty("num_progress_quests_completed")]
        public int? NumProgressQuestsCompleted { get; set; }

        [JsonProperty("grantedquestinstanceids")]
        public List<string>? GrantedQuestInstanceIds { get; set; }

        [JsonProperty("allowed_to_receive_gifts")]
        public bool? AllowedToReceiveGifts { get; set; }

        [JsonProperty("gift_history")]
        public object? GiftHistory { get; set; }

        [JsonProperty("gifts")]
        public List<Gifts>? Gifts { get; set; }

        [JsonProperty("banner_color")]
        public string? BannerColor { get; set; }

        [JsonProperty("banner_icon")]
        public string? BannerIcon { get; set; }

        [JsonProperty("ban_status")]
        public BanStatus? BanStatus { get; set; }

        [JsonProperty("survey_data")]
        public object? SurveyData { get; set; }

        [JsonProperty("personal_offers")]
        public object? PersonalOffers { get; set; }

        [JsonProperty("intro_game_played")]
        public bool? IntroGamePlayed { get; set; }

        [JsonProperty("import_friends_claimed")]
        public object? ImportFriendsClaimed { get; set; }

        [JsonProperty("mtx_purchase_history")]
        public MtxPurchaseHistory? MtxPurchaseHistory { get; set; }

        [JsonProperty("locker_slots_data")]
        public LockerSlot LockerSlotsData { get; set; }

        [JsonProperty("use_count")]
        public int UseCount { get; set; }

        [JsonProperty("banner_icon_template")]
        public string BannerIconTemplate { get; set; }

        [JsonProperty("banner_color_template")]
        public string BannerColorTemplate { get; set; }

        [JsonProperty("locker_name")]
        public string LockerName { get; set; }

        [JsonProperty("favorite")]
        public bool Favorite { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

    }
}
