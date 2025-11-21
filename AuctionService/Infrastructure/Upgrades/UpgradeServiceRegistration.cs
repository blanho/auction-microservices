using Microsoft.Extensions.DependencyInjection;

namespace AuctionService.Infrastructure.Upgrades;

public static class UpgradeServiceRegistration
{
    public static IServiceCollection AddAuctionUpgradeTasks(this IServiceCollection services)
    {
        services.AddScoped<IUpgradeTask, ApplyMigrationsTask>();
        services.AddScoped<IUpgradeTask, SeedDataTask>();
        services.AddHostedService<UpgradeRunnerHostedService>();
        return services;
    }
}
