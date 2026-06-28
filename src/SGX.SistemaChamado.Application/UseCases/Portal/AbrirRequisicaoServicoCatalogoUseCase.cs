using SGX.SistemaChamado.Application.DTOs.Portal;
using SGX.SistemaChamado.Application.Interfaces.Portal;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Portal;

public sealed class AbrirRequisicaoServicoCatalogoUseCase(
    IAbrirChamadoUseCase abrirChamadoUseCase) : IAbrirRequisicaoServicoCatalogoUseCase
{
    public async Task<ChamadoDetalheResponse> ExecutarAsync(
        AbrirRequisicaoServicoCatalogoRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var criarChamadoRequest = new CriarChamadoRequest
        {
            CatalogoServicoId = request.CatalogoServicoId,
            Titulo = request.Titulo,
            Descricao = request.Descricao ?? string.Empty,
            NaturezaChamado = NaturezaChamadoEnum.Requisicao
        };

        return await abrirChamadoUseCase.ExecutarAsync(criarChamadoRequest, cancellationToken);
    }
}
