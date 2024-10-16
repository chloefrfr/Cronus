using Npgsql;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Transactions;
using Cronus.Source.Database.Attributes;
using Cronus.Source.Database.Entities;
using Cronus.Source.Repositories;

namespace Cronus.Source.Database
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

            _connection = new NpgsqlConnection(connectionString);
            _connection.Open();

            Log.Information("Database connection opened.");

            CreateTables();
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
                CreateTable(entityType);
            }
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
                Log.Error($"Entity '{entityType.Name}' does not have a TableAttribute specified.");
                return;
            }

            if (TableExists(tableName))
            {
                Log.Information($"Table '{tableName}' already exists.");
                return;
            }

            var columns = entityType.GetProperties()
                .Where(p => p.Name != "Id") // Exclude Id to avoid duplicates
                .Select(p => $"{p.GetCustomAttribute<ColumnAttribute>()?.ColumnName ?? p.Name} {GetPostgresType(p.PropertyType)}");

            var primaryKey = "Id SERIAL PRIMARY KEY"; // Define Id here
            var sql = $"CREATE TABLE IF NOT EXISTS {tableName} ({primaryKey}, {string.Join(", ", columns)});";

            try
            {
                using (var command = new NpgsqlCommand(sql, _connection))
                {
                    command.ExecuteNonQuery();
                    Log.Information($"Table {tableName} created.");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error creating table {tableName}: {ex.Message}");
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
            if (type.IsArray)
            {
                var elementType = type.GetElementType();

                var postgesType = GetPostgresType(elementType);
                return $"{postgesType}[]";
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
        /// Retrieves or creates a new database connection asynchronously.
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
