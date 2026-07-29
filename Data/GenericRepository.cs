using System.Linq.Expressions;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Data;

/// <inheritdoc />
public class GenericRepository<TEntity, TKey>(AppDbContext dbContext) 
    : IGenericRepository<TEntity, TKey> 
    where TEntity : class
{
    /// <inheritdoc />
    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default, params string[] includes)
    {
        return await ApplyIncludes(dbContext.Set<TEntity>().AsNoTracking(), includes)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        // Обычный запрос вместо Find: так к поиску по ключу применяются глобальные фильтры (в том числе по ферме)
        return await dbContext.Set<TEntity>()
            .FirstOrDefaultAsync(ByPrimaryKey(id), cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, int page, int pageSize, CancellationToken cancellationToken = default, params string[] includes)
    {
        return await ApplyIncludes(dbContext.Set<TEntity>().AsNoTracking(), includes)
            .Where(predicate)
            .OrderBy(e => EF.Property<TKey>(e, "Id"))
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
    
    /// <inheritdoc />
    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await dbContext.Set<TEntity>().AddAsync(entity, cancellationToken);
        return entity;
    }

    /// <inheritdoc />
    public async Task DeleteAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity == null)
        {
            return;
        }
        
        dbContext.Set<TEntity>().Remove(entity);
    }

    private static IQueryable<TEntity> ApplyIncludes(IQueryable<TEntity> query, string[] includes)
    {
        return includes.Aggregate(query, (current, include) => current.Include(include));
    }

    /// <summary>
    /// Строит условие "первичный ключ сущности равен указанному значению"
    /// </summary>
    private static Expression<Func<TEntity, bool>> ByPrimaryKey(TKey id)
    {
        var entity = Expression.Parameter(typeof(TEntity), "e");
        var key = Expression.Call(
            typeof(EF),
            nameof(EF.Property),
            [typeof(TKey)],
            entity,
            Expression.Constant("Id"));
        // Значение оборачивается в объект, чтобы EF Core подставил его параметром запроса, а не константой
        var value = Expression.Property(Expression.Constant(new KeyHolder(id)), nameof(KeyHolder.Value));

        return Expression.Lambda<Func<TEntity, bool>>(Expression.Equal(key, value), entity);
    }

    private sealed record KeyHolder(TKey Value);
}