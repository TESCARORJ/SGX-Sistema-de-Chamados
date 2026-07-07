using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Tests;

public sealed class FormularioDinamicoMigrationsTests
{
    private static readonly string[] MigrationsEstruturais =
    [
        "AdicionarFormularioServico",
        "AdicionarCamposFormularioServico",
        "AdicionarTipoCampoFormularioServico",
        "AdicionarMetadadosCampoFormularioServico",
        "AdicionarOpcoesCampoFormularioServico",
        "AdicionarVersionamentoFormularioServico",
        "AdicionarRespostasFormularioChamado"
    ];

    private static readonly string[] MigrationsChecklist =
    [
        "SincronizarChecklistSprint8FormularioServico",
        "SincronizarChecklistSprint8CamposFormularioServico",
        "SincronizarChecklistSprint8TiposCampoFormularioServico",
        "SincronizarChecklistSprint8MetadadosCampoFormularioServico",
        "SincronizarChecklistSprint8OpcoesCampoFormularioServico",
        "SincronizarChecklistSprint8VersionamentoFormularioServico",
        "SincronizarChecklistSprint8EfCoreFormularioCampos",
        "SincronizarChecklistSprint8MigrationEstruturalFormularioDinamico",
        "SincronizarChecklistSprint8ContratosFormularioServico",
        "SincronizarChecklistSprint8ValidatorsFormularioServico",
        "SincronizarChecklistSprint8UseCasesFormularioServico",
        "SincronizarChecklistSprint8EndpointsFormularioServico",
        "SincronizarChecklistSprint8FrontendFormularioServico",
        "SincronizarChecklistSprint8TestesFormularioServico",
        "SincronizarChecklistSprint8PreparacaoFormularioAbertura",
        "SincronizarChecklistSprint8ContratoRespostasFormularioAbertura",
        "SincronizarChecklistSprint8ValidatorRespostasFormularioAbertura",
        "SincronizarChecklistSprint8ObrigatoriedadeRespostasFormularioAbertura",
        "SincronizarChecklistSprint8TiposFormatosRespostasFormularioAbertura",
        "SincronizarChecklistSprint8EscopoRespostasFormularioAbertura",
        "SincronizarChecklistSprint8CompatibilidadeAberturaSemFormulario",
        "SincronizarChecklistSprint8RenderizacaoFormularioPortal",
        "SincronizarChecklistSprint8EnvioRespostasFormularioPortal",
        "SincronizarChecklistSprint8TestesAberturaGuiadaFormularioValido",
        "SincronizarChecklistSprint8TestesObrigatoriedadeAberturaGuiada",
        "SincronizarChecklistSprint8TestesRespostasInvalidasAberturaGuiada",
        "SincronizarChecklistSprint8TestesAberturaGuiadaSemFormulario",
        "SincronizarChecklistSprint8ModelagemPersistenciaRespostasChamado",
        "SincronizarChecklistSprint8MigrationEstruturalRespostasFormulario",
        "SincronizarChecklistSprint8PersistenciaRespostasFormularioAbertura",
        "SincronizarChecklistSprint8ExibicaoRespostasFormularioDetalheChamado",
        "SincronizarChecklistSprint8ExibicaoRespostasFormularioPortalSolicitante",
        "SincronizarChecklistSprint8ExibicaoRespostasFormularioAdminAtendimento",
        "SincronizarChecklistSprint8HistoricoAberturaFormularioPreenchido",
        "SincronizarChecklistSprint8AuditoriaRespostasFormularioPersistidas",
        "SincronizarChecklistSprint8TestesPersistenciaRespostasFormulario",
        "SincronizarChecklistSprint8ExibicaoRespostasPortal",
        "SincronizarChecklistSprint8ExibicaoRespostasAdmin",
        "SincronizarChecklistSprint8AplicacaoClassificacaoCatalogo",
        "SincronizarChecklistSprint8AutorizacaoFormularioAdministrativo",
        "SincronizarChecklistSprint8ProtecaoPayloadSolicitante",
        "SincronizarChecklistSprint8RespostasPermitidasPorServico",
        "SincronizarChecklistSprint8TestesSegurancaFormularioRespostas"
    ];

    private static readonly string MigrationsDir = Path.Combine(
        AppContext.BaseDirectory,
        "..", "..", "..", "..", "..",
        "src", "SGX.SistemaChamado.Infrastructure", "Persistence", "Migrations");

    [Fact]
    public void MigrationsEstruturaisDoFormularioDinamicoDevemExistir()
    {
        foreach (var nome in MigrationsEstruturais)
        {
            var caminho = Directory.GetFiles(MigrationsDir, $"*_{nome}.cs")
                .SingleOrDefault(x => !x.EndsWith(".Designer.cs", StringComparison.Ordinal));

            Assert.False(string.IsNullOrWhiteSpace(caminho), $"Migration estrutural nao encontrada: {nome}");
        }
    }

    [Fact]
    public async Task MigrationsEstruturaisDevemConterSomenteEstrutura()
    {
        foreach (var nome in MigrationsEstruturais)
        {
            var conteudo = await LerMigrationAsync(nome);

            Assert.DoesNotContain("UpdateData(", conteudo, StringComparison.Ordinal);
            Assert.DoesNotContain("InsertData(", conteudo, StringComparison.Ordinal);
            Assert.DoesNotContain("DeleteData(", conteudo, StringComparison.Ordinal);
            Assert.DoesNotContain("roadmap", conteudo, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("checklist", conteudo, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public async Task MigrationsDeChecklistDevemConterSomenteOperacoesDeDados()
    {
        foreach (var nome in MigrationsChecklist)
        {
            var conteudo = await LerMigrationAsync(nome);

            Assert.Contains("UpdateData(", conteudo, StringComparison.Ordinal);
            Assert.DoesNotContain("CreateTable(", conteudo, StringComparison.Ordinal);
            Assert.DoesNotContain("AddColumn(", conteudo, StringComparison.Ordinal);
            Assert.DoesNotContain("CreateIndex(", conteudo, StringComparison.Ordinal);
            Assert.DoesNotContain("AddForeignKey(", conteudo, StringComparison.Ordinal);
            Assert.DoesNotContain("DropTable(", conteudo, StringComparison.Ordinal);
        }
    }

    [Fact]
    public async Task PacoteEstruturalDoFormularioDinamicoDeveEstarConsolidadoNoSnapshot()
    {
        var snapshot = await File.ReadAllTextAsync(Path.Combine(MigrationsDir, "SGXSistemaChamadoDbContextModelSnapshot.cs"));

        Assert.Contains("formularios_servico", snapshot, StringComparison.Ordinal);
        Assert.Contains("formularios_servico_versoes", snapshot, StringComparison.Ordinal);
        Assert.Contains("campos_formulario_servico", snapshot, StringComparison.Ordinal);
        Assert.Contains("opcoes_campos_formulario_servico", snapshot, StringComparison.Ordinal);
        Assert.Contains("respostas_formulario_chamado", snapshot, StringComparison.Ordinal);

        Assert.Contains("ux_formularios_servico_catalogo_servico_id", snapshot, StringComparison.Ordinal);
        Assert.Contains("ux_form_serv_versao_num", snapshot, StringComparison.Ordinal);
        Assert.Contains("ux_campo_form_serv_nome", snapshot, StringComparison.Ordinal);
        Assert.Contains("ux_campo_form_serv_ordem", snapshot, StringComparison.Ordinal);
        Assert.Contains("ux_opcao_form_serv_valor", snapshot, StringComparison.Ordinal);
        Assert.Contains("ux_opcao_form_serv_ordem", snapshot, StringComparison.Ordinal);
        Assert.Contains("ix_resp_form_chamado", snapshot, StringComparison.Ordinal);
        Assert.Contains("ix_resp_form_versao", snapshot, StringComparison.Ordinal);
        Assert.Contains("ix_resp_form_campo", snapshot, StringComparison.Ordinal);
        Assert.Contains("ix_resp_form_chamado_ver", snapshot, StringComparison.Ordinal);
        Assert.Contains("ux_resp_form_chamado_cmp", snapshot, StringComparison.Ordinal);
    }

    [Fact]
    public void Item23SoDevePermanecerConcluidoSemPendingModelChangesNoSeed()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();

        var item = context.RoadmapItsmItens.Single(x => x.Id == SeedData.RoadmapItsmItem19Id);
        Assert.Equal(96, item.PercentualImplementacao);
        Assert.Equal("Registrar homologacao funcional.", item.ProximaAcao);

        var checklist23 = context.RoadmapChecklistItens.Single(x => x.Id == Guid.Parse("78787878-7878-7878-7878-000000001023"));
        Assert.True(checklist23.Concluido);

        var checklist44 = context.RoadmapChecklistItens.Single(x => x.Id == Guid.Parse("78787878-7878-7878-7878-000000001044"));
        Assert.True(checklist44.Concluido);

        var checklist45 = context.RoadmapChecklistItens.Single(x => x.Id == Guid.Parse("78787878-7878-7878-7878-000000001045"));
        Assert.True(checklist45.Concluido);

        var checklist46 = context.RoadmapChecklistItens.Single(x => x.Id == Guid.Parse("78787878-7878-7878-7878-000000001046"));
        Assert.True(checklist46.Concluido);

        var checklist47 = context.RoadmapChecklistItens.Single(x => x.Id == Guid.Parse("78787878-7878-7878-7878-000000001047"));
        Assert.True(checklist47.Concluido);

        var checklist48 = context.RoadmapChecklistItens.Single(x => x.Id == Guid.Parse("78787878-7878-7878-7878-000000001048"));
        Assert.True(checklist48.Concluido);

        var checklist49 = context.RoadmapChecklistItens.Single(x => x.Id == Guid.Parse("78787878-7878-7878-7878-000000001049"));
        Assert.True(checklist49.Concluido);

        var checklist50 = context.RoadmapChecklistItens.Single(x => x.Id == Guid.Parse("78787878-7878-7878-7878-000000001050"));
        Assert.True(checklist50.Concluido);

        var checklist51 = context.RoadmapChecklistItens.Single(x => x.Id == Guid.Parse("78787878-7878-7878-7878-000000001051"));
        Assert.True(checklist51.Concluido);

        var checklist52 = context.RoadmapChecklistItens.Single(x => x.Id == Guid.Parse("78787878-7878-7878-7878-000000001052"));
        Assert.True(checklist52.Concluido);

        var checklist53 = context.RoadmapChecklistItens.Single(x => x.Id == Guid.Parse("78787878-7878-7878-7878-000000001053"));
        Assert.True(checklist53.Concluido);

        var checklist54 = context.RoadmapChecklistItens.Single(x => x.Id == Guid.Parse("78787878-7878-7878-7878-000000001054"));
        Assert.True(checklist54.Concluido);

        var checklist55 = context.RoadmapChecklistItens.Single(x => x.Id == Guid.Parse("78787878-7878-7878-7878-000000001055"));
        Assert.True(checklist55.Concluido);

        var checklist63 = context.RoadmapChecklistItens.Single(x => x.Id == Guid.Parse("78787878-7878-7878-7878-000000001063"));
        Assert.True(checklist63.Concluido);

        var checklist64 = context.RoadmapChecklistItens.Single(x => x.Id == Guid.Parse("78787878-7878-7878-7878-000000001064"));
        Assert.True(checklist64.Concluido);

        var checklist65 = context.RoadmapChecklistItens.Single(x => x.Id == Guid.Parse("78787878-7878-7878-7878-000000001065"));
        Assert.True(checklist65.Concluido);

        var checklist66 = context.RoadmapChecklistItens.Single(x => x.Id == Guid.Parse("78787878-7878-7878-7878-000000001066"));
        Assert.True(checklist66.Concluido);
    }

    [Fact]
    public async Task MigrationEstruturalDeRespostasDeveConterTabelaFksEIndicesEsperados()
    {
        var conteudo = await LerMigrationAsync("AdicionarRespostasFormularioChamado");

        Assert.Contains("CreateTable(", conteudo, StringComparison.Ordinal);
        Assert.Contains("respostas_formulario_chamado", conteudo, StringComparison.Ordinal);
        Assert.Contains("FK_respostas_formulario_chamado_chamados_chamado_id", conteudo, StringComparison.Ordinal);
        Assert.Contains("FK_respostas_formulario_chamado_formularios_servico_versoes", conteudo, StringComparison.Ordinal);
        Assert.Contains("FK_respostas_formulario_chamado_campos_formulario_servico", conteudo, StringComparison.Ordinal);
        Assert.Contains("ix_resp_form_chamado", conteudo, StringComparison.Ordinal);
        Assert.Contains("ix_resp_form_versao", conteudo, StringComparison.Ordinal);
        Assert.Contains("ix_resp_form_campo", conteudo, StringComparison.Ordinal);
        Assert.Contains("ix_resp_form_chamado_ver", conteudo, StringComparison.Ordinal);
        Assert.Contains("ux_resp_form_chamado_cmp", conteudo, StringComparison.Ordinal);
        Assert.DoesNotContain("UpdateData(", conteudo, StringComparison.Ordinal);
    }

    private static async Task<string> LerMigrationAsync(string nome)
    {
        var caminho = Directory.GetFiles(MigrationsDir, $"*_{nome}.cs")
            .Single(x => !x.EndsWith(".Designer.cs", StringComparison.Ordinal));

        return await File.ReadAllTextAsync(caminho);
    }
}
