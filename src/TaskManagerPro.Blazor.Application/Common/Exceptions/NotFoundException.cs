namespace TaskManagerPro.Blazor.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when an entity is not found in the database.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" with key \"{key}\" was not found.")
    {
    }
}
