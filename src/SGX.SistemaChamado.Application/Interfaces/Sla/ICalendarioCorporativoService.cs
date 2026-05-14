using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Interfaces.Sla;

public interface ICalendarioCorporativoService
{
    Task<IReadOnlyCollection<CalendarioCorporativoResponse>> ListarAsync(CancellationToken cancellationToken = default);
    Task<CalendarioCorporativoResponse> ObterAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CalendarioCorporativoResponse> CriarAsync(CriarCalendarioCorporativoRequest request, string usuarioLogin, CancellationToken cancellationToken = default);
    Task<CalendarioCorporativoResponse> AtualizarAsync(Guid id, AtualizarCalendarioCorporativoRequest request, string usuarioLogin, CancellationToken cancellationToken = default);
    Task<CalendarioCorporativoResponse> AtualizarStatusAsync(Guid id, bool ativo, string usuarioLogin, CancellationToken cancellationToken = default);
    Task<CalendarioCorporativoResponse> DefinirPadraoAsync(Guid id, string usuarioLogin, CancellationToken cancellationToken = default);
    Task<CalendarioCorporativoResponse> AdicionarHorarioAsync(Guid calendarioId, HorarioAtendimentoCalendarioRequest request, string usuarioLogin, CancellationToken cancellationToken = default);
    Task<CalendarioCorporativoResponse> AtualizarHorarioAsync(Guid calendarioId, Guid horarioId, HorarioAtendimentoCalendarioRequest request, string usuarioLogin, CancellationToken cancellationToken = default);
    Task<CalendarioCorporativoResponse> ExcluirHorarioAsync(Guid calendarioId, Guid horarioId, string usuarioLogin, CancellationToken cancellationToken = default);
    Task<CalendarioCorporativoResponse> AdicionarExcecaoAsync(Guid calendarioId, ExcecaoCalendarioCorporativoRequest request, string usuarioLogin, CancellationToken cancellationToken = default);
    Task<CalendarioCorporativoResponse> AtualizarExcecaoAsync(Guid calendarioId, Guid excecaoId, ExcecaoCalendarioCorporativoRequest request, string usuarioLogin, CancellationToken cancellationToken = default);
    Task<CalendarioCorporativoResponse> ExcluirExcecaoAsync(Guid calendarioId, Guid excecaoId, string usuarioLogin, CancellationToken cancellationToken = default);
}
