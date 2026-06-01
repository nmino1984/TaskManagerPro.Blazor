namespace TaskManagerPro.Blazor.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; internal set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
}
