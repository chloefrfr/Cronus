using Serilog;
using Larry.Source.Database;
using Larry.Source.Database.Entities;
using Larry.Source.Repositories;
using Larry.Source.Utilities;
using Microsoft.AspNetCore;
using Larry.Source.Discord;

namespace Larry
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                Config config;

                try
                {
                    config = Config.Load();
                }
                catch (Exception ex)
                {
                    Logger.Error($"Failed to load config: {ex.Message}");
                    return;
                }

                using var dbContext = new DatabaseContext(config.ConnectionUrl);

                var builder = WebApplication.CreateBuilder(args);

                builder.Services.AddControllers();
                builder.Services.AddEndpointsApiExplorer();
                builder.Host.UseSerilog();

                var app = builder.Build();


                app.UseHttpsRedirection();
                app.UseAuthorization();
                app.MapControllers();

                var bot = new Bot();

                var botTask = bot.RunAsync(config.Token);
                var webAppTask = app.RunAsync();

                await Task.WhenAll(botTask, webAppTask);
            }
            catch (Exception ex)
            {
                Logger.Error($"An error occurred: {ex.Message}");
            }
        }
    }
}
