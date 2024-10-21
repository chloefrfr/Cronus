using Larry.Source.Classes.MCP.RequestBody;
using Larry.Source.Classes.MCP.Response;
using Larry.Source.Classes.Profiles;
using Larry.Source.Database.Entities;
using Larry.Source.Repositories;
using Larry.Source.Responses;
using Larry.Source.Utilities;
using System.Diagnostics;

namespace Larry.Source.Controllers.Profile.Operations
{
    public class EquipBattleRoyaleCustomization
    {
        public static async Task<BaseResponse> Init(string accountId, string profileId, EquipRequestBody body)
        {
            var stopwatch = Stopwatch.StartNew();
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

            string itemSlotFixed = body.ItemToSlot.Replace("item:", "");
            var item = await itemsRepository.FindByTemplateIdAsync(body.ItemToSlot)
                         ?? await itemsRepository.FindByTemplateIdAsync(body.ItemToSlot);

            if (item != null)
            {
                string statKey = body.SlotName switch
                {
                    "Dance" when body.IndexWithinSlot is >= 0 and <= 5 => "favorite_dance",
                    "ItemWrap" => "favorite_itemwraps",
                    _ when new[] { "Character", "Backpack", "Pickaxe", "Glider", "SkyDiveContrail", "MusicPack", "LoadingScreen" }
                        .Contains(body.SlotName) => $"favorite_{body.SlotName.ToLower()}",
                    _ => null
                };

                if (statKey != null)
                {
                    var statItem = await itemsRepository.FindByTemplateIdAsync(statKey);
                    if (statItem?.IsStat == true)
                    {
                        string value = body.SlotName switch
                        {
                            "ItemWrap" when body.IndexWithinSlot == -1 || body.IndexWithinSlot <= 7
                                => string.Join(",", new string[7].Select(_ => item.TemplateId ?? "")),

                            "Dance" when body.IndexWithinSlot is int index =>
                                string.Join(",", ((statItem.Value as string)?.Split(',') ?? new string[6])
                                    .Select((v, i) => i == index ? item.TemplateId ?? "" : v)),

                            _ => item.TemplateId
                        };

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
            }

            if (profileChanges.Count > 0)
            {
                profile.Revision++;
                await profilesRepository.SaveAsync(profile);
            }

            var athenaProfile = new AthenaProfile(user.AccountId, items, profile).GetProfile();
            var changes = profileChanges.Count > 0
                ? profileChanges
                : new List<object> { new { changeType = "fullProfileUpdate", profile = athenaProfile } };

            Logger.Information($"Execution time {stopwatch.ElapsedMilliseconds} ms");
            return MCPResponses.Generate(profile, changes, profileId);
        }
    }
}
