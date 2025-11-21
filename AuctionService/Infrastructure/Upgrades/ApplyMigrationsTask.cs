using System.Threading;
using System.Threading.Tasks;
using AuctionService.Infrastructure.Data;
using Common.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AuctionService.Infrastructure.Upgrades;

public class ApplyMigrationsTask : IUpgradeTask
{
    private readonly IAppLogger<ApplyMigrationsTask> _logger;

    public ApplyMigrationsTask(IAppLogger<ApplyMigrationsTask> logger)
    {
        _logger = logger;
    }

    public async Task RunAsync(IServiceProvider services, CancellationToken cancellationToken)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AuctionDbContext>();

        _logger.LogInformation("Applying database migrations...");
        await context.Database.MigrateAsync(cancellationToken);
        _logger.LogInformation("Database migrations applied successfully");
    }
}
