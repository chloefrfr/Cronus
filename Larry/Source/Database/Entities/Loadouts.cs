using Larry.Source.Database.Attributes;
using System.Text.Json;

namespace Larry.Source.Database.Entities
{
    [Entity("loadouts")]
    public class Loadouts : BaseEntity
    {
        [Column("accountId")]
        public string AccountId { get; set; }

        [Column("profileId")]
        public string ProfileId { get; set; }

        [Column("templateId")]
        public string TemplateId { get; set; }

        [Column("lockerName")]
        public string LockerName { get; set; }

        [Column("bannerId")]
        public string BannerId { get; set; }

        [Column("bannerColorId")]
        public string BannerColorId { get; set; }

        [Column("characterId")]
        public string CharacterId { get; set; }

        [Column("backpackId")]
        public string BackpackId { get; set; }

        [Column("gliderId")]
        public string GliderId { get; set; }

        [Column("danceId")]
        public string DanceId { get; set; }

        [Column("pickaxeId")]
        public string PickaxeId { get; set; }

        [Column("itemWrapId")]
        public string ItemWrapId { get; set; }

        [Column("contrailId")]
        public string ContrailId { get; set; }

        [Column("loadingScreenId")]
        public string LoadingScreenId { get; set; }

        [Column("musicPackId")]
        public string MusicPackId { get; set; }
    }
}
