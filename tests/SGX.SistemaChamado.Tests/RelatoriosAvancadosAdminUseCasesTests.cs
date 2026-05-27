using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class RelatoriosAvancadosAdminUseCasesTests
{
    [Fact]
    public async Task ObterMetadadosAsync_DeveRetornarEstruturaBaseEsperada()
    {
        await using var contexto = PortalUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            contexto,
            "Admin Relatorios",
            $"admin.relatorios.{Guid.NewGuid():N}@sgx.local",
            TipoPerfil.Administrador);

        var useCase = CriarUseCase(contexto, admin);

        var metadados = await useCase.ObterMetadadosAsync();

        Assert.NotEmpty(metadados.PeriodosSuportados);
        Assert.NotEmpty(metadados.TiposRelatorioDisponiveis);
        Assert.NotEmpty(metadados.AgrupamentosSuportados);
        Assert.NotEmpty(metadados.FiltrosDisponiveis);
        Assert.NotEmpty(metadados.FormatosExportacaoPlanejados);
        Assert.Contains("RelatoriosAvancados.Visualizar", metadados.PermissoesRelevantes);
        Assert.Contains(TipoRelatorioAvancado.Chamados, metadados.TiposRelatorioDisponiveis);
        Assert.Contains(TipoRelatorioAvancado.Atendimento, metadados.TiposRelatorioDisponiveis);
    }

    [Fact]
    public async Task ObterResumoChamadosAsync_DeveRetornarTotalNoPeriodo()
    {
        await using var cenario = await CriarCenarioAsync();

        var resumo = await cenario.UseCase.ObterResumoChamadosAsync(new FiltroRelatorioChamadosRequest
        {
            DataInicial = cenario.BaseData.AddDays(-15),
            DataFinal = cenario.BaseData
        });

        Assert.Equal(4, resumo.TotalChamados);
        Assert.Equal(1, resumo.TotalAbertos);
        Assert.Equal(1, resumo.TotalEmAtendimento);
        Assert.Equal(1, resumo.TotalEncerradosOuConcluidos);
        Assert.Equal(1, resumo.TotalCancelados);
    }

    [Fact]
    public async Task ObterResumoChamadosAsync_DeveFiltrarPorDepartamento()
    {
        await using var cenario = await CriarCenarioAsync();

        var resumo = await cenario.UseCase.ObterResumoChamadosAsync(new FiltroRelatorioChamadosRequest
        {
            DataInicial = cenario.BaseData.AddDays(-15),
            DataFinal = cenario.BaseData,
            DepartamentoId = cenario.DepartamentoInfraId
        });

        Assert.Equal(2, resumo.TotalChamados);
    }

    [Fact]
    public async Task ObterResumoChamadosAsync_DeveFiltrarPorCategoria()
    {
        await using var cenario = await CriarCenarioAsync();

        var resumo = await cenario.UseCase.ObterResumoChamadosAsync(new FiltroRelatorioChamadosRequest
        {
            DataInicial = cenario.BaseData.AddDays(-15),
            DataFinal = cenario.BaseData,
            CategoriaId = cenario.CategoriaAppsId
        });

        Assert.Equal(2, resumo.TotalChamados);
    }

    [Fact]
    public async Task ObterResumoChamadosAsync_DeveFiltrarPorPrioridade()
    {
        await using var cenario = await CriarCenarioAsync();

        var resumo = await cenario.UseCase.ObterResumoChamadosAsync(new FiltroRelatorioChamadosRequest
        {
            DataInicial = cenario.BaseData.AddDays(-15),
            DataFinal = cenario.BaseData,
            PrioridadeId = cenario.PrioridadeAltaId
        });

        Assert.Equal(2, resumo.TotalChamados);
    }

    [Fact]
    public async Task ObterResumoChamadosAsync_DeveFiltrarPorNaturezaIncidente()
    {
        await using var cenario = await CriarCenarioAsync();

        var resumo = await cenario.UseCase.ObterResumoChamadosAsync(new FiltroRelatorioChamadosRequest
        {
            DataInicial = cenario.BaseData.AddDays(-15),
            DataFinal = cenario.BaseData,
            NaturezaChamado = NaturezaChamadoEnum.Incidente
        });

        Assert.Equal(2, resumo.TotalChamados);
        Assert.Equal(2, resumo.TotalPorNatureza.First(x => x.Chave == ((int)NaturezaChamadoEnum.Incidente).ToString()).Quantidade);
        Assert.Equal(0, resumo.TotalPorNatureza.First(x => x.Chave == ((int)NaturezaChamadoEnum.Requisicao).ToString()).Quantidade);
    }

    [Fact]
    public async Task ObterResumoChamadosAsync_DeveFiltrarPorNaturezaRequisicao()
    {
        await using var cenario = await CriarCenarioAsync();

        var resumo = await cenario.UseCase.ObterResumoChamadosAsync(new FiltroRelatorioChamadosRequest
        {
            DataInicial = cenario.BaseData.AddDays(-15),
            DataFinal = cenario.BaseData,
            NaturezaChamado = NaturezaChamadoEnum.Requisicao
        });

        Assert.Equal(1, resumo.TotalChamados);
    }

    [Fact]
    public async Task ObterResumoChamadosAsync_DeveCombinarFiltroNaturezaComPrioridade()
    {
        await using var cenario = await CriarCenarioAsync();

        var resumo = await cenario.UseCase.ObterResumoChamadosAsync(new FiltroRelatorioChamadosRequest
        {
            DataInicial = cenario.BaseData.AddDays(-15),
            DataFinal = cenario.BaseData,
            NaturezaChamado = NaturezaChamadoEnum.Incidente,
            PrioridadeId = cenario.PrioridadeAltaId
        });

        Assert.Equal(1, resumo.TotalChamados);
    }

    [Fact]
    public async Task ObterResumoChamadosAsync_DeveIncluirChamadosComAprovacaoPendente()
    {
        await using var cenario = await CriarCenarioAsync();

        var resumo = await cenario.UseCase.ObterResumoChamadosAsync(new FiltroRelatorioChamadosRequest
        {
            DataInicial = cenario.BaseData.AddDays(-15),
            DataFinal = cenario.BaseData
        });

        Assert.Equal(1, resumo.TotalComAprovacaoPendente);
        Assert.Equal(1, resumo.TotalReprovadosNaAprovacao);
    }

    [Fact]
    public async Task ObterSerieTemporalChamadosAsync_DeveRetornarSeriePorDia()
    {
        await using var cenario = await CriarCenarioAsync();

        var serie = await cenario.UseCase.ObterSerieTemporalChamadosAsync(new FiltroRelatorioChamadosRequest
        {
            DataInicial = cenario.BaseData.AddDays(-15),
            DataFinal = cenario.BaseData,
            Agrupamento = AgrupamentoRelatorio.Dia
        });

        Assert.Equal(AgrupamentoRelatorio.Dia, serie.Agrupamento);
        Assert.NotEmpty(serie.Itens);
        Assert.Contains(serie.Itens, x => x.Abertos > 0);
    }

    [Fact]
    public async Task ObterSerieTemporalChamadosAsync_DeveRetornarSeriePorMes()
    {
        await using var cenario = await CriarCenarioAsync();

        var serie = await cenario.UseCase.ObterSerieTemporalChamadosAsync(new FiltroRelatorioChamadosRequest
        {
            DataInicial = cenario.BaseData.AddDays(-150),
            DataFinal = cenario.BaseData,
            Agrupamento = AgrupamentoRelatorio.Mes
        });

        Assert.Equal(AgrupamentoRelatorio.Mes, serie.Agrupamento);
        Assert.True(serie.Itens.Count >= 2);
    }

    [Fact]
    public async Task ObterDistribuicaoChamadosAsync_DeveDistribuirPorStatus()
    {
        await using var cenario = await CriarCenarioAsync();

        var distribuicao = await cenario.UseCase.ObterDistribuicaoChamadosAsync(new FiltroRelatorioChamadosRequest
        {
            DataInicial = cenario.BaseData.AddDays(-15),
            DataFinal = cenario.BaseData,
            AgruparPor = AgruparPorRelatorioChamados.Status
        });

        Assert.Equal(AgruparPorRelatorioChamados.Status, distribuicao.AgruparPor);
        Assert.Contains(distribuicao.Itens, x => x.Nome == "Aberto");
    }

    [Fact]
    public async Task ObterDistribuicaoChamadosAsync_DeveDistribuirPorPrioridade()
    {
        await using var cenario = await CriarCenarioAsync();

        var distribuicao = await cenario.UseCase.ObterDistribuicaoChamadosAsync(new FiltroRelatorioChamadosRequest
        {
            DataInicial = cenario.BaseData.AddDays(-15),
            DataFinal = cenario.BaseData,
            AgruparPor = AgruparPorRelatorioChamados.Prioridade
        });

        Assert.Equal(AgruparPorRelatorioChamados.Prioridade, distribuicao.AgruparPor);
        Assert.True(distribuicao.Itens.Sum(x => x.Quantidade) >= 4);
    }

    [Fact]
    public async Task ObterDistribuicaoChamadosAsync_DeveDistribuirPorDepartamento()
    {
        await using var cenario = await CriarCenarioAsync();

        var distribuicao = await cenario.UseCase.ObterDistribuicaoChamadosAsync(new FiltroRelatorioChamadosRequest
        {
            DataInicial = cenario.BaseData.AddDays(-15),
            DataFinal = cenario.BaseData,
            AgruparPor = AgruparPorRelatorioChamados.Departamento
        });

        Assert.Equal(AgruparPorRelatorioChamados.Departamento, distribuicao.AgruparPor);
        Assert.Contains(distribuicao.Itens, x => x.Chave == cenario.DepartamentoInfraId.ToString());
        Assert.Contains(distribuicao.Itens, x => x.Chave == cenario.DepartamentoAppsId.ToString());
    }

    [Fact]
    public async Task ObterDistribuicaoChamadosAsync_DeveDistribuirPorNaturezaComSeisNaturezas()
    {
        await using var cenario = await CriarCenarioAsync();

        var distribuicao = await cenario.UseCase.ObterDistribuicaoChamadosAsync(new FiltroRelatorioChamadosRequest
        {
            DataInicial = cenario.BaseData.AddDays(-15),
            DataFinal = cenario.BaseData,
            AgruparPor = AgruparPorRelatorioChamados.Natureza
        });

        Assert.Equal(AgruparPorRelatorioChamados.Natureza, distribuicao.AgruparPor);
        Assert.Equal(6, distribuicao.Itens.Count);
        Assert.Contains(distribuicao.Itens, x => x.Chave == ((int)NaturezaChamadoEnum.EventoAlerta).ToString() && x.Quantidade == 1);
        Assert.Contains(distribuicao.Itens, x => x.Chave == ((int)NaturezaChamadoEnum.TarefaOperacional).ToString() && x.Quantidade == 0);
    }

    [Fact]
    public async Task ObterProdutividadeAtendimentoAsync_DeveRetornarRankingPorAtendente()
    {
        await using var cenario = await CriarCenarioAsync();

        var produtividade = await cenario.UseCase.ObterProdutividadeAtendimentoAsync(new FiltroRelatorioAtendimentoRequest
        {
            DataInicial = cenario.BaseData.AddDays(-15),
            DataFinal = cenario.BaseData,
            LimiteRanking = 10
        });

        Assert.NotEmpty(produtividade.Ranking);
        Assert.Contains(produtividade.Ranking, x => x.AtendenteId == cenario.AtendenteInfraId);
        Assert.Contains(produtividade.Ranking, x => x.AtendenteId == cenario.AtendenteAppsId);
    }

    [Fact]
    public async Task ObterProdutividadeAtendimentoAsync_DeveRespeitarPeriodo()
    {
        await using var cenario = await CriarCenarioAsync();

        var produtividade = await cenario.UseCase.ObterProdutividadeAtendimentoAsync(new FiltroRelatorioAtendimentoRequest
        {
            DataInicial = cenario.BaseData.AddDays(-3),
            DataFinal = cenario.BaseData,
            LimiteRanking = 10
        });

        var atendenteInfra = produtividade.Ranking.First(x => x.AtendenteId == cenario.AtendenteInfraId);
        Assert.Equal(1, atendenteInfra.ChamadosAssumidos);
    }

    [Fact]
    public async Task ObterResumoChamadosAsync_ComDataInicialMaiorQueDataFinal_DeveFalhar()
    {
        await using var cenario = await CriarCenarioAsync();

        var erro = await Assert.ThrowsAsync<ArgumentException>(() => cenario.UseCase.ObterResumoChamadosAsync(new FiltroRelatorioChamadosRequest
        {
            DataInicial = cenario.BaseData,
            DataFinal = cenario.BaseData.AddDays(-1)
        }));

        Assert.Contains("DataInicial", erro.Message);
    }

    [Fact]
    public async Task ObterSerieTemporalChamadosAsync_ComAgrupamentoInvalido_DeveFalhar()
    {
        await using var cenario = await CriarCenarioAsync();

        var erro = await Assert.ThrowsAsync<ArgumentException>(() => cenario.UseCase.ObterSerieTemporalChamadosAsync(new FiltroRelatorioChamadosRequest
        {
            DataInicial = cenario.BaseData.AddDays(-10),
            DataFinal = cenario.BaseData,
            Agrupamento = AgrupamentoRelatorio.Ano
        }));

        Assert.Contains("Agrupamento temporal invalido", erro.Message);
    }

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

    private static async Task<CenarioRelatorios> CriarCenarioAsync()
    {
        var context = AdminUseCasesTestFactory.CriarContexto();

        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            context,
            "Admin Relatorios",
            $"admin.relatorios.{Guid.NewGuid():N}@sgx.local",
            TipoPerfil.Administrador);

        var atendenteInfra = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            context,
            "Atendente Infra",
            $"atendente.infra.{Guid.NewGuid():N}@sgx.local",
            TipoPerfil.Atendente);

        var atendenteApps = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            context,
            "Atendente Apps",
            $"atendente.apps.{Guid.NewGuid():N}@sgx.local",
            TipoPerfil.Atendente);

        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            context,
            "Solicitante Relatorios",
            $"solicitante.relatorios.{Guid.NewGuid():N}@sgx.local",
            TipoPerfil.Solicitante);

        var departamentoInfra = new Departamento("Infraestrutura", "INF", "Infra de testes", "teste");
        var departamentoApps = new Departamento("Aplicacoes", "APP", "Apps de testes", "teste");
        context.Departamentos.AddRange(departamentoInfra, departamentoApps);

        var categoriaInfra = new CategoriaChamado("Incidentes Infra", "Categoria Infra", departamentoInfra.Id, "teste");
        var categoriaApps = new CategoriaChamado("Incidentes Apps", "Categoria Apps", departamentoApps.Id, "teste");
        context.CategoriasChamado.AddRange(categoriaInfra, categoriaApps);

        var prioridadeAlta = await context.PrioridadesChamado.FirstOrDefaultAsync(x => x.Nivel == PrioridadeChamadoEnum.Alta)
            ?? await context.PrioridadesChamado.FirstAsync();
        var prioridadeMedia = await context.PrioridadesChamado.FirstOrDefaultAsync(x => x.Nivel == PrioridadeChamadoEnum.Media)
            ?? await context.PrioridadesChamado.OrderBy(x => x.Nome).FirstAsync();

        var statusAberto = await ObterOuCriarStatusAsync(context, StatusChamadoEnum.Aberto, "Aberto");
        var statusAtendimento = await ObterOuCriarStatusAsync(context, StatusChamadoEnum.EmAtendimento, "Em atendimento");
        var statusEncerrado = await ObterOuCriarStatusAsync(context, StatusChamadoEnum.Encerrado, "Encerrado");
        var statusCancelado = await ObterOuCriarStatusAsync(context, StatusChamadoEnum.Cancelado, "Cancelado");

        var baseData = DateTime.UtcNow.Date;

        var chamadoEncerrado = new Chamado(
            "CH-RAV-001",
            "Chamado encerrado",
            "Descricao",
            solicitante.Id,
            categoriaInfra.Id,
            prioridadeAlta.Id,
            statusAberto.Id,
            OrigemChamado.Portal,
            "teste",
            categoriaInfra.DepartamentoId,
            catalogoServicoId: Guid.NewGuid(),
            naturezaChamado: NaturezaChamadoEnum.Incidente);
        chamadoEncerrado.AtribuirResponsavel(atendenteInfra.Id, "teste");
        chamadoEncerrado.Encerrar(statusEncerrado.Id, "teste");
        chamadoEncerrado.VincularInventarioAtivo(Guid.NewGuid(), "teste");

        var chamadoAberto = new Chamado(
            "CH-RAV-002",
            "Chamado aberto",
            "Descricao",
            solicitante.Id,
            categoriaInfra.Id,
            prioridadeAlta.Id,
            statusAberto.Id,
            OrigemChamado.Email,
            "teste",
            categoriaInfra.DepartamentoId,
            naturezaChamado: NaturezaChamadoEnum.Requisicao);
        chamadoAberto.AtribuirResponsavel(atendenteInfra.Id, "teste");

        var chamadoAtendimento = new Chamado(
            "CH-RAV-003",
            "Chamado atendimento",
            "Descricao",
            solicitante.Id,
            categoriaApps.Id,
            prioridadeMedia.Id,
            statusAberto.Id,
            OrigemChamado.Admin,
            "teste",
            categoriaApps.DepartamentoId,
            naturezaChamado: NaturezaChamadoEnum.Incidente);
        chamadoAtendimento.AtribuirResponsavel(atendenteApps.Id, "teste");
        chamadoAtendimento.AlterarStatus(statusAtendimento.Id, "teste");

        var chamadoCancelado = new Chamado(
            "CH-RAV-004",
            "Chamado cancelado",
            "Descricao",
            solicitante.Id,
            categoriaApps.Id,
            prioridadeMedia.Id,
            statusAberto.Id,
            OrigemChamado.Portal,
            "teste",
            categoriaApps.DepartamentoId,
            naturezaChamado: NaturezaChamadoEnum.EventoAlerta);
        chamadoCancelado.AlterarStatus(statusCancelado.Id, "teste");

        var chamadoForaPeriodo = new Chamado(
            "CH-RAV-005",
            "Chamado fora do periodo",
            "Descricao",
            solicitante.Id,
            categoriaInfra.Id,
            prioridadeMedia.Id,
            statusAberto.Id,
            OrigemChamado.Portal,
            "teste",
            categoriaInfra.DepartamentoId,
            naturezaChamado: NaturezaChamadoEnum.TarefaOperacional);
        chamadoForaPeriodo.AtribuirResponsavel(atendenteInfra.Id, "teste");

        context.Chamados.AddRange(chamadoEncerrado, chamadoAberto, chamadoAtendimento, chamadoCancelado, chamadoForaPeriodo);
        await context.SaveChangesAsync();

        DefinirDatasChamado(context, chamadoEncerrado, baseData.AddDays(-6), baseData.AddDays(-4));
        DefinirDatasChamado(context, chamadoAberto, baseData.AddDays(-3), null);
        DefinirDatasChamado(context, chamadoAtendimento, baseData.AddDays(-2), null);
        DefinirDatasChamado(context, chamadoCancelado, baseData.AddDays(-1), null);
        DefinirDatasChamado(context, chamadoForaPeriodo, baseData.AddDays(-90), null);

        var historicoReabertura = new HistoricoChamado(
            chamadoEncerrado.Id,
            TipoHistoricoChamado.Reaberto,
            "Chamado reaberto para ajuste",
            atendenteInfra.Id,
            "teste");
        context.HistoricosChamado.Add(historicoReabertura);
        context.Entry(historicoReabertura).Property(nameof(HistoricoChamado.CriadoEm)).CurrentValue = baseData.AddDays(-2).AddHours(2);

        var aprovacaoPendente = new AprovacaoChamado(
            chamadoAberto.Id,
            TipoOrigemAprovacaoChamado.Manual,
            admin.Id,
            "teste",
            solicitante.Id,
            "Fluxo teste",
            "Precisa validar impacto");

        var aprovacaoReprovada = new AprovacaoChamado(
            chamadoAtendimento.Id,
            TipoOrigemAprovacaoChamado.Categoria,
            admin.Id,
            "teste",
            solicitante.Id,
            "Fluxo teste",
            "Precisa evidencias");
        aprovacaoReprovada.Reprovar(admin.Id, admin.Id, "teste", "Evidencias insuficientes");

        context.AprovacoesChamado.AddRange(aprovacaoPendente, aprovacaoReprovada);

        var slaChamadoEncerrado = new ChamadoSla(
            chamadoEncerrado.Id,
            null,
            prioridadeAlta.Id,
            baseData.AddDays(-6).AddHours(8),
            baseData.AddDays(-6).AddHours(12),
            baseData.AddDays(-4).AddHours(18),
            false,
            false,
            null,
            "teste");
        slaChamadoEncerrado.RegistrarPrimeiraResposta(baseData.AddDays(-6).AddHours(10), "teste");

        context.ChamadosSla.Add(slaChamadoEncerrado);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, admin);
        return new CenarioRelatorios(
            context,
            useCase,
            baseData,
            departamentoInfra.Id,
            departamentoApps.Id,
            categoriaInfra.Id,
            categoriaApps.Id,
            prioridadeAlta.Id,
            atendenteInfra.Id,
            atendenteApps.Id);
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

    private sealed record CenarioRelatorios(
        SGXSistemaChamadoDbContext Context,
        RelatoriosAvancadosAdminUseCases UseCase,
        DateTime BaseData,
        Guid DepartamentoInfraId,
        Guid DepartamentoAppsId,
        Guid CategoriaInfraId,
        Guid CategoriaAppsId,
        Guid PrioridadeAltaId,
        Guid AtendenteInfraId,
        Guid AtendenteAppsId) : IAsyncDisposable
    {
        public ValueTask DisposeAsync()
            => Context.DisposeAsync();
    }
}

