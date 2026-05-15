using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Tests;

public sealed class AuditoriaModulosCriticosTests
{
    [Fact]
    public async Task CriarRoadmapItsmItemDeveGerarAuditoria()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var auditoria = new FakeAuditoriaService();

        var useCase = new CriarRoadmapItsmItemUseCase(
            PortalUseCasesTestFactory.Repo<RoadmapItsmItem>(context),
            PortalUseCasesTestFactory.Repo<RoadmapCategoria>(context),
            CriarUsuarioAdmin(),
            PortalUseCasesTestFactory.Uow(context),
            auditoria);

        await useCase.ExecutarAsync(new CriarRoadmapItsmItemRequest
        {
            Area = "Historico/Auditoria",
            Categoria = "Governanca",
            Objetivo = "Aplicar auditoria no roadmap",
            SituacaoAtual = "Base pronta",
            AtencaoTecnica = "Sprint 2",
            Status = StatusRoadmapItsm.EmValidacao,
            Prioridade = PrioridadeRoadmapItsm.Alta,
            Impacto = ImpactoRoadmapItsm.Alto,
            Decisao = DecisaoRoadmapItsm.DesenvolverAgora,
            Ordem = 1,
            StatusImplementacao = StatusImplementacaoRoadmapItsm.EmDesenvolvimento,
            StatusTecnico = StatusTecnicoRoadmapItsm.Parcial,
            PercentualImplementacao = 10,
            Ativo = true
        });

        var evento = Assert.Single(auditoria.Eventos);
        Assert.Equal("Roadmap ITSM", evento.Modulo);
        Assert.Equal("RoadmapItsmItem", evento.Entidade);
        Assert.Equal(TipoAcaoAuditoria.Criacao, evento.Acao);
    }

    [Fact]
    public async Task ConcluirChecklistRoadmapDeveGerarAuditoria()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var auditoria = new FakeAuditoriaService();

        var roadmap = new RoadmapItsmItem(
            "SLA",
            "Governanca",
            "Checklist SLA",
            null,
            "Em andamento",
            "Ajustes",
            StatusRoadmapItsm.Pendente,
            PrioridadeRoadmapItsm.Media,
            ImpactoRoadmapItsm.Medio,
            DecisaoRoadmapItsm.DesenvolverAgora,
            null,
            "Equipe SLA",
            null,
            1,
            StatusImplementacaoRoadmapItsm.EmDesenvolvimento,
            StatusTecnicoRoadmapItsm.Parcial,
            0,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            "teste");

        context.RoadmapItsmItens.Add(roadmap);
        await context.SaveChangesAsync();

        var checklist = new RoadmapChecklistItem(
            roadmap.Id,
            "Criar testes",
            null,
            GrupoRoadmapChecklist.Testes,
            1,
            false,
            true,
            "teste");

        context.RoadmapChecklistItens.Add(checklist);
        await context.SaveChangesAsync();

        var useCase = new ConcluirRoadmapChecklistItemUseCase(
            PortalUseCasesTestFactory.Repo<RoadmapChecklistItem>(context),
            PortalUseCasesTestFactory.Repo<RoadmapItsmItem>(context),
            CriarUsuarioAdmin(),
            PortalUseCasesTestFactory.Uow(context),
            auditoria);

        await useCase.ExecutarAsync(checklist.Id);

        var evento = Assert.Single(auditoria.Eventos);
        Assert.Equal("RoadmapChecklistItem", evento.Entidade);
        Assert.Equal(TipoAcaoAuditoria.Edicao, evento.Acao);
        Assert.Contains("concluido", evento.Descricao, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CriarPoliticaSlaDeveGerarAuditoria()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var auditoria = new FakeAuditoriaService();

        var useCase = new CriarPoliticaSlaUseCase(
            PortalUseCasesTestFactory.Repo<PoliticaSla>(context),
            PortalUseCasesTestFactory.Repo<MetaSla>(context),
            PortalUseCasesTestFactory.Repo<PrioridadeChamado>(context),
            PortalUseCasesTestFactory.Repo<CategoriaChamado>(context),
            PortalUseCasesTestFactory.Repo<Departamento>(context),
            PortalUseCasesTestFactory.Repo<CalendarioCorporativo>(context),
            CriarUsuarioAdmin(),
            PortalUseCasesTestFactory.Uow(context),
            auditoria);

        await useCase.ExecutarAsync(new CriarPoliticaSlaRequest
        {
            Nome = "SLA Teste Auditoria",
            Ordem = 50,
            Metas =
            [
                new MetaSlaUpsertRequest
                {
                    PrioridadeId = SeedData.PrioridadeAltaId,
                    TempoPrimeiraRespostaMinutos = 30,
                    TempoResolucaoMinutos = 120,
                    Ativo = true
                }
            ]
        });

        var evento = Assert.Single(auditoria.Eventos);
        Assert.Equal("SLA", evento.Modulo);
        Assert.Equal("PoliticaSla", evento.Entidade);
        Assert.Equal(TipoAcaoAuditoria.Criacao, evento.Acao);
    }

    [Fact]
    public async Task ComentarioInternoAdminNaoDeveExporTextoNaAuditoria()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var auditoria = new FakeAuditoriaService();

        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "admin@empresa.com", TipoPerfil.Administrador);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solic", "sol@empresa.com", TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Infra");
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.EmAtendimento, null, "AUD1");

        var useCase = new ComentarChamadoAdminUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<ComentarioChamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            SlaTestFactory.CriarService(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context),
            auditoria);

        await useCase.ExecutarAsync(chamado.Id, new ComentarioAdminChamadoRequest
        {
            Interno = true,
            Mensagem = "senha=123 token=abc conteudo sensivel"
        });

        var evento = Assert.Single(auditoria.Eventos);
        Assert.Equal("Chamados", evento.Modulo);
        Assert.Equal("Chamado", evento.Entidade);
        Assert.DoesNotContain("conteudo sensivel", evento.DadosDepois ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("senha=123", evento.DadosDepois ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("token=abc", evento.DadosDepois ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("TamanhoMensagem", evento.DadosDepois ?? string.Empty, StringComparison.OrdinalIgnoreCase);
    }

    private static IUsuarioContextoAplicacaoService CriarUsuarioAdmin()
        => new FakeUsuarioContextoAplicacaoService(new UsuarioContextoAplicacao(
            Guid.NewGuid(),
            "Administrador",
            "admin@sgx.local",
            "admin",
            ["Administrador"]));
}
