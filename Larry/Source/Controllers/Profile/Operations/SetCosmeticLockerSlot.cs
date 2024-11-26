using Larry.Source.Classes.MCP;
using Larry.Source.Classes.MCP.RequestBody;
using Larry.Source.Classes.MCP.Response;
using Larry.Source.Classes.Profiles.Builders;
using Larry.Source.Database.Entities;
using Larry.Source.Repositories;
using Larry.Source.Responses;
using Larry.Source.Utilities;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;

namespace Larry.Source.Controllers.Profile.Operations
{
    public class SetCosmeticLockerSlot
    {
        public static async ValueTask<BaseResponse> Init(string accountId, string profileId, SetLockerSlotRequestBody body)
        {
            var stopwatch = Stopwatch.StartNew();
            var config = Config.GetConfig();

            var userRepository = new Repository<User>(config.ConnectionUrl);
            var profilesRepository = new Repository<Profiles>(config.ConnectionUrl);
            var itemsRepository = new Repository<Items>(config.ConnectionUrl);
            var loadoutsRepository = new Repository<Loadouts>(config.ConnectionUrl);

            Task<User> userTask = userRepository.FindByAccountIdAsync(accountId);
            Task<Profiles> profileTask = profilesRepository.FindByProfileIdAndAccountIdAsync(profileId, accountId);
            Task<Items> itemTask = itemsRepository.FindByTemplateIdAsync(body.ItemToSlot.Replace("item:", ""));
            Task<Loadouts> loadoutTask = loadoutsRepository.FindByLockerNameAsync(body.LockerItem);
            Task<List<Loadouts>> allLoadoutsTask = loadoutsRepository.GetAllItemsByAccountIdAsync(accountId, profileId);

            await Task.WhenAll(userTask, profileTask, itemTask, loadoutTask, allLoadoutsTask);

            var user = await userTask;
            var profile = await profileTask;
            var item = await itemTask;
            var loadout = await loadoutTask;
            var allLoadouts = await allLoadoutsTask;

            if (user == null || profile == null || item == null || loadout == null)
            {
                Logger.Information($"Execution time: {stopwatch.ElapsedMilliseconds} ms");
                throw new Exception("User, Profile, Item or Loadout not found.");
            }

            var locker = LoadoutBuilder.Build(allLoadouts).FirstOrDefault();

            if (locker.Value?.attributes?.locker_slots_data?.slots?.ContainsKey(body.Category) != true)
            {
                Logger.Information($"Execution time: {stopwatch.ElapsedMilliseconds} ms");
                throw new Exception("Category not found.");
            }

            var lockerData = locker.Value.attributes.locker_slots_data;

            var categoryInDb = LoadoutBuilder.GetDatabaseId(body.Category);

            var categoryToPropertyMap = new Dictionary<string, Action<string>>
            {
               { "CharacterId", item => loadout.CharacterId = item },
               { "BackpackId", item => loadout.BackpackId = item },
               { "PickaxeId", item => loadout.PickaxeId = item },
               { "GliderId", item => loadout.GliderId = item },
               { "ContrailId", item => loadout.ContrailId = item }
            };

            if (categoryToPropertyMap.ContainsKey(categoryInDb))
            {
                lockerData.slots[body.Category].items = new List<string> { body.ItemToSlot };
                categoryToPropertyMap[categoryInDb](body.ItemToSlot);
            }
            else if (categoryInDb == "DanceId")
            {
                var currentDanceItems = loadout.DanceId?.Split(',').ToList() ?? new List<string> { "", "", "", "", "", "" };

                if (body.SlotIndex == -1)
                {
                    currentDanceItems.ForEach(_ => _ = body.ItemToSlot);
                }
                else
                {
                    currentDanceItems[body.SlotIndex] = body.ItemToSlot;
                }

                lockerData.slots[body.Category].items = new List<string>(currentDanceItems);
                loadout.DanceId = string.Join(",", currentDanceItems);
            }
            else if (categoryInDb == "ItemWrapId")
            {
                var currentWrapItems = loadout.ItemWrapId?.Split(',').ToList() ?? new List<string> { "", "", "", "", "", "", "" };

                if (body.SlotIndex == -1)
                {
                    currentWrapItems.ForEach(_ => _ = body.ItemToSlot);
                }
                else
                {
                    currentWrapItems[body.SlotIndex] = body.ItemToSlot;
                }

                lockerData.slots[body.Category].items = new List<string>(currentWrapItems);
                loadout.ItemWrapId = string.Join(",", currentWrapItems);
            }
            else
            {
                throw new InvalidOperationException($"Unknown category: {categoryInDb}");
            }


            var updateLoadoutTask = loadoutsRepository.UpdateAsync(loadout);

            List<object> profileChanges = new List<object>();
            if (profile.Revision != 0)
            {
                profileChanges.Add(new
                {
                    changeType = "itemAttrChanged",
                    itemId = body.LockerItem,
                    attributeName = "locker_slots_data",
                    attributeValue = lockerData
                });
            }

            if (profileChanges.Any())
            {
                profile.Revision++;
                var updateProfileTask = profilesRepository.UpdateAsync(profile);
                await Task.WhenAll(updateLoadoutTask, updateProfileTask);
            }
            else
            {
                await updateLoadoutTask;
            }

            Logger.Information($"Execution time: {stopwatch.ElapsedMilliseconds} ms");
            return MCPResponses.Generate(profile, profileChanges, profileId);
        }
    }
}
