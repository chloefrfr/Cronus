using Larry.Source.Classes.MCP;
using Larry.Source.Classes.Profile;
using Larry.Source.Database.Entities;
using Larry.Source.Utilities;
using System.Text.Json;

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

        private static List<string> GetItemsForSlot(string dbId, List<Loadouts> loadouts, int maxItems)
        {
            var items = loadouts
                .Where(item => item.GetType().GetProperty(dbId)?.GetValue(item) != null)
                .Select(item =>
                {
                    var value = item.GetType().GetProperty(dbId)?.GetValue(item);
                    if (value is IEnumerable<object> collection)
                    {
                        return collection.Select(v => v?.ToString()).ToList();
                    }
                    return new List<string> { value?.ToString() };
                })
                .Where(value => value.Any(v => !string.IsNullOrEmpty(v)))
                .Take(maxItems)
                .SelectMany(value => value)
                .Take(maxItems)
                .ToList();

            return items.Concat(Enumerable.Repeat<string>(null, maxItems - items.Count)).Take(maxItems).ToList();
        }

        public static Dictionary<string, ItemDefinition> Build(List<Loadouts> loadouts)
        {
            var lockerLoadout = new Dictionary<string, ItemDefinition>();

            foreach (var loadout in loadouts)
            {
                var slots = new[] { "Pickaxe", "Dance", "Glider", "Character", "Backpack", "ItemWrap", "LoadingScreen", "SkyDiveContrail", "MusicPack" };

                var slotsResult = slots.Aggregate(new Dictionary<string, LockerSlot>(), (acc, key) =>
                {
                    var dbId = GetDatabaseId(key);
                    if (string.IsNullOrEmpty(dbId))
                    {
                        Logger.Error($"No database ID found for slot: {key}");
                        return acc;
                    }

                    List<string> items;
                    if (key == "Dance")
                    {
                        items = GetItemsForSlot(dbId, loadouts, 6);
                    }
                    else if (key == "ItemWrap")
                    {
                        items = GetItemsForSlot(dbId, loadouts, 7);
                    }
                    else
                    {
                        items = loadouts
                            .Where(item => item.GetType().GetProperty(dbId)?.GetValue(item) != null)
                            .Select(item => item.GetType().GetProperty(dbId)?.GetValue(item)?.ToString())
                            .Where(value => !string.IsNullOrEmpty(value))
                            .ToList();
                    }

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
                        locker_slots_data = new LockerSlotData
                        {
                            slots = slotsResult
                        },
                        use_count = 0,
                        banner_color_template = loadout.BannerColorId,
                        banner_icon_template = loadout.BannerId,
                        locker_name = loadout.LockerName,
                        item_seen = false,
                        favorite = false
                    },
                    quantity = 1
                };
            }

            return lockerLoadout;
        }
    }
}
