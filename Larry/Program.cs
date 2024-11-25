using Serilog;
using Larry.Source.Database;
using Larry.Source.Database.Entities;
using Larry.Source.Repositories;
using Larry.Source.Utilities;
using Microsoft.AspNetCore;
using Larry.Source.Discord;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using Newtonsoft.Json;
using Larry.Source.Utilities.Managers;
using ShopGenerator.Storefront.Services;
using Larry.Source.Utilities.Schedule;

namespace Larry
{
    class Program
    {
        public static FileProviderManager _fileProviderManager;
        private static ShopGenerator.Storefront.Services.ShopGenerator _shopGenerator;

        static async Task Main(string[] args)
        {
            try
            {
                Config config = Config.GetConfig();

                using var dbContext = new DatabaseContext(config.ConnectionUrl);

                var builder = WebApplication.CreateBuilder(args);

                builder.Services.AddSingleton<Config>(Config.GetConfig());
                builder.Services.AddMemoryCache();

                builder.Services.AddControllers();
                builder.Services.AddEndpointsApiExplorer();
                builder.Host.UseSerilog();
                builder.Services.AddSingleton<FileProviderManager>();
                builder.Services.AddHttpClient<IAPIService, APIService>();
                builder.Services.AddSingleton<ShopGenerator.Storefront.Services.ShopGenerator>();

                builder.Services.AddHostedService<ShopGeneratorScheduler>();

                builder.Services.AddHttpContextAccessor();

                var app = builder.Build();

                _fileProviderManager = app.Services.GetRequiredService<FileProviderManager>();
                await _fileProviderManager.InitializeAsync();
                await _fileProviderManager.LoadAllCosmeticsAsync();

                _shopGenerator = app.Services.GetRequiredService<ShopGenerator.Storefront.Services.ShopGenerator>();

                app.UseHttpsRedirection();
                app.UseAuthorization();
                app.MapControllers();

                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async ctx =>
                    {
                        var exceptionHandlerPathFeature = ctx.Features.Get<IExceptionHandlerPathFeature>();
                        if (exceptionHandlerPathFeature != null)
                        {
                            var error = Errors.CreateError(
                                    (int)HttpStatusCode.InternalServerError,
                                    ctx.Request.Path,
                                    exceptionHandlerPathFeature.Error.Message,
                                    DateTime.UtcNow.ToString("o")
                                );

                            ctx.Response.ContentType = "application/json";
                            ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            await ctx.Response.WriteAsync(JsonConvert.SerializeObject(error));
                        }
                    });
                });

                app.UseStatusCodePages(async context =>
                {
                    context.HttpContext.Response.ContentType = "application/json";
                    if (context.HttpContext.Response.StatusCode == (int)HttpStatusCode.NotFound)
                    {
                        var error = Errors.CreateError(
                            (int)HttpStatusCode.NotFound,
                            context.HttpContext.Request.Path,
                            "The requested resource was not found.",
                            DateTime.UtcNow.ToString("o")
                        );

                        await context.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(error));
                    }
                });

                var bot = new Bot();

                var botTask = bot.RunAsync(config.Token);
                var webAppTask = app.RunAsync();

                if (config.GenerateShopImmediately)
                {
                    Logger.Information("Generating shop immediately.");
                    await _shopGenerator.GenerateShopAsync();
                }

                await Task.WhenAll(botTask, webAppTask);
            }
            catch (Exception ex)
            {
                Logger.Error($"An error occurred: {ex.Message}");
            }
        }
    }
}
