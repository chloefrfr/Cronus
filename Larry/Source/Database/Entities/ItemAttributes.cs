using Larry.Source.Database.Attributes;
using System.Text.Json;

namespace Larry.Source.Database.Entities
{
    [Entity("itemattributes")]
    public class ItemAttributes : BaseEntity
    {
        [Column("accountId")]
        public string AccountId { get; set; }


        [Column("profileId")]
        public string ProfileId { get; set; }

        [Column("property")]
        public string Property {  get; set; }

        [Column("value")]
        public object Value { get; set; }
    }
}
