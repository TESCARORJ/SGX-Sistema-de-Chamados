using System;
using System.Threading;
using System.Threading.Tasks;
using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Interfaces.Admin;

public interface IResolverChamadoUseCase
{
    Task<ChamadoAdminDetalheResponse> ExecutarAsync(Guid chamadoId, ResolverChamadoRequest request, CancellationToken cancellationToken = default);
}
