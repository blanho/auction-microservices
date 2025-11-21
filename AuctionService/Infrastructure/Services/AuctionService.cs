using AuctionService.Application.DTOs;
using AuctionService.Application.Interfaces;
using AutoMapper;
using Common.Application.Abstractions;
using Common.Application.Errors;

namespace AuctionService.Infrastructure.Services;

public class AuctionServiceImpl : IAuctionService
{
    private readonly IAuctionRepository _repository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<AuctionServiceImpl> _logger;
    private readonly IDateTimeProvider _dateTime;

    public AuctionServiceImpl(
        IAuctionRepository repository, 
        IMapper mapper,
        IAppLogger<AuctionServiceImpl> logger,
        IDateTimeProvider dateTime)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
        _dateTime = dateTime;
    }

    public async Task<List<AuctionDto>> GetAllAuctionsAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching all auctions at {Timestamp}", _dateTime.UtcNow);

        // Repository is automatically cached via CachedRepository decorator
        var auctions = await _repository.GetAllAsync(cancellationToken);
        var result = auctions.Select(a => _mapper.Map<AuctionDto>(a)).ToList();
        
        _logger.LogInformation("Retrieved {Count} auctions", result.Count);
        return result;
    }

    public async Task<AuctionDto> GetAuctionByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching auction {AuctionId}", id);

        // Repository is automatically cached via CachedRepository decorator
        var auction = await _repository.GetByIdAsync(id, cancellationToken);
        
        if (auction == null)
        {
            _logger.LogWarning("Auction {AuctionId} not found", id);
            throw new NotFoundException($"Auction with ID {id} was not found");
        }
        
        return _mapper.Map<AuctionDto>(auction);
    }

    public async Task<AuctionDto> CreateAuctionAsync(CreateAuctionDto dto, string seller, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating auction for seller {Seller} at {Timestamp}", seller, _dateTime.UtcNow);

        var auction = _mapper.Map<Domain.Entities.Auction>(dto);
        auction.Seller = seller;
        
        // Repository automatically handles cache invalidation via decorator
        var createdAuction = await _repository.CreateAsync(auction, cancellationToken);
        
        _logger.LogInformation("Created auction {AuctionId} for seller {Seller}", createdAuction.Id, seller);
        return _mapper.Map<AuctionDto>(createdAuction);
    }

    public async Task<bool> UpdateAuctionAsync(Guid id, UpdateAuctionDto dto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating auction {AuctionId} at {Timestamp}", id, _dateTime.UtcNow);

        var auction = await _repository.GetByIdAsync(id, cancellationToken);
        
        if (auction == null)
        {
            _logger.LogWarning("Auction {AuctionId} not found for update", id);
            throw new NotFoundException($"Auction with ID {id} was not found");
        }

        auction.Item.Make = dto.Make ?? auction.Item.Make;
        auction.Item.Model = dto.Model ?? auction.Item.Model;
        auction.Item.Color = dto.Color ?? auction.Item.Color;
        auction.Item.Mileage = dto.Mileage ?? auction.Item.Mileage;
        auction.Item.Year = dto.Year ?? auction.Item.Year;
        
        // Repository automatically handles cache invalidation via decorator
        await _repository.UpdateAsync(auction, cancellationToken);
        
        _logger.LogInformation("Updated auction {AuctionId}", id);
        return true;
    }

    public async Task<bool> DeleteAuctionAsync(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting auction {AuctionId} at {Timestamp}", id, _dateTime.UtcNow);

        var exists = await _repository.ExistsAsync(id, cancellationToken);
        
        if (!exists)
        {
            _logger.LogWarning("Auction {AuctionId} not found for deletion", id);
            throw new NotFoundException($"Auction with ID {id} was not found");
        }

        // Repository automatically handles cache invalidation via decorator
        await _repository.DeleteAsync(id, cancellationToken);
        
        _logger.LogInformation("Deleted auction {AuctionId}", id);
        return true;
    }
}
