using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.UseCases.Notificacoes;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class DefinirPreferenciaNotificacaoUsuarioUseCaseTests
{
    [Fact]
    public async Task DeveCriarPreferenciaQuandoNaoExistir()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var usuario = CriarUsuario();
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context);
        var response = await useCase.ExecutarAsync(new DefinirPreferenciaNotificacaoUsuarioRequest(
            usuario.Id,
            TipoEventoNotificacao.EventoChamado,
            CanalNotificacao.Email,
            false));

        Assert.True(response.Criada);
        Assert.False(response.Atualizada);
        Assert.False(response.Habilitada);
        Assert.Single(context.Set<PreferenciaNotificacaoUsuario>());
        Assert.Equal(0, await context.Notificacoes.CountAsync());
    }

    [Fact]
    public async Task DeveAtualizarPreferenciaExistenteSemDuplicar()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var usuario = CriarUsuario();
        context.Usuarios.Add(usuario);
        var preferencia = new PreferenciaNotificacaoUsuario(
            usuario.Id,
            TipoEventoNotificacao.EventoChamado,
            CanalNotificacao.Email,
            false,
            Guid.NewGuid(),
            "teste");
        context.Add(preferencia);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context);
        var response = await useCase.ExecutarAsync(new DefinirPreferenciaNotificacaoUsuarioRequest(
            usuario.Id,
            TipoEventoNotificacao.EventoChamado,
            CanalNotificacao.Email,
            true));

        Assert.False(response.Criada);
        Assert.True(response.Atualizada);
        Assert.True(response.Habilitada);
        Assert.Single(context.Set<PreferenciaNotificacaoUsuario>());
        Assert.True((await context.Set<PreferenciaNotificacaoUsuario>().SingleAsync()).Habilitada);
    }

    [Fact]
    public async Task DeveRejeitarUsuarioInexistente()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var useCase = CriarUseCase(context);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            useCase.ExecutarAsync(new DefinirPreferenciaNotificacaoUsuarioRequest(
                Guid.NewGuid(),
                TipoEventoNotificacao.EventoChamado,
                CanalNotificacao.Sistema,
                true)));
    }

    [Fact]
    public async Task DeveRespeitarCancellationToken()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var useCase = CriarUseCase(context);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            useCase.ExecutarAsync(new DefinirPreferenciaNotificacaoUsuarioRequest(
                Guid.NewGuid(),
                TipoEventoNotificacao.EventoChamado,
                CanalNotificacao.Sistema,
                true), cts.Token));
    }

    private static DefinirPreferenciaNotificacaoUsuarioUseCase CriarUseCase(
        SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context)
        => new(
            PortalUseCasesTestFactory.Repo<Usuario>(context),
            PortalUseCasesTestFactory.Repo<PreferenciaNotificacaoUsuario>(context),
            PortalUseCasesTestFactory.Uow(context),
            new FakeUsuarioContextoAplicacaoService(new SGX.SistemaChamado.Application.Interfaces.UsuarioContextoAplicacao(
                Guid.NewGuid(),
                "Admin",
                "admin@sgx.local",
                "admin",
                ["Administrador"])));

    private static Usuario CriarUsuario()
        => new($"Usuario {Guid.NewGuid():N}"[..20], $"usuario.{Guid.NewGuid():N}@teste.local", $"usr{Guid.NewGuid():N}"[..20], "teste");
}
