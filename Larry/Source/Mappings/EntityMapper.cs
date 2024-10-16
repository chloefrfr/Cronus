using Npgsql;
using System.Reflection;
using Larry.Source.Database.Attributes;
using System.Threading.Tasks;
using Larry.Source.Database.Entities;

namespace Larry.Source.Mappings
{
    public static class EntityMapper
    {
        /// <summary>
        /// Gets the name of the database table associated with the entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity instance.</param>
        /// <returns>The name of the table as a string.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the Entity attribute is missing.</exception>
        public static string GetTableName<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            var entityType = entity.GetType();
            var entityAttribute = entityType.GetCustomAttribute<EntityAttribute>();

            if (entityAttribute == null)
            {
                throw new InvalidOperationException("Missing Table Attribute");
            }

            return entityAttribute.TableName;
        }

        /// <summary>
        /// Gets a comma-separated list of column names for the entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="e">The entity instance.</param>
        /// <returns>A string of column names.</returns>
        public static string GetColumnNames<TEntity>(TEntity e)
        {
            var properties = e.GetType().GetProperties();
            return string.Join(", ", properties.Where(p => p.GetCustomAttribute<ColumnAttribute>() != null)
                                                 .Select(p => p.GetCustomAttribute<ColumnAttribute>().ColumnName));
        }

        /// <summary>
        /// Gets a comma-separated list of parameter placeholders for the entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity instance.</param>
        /// <returns>A string of parameter placeholders.</returns>
        public static string GetParameterValues<TEntity>(TEntity entity)
        {
            var properties = entity.GetType().GetProperties();
            return string.Join(", ", properties.Where(p => p.GetCustomAttribute<ColumnAttribute>() != null)
                                                 .Select(p => $"@{p.Name}"));
        }

        /// <summary>
        /// Maps the entity properties to the command parameters.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="command">The NpgsqlCommand to which parameters will be added.</param>
        /// <param name="entity">The entity instance.</param>
        public static void MapParameters<TEntity>(NpgsqlCommand command, TEntity entity)
        {
            foreach (var prop in entity.GetType().GetProperties())
            {
                var columnAttr = prop.GetCustomAttribute<ColumnAttribute>();
                if (columnAttr != null)
                {
                    command.Parameters.AddWithValue($"@{prop.Name}", prop.GetValue(entity));
                }
            }
        }

        /// <summary>
        /// Maps a data reader row to an entity instance.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="reader">The NpgsqlDataReader to read from.</param>
        /// <returns>The mapped entity instance.</returns>
        public static async Task<TEntity> MapToEntityAsync<TEntity>(NpgsqlDataReader reader) where TEntity : new()
        {
            if (!await reader.ReadAsync()) return default;

            var entity = new TEntity();

            foreach (var prop in typeof(TEntity).GetProperties())
            {
                var columnAttr = prop.GetCustomAttribute<ColumnAttribute>();
                if (columnAttr != null)
                {
                    var value = reader[columnAttr.ColumnName];
                    prop.SetValue(entity, value == DBNull.Value ? null : value);
                }
            }

            return entity;
        }

        /// <summary>
        /// Gets the SQL SET clause for updating an entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity instance.</param>
        /// <returns>The SQL SET clause as a string.</returns>
        public static string GetUpdateSetClause<TEntity>(TEntity entity)
        {
            var properties = typeof(TEntity).GetProperties()
                .Where(p => p.Name != "Id")
                .Select(p =>
                {
                    var columnName = p.GetCustomAttribute<ColumnAttribute>()?.ColumnName ?? p.Name;
                    return $"{columnName} = @{p.Name}";
                });

            return string.Join(", ", properties);
        }
    }
}
