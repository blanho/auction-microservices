namespace Common.Core.Exceptions;

/// <summary>
/// Base exception for domain invariant violations.
/// Use for business rule failures within entities/value objects.
/// </summary>
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
    
    protected DomainException(string message, Exception inner) : base(message, inner) { }
}
