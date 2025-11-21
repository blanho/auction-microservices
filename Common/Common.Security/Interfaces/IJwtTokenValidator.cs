namespace Common.Security.Interfaces;

/// <summary>
/// Validates JWT tokens and extracts claims.
/// </summary>
public interface IJwtTokenValidator
{
    Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);
    IDictionary<string, string> ExtractClaims(string token);
}
