using Npgsql;
using Larry.Source.Database.Entities;
using Larry.Source.Mappings;

namespace Larry.Source.Repositories
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
        /// Finds an entity by its DiscordId asynchronously.
        /// Available only for User entities.
        /// </summary>
        /// <param name="discordId">The Discord ID of the user.</param>
        /// <returns>The user instance if found; otherwise, null.</returns>
        public async Task<TEntity> FindByDiscordIdAsync(string discordId)
        {
            if (typeof(TEntity) != typeof(User))
                throw new InvalidOperationException("This method is only available for User entities.");

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var entity = new TEntity();
            var tableName = EntityMapper.GetTableName(entity);
            var query = $"SELECT * FROM {tableName} WHERE discordid = @discordId";

            using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@discordId", discordId);

            using var reader = await command.ExecuteReaderAsync();
            return await EntityMapper.MapToEntityAsync<TEntity>(reader);
        }

        /// <summary>
        /// Finds an entity by its Email asynchronously.
        /// Available only for User entities.
        /// </summary>
        /// <param name="email">The email of the user.</param>
        /// <returns>The user instance if found; otherwise, null.</returns>
        public async Task<TEntity> FindByEmailAsync(string email)
        {
            if (typeof(TEntity) != typeof(User))
                throw new InvalidOperationException("This method is only available for User entities.");

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var entity = new TEntity();
            var tableName = EntityMapper.GetTableName(entity);
            var query = $"SELECT * FROM {tableName} WHERE email = @Email";

            using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Email", email);

            using var reader = await command.ExecuteReaderAsync();
            return await EntityMapper.MapToEntityAsync<TEntity>(reader);
        }

        /// <summary>
        /// Finds an entity by its Username asynchronously.
        /// Available only for User entities.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <returns>The user instance if found; otherwise, null.</returns>
        public async Task<TEntity> FindByUsernameAsync(string username)
        {
            if (typeof(TEntity) != typeof(User))
                throw new InvalidOperationException("This method is only available for User entities.");

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var entity = new TEntity();
            var tableName = EntityMapper.GetTableName(entity);
            var query = $"SELECT * FROM {tableName} WHERE username = @Username";

            using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@Username", username);

            using var reader = await command.ExecuteReaderAsync();
            return await EntityMapper.MapToEntityAsync<TEntity>(reader);
        }

        /// <summary>
        /// Finds an entity by its AccountiD asynchronously.
        /// Available only for User entities.
        /// </summary>
        /// <param name="accountId">The accountId of the user.</param>
        /// <returns>The user instance if found; otherwise, null.</returns>
        public async Task<TEntity?> FindByAccountIdAsync(string accountId)
        {
            if (typeof(TEntity) != typeof(User))
                throw new InvalidOperationException("This method is only available for User entities.");

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var entity = new TEntity();
            var tableName = EntityMapper.GetTableName(entity);
            var query = $"SELECT * FROM {tableName} WHERE accountid = @AccountId";

            using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@AccountId", accountId);

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

        /// <summary>
        /// Deletes a Tokens entity by accountId and type.
        /// This method is only available for Tokens entities.
        /// </summary>
        /// <param name="accountId">The account ID associated with the token.</param>
        /// <param name="type">The type of the token.</param>
        public async Task DeleteByTypeAsync(string accountId, string type)
        {
            if (typeof(TEntity) != typeof(Tokens))
                throw new InvalidOperationException("This method is only available for Tokens entities.");

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var tableName = EntityMapper.GetTableName(new TEntity());
            var query = $"DELETE FROM {tableName} WHERE accountid = @AccountId AND type = @Type";

            using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@AccountId", accountId);
            command.Parameters.AddWithValue("@Type", type);

            await command.ExecuteNonQueryAsync();
        }
    }
}
