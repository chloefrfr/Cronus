using Larry.Source.Classes.MCP.RequestBody;
using Larry.Source.Classes.MCP.Response;
using Larry.Source.Database.Entities;
using Larry.Source.Repositories;
using Larry.Source.Utilities;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Diagnostics;
using Larry.Source.Responses;

namespace Larry.Source.Controllers.Profile.Operations
{
    public class MarkItemSeen
    {
        public static async Task<BaseResponse> Init(string accountId, string profileId, MITSRequestBody body)
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
            var itemIds = body.ItemIds;
            
            foreach (var itemId in itemIds)
            {
                var item = await itemsRepository.FindByTemplateIdAsync(itemId);
                if (item == null)
                {
                    Logger.Error($"Item '{itemId}' was not found.");
                    return new BaseResponse();
                }

                var deserializedValue = JsonConvert.DeserializeObject<dynamic>(item.Value) ?? new JObject();

                deserializedValue.item_seen = true;

                item.Value = JsonConvert.SerializeObject(deserializedValue);

                await itemsRepository.UpdateAsync(item);

                profileChanges.Add(new
                {
                    changeType = "itemAttrChanged",
                    itemId = itemId,
                    attributeName = "item_seen",
                    attributeValue = deserializedValue.item_seen
                });
            }

            if (profileChanges.Count > 0)
            {
                profile.Revision++;
                await profilesRepository.UpdateAsync(profile);
            }

            return MCPResponses.Generate(profile, profileChanges, profileId);
        }
    }
}
