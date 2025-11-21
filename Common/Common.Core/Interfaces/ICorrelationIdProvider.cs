namespace Common.Core.Interfaces;

/// <summary>
/// Provides correlation ID for distributed tracing.
/// </summary>
public interface ICorrelationIdProvider
{
    string Get();
    void Set(string correlationId);
}
