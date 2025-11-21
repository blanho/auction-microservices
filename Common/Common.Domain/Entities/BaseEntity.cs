#nullable enable

namespace Common.Domain.Entities;

/// <summary>
/// Base entity with common audit properties for all domain entities.
/// Provides tracking for creation, updates, and soft deletes.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid DeletedBy { get; set; }
}
