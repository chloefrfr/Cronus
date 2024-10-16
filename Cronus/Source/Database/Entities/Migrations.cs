using Cronus.Source.Database.Attributes;

namespace Cronus.Source.Database.Entities
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
