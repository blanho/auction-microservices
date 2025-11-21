# Common Shared Libraries

Production-ready shared libraries for the auction-microservices solution. These packages provide cross-cutting concerns without domain logic or persistence.

## 📦 Packages

### Common.Core

Foundation types and pure helpers with zero external dependencies.

**Key Types**:

- `Result<T>`, `Error` — Railway-oriented error handling
- `IDateTimeProvider`, `ICorrelationIdProvider` — Testable abstractions
- `DomainException`, `ValidationException` — Base exception types
- `DateTimeExtensions`, `StringExtensions` — Utility extensions

### Common.Logging

Structured logging abstractions and request/response middleware.

**Key Types**:

- `ILoggerAdapter<T>` — Decoupled logging interface
- `RequestLoggingMiddleware` — HTTP request logging with correlation IDs
- `LogContext` — Structured log metadata

### Common.Caching

Distributed caching with Redis support and locking primitives.

**Key Types**:

- `ICacheService` — Cache operations abstraction
- `IDistributedLock` — Distributed locking for coordination
- `RedisCacheService` — Redis-backed implementation
- `CacheKeys` — Centralized cache key definitions

### Common.Security

Authentication and authorization abstractions.

**Key Types**:

- `IJwtTokenValidator` — JWT validation interface
- `IUserContext` — Current user access
- `CurrentUser` — User model

### Common.OpenApi

OpenAPI/Swagger configuration and API versioning.

**Key Features**:

- Versioned API support (URL segment + header)
- Swagger UI with multi-version endpoints
- Generic exception → ProblemDetails mapping

## 🚀 Quick Start

### Add to your service

```xml
<!-- In YourService.API.csproj -->
<ItemGroup>
  <ProjectReference Include="..\..\Common\Common.Core\Common.Core.csproj" />
  <ProjectReference Include="..\..\Common\Common.Logging\Common.Logging.csproj" />
  <ProjectReference Include="..\..\Common\Common.Caching\Common.Caching.csproj" />
  <ProjectReference Include="..\..\Common\Common.OpenApi\Common.OpenApi.csproj" />
</ItemGroup>
```

### Configure in Program.cs

```csharp
using Common.OpenApi.Extensions;
using Common.Caching.Abstractions;
using Common.Caching.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Add Common services
builder.Services.AddCommonApiVersioning();
builder.Services.AddCommonOpenApi();
builder.Services.AddScoped<ICacheService, RedisCacheService>();

var app = builder.Build();

// Use Common middleware
app.UseCommonExceptionHandling();
app.UseCommonOpenApi();
app.UseCommonSwaggerUI("My Service API");

app.Run();
```

## 🎯 Design Principles

1. **No Domain Logic** — Common libraries are domain-agnostic
2. **No Persistence** — DbContext and EF implementations belong in services
3. **Abstractions First** — Interfaces over concrete implementations
4. **Single Responsibility** — Each package has one focus
5. **Testability** — All dependencies are mockable

## ❌ Anti-Patterns

**Don't** put these in Common:

- Domain entities (e.g., `Auction`, `User`)
- Service-specific business rules
- DbContext or EF migrations
- Service-specific exception types
- Direct API controllers

## ✅ Do This Instead

- Define abstractions in Common.Core
- Implement in service Infrastructure/Application layers
- Use `Result<T>` for expected errors
- Inject `IDateTimeProvider` for testable time
- Cache DTOs, not domain entities

## 📚 Documentation

See [.github/copilot-instructions.md](../.github/copilot-instructions.md) for comprehensive architectural guidance.

## 🧪 Testing

Each package should have corresponding tests:

- `tests/Common.Core.Tests`
- `tests/Common.Caching.Tests`
- etc.

## 🔄 Migrating from Old Structure

| Old                     | New                                        |
| ----------------------- | ------------------------------------------ |
| `Common.Domain`         | ❌ Removed (no domain in Common)           |
| `Common.Application`    | `Common.Core` + service Application layers |
| `Common.Infrastructure` | `Common.Logging` + `Common.Caching`        |
| `Common.API`            | `Common.OpenApi`                           |

Update namespaces:

- `Common.Application.Abstractions` → `Common.Core.Interfaces`
- `Common.API.Extensions` → `Common.OpenApi.Extensions`

## 📝 Contributing

When adding to Common:

1. Ensure it's used by 2+ services
2. Keep it pure (no domain/persistence)
3. Add XML comments
4. Update this README
5. Add tests

## 📦 Version

All Common packages follow semantic versioning. Breaking changes require a major version bump.

---

**Maintained By**: Architecture Team  
**Last Updated**: November 2025
