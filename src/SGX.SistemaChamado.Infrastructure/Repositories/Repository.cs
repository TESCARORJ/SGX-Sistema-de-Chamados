using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Infrastructure.Repositories;

public class Repository<T>(SGXSistemaChamadoDbContext context) : IRepository<T> where T : class
{
    protected readonly SGXSistemaChamadoDbContext Context = context;
    protected readonly DbSet<T> DbSet = context.Set<T>();

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync([id], cancellationToken);
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
    }

    public void Update(T entity)
    {
        DbSet.Update(entity);
    }

    public void Remove(T entity)
    {
        DbSet.Remove(entity);
    }

    public async Task<IReadOnlyList<T>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking().ToListAsync(cancellationToken);
    }

    public IQueryable<T> Query()
    {
        return DbSet.AsQueryable();
    }
}
