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
using System.Text.Json;

namespace Larry.Source.Controllers.Profile.Operations
{
    public class QueryProfile
    {
        public static async Task<BaseResponse> Init(string accountId, string profileId)
        {
            string timestamp = DateTime.UtcNow.ToString("o");

            Config config = Config.GetConfig();

            Repository<User> userRepository = new Repository<User>(config.ConnectionUrl);
            Repository<Profiles> profilesRepository = new Repository<Profiles>(config.ConnectionUrl);

            var user = await userRepository.FindByAccountIdAsync(accountId);

            if (user == null)
            {
                return new BaseResponse();
            }

            List<object> applyProfileChanges = new List<object>();


            Console.WriteLine(profileId);
            var profile = await profilesRepository.FindByProfileIdAndAccountIdAsync(profileId, user.AccountId);
            if (profile == null && profileId == "common_public") {
                var profileChanges = new List<object>
                {
                    new
                    {
                        changeType = "fullProfileUpdate",
                        profile = new MCPProfile
                        {
                            created = DateTime.UtcNow.ToString("o"),
                            updated = DateTime.UtcNow.ToString("o"),
                            rvn = 0,
                            wipeNumber = 1,
                            accountId = accountId,
                            profileId = "common_public",
                            version = "no_version",
                            stats = new StatsAttributes(),
                            items = new Dictionary<Guid, Classes.MCP.ItemDefinition>(),
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
                // TODO: Add PastSeasons
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
                            created = DateTime.UtcNow.ToString("o"),
                            updated = DateTime.UtcNow.ToString("o"),
                            rvn = 0,
                            wipeNumber = 1,
                            accountId = accountId,
                            profileId = "common_public",
                            version = "no_version",
                            stats = new StatsAttributes(),
                            items = new Dictionary<Guid, Classes.MCP.ItemDefinition>(),
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

            var profileData = await ProfileManager.GetProfileAsync(user.AccountId, profileId);

            applyProfileChanges.Add(new
            {
                changeType = "fullProfileUpdate",
                profile = profileData
            });

            return MCPResponses.Generate(profile, applyProfileChanges, profileId);
        }
    }
}
