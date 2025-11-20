# CommonService

A shared class library containing common functionality used across all microservices to eliminate circular dependencies.

## Structure

```
CommonService/
  src/
    Domain/              # Shared domain entities and value objects
      Entities/          # Base entities (e.g., BaseEntity)
      ValueObjects/      # Shared value objects
    Application/         # Shared application layer
      Interfaces/        # Generic interfaces (e.g., IRepository)
      DTOs/              # Common DTOs (e.g., Result, PagedResult)
      Extensions/        # Helper extension methods
```

## Purpose

This service contains:

- **Base entities** - Common entity properties (Id, CreatedAt, UpdatedAt)
- **Generic interfaces** - Repository patterns, common contracts
- **Shared DTOs** - Result pattern, pagination, common responses
- **Extension methods** - String helpers, utility methods
- **Value objects** - Shared domain value objects

## Usage

Reference this service in other microservices:

```bash
dotnet add reference ../../CommonService/src/Domain/CommonService.Domain.csproj
dotnet add reference ../../CommonService/src/Application/CommonService.Application.csproj
```

## No API Layer

This service intentionally does NOT have an API project or controllers. It's purely a shared library that other services can depend on without creating circular dependencies.

## Examples

### Using BaseEntity

```csharp
using CommonService.Domain.Entities;

public class Auction : BaseEntity
{
    public string Title { get; set; }
    // Id, CreatedAt, UpdatedAt inherited from BaseEntity
}
```

### Using Result Pattern

```csharp
using CommonService.Application.DTOs;

public Result<Auction> GetAuction(Guid id)
{
    var auction = _repository.GetById(id);
    if (auction == null)
        return Result<Auction>.Failure("Auction not found");

    return Result<Auction>.Success(auction);
}
```

### Using Generic Repository

```csharp
using CommonService.Application.Interfaces;

public class AuctionRepository : IRepository<Auction>
{
    // Implement standard CRUD operations
}
```
