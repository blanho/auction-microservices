using Common.Core.Interfaces;

namespace Common.Core.Implementations;

/// <summary>
/// Thread-safe correlation ID provider using AsyncLocal.
/// </summary>
public class CorrelationIdProvider : ICorrelationIdProvider
{
    private static readonly AsyncLocal<string?> _correlationId = new();

    public string Get()
    {
        return _correlationId.Value ?? Guid.NewGuid().ToString();
    }

    public void Set(string correlationId)
    {
        _correlationId.Value = correlationId;
    }
}
