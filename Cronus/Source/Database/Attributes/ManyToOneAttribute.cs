namespace Cronus.Source.Database.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ManyToOneAttribute : Attribute
    {
        public Type RelatedEntity { get; }
        public ManyToOneAttribute(Type relatedEntity)
        {
            RelatedEntity = relatedEntity;
        }
    }
}
