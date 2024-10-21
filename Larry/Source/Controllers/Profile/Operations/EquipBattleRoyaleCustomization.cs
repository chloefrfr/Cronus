using Larry.Source.Classes.MCP.RequestBody;
using Larry.Source.Classes.MCP.Response;
using Larry.Source.Classes.Profile;
using Larry.Source.Classes.Profiles;
using Larry.Source.Database.Entities;
using Larry.Source.Repositories;
using Larry.Source.Responses;
using Larry.Source.Utilities;
using Larry.Source.Utilities.Managers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace Larry.Source.Controllers.Profile.Operations
{
    public class EquipBattleRoyaleCustomization
    {
        public static async Task<BaseResponse> Init(string accountId, string profileId, EquipRequestBody body)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            var timestamp = DateTime.UtcNow.ToString("o");
            var config = Config.GetConfig();

            var userRepository = new Repository<User>(config.ConnectionUrl);
            var profilesRepository = new Repository<Profiles>(config.ConnectionUrl);
            var itemsRepository = new Repository<Items>(config.ConnectionUrl);

            var user = await userRepository.FindByAccountIdAsync(accountId);
            if (user == null)
            {
                Logger.Information($"Execution time {stopwatch.ElapsedMilliseconds} ms");
                return new BaseResponse();
            }

            var profile = await profilesRepository.FindByProfileIdAndAccountIdAsync(profileId, user.AccountId);
            if (profile == null)
            {
                Logger.Information($"Execution time {stopwatch.ElapsedMilliseconds} ms");
                return new BaseResponse();
            }

            var profileChanges = new List<object>();
            var items = await itemsRepository.GetAllItemsByAccountIdAsync(user.AccountId, profileId);

            await UpdateProfileItemStatsAsync(body, user, profileId, items, itemsRepository, profileChanges);

            if (profileChanges.Count > 0)
            {
                profile.Revision++;
                await profilesRepository.SaveAsync(profile);
            }

            var athenaProfile = new AthenaProfile(user.AccountId, items, profile).GetProfile();
            var changes = new List<object>();

            if (profileChanges.Count == 0)
            {
                changes.Add(new
                {
                    changeType = "fullProfileUpdate",
                    profile = athenaProfile
                });
                
            }

            Logger.Information($"Execution time {stopwatch.ElapsedMilliseconds} ms");
            return MCPResponses.Generate(profile, profileChanges.Count > 0 ? profileChanges : changes, profileId);
        }

        private static async Task UpdateProfileItemStatsAsync(EquipRequestBody body, User user, string profileId, IEnumerable<Items> items, Repository<Items> itemsRepository, List<object> profileChanges)
        {
            string itemSlotFixed = body.ItemToSlot.Replace("item:", "");
            var item = await itemsRepository.FindByTemplateIdAsync(body.ItemToSlot);

            if (item == null)
            {
                item = await itemsRepository.FindByTemplateIdAsync(body.ItemToSlot);
                if (item == null) return;
            }

            await UpdateStatBasedOnSlotAsync(body, user.AccountId, profileId, item, itemsRepository, profileChanges);
        }

        private static async Task UpdateStatBasedOnSlotAsync(EquipRequestBody body, string accountId, string profileId, Items item, Repository<Items> itemsRepository, List<object> profileChanges)
        {
            string statKey = GetStatKey(body.SlotName, body.IndexWithinSlot);
            if (statKey == null) return;

            var statItem = await itemsRepository.FindByTemplateIdAsync(statKey);
            if (statItem?.IsStat == true)
            {
                string value = GetUpdatedValue(body, statItem, item.TemplateId);
                statItem.Value = value;

                await itemsRepository.UpdateAsync(statItem);

                profileChanges.Add(new
                {
                    changeType = "statModified",
                    name = statKey,
                    value = value
                });
            }
        }

        private static string GetStatKey(string slotName, int? indexWithinSlot)
        {
            return slotName switch
            {
                "Dance" when indexWithinSlot >= 0 && indexWithinSlot <= 5 => "favorite_dance",
                "ItemWrap" => "favorite_itemwraps",
                _ when new[] { "Character", "Backpack", "Pickaxe", "Glider", "SkyDiveContrail", "MusicPack", "LoadingScreen" }.Contains(slotName) =>
                    $"favorite_{slotName.ToLower()}",
                _ => null
            };
        }

        private static string GetUpdatedValue(EquipRequestBody body, Items statItem, string templateId)
        {
            if (body.SlotName == "ItemWrap" && (body.IndexWithinSlot == -1 || body.IndexWithinSlot <= 7))
            {
                return string.Join(",", new string[7].Select(_ => templateId ?? ""));
            }


            if (body.SlotName == "Dance" && body.IndexWithinSlot is int index)
            {
                string[] values = (statItem.Value as string)?.Split(',') ?? new string[6];

                if (values.Length < 6)
                {
                    Array.Resize(ref values, 6);  
                }

                if (index >= 0 && index < 6)
                {
                    values[index] = templateId ?? ""; 
                }

                return string.Join(",", values);
            }

            return templateId;
        }
    }
}
