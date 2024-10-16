using System;
using System.Collections.Generic;
using System.Text;

namespace Cronus.Source.QueryBuilder
{
    public class QueryBuilder
    {
        private readonly StringBuilder _query;
        private readonly List<object> _parameters;

        public QueryBuilder()
        {
            _query = new StringBuilder();
            _parameters = new List<object>();
        }

        public QueryBuilder Select(string columns)
        {
            _query.Append($"SELECT {columns} ");
            return this;
        }

        public QueryBuilder From(string table)
        {
            _query.Append($"FROM {table} ");
            return this;
        }

        public QueryBuilder Where(string condition, object parameter)
        {
            _query.Append($"WHERE {condition} ");
            _parameters.Add(parameter);
            return this;
        }

        public string Build()
        {
            return _query.ToString();
        }

        public IEnumerable<object> GetParameters()
        {
            return _parameters;
        }
    }
}
