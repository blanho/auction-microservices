namespace Common.Repository.Interfaces;

/// <summary>
/// Generic logging abstraction for application services.
/// Decouples application code from specific logging implementations.
/// </summary>
public interface IAppLogger<T>
{
    void LogInformation(string message, params object[] args);
    void LogWarning(string message, params object[] args);
    void LogError(string message, params object[] args);
    void LogError(Exception exception, string message, params object[] args);
    void LogDebug(string message, params object[] args);
}
