namespace Common.Security.Interfaces;

/// <summary>
/// Provides access to the current authenticated user's information.
/// </summary>
public interface IUserContext
{
    string? UserId { get; }
    string? Email { get; }
    string[] Roles { get; }
    bool IsAuthenticated { get; }
}
