using Larry.Source.Classes.MCP.RequestBody;
using Larry.Source.Classes.MCP.Response;
using Larry.Source.Database.Entities;
using Larry.Source.Repositories;
using Larry.Source.Utilities;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Larry.Source.Responses;

namespace Larry.Source.Controllers.Profile.Operations
{
    public class SetMtxPlatform
    {
        public static async Task<BaseResponse> Init(string accountId, string profileId, MtxPlatformBody body)
        {
            var stopwatch = Stopwatch.StartNew();
            var config = Config.GetConfig();

            var userRepository = new Repository<User>(config.ConnectionUrl);
            var profilesRepository = new Repository<Profiles>(config.ConnectionUrl);
            var itemsRepository = new Repository<Items>(config.ConnectionUrl);

            var userTask = userRepository.FindByAccountIdAsync(accountId);
            var profileTask = profilesRepository.FindByProfileIdAndAccountIdAsync(profileId, accountId);

            await Task.WhenAll(userTask, profileTask);

            var user = userTask.Result;
            if (user == null)
            {
                Logger.Information($"Execution time {stopwatch.ElapsedMilliseconds} ms");
                return new BaseResponse();
            }

            var profile = profileTask.Result;
            if (profile == null)
            {
                Logger.Information($"Execution time {stopwatch.ElapsedMilliseconds} ms");
                return new BaseResponse();
            }

            var profileChanges = new List<object>();
            var newPlatform = body.NewPlatform ?? "EpicPC";

            var allItems = await itemsRepository.GetAllItemsByAccountIdAsync(accountId, profileId);
            if (allItems == null)
            {
                return new BaseResponse();
            }

            var itemUpdateTasks = new List<Task>();
            foreach (var item in allItems)
            {
                var deserializedValue = JObject.Parse(item.Value ?? "{}");
                deserializedValue["platform"] = newPlatform;

                if (item.IsStat && item.TemplateId == "current_mtx_platform")
                {
                    item.Value = newPlatform;
                }

                itemUpdateTasks.Add(itemsRepository.UpdateAsync(item));

                profileChanges.Add(new
                {
                    changeType = "statModified",
                    name = item.TemplateId,
                    value = newPlatform
                });
            }

            await Task.WhenAll(itemUpdateTasks);

            if (profileChanges.Count > 0)
            {
                profile.Revision++;
                await profilesRepository.UpdateAsync(profile);
            }

            Logger.Information($"Execution time {stopwatch.ElapsedMilliseconds} ms");

            return MCPResponses.Generate(profile, profileChanges, profileId);
        }
    }
}
