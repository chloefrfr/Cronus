using Larry.Source.Database.Attributes;

namespace Larry.Source.Database.Entities
{
    [Entity("migrations")]
    public class Migrations : BaseEntity
    {
        [Column("name")]
        public string Name { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
