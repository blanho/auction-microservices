using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Common.Infrastructure.Caching;
using Common.Infrastructure.CorrelationId;
using Common.Infrastructure.Logging;
using Common.Infrastructure.Services;
using Common.Application.Abstractions;

namespace Common.Infrastructure.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommonLogging(this IServiceCollection services)
    {
        services.AddScoped(typeof(IAppLogger<>), typeof(AppLogger<>));
        return services;
    }

    public static IServiceCollection AddCorrelationId(this IServiceCollection services)
    {
        services.AddScoped<ICorrelationIdService, CorrelationIdService>();
        return services;
    }

    public static IServiceCollection AddDistributedCacheService(this IServiceCollection services)
    {
        services.AddScoped<IDistributedCacheService, DistributedCacheService>();
        return services;
    }

    public static IServiceCollection AddCommonUtilities(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
        return services;
    }
}


public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        return app;
    }
}
