using Larry.Source.Database.Attributes;
using System.Text.Json;

namespace Larry.Source.Database.Entities
{
    [Entity("items")]
    public class Items : BaseEntity
    {
        [Column("accountId")]
        public string AccountId { get; set; }

        [Column("profileId")]
        public string ProfileId { get; set; }

        [Column("templateId")]
        public string TemplateId { get; set; }

        [Column("value")]
        public string Value { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("isStat")]
        public bool IsStat { get; set; }
    }
}
