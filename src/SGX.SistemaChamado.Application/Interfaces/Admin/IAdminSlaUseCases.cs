using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Interfaces.Admin;

public interface IListarPoliticasSlaUseCase
{
    Task<IReadOnlyCollection<PoliticaSlaResponse>> ExecutarAsync(
        FiltroPoliticaSlaRequest request,
        CancellationToken cancellationToken = default);
}

public interface IObterPoliticaSlaUseCase
{
    Task<PoliticaSlaResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface ICriarPoliticaSlaUseCase
{
    Task<PoliticaSlaResponse> ExecutarAsync(CriarPoliticaSlaRequest request, CancellationToken cancellationToken = default);
}

public interface IAtualizarPoliticaSlaUseCase
{
    Task<PoliticaSlaResponse> ExecutarAsync(Guid id, AtualizarPoliticaSlaRequest request, CancellationToken cancellationToken = default);
}

public interface IAtualizarStatusPoliticaSlaUseCase
{
    Task<PoliticaSlaResponse> ExecutarAsync(
        Guid id,
        AtualizarStatusPoliticaSlaRequest request,
        CancellationToken cancellationToken = default);
}

public interface IInativarPoliticaSlaUseCase
{
    Task<PoliticaSlaResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IObterConfiguracaoAlertaSlaUseCase
{
    Task<ConfiguracaoAlertaSlaResponse> ExecutarAsync(CancellationToken cancellationToken = default);
}

public interface IAtualizarConfiguracaoAlertaSlaUseCase
{
    Task<ConfiguracaoAlertaSlaResponse> ExecutarAsync(
        AtualizarConfiguracaoAlertaSlaRequest request,
        CancellationToken cancellationToken = default);
}

public interface IObterDashboardSlaUseCase
{
    Task<SlaDashboardResponse> ExecutarAsync(FiltroDashboardSlaRequest request, CancellationToken cancellationToken = default);
}

public interface IListarRelatorioSlaUseCase
{
    Task<IReadOnlyCollection<SlaRelatorioItemResponse>> ExecutarAsync(
        FiltroDashboardSlaRequest request,
        CancellationToken cancellationToken = default);
}

public interface IListarCalendariosCorporativosUseCase
{
    Task<IReadOnlyCollection<CalendarioCorporativoResponse>> ExecutarAsync(CancellationToken cancellationToken = default);
}

public interface IObterCalendarioCorporativoUseCase
{
    Task<CalendarioCorporativoResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface ICriarCalendarioCorporativoUseCase
{
    Task<CalendarioCorporativoResponse> ExecutarAsync(CriarCalendarioCorporativoRequest request, CancellationToken cancellationToken = default);
}

public interface IAtualizarCalendarioCorporativoUseCase
{
    Task<CalendarioCorporativoResponse> ExecutarAsync(Guid id, AtualizarCalendarioCorporativoRequest request, CancellationToken cancellationToken = default);
}

public interface IAtualizarStatusCalendarioCorporativoUseCase
{
    Task<CalendarioCorporativoResponse> ExecutarAsync(Guid id, AtualizarStatusCalendarioCorporativoRequest request, CancellationToken cancellationToken = default);
}

public interface IDefinirCalendarioCorporativoPadraoUseCase
{
    Task<CalendarioCorporativoResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface ICriarHorarioAtendimentoCalendarioUseCase
{
    Task<CalendarioCorporativoResponse> ExecutarAsync(Guid calendarioId, HorarioAtendimentoCalendarioRequest request, CancellationToken cancellationToken = default);
}

public interface IAtualizarHorarioAtendimentoCalendarioUseCase
{
    Task<CalendarioCorporativoResponse> ExecutarAsync(Guid calendarioId, Guid horarioId, HorarioAtendimentoCalendarioRequest request, CancellationToken cancellationToken = default);
}

public interface IExcluirHorarioAtendimentoCalendarioUseCase
{
    Task<CalendarioCorporativoResponse> ExecutarAsync(Guid calendarioId, Guid horarioId, CancellationToken cancellationToken = default);
}

public interface ICriarExcecaoCalendarioCorporativoUseCase
{
    Task<CalendarioCorporativoResponse> ExecutarAsync(Guid calendarioId, ExcecaoCalendarioCorporativoRequest request, CancellationToken cancellationToken = default);
}

public interface IAtualizarExcecaoCalendarioCorporativoUseCase
{
    Task<CalendarioCorporativoResponse> ExecutarAsync(Guid calendarioId, Guid excecaoId, ExcecaoCalendarioCorporativoRequest request, CancellationToken cancellationToken = default);
}

public interface IExcluirExcecaoCalendarioCorporativoUseCase
{
    Task<CalendarioCorporativoResponse> ExecutarAsync(Guid calendarioId, Guid excecaoId, CancellationToken cancellationToken = default);
}
