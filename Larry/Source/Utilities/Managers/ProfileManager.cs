using Larry.Source.Classes.Profiles;
using Larry.Source.Classes.Profile;
using Larry.Source.Database.Entities;
using Larry.Source.Repositories;
using Larry.Source.QueryBuilder;
using Larry.Source.Mappings;
using System.Diagnostics;
using Newtonsoft.Json;
using Larry.Source.Interfaces;
using Larry.Source.Classes.MCP;
using System.Text.Json;
using System.Reflection;
using Npgsql;
using NpgsqlTypes;
using System.Collections;
using Serilog;

namespace Larry.Source.Utilities.Managers
{
    /// <summary>
    /// Manages user profiles, including creating and retrieving profile data.
    /// </summary>
    public class ProfileManager
    {
        /// <summary>
        /// Asynchronously creates a new profile based on the specified profile type and account ID.
        /// </summary>
        /// <param name="type">The type of the profile to create.</param>
        /// <param name="accountId">The account ID associated with the profile.</param>
        /// <returns>A new instance of <see cref="Profiles"/>.</returns>
        /// <exception cref="Exception">Thrown when the profile type is not found.</exception>
        public static async Task<Profiles> CreateProfileAsync(string type, string accountId)
        {
            var config = Config.GetConfig();
            var profileRepository = new Repository<Profiles>(config.ConnectionUrl);
            var itemsRepository = new Repository<Items>(config.ConnectionUrl);
            var itemAttributesRepository = new Repository<ItemAttributes>(config.ConnectionUrl);

            var newProfile = new Profiles
            {
                AccountId = accountId,
                ProfileId = type,
                Revision = 0
            };
            await profileRepository.SaveAsync(newProfile);

            switch (type)
            {
                case ProfileIds.Athena:
                    var items = new List<Items>
                    {
                        CreateItem(type, accountId, "AthenaPickaxe:DefaultPickaxe"),
                        CreateItem(type, accountId, "AthenaGlider:DefaultGlider"),
                        CreateItem(type, accountId, "AthenaDance:EID_DanceMove"),
                        CreateItem(type, accountId, "AthenaCharacter:CID_001_Athena_Commando_F_Default")
                    };

                    var athenaProfile = new AthenaProfile(accountId, items, new List<ItemAttributes>());
                    var constructedProfile = athenaProfile.CreateProfile(accountId, items, new List<ItemAttributes>());

                    foreach (var itemData in constructedProfile.Items)
                    {
                        await SaveItemAttributesAsync(type, accountId, itemData.Value, itemsRepository);
                    }

                    await SaveProfileAttributesAsync(constructedProfile.Stats.Attributes, type, accountId, itemAttributesRepository);
                    break;

                default:
                    Logger.Error($"Failed to find profileId: {type}");
                    break;
            }

            return newProfile;
        }

        /// <summary>
        /// Creates a new item with the specified profile ID and template ID.
        /// </summary>
        /// <param name="profileId">The ID of the profile the item belongs to.</param>
        /// <param name="templateId">The template ID of the item.</param>
        /// <returns>A new instance of <see cref="Items"/>.</returns>
        private static Items CreateItem(string profileId, string accountId, string templateId)
        {
            return new Items
            {
                AccountId = accountId,
                ProfileId = profileId,
                TemplateId = templateId,
                Value = System.Text.Json.JsonSerializer.Serialize(new ItemValue
                {
                    Xp = 0,
                    Level = 1,
                    Variants = new List<Variants>(),
                    ItemSeen = false,
                }),
                Quantity = 1,
                Favorite = false,
                ItemSeen = false,
            };
        }

        /// <summary>
        /// Saves the attributes of an item asynchronously.
        /// </summary>
        /// <param name="item">The item whose attributes are to be saved.</param>
        /// <param name="itemsRepository">The repository for items.</param>
        private static async Task SaveItemAttributesAsync(string profileId, string accountId, ItemDefinition item, Repository<Items> itemsRepository)
        {
            if (item.Attributes == null)
            {
                return;
            }

            dynamic relevantAttributes = new System.Dynamic.ExpandoObject();

            var attributesType = item.Attributes.GetType();
            foreach (var property in attributesType.GetProperties())
            {
                var jsonProperty = property.GetCustomAttribute<JsonPropertyAttribute>();
                var jsonPropertyName = jsonProperty != null ? jsonProperty.PropertyName : property.Name;

                switch (jsonPropertyName)
                {
                    case "favorite":
                        ((IDictionary<string, object>)relevantAttributes)["favorite"] = (bool)property.GetValue(item.Attributes);
                        break;
                    case "item_seen":
                        ((IDictionary<string, object>)relevantAttributes)["item_seen"] = (bool)property.GetValue(item.Attributes);
                        break;
                    case "xp":
                        ((IDictionary<string, object>)relevantAttributes)["xp"] = (int)property.GetValue(item.Attributes);
                        break;
                    case "variants":
                        ((IDictionary<string, object>)relevantAttributes)["variants"] = (List<Variants>)property.GetValue(item.Attributes);
                        break;
                }
            }

            var newItem = new Items
            {
                ProfileId = profileId,
                AccountId = accountId,
                TemplateId = item.TemplateId,
                Value = System.Text.Json.JsonSerializer.Serialize(relevantAttributes),
                Quantity = item.Quantity,
                Favorite = item.Attributes.Favorite,
                ItemSeen = item.Attributes.ItemSeen,
            };

            await itemsRepository.SaveAsync(newItem);
        }

        /// <summary>
        /// Saves the attributes of the profile asynchronously.
        /// </summary>
        /// <param name="itemAttributes">The profile attributes to save.</param>
        /// <param name="profileId">The profile ID associated with the attributes.</param>
        /// <param name="itemAttributesRepository">The repository for item attributes.</param>
        private static async Task SaveProfileAttributesAsync(object itemAttributes, string profileId, string accountId, Repository<ItemAttributes> itemAttributesRepository)
        {
            foreach (var property in itemAttributes.GetType().GetProperties())
            {
                var value = property.GetValue(itemAttributes);
                if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
                    continue;

                var jsonValue = JsonConvert.SerializeObject(value);
                var itemAttribute = new ItemAttributes
                {
                    AccountId = accountId,
                    ProfileId = profileId,
                    Property = property.Name,
                    Value = jsonValue
                };

                if (value is IList)
                {
                    await itemAttributesRepository.SaveAsync(itemAttribute, NpgsqlDbType.Jsonb);
                }
                else
                {
                    await itemAttributesRepository.SaveAsync(itemAttribute);
                }
            }
        }

        /// <summary>
        /// Asynchronously gets a user profile based on the provided account ID.
        /// </summary>
        /// <param name="accountId">The account ID associated with the profile.</param>
        /// <returns>A task that represents the asynchronous operation, containing the <see cref="IProfile"/> object if found, or null otherwise.</returns>
        /// <summary>
        /// Asynchronously gets a user profile based on the provided account ID.
        /// </summary>
        /// <param name="accountId">The account ID associated with the profile.</param>
        /// <returns>A task that represents the asynchronous operation, containing the <see cref="Profile"/> object if found, or null otherwise.</returns>
        public static async Task<IProfile?> GetProfileAsync(string accountId)
        {
            var timer = Stopwatch.StartNew();

            try
            {
                Config config = Config.GetConfig();
                Repository<Items> itemsRepository = new Repository<Items>(config.ConnectionUrl);
                Repository<Profiles> profilesRepository = new Repository<Profiles>(config.ConnectionUrl);
                Repository<ItemAttributes> itemAttribuesRepository = new Repository<ItemAttributes>(config.ConnectionUrl);

                Profiles primaryProfile = await profilesRepository.FindByAccountIdAsync(accountId);
                List<ItemAttributes> profileAttributes = new List<ItemAttributes> { await itemAttribuesRepository.FindByAccountIdAsync(accountId) };
                List<Items> profileItems = new List<Items> { await itemsRepository.FindByAccountIdAsync(accountId) };



                IProfile constructedItems = null;

                switch (primaryProfile.ProfileId)
                {
                    case "athena":
                        var athenaBuilder = new AthenaProfile(accountId, profileItems, profileAttributes);
                        constructedItems = athenaBuilder.GetProfile();
                        break;

                    default:
                        Logger.Error($"Missing profile: {primaryProfile.ProfileId}");
                        break;
                }

                if (constructedItems == null)
                {
                    Logger.Error($"constructedItems is null, ProfileId: {primaryProfile.ProfileId}.");
                    return null;
                }

                return constructedItems;
            }
            catch (Exception ex)
            {
                Logger.Error($"An error occurred while getting profile for accountId {accountId}: {ex.Message}");
                return null;
            }
            finally
            {
                if (timer.IsRunning)
                {
                    timer.Stop();
                }
            }
        }
    }
}
