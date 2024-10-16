using Serilog;
using Larry.Source.Database;
using Larry.Source.Database.Entities;
using Larry.Source.Repositories;
using Larry.Source.Utilities;
using Microsoft.AspNetCore;

namespace Larry
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                string connectionString = "Host=localhost;Port=5432;Database=dbtest;Username=postgres;Password=a";

                using var dbContext = new DatabaseContext(connectionString);

                var builder = WebApplication.CreateBuilder(args);

                builder.Services.AddControllers();
                builder.Services.AddEndpointsApiExplorer();
                builder.Host.UseSerilog();

                var app = builder.Build();

           
                app.UseHttpsRedirection();
                app.UseAuthorization();
                app.MapControllers();
                app.Run();
            }
            catch (Exception ex)
            {
                Logger.Error($"An error occurred: {ex.Message}");
            }
        }
    }
}
