using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.UseCases.Notificacoes;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class AvaliarPreferenciaNotificacaoUseCaseTests
{
    [Fact]
    public async Task DevePermitirQuandoPreferenciaHabilitada()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var usuario = CriarUsuario();
        context.Add(usuario);
        context.Add(new PreferenciaNotificacaoUsuario(usuario.Id, TipoEventoNotificacao.EventoChamado, CanalNotificacao.Email, true, Guid.NewGuid(), "teste"));
        await context.SaveChangesAsync();

        var response = await CriarUseCase(context).ExecutarAsync(new AvaliarPreferenciaNotificacaoRequest(usuario.Id, TipoEventoNotificacao.EventoChamado, CanalNotificacao.Email));

        Assert.True(response.Permitida);
        Assert.True(response.PreferenciaExplicita);
        Assert.True(response.HabilitadaConfigurada);
        Assert.Equal(MotivoDecisaoPreferenciaNotificacao.PreferenciaHabilitada, response.Motivo);
    }

    [Fact]
    public async Task DeveBloquearQuandoPreferenciaDesabilitada()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var usuario = CriarUsuario();
        context.Add(usuario);
        context.Add(new PreferenciaNotificacaoUsuario(usuario.Id, TipoEventoNotificacao.EventoChamado, CanalNotificacao.Email, false, Guid.NewGuid(), "teste"));
        await context.SaveChangesAsync();

        var response = await CriarUseCase(context).ExecutarAsync(new AvaliarPreferenciaNotificacaoRequest(usuario.Id, TipoEventoNotificacao.EventoChamado, CanalNotificacao.Email));

        Assert.False(response.Permitida);
        Assert.True(response.PreferenciaExplicita);
        Assert.False(response.HabilitadaConfigurada);
        Assert.Equal(MotivoDecisaoPreferenciaNotificacao.PreferenciaDesabilitada, response.Motivo);
    }

    [Fact]
    public async Task DevePermitirPorFallbackQuandoNaoHouverRegistro()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var usuario = CriarUsuario();
        context.Add(usuario);
        await context.SaveChangesAsync();

        var response = await CriarUseCase(context).ExecutarAsync(new AvaliarPreferenciaNotificacaoRequest(usuario.Id, TipoEventoNotificacao.EventoSla, CanalNotificacao.Sistema));

        Assert.True(response.Permitida);
        Assert.False(response.PreferenciaExplicita);
        Assert.Null(response.HabilitadaConfigurada);
        Assert.Equal(MotivoDecisaoPreferenciaNotificacao.FallbackPermitido, response.Motivo);
    }

    [Fact]
    public async Task DeveBloquearUsuarioInexistente()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();

        var response = await CriarUseCase(context).ExecutarAsync(new AvaliarPreferenciaNotificacaoRequest(Guid.NewGuid(), TipoEventoNotificacao.EventoChamado, CanalNotificacao.Sistema));

        Assert.False(response.Permitida);
        Assert.Equal(MotivoDecisaoPreferenciaNotificacao.UsuarioInexistente, response.Motivo);
    }

    [Fact]
    public async Task DeveBloquearUsuarioInativo()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var usuario = CriarUsuario();
        usuario.AlterarSituacao(SituacaoUsuario.Inativo, "teste");
        context.Add(usuario);
        await context.SaveChangesAsync();

        var response = await CriarUseCase(context).ExecutarAsync(new AvaliarPreferenciaNotificacaoRequest(usuario.Id, TipoEventoNotificacao.EventoChamado, CanalNotificacao.Sistema));

        Assert.False(response.Permitida);
        Assert.Equal(MotivoDecisaoPreferenciaNotificacao.UsuarioInativo, response.Motivo);
    }

    [Fact]
    public async Task DeveBloquearUsuarioBloqueado()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var usuario = CriarUsuario();
        usuario.AlterarSituacao(SituacaoUsuario.Bloqueado, "teste");
        context.Add(usuario);
        await context.SaveChangesAsync();

        var response = await CriarUseCase(context).ExecutarAsync(new AvaliarPreferenciaNotificacaoRequest(usuario.Id, TipoEventoNotificacao.EventoChamado, CanalNotificacao.Sistema));

        Assert.False(response.Permitida);
        Assert.Equal(MotivoDecisaoPreferenciaNotificacao.UsuarioBloqueado, response.Motivo);
    }

    [Fact]
    public async Task DevePermitirCanalSistemaSemEmail()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var usuario = CriarUsuario();
        DefinirEmailViaReflection(usuario, string.Empty);
        context.Add(usuario);
        await context.SaveChangesAsync();

        var response = await CriarUseCase(context).ExecutarAsync(new AvaliarPreferenciaNotificacaoRequest(usuario.Id, TipoEventoNotificacao.EventoChamado, CanalNotificacao.Sistema));

        Assert.True(response.Permitida);
        Assert.Equal(MotivoDecisaoPreferenciaNotificacao.FallbackPermitido, response.Motivo);
    }

    [Fact]
    public async Task DeveBloquearCanalEmailSemEmail()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var usuario = CriarUsuario();
        DefinirEmailViaReflection(usuario, string.Empty);
        context.Add(usuario);
        await context.SaveChangesAsync();

        var response = await CriarUseCase(context).ExecutarAsync(new AvaliarPreferenciaNotificacaoRequest(usuario.Id, TipoEventoNotificacao.EventoChamado, CanalNotificacao.Email));

        Assert.False(response.Permitida);
        Assert.Equal(MotivoDecisaoPreferenciaNotificacao.CanalSemEndereco, response.Motivo);
    }

    [Fact]
    public async Task DeveRespeitarCancellationToken()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            CriarUseCase(context).ExecutarAsync(new AvaliarPreferenciaNotificacaoRequest(Guid.NewGuid(), TipoEventoNotificacao.EventoChamado, CanalNotificacao.Email), cts.Token));
    }

    [Fact]
    public async Task NaoDeveCriarPreferenciaNemNotificacaoDuranteAvaliacao()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var usuario = CriarUsuario();
        context.Add(usuario);
        await context.SaveChangesAsync();

        _ = await CriarUseCase(context).ExecutarAsync(new AvaliarPreferenciaNotificacaoRequest(usuario.Id, TipoEventoNotificacao.EventoChamado, CanalNotificacao.Email));

        Assert.Equal(0, await context.Set<PreferenciaNotificacaoUsuario>().CountAsync());
        Assert.Equal(0, await context.Notificacoes.CountAsync());
    }

    private static AvaliarPreferenciaNotificacaoUseCase CriarUseCase(
        SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context)
        => new(
            PortalUseCasesTestFactory.Repo<Usuario>(context),
            PortalUseCasesTestFactory.Repo<PreferenciaNotificacaoUsuario>(context));

    private static Usuario CriarUsuario()
        => new($"Usuario {Guid.NewGuid():N}"[..20], $"usuario.{Guid.NewGuid():N}@teste.local", $"usr{Guid.NewGuid():N}"[..20], "teste");

    private static void DefinirEmailViaReflection(Usuario usuario, string email)
    {
        typeof(Usuario)
            .GetProperty(nameof(Usuario.Email))!
            .SetValue(usuario, email);
    }
}
