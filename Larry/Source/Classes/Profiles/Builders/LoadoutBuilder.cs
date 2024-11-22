using Larry.Source.Classes.MCP;
using Larry.Source.Classes.Profile;
using Larry.Source.Database.Entities;
using Larry.Source.Utilities;

namespace Larry.Source.Classes.Profiles.Builders
{
    public class LoadoutBuilder
    {
        private static readonly Dictionary<string, string> databaseSlots = new Dictionary<string, string>()
        {
            { "Character", "CharacterId" },
            { "Backpack", "BackpackId" },
            { "Pickaxe", "PickaxeId" },
            { "Glider", "GliderId" },
            { "SkyDiveContrail", "ContrailId" },
            { "MusicPack", "MusicPackId" },
            { "LoadingScreen", "LoadingScreenId" },
            { "Dance", "DanceId" },
            { "ItemWrap", "ItemWrapId" }
        };

        public static string GetDatabaseId(string slotName)
        {
            return databaseSlots.TryGetValue(slotName, out var databaseId) ? databaseId : null;
        }

        public static Dictionary<string, ItemDefinition> Build(List<Loadouts> loadouts)
        {
            var lockerLoadout = new Dictionary<string, ItemDefinition>();

            foreach (var loadout in loadouts)
            {
                var slots = new[] { "Pickaxe", "Dance", "Glider", "Character", "Backpack", "ItemWrap", "LoadingScreen", "SkyDiveContrail", "MusicPack" };

                var slotsResult = slots.Aggregate(new Dictionary<string, LockerSlot>(), (acc, key) =>
                {
                    var dbId = LoadoutBuilder.GetDatabaseId(key);
                    if (dbId == null)
                    {
                        Logger.Error($"No database ID found for slot: {key}");
                        return acc;
                    }

                    var items = loadouts
                        .Where(item => item.GetType().GetProperty(dbId)?.GetValue(item) != null) 
                        .Select(item => item.GetType().GetProperty(dbId)?.GetValue(item)?.ToString()) 
                        .Where(value => !string.IsNullOrEmpty(value))
                        .ToList();

                    if (key == "ItemWrap")
                        items = Enumerable.Repeat(items, 7).SelectMany(x => x).ToList();
                    else if (key == "Dance")
                        items = Enumerable.Repeat(items, 6).SelectMany(x => x).ToList();

                    var newItemSlot = new LockerSlot
                    {
                        items = items,
                        activeVariants = null 
                    };

                    acc[key] = newItemSlot;
                    return acc;
                });

                lockerLoadout[loadout.TemplateId] = new ItemDefinition
                {
                    templateId = loadout.TemplateId,
                    attributes = new ItemValue
                    {
                        use_count = 0,
                        banner_color_template = loadout.BannerColorId,
                        banner_icon_template = loadout.BannerId,
                        locker_name = loadout.LockerName,
                        item_seen = true,
                        favorite = false,
                        locker_slots_data = new LockerSlotData
                        {
                            slots = slotsResult
                        }
                    },
                    quantity = 1
                };
            }

            return lockerLoadout;
        }
    }
}
