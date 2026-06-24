using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class ProcessarEventoCandidatoNotificacaoUseCaseTests
{
    [Fact]
    public async Task DeveGerarNotificacoesSistemaEEmailParaSolicitante()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante Notificacao", "sol.notificacao@sgx.local", TipoPerfil.Solicitante);
        var origem = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin Origem", "admin.origem@sgx.local", TipoPerfil.Administrador);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Infra Notificacao");
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.Aberto, criadoPor: "test.notificacao");

        await NotificacoesItsmTestFactory.CriarTemplatesPadraoChamadoAsync(context, origem.Id);

        var useCase = NotificacoesItsmTestFactory.CriarOrquestrador(context, AdminUseCasesTestFactory.Contexto(origem, "Administrador"));

        var response = await useCase.ExecutarAsync(CriarRequest(chamado, origem.Id));

        Assert.Equal("chamado-aberto:processo", response.EventoId);
        Assert.Equal(1, response.DestinatariosResolvidos);
        Assert.Equal(2, response.DestinatariosPermitidos);
        Assert.Equal(2, response.NotificacoesCriadas);
        Assert.Equal(0, response.NotificacoesDuplicadas);
        Assert.Equal(0, response.Ignoradas);

        var notificacoes = await context.Notificacoes
            .OrderBy(x => x.Canal)
            .ToListAsync();

        Assert.Equal(2, notificacoes.Count);
        Assert.Contains(notificacoes, x => x.Canal == CanalNotificacao.Sistema && x.DestinatarioUsuarioId == solicitante.Id);
        Assert.Contains(notificacoes, x => x.Canal == CanalNotificacao.Email && x.DestinatarioEndereco == solicitante.Email);
    }

    [Fact]
    public async Task DeveRespeitarPreferenciaDesabilitada()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante Preferencia", "sol.pref@sgx.local", TipoPerfil.Solicitante);
        var origem = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin Preferencia", "admin.pref@sgx.local", TipoPerfil.Administrador);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Infra Preferencia");
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.Aberto, criadoPor: "test.notificacao");

        await NotificacoesItsmTestFactory.CriarTemplatesPadraoChamadoAsync(context, origem.Id);
        context.PreferenciasNotificacaoUsuario.Add(new PreferenciaNotificacaoUsuario(
            solicitante.Id,
            TipoEventoNotificacao.EventoChamado,
            CanalNotificacao.Email,
            false,
            origem.Id,
            "test.pref"));
        await context.SaveChangesAsync();

        var useCase = NotificacoesItsmTestFactory.CriarOrquestrador(context, AdminUseCasesTestFactory.Contexto(origem, "Administrador"));
        var response = await useCase.ExecutarAsync(CriarRequest(chamado, origem.Id));

        Assert.Equal(1, response.DestinatariosResolvidos);
        Assert.Equal(1, response.NotificacoesCriadas);
        Assert.Equal(1, response.Ignoradas);
        Assert.Single(await context.Notificacoes.ToListAsync());
        Assert.Contains(response.Avisos, x => x.Contains("bloqueado", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task DeveSerIdempotenteQuandoEventoForProcessadoMaisDeUmaVez()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante Idempotente", "sol.idem@sgx.local", TipoPerfil.Solicitante);
        var origem = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin Idempotente", "admin.idem@sgx.local", TipoPerfil.Administrador);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Infra Idempotente");
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.Aberto, criadoPor: "test.notificacao");

        await NotificacoesItsmTestFactory.CriarTemplatesPadraoChamadoAsync(context, origem.Id);

        var useCase = NotificacoesItsmTestFactory.CriarOrquestrador(context, AdminUseCasesTestFactory.Contexto(origem, "Administrador"));
        await useCase.ExecutarAsync(CriarRequest(chamado, origem.Id));
        var response = await useCase.ExecutarAsync(CriarRequest(chamado, origem.Id));

        Assert.Equal(0, response.NotificacoesCriadas);
        Assert.Equal(2, response.NotificacoesDuplicadas);
        Assert.Equal(2, await context.Notificacoes.CountAsync());
    }

    [Fact]
    public async Task DeveRetornarResultadoParcialQuandoTemplateAusente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante Sem Template", "sol.sem.template@sgx.local", TipoPerfil.Solicitante);
        var origem = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin Sem Template", "admin.sem.template@sgx.local", TipoPerfil.Administrador);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Infra Sem Template");
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.Aberto, criadoPor: "test.notificacao");

        var useCase = NotificacoesItsmTestFactory.CriarOrquestrador(context, AdminUseCasesTestFactory.Contexto(origem, "Administrador"));
        var response = await useCase.ExecutarAsync(CriarRequest(chamado, origem.Id));

        Assert.Equal(0, response.NotificacoesCriadas);
        Assert.Equal(2, response.Ignoradas);
        Assert.Empty(await context.Notificacoes.ToListAsync());
    }

    [Fact]
    public async Task DeveRespeitarCancellationToken()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante Cancelado", "sol.cancelado@sgx.local", TipoPerfil.Solicitante);
        var origem = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin Cancelado", "admin.cancelado@sgx.local", TipoPerfil.Administrador);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Infra Cancelado");
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.Aberto, criadoPor: "test.notificacao");

        await NotificacoesItsmTestFactory.CriarTemplatesPadraoChamadoAsync(context, origem.Id);
        var useCase = NotificacoesItsmTestFactory.CriarOrquestrador(context, AdminUseCasesTestFactory.Contexto(origem, "Administrador"));

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => useCase.ExecutarAsync(CriarRequest(chamado, origem.Id), cts.Token));
    }

    private static ProcessarEventoCandidatoNotificacaoRequest CriarRequest(Chamado chamado, Guid usuarioOriginadorId)
    {
        return new ProcessarEventoCandidatoNotificacaoRequest(
            "chamado-aberto:processo",
            new EventoCandidatoNotificacao(
                TipoEventoNotificacao.EventoChamado,
                chamado.Id,
                usuarioOriginadorId,
                new DateTime(2026, 6, 23, 12, 0, 0, DateTimeKind.Utc),
                $"chamado:{chamado.Id}",
                "chamado-aberto:processo",
                new Dictionary<string, string>
                {
                    ["evento"] = "chamado-aberto"
                }),
            new Dictionary<string, string>
            {
                ["chamado.codigo"] = chamado.Codigo,
                ["chamado.status"] = "Aberto",
                ["chamado.titulo"] = chamado.Titulo,
                ["evento.descricao"] = "Chamado criado pelo portal",
                ["evento.nome"] = "Chamado aberto"
            },
            [TipoParticipacaoDestinatarioNotificacao.Solicitante],
            [CanalNotificacao.Sistema, CanalNotificacao.Email]);
    }
}
