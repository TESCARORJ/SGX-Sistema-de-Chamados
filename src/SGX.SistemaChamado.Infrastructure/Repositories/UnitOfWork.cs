using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Infrastructure.Repositories;

public sealed class UnitOfWork(SGXSistemaChamadoDbContext context) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }
}
