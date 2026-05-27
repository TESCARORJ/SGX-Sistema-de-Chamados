using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class RelatoriosAvancadosSprint3AdminUseCasesTests
{
    [Fact]
    public async Task ObterResumoSlaAsync_DeveRetornarTotaisDentroEFora()
    {
        await using var cenario = await CriarCenarioAsync();

        var resumo = await cenario.UseCase.ObterResumoSlaAsync(FiltroSla(cenario));

        Assert.Equal(3, resumo.TotalChamadosComSla);
        Assert.Equal(2, resumo.TotalDentroSla);
        Assert.Equal(1, resumo.TotalForaSla);
        Assert.Equal(1, resumo.TotalSemSla);
    }

    [Fact]
    public async Task ObterSlaPorDepartamentoAsync_DeveAgruparPorDepartamento()
    {
        await using var cenario = await CriarCenarioAsync();

        var itens = await cenario.UseCase.ObterSlaPorDepartamentoAsync(FiltroSla(cenario));

        var infra = Assert.Single(itens, x => x.DepartamentoId == cenario.DepartamentoInfraId);
        Assert.Equal(2, infra.TotalComSla);
        Assert.Equal(1, infra.DentroSla);
        Assert.Equal(1, infra.ForaSla);
    }

    [Fact]
    public async Task ObterSlaPorPrioridadeAsync_DeveAgruparPorPrioridade()
    {
        await using var cenario = await CriarCenarioAsync();

        var itens = await cenario.UseCase.ObterSlaPorPrioridadeAsync(FiltroSla(cenario));

        var alta = Assert.Single(itens, x => x.PrioridadeId == cenario.PrioridadeAltaId);
        Assert.Equal(2, alta.TotalComSla);
        Assert.Equal(1, alta.DentroSla);
        Assert.Equal(1, alta.ForaSla);
    }

    [Fact]
    public async Task ObterViolacoesSlaAsync_DeveRespeitarPeriodo()
    {
        await using var cenario = await CriarCenarioAsync();

        var itens = await cenario.UseCase.ObterViolacoesSlaAsync(new FiltroRelatorioSlaRequest
        {
            DataInicial = cenario.BaseData.AddDays(-8),
            DataFinal = cenario.BaseData,
            LimiteRanking = 10
        });

        var violacao = Assert.Single(itens);
        Assert.Equal("CH-RAV3-002", violacao.NumeroProtocolo);
    }

    [Fact]
    public async Task ObterResumoSlaAsync_DeveFiltrarPorNaturezaChamado()
    {
        await using var cenario = await CriarCenarioAsync();

        var resumo = await cenario.UseCase.ObterResumoSlaAsync(new FiltroRelatorioSlaRequest
        {
            DataInicial = cenario.BaseData.AddDays(-20),
            DataFinal = cenario.BaseData,
            NaturezaChamado = NaturezaChamadoEnum.Incidente
        });

        Assert.Equal(1, resumo.TotalChamadosComSla);
        Assert.Equal(1, resumo.TotalForaSla);
        Assert.Equal(0, resumo.TotalDentroSla);
    }

    [Fact]
    public async Task ObterResumoAprovacoesAsync_DeveConsolidarPorStatus()
    {
        await using var cenario = await CriarCenarioAsync();

        var resumo = await cenario.UseCase.ObterResumoAprovacoesAsync(FiltroAprovacoes(cenario));

        Assert.Equal(4, resumo.TotalAprovacoes);
        Assert.Equal(1, resumo.Pendentes);
        Assert.Equal(1, resumo.Aprovadas);
        Assert.Equal(1, resumo.Reprovadas);
        Assert.Equal(1, resumo.Canceladas);
        Assert.NotNull(resumo.TempoMedioDecisaoHoras);
    }

    [Fact]
    public async Task ObterTempoMedioAprovacoesAsync_DeveRetornarAgrupadoPorPeriodo()
    {
        await using var cenario = await CriarCenarioAsync();

        var itens = await cenario.UseCase.ObterTempoMedioAprovacoesAsync(new FiltroRelatorioAprovacoesRequest
        {
            DataInicial = cenario.BaseData.AddDays(-20),
            DataFinal = cenario.BaseData,
            AgruparPor = AgruparTempoMedioAprovacoesPor.Periodo,
            Agrupamento = AgrupamentoRelatorio.Dia
        });

        Assert.NotEmpty(itens);
        Assert.All(itens, x => Assert.True(x.TotalDecididas > 0));
    }

    [Fact]
    public async Task ObterAprovacoesPorOrigemAsync_DeveAgruparPorOrigem()
    {
        await using var cenario = await CriarCenarioAsync();

        var itens = await cenario.UseCase.ObterAprovacoesPorOrigemAsync(FiltroAprovacoes(cenario));

        Assert.Contains(itens, x => x.TipoOrigem == TipoOrigemAprovacaoChamado.Manual.ToString() && x.Aprovadas == 1);
        Assert.Contains(itens, x => x.TipoOrigem == TipoOrigemAprovacaoChamado.Categoria.ToString() && x.Reprovadas == 1);
    }

    [Fact]
    public async Task ObterResumoAprovacoesAsync_DeveFiltrarPorStatusAprovacao()
    {
        await using var cenario = await CriarCenarioAsync();

        var resumo = await cenario.UseCase.ObterResumoAprovacoesAsync(new FiltroRelatorioAprovacoesRequest
        {
            DataInicial = cenario.BaseData.AddDays(-20),
            DataFinal = cenario.BaseData,
            StatusAprovacao = StatusAprovacaoChamado.Pendente.ToString()
        });

        Assert.Equal(1, resumo.TotalAprovacoes);
        Assert.Equal(1, resumo.Pendentes);
    }

    [Fact]
    public async Task ObterResumoAprovacoesAsync_DeveFiltrarPorTipoOrigem()
    {
        await using var cenario = await CriarCenarioAsync();

        var resumo = await cenario.UseCase.ObterResumoAprovacoesAsync(new FiltroRelatorioAprovacoesRequest
        {
            DataInicial = cenario.BaseData.AddDays(-20),
            DataFinal = cenario.BaseData,
            TipoOrigemAprovacao = TipoOrigemAprovacaoChamado.CatalogoServico.ToString()
        });

        Assert.Equal(1, resumo.TotalAprovacoes);
        Assert.Equal(1, resumo.Pendentes);
    }

    [Fact]
    public async Task ObterResumoCatalogoServicosAsync_DeveRetornarIndicadoresGerais()
    {
        await using var cenario = await CriarCenarioAsync();

        var resumo = await cenario.UseCase.ObterResumoCatalogoServicosAsync(FiltroCatalogo(cenario));

        Assert.Equal(3, resumo.TotalServicos);
        Assert.Equal(2, resumo.ServicosPublicados);
        Assert.Equal(1, resumo.ServicosArquivados);
        Assert.Equal(3, resumo.ChamadosAbertosPorCatalogo);
    }

    [Fact]
    public async Task ObterCatalogoServicosMaisSolicitadosAsync_DeveRetornarRanking()
    {
        await using var cenario = await CriarCenarioAsync();

        var itens = await cenario.UseCase.ObterCatalogoServicosMaisSolicitadosAsync(FiltroCatalogo(cenario));

        var primeiro = Assert.Single(itens, x => x.CatalogoServicoId == cenario.CatalogoAcessoId);
        Assert.Equal(2, primeiro.TotalChamados);
        Assert.Equal(2, primeiro.TotalComAprovacao);
        Assert.Equal(1, primeiro.TotalReprovadosNaAprovacao);
        Assert.Equal(1, primeiro.TotalForaSla);
    }

    [Fact]
    public async Task ObterCatalogoServicosPorDepartamentoAsync_DeveAgruparPorDepartamento()
    {
        await using var cenario = await CriarCenarioAsync();

        var itens = await cenario.UseCase.ObterCatalogoServicosPorDepartamentoAsync(FiltroCatalogo(cenario));

        Assert.Contains(itens, x => x.DepartamentoId == cenario.DepartamentoInfraId && x.TotalServicos >= 1);
        Assert.Contains(itens, x => x.DepartamentoId == cenario.DepartamentoAppsId && x.TotalServicos >= 1);
    }

    [Fact]
    public async Task ObterCatalogoServicosMaisSolicitadosAsync_DeveFiltrarPorCatalogoServicoId()
    {
        await using var cenario = await CriarCenarioAsync();

        var itens = await cenario.UseCase.ObterCatalogoServicosMaisSolicitadosAsync(new FiltroRelatorioCatalogoServicosRequest
        {
            DataInicial = cenario.BaseData.AddDays(-20),
            DataFinal = cenario.BaseData,
            CatalogoServicoId = cenario.CatalogoAcessoId,
            LimiteRanking = 10
        });

        var item = Assert.Single(itens);
        Assert.Equal(cenario.CatalogoAcessoId, item.CatalogoServicoId);
    }

    [Fact]
    public async Task ObterResumoSlaAsync_ComDataInicialMaiorQueDataFinal_DeveFalhar()
    {
        await using var cenario = await CriarCenarioAsync();

        var erro = await Assert.ThrowsAsync<ArgumentException>(() => cenario.UseCase.ObterResumoSlaAsync(new FiltroRelatorioSlaRequest
        {
            DataInicial = cenario.BaseData,
            DataFinal = cenario.BaseData.AddDays(-1)
        }));

        Assert.Contains("DataInicial", erro.Message);
    }

    [Fact]
    public async Task ObterResumoAprovacoesAsync_ComPeriodoAcimaDoLimite_DeveFalhar()
    {
        await using var cenario = await CriarCenarioAsync();

        var erro = await Assert.ThrowsAsync<ArgumentException>(() => cenario.UseCase.ObterResumoAprovacoesAsync(new FiltroRelatorioAprovacoesRequest
        {
            DataInicial = cenario.BaseData.AddDays(-500),
            DataFinal = cenario.BaseData
        }));

        Assert.Contains("limite maximo", erro.Message);
    }

    [Fact]
    public async Task ObterResumoAprovacoesAsync_ComEnumInvalido_DeveFalhar()
    {
        await using var cenario = await CriarCenarioAsync();

        var erro = await Assert.ThrowsAsync<ArgumentException>(() => cenario.UseCase.ObterResumoAprovacoesAsync(new FiltroRelatorioAprovacoesRequest
        {
            DataInicial = cenario.BaseData.AddDays(-20),
            DataFinal = cenario.BaseData,
            TipoOrigemAprovacao = "NaoExiste"
        }));

        Assert.Contains("TipoOrigemAprovacao", erro.Message);
    }

    private static FiltroRelatorioSlaRequest FiltroSla(Cenario cenario)
        => new()
        {
            DataInicial = cenario.BaseData.AddDays(-20),
            DataFinal = cenario.BaseData,
            LimiteRanking = 20
        };

    private static FiltroRelatorioAprovacoesRequest FiltroAprovacoes(Cenario cenario)
        => new()
        {
            DataInicial = cenario.BaseData.AddDays(-20),
            DataFinal = cenario.BaseData
        };

    private static FiltroRelatorioCatalogoServicosRequest FiltroCatalogo(Cenario cenario)
        => new()
        {
            DataInicial = cenario.BaseData.AddDays(-20),
            DataFinal = cenario.BaseData,
            ApenasAtivos = null,
            LimiteRanking = 10
        };

    private static RelatoriosAvancadosAdminUseCases CriarUseCase(SGXSistemaChamadoDbContext context, Usuario usuario)
    {
        IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService = new FakeUsuarioContextoAplicacaoService(
            AdminUseCasesTestFactory.Contexto(usuario, "Administrador"));

        return new RelatoriosAvancadosAdminUseCases(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            PortalUseCasesTestFactory.Repo<AprovacaoChamado>(context),
            PortalUseCasesTestFactory.Repo<CatalogoServico>(context),
            PortalUseCasesTestFactory.Repo<InventarioAtivo>(context),
            PortalUseCasesTestFactory.Repo<BaseConhecimentoArtigo>(context),
            PortalUseCasesTestFactory.Repo<ChamadoArtigoConhecimento>(context),
            PortalUseCasesTestFactory.Repo<EventoAuditoria>(context),
            usuarioContextoAplicacaoService);
    }

    private static async Task<Cenario> CriarCenarioAsync()
    {
        var context = AdminUseCasesTestFactory.CriarContexto();

        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            context,
            "Admin Relatorios Sprint3",
            $"admin.relatorios.sprint3.{Guid.NewGuid():N}@sgx.local",
            TipoPerfil.Administrador);

        var atendente = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            context,
            "Atendente Sprint3",
            $"atendente.relatorios.sprint3.{Guid.NewGuid():N}@sgx.local",
            TipoPerfil.Atendente);

        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            context,
            "Solicitante Sprint3",
            $"solicitante.relatorios.sprint3.{Guid.NewGuid():N}@sgx.local",
            TipoPerfil.Solicitante);

        var departamentoInfra = new Departamento("Infraestrutura Sprint3", "INF3", "Infra", "teste");
        var departamentoApps = new Departamento("Aplicacoes Sprint3", "APP3", "Apps", "teste");
        context.Departamentos.AddRange(departamentoInfra, departamentoApps);

        var categoriaInfra = new CategoriaChamado("Incidentes Infra Sprint3", "Categoria Infra", departamentoInfra.Id, "teste");
        var categoriaApps = new CategoriaChamado("Incidentes Apps Sprint3", "Categoria Apps", departamentoApps.Id, "teste");
        context.CategoriasChamado.AddRange(categoriaInfra, categoriaApps);

        var prioridadeAlta = await context.PrioridadesChamado.FirstOrDefaultAsync(x => x.Nivel == PrioridadeChamadoEnum.Alta)
            ?? await context.PrioridadesChamado.FirstAsync();
        var prioridadeMedia = await context.PrioridadesChamado.FirstOrDefaultAsync(x => x.Nivel == PrioridadeChamadoEnum.Media)
            ?? await context.PrioridadesChamado.OrderBy(x => x.Nome).FirstAsync();

        var statusAberto = await ObterOuCriarStatusAsync(context, StatusChamadoEnum.Aberto, "Aberto");
        var statusEncerrado = await ObterOuCriarStatusAsync(context, StatusChamadoEnum.Encerrado, "Encerrado");

        var catalogoAcesso = new CatalogoServico(
            "Acesso VPN Sprint3",
            $"acesso-vpn-{Guid.NewGuid():N}",
            "Servico de acesso",
            null,
            departamentoInfra.Id,
            categoriaInfra.Id,
            null,
            prioridadeAlta.Id,
            null,
            null,
            VisibilidadeCatalogoServico.Interno,
            true,
            true,
            1,
            admin.Id,
            "teste");
        catalogoAcesso.Publicar(admin.Id, "teste");

        var catalogoNotebook = new CatalogoServico(
            "Notebook Sprint3",
            $"notebook-{Guid.NewGuid():N}",
            "Servico de notebook",
            null,
            departamentoApps.Id,
            categoriaApps.Id,
            null,
            prioridadeMedia.Id,
            null,
            null,
            VisibilidadeCatalogoServico.Interno,
            true,
            false,
            2,
            admin.Id,
            "teste");
        catalogoNotebook.Publicar(admin.Id, "teste");

        var catalogoLegacy = new CatalogoServico(
            "Legacy Sprint3",
            $"legacy-{Guid.NewGuid():N}",
            "Servico legado",
            null,
            departamentoApps.Id,
            categoriaApps.Id,
            null,
            prioridadeMedia.Id,
            null,
            null,
            VisibilidadeCatalogoServico.Interno,
            true,
            false,
            3,
            admin.Id,
            "teste");
        catalogoLegacy.Arquivar(admin.Id, "teste");

        context.CatalogosServico.AddRange(catalogoAcesso, catalogoNotebook, catalogoLegacy);
        await context.SaveChangesAsync();

        var baseData = DateTime.UtcNow.Date;

        var chamado1 = new Chamado("CH-RAV3-001", "Dentro SLA", "Descricao", solicitante.Id, categoriaInfra.Id, prioridadeAlta.Id, statusAberto.Id, OrigemChamado.Portal, "teste", categoriaInfra.DepartamentoId, catalogoServicoId: catalogoAcesso.Id);
        chamado1.AtribuirResponsavel(atendente.Id, "teste");
        chamado1.Encerrar(statusEncerrado.Id, "teste");

        var chamado2 = new Chamado("CH-RAV3-002", "Fora SLA", "Descricao", solicitante.Id, categoriaInfra.Id, prioridadeAlta.Id, statusAberto.Id, OrigemChamado.Portal, "teste", categoriaInfra.DepartamentoId, catalogoServicoId: catalogoAcesso.Id, naturezaChamado: NaturezaChamadoEnum.Incidente);
        chamado2.AtribuirResponsavel(atendente.Id, "teste");
        chamado2.Encerrar(statusEncerrado.Id, "teste");

        var chamado3 = new Chamado("CH-RAV3-003", "Sem SLA", "Descricao", solicitante.Id, categoriaApps.Id, prioridadeMedia.Id, statusAberto.Id, OrigemChamado.Email, "teste", categoriaApps.DepartamentoId, catalogoServicoId: catalogoNotebook.Id);
        chamado3.AtribuirResponsavel(atendente.Id, "teste");

        var chamado4 = new Chamado("CH-RAV3-004", "Dentro SLA 2", "Descricao", solicitante.Id, categoriaApps.Id, prioridadeMedia.Id, statusAberto.Id, OrigemChamado.Admin, "teste", categoriaApps.DepartamentoId);
        chamado4.AtribuirResponsavel(atendente.Id, "teste");
        chamado4.Encerrar(statusEncerrado.Id, "teste");

        var chamadoForaPeriodo = new Chamado("CH-RAV3-005", "Fora periodo", "Descricao", solicitante.Id, categoriaInfra.Id, prioridadeAlta.Id, statusAberto.Id, OrigemChamado.Portal, "teste", categoriaInfra.DepartamentoId, catalogoServicoId: catalogoAcesso.Id);
        chamadoForaPeriodo.AtribuirResponsavel(atendente.Id, "teste");
        chamadoForaPeriodo.Encerrar(statusEncerrado.Id, "teste");

        context.Chamados.AddRange(chamado1, chamado2, chamado3, chamado4, chamadoForaPeriodo);
        await context.SaveChangesAsync();

        DefinirDatasChamado(context, chamado1, baseData.AddDays(-10), baseData.AddDays(-9));
        DefinirDatasChamado(context, chamado2, baseData.AddDays(-7), baseData.AddDays(-4));
        DefinirDatasChamado(context, chamado3, baseData.AddDays(-5), null);
        DefinirDatasChamado(context, chamado4, baseData.AddDays(-3), baseData.AddDays(-2));
        DefinirDatasChamado(context, chamadoForaPeriodo, baseData.AddDays(-90), baseData.AddDays(-89));

        var sla1 = new ChamadoSla(chamado1.Id, null, prioridadeAlta.Id, baseData.AddDays(-10).AddHours(8), baseData.AddDays(-10).AddHours(9), baseData.AddDays(-9).AddHours(8), false, false, null, "teste");
        sla1.RegistrarResolucao(baseData.AddDays(-9).AddHours(7), "teste");

        var sla2 = new ChamadoSla(chamado2.Id, null, prioridadeAlta.Id, baseData.AddDays(-7).AddHours(8), baseData.AddDays(-7).AddHours(10), baseData.AddDays(-6).AddHours(8), false, false, null, "teste");
        sla2.RegistrarResolucao(baseData.AddDays(-4).AddHours(9), "teste");

        var sla4 = new ChamadoSla(chamado4.Id, null, prioridadeMedia.Id, baseData.AddDays(-3).AddHours(8), baseData.AddDays(-3).AddHours(10), baseData.AddDays(-2).AddHours(20), false, false, null, "teste");
        sla4.RegistrarResolucao(baseData.AddDays(-2).AddHours(18), "teste");

        context.ChamadosSla.AddRange(sla1, sla2, sla4);

        var aprovacaoAprovada = new AprovacaoChamado(chamado1.Id, TipoOrigemAprovacaoChamado.Manual, admin.Id, "teste", solicitante.Id, "Fluxo", "Solicitacao");
        aprovacaoAprovada.Aprovar(admin.Id, admin.Id, "teste", "Aprovado");
        context.Entry(aprovacaoAprovada).Property(nameof(AprovacaoChamado.SolicitadaEm)).CurrentValue = baseData.AddDays(-10).AddHours(8);
        context.Entry(aprovacaoAprovada).Property(nameof(AprovacaoChamado.DecididaEm)).CurrentValue = baseData.AddDays(-10).AddHours(10);

        var aprovacaoReprovada = new AprovacaoChamado(chamado2.Id, TipoOrigemAprovacaoChamado.Categoria, admin.Id, "teste", solicitante.Id, "Fluxo", "Solicitacao");
        aprovacaoReprovada.Reprovar(admin.Id, admin.Id, "teste", "Reprovado");
        context.Entry(aprovacaoReprovada).Property(nameof(AprovacaoChamado.SolicitadaEm)).CurrentValue = baseData.AddDays(-7).AddHours(8);
        context.Entry(aprovacaoReprovada).Property(nameof(AprovacaoChamado.DecididaEm)).CurrentValue = baseData.AddDays(-7).AddHours(11);

        var aprovacaoPendente = new AprovacaoChamado(chamado3.Id, TipoOrigemAprovacaoChamado.CatalogoServico, admin.Id, "teste", solicitante.Id, "Fluxo", "Solicitacao");
        context.Entry(aprovacaoPendente).Property(nameof(AprovacaoChamado.SolicitadaEm)).CurrentValue = baseData.AddDays(-5).AddHours(8);

        var aprovacaoCancelada = new AprovacaoChamado(chamado4.Id, TipoOrigemAprovacaoChamado.Departamento, admin.Id, "teste", solicitante.Id, "Fluxo", "Solicitacao");
        aprovacaoCancelada.Cancelar(admin.Id, "teste", "Cancelada");
        context.Entry(aprovacaoCancelada).Property(nameof(AprovacaoChamado.SolicitadaEm)).CurrentValue = baseData.AddDays(-3).AddHours(8);
        context.Entry(aprovacaoCancelada).Property(nameof(AprovacaoChamado.DecididaEm)).CurrentValue = baseData.AddDays(-3).AddHours(9);

        context.AprovacoesChamado.AddRange(aprovacaoAprovada, aprovacaoReprovada, aprovacaoPendente, aprovacaoCancelada);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, admin);
        return new Cenario(
            context,
            useCase,
            baseData,
            departamentoInfra.Id,
            departamentoApps.Id,
            prioridadeAlta.Id,
            catalogoAcesso.Id);
    }

    private static void DefinirDatasChamado(SGXSistemaChamadoDbContext context, Chamado chamado, DateTime abertoEm, DateTime? encerradoEm)
    {
        context.Entry(chamado).Property(nameof(Chamado.AbertoEm)).CurrentValue = abertoEm;
        context.Entry(chamado).Property(nameof(Chamado.EncerradoEm)).CurrentValue = encerradoEm;
    }

    private static async Task<StatusChamado> ObterOuCriarStatusAsync(
        SGXSistemaChamadoDbContext context,
        StatusChamadoEnum codigo,
        string nome)
    {
        var existente = await context.StatusChamado.FirstOrDefaultAsync(x => x.Codigo == codigo);
        if (existente is not null)
        {
            return existente;
        }

        var status = new StatusChamado(nome, codigo, $"Status {nome} para testes.", false, false, "teste");
        context.StatusChamado.Add(status);
        await context.SaveChangesAsync();
        return status;
    }

    private sealed record Cenario(
        SGXSistemaChamadoDbContext Context,
        RelatoriosAvancadosAdminUseCases UseCase,
        DateTime BaseData,
        Guid DepartamentoInfraId,
        Guid DepartamentoAppsId,
        Guid PrioridadeAltaId,
        Guid CatalogoAcessoId) : IAsyncDisposable
    {
        public ValueTask DisposeAsync()
            => Context.DisposeAsync();
    }
}
