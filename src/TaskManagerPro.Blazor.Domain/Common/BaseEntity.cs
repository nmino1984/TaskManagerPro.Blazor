namespace TaskManagerPro.Blazor.Domain.Common;

/// <summary>
/// Base class for all domain entities. Provides identity, audit timestamps,
/// and soft-delete support so no entity is permanently removed from the database.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Unique identifier assigned on creation. Private setter prevents external reassignment.
    /// </summary>
    public Guid Id { get; private set; } = Guid.NewGuid();

    /// <summary>
    /// UTC timestamp recorded once when the entity is first persisted.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// UTC timestamp updated by the Infrastructure layer on every save operation.
    /// Internal setter restricts writes to the same assembly.
    /// </summary>
    public DateTime UpdatedAt { get; internal set; } = DateTime.UtcNow;

    /// <summary>
    /// Soft-delete flag. When true the entity is excluded from normal queries
    /// without being physically deleted from the database.
    /// </summary>
    public bool IsDeleted { get; set; } = false;
}
