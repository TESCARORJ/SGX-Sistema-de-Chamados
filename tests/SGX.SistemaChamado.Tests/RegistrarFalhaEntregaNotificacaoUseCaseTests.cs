using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.UseCases.Notificacoes;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class RegistrarFalhaEntregaNotificacaoUseCaseTests
{
    [Fact]
    public async Task DeveReagendarFalhaTransitoria()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var usuario = CriarUsuario("falha-transitoria");
        context.Usuarios.Add(usuario);
        var notificacao = CriarNotificacao(usuario.Id, "falha-transitoria");
        notificacao.IniciarProcessamento(new DateTime(2026, 6, 21, 12, 0, 0, DateTimeKind.Utc), "teste");
        context.Notificacoes.Add(notificacao);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context);
        var resposta = await useCase.ExecutarAsync(new RegistrarFalhaEntregaNotificacaoRequest(
            notificacao.Id,
            "timeout",
            true,
            new DateTime(2026, 6, 21, 12, 5, 0, DateTimeKind.Utc)));

        Assert.Equal(StatusNotificacao.Agendada, resposta.Status);
        Assert.Equal(new DateTime(2026, 6, 21, 12, 6, 0, DateTimeKind.Utc), resposta.AgendadaEm);
        Assert.Equal("timeout", resposta.UltimoErro);
    }

    [Fact]
    public async Task DeveEncerrarFalhaDefinitiva()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var usuario = CriarUsuario("falha-definitiva");
        context.Usuarios.Add(usuario);
        var notificacao = CriarNotificacao(usuario.Id, "falha-definitiva");
        notificacao.IniciarProcessamento(DateTime.UtcNow, "teste");
        context.Notificacoes.Add(notificacao);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context);
        var resposta = await useCase.ExecutarAsync(new RegistrarFalhaEntregaNotificacaoRequest(
            notificacao.Id,
            "endereco invalido",
            false,
            DateTime.UtcNow.AddMinutes(1)));

        Assert.Equal(StatusNotificacao.Falhou, resposta.Status);
        Assert.Null(resposta.AgendadaEm);
    }

    [Fact]
    public async Task DeveEncerrarQuandoLimiteDeTentativasForAtingido()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var usuario = CriarUsuario("falha-limite");
        context.Usuarios.Add(usuario);
        var notificacao = CriarNotificacao(usuario.Id, "falha-limite");
        for (var tentativa = 0; tentativa < 5; tentativa++)
        {
            notificacao.IniciarProcessamento(DateTime.UtcNow.AddMinutes(tentativa), "teste");
            if (tentativa < 4)
            {
                notificacao.RegistrarFalha($"falha {tentativa}", DateTime.UtcNow.AddMinutes(tentativa + 1), "teste");
                notificacao.ReagendarAposFalha(DateTime.UtcNow.AddMinutes(tentativa + 2), "teste");
            }
        }

        context.Notificacoes.Add(notificacao);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context);
        var resposta = await useCase.ExecutarAsync(new RegistrarFalhaEntregaNotificacaoRequest(
            notificacao.Id,
            "limite atingido",
            true,
            DateTime.UtcNow.AddMinutes(10)));

        Assert.Equal(StatusNotificacao.Falhou, resposta.Status);
        Assert.Null(resposta.AgendadaEm);
        Assert.Equal(5, resposta.QuantidadeTentativas);
    }

    private static RegistrarFalhaEntregaNotificacaoUseCase CriarUseCase(Infrastructure.Persistence.SGXSistemaChamadoDbContext context)
        => new(
            PortalUseCasesTestFactory.Repo<Notificacao>(context),
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
}
