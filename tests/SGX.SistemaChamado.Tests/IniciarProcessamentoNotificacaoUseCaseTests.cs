using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces.Notificacoes;
using SGX.SistemaChamado.Application.UseCases.Notificacoes;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class IniciarProcessamentoNotificacaoUseCaseTests
{
    [Fact]
    public async Task DeveIniciarProcessamentoEIncrementarTentativa()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var usuario = CriarUsuario("inicio");
        context.Usuarios.Add(usuario);
        var notificacao = CriarNotificacao(usuario.Id, "inicio");
        context.Notificacoes.Add(notificacao);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context);
        var resposta = await useCase.ExecutarAsync(new IniciarProcessamentoNotificacaoRequest(
            notificacao.Id,
            new DateTime(2026, 6, 21, 12, 0, 0, DateTimeKind.Utc)));

        Assert.Equal(StatusNotificacao.EmProcessamento, resposta.Status);
        Assert.Equal(1, resposta.QuantidadeTentativas);

        var persistida = await context.Notificacoes.SingleAsync(x => x.Id == notificacao.Id);
        Assert.Equal(StatusNotificacao.EmProcessamento, persistida.Status);
        Assert.Equal(1, persistida.QuantidadeTentativas);
        Assert.Equal("Conteudo de teste", persistida.Conteudo);
    }

    [Fact]
    public async Task DeveRejeitarNotificacaoInexistente()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var useCase = CriarUseCase(context);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => useCase.ExecutarAsync(
            new IniciarProcessamentoNotificacaoRequest(Guid.NewGuid(), DateTime.UtcNow)));
    }

    [Fact]
    public async Task DeveRejeitarEstadoIncompativel()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var usuario = CriarUsuario("invalida");
        context.Usuarios.Add(usuario);
        var notificacao = CriarNotificacao(usuario.Id, "invalida");
        notificacao.IniciarProcessamento(DateTime.UtcNow, "teste");
        notificacao.RegistrarEnvio(DateTime.UtcNow.AddMinutes(1), "teste");
        context.Notificacoes.Add(notificacao);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(
            new IniciarProcessamentoNotificacaoRequest(notificacao.Id, DateTime.UtcNow.AddMinutes(2))));
    }

    private static IniciarProcessamentoNotificacaoUseCase CriarUseCase(SGXSistemaChamadoDbContext context)
        => new(
            PortalUseCasesTestFactory.Repo<Notificacao>(context),
            new FakeNotificacaoProcessamentoRepository(context),
            new FakeUsuarioContextoAplicacaoService(new(
                Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                "Processador",
                "processador@sgx.local",
                "processador",
                ["Sistema"])),
            PortalUseCasesTestFactory.Uow(context));

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

    private sealed class FakeNotificacaoProcessamentoRepository(SGXSistemaChamadoDbContext context) : INotificacaoProcessamentoRepository
    {
        public async Task<bool> TentarIniciarProcessamentoAsync(
            Guid notificacaoId,
            DateTime iniciadaEm,
            string atualizadoPor,
            Guid? atualizadoPorUsuarioId,
            int limiteTentativas,
            CancellationToken cancellationToken = default)
        {
            var notificacao = await context.Notificacoes.SingleOrDefaultAsync(x => x.Id == notificacaoId, cancellationToken);
            if (notificacao is null || !notificacao.EstaProcessavel(iniciadaEm, limiteTentativas))
            {
                return false;
            }

            notificacao.IniciarProcessamento(iniciadaEm, atualizadoPor, atualizadoPorUsuarioId);
            return true;
        }

        public Task<bool> TentarRegistrarSucessoAsync(
            Guid notificacaoId,
            DateTime enviadaEm,
            string atualizadoPor,
            Guid? atualizadoPorUsuarioId,
            CancellationToken cancellationToken = default)
            => throw new NotSupportedException();
    }
}
