using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace AuctionService.Infrastructure.Upgrades;

public interface IUpgradeTask
{
    Task RunAsync(IServiceProvider services, CancellationToken cancellationToken);
}
