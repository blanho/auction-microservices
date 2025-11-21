using Common.Application.Abstractions;

namespace Common.Infrastructure.Services;

public sealed class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
    public DateTime TodayUtc => DateTime.UtcNow.Date;
}
