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
                    "Dance" when body.IndexWithinSlot is >= 0 and <= 6 => "favorite_dance",
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
                        string[] valueArray;

                        switch (body.SlotName)
                        {
                            case "Dance":
                                valueArray = (statItem.Value as string)?.Split(',') ?? new string[6];
                                if (body.IndexWithinSlot >= 0 && body.IndexWithinSlot < valueArray.Length)
                                {
                                    valueArray[body.IndexWithinSlot] = item.TemplateId ?? "";
                                }
                                else
                                {
                                    Array.Resize(ref valueArray, body.IndexWithinSlot + 1);
                                    valueArray[body.IndexWithinSlot] = item.TemplateId ?? "";
                                }

                                statItem.Value = string.Join(",", valueArray);
                                profileChanges.Add(new
                                {
                                    changeType = "statModified",
                                    name = "favorite_dance",
                                    value = valueArray
                                });
                                break;

                            case "ItemWrap":
                                valueArray = new string[7];
                                for (int i = 0; i < 7; i++)
                                {
                                    valueArray[i] = item.TemplateId ?? "";
                                }
                                statItem.Value = string.Join(",", valueArray);
                                profileChanges.Add(new
                                {
                                    changeType = "statModified",
                                    name = "favorite_itemwraps",
                                    value = valueArray
                                });
                                break;

                            default:
                                statItem.Value = item.TemplateId;
                                profileChanges.Add(new
                                {
                                    changeType = "statModified",
                                    name = $"favorite_{body.SlotName.ToLower()}",
                                    value= item.TemplateId
                                });
                                break;
                        }

                        await itemsRepository.UpdateAsync(statItem);
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
