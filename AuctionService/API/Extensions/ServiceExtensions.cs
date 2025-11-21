using AuctionService.Application.Interfaces;
using AuctionService.Application.Services;
using AuctionService.Infrastructure.Data;
using AuctionService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Common.Infrastructure.Extensions;
using Common.Application.Interfaces;
using AuctionService.Domain.Entities;
using AuctionService.Infrastructure.Upgrades;

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

            services.AddCachedRepository<Auction, AuctionRepository>(
                servicePrefix: "auction",
                cacheExpiration: TimeSpan.FromMinutes(10));

            services.AddScoped<IAuctionRepository>(provider =>
            {
                var genericRepo = provider.GetRequiredService<IRepository<Auction>>();
                return new AuctionRepositoryAdapter(genericRepo);
            });

            services.AddScoped<IAuctionService, AuctionServiceImpl>();

            services.AddAuctionUpgradeTasks();

            return services;
        }
    }
}
