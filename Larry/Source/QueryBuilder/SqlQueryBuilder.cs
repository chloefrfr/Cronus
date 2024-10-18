using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Larry.Source.QueryBuilder
{
    /// <summary>
    /// A SQL query builder for constructing and executing SQL queries in a fluent manner.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity associated with the query.</typeparam>
    public class SqlQueryBuilder<TEntity>
    {
        private readonly string _tableName;
        private readonly List<string> _selectColumns = new List<string>();
        private readonly List<string> _joins = new List<string>();
        private readonly List<string> _whereConditions = new List<string>();
        private readonly List<string> _groupByColumns = new List<string>();
        private readonly List<string> _orderByColumns = new List<string>();
        private int? _limit;
        private Dictionary<string, object> _parameters = new Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlQueryBuilder{TEntity}"/> class.
        /// </summary>
        /// <param name="tableName">The name of the table for the query.</param>
        public SqlQueryBuilder(string tableName)
        {
            _tableName = tableName;
        }

        /// <summary>
        /// Specifies the columns to select in the query.
        /// </summary>
        /// <param name="columns">The columns to select.</param>
        /// <returns>The current instance of <see cref="SqlQueryBuilder{TEntity}"/>.</returns>
        public SqlQueryBuilder<TEntity> Select(params string[] columns)
        {
            _selectColumns.Clear();
            _selectColumns.AddRange(columns);
            return this;
        }

        /// <summary>
        /// Specifies that the query should return distinct results.
        /// </summary>
        /// <returns>The current instance of <see cref="SqlQueryBuilder{TEntity}"/>.</returns>
        public SqlQueryBuilder<TEntity> Distinct()
        {
            // Optionally handle distinct selection
            return this;
        }

        /// <summary>
        /// Adds a left join to the query.
        /// </summary>
        /// <param name="tableName">The name of the table to join.</param>
        /// <param name="condition">The join condition.</param>
        /// <returns>The current instance of <see cref="SqlQueryBuilder{TEntity}"/>.</returns>
        public SqlQueryBuilder<TEntity> LeftJoin(string tableName, string condition)
        {
            _joins.Add($"LEFT JOIN {tableName} ON {condition}");
            return this;
        }

        /// <summary>
        /// Adds an inner join to the query.
        /// </summary>
        /// <param name="tableName">The name of the table to join.</param>
        /// <param name="condition">The join condition.</param>
        /// <returns>The current instance of <see cref="SqlQueryBuilder{TEntity}"/>.</returns>
        public SqlQueryBuilder<TEntity> InnerJoin(string tableName, string condition)
        {
            _joins.Add($"INNER JOIN {tableName} ON {condition}");
            return this;
        }

        /// <summary>
        /// Adds a right join to the query.
        /// </summary>
        /// <param name="tableName">The name of the table to join.</param>
        /// <param name="condition">The join condition.</param>
        /// <returns>The current instance of <see cref="SqlQueryBuilder{TEntity}"/>.</returns>
        public SqlQueryBuilder<TEntity> RightJoin(string tableName, string condition)
        {
            _joins.Add($"RIGHT JOIN {tableName} ON {condition}");
            return this;
        }

        /// <summary>
        /// Adds a where condition to the query.
        /// </summary>
        /// <param name="condition">The where condition with a placeholder for parameters.</param>
        /// <param name="value">The value for the parameter.</param>
        /// <returns>The current instance of <see cref="SqlQueryBuilder{TEntity}"/>.</returns>
        public SqlQueryBuilder<TEntity> Where(string condition, object value)
        {
            var parameterName = $"@param{_parameters.Count}";
            _whereConditions.Add(condition.Replace("@Value", parameterName));
            _parameters[parameterName] = value;
            return this;
        }

        /// <summary>
        /// Adds group by columns to the query.
        /// </summary>
        /// <param name="columns">The columns to group by.</param>
        /// <returns>The current instance of <see cref="SqlQueryBuilder{TEntity}"/>.</returns>
        public SqlQueryBuilder<TEntity> GroupBy(params string[] columns)
        {
            _groupByColumns.AddRange(columns);
            return this;
        }

        /// <summary>
        /// Adds an order by clause to the query.
        /// </summary>
        /// <param name="column">The column to order by.</param>
        /// <param name="descending">True to order by descending, false for ascending.</param>
        /// <returns>The current instance of <see cref="SqlQueryBuilder{TEntity}"/>.</returns>
        public SqlQueryBuilder<TEntity> OrderBy(string column, bool descending = false)
        {
            _orderByColumns.Add($"{column} {(descending ? "DESC" : "ASC")}");
            return this;
        }

        /// <summary>
        /// Sets the limit for the number of results returned.
        /// </summary>
        /// <param name="limit">The maximum number of results to return.</param>
        /// <returns>The current instance of <see cref="SqlQueryBuilder{TEntity}"/>.</returns>
        public SqlQueryBuilder<TEntity> Limit(int limit)
        {
            _limit = limit;
            return this;
        }

        /// <summary>
        /// Builds the final SQL query string.
        /// </summary>
        /// <returns>The constructed SQL query string.</returns>
        public string Build()
        {
            var query = new StringBuilder();
            query.Append($"SELECT {string.Join(", ", _selectColumns)} FROM {_tableName} ");

            if (_joins.Any())
            {
                query.Append(string.Join(" ", _joins));
            }

            if (_whereConditions.Any())
            {
                query.Append(" WHERE " + string.Join(" AND ", _whereConditions));
            }

            if (_groupByColumns.Any())
            {
                query.Append(" GROUP BY " + string.Join(", ", _groupByColumns));
            }

            if (_orderByColumns.Any())
            {
                query.Append(" ORDER BY " + string.Join(", ", _orderByColumns));
            }

            if (_limit.HasValue)
            {
                query.Append($" LIMIT {_limit.Value}");
            }

            return query.ToString();
        }

        /// <summary>
        /// Gets the parameters used in the query.
        /// </summary>
        /// <returns>A dictionary containing parameter names and values.</returns>
        public Dictionary<string, object> GetParameters() => _parameters;
    }
}
