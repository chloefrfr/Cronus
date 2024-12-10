using System.ComponentModel.DataAnnotations;
using Larry.Source.Database.Attributes;

namespace Larry.Source.Database.Entities
{
    [Entity("friends")]
    public class Friends: BaseEntity
    {
        [Column("accountId")]
        public string AccountId { get; set; }

        [Column("createdAt")]
        public string CreatedAt { get; set; }

        [Column("friendId")]
        public string FriendId { get; set; }

        [Column("alias")] 
        public string Alias { get; set; }

        [Column("direction")]
        public string Direction { get; set; }

        [Column("status")]
        public string Status { get; set; }
    }
}
