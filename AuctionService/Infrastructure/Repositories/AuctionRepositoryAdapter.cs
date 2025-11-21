using System.Threading;
using System.Threading.Tasks;
using AuctionService.Application.Interfaces;
using AuctionService.Domain.Entities;
using Common.Application.Interfaces;

namespace AuctionService.Infrastructure.Repositories
{
    public class AuctionRepositoryAdapter : IAuctionRepository
    {
        private readonly IRepository<Auction> _inner;

        public AuctionRepositoryAdapter(IRepository<Auction> inner)
        {
            _inner = inner;
        }

        public Task<IEnumerable<Auction>> GetAllAsync(CancellationToken cancellationToken = default)
            => _inner.GetAllAsync(cancellationToken);

        public Task<Auction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => _inner.GetByIdAsync(id, cancellationToken);

        public Task<Auction> CreateAsync(Auction entity, CancellationToken cancellationToken = default)
            => _inner.CreateAsync(entity, cancellationToken);

        public Task<IEnumerable<Auction>> AddRangeAsync(IEnumerable<Auction> entities, CancellationToken cancellationToken = default)
            => _inner.AddRangeAsync(entities, cancellationToken);

        public Task UpdateAsync(Auction entity, CancellationToken cancellationToken = default)
            => _inner.UpdateAsync(entity, cancellationToken);

        public Task UpdateRangeAsync(IEnumerable<Auction> entities, CancellationToken cancellationToken = default)
            => _inner.UpdateRangeAsync(entities, cancellationToken);

        public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
            => _inner.DeleteAsync(id, cancellationToken);

        public Task DeleteRangeAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
            => _inner.DeleteRangeAsync(ids, cancellationToken);

        public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
            => _inner.ExistsAsync(id, cancellationToken);
    }
}
