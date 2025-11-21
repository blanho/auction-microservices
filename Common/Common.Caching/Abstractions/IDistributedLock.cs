namespace Common.Caching.Abstractions;

/// <summary>
/// Abstraction for distributed locking to coordinate operations across instances.
/// </summary>
public interface IDistributedLock
{
    Task<bool> AcquireLockAsync(string key, TimeSpan expiration, CancellationToken cancellationToken = default);
    Task<bool> ReleaseLockAsync(string key, CancellationToken cancellationToken = default);
}
