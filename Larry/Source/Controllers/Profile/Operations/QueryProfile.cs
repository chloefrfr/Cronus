using Discord.Net;
using Larry.Source.Classes.MCP.Response;
using Larry.Source.Classes.Profile;
using Larry.Source.Database.Entities;
using Larry.Source.Interfaces;
using Larry.Source.Repositories;
using Larry.Source.Responses;
using Larry.Source.Utilities;
using Larry.Source.Utilities.Managers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using Larry.Source.Utilities.Parsers;
using System.Threading.Tasks;

namespace Larry.Source.Controllers.Profile.Operations
{
    public class QueryProfile
    {
        public static async Task<BaseResponse> Init(string accountId, string profileId, string userAgent)
        {
            var timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            var config = Config.GetConfig();

            var userRepository = new Repository<User>(config.ConnectionUrl);
            var profilesRepository = new Repository<Profiles>(config.ConnectionUrl);
            var uaHelper = UserAgentParser.Parse(userAgent);

            var userTask = userRepository.FindByAccountIdAsync(accountId);
            var profileTask = profilesRepository.FindByProfileIdAndAccountIdAsync(profileId, accountId);

            await Task.WhenAll(userTask, profileTask);

            var user = userTask.Result;
            var profile = profileTask.Result;

            if (user == null)
            {
                return new BaseResponse();
            }

            List<object> applyProfileChanges = new List<object>();

            if (profile == null && profileId == "common_public")
            {
                var profileChanges = new List<object>
                {
                    new
                    {
                        changeType = "fullProfileUpdate",
                        profile = new MCPProfile
                        {
                            created = timestamp,
                            updated = timestamp,
                            rvn = 0,
                            wipeNumber = 1,
                            accountId = accountId,
                            profileId = "common_public",
                            version = "no_version",
                            stats = new StatsAttributes(),
                            items = new Dictionary<string, Classes.MCP.ItemDefinition>(),
                            commandRevision = 0
                        }
                    }
                };

                return MCPResponses.Generate(new Profiles
                {
                    AccountId = accountId,
                    ProfileId = "common_public",
                    Revision = 0
                }, profileChanges, profileId);
            }

            if (profile.ProfileId == "athena")
            {
                var itemsRepository = new Repository<Items>(config.ConnectionUrl);
                var itemsTask = itemsRepository.GetAllItemsByAccountIdAsync(accountId, profileId);

                var items = await itemsTask;

                var itemProcessingTasks = items.Where(item => item.IsStat && item.TemplateId == "season_num")
                                               .Select(async item =>
                                               {
                                                   try
                                                   {
                                                       var deserializedValue = JsonSerializer.Deserialize<dynamic>(item.Value) ?? new JObject();
                                                       deserializedValue = uaHelper.Season.ToString();
                                                       item.Value = JsonSerializer.Serialize(deserializedValue);

                                                       await itemsRepository.UpdateAsync(item);
                                                       Logger.Information($"Updated season for item: {item.TemplateId}, New Value: {item.Value}");
                                                   }
                                                   catch (Exception ex)
                                                   {
                                                       Logger.Error($"Error updating item (TemplateId: {item.TemplateId}): {ex.Message}");
                                                   }
                                               }).ToArray();

                await Task.WhenAll(itemProcessingTasks);
            }
            else if (profile.ProfileId == "common_core")
            {
            }
            else if (profileId == "common_public")
            {
                var profileChanges = new List<object>
                {
                    new
                    {
                        changeType = "fullProfileUpdate",
                        profile = new MCPProfile
                        {
                            created = timestamp,
                            updated = timestamp,
                            rvn = 0,
                            wipeNumber = 1,
                            accountId = accountId,
                            profileId = "common_public",
                            version = "no_version",
                            stats = new StatsAttributes(),
                            items = new Dictionary<string, Classes.MCP.ItemDefinition>(),
                            commandRevision = 0
                        }
                    }
                };

                return MCPResponses.Generate(new Profiles
                {
                    AccountId = accountId,
                    ProfileId = "common_public",
                    Revision = 0
                }, profileChanges, profileId);
            }

            var profileDataTask = ProfileManager.GetProfileAsync(user.AccountId, profileId);
            var profileData = await profileDataTask;

            applyProfileChanges.Add(new
            {
                changeType = "fullProfileUpdate",
                profile = profileData
            });

            return MCPResponses.Generate(profile, applyProfileChanges, profileId);
        }
    }
}
