using System.Linq.Expressions;
using TaskManagerPro.Blazor.Domain.Common;

namespace TaskManagerPro.Blazor.Domain.Interfaces;

/// <summary>
/// Generic repository contract for basic persistence operations.
/// Keeping this interface in the Domain layer ensures Application handlers
/// are not coupled to any specific ORM or data access technology.
/// </summary>
public interface IRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Retrieves a single entity by its surrogate key, or null if not found.
    /// </summary>
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all non-deleted entities of this type. Use with caution on large tables;
    /// prefer <see cref="FindAsync"/> with a filter for production queries.
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns entities matching the given predicate. Allows the caller to push
    /// filter criteria to the database rather than loading all rows into memory.
    /// </summary>
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stages a new entity for insertion on the next <see cref="IUnitOfWork.SaveChangesAsync"/> call.
    /// </summary>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks an existing entity as modified so its changes are persisted on the next save.
    /// </summary>
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes the entity from the store. Implementations may perform a soft delete
    /// by setting <see cref="BaseEntity.IsDeleted"/> instead of a physical delete.
    /// </summary>
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
}
