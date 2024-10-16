namespace Larry.Source.Database.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        public string TableName { get; }
        public TableAttribute(string tableName) => TableName = tableName;
    }
}
