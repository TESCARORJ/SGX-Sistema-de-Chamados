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
            NaturezaChamado = NaturezaChamadoEnum.Requisicao,
            RespostasFormulario = request.RespostasFormulario?
                .Select(x => new RespostaFormularioAberturaRequest
                {
                    CampoFormularioServicoId = x.CampoFormularioServicoId,
                    Valor = x.Valor,
                    Valores = x.Valores is null ? null : [.. x.Valores]
                })
                .ToList()
        };

        return await abrirChamadoUseCase.ExecutarAsync(criarChamadoRequest, cancellationToken);
    }
}
