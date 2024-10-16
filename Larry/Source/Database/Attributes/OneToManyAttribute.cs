namespace Larry.Source.Database.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class OneToManyAttribute : Attribute
    {
        public Type RelatedEntity { get; }
        public OneToManyAttribute(Type relatedEntity)
        {
            RelatedEntity = relatedEntity;
        }
    }
}
