using Erc20Listener.Config;
using Microsoft.Extensions.Options;

namespace Erc20Listener.Services;

public class Worker(ILogger<Worker> logger, IOptions<AppSettings> options, IServiceScopeFactory serviceScopeFactory)
    : BackgroundService
{
    private async Task RunLoop(string network, TimeSpan pollInterval, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("start listen loop for network {Network}, poll interval = {PollInterval}", network,
                pollInterval);

            using var periodicTimer = new PeriodicTimer(pollInterval);

            while (await periodicTimer.WaitForNextTickAsync(cancellationToken))
            {
                using var serviceScope = serviceScopeFactory.CreateScope();
                var erc20EventsFetcher = serviceScope.ServiceProvider.GetRequiredService<IErc20EventsFetcher>();
                await erc20EventsFetcher.Fetch(network);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "poll error");
        }
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        foreach (var listenConfig in options.Value.ListenConfigs)
        {
            _ = Task.Run(
                () => RunLoop(listenConfig.Network, TimeSpan.FromSeconds(listenConfig.PollIntervalSeconds),
                    stoppingToken), stoppingToken);
        }

        return Task.CompletedTask;
    }
}