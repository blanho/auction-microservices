using Common.Core.Interfaces;

namespace Common.Core.Implementations;

/// <summary>
/// Default implementation of IDateTimeProvider using system clock.
/// </summary>
public class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
    public DateTime Now => DateTime.Now;
}
