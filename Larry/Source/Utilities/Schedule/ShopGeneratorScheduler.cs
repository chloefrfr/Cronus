namespace Larry.Source.Utilities.Schedule
{
    public class ShopGeneratorScheduler: IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IServiceProvider _serviceProvider;

        public ShopGeneratorScheduler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.Information("Starting Storefront Schedule.");

            var now = DateTime.UtcNow;
            var nextRunTime = new DateTime(now.Year, now.Month, now.Day).AddDays(1);
            var timeToNextRun = nextRunTime - now;

            _timer = new Timer(ExecuteTask, null, timeToNextRun, TimeSpan.FromDays(1));

            Logger.Information("Shop scheduled to run at 00:00 UTC daily.");

            return Task.CompletedTask;
        }

        private async void ExecuteTask(object state)
        {
            Logger.Information($"ShopGeneratorScheduler triggered at {DateTime.UtcNow} UTC.");
            using (var scope = _serviceProvider.CreateScope())
            {
                var shopGenerator = scope.ServiceProvider.GetRequiredService<ShopGenerator.Storefront.Services.ShopGenerator>();
                await shopGenerator.GenerateShopAsync();
                Logger.Information($"ShopGenerator completed successfully at {DateTime.UtcNow} UTC.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Logger.Information("Stopping ShopGeneratorScheduler...");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
