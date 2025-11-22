using SearchService.Application.Interfaces;
using SearchService.Application.Services;
using SearchService.Infrastructure.Data;
using SearchService.Infrastructure.Repositories;
using MongoDB.Driver;
using Common.Caching.Abstractions;
using Common.Caching.Implementations;
using Common.Repository.Interfaces;
using Common.Repository.Implementations;
using AutoMapper;
using Serilog;

namespace SearchService.API.Extensions
{
    public static class ServiceExtensions
    {
        
        
        
        public static WebApplicationBuilder AddApplicationLogging(this WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog((context, loggerConfig) =>
            {
                loggerConfig
                    .ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.WithEnvironmentUserName();
            });

            return builder;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            
            
            var mongoSettings = new MongoDbSettings();
            configuration.GetSection("MongoDb").Bind(mongoSettings);
            services.AddSingleton(mongoSettings);
            services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoSettings.ConnectionString));
            services.AddSingleton<MongoSearchDbContext>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            
            
            services.AddScoped(typeof(IAppLogger<>), typeof(AppLogger<>));
            
            services.AddScoped<SearchItemRepository>(); 
            services.AddScoped<ISearchItemRepository>(sp =>
            {
                var inner = sp.GetRequiredService<SearchItemRepository>();
                var cache = sp.GetRequiredService<ICacheService>();
                var mapper = sp.GetRequiredService<IMapper>();
                var logger = sp.GetRequiredService<IAppLogger<CachedSearchItemRepository>>();
                return new CachedSearchItemRepository(inner, cache, mapper, logger);
            });

            services.AddScoped<ISearchService, SearchServiceImpl>();

            return services;
        }
    }
}