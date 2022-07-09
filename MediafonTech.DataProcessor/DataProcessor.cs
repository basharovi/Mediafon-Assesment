using MediafonTech.ApplicationCore.Interfaces.Services;
using Serilog;

namespace MediafonTech.DataProcessor
{
    public class DataProcessor : BackgroundService
    {
        private readonly IDataSyncService _syncService;

        public DataProcessor(IDataSyncService syncService)
        {
            _syncService = syncService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //The hosted service, which processes data after every 1 minute
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    Log.Information("Data Processor running at: {0}", DateTimeOffset.Now);

                    await _syncService.StartDataProcessing();
                }
                catch (Exception ex)
                {
                    Log.Error(ex?.InnerException.ToString());
                    Log.Error(ex.ToString());
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}