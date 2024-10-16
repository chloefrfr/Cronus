using Npgsql;

namespace Cronus.Source.Database.Migrations
{
    /// <summary>
    /// Base class for database migrations.
    /// </summary>
    public abstract class Migration
    {
        public abstract Task UpAsync(NpgsqlConnection connection);
        public abstract Task DownAsync(NpgsqlConnection connection);
    }
}
