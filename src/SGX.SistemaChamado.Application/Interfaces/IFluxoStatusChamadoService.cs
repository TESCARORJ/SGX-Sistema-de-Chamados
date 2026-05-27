using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.Interfaces;

public interface IFluxoStatusChamadoService
{
    IReadOnlyCollection<StatusChamadoEnum> ObterStatusPermitidos(NaturezaChamadoEnum natureza);
    bool StatusEhPermitido(NaturezaChamadoEnum natureza, StatusChamadoEnum status);
    void ValidarStatusPermitido(NaturezaChamadoEnum natureza, StatusChamadoEnum status);
}
