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
using NpgsqlTypes;
using System.Data.Common;

namespace Larry.Source.Repositories
{
    public class Repository<TEntity> where TEntity : BaseEntity, new()
    {
        private readonly string _connectionString;

        public Repository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }

        public async Task OpenConnectionAsync(IDbConnection connection)
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
            //  Logger.Information($"SaveAsync took {stopwatch.ElapsedMilliseconds} ms");
        }

        /// <summary>
        /// Gets all items associated with a specific account ID asynchronously.
        /// </summary>
        /// <param name="accountId">The unique identifier for the account. Used to filter items associated with a specific account.</param>
        /// <param name="profileId">The unique identifier for the profile. Used to filter items associated with a specific user profile.</param>
        /// <returns>A list of items associated with the given account ID.</returns>
        public async Task<List<TEntity>> GetAllItemsByAccountIdAsync(string accountId, string profileId)
        {
            const int maxRetries = 3;
            const int commandTimeout = 5;

            var query = $@"
        SELECT * 
        FROM {EntityMapper.GetTableName(new TEntity())} 
        WHERE accountid = @AccountId AND profileid = @ProfileId;";

            var parameters = new { AccountId = accountId, ProfileId = profileId };

            using var connection = CreateConnection();
            await OpenConnectionAsync(connection);

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                Config config = Config.GetConfig();
                var asyncRepository = new AsyncRepositoryBase(config.ConnectionUrl);

                using var transaction = await asyncRepository.BeginTransactionAsync(connection);
                try
                {
                    var commandDefinition = new CommandDefinition(
                        query,
                        parameters,
                        transaction: transaction,
                        commandTimeout: commandTimeout
                    );

                    var result = await connection.QueryAsync<TEntity>(commandDefinition).ConfigureAwait(false);
                    transaction.Commit();
                    stopwatch.Stop();
                    //Logger.Information($"GetAllItemsByAccountIdAsync took {stopwatch.ElapsedMilliseconds} ms");
                    return result.AsList();
                }
                catch (TimeoutException ex)
                {
                    transaction.Rollback();
                    Logger.Error($"Timeout error on attempt {attempt + 1}: {ex.Message}");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Logger.Error($"An error occurred on attempt {attempt + 1}: {ex.Message}");
                }
            }

            throw new Exception("Max retries reached.");
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
            return result;
        }

        /// <summary>
        /// Finds all friend entities by the given account ID asynchronously.
        /// </summary>
        /// <param name="accountId">The account ID of the user.</param>
        /// <returns>A list of friend entities if found; otherwise, an empty list.</returns>
        public async Task<List<Friends>> FindFriendsByAccountIdAsync(string accountId)
        {
            EnsureIsCorrectEntity();

            // Call the non-generic method to find entities specifically for Friends
            return await FindFriendsByColumnAsync("accountid", accountId);
        }

        /// <summary>
        /// Finds multiple friend entities by a specified column asynchronously.
        /// </summary>
        /// <param name="columnName">The name of the column to search.</param>
        /// <param name="value">The value to search for.</param>
        /// <returns>A list of friend entities if found; otherwise, an empty list.</returns>
        private async Task<List<Friends>> FindFriendsByColumnAsync(string columnName, string value)
        {
            var stopwatch = Stopwatch.StartNew();

            using var connection = CreateConnection();
            await OpenConnectionAsync(connection);

            var query = $"SELECT * FROM {EntityMapper.GetTableName(new Friends())} WHERE {columnName} = @Value";
            var result = (await connection.QueryAsync<Friends>(query, new { Value = value })).ToList();

            stopwatch.Stop();
            return result;
        }

        /// <summary>
        /// Finds a profile entity by its profile ID and account ID asynchronously.
        /// </summary>
        /// <param name="profileId">The profile ID of the profile.</param>
        /// <param name="accountId">The account ID associated with the profile.</param>
        /// <returns>The profile instance if found; otherwise, null.</returns>
        public async Task<TEntity?> FindByProfileIdAndAccountIdAsync(string profileId, string accountId)
        {
            var stopwatch = Stopwatch.StartNew();
            EnsureIsCorrectEntity();

            using var connection = CreateConnection();
            await OpenConnectionAsync(connection);

            var query = $"SELECT DISTINCT * FROM {EntityMapper.GetTableName(new TEntity())} WHERE profileid = @ProfileId AND accountid = @AccountId";

            Logger.Information($"Executing Query: {query}, ProfileId: {profileId}, AccountId: {accountId}");

            try
            {
                var result = await connection.QueryFirstOrDefaultAsync<TEntity>(query, new { ProfileId = profileId, AccountId = accountId });
                return result;
            }
            catch (NpgsqlException npgsqlEx)
            {
                Logger.Error($"PostgreSQL error: {npgsqlEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error($"An error occured: {ex.Message}");
                throw;
            }
            finally
            {
                stopwatch.Stop();
                Logger.Information($"FindByProfileIdAndAccountIdAsync took {stopwatch.ElapsedMilliseconds} ms");
            }
        }


        /// <summary>
        /// Finds a user entity by its Discord ID asynchronously.
        /// </summary>
        /// <param name="discordId">The Discord ID of the user.</param>
        /// <returns>The user instance if found; otherwise, null.</returns>
        public async Task<TEntity> FindByDiscordIdAsync(string discordId)
        {
            EnsureIsCorrectEntity();
            return await FindByColumnAsync("discordid", discordId);
        }

        /// <summary>
        /// Finds a user entity by its email asynchronously.
        /// </summary>
        /// <param name="email">The email of the user.</param>
        /// <returns>The user instance if found; otherwise, null.</returns>
        public async Task<TEntity> FindByEmailAsync(string email)
        {
            EnsureIsCorrectEntity();
            return await FindByColumnAsync("email", email);
        }

        /// <summary>
        /// Finds a user entity by its username asynchronously.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <returns>The user instance if found; otherwise, null.</returns>
        public async Task<TEntity> FindByUsernameAsync(string username)
        {
            EnsureIsCorrectEntity();
            return await FindByColumnAsync("username", username);
        }

        /// <summary>
        /// Finds a user entity by its account ID asynchronously.
        /// </summary>
        /// <param name="accountId">The account ID of the user.</param>
        /// <returns>The user instance if found; otherwise, null.</returns>
        public async Task<TEntity> FindByAccountIdAsync(string accountId)
        {
            EnsureIsCorrectEntity();

            return await FindByColumnAsync("accountid", accountId);
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
        /// Finds an item entity by its template ID asynchronously.
        /// </summary>
        /// <param name="templateId">The template ID of the item.</param>
        /// <returns>The item instance if found; otherwise, null.</returns>
        public async Task<TEntity> FindByTemplateIdAsync(string templateId)
        {
            EnsureIsCorrectEntity();
            return await FindByColumnAsync("templateid", templateId);
        }
        /// <summary>
        /// Finds an item entity by its lockerName asynchronously.
        /// </summary>
        /// <param name="templateId">The lockerName of the item.</param>
        /// <returns>The item instance if found; otherwise, null.</returns>
        public async Task<TEntity> FindByLockerNameAsync(string templateId)
        {
            EnsureIsCorrectEntity();
            return await FindByColumnAsync("lockername", templateId);
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
        /// Finds a token entity by its account ID and type asynchronously.
        /// </summary>
        /// <param name="accountId">The account ID associated with the token.</param>
        /// <param name="type">The type of the token.</param>
        /// <returns>The token instance if found; otherwise, null.</returns>
        public async Task<TEntity> FindTokenByAccountIdAndTypeAsync(string accountId, string type)
        {
            EnsureTokensEntity();

            var stopwatch = Stopwatch.StartNew();
            using var connection = CreateConnection();
            await OpenConnectionAsync(connection);

            var query = $@"
        SELECT * 
        FROM {EntityMapper.GetTableName(new TEntity())} 
        WHERE accountid = @AccountId AND type = @Type;";

            try
            {
                var result = await connection.QueryFirstOrDefaultAsync<TEntity>(
                    query,
                    new { AccountId = accountId, Type = type }
                );

                stopwatch.Stop();
                Log.Information("FindTokenByAccountIdAndTypeAsync took {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);
                return result;
            }
            catch (NpgsqlException npgsqlEx)
            {
                Log.Error($"PostgreSQL error: {npgsqlEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred: {ex.Message}");
                throw;
            }
        }


        /// <summary>
        /// Queries the database asynchronously with optimized performance.
        /// </summary>
        /// <param name="query">The SQL query string.</param>
        /// <param name="parameters">Query parameters, if any.</param>
        /// <returns>A list of results mapped to TEntity.</returns>
        public async Task<IEnumerable<TResult>> QueryAsync<TResult>(string query, object parameters)
        {
            var stopwatch = Stopwatch.StartNew();

            using var connection = CreateConnection();
            try
            {
                Log.Information("Executing SQL Query: {Query}, Parameters: {Parameters}", query, parameters);
                var result = await connection.QueryAsync<TResult>(query, parameters);

                stopwatch.Stop();
                Log.Information("QueryAsync executed in {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "QueryAsync encountered an error: {Message}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Finds an entity by its token and type asynchronously.
        /// </summary>
        /// <param name="token">The token to search for.</param>
        /// <param name="type">The type associated with the token.</param>
        /// <returns>The entity instance if found; otherwise, null.</returns>
        public async Task<TEntity> FindByTokenAndTypeAsync(string token, string type)
        {
            EnsureTokensEntity();

            var stopwatch = Stopwatch.StartNew();
            using var connection = CreateConnection();
            await OpenConnectionAsync(connection);

            var query = $@"
        SELECT * 
        FROM {EntityMapper.GetTableName(new TEntity())} 
        WHERE token = @Token AND type = @Type";

            try
            {
                var result = await connection.QueryFirstOrDefaultAsync<TEntity>(query, new { Token = token, Type = type });
                return result;
            }
            catch (NpgsqlException npgsqlEx)
            {
                Logger.Error($"PostgreSQL error: {npgsqlEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error($"An error occurred: {ex.Message}");
                throw;
            }
            finally
            {
                stopwatch.Stop();
                Logger.Information($"FindByTokenAndTypeAsync took {stopwatch.ElapsedMilliseconds} ms");
            }
        }

        /// <summary>
        /// Ensures the entity is correct.
        /// </summary>
        private void EnsureIsCorrectEntity()
        {
            if (typeof(TEntity) != typeof(User) &&
                typeof(TEntity) != typeof(Profiles) &&
                typeof(TEntity) != typeof(Items) &&
                typeof(TEntity) != typeof(Loadouts) &&
                typeof(TEntity) != typeof(Friends))
            {
                throw new InvalidOperationException("This method is only available for User, Profiles, Items, Loadouts, Friends entities.");
            }
        }

        /// <summary>
        /// Ensures the entity is of type Friends.
        /// </summary>
        private void EnsureFriendsEntity()
        {
            if (typeof(TEntity) != typeof(Friends))
                throw new InvalidOperationException("This method is only available for Friends entities.");
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