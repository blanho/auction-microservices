using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;

namespace Common.OpenApi.Extensions;

public static class OpenApiExtensions
{
    public static IServiceCollection AddCommonOpenApi(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddCommonSwaggerGen();
        return services;
    }

    public static IApplicationBuilder UseCommonOpenApi(this IApplicationBuilder app)
    {
        var webApp = (WebApplication)app;
        webApp.UseSwagger(options =>
        {
            options.RouteTemplate = "openapi/{documentName}.json";
        });
        return app;
    }
}
