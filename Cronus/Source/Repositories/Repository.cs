using Npgsql;
using Cronus.Source.Database.Entities;
using Cronus.Source.Mappings;

namespace Cronus.Source.Repositories
{
    public class Repository<TEntity> where TEntity : BaseEntity, new()
    {
        private readonly string _connectionString;

        public Repository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Saves an entity to the database asynchronously.
        /// </summary>
        /// <param name="entity">The entity instance to save.</param>
        /// <remarks>
        /// This method inserts a new entity if its Id is zero, or updates the existing entity if its Id is greater than zero.
        /// </remarks>
        public async Task SaveAsync(TEntity entity)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = $"INSERT INTO {EntityMapper.GetTableName(entity)} ({EntityMapper.GetColumnNames(entity)}) " +
                        $"VALUES ({EntityMapper.GetParameterValues(entity)}) RETURNING id";

            using var command = new NpgsqlCommand(query, connection);
            EntityMapper.MapParameters(command, entity);

            var id = await command.ExecuteScalarAsync();
            entity.Id = Convert.ToInt32(id);
        }

        /// <summary>
        /// Finds an entity by its unique identifier asynchronously.
        /// </summary>
        /// <param name="id">The unique identifier of the entity.</param>
        /// <returns>The entity instance if found; otherwise, null.</returns
        public async Task<TEntity> FindByIdAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var entity = new TEntity();
            var tableName = EntityMapper.GetTableName(entity);
            var query = $"SELECT * FROM {tableName} WHERE id = @id";

            using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);


            using var reader = await command.ExecuteReaderAsync();
            return await EntityMapper.MapToEntityAsync<TEntity>(reader);
        }

        /// <summary>
        /// Deletes an entity by its unique identifier asynchronously.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to delete.</param>
        public async Task DeleteAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var tableName = EntityMapper.GetTableName(new TEntity());
            var query = $"DELETE FROM {tableName} WHERE id = @id";

            using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Finds all entities asynchronously.
        /// </summary>
        /// <returns>A list of all entities.</returns>
        public async Task<List<TEntity>> GetAllAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var tableName = EntityMapper.GetTableName(new TEntity());
            var query = $"SELECT * FROM {tableName}";

            using var command = new NpgsqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            var entities = new List<TEntity>();
            while (await reader.ReadAsync())
            {
                var entity = await EntityMapper.MapToEntityAsync<TEntity>(reader);
                entities.Add(entity);
            }

            return entities;
        }

        /// <summary>
        /// Updates an existing entity asynchronously.
        /// </summary>
        /// <param name="entity">The entity instance to update.</param>
        public async Task UpdateAsync(TEntity entity)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = $"UPDATE {EntityMapper.GetTableName(entity)} SET {EntityMapper.GetUpdateSetClause(entity)} WHERE id = @id";

            using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", entity.Id);
            EntityMapper.MapParameters(command, entity);

            await command.ExecuteNonQueryAsync();
        }
    }
}
