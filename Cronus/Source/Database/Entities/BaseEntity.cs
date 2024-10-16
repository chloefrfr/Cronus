using Cronus.Source.Database.Attributes;

namespace Cronus.Source.Database.Entities
{
    public abstract class BaseEntity
    {
        [PrimaryKey]
        public int Id { get; set; }
    }
}
