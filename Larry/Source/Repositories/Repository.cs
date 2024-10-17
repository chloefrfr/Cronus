using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using Larry.Source.Database.Entities;
using Larry.Source.Mappings;
using Serilog;
using Larry.Source.Utilities;

namespace Larry.Source.Repositories
{
    public class Repository<TEntity> where TEntity : BaseEntity, new()
    {
        private readonly string _connectionString;

        public Repository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private IDbConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }

        private async Task OpenConnectionAsync(IDbConnection connection)
        {
            await Task.Run(() => connection.Open());
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
            var stopwatch = Stopwatch.StartNew();

            using var connection = CreateConnection();
            await OpenConnectionAsync(connection);

            var tableName = EntityMapper.GetTableName(entity);
            var id = entity.Id;

            var updateSetClause = EntityMapper.GetUpdateSetClause(entity);
            var columnNames = EntityMapper.GetColumnNames(entity);
            var parameterValues = EntityMapper.GetParameterValues(entity);

            var query = $@"
INSERT INTO {tableName} ({columnNames})
VALUES ({parameterValues})
ON CONFLICT (id) 
DO UPDATE SET {updateSetClause}
RETURNING id;";

            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Int32); 
            parameters.AddDynamicParams(entity); 

            entity.Id = await connection.ExecuteScalarAsync<int>(query, parameters);

            stopwatch.Stop();
            Logger.Information($"SaveAsync took {stopwatch.ElapsedMilliseconds} ms");
        }

        /// <summary>
        /// Finds an entity by its unique identifier asynchronously.
        /// </summary>
        /// <param name="id">The unique identifier of the entity.</param>
        /// <returns>The entity instance if found; otherwise, null.</returns>
        public async Task<TEntity> FindByIdAsync(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            using var connection = CreateConnection();
            var query = $"SELECT * FROM {EntityMapper.GetTableName(new TEntity())} WHERE id = @Id";
            var result = await connection.QueryFirstOrDefaultAsync<TEntity>(query, new { Id = id });
            stopwatch.Stop();
            Log.Information("FindByIdAsync took {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);
            return result;
        }

        /// <summary>
        /// Finds an entity by a specified column asynchronously.
        /// </summary>
        /// <param name="columnName">The name of the column to search.</param>
        /// <param name="value">The value to search for.</param>
        /// <returns>The entity instance if found; otherwise, null.</returns>
        private async Task<TEntity> FindByColumnAsync(string columnName, string value)
        {
            var stopwatch = Stopwatch.StartNew();

            using var connection = CreateConnection();
            await OpenConnectionAsync(connection); 

            var query = $"SELECT * FROM {EntityMapper.GetTableName(new TEntity())} WHERE {columnName} = @Value";

            var result = await connection.QueryFirstOrDefaultAsync<TEntity>(query, new { Value = value });

            stopwatch.Stop();
            Logger.Information($"FindByColumnAsync ({columnName}) took {stopwatch.ElapsedMilliseconds} ms");

            return result;
        }

        /// <summary>
        /// Finds a user entity by its Discord ID asynchronously.
        /// </summary>
        /// <param name="discordId">The Discord ID of the user.</param>
        /// <returns>The user instance if found; otherwise, null.</returns>
        public Task<TEntity> FindByDiscordIdAsync(string discordId)
        {
            EnsureUserEntity();
            return FindByColumnAsync("discordid", discordId);
        }

        /// <summary>
        /// Finds a user entity by its email asynchronously.
        /// </summary>
        /// <param name="email">The email of the user.</param>
        /// <returns>The user instance if found; otherwise, null.</returns>
        public Task<TEntity> FindByEmailAsync(string email)
        {
            EnsureUserEntity();
            return FindByColumnAsync("email", email);
        }

        /// <summary>
        /// Finds a user entity by its username asynchronously.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <returns>The user instance if found; otherwise, null.</returns>
        public Task<TEntity> FindByUsernameAsync(string username)
        {
            EnsureUserEntity();
            return FindByColumnAsync("username", username);
        }

        /// <summary>
        /// Finds a user entity by its account ID asynchronously.
        /// </summary>
        /// <param name="accountId">The account ID of the user.</param>
        /// <returns>The user instance if found; otherwise, null.</returns>
        public Task<TEntity> FindByAccountIdAsync(string accountId)
        {
            EnsureUserEntity();
            return FindByColumnAsync("accountid", accountId);
        }

        /// <summary>
        /// Deletes an entity by its unique identifier asynchronously.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to delete.</param>
        public async Task DeleteAsync(int id)
        {
            var stopwatch = Stopwatch.StartNew();
            using var connection = CreateConnection();
            var query = $"DELETE FROM {EntityMapper.GetTableName(new TEntity())} WHERE id = @Id";
            await connection.ExecuteAsync(query, new { Id = id });
            stopwatch.Stop();
            Log.Information("DeleteAsync took {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Finds all entities asynchronously.
        /// </summary>
        /// <returns>A list of all entities.</returns>
        public async Task<List<TEntity>> GetAllAsync()
        {
            var stopwatch = Stopwatch.StartNew();
            using var connection = CreateConnection();
            var query = $"SELECT * FROM {EntityMapper.GetTableName(new TEntity())}";
            var result = await connection.QueryAsync<TEntity>(query);
            stopwatch.Stop();
            Log.Information("GetAllAsync took {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);
            return result.AsList();
        }

        /// <summary>
        /// Updates an existing entity asynchronously.
        /// </summary>
        /// <param name="entity">The entity instance to update.</param>
        public async Task UpdateAsync(TEntity entity)
        {
            var stopwatch = Stopwatch.StartNew();
            using var connection = CreateConnection();
            var query = $@"
                UPDATE {EntityMapper.GetTableName(entity)}
                SET {EntityMapper.GetUpdateSetClause(entity)}
                WHERE id = @Id";
            await connection.ExecuteAsync(query, entity);
            stopwatch.Stop();
            Log.Information("UpdateAsync took {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Deletes a Tokens entity by account ID and type asynchronously.
        /// </summary>
        /// <param name="accountId">The account ID associated with the token.</param>
        /// <param name="type">The type of the token.</param>
        public async Task DeleteByTypeAsync(string accountId, string type)
        {
            EnsureTokensEntity();
            var stopwatch = Stopwatch.StartNew();
            using var connection = CreateConnection();
            var query = $"DELETE FROM {EntityMapper.GetTableName(new TEntity())} WHERE accountid = @AccountId AND type = @Type";
            await connection.ExecuteAsync(query, new { AccountId = accountId, Type = type });
            stopwatch.Stop();
            Log.Information("DeleteByTypeAsync took {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Ensures the entity is of type User.
        /// </summary>
        private void EnsureUserEntity()
        {
            if (typeof(TEntity) != typeof(User))
                throw new InvalidOperationException("This method is only available for User entities.");
        }

        /// <summary>
        /// Ensures the entity is of type Tokens.
        /// </summary>
        private void EnsureTokensEntity()
        {
            if (typeof(TEntity) != typeof(Tokens))
                throw new InvalidOperationException("This method is only available for Tokens entities.");
        }
    }
}
