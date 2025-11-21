namespace Common.Application.Abstractions;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
    DateTime TodayUtc { get; }
}
