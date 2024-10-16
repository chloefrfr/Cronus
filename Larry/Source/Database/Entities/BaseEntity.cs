using Larry.Source.Database.Attributes;

namespace Larry.Source.Database.Entities
{
    public abstract class BaseEntity
    {
        [PrimaryKey]
        public int Id { get; set; }
    }
}
