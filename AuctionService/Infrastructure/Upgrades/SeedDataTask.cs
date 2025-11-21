using System.Threading;
using System.Threading.Tasks;
using AuctionService.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Common.Application.Abstractions;

namespace AuctionService.Infrastructure.Upgrades;

public class SeedDataTask : IUpgradeTask
{
    private readonly IAppLogger<SeedDataTask> _logger;

    public SeedDataTask(IAppLogger<SeedDataTask> logger)
    {
        _logger = logger;
    }

    public Task RunAsync(IServiceProvider services, CancellationToken cancellationToken)
    {
        using var scope = services.CreateScope();
        var appServices = scope.ServiceProvider;
        var app = appServices.GetRequiredService<IServiceProvider>();
        var context = appServices.GetRequiredService<AuctionDbContext>();

        if (!context.Auctions.Any())
        {
            _logger.LogInformation("Seeding initial data...");
            DbInitializer.InitDbShim(context);
            _logger.LogInformation("Seeding completed");
        }

        return Task.CompletedTask;
    }
}
