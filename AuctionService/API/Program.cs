using AuctionService.API.Extensions;
using AuctionService.Infrastructure.Data;
using Common.Infrastructure.Extensions;
using Common.API.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCommonLogging();
builder.Services.AddCorrelationId();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});
builder.Services.AddDistributedCacheService();
builder.Services.AddCommonUtilities();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddCommonApiVersioning();
builder.Services.AddCommonOpenApi();

var app = builder.Build();

// Database migrations and seeding are handled by upgrade hosted service

// Optional path base support (e.g., /auction)
var pathBase = builder.Configuration["PathBase"] ?? builder.Configuration["ASPNETCORE_PATHBASE"]; 
if (!string.IsNullOrWhiteSpace(pathBase))
{
    app.UsePathBase(pathBase);
}

app.UseCorrelationId();
app.UseCommonExceptionHandling();

// Health check endpoint (used by Docker healthcheck)
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
   .AllowAnonymous()
   .WithTags("Health");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCommonOpenApi();
app.UseCommonSwaggerUI("Auction Service");

app.Run();
