using Larry.Source.Database.Attributes;

namespace Larry.Source.Database.Entities
{
    [Entity("profiles")]
    public class Profiles : BaseEntity
    {
        [Column("accountId")]
        public string AccountId { get; set; }

        [Column("profileId")]
        public string ProfileId { get; set; }

        [Column("revision")]
        public int Revision { get; set; }
    }
}
