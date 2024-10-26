using Larry.Source.Classes.MCP;
using Larry.Source.Classes.MCP.RequestBody;
using Larry.Source.Classes.MCP.Response;
using Larry.Source.Classes.Profile;
using Larry.Source.Classes.Profiles;
using Larry.Source.Database.Entities;
using Larry.Source.Repositories;
using Larry.Source.Responses;
using Larry.Source.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Larry.Source.Controllers.Profile.Operations
{
    public class EquipBattleRoyaleCustomization
    {
        public static async ValueTask<BaseResponse> Init(string accountId, string profileId, EquipRequestBody body)
        {
            var stopwatch = Stopwatch.StartNew();
            var config = Config.GetConfig();

            var userRepository = new Repository<User>(config.ConnectionUrl);
            var profilesRepository = new Repository<Profiles>(config.ConnectionUrl);
            var itemsRepository = new Repository<Items>(config.ConnectionUrl);

            var userTask = userRepository.FindByAccountIdAsync(accountId);
            var profileTask = profilesRepository.FindByProfileIdAndAccountIdAsync(profileId, accountId);
            var itemTask = itemsRepository.FindByTemplateIdAsync(body.ItemToSlot.Replace("item:", ""));

            await Task.WhenAll(userTask, profileTask, itemTask);

            var user = await userTask;
            var profile = await profileTask;
            var item = await itemTask;

            if (user == null || profile == null)
            {
                Logger.Information($"Execution time {stopwatch.ElapsedMilliseconds} ms");
                return new BaseResponse();
            }

            var profileChanges = new ConcurrentBag<object>();

            if (item != null)
            {
                var statKey = GetStatKey(body.SlotName, body.IndexWithinSlot);
                if (statKey != null)
                {
                    var statItem = await itemsRepository.FindByTemplateIdAsync(statKey);

                    if (statItem?.IsStat == true)
                    {
                        UpdateStatItem(body, item, statItem, profileChanges);
                        await itemsRepository.UpdateAsync(statItem); 
                    }
                }
            }
            var variantUpdates = body.VariantUpdates;
            if (variantUpdates != null)
            {
                await Task.WhenAll(variantUpdates.Select(async variant =>
                {
                    var channel = variant.channel.ToString();
                    var variantActive = variant.active;
                    var variantOwned = variant.owned;

                    var deserializedValue = JsonConvert.DeserializeObject<ItemValue>(item.Value) ?? new ItemValue();
                    if (deserializedValue.variants == null || deserializedValue.variants.Count == 0)
                    {
                        deserializedValue.variants = new List<Variants>();
                    }
                    var variants = deserializedValue.variants ?? new List<Variants>();
                    var variantIndex = variants.IndexOf(variants.FirstOrDefault(v => v.channel?.ToString() == channel));

                    if (variantIndex == -1)
                    {
                        variants.Add(new Variants
                        {
                            channel = channel,
                            active = variantActive,
                            owned = variantOwned
                        });
                    } else
                    {
                        variants[variantIndex].active = variantActive;
                    }

                    deserializedValue.variants = variants;
                    item.Value = JsonConvert.SerializeObject(deserializedValue);
                    await itemsRepository.UpdateAsync(item);

                    profileChanges.Add(new
                    {
                        changeType = "itemAttrChanged",
                        itemId = item.TemplateId,
                        attributeName = "variants",
                        attributeValue = variants
                    });
                }));
            }

            if (profileChanges.Count > 0)
            {
                profile.Revision++;
                await profilesRepository.UpdateAsync(profile);
            }

            Logger.Information($"Execution time {stopwatch.ElapsedMilliseconds} ms");
            return MCPResponses.Generate(profile, profileChanges, profileId);
        }


        private static string GetStatKey(string slotName, int? indexWithinSlot)
        {
            return slotName switch
            {
                "Dance" when indexWithinSlot is >= 0 and <= 6 => "favorite_dance",
                "ItemWrap" => "favorite_itemwraps",
                _ when new[] { "Character", "Backpack", "Pickaxe", "Glider", "SkyDiveContrail", "MusicPack", "LoadingScreen" }
                    .Contains(slotName) => $"favorite_{slotName.ToLower()}",
                _ => null
            };
        }

        private static void UpdateStatItem(EquipRequestBody body, Items item, Items statItem, ConcurrentBag<object> profileChanges)
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
                    valueArray = Enumerable.Repeat(item.TemplateId ?? "", 7).ToArray();
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
                        value = item.TemplateId
                    });
                    break;
            }
        }
    }
}
