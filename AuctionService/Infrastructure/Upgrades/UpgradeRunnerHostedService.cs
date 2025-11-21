using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AuctionService.Infrastructure.Upgrades;

public class UpgradeRunnerHostedService : IHostedService
{
    private readonly IServiceProvider _services;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<UpgradeRunnerHostedService> _logger;

    public UpgradeRunnerHostedService(
        IServiceProvider services,
        IServiceScopeFactory scopeFactory,
        ILogger<UpgradeRunnerHostedService> logger)
    {
        _services = services;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting upgrade tasks...");
        using var scope = _scopeFactory.CreateScope();
        var tasks = scope.ServiceProvider.GetServices<IUpgradeTask>();
        foreach (var task in tasks)
        {
            var name = task.GetType().Name;
            _logger.LogInformation("Running upgrade task: {Task}", name);
            await task.RunAsync(scope.ServiceProvider, cancellationToken);
            _logger.LogInformation("Completed upgrade task: {Task}", name);
        }
        _logger.LogInformation("All upgrade tasks completed");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
