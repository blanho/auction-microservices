using AuctionService.Application.Interfaces;
using AuctionService.Application.Services;
using AuctionService.Infrastructure.Data;
using AuctionService.Infrastructure.Repositories;
using AuctionService.Infrastructure.Logging;
using Microsoft.EntityFrameworkCore;
using AuctionService.Domain.Entities;
using AuctionService.Infrastructure.Upgrades;
using Common.Caching.Abstractions;
using Common.Caching.Implementations;
using Common.Repository.Interfaces;

namespace AuctionService.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddDbContext<AuctionDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            
            // Register logger adapter
            services.AddScoped(typeof(IAppLogger<>), typeof(AppLogger<>));

            // Register repository directly (caching disabled for now - can add decorator later)
            services.AddScoped<IAuctionRepository, AuctionRepository>();

            services.AddScoped<IAuctionService, AuctionServiceImpl>();

            services.AddAuctionUpgradeTasks();

            return services;
        }
    }
}
