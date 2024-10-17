using Npgsql;
using System.Data;

namespace Larry.Source.Repositories
{
    public abstract class AsyncRepositoryBase
    {
        private readonly string _connectionString;

        protected AsyncRepositoryBase(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected IDbConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }

        /// <summary>
        /// Opens a database connection asynchronously.
        /// </summary>
        protected async Task<IDbConnection> OpenConnectionAsync()
        {
            var connection = CreateConnection();
            await Task.Run(() => connection.Open());
            return connection;
        }

        /// <summary>
        /// Begins a database transaction asynchronously.
        /// </summary>
        protected async Task<IDbTransaction> BeginTransactionAsync(IDbConnection connection)
        {
            return await Task.Run(() => connection.BeginTransaction());
        }
    }
}
