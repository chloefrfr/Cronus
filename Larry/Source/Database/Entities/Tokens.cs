using Larry.Source.Database.Attributes;

namespace Larry.Source.Database.Entities
{
    [Entity("tokens")]
    public class Tokens : BaseEntity
    {
        [Column("accountId")]
        public string AccountId { get; set; }

        [Column("type")]
        public string Type { get; set; }

        [Column("token")]
        public string Token { get; set; }

        [Column("clientId")]
        public string ClientId { get; set; }

        [Column("grantType")]
        public string GrantType { get; set; }
    }
}
