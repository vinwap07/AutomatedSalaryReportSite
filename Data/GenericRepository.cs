using System.Linq.Expressions;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class GenericRepository<TEntity, TKey>(AppDbContext dbContext) 
    : IGenericRepository<TEntity, TKey> 
    where TEntity : class
{
    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<TEntity>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<TEntity>()
            .FindAsync([id], cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await dbContext.Set<TEntity>()
            .AsNoTracking()
            .Where(predicate)
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity == null)
        {
            return;
        }
        
        dbContext.Set<TEntity>().Remove(entity);
    }
}