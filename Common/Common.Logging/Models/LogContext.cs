namespace Common.Logging.Models;

/// <summary>
/// Context information attached to log entries.
/// </summary>
public sealed record LogContext
{
    public string? CorrelationId { get; init; }
    public string? UserId { get; init; }
    public string? TenantId { get; init; }
    public string? TraceId { get; init; }
    public Dictionary<string, object>? Properties { get; init; }
}
