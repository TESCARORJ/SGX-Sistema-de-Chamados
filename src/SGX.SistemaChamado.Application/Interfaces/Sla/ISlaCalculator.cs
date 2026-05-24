using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Application.Interfaces.Sla;

public sealed record SlaPrazosAplicados(
    Guid? PoliticaSlaId,
    string? PoliticaSlaNome,
    Guid PrioridadeId,
    int PrazoPrimeiraRespostaMinutos,
    int PrazoResolucaoMinutos,
    bool UsarHorarioComercial,
    Guid? CalendarioCorporativoId,
    string? CalendarioCorporativoNome,
    CalendarioCorporativo? CalendarioCorporativo,
    bool PausarQuandoAguardandoSolicitante,
    string Fonte);

public interface ISlaCalculator
{
    Task<SlaPrazosAplicados?> CalcularPrazosAsync(
        Guid prioridadeId,
        Guid? categoriaId,
        Guid? departamentoId,
        CancellationToken cancellationToken = default);

    Task<SlaPrazosAplicados?> CalcularPrazosAsync(
        Guid prioridadeId,
        Guid? categoriaId,
        Guid? departamentoId,
        Guid? politicaSlaIdPreferencial,
        CancellationToken cancellationToken = default);
}
