using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.UseCases.Notificacoes;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class RegistrarSucessoEntregaNotificacaoUseCaseTests
{
    [Fact]
    public async Task DeveMarcarNotificacaoComoEnviada()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var usuario = CriarUsuario("sucesso");
        context.Usuarios.Add(usuario);
        var notificacao = CriarNotificacao(usuario.Id, "sucesso");
        notificacao.IniciarProcessamento(DateTime.UtcNow, "teste");
        context.Notificacoes.Add(notificacao);
        await context.SaveChangesAsync();

        var useCase = new RegistrarSucessoEntregaNotificacaoUseCase(
            PortalUseCasesTestFactory.Repo<Notificacao>(context),
            CriarUsuarioContexto(),
            PortalUseCasesTestFactory.Uow(context));

        var enviadaEm = new DateTime(2026, 6, 21, 12, 10, 0, DateTimeKind.Utc);
        await useCase.ExecutarAsync(new RegistrarSucessoEntregaNotificacaoRequest(notificacao.Id, enviadaEm));

        var persistida = await context.Notificacoes.SingleAsync(x => x.Id == notificacao.Id);
        Assert.Equal(StatusNotificacao.Enviada, persistida.Status);
        Assert.Equal(enviadaEm, persistida.EnviadaEm);
        Assert.Null(persistida.AgendadaEm);
    }

    [Fact]
    public async Task DeveRejeitarEstadoInvalido()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var usuario = CriarUsuario("sucesso-invalido");
        context.Usuarios.Add(usuario);
        var notificacao = CriarNotificacao(usuario.Id, "sucesso-invalido");
        context.Notificacoes.Add(notificacao);
        await context.SaveChangesAsync();

        var useCase = new RegistrarSucessoEntregaNotificacaoUseCase(
            PortalUseCasesTestFactory.Repo<Notificacao>(context),
            CriarUsuarioContexto(),
            PortalUseCasesTestFactory.Uow(context));

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(
            new RegistrarSucessoEntregaNotificacaoRequest(notificacao.Id, DateTime.UtcNow)));
    }

    private static FakeUsuarioContextoAplicacaoService CriarUsuarioContexto()
        => new(new(
            Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            "Processador",
            "processador@sgx.local",
            "processador",
            ["Sistema"]));

    private static Usuario CriarUsuario(string sufixo)
        => new($"Usuario {sufixo}", $"usuario.{sufixo}@teste.local", $"login.{sufixo}", "teste");

    private static Notificacao CriarNotificacao(Guid usuarioId, string chave)
        => new(
            TipoEventoNotificacao.EventoChamado,
            CanalNotificacao.Email,
            "Conteudo de teste",
            $"notif:{chave}",
            "teste",
            usuarioId,
            null,
            null,
            "Assunto",
            $"corr:{chave}",
            usuarioId);
}
