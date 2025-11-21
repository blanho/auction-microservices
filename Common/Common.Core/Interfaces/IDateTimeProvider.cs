namespace Common.Core.Interfaces;

/// <summary>
/// Abstraction for system time. Use for testability.
/// </summary>
public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
    DateTime Now { get; }
}
