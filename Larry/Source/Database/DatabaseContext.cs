using Npgsql;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Transactions;
using Larry.Source.Database.Attributes;
using Larry.Source.Database.Entities;
using Larry.Source.Repositories;
using Larry.Source.Utilities;

namespace Larry.Source.Database
{
    /// <summary>
    /// Represents a context for interacting with the database, managing connections, and performing CRUD operations.
    /// </summary>
    public class DatabaseContext : IDisposable
    {
        private readonly string _connectionString;
        private readonly ConcurrentDictionary<Type, object> _repositories = new();
        private NpgsqlConnection _connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseContext"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string to the database.</param>
        public DatabaseContext(string connectionString)
        {
            _connectionString = connectionString;

            try
            {
                _connection = new NpgsqlConnection(connectionString);
                _connection.Open();
                Logger.Information("Database connection opened.");
                CreateTables();
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to open database connection: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Gets the current database connection.
        /// </summary>
        public NpgsqlConnection Connection => _connection;

        /// <summary>
        /// Creates the necessary tables based on the registered entities.
        /// </summary>
        private void CreateTables()
        {
            var entityTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.GetCustomAttributes(typeof(EntityAttribute), true).Any());

            foreach (var entityType in entityTypes)
            {
                CreateOrUpdateTable(entityType);
            }
        }

        /// <summary>
        /// Creates a new table or updates an existing table based on the specified entity type.
        /// </summary>
        /// <param name="entityType">The type of the entity to create or update.</param>
        private void CreateOrUpdateTable(Type entityType)
        {
            var tableName = entityType.GetCustomAttribute<EntityAttribute>()?.TableName;

            if (string.IsNullOrEmpty(tableName))
            {
                Logger.Error($"Entity '{entityType.Name}' does not have a TableAttribute specified.");
                return;
            }

            if (!TableExists(tableName))
            {
                CreateTable(entityType);
            }
        }


        /// <summary>
        /// Gets the data type of a specified column in a given table.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="columnName">The name of the column to retrieve the type for.</param>
        /// <returns>The data type of the specified column as a string.</returns>
        private string GetColumnType(string tableName, string columnName)
        {            
            var sql = $"SELECT data_type FROM information_schema.columns WHERE table_name = @tableName AND column_name = @columnName;";

            using (var command = new NpgsqlCommand(sql, _connection))
            {
                command.Parameters.AddWithValue("@tableName", tableName);
                command.Parameters.AddWithValue("@columnName", columnName);

                return command.ExecuteScalar()?.ToString() ?? string.Empty;
            }
        }

        /// <summary>
        /// Checks if the column type matches the specified type in the database.
        /// </summary>
        /// <param name="tableName">The name of the table to check.</param>
        /// <param name="columnName">The name of the column to check.</param>
        /// <param name="expectedType">The expected PostgreSQL type of the column.</param>
        /// <returns>True if the column type matches; otherwise, false.</returns>
        private bool IsColumnTypeMatching(string tableName, string columnName, string expectedType)
        {
            var sql = $@"
                SELECT data_type 
                FROM information_schema.columns 
                WHERE table_name = '{tableName}' AND column_name = '{columnName}';";

            using (var command = new NpgsqlCommand(sql, _connection))
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    var actualType = reader.GetString(0);
                    return actualType.Equals(expectedType, StringComparison.OrdinalIgnoreCase);
                }
            }

            return false;
        }


        /// <summary>
        /// Gets the existing columns for a specified table.
        /// </summary>
        /// <param name="tableName">The name of the table to check for existing columns.</param>
        /// <returns>A set of existing column names.</returns>
        private HashSet<string> GetExistingColumns(string tableName)
        {
            var columns = new HashSet<string>();
            var sql = $"SELECT column_name FROM information_schema.columns WHERE table_name = '{tableName}';";
            using (var command = new NpgsqlCommand(sql, _connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        columns.Add(reader.GetString(0));
                    }
                }
            }
            return columns;
        }

        /// <summary>
        /// Creates a table for the specified entity type if it does not already exist.
        /// </summary>
        /// <param name="entityType">The type of the entity.</param>
        private void CreateTable(Type entityType)
        {
            var tableName = entityType.GetCustomAttribute<EntityAttribute>()?.TableName;

            if (string.IsNullOrEmpty(tableName))
            {
                Logger.Error($"Entity '{entityType.Name}' does not have a TableAttribute specified.");
                return;
            }

            if (TableExists(tableName))
            {
                Logger.Information($"Table '{tableName}' already exists.");
                return;
            }

            var columns = entityType.GetProperties()
                .Where(p => p.Name != "Id") 
                .Select(p => $"{p.GetCustomAttribute<ColumnAttribute>()?.ColumnName ?? p.Name} {GetPostgresType(p.PropertyType)}");

            var primaryKey = "Id SERIAL PRIMARY KEY"; 
            var sql = $"CREATE TABLE IF NOT EXISTS {tableName} ({primaryKey}, {string.Join(", ", columns)});";

            try
            {
                using (var command = new NpgsqlCommand(sql, _connection))
                {
                    command.ExecuteNonQuery();
                    Logger.Information($"Table {tableName} created.");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error creating table {tableName}: {ex.Message}");
            }
        }

        /// <summary>
        /// Checks if a table exists in the database.
        /// </summary>
        /// <param name="tableName">The name of the table to check.</param>
        /// <returns>True if the table exists; otherwise, false.</returns>
        private bool TableExists(string tableName)
        {
            var sql = $"SELECT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = '{tableName}');";
            using (var command = new NpgsqlCommand(sql, _connection))
            {
                return (bool)command.ExecuteScalar();
            }
        }

        /// <summary>
        /// Maps C# types to PostgreSQL types.
        /// </summary>
        /// <param name="type">The type to map.</param>
        /// <returns>The corresponding PostgreSQL type as a string.</returns>
        private string GetPostgresType(Type type)
        {
            if (type == typeof(System.Dynamic.ExpandoObject) || type == typeof(object))
            {
                return "TEXT"; 
            }

            if (type.IsArray)
            {
                var elementType = type.GetElementType();
                var postgresType = GetPostgresType(elementType);
                return $"{postgresType}[]";
            }

            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
            return underlyingType switch
            {
                Type t when t == typeof(string) => "VARCHAR",
                Type t when t == typeof(int) => "INTEGER",
                Type t when t == typeof(long) => "BIGINT",
                Type t when t == typeof(double) => "DOUBLE PRECISION",
                Type t when t == typeof(float) => "REAL",
                Type t when t == typeof(bool) => "BOOLEAN",
                Type t when t == typeof(DateTime) => "TIMESTAMP",
                Type t when t == typeof(decimal) => "DECIMAL",
                Type t when t == typeof(short) => "SMALLINT",
                Type t when t == typeof(byte) => "BYTEA",
                Type t when t == typeof(char) => "CHAR",
                Type t when t == typeof(Guid) => "UUID",
                Type t when t == typeof(object) => "JSONB",
                _ => "TEXT" // Fallback for unsupported types.
            };
        }



        /// <summary>
        /// Gets the repository for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity managed by the repository.</typeparam>
        /// <returns>A <see cref="Repository{TEntity}"/> for the specified entity type.</returns>
        public Repository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new()
        {
            var type = typeof(TEntity);
            if (_repositories.ContainsKey(type))
            {
                return (Repository<TEntity>)_repositories[type];
            }

            var repository = new Repository<TEntity>(_connectionString);
            _repositories[type] = repository;
            return repository;
        }

        /// <summary>
        /// Begins a new transaction scope for managing multiple database operations.
        /// </summary>
        /// <returns>A <see cref="TransactionScope"/> that manages the transaction context.</returns>
        public TransactionScope BeginTransaction()
        {
            return new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        }

        /// <summary>
        /// Gets or creates a new database connection asynchronously.
        /// </summary>
        /// <returns>A <see cref="NpgsqlConnection"/> object that represents the database connection.</returns>
        public async Task<NpgsqlConnection> GetConnectionAsync()
        {
            if (_connection == null || _connection.State != System.Data.ConnectionState.Open)
            {
                _connection = new NpgsqlConnection(_connectionString);
                await _connection.OpenAsync();
            }

            return _connection;
        }

        /// <summary>
        /// Saves changes for a specific entity asynchronously.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity being saved.</typeparam>
        /// <param name="entity">The entity instance to save.</param>
        public async Task SaveChangesAsync<TEntity>(TEntity entity) where TEntity : BaseEntity, new()
        {
            var repository = GetRepository<TEntity>();
            await repository.SaveAsync(entity);
        }

        /// <summary>
        /// Finds an entity by its identifier asynchronously.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity to find.</typeparam>
        /// <param name="id">The unique identifier of the entity.</param>
        /// <returns>The entity instance if found; otherwise, null.</returns>
        public async Task<TEntity> FindByIdAsync<TEntity>(int id) where TEntity : BaseEntity, new()
        {
            var repository = GetRepository<TEntity>();
            return await repository.FindByIdAsync(id);
        }

        /// <summary>
        /// Deletes an entity by its ID asynchronously.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity to delete.</typeparam>
        /// <param name="id">The unique identifier of the entity to delete.</param>
        public async Task DeleteAsync<TEntity>(int id) where TEntity : BaseEntity, new()
        {
            var repository = GetRepository<TEntity>();
            // ICBA
        }

        /// <summary>
        /// Disposes the database connection and cleans up resources.
        /// </summary>
        public void Dispose()
        {
            if (_connection != null && _connection.State == System.Data.ConnectionState.Open)
            {
                _connection.Close();
                _connection.Dispose();
            }
        }
    }
}
