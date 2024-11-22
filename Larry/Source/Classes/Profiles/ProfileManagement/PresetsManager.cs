using Larry.Source.Classes.MCP;
using Larry.Source.Classes.Profile;
using Larry.Source.Database.Entities;
using Larry.Source.Interfaces;
using Larry.Source.Utilities;

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
                { "Backpack", CreateEmptySlot() },
                { "Character", CreateSlot(new[] { DefaultCharacter }) },
                { "Dance", CreateSlot(new string[6]) },
                { "Glider", CreateSlot(new[] { DefaultGlider }) },
                { "ItemWrap", CreateSlot(new string[7]) },
                { "LoadingScreen", CreateEmptySlot() },
                { "MusicPack", CreateEmptySlot() },
                { "Pickaxe", CreateSlot(new[] { DefaultPickaxe }) },
                { "SkyDiveContrail", CreateEmptySlot() }
            };
        }

        /// <summary>
        /// Creates a locker slot with the specified items.
        /// </summary>
        /// <param name="items">An array of item IDs to include in the slot.</param>
        /// <returns>A <see cref="LockerSlot"/> with the specified items.</returns>
        private LockerSlot CreateSlot(string[] items)
        {
            return new LockerSlot
            {
                activeVariants = new List<ActiveVariant>(items.Length),
                items = new List<string>(items)
            };
        }

        /// <summary>
        /// Creates an empty locker slot with no items and no active variants.
        /// </summary>
        /// <returns>An empty <see cref="LockerSlot"/>.</returns>
        private LockerSlot CreateEmptySlot()
        {
            return new LockerSlot
            {
                activeVariants = new List<ActiveVariant>(),
                items = new List<string>()
            };
        }
    }
}
