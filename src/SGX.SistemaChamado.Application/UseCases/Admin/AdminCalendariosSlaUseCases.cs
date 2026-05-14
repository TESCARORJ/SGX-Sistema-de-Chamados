using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Sla;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public abstract class CalendarioCorporativoUseCaseBase(
    ICalendarioCorporativoService calendarioService,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService)
{
    protected ICalendarioCorporativoService CalendarioService { get; } = calendarioService;
    protected IUsuarioContextoAplicacaoService UsuarioContextoAplicacaoService { get; } = usuarioContextoAplicacaoService;

    protected async Task<string> GarantirAdministradorAsync(CancellationToken cancellationToken)
    {
        var usuarioAtual = await UsuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);
        return usuarioAtual.Login;
    }

    protected async Task GarantirAdminOuAtendenteAsync(CancellationToken cancellationToken)
    {
        var usuarioAtual = await UsuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);
    }
}

public sealed class ListarCalendariosCorporativosUseCase(
    ICalendarioCorporativoService calendarioService,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService)
    : CalendarioCorporativoUseCaseBase(calendarioService, usuarioContextoAplicacaoService), IListarCalendariosCorporativosUseCase
{
    public async Task<IReadOnlyCollection<CalendarioCorporativoResponse>> ExecutarAsync(CancellationToken cancellationToken = default)
    {
        await GarantirAdminOuAtendenteAsync(cancellationToken);
        return await CalendarioService.ListarAsync(cancellationToken);
    }
}

public sealed class ObterCalendarioCorporativoUseCase(
    ICalendarioCorporativoService calendarioService,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService)
    : CalendarioCorporativoUseCaseBase(calendarioService, usuarioContextoAplicacaoService), IObterCalendarioCorporativoUseCase
{
    public async Task<CalendarioCorporativoResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await GarantirAdminOuAtendenteAsync(cancellationToken);
        return await CalendarioService.ObterAsync(id, cancellationToken);
    }
}

public sealed class CriarCalendarioCorporativoUseCase(
    ICalendarioCorporativoService calendarioService,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService)
    : CalendarioCorporativoUseCaseBase(calendarioService, usuarioContextoAplicacaoService), ICriarCalendarioCorporativoUseCase
{
    public async Task<CalendarioCorporativoResponse> ExecutarAsync(CriarCalendarioCorporativoRequest request, CancellationToken cancellationToken = default)
        => await CalendarioService.CriarAsync(request, await GarantirAdministradorAsync(cancellationToken), cancellationToken);
}

public sealed class AtualizarCalendarioCorporativoUseCase(
    ICalendarioCorporativoService calendarioService,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService)
    : CalendarioCorporativoUseCaseBase(calendarioService, usuarioContextoAplicacaoService), IAtualizarCalendarioCorporativoUseCase
{
    public async Task<CalendarioCorporativoResponse> ExecutarAsync(Guid id, AtualizarCalendarioCorporativoRequest request, CancellationToken cancellationToken = default)
        => await CalendarioService.AtualizarAsync(id, request, await GarantirAdministradorAsync(cancellationToken), cancellationToken);
}

public sealed class AtualizarStatusCalendarioCorporativoUseCase(
    ICalendarioCorporativoService calendarioService,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService)
    : CalendarioCorporativoUseCaseBase(calendarioService, usuarioContextoAplicacaoService), IAtualizarStatusCalendarioCorporativoUseCase
{
    public async Task<CalendarioCorporativoResponse> ExecutarAsync(Guid id, AtualizarStatusCalendarioCorporativoRequest request, CancellationToken cancellationToken = default)
        => await CalendarioService.AtualizarStatusAsync(id, request.Ativo, await GarantirAdministradorAsync(cancellationToken), cancellationToken);
}

public sealed class DefinirCalendarioCorporativoPadraoUseCase(
    ICalendarioCorporativoService calendarioService,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService)
    : CalendarioCorporativoUseCaseBase(calendarioService, usuarioContextoAplicacaoService), IDefinirCalendarioCorporativoPadraoUseCase
{
    public async Task<CalendarioCorporativoResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
        => await CalendarioService.DefinirPadraoAsync(id, await GarantirAdministradorAsync(cancellationToken), cancellationToken);
}

public sealed class CriarHorarioAtendimentoCalendarioUseCase(
    ICalendarioCorporativoService calendarioService,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService)
    : CalendarioCorporativoUseCaseBase(calendarioService, usuarioContextoAplicacaoService), ICriarHorarioAtendimentoCalendarioUseCase
{
    public async Task<CalendarioCorporativoResponse> ExecutarAsync(Guid calendarioId, HorarioAtendimentoCalendarioRequest request, CancellationToken cancellationToken = default)
        => await CalendarioService.AdicionarHorarioAsync(calendarioId, request, await GarantirAdministradorAsync(cancellationToken), cancellationToken);
}

public sealed class AtualizarHorarioAtendimentoCalendarioUseCase(
    ICalendarioCorporativoService calendarioService,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService)
    : CalendarioCorporativoUseCaseBase(calendarioService, usuarioContextoAplicacaoService), IAtualizarHorarioAtendimentoCalendarioUseCase
{
    public async Task<CalendarioCorporativoResponse> ExecutarAsync(Guid calendarioId, Guid horarioId, HorarioAtendimentoCalendarioRequest request, CancellationToken cancellationToken = default)
        => await CalendarioService.AtualizarHorarioAsync(calendarioId, horarioId, request, await GarantirAdministradorAsync(cancellationToken), cancellationToken);
}

public sealed class ExcluirHorarioAtendimentoCalendarioUseCase(
    ICalendarioCorporativoService calendarioService,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService)
    : CalendarioCorporativoUseCaseBase(calendarioService, usuarioContextoAplicacaoService), IExcluirHorarioAtendimentoCalendarioUseCase
{
    public async Task<CalendarioCorporativoResponse> ExecutarAsync(Guid calendarioId, Guid horarioId, CancellationToken cancellationToken = default)
        => await CalendarioService.ExcluirHorarioAsync(calendarioId, horarioId, await GarantirAdministradorAsync(cancellationToken), cancellationToken);
}

public sealed class CriarExcecaoCalendarioCorporativoUseCase(
    ICalendarioCorporativoService calendarioService,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService)
    : CalendarioCorporativoUseCaseBase(calendarioService, usuarioContextoAplicacaoService), ICriarExcecaoCalendarioCorporativoUseCase
{
    public async Task<CalendarioCorporativoResponse> ExecutarAsync(Guid calendarioId, ExcecaoCalendarioCorporativoRequest request, CancellationToken cancellationToken = default)
        => await CalendarioService.AdicionarExcecaoAsync(calendarioId, request, await GarantirAdministradorAsync(cancellationToken), cancellationToken);
}

public sealed class AtualizarExcecaoCalendarioCorporativoUseCase(
    ICalendarioCorporativoService calendarioService,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService)
    : CalendarioCorporativoUseCaseBase(calendarioService, usuarioContextoAplicacaoService), IAtualizarExcecaoCalendarioCorporativoUseCase
{
    public async Task<CalendarioCorporativoResponse> ExecutarAsync(Guid calendarioId, Guid excecaoId, ExcecaoCalendarioCorporativoRequest request, CancellationToken cancellationToken = default)
        => await CalendarioService.AtualizarExcecaoAsync(calendarioId, excecaoId, request, await GarantirAdministradorAsync(cancellationToken), cancellationToken);
}

public sealed class ExcluirExcecaoCalendarioCorporativoUseCase(
    ICalendarioCorporativoService calendarioService,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService)
    : CalendarioCorporativoUseCaseBase(calendarioService, usuarioContextoAplicacaoService), IExcluirExcecaoCalendarioCorporativoUseCase
{
    public async Task<CalendarioCorporativoResponse> ExecutarAsync(Guid calendarioId, Guid excecaoId, CancellationToken cancellationToken = default)
        => await CalendarioService.ExcluirExcecaoAsync(calendarioId, excecaoId, await GarantirAdministradorAsync(cancellationToken), cancellationToken);
}
