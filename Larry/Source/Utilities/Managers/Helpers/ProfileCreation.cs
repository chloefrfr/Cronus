using Larry.Source.Classes.MCP;
using Larry.Source.Classes.Profile;
using Larry.Source.Database.Entities;
using System.Text.Json;

namespace Larry.Source.Utilities.Managers.Helpers
{
    public static class ProfileCreation
    {
        /// <summary>
        /// Creates a new item with the specified profile ID and template ID.
        /// </summary>
        /// <param name="profileId">The ID of the profile the item belongs to.</param>
        /// <param name="accountId">The account ID of the user.</param>
        /// <param name="templateId">The template ID of the item.</param>
        /// <param name="value">Optional: The value to be serialized into the item's data (defaults to null if not provided).</param>
        /// <param name="quantity">Optional: The quantity of the item (defaults to 1).</param>
        /// <param name="isStat">Optional: Indicates if the item is a stat item (defaults to false).</param>
        /// <returns>A new instance of <see cref="Items"/>.</returns>
        private static Items CreateItemBase(string profileId, string accountId, string templateId, object value = null, int quantity = 1, bool isStat = false)
        {
            var itemValue = value ?? new { xp = 0, level = 1, variants = new List<Variants>(), item_seen = false };

            return new Items
            {
                AccountId = accountId,
                ProfileId = profileId,
                TemplateId = templateId,
                Value = JsonSerializer.Serialize(itemValue),
                Quantity = quantity,
                IsStat = isStat
            };
        }

        /// <summary>
        /// Creates a new regular item.
        /// </summary>
        /// <param name="profileId">The ID of the profile the item belongs to.</param>
        /// <param name="accountId">The account ID of the user.</param>
        /// <param name="templateId">The template ID of the item.</param>
        /// <returns>A new instance of <see cref="Items"/>.</returns>
        public static Items CreateItem(string profileId, string accountId, string templateId)
        {
            return CreateItemBase(profileId, accountId, templateId);
        }

        /// <summary>
        /// Creates a new currency or special item (e.g., MtxPurchased).
        /// </summary>
        /// <param name="profileId">The ID of the profile the item belongs to.</param>
        /// <param name="accountId">The account ID of the user.</param>
        /// <param name="templateId">The template ID of the item.</param>
        /// <returns>A new instance of <see cref="Items"/>.</returns>
        public static Items CreateCCItem(string profileId, string accountId, string templateId)
        {
            var itemValue = new { platform = "EpicPC", level = 1 };

            int quantity = templateId == "Currency:MtxPurchased" ? 0 : 1;

            return CreateItemBase(profileId, accountId, templateId, itemValue, quantity);
        }

        /// <summary>
        /// Creates a new stat item with the provided details.
        /// </summary>
        /// <param name="profileId">The ID of the profile the item belongs to.</param>
        /// <param name="accountId">The account ID of the user.</param>
        /// <param name="templateId">The template ID of the item.</param>
        /// <param name="value">The value associated with the stat item (can be any dynamic data).</param>
        /// <returns>A new instance of <see cref="Items"/>.</returns>
        public static Items CreateStatItem(string profileId, string accountId, string templateId, dynamic value)
        {
            return CreateItemBase(profileId, accountId, templateId, value, isStat: true);
        }
    }
}
