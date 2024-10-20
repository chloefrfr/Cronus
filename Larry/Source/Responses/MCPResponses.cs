using Larry.Source.Classes.MCP.Response;
using Larry.Source.Classes.Profile;
using Larry.Source.Database.Entities;

namespace Larry.Source.Responses
{
    public class MCPResponses
    {
        /// <summary>
        /// Generates a basic profile response.
        /// </summary>
        /// <param name="profile">The profile data.</param>
        /// <param name="changes">Profile changes.</param>
        /// <param name="profileId">Profile identifier.</param>
        /// <returns>A <see cref="BaseResponse"/> containing profile data.</returns>
        public static BaseResponse Generate(Larry.Source.Database.Entities.Profiles profile, IEnumerable<object> changes, string profileId)
        {
            return GenerateBaseResponse(profile, changes.ToList(), profileId);
        }

        /// <summary>
        /// Generates a basic response with optional notifications and multi-updates.
        /// </summary>
        /// <param name="profile">The profile data.</param>
        /// <param name="changes">Profile changes.</param>
        /// <param name="profileId">Profile identifier.</param>
        /// <param name="notifications">Optional notifications.</param>
        /// <param name="multiUpdate">Optional multi-updates.</param>
        /// <returns>A dynamic response object.</returns>
        public static dynamic GenerateBasic(Larry.Source.Database.Entities.Profiles profile, IEnumerable<object> changes, string profileId,
            List<object> notifications = null, List<MultiUpdate> multiUpdate = null)
        {
            return new
            {
                ProfileRevision = profile.Revision,
                ProfileId = profileId,
                ProfileChangesBaseRevision = profile.Revision - 1,
                ProfileChanges = changes.ToList(),
                Notifications = notifications ?? new List<object>(),
                MultiUpdate = multiUpdate ?? new List<MultiUpdate>(),
                ProfileCommandRevision = profile.Revision,
                ServerTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                ResponseVersion = 1
            };
        }

        /// <summary>
        /// Generates a base response structure.
        /// </summary>
        private static BaseResponse GenerateBaseResponse(Larry.Source.Database.Entities.Profiles profile, List<object> changes, string profileId)
        {
            return new BaseResponse
            {
                ProfileRevision = profile.Revision,
                ProfileId = profileId,
                ProfileChangesBaseRevision = profile.Revision - 1,
                ProfileChanges = changes,
                ProfileCommandRevision = profile.Revision,
                ServerTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                ResponseVersion = 1
            };
        }

        /// <summary>
        /// Generates a refund response with multi-updates.
        /// </summary>
        public static RefundResponse GenerateRefundResponse(Larry.Source.Database.Entities.Profiles profile, Larry.Source.Database.Entities.Profiles athenaProfile,
            IEnumerable<object> changes, IEnumerable<object> multiUpdates, string profileId)
        {
            return new RefundResponse
            {
                ProfileRevision = profile.Revision,
                ProfileId = profileId,
                ProfileChangesBaseRevision = profile.Revision - 1,
                ProfileChanges = changes.ToList(),
                ProfileCommandRevision = profile.Revision,
                MultiUpdate = new List<MultiUpdate>
                {
                    GenerateMultiUpdate(athenaProfile, multiUpdates.ToList(), ProfileIds.Athena)
                }
            };
        }

        /// <summary>
        /// Generates a purchase response with notifications and multi-updates.
        /// </summary>
        public static PurchaseResponse GeneratePurchaseResponse(Larry.Source.Database.Entities.Profiles profile, Larry.Source.Database.Entities.Profiles athenaProfile,
            IEnumerable<object> changes, IEnumerable<object> multiUpdates, IEnumerable<object> notifications,
            string profileId)
        {
            return new PurchaseResponse
            {
                ProfileRevision = profile.Revision,
                ProfileId = profileId,
                ProfileChangesBaseRevision = profile.Revision - 1,
                ProfileChanges = changes.ToList(),
                ProfileCommandRevision = profile.Revision,
                Notifications = notifications.ToList(),
                MultiUpdate = new List<MultiUpdate>
                {
                    GenerateMultiUpdate(athenaProfile, multiUpdates.ToList(), ProfileIds.Athena)
                }
            };
        }

        /// <summary>
        /// Generates a multi-update response.
        /// </summary>
        private static MultiUpdate GenerateMultiUpdate(Larry.Source.Database.Entities.Profiles profile, List<object> changes, string profileId)
        {
            return new MultiUpdate
            {
                ProfileRevision = profile.Revision,
                ProfileId = (dynamic)profileId,
                ProfileChangesBaseRevision = profile.Revision - 1,
                ProfileChanges = changes,
                ProfileCommandRevision = profile.Revision
            };
        }
    }
}
