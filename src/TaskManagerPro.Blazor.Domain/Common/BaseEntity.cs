namespace TaskManagerPro.Blazor.Domain.Common;

/// <summary>
/// Base for all domain entities. Provides identity, audit timestamps,
/// and soft-delete so records are never physically removed from the database.
/// </summary>
public abstract class BaseEntity
{
    private DateTime _updatedAt = DateTime.UtcNow;

    public Guid Id { get; private set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // EF Core writes this via the _updatedAt backing field on every SaveChanges
    public DateTime UpdatedAt => _updatedAt;

    public bool IsDeleted { get; set; } = false;
}
