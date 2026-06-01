using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TaskManagerPro.Blazor.Domain.Common;
using TaskManagerPro.Blazor.Domain.Interfaces;
using TaskManagerPro.Blazor.Infrastructure.Persistence.Context;

namespace TaskManagerPro.Blazor.Infrastructure.Persistence.Repositories;

/// <summary>
/// Generic EF Core implementation of IRepository&lt;T&gt;.
/// All queries filter out soft-deleted records by default so callers
/// never accidentally surface deleted data without explicitly opting in.
/// FindAsync pushes the predicate to the database rather than loading
/// all rows into memory, which matters at scale.
/// </summary>
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _dbSet;

    /// <summary>
    /// Initialises the repository with the shared DbContext for the current request scope.
    /// </summary>
    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    /// <inheritdoc />
    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(e => !e.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(predicate)
            .Where(e => !e.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    /// <inheritdoc />
    public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        // Soft delete: mark IsDeleted so the record is excluded from queries.
        // UpdatedAt is stamped automatically by DbContext.SaveChangesAsync.
        entity.IsDeleted = true;
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }
}
