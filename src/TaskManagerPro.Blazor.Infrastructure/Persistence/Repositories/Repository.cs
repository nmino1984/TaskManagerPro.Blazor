using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TaskManagerPro.Blazor.Domain.Common;
using TaskManagerPro.Blazor.Domain.Interfaces;
using TaskManagerPro.Blazor.Infrastructure.Persistence.Context;

namespace TaskManagerPro.Blazor.Infrastructure.Persistence.Repositories;

/// <summary>
/// Generic EF Core implementation of IRepository. All queries filter out soft-deleted
/// records by default so callers never accidentally surface deleted data.
/// </summary>
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbSet.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, cancellationToken);

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbSet.Where(e => !e.IsDeleted).ToListAsync(cancellationToken);

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        => await _dbSet.Where(predicate).Where(e => !e.IsDeleted).ToListAsync(cancellationToken);

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => await _dbSet.AddAsync(entity, cancellationToken);

    public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        // Soft delete — IsDeleted excludes the record from queries; UpdatedAt is stamped by SaveChangesAsync
        entity.IsDeleted = true;
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }
}
