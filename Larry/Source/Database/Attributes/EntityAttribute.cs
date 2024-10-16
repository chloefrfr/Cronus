namespace Larry.Source.Database.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EntityAttribute : Attribute
    {
        public string TableName { get; }

        public EntityAttribute(string tableName)
        {
            TableName = tableName;
        }
    }
}
