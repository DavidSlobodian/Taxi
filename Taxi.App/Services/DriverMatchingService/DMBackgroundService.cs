using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taxi.App.Services.DriverMatchingService
{
    public class DMBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        //private readonly ILogger<DMBackgroundService> _logger;

        public DMBackgroundService(
            IServiceScopeFactory scopeFactory)
            //ILogger<DMBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            //_logger = logger;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    var matchingService = scope.ServiceProvider
                        .GetRequiredService<IDriverMatchingService>();

                    await matchingService.ProcessPendingMatches();
                }
                catch (OperationCanceledException)
                    when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    //_logger.LogError(
                        //ex, "Error while processing ride matching.");
                }

                try
                {
                    if (!await timer.WaitForNextTickAsync(stoppingToken))
                        break;
                }
                catch (OperationCanceledException)
                    when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
            }
        }
    }
}
