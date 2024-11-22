using Larry.Source.Classes.MCP;
using Larry.Source.Classes.Profile;
using Larry.Source.Database.Entities;
using Larry.Source.Interfaces;
using Larry.Source.Utilities;
using static SharpGLTF.Scenes.LightBuilder;

namespace Larry.Source.Classes.Profiles.ProfileManagement
{
    public class PresetsManager
    {
        private const string DefaultLockerName = "PRESET 1";
        private const string DefaultBannerColor = "DefaultColor1";
        private const string DefaultBannerIcon = "StandardBanner1";
        private const string DefaultCharacter = "AthenaCharacter:CID_001_Athena_Commando_F_Default";
        private const string DefaultPickaxe = "AthenaPickaxe:DefaultPickaxe";
        private const string DefaultGlider = "AthenaGlider:DefaultGlider";

        private readonly string _loadoutId;
        private StatsAttributes _stats;
        public Items _items;


        /// <summary>
        /// Initializes a new instance of the <see cref="PresetsManager"/> class.
        /// </summary>
        /// <param name="stats">The profile's stats.</param>
        /// <param name="items">The items associated with the loadout.</param>
        /// <param name="loadoutId">The ID of the loadout to be managed.</param>
        /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
        public PresetsManager(StatsAttributes stats, Items items, string loadoutId)
        {
            _loadoutId = loadoutId ?? throw new ArgumentNullException(nameof(loadoutId));
            _stats = stats ?? throw new ArgumentNullException(nameof(stats));
            _items = items ?? throw new ArgumentNullException(nameof(items));
        }

        /// <summary>
        /// Gets the loadout ID.
        /// </summary>
        /// <returns>The ID of the current loadout.</returns>
        public string GetLoadoutId() => _loadoutId;

        /// <summary>
        /// Adds a loadout to the profile if it doesn't already exist.
        /// </summary>
        /// <param name="accountId">The account ID to associate the loadout with.</param>
        public void AddLoadout(string accountId)
        {

            if (!_stats.attributes.loadouts.Contains(_loadoutId))
            {
                _stats.attributes.loadouts.Add(_loadoutId);
                _stats.attributes.last_applied_loadout = _loadoutId;
            }
            else
            {
                Logger.Warning($"Loadout with ID {_loadoutId} already exists.");
            }
        }

        /// <summary>
        /// Creates a template for the loadout to be stored in the profile.
        /// </summary>
        /// <returns>A new <see cref="ItemValue"/> containing the default loadout settings.</returns>
        public ItemValue CreateLoadoutTemplate()
        {
            return new ItemValue
            {
                banner_color_template = DefaultBannerColor,
                banner_icon_template = DefaultBannerIcon,
                item_seen = true,
                locker_name = DefaultLockerName,
                locker_slots_data = new MCP.LockerSlotData
                {
                    slots = CreateDefaultSlots()
                }
            };
        }

        /// <summary>
        /// Creates the default slots for the loadout, such as character, backpack, and glider.
        /// </summary>
        /// <returns>A dictionary representing the default slots for the loadout.</returns>
        private Dictionary<string, LockerSlot> CreateDefaultSlots()
        {
            return new Dictionary<string, LockerSlot>
            {
                { "Pickaxe", new LockerSlot { items = new List<string> { "AthenaPickaxe:DefaultPickaxe" }, activeVariants = new List<ActiveVariant>() } },
                { "Dance", new LockerSlot { items = new List<string> { "AthenaDance:EID_BoogieDown", "AthenaDance:EID_DanceMoves", "", "", "", "" }, activeVariants = new List<ActiveVariant>() } },
                { "Glider", new LockerSlot { items = new List<string> { "AthenaGlider:DefaultGlider" }, activeVariants = new List<ActiveVariant>() } },
                { "Character", new LockerSlot { items = new List<string> { "AthenaCharacter:CID_001_Athena_Commando_F_Default" }, activeVariants = new List<ActiveVariant> { new ActiveVariant { variants = new List<Variants>() } } } },
                { "Backpack", new LockerSlot { items = new List<string> { "" }, activeVariants = new List<ActiveVariant> { new ActiveVariant { variants = new List<Variants>() } } } },
                { "ItemWrap", new LockerSlot { items = new List<string> { "", "", "", "", "", "", "" }, activeVariants = new List<ActiveVariant> { null, null, null, null, null, null, null } } },
                { "LoadingScreen", new LockerSlot { items = new List<string> { "" }, activeVariants = new List<ActiveVariant> { null } } },
                { "MusicPack", new LockerSlot { items = new List<string> { "" }, activeVariants = new List<ActiveVariant> { null } } },
                { "SkyDiveContrail", new LockerSlot { items = new List<string> { "" }, activeVariants = new List<ActiveVariant> { null } } }
            };
        }
    }
}
