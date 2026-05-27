using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public sealed class RelatoriosAvancadosAdminUseCases(
    IRepository<Chamado> chamadoRepository,
    IRepository<HistoricoChamado> historicoChamadoRepository,
    IRepository<AprovacaoChamado> aprovacaoChamadoRepository,
    IRepository<CatalogoServico> catalogoServicoRepository,
    IRepository<InventarioAtivo> inventarioAtivoRepository,
    IRepository<BaseConhecimentoArtigo> baseConhecimentoArtigoRepository,
    IRepository<ChamadoArtigoConhecimento> chamadoArtigoConhecimentoRepository,
    IRepository<EventoAuditoria> eventoAuditoriaRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IAdminRelatoriosAvancadosUseCases
{
    private const int LimiteMaximoRanking = 100;
    private const int LimitePadraoRanking = 20;
    private const int MaxDiasPeriodo = 366;

    private static readonly string[] PeriodosSuportados =
    [
        "Hoje",
        "Ontem",
        "Ultimos7Dias",
        "Ultimos30Dias",
        "MesAtual",
        "MesAnterior",
        "TrimestreAtual",
        "AnoAtual",
        "Personalizado"
    ];

    private static readonly string[] FiltrosDisponiveis =
    [
        "DataInicial",
        "DataFinal",
        "DepartamentoId",
        "LocalUnidadeId",
        "UsuarioResponsavelId",
        "TipoAtivoInventarioId",
        "StatusOperacional",
        "StatusPatrimonial",
        "Criticidade",
        "CategoriaId",
        "SubcategoriaId",
        "PrioridadeId",
        "Status",
        "StatusArtigo",
        "VisibilidadeArtigo",
        "StatusId",
        "AtendenteId",
        "SolicitanteId",
        "CatalogoServicoId",
        "UsuarioId",
        "Entidade",
        "TipoAcao",
        "Termo",
        "TipoOrigemAprovacao",
        "StatusAprovacao",
        "InventarioAtivoId",
        "Origem",
        "NaturezaChamado",
        "Ativo",
        "ApenasAtivos",
        "Agrupamento",
        "AgruparPor"
    ];

    private static readonly (NaturezaChamadoEnum Codigo, string Nome)[] NaturezasOrdenadas =
    [
        (NaturezaChamadoEnum.Incidente, "Incidente"),
        (NaturezaChamadoEnum.Requisicao, "Requisicao"),
        (NaturezaChamadoEnum.Mudanca, "Mudanca"),
        (NaturezaChamadoEnum.Problema, "Problema"),
        (NaturezaChamadoEnum.EventoAlerta, "Evento/Alerta"),
        (NaturezaChamadoEnum.TarefaOperacional, "Tarefa operacional")
    ];

    private static readonly string[] PermissoesRelevantes =
    [
        "RelatoriosAvancados.Visualizar",
        "RelatoriosAvancados.Exportar",
        "RelatoriosAvancados.Gerencial",
        "RelatoriosAvancados.Operacional",
        "RelatoriosAvancados.Auditoria"
    ];

    private static readonly TipoRelatorioAvancado[] TiposRelatorioDisponiveis =
    [
        TipoRelatorioAvancado.Chamados,
        TipoRelatorioAvancado.Sla,
        TipoRelatorioAvancado.Atendimento,
        TipoRelatorioAvancado.Departamentos,
        TipoRelatorioAvancado.CatalogoServicos,
        TipoRelatorioAvancado.Aprovacoes,
        TipoRelatorioAvancado.InventarioAtivos,
        TipoRelatorioAvancado.BaseConhecimento,
        TipoRelatorioAvancado.Auditoria
    ];

    private static readonly AgrupamentoRelatorio[] AgrupamentosSuportados =
    [
        AgrupamentoRelatorio.Dia,
        AgrupamentoRelatorio.Semana,
        AgrupamentoRelatorio.Mes,
        AgrupamentoRelatorio.Trimestre,
        AgrupamentoRelatorio.Ano,
        AgrupamentoRelatorio.Departamento,
        AgrupamentoRelatorio.Categoria,
        AgrupamentoRelatorio.Prioridade,
        AgrupamentoRelatorio.Responsavel,
        AgrupamentoRelatorio.Status,
        AgrupamentoRelatorio.CatalogoServico,
        AgrupamentoRelatorio.Solicitante,
        AgrupamentoRelatorio.AtivoVinculado,
        AgrupamentoRelatorio.Atendente
    ];

    private static readonly FormatoExportacaoRelatorio[] FormatosExportacaoPlanejados =
    [
        FormatoExportacaoRelatorio.Csv,
        FormatoExportacaoRelatorio.Xlsx,
        FormatoExportacaoRelatorio.Pdf,
        FormatoExportacaoRelatorio.Json
    ];

    public async Task<RelatorioMetadadosDto> ObterMetadadosAsync(CancellationToken cancellationToken = default)
    {
        await GarantirAcessoAdminAsync(cancellationToken);

        return new RelatorioMetadadosDto(
            PeriodosSuportados,
            TiposRelatorioDisponiveis,
            AgrupamentosSuportados,
            FiltrosDisponiveis,
            FormatosExportacaoPlanejados,
            PermissoesRelevantes);
    }

    public async Task<RelatorioChamadosResumoDto> ObterResumoChamadosAsync(
        FiltroRelatorioChamadosRequest request,
        CancellationToken cancellationToken = default)
    {
        await GarantirAcessoAdminAsync(cancellationToken);

        var periodo = NormalizarPeriodo(request);
        var queryChamados = CriarQueryChamados(request, periodo);

        var totalChamados = await queryChamados.CountAsync(cancellationToken);

        var totaisPorStatus = await queryChamados
            .GroupBy(x => x.Status.Codigo)
            .Select(grupo => new { Status = grupo.Key, Total = grupo.Count() })
            .ToListAsync(cancellationToken);

        var totalAbertos = totaisPorStatus.FirstOrDefault(x => x.Status == StatusChamadoEnum.Aberto)?.Total ?? 0;
        var totalEmAtendimento = totaisPorStatus.FirstOrDefault(x => x.Status == StatusChamadoEnum.EmAtendimento)?.Total ?? 0;
        var totalEncerradosOuConcluidos = totaisPorStatus
            .Where(x => x.Status is StatusChamadoEnum.Encerrado or StatusChamadoEnum.Resolvido)
            .Sum(x => x.Total);
        var totalCancelados = totaisPorStatus.FirstOrDefault(x => x.Status == StatusChamadoEnum.Cancelado)?.Total ?? 0;

        var chamadosFiltradosIds = queryChamados.Select(x => x.Id);

        var totalReabertos = await historicoChamadoRepository.Query()
            .AsNoTracking()
            .Where(x => x.Tipo == TipoHistoricoChamado.Reaberto && chamadosFiltradosIds.Contains(x.ChamadoId))
            .Select(x => x.ChamadoId)
            .Distinct()
            .CountAsync(cancellationToken);

        var totalComAprovacaoPendente = await aprovacaoChamadoRepository.Query()
            .AsNoTracking()
            .Where(x => x.Status == StatusAprovacaoChamado.Pendente && chamadosFiltradosIds.Contains(x.ChamadoId))
            .Select(x => x.ChamadoId)
            .Distinct()
            .CountAsync(cancellationToken);

        var totalReprovadosNaAprovacao = await aprovacaoChamadoRepository.Query()
            .AsNoTracking()
            .Where(x => x.Status == StatusAprovacaoChamado.Reprovado && chamadosFiltradosIds.Contains(x.ChamadoId))
            .Select(x => x.ChamadoId)
            .Distinct()
            .CountAsync(cancellationToken);

        var totalComAtivoVinculado = await queryChamados.CountAsync(x => x.InventarioAtivoId.HasValue, cancellationToken);

        var tempoMedioAtendimentoHoras = await CalcularTempoMedioAtendimentoAsync(queryChamados, cancellationToken);
        var tempoMedioAtePrimeiraAcaoHoras = await CalcularTempoMedioPrimeiraAcaoAsync(queryChamados, cancellationToken);

        var totalPorPrioridadeBruto = await queryChamados
            .GroupBy(x => new { x.PrioridadeId, x.Prioridade.Nome })
            .Select(grupo => new { grupo.Key.PrioridadeId, grupo.Key.Nome, Quantidade = grupo.Count() })
            .ToListAsync(cancellationToken);

        var totalPorPrioridade = totalPorPrioridadeBruto
            .Select(item => new IndicadorRelatorioDto(
                item.PrioridadeId.ToString(),
                item.Nome,
                item.Quantidade,
                CalcularPercentual(item.Quantidade, totalChamados)))
            .OrderByDescending(x => x.Quantidade)
            .ThenBy(x => x.Nome)
            .ToArray();

        var totalPorDepartamentoBruto = await queryChamados
            .GroupBy(x => new { x.DepartamentoId, Nome = x.Departamento != null ? x.Departamento.Nome : "Sem departamento" })
            .Select(grupo => new { grupo.Key.DepartamentoId, grupo.Key.Nome, Quantidade = grupo.Count() })
            .ToListAsync(cancellationToken);

        var totalPorDepartamento = totalPorDepartamentoBruto
            .Select(item => new IndicadorRelatorioDto(
                item.DepartamentoId.HasValue ? item.DepartamentoId.Value.ToString() : "sem-departamento",
                item.Nome,
                item.Quantidade,
                CalcularPercentual(item.Quantidade, totalChamados)))
            .OrderByDescending(x => x.Quantidade)
            .ThenBy(x => x.Nome)
            .ToArray();

        var totalPorNaturezaBruto = await queryChamados
            .GroupBy(x => x.NaturezaChamado)
            .Select(grupo => new { Natureza = grupo.Key, Quantidade = grupo.Count() })
            .ToListAsync(cancellationToken);

        var totaisPorNaturezaLookup = totalPorNaturezaBruto
            .ToDictionary(x => x.Natureza, x => x.Quantidade);

        var totalPorNatureza = NaturezasOrdenadas
            .Select(item =>
            {
                var quantidade = totaisPorNaturezaLookup.GetValueOrDefault(item.Codigo, 0);
                return new IndicadorRelatorioDto(
                    ((int)item.Codigo).ToString(),
                    item.Nome,
                    quantidade,
                    CalcularPercentual(quantidade, totalChamados));
            })
            .ToArray();

        var totalPorCategoriaBruto = await queryChamados
            .GroupBy(x => new { x.CategoriaId, x.Categoria.Nome })
            .Select(grupo => new { grupo.Key.CategoriaId, grupo.Key.Nome, Quantidade = grupo.Count() })
            .ToListAsync(cancellationToken);

        var totalPorCategoria = totalPorCategoriaBruto
            .Select(item => new IndicadorRelatorioDto(
                item.CategoriaId.ToString(),
                item.Nome,
                item.Quantidade,
                CalcularPercentual(item.Quantidade, totalChamados)))
            .OrderByDescending(x => x.Quantidade)
            .ThenBy(x => x.Nome)
            .ToArray();

        return new RelatorioChamadosResumoDto(
            totalChamados,
            totalAbertos,
            totalEmAtendimento,
            totalEncerradosOuConcluidos,
            totalCancelados,
            totalReabertos,
            totalComAprovacaoPendente,
            totalReprovadosNaAprovacao,
            totalComAtivoVinculado,
            tempoMedioAtendimentoHoras,
            tempoMedioAtePrimeiraAcaoHoras,
            totalPorPrioridade,
            totalPorDepartamento,
            totalPorNatureza,
            totalPorCategoria);
    }

    public async Task<RelatorioChamadosSerieTemporalDto> ObterSerieTemporalChamadosAsync(
        FiltroRelatorioChamadosRequest request,
        CancellationToken cancellationToken = default)
    {
        await GarantirAcessoAdminAsync(cancellationToken);

        var periodo = NormalizarPeriodo(request);
        var agrupamento = NormalizarAgrupamentoTemporal(request.Agrupamento);
        var queryChamados = CriarQueryChamados(request, periodo);
        var chamadosFiltradosIds = queryChamados.Select(x => x.Id);

        var abertosPorDia = await queryChamados
            .GroupBy(x => x.AbertoEm.Date)
            .Select(grupo => new TotalDia(grupo.Key, grupo.Count()))
            .ToListAsync(cancellationToken);

        var encerradosPorDia = await queryChamados
            .Where(x => x.EncerradoEm.HasValue)
            .GroupBy(x => x.EncerradoEm!.Value.Date)
            .Select(grupo => new TotalDia(grupo.Key, grupo.Count()))
            .ToListAsync(cancellationToken);

        var reabertosPorDia = await historicoChamadoRepository.Query()
            .AsNoTracking()
            .Where(x => x.Tipo == TipoHistoricoChamado.Reaberto && chamadosFiltradosIds.Contains(x.ChamadoId))
            .GroupBy(x => x.CriadoEm.Date)
            .Select(grupo => new TotalDia(grupo.Key, grupo.Count()))
            .ToListAsync(cancellationToken);

        var consolidadoPorDia = new Dictionary<DateTime, SerieTemporalValores>();

        foreach (var item in abertosPorDia)
        {
            if (!consolidadoPorDia.TryGetValue(item.Data, out var valores))
            {
                valores = new SerieTemporalValores();
                consolidadoPorDia[item.Data] = valores;
            }

            valores.Abertos += item.Total;
        }

        foreach (var item in encerradosPorDia)
        {
            if (!consolidadoPorDia.TryGetValue(item.Data, out var valores))
            {
                valores = new SerieTemporalValores();
                consolidadoPorDia[item.Data] = valores;
            }

            valores.Encerrados += item.Total;
        }

        foreach (var item in reabertosPorDia)
        {
            if (!consolidadoPorDia.TryGetValue(item.Data, out var valores))
            {
                valores = new SerieTemporalValores();
                consolidadoPorDia[item.Data] = valores;
            }

            valores.Reabertos += item.Total;
        }

        var itens = consolidadoPorDia
            .GroupBy(x => ObterChaveAgrupamento(x.Key, agrupamento))
            .OrderBy(grupo => grupo.Key)
            .Select(grupo => new SerieTemporalRelatorioDto(
                FormatarPeriodo(grupo.Key, agrupamento),
                grupo.Sum(x => x.Value.Abertos),
                grupo.Sum(x => x.Value.Encerrados),
                grupo.Sum(x => x.Value.Reabertos)))
            .ToArray();

        return new RelatorioChamadosSerieTemporalDto(agrupamento, itens);
    }

    public async Task<RelatorioChamadosDistribuicaoDto> ObterDistribuicaoChamadosAsync(
        FiltroRelatorioChamadosRequest request,
        CancellationToken cancellationToken = default)
    {
        await GarantirAcessoAdminAsync(cancellationToken);

        var periodo = NormalizarPeriodo(request);
        var queryChamados = CriarQueryChamados(request, periodo);
        var totalChamados = await queryChamados.CountAsync(cancellationToken);

        var itens = request.AgruparPor switch
        {
            AgruparPorRelatorioChamados.Status => MapDistribuicao(
                (await queryChamados
                    .GroupBy(x => new { x.StatusId, x.Status.Nome })
                    .Select(grupo => new { Chave = grupo.Key.StatusId, grupo.Key.Nome, Quantidade = grupo.Count() })
                    .ToListAsync(cancellationToken))
                    .Select(item => new DistribuicaoBase(item.Chave.ToString(), item.Nome, item.Quantidade)),
                totalChamados),
            AgruparPorRelatorioChamados.Prioridade => MapDistribuicao(
                (await queryChamados
                    .GroupBy(x => new { x.PrioridadeId, x.Prioridade.Nome })
                    .Select(grupo => new { Chave = grupo.Key.PrioridadeId, grupo.Key.Nome, Quantidade = grupo.Count() })
                    .ToListAsync(cancellationToken))
                    .Select(item => new DistribuicaoBase(item.Chave.ToString(), item.Nome, item.Quantidade)),
                totalChamados),
            AgruparPorRelatorioChamados.Departamento => MapDistribuicao(
                (await queryChamados
                    .GroupBy(x => new { x.DepartamentoId, Nome = x.Departamento != null ? x.Departamento.Nome : "Sem departamento" })
                    .Select(grupo => new { grupo.Key.DepartamentoId, grupo.Key.Nome, Quantidade = grupo.Count() })
                    .ToListAsync(cancellationToken))
                    .Select(item => new DistribuicaoBase(
                        item.DepartamentoId.HasValue ? item.DepartamentoId.Value.ToString() : "sem-departamento",
                        item.Nome,
                        item.Quantidade)),
                totalChamados),
            AgruparPorRelatorioChamados.Categoria => MapDistribuicao(
                (await queryChamados
                    .GroupBy(x => new { x.CategoriaId, x.Categoria.Nome })
                    .Select(grupo => new { Chave = grupo.Key.CategoriaId, grupo.Key.Nome, Quantidade = grupo.Count() })
                    .ToListAsync(cancellationToken))
                    .Select(item => new DistribuicaoBase(item.Chave.ToString(), item.Nome, item.Quantidade)),
                totalChamados),
            AgruparPorRelatorioChamados.CatalogoServico => MapDistribuicao(
                (await queryChamados
                    .GroupBy(x => new { x.CatalogoServicoId, Nome = x.CatalogoServico != null ? x.CatalogoServico.Nome : "Sem catalogo" })
                    .Select(grupo => new { grupo.Key.CatalogoServicoId, grupo.Key.Nome, Quantidade = grupo.Count() })
                    .ToListAsync(cancellationToken))
                    .Select(item => new DistribuicaoBase(
                        item.CatalogoServicoId.HasValue ? item.CatalogoServicoId.Value.ToString() : "sem-catalogo",
                        item.Nome,
                        item.Quantidade)),
                totalChamados),
            AgruparPorRelatorioChamados.Atendente => MapDistribuicao(
                (await queryChamados
                    .GroupBy(x => new { x.ResponsavelId, Nome = x.Responsavel != null ? x.Responsavel.Nome : "Sem atendente" })
                    .Select(grupo => new { grupo.Key.ResponsavelId, grupo.Key.Nome, Quantidade = grupo.Count() })
                    .ToListAsync(cancellationToken))
                    .Select(item => new DistribuicaoBase(
                        item.ResponsavelId.HasValue ? item.ResponsavelId.Value.ToString() : "sem-atendente",
                        item.Nome,
                        item.Quantidade)),
                totalChamados),
            AgruparPorRelatorioChamados.Solicitante => MapDistribuicao(
                (await queryChamados
                    .GroupBy(x => new { x.SolicitanteId, x.Solicitante.Nome })
                    .Select(grupo => new { Chave = grupo.Key.SolicitanteId, Nome = grupo.Key.Nome, Quantidade = grupo.Count() })
                    .ToListAsync(cancellationToken))
                    .Select(item => new DistribuicaoBase(item.Chave.ToString(), item.Nome, item.Quantidade)),
                totalChamados),
            AgruparPorRelatorioChamados.AtivoVinculado => MapDistribuicao(
                (await queryChamados
                    .GroupBy(x => x.InventarioAtivoId.HasValue)
                    .Select(grupo => new { ComAtivo = grupo.Key, Quantidade = grupo.Count() })
                    .ToListAsync(cancellationToken))
                    .Select(item => new DistribuicaoBase(
                        item.ComAtivo ? "com-ativo-vinculado" : "sem-ativo-vinculado",
                        item.ComAtivo ? "Com ativo vinculado" : "Sem ativo vinculado",
                        item.Quantidade)),
                totalChamados),
            AgruparPorRelatorioChamados.Natureza => MapDistribuicao(
                NaturezasOrdenadas
                    .GroupJoin(
                        await queryChamados
                            .GroupBy(x => x.NaturezaChamado)
                            .Select(grupo => new { Natureza = grupo.Key, Quantidade = grupo.Count() })
                            .ToListAsync(cancellationToken),
                        natureza => natureza.Codigo,
                        total => total.Natureza,
                        (natureza, total) => new DistribuicaoBase(
                            ((int)natureza.Codigo).ToString(),
                            natureza.Nome,
                            total.FirstOrDefault()?.Quantidade ?? 0)),
                totalChamados),
            _ => throw new ArgumentException("Agrupamento de distribuicao invalido.", nameof(request.AgruparPor))
        };

        return new RelatorioChamadosDistribuicaoDto(request.AgruparPor, itens);
    }

    public async Task<RelatorioAtendimentoProdutividadeDto> ObterProdutividadeAtendimentoAsync(
        FiltroRelatorioAtendimentoRequest request,
        CancellationToken cancellationToken = default)
    {
        await GarantirAcessoAdminAsync(cancellationToken);

        var periodo = NormalizarPeriodo(request);
        var limiteRanking = NormalizarLimiteRanking(request.LimiteRanking);

        var queryChamados = CriarQueryChamadosAtendimento(request, periodo)
            .Where(x => x.ResponsavelId.HasValue);

        var totaisPorAtendente = await queryChamados
            .GroupBy(x => new
            {
                Id = x.ResponsavelId!.Value,
                Nome = x.Responsavel != null ? x.Responsavel.Nome : "Atendente removido"
            })
            .Select(grupo => new ProducaoAtendenteBase(
                grupo.Key.Id,
                grupo.Key.Nome,
                grupo.Count(),
                grupo.Count(x => x.Status.Codigo == StatusChamadoEnum.Encerrado || x.Status.Codigo == StatusChamadoEnum.Resolvido),
                grupo.Count(x => x.Status.Codigo == StatusChamadoEnum.Aberto || x.Status.Codigo == StatusChamadoEnum.EmAtendimento || x.Status.Codigo == StatusChamadoEnum.AguardandoSolicitante)))
            .ToListAsync(cancellationToken);

        var chamadosComResponsavel = queryChamados.Select(x => new { x.Id, ResponsavelId = x.ResponsavelId!.Value });

        var reabertosPorAtendente = await historicoChamadoRepository.Query()
            .AsNoTracking()
            .Where(x => x.Tipo == TipoHistoricoChamado.Reaberto)
            .Join(chamadosComResponsavel, historico => historico.ChamadoId, chamado => chamado.Id, (historico, chamado) => new
            {
                chamado.ResponsavelId,
                chamado.Id
            })
            .Distinct()
            .GroupBy(x => x.ResponsavelId)
            .Select(grupo => new { AtendenteId = grupo.Key, Quantidade = grupo.Count() })
            .ToListAsync(cancellationToken);

        var mapaReabertos = reabertosPorAtendente.ToDictionary(x => x.AtendenteId, x => x.Quantidade);

        var temposConclusao = await queryChamados
            .Where(x => x.EncerradoEm.HasValue)
            .Select(x => new TempoConclusaoBase(x.ResponsavelId!.Value, x.AbertoEm, x.EncerradoEm!.Value))
            .ToListAsync(cancellationToken);

        var mapaTempoMedio = temposConclusao
            .GroupBy(x => x.AtendenteId)
            .ToDictionary(
                grupo => grupo.Key,
                grupo =>
                {
                    var horas = grupo
                        .Select(item => (item.EncerradoEm - item.AbertoEm).TotalHours)
                        .Where(valor => valor >= 0)
                        .ToArray();

                    if (horas.Length == 0)
                    {
                        return (double?)null;
                    }

                    return Math.Round(horas.Average(), 2);
                });

        var ranking = totaisPorAtendente
            .Select(item =>
            {
                var chamadosReabertos = mapaReabertos.GetValueOrDefault(item.AtendenteId);
                var tempoMedioConclusao = mapaTempoMedio.GetValueOrDefault(item.AtendenteId);
                var percentualConclusao = item.ChamadosAssumidos == 0
                    ? 0
                    : Math.Round((decimal)item.ChamadosConcluidos * 100 / item.ChamadosAssumidos, 2);

                return new RankingAtendimentoDto(
                    item.AtendenteId,
                    item.AtendenteNome,
                    item.ChamadosAssumidos,
                    item.ChamadosConcluidos,
                    item.ChamadosEmAberto,
                    chamadosReabertos,
                    tempoMedioConclusao,
                    percentualConclusao);
            })
            .OrderByDescending(x => x.ChamadosAssumidos)
            .ThenByDescending(x => x.ChamadosConcluidos)
            .ThenBy(x => x.AtendenteNome)
            .Take(limiteRanking)
            .ToArray();

        return new RelatorioAtendimentoProdutividadeDto(limiteRanking, ranking);
    }

    public async Task<RelatorioSlaResumoDto> ObterResumoSlaAsync(
        FiltroRelatorioSlaRequest request,
        CancellationToken cancellationToken = default)
    {
        await GarantirAcessoAdminAsync(cancellationToken);

        var periodo = NormalizarPeriodo(request);
        var queryChamados = CriarQueryChamadosSla(request, periodo);

        var totalChamadosComSla = await queryChamados.CountAsync(x => x.ChamadoSla != null, cancellationToken);
        var totalDentroSla = await queryChamados.CountAsync(
            x => x.ChamadoSla != null && x.ChamadoSla.ResolucaoCumprida == true,
            cancellationToken);
        var totalForaSla = await queryChamados.CountAsync(
            x => x.ChamadoSla != null && (x.ChamadoSla.ResolucaoCumprida == false || x.ChamadoSla.ResolucaoViolada),
            cancellationToken);
        var totalSemSla = await queryChamados.CountAsync(x => x.ChamadoSla == null, cancellationToken);
        var chamadosComSlaPausado = await queryChamados.CountAsync(x => x.ChamadoSla != null && x.ChamadoSla.Pausado, cancellationToken);

        var minutosResolucao = await queryChamados
            .Where(x => x.ChamadoSla != null && x.ChamadoSla.MinutosResolucao.HasValue)
            .Select(x => x.ChamadoSla!.MinutosResolucao!.Value)
            .ToListAsync(cancellationToken);

        double? tempoMedioResolucaoHoras = null;
        if (minutosResolucao.Count > 0)
        {
            tempoMedioResolucaoHoras = Math.Round(minutosResolucao.Where(x => x >= 0).Average() / 60d, 2);
        }

        return new RelatorioSlaResumoDto(
            totalChamadosComSla,
            totalDentroSla,
            totalForaSla,
            CalcularPercentual(totalDentroSla, totalChamadosComSla),
            CalcularPercentual(totalForaSla, totalChamadosComSla),
            tempoMedioResolucaoHoras,
            ChamadosProximosVencimento: null,
            chamadosComSlaPausado,
            totalSemSla);
    }

    public async Task<IReadOnlyCollection<RelatorioSlaViolacaoDto>> ObterViolacoesSlaAsync(
        FiltroRelatorioSlaRequest request,
        CancellationToken cancellationToken = default)
    {
        await GarantirAcessoAdminAsync(cancellationToken);

        var periodo = NormalizarPeriodo(request);
        var limite = NormalizarLimiteRanking(request.LimiteRanking);
        var agora = DateTime.UtcNow;

        var itens = await CriarQueryChamadosSla(request, periodo)
            .Where(x => x.ChamadoSla != null && (x.ChamadoSla.ResolucaoCumprida == false || x.ChamadoSla.ResolucaoViolada))
            .Select(x => new
            {
                x.Id,
                x.Codigo,
                x.Titulo,
                x.NaturezaChamado,
                x.ImpactoChamado,
                x.UrgenciaChamado,
                Departamento = x.Departamento != null ? x.Departamento.Nome : "Sem departamento",
                Prioridade = x.Prioridade.Nome,
                Status = x.Status.Nome,
                DataAbertura = x.AbertoEm,
                DataLimiteSla = (DateTime?)x.ChamadoSla!.PrazoResolucao,
                DataConclusao = x.EncerradoEm,
                MinutosResolucao = x.ChamadoSla!.MinutosResolucao
            })
            .OrderBy(x => x.DataLimiteSla)
            .Take(limite)
            .ToListAsync(cancellationToken);

        return itens
            .Select(item =>
            {
                double? horasExcedidas = null;
                if (item.DataLimiteSla.HasValue)
                {
                    if (item.MinutosResolucao.HasValue && item.MinutosResolucao.Value >= 0 && item.DataConclusao.HasValue)
                    {
                        horasExcedidas = Math.Round(Math.Max(0, (item.DataConclusao.Value - item.DataLimiteSla.Value).TotalHours), 2);
                    }
                    else
                    {
                        horasExcedidas = Math.Round(Math.Max(0, (agora - item.DataLimiteSla.Value).TotalHours), 2);
                    }
                }

                return new RelatorioSlaViolacaoDto(
                    item.Id,
                    item.Codigo,
                    item.Titulo,
                    item.NaturezaChamado,
                    item.Departamento,
                    item.Prioridade,
                    item.Status,
                    item.DataAbertura,
                    item.DataLimiteSla,
                    item.DataConclusao,
                    horasExcedidas,
                    item.ImpactoChamado,
                    item.UrgenciaChamado);
            })
            .OrderByDescending(x => x.HorasExcedidas ?? 0)
            .ThenBy(x => x.DataLimiteSla)
            .ToArray();
    }

    public async Task<IReadOnlyCollection<RelatorioSlaPorDepartamentoDto>> ObterSlaPorDepartamentoAsync(
        FiltroRelatorioSlaRequest request,
        CancellationToken cancellationToken = default)
    {
        await GarantirAcessoAdminAsync(cancellationToken);

        var periodo = NormalizarPeriodo(request);
        var itens = await CriarQueryChamadosSla(request, periodo)
            .Where(x => x.ChamadoSla != null)
            .GroupBy(x => new
            {
                x.DepartamentoId,
                Nome = x.Departamento != null ? x.Departamento.Nome : "Sem departamento"
            })
            .Select(grupo => new
            {
                grupo.Key.DepartamentoId,
                grupo.Key.Nome,
                TotalComSla = grupo.Count(),
                DentroSla = grupo.Count(x => x.ChamadoSla!.ResolucaoCumprida == true),
                ForaSla = grupo.Count(x => x.ChamadoSla!.ResolucaoCumprida == false || x.ChamadoSla!.ResolucaoViolada)
            })
            .ToListAsync(cancellationToken);

        return itens
            .Select(item => new RelatorioSlaPorDepartamentoDto(
                item.DepartamentoId,
                item.Nome,
                item.TotalComSla,
                item.DentroSla,
                item.ForaSla,
                CalcularPercentual(item.DentroSla, item.TotalComSla)))
            .OrderByDescending(x => x.TotalComSla)
            .ThenBy(x => x.DepartamentoNome)
            .ToArray();
    }

    public async Task<IReadOnlyCollection<RelatorioSlaPorPrioridadeDto>> ObterSlaPorPrioridadeAsync(
        FiltroRelatorioSlaRequest request,
        CancellationToken cancellationToken = default)
    {
        await GarantirAcessoAdminAsync(cancellationToken);

        var periodo = NormalizarPeriodo(request);
        var itens = await CriarQueryChamadosSla(request, periodo)
            .Where(x => x.ChamadoSla != null)
            .GroupBy(x => new { x.PrioridadeId, x.Prioridade.Nome })
            .Select(grupo => new
            {
                grupo.Key.PrioridadeId,
                grupo.Key.Nome,
                TotalComSla = grupo.Count(),
                DentroSla = grupo.Count(x => x.ChamadoSla!.ResolucaoCumprida == true),
                ForaSla = grupo.Count(x => x.ChamadoSla!.ResolucaoCumprida == false || x.ChamadoSla!.ResolucaoViolada)
            })
            .ToListAsync(cancellationToken);

        return itens
            .Select(item => new RelatorioSlaPorPrioridadeDto(
                item.PrioridadeId,
                item.Nome,
                item.TotalComSla,
                item.DentroSla,
                item.ForaSla,
                CalcularPercentual(item.DentroSla, item.TotalComSla)))
            .OrderByDescending(x => x.TotalComSla)
            .ThenBy(x => x.PrioridadeNome)
            .ToArray();
    }

    public async Task<RelatorioAprovacoesResumoDto> ObterResumoAprovacoesAsync(
        FiltroRelatorioAprovacoesRequest request,
        CancellationToken cancellationToken = default)
    {
        await GarantirAcessoAdminAsync(cancellationToken);

        var periodo = NormalizarPeriodo(request);
        var queryAprovacoes = CriarQueryAprovacoes(request, periodo);

        var totalAprovacoes = await queryAprovacoes.CountAsync(cancellationToken);
        var totaisPorStatus = await queryAprovacoes
            .GroupBy(x => x.Status)
            .Select(grupo => new { Status = grupo.Key, Total = grupo.Count() })
            .ToListAsync(cancellationToken);

        var pendentes = totaisPorStatus.FirstOrDefault(x => x.Status == StatusAprovacaoChamado.Pendente)?.Total ?? 0;
        var aprovadas = totaisPorStatus.FirstOrDefault(x => x.Status == StatusAprovacaoChamado.Aprovado)?.Total ?? 0;
        var reprovadas = totaisPorStatus.FirstOrDefault(x => x.Status == StatusAprovacaoChamado.Reprovado)?.Total ?? 0;
        var canceladas = totaisPorStatus.FirstOrDefault(x => x.Status == StatusAprovacaoChamado.Cancelado)?.Total ?? 0;

        var temposDecisao = await queryAprovacoes
            .Where(x => x.DecididaEm.HasValue)
            .Select(x => new { x.SolicitadaEm, DecididaEm = x.DecididaEm!.Value })
            .ToListAsync(cancellationToken);

        double? tempoMedioDecisaoHoras = null;
        var horasDecisao = temposDecisao
            .Select(x => (x.DecididaEm - x.SolicitadaEm).TotalHours)
            .Where(x => x >= 0)
            .ToArray();
        if (horasDecisao.Length > 0)
        {
            tempoMedioDecisaoHoras = Math.Round(horasDecisao.Average(), 2);
        }

        return new RelatorioAprovacoesResumoDto(
            totalAprovacoes,
            pendentes,
            aprovadas,
            reprovadas,
            canceladas,
            CalcularPercentual(aprovadas, totalAprovacoes),
            CalcularPercentual(reprovadas, totalAprovacoes),
            tempoMedioDecisaoHoras);
    }

    public async Task<IReadOnlyCollection<RelatorioAprovacoesTempoMedioDto>> ObterTempoMedioAprovacoesAsync(
        FiltroRelatorioAprovacoesRequest request,
        CancellationToken cancellationToken = default)
    {
        await GarantirAcessoAdminAsync(cancellationToken);

        var periodo = NormalizarPeriodo(request);
        var queryAprovacoes = CriarQueryAprovacoes(request, periodo)
            .Where(x => x.DecididaEm.HasValue);

        var itens = await queryAprovacoes
            .Select(x => new
            {
                x.TipoOrigem,
                x.SolicitadaEm,
                DecididaEm = x.DecididaEm!.Value,
                x.Chamado.DepartamentoId,
                Departamento = x.Chamado.Departamento != null ? x.Chamado.Departamento.Nome : "Sem departamento"
            })
            .ToListAsync(cancellationToken);

        return request.AgruparPor switch
        {
            AgruparTempoMedioAprovacoesPor.Departamento => itens
                .GroupBy(x => x.Departamento)
                .Select(grupo =>
                {
                    var horas = grupo
                        .Select(x => (x.DecididaEm - x.SolicitadaEm).TotalHours)
                        .Where(x => x >= 0)
                        .ToArray();

                    return new RelatorioAprovacoesTempoMedioDto(
                        grupo.Key,
                        grupo.Count(),
                        horas.Length == 0 ? null : Math.Round(horas.Average(), 2));
                })
                .OrderByDescending(x => x.TotalDecididas)
                .ThenBy(x => x.Grupo)
                .ToArray(),
            AgruparTempoMedioAprovacoesPor.Periodo => itens
                .GroupBy(x =>
                {
                    var agrupamento = request.Agrupamento ?? AgrupamentoRelatorio.Mes;
                    var referencia = agrupamento == AgrupamentoRelatorio.Dia
                        ? x.DecididaEm.Date
                        : new DateTime(x.DecididaEm.Year, x.DecididaEm.Month, 1);
                    return FormatarPeriodo(referencia, agrupamento);
                })
                .Select(grupo =>
                {
                    var horas = grupo
                        .Select(x => (x.DecididaEm - x.SolicitadaEm).TotalHours)
                        .Where(x => x >= 0)
                        .ToArray();

                    return new RelatorioAprovacoesTempoMedioDto(
                        grupo.Key,
                        grupo.Count(),
                        horas.Length == 0 ? null : Math.Round(horas.Average(), 2));
                })
                .OrderBy(x => x.Grupo)
                .ToArray(),
            _ => itens
                .GroupBy(x => x.TipoOrigem.ToString())
                .Select(grupo =>
                {
                    var horas = grupo
                        .Select(x => (x.DecididaEm - x.SolicitadaEm).TotalHours)
                        .Where(x => x >= 0)
                        .ToArray();

                    return new RelatorioAprovacoesTempoMedioDto(
                        grupo.Key,
                        grupo.Count(),
                        horas.Length == 0 ? null : Math.Round(horas.Average(), 2));
                })
                .OrderByDescending(x => x.TotalDecididas)
                .ThenBy(x => x.Grupo)
                .ToArray()
        };
    }

    public async Task<IReadOnlyCollection<RelatorioAprovacoesPorOrigemDto>> ObterAprovacoesPorOrigemAsync(
        FiltroRelatorioAprovacoesRequest request,
        CancellationToken cancellationToken = default)
    {
        await GarantirAcessoAdminAsync(cancellationToken);

        var periodo = NormalizarPeriodo(request);
        var itens = await CriarQueryAprovacoes(request, periodo)
            .Select(x => new
            {
                x.TipoOrigem,
                x.Status,
                x.SolicitadaEm,
                x.DecididaEm
            })
            .ToListAsync(cancellationToken);

        return itens
            .GroupBy(x => x.TipoOrigem)
            .Select(grupo =>
            {
                var horas = grupo
                    .Where(x => x.DecididaEm.HasValue)
                    .Select(x => (x.DecididaEm!.Value - x.SolicitadaEm).TotalHours)
                    .Where(x => x >= 0)
                    .ToArray();

                return new RelatorioAprovacoesPorOrigemDto(
                    grupo.Key.ToString(),
                    grupo.Count(),
                    grupo.Count(x => x.Status == StatusAprovacaoChamado.Pendente),
                    grupo.Count(x => x.Status == StatusAprovacaoChamado.Aprovado),
                    grupo.Count(x => x.Status == StatusAprovacaoChamado.Reprovado),
                    grupo.Count(x => x.Status == StatusAprovacaoChamado.Cancelado),
                    horas.Length == 0 ? null : Math.Round(horas.Average(), 2));
            })
            .OrderByDescending(x => x.Total)
            .ThenBy(x => x.TipoOrigem)
            .ToArray();
    }

    public async Task<RelatorioCatalogoServicosResumoDto> ObterResumoCatalogoServicosAsync(
        FiltroRelatorioCatalogoServicosRequest request,
        CancellationToken cancellationToken = default)
    {
        await GarantirAcessoAdminAsync(cancellationToken);

        var periodo = NormalizarPeriodo(request);
        var queryCatalogo = CriarQueryCatalogoServicos(request);
        var queryChamados = CriarQueryChamadosCatalogo(request, periodo);

        var totalServicos = await queryCatalogo.CountAsync(cancellationToken);
        var servicosPublicados = await queryCatalogo.CountAsync(x => x.Status == StatusCatalogoServico.Publicado, cancellationToken);
        var servicosArquivados = await queryCatalogo.CountAsync(x => x.Status == StatusCatalogoServico.Arquivado, cancellationToken);
        var servicosAtivos = await queryCatalogo.CountAsync(x => x.Ativo, cancellationToken);
        var servicosQuePermitemAbertura = await queryCatalogo.CountAsync(x => x.PermiteAberturaChamado, cancellationToken);
        var servicosQueRequeremAprovacao = await queryCatalogo.CountAsync(x => x.RequerAprovacao, cancellationToken);

        var totalChamadosPeriodo = await queryChamados.CountAsync(cancellationToken);
        var chamadosAbertosPorCatalogo = await queryChamados.CountAsync(x => x.CatalogoServicoId.HasValue, cancellationToken);

        return new RelatorioCatalogoServicosResumoDto(
            totalServicos,
            servicosPublicados,
            servicosArquivados,
            servicosAtivos,
            servicosQuePermitemAbertura,
            servicosQueRequeremAprovacao,
            chamadosAbertosPorCatalogo,
            CalcularPercentual(chamadosAbertosPorCatalogo, totalChamadosPeriodo));
    }

    public async Task<IReadOnlyCollection<RelatorioCatalogoServicosMaisSolicitadosDto>> ObterCatalogoServicosMaisSolicitadosAsync(
        FiltroRelatorioCatalogoServicosRequest request,
        CancellationToken cancellationToken = default)
    {
        await GarantirAcessoAdminAsync(cancellationToken);

        var periodo = NormalizarPeriodo(request);
        var limite = NormalizarLimiteRanking(request.LimiteRanking);

        var chamados = await CriarQueryChamadosCatalogo(request, periodo)
            .Where(x => x.CatalogoServicoId.HasValue)
            .Select(x => new
            {
                x.Id,
                CatalogoServicoId = x.CatalogoServicoId!.Value,
                NomeServico = x.CatalogoServico != null ? x.CatalogoServico.Nome : "Servico removido",
                DepartamentoResponsavel = x.CatalogoServico != null ? x.CatalogoServico.DepartamentoResponsavel.Nome : "Sem departamento"
            })
            .ToListAsync(cancellationToken);

        var chamadoIds = chamados.Select(x => x.Id).Distinct().ToArray();
        var aprovacoes = await aprovacaoChamadoRepository.Query()
            .AsNoTracking()
            .Where(x => chamadoIds.Contains(x.ChamadoId))
            .Select(x => new { x.ChamadoId, x.Status })
            .ToListAsync(cancellationToken);
        var chamadosForaSla = await chamadoRepository.Query()
            .AsNoTracking()
            .Where(x => chamadoIds.Contains(x.Id) && x.ChamadoSla != null && (x.ChamadoSla.ResolucaoCumprida == false || x.ChamadoSla.ResolucaoViolada))
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        var chamadosComAprovacao = aprovacoes
            .Select(x => x.ChamadoId)
            .Distinct()
            .ToHashSet();
        var chamadosReprovados = aprovacoes
            .Where(x => x.Status == StatusAprovacaoChamado.Reprovado)
            .Select(x => x.ChamadoId)
            .Distinct()
            .ToHashSet();
        var chamadosForaSlaSet = chamadosForaSla.ToHashSet();

        return chamados
            .GroupBy(x => new { x.CatalogoServicoId, x.NomeServico, x.DepartamentoResponsavel })
            .Select(grupo => new RelatorioCatalogoServicosMaisSolicitadosDto(
                grupo.Key.CatalogoServicoId,
                grupo.Key.NomeServico,
                grupo.Key.DepartamentoResponsavel,
                grupo.Count(),
                grupo.Count(x => chamadosComAprovacao.Contains(x.Id)),
                grupo.Count(x => chamadosReprovados.Contains(x.Id)),
                grupo.Count(x => chamadosForaSlaSet.Contains(x.Id))))
            .OrderByDescending(x => x.TotalChamados)
            .ThenBy(x => x.NomeServico)
            .Take(limite)
            .ToArray();
    }

    public async Task<IReadOnlyCollection<RelatorioCatalogoServicosPorDepartamentoDto>> ObterCatalogoServicosPorDepartamentoAsync(
        FiltroRelatorioCatalogoServicosRequest request,
        CancellationToken cancellationToken = default)
    {
        await GarantirAcessoAdminAsync(cancellationToken);

        var periodo = NormalizarPeriodo(request);
        var queryCatalogo = CriarQueryCatalogoServicos(request);
        var queryChamados = CriarQueryChamadosCatalogo(request, periodo).Where(x => x.CatalogoServicoId.HasValue);

        var servicosPorDepartamento = await queryCatalogo
            .GroupBy(x => new { x.DepartamentoResponsavelId, x.DepartamentoResponsavel.Nome })
            .Select(grupo => new
            {
                grupo.Key.DepartamentoResponsavelId,
                grupo.Key.Nome,
                TotalServicos = grupo.Count(),
                ServicosPublicados = grupo.Count(x => x.Status == StatusCatalogoServico.Publicado),
                ServicosQueRequeremAprovacao = grupo.Count(x => x.RequerAprovacao)
            })
            .ToListAsync(cancellationToken);

        var chamadosPorDepartamento = await queryChamados
            .GroupBy(x => new { x.CatalogoServico!.DepartamentoResponsavelId, x.CatalogoServico.DepartamentoResponsavel.Nome })
            .Select(grupo => new
            {
                grupo.Key.DepartamentoResponsavelId,
                ChamadosAbertos = grupo.Count()
            })
            .ToListAsync(cancellationToken);

        var mapaChamados = chamadosPorDepartamento.ToDictionary(x => x.DepartamentoResponsavelId, x => x.ChamadosAbertos);

        return servicosPorDepartamento
            .Select(item => new RelatorioCatalogoServicosPorDepartamentoDto(
                item.DepartamentoResponsavelId,
                item.Nome,
                item.TotalServicos,
                item.ServicosPublicados,
                mapaChamados.GetValueOrDefault(item.DepartamentoResponsavelId),
                item.ServicosQueRequeremAprovacao))
            .OrderByDescending(x => x.ChamadosAbertos)
            .ThenBy(x => x.DepartamentoNome)
            .ToArray();
    }

    public async Task<RelatorioInventarioAtivosResumoDto> ObterResumoInventarioAtivosAsync(
        FiltroRelatorioInventarioAtivosRequest request,
        CancellationToken cancellationToken = default)
    {
        await GarantirAcessoAdminAsync(cancellationToken);

        var periodo = NormalizarPeriodo(request);
        var queryAtivos = CriarQueryInventarioAtivos(request, periodo);
        var ativosIds = queryAtivos.Select(x => x.Id);

        var totalAtivos = await queryAtivos.CountAsync(cancellationToken);
        var ativosAtivos = await queryAtivos.CountAsync(x => x.Ativo, cancellationToken);
        var ativosInativos = await queryAtivos.CountAsync(x => !x.Ativo, cancellationToken);
        var totalEmManutencao = await queryAtivos.CountAsync(x => x.StatusOperacional == StatusOperacionalAtivo.EmManutencao, cancellationToken);
        var totalComDefeito = await queryAtivos.CountAsync(x => x.StatusOperacional == StatusOperacionalAtivo.ComDefeito, cancellationToken);

        var totalPorTipoBruto = await queryAtivos
            .GroupBy(x => new { x.TipoAtivoInventarioId, x.TipoAtivoInventario.Nome })
            .Select(grupo => new { grupo.Key.TipoAtivoInventarioId, grupo.Key.Nome, Quantidade = grupo.Count() })
            .ToListAsync(cancellationToken);
        var totalPorTipo = totalPorTipoBruto
            .Select(item => new IndicadorRelatorioDto(
                item.TipoAtivoInventarioId.ToString(),
                item.Nome,
                item.Quantidade,
                CalcularPercentual(item.Quantidade, totalAtivos)))
            .OrderByDescending(x => x.Quantidade)
            .ThenBy(x => x.Nome)
            .ToArray();

        var totalPorCriticidadeBruto = await queryAtivos
            .GroupBy(x => x.Criticidade)
            .Select(grupo => new { Chave = grupo.Key, Quantidade = grupo.Count() })
            .ToListAsync(cancellationToken);
        var totalPorCriticidade = totalPorCriticidadeBruto
            .Select(item => new IndicadorRelatorioDto(
                ((int)item.Chave).ToString(),
                item.Chave.ToString(),
                item.Quantidade,
                CalcularPercentual(item.Quantidade, totalAtivos)))
            .OrderByDescending(x => x.Quantidade)
            .ThenBy(x => x.Nome)
            .ToArray();

        var totalPorStatusOperacionalBruto = await queryAtivos
            .GroupBy(x => x.StatusOperacional)
            .Select(grupo => new { Chave = grupo.Key, Quantidade = grupo.Count() })
            .ToListAsync(cancellationToken);
        var totalPorStatusOperacional = totalPorStatusOperacionalBruto
            .Select(item => new IndicadorRelatorioDto(
                ((int)item.Chave).ToString(),
                item.Chave.ToString(),
                item.Quantidade,
                CalcularPercentual(item.Quantidade, totalAtivos)))
            .OrderByDescending(x => x.Quantidade)
            .ThenBy(x => x.Nome)
            .ToArray();

        var totalPorStatusPatrimonialBruto = await queryAtivos
            .GroupBy(x => x.StatusPatrimonial)
            .Select(grupo => new { Chave = grupo.Key, Quantidade = grupo.Count() })
            .ToListAsync(cancellationToken);
        var totalPorStatusPatrimonial = totalPorStatusPatrimonialBruto
            .Select(item => new IndicadorRelatorioDto(
                ((int)item.Chave).ToString(),
                item.Chave.ToString(),
                item.Quantidade,
                CalcularPercentual(item.Quantidade, totalAtivos)))
            .OrderByDescending(x => x.Quantidade)
            .ThenBy(x => x.Nome)
            .ToArray();

        var totalComChamadosRelacionados = await chamadoRepository.Query()
            .AsNoTracking()
            .Where(x =>
                x.AbertoEm >= periodo.Inicio &&
                x.AbertoEm < periodo.FimExclusivo &&
                x.InventarioAtivoId.HasValue &&
                ativosIds.Contains(x.InventarioAtivoId.Value))
            .Select(x => x.InventarioAtivoId!.Value)
            .Distinct()
            .CountAsync(cancellationToken);

        return new RelatorioInventarioAtivosResumoDto(
            totalAtivos,
            ativosAtivos,
            ativosInativos,
            totalPorTipo,
            totalPorCriticidade,
            totalPorStatusOperacional,
            totalPorStatusPatrimonial,
            totalComChamadosRelacionados,
            totalEmManutencao,
            totalComDefeito);
    }

    public async Task<RelatorioInventarioAtivosPorStatusDto> ObterInventarioAtivosPorStatusAsync(
        FiltroRelatorioInventarioAtivosRequest request,
        CancellationToken cancellationToken = default)
    {
        await GarantirAcessoAdminAsync(cancellationToken);

        var periodo = NormalizarPeriodo(request);
        var queryAtivos = CriarQueryInventarioAtivos(request, periodo);
        var totalAtivos = await queryAtivos.CountAsync(cancellationToken);

        var porStatusOperacional = MapDistribuicao(
            (await queryAtivos
                .GroupBy(x => x.StatusOperacional)
                .Select(grupo => new { Chave = grupo.Key, Quantidade = grupo.Count() })
                .ToListAsync(cancellationToken))
                .Select(item => new DistribuicaoBase(((int)item.Chave).ToString(), item.Chave.ToString(), item.Quantidade)),
            totalAtivos);

        var porStatusPatrimonial = MapDistribuicao(
            (await queryAtivos
                .GroupBy(x => x.StatusPatrimonial)
                .Select(grupo => new { Chave = grupo.Key, Quantidade = grupo.Count() })
                .ToListAsync(cancellationToken))
                .Select(item => new DistribuicaoBase(((int)item.Chave).ToString(), item.Chave.ToString(), item.Quantidade)),
            totalAtivos);

        var porCriticidade = MapDistribuicao(
            (await queryAtivos
                .GroupBy(x => x.Criticidade)
                .Select(grupo => new { Chave = grupo.Key, Quantidade = grupo.Count() })
                .ToListAsync(cancellationToken))
                .Select(item => new DistribuicaoBase(((int)item.Chave).ToString(), item.Chave.ToString(), item.Quantidade)),
            totalAtivos);

        return new RelatorioInventarioAtivosPorStatusDto(porStatusOperacional, porStatusPatrimonial, porCriticidade);
    }

    public async Task<IReadOnlyCollection<RelatorioInventarioAtivosChamadosRecorrentesDto>> ObterInventarioAtivosChamadosRecorrentesAsync(
        FiltroRelatorioInventarioAtivosRequest request,
        CancellationToken cancellationToken = default)
    {
        await GarantirAcessoAdminAsync(cancellationToken);

        var periodo = NormalizarPeriodo(request);
        var limite = NormalizarLimiteRanking(request.LimiteRanking);
        var queryAtivos = CriarQueryInventarioAtivos(request, periodo);
        var ativosIds = queryAtivos.Select(x => x.Id);

        var chamados = await chamadoRepository.Query()
            .AsNoTracking()
            .Where(x =>
                x.AbertoEm >= periodo.Inicio &&
                x.AbertoEm < periodo.FimExclusivo &&
                x.InventarioAtivoId.HasValue &&
                ativosIds.Contains(x.InventarioAtivoId.Value))
            .Select(x => new
            {
                x.InventarioAtivoId,
                Status = x.Status.Codigo,
                x.AbertoEm
            })
            .ToListAsync(cancellationToken);

        var ativosBase = await queryAtivos
            .Select(x => new
            {
                x.Id,
                x.Codigo,
                x.Nome,
                TipoAtivo = x.TipoAtivoInventario.Nome,
                Departamento = x.Departamento != null ? x.Departamento.Nome : "Sem departamento",
                UsuarioResponsavel = x.UsuarioResponsavel != null ? x.UsuarioResponsavel.Nome : "Sem usuario responsavel"
            })
            .ToListAsync(cancellationToken);

        var mapaAtivos = ativosBase.ToDictionary(x => x.Id, x => x);

        return chamados
            .Where(x => x.InventarioAtivoId.HasValue && mapaAtivos.ContainsKey(x.InventarioAtivoId.Value))
            .GroupBy(x => x.InventarioAtivoId!.Value)
            .Select(grupo =>
            {
                var ativo = mapaAtivos[grupo.Key];
                return new RelatorioInventarioAtivosChamadosRecorrentesDto(
                    grupo.Key,
                    ativo.Codigo,
                    ativo.Nome,
                    ativo.TipoAtivo,
                    ativo.Departamento,
                    ativo.UsuarioResponsavel,
                    grupo.Count(),
                    grupo.Count(x => x.Status == StatusChamadoEnum.Aberto || x.Status == StatusChamadoEnum.EmAtendimento || x.Status == StatusChamadoEnum.AguardandoSolicitante),
                    grupo.Count(x => x.Status == StatusChamadoEnum.Encerrado || x.Status == StatusChamadoEnum.Resolvido),
                    grupo.Max(x => (DateTime?)x.AbertoEm));
            })
            .OrderByDescending(x => x.TotalChamados)
            .ThenBy(x => x.Codigo)
            .Take(limite)
            .ToArray();
    }

    public async Task<IReadOnlyCollection<RelatorioInventarioAtivosPorDepartamentoDto>> ObterInventarioAtivosPorDepartamentoAsync(
        FiltroRelatorioInventarioAtivosRequest request,
        CancellationToken cancellationToken = default)
    {
        await GarantirAcessoAdminAsync(cancellationToken);

        var periodo = NormalizarPeriodo(request);
        var queryAtivos = CriarQueryInventarioAtivos(request, periodo);
        var ativosIds = queryAtivos.Select(x => x.Id);

        var ativosPorDepartamento = await queryAtivos
            .GroupBy(x => new
            {
                x.DepartamentoId,
                Nome = x.Departamento != null ? x.Departamento.Nome : "Sem departamento"
            })
            .Select(grupo => new
            {
                grupo.Key.DepartamentoId,
                grupo.Key.Nome,
                TotalAtivos = grupo.Count(),
                AtivosAtivos = grupo.Count(x => x.Ativo),
                AtivosInativos = grupo.Count(x => !x.Ativo),
                Criticos = grupo.Count(x => x.Criticidade == CriticidadeAtivo.Critica)
            })
            .ToListAsync(cancellationToken);

        var chamadosPorDepartamento = await chamadoRepository.Query()
            .AsNoTracking()
            .Where(x =>
                x.AbertoEm >= periodo.Inicio &&
                x.AbertoEm < periodo.FimExclusivo &&
                x.InventarioAtivoId.HasValue &&
                ativosIds.Contains(x.InventarioAtivoId.Value))
            .GroupBy(x => new
            {
                x.InventarioAtivo!.DepartamentoId
            })
            .Select(grupo => new
            {
                grupo.Key.DepartamentoId,
                TotalComChamados = grupo.Select(x => x.InventarioAtivoId!.Value).Distinct().Count()
            })
            .ToListAsync(cancellationToken);

        var chamadosSemDepartamento = chamadosPorDepartamento
            .Where(x => !x.DepartamentoId.HasValue)
            .Select(x => x.TotalComChamados)
            .FirstOrDefault();
        var mapaChamados = chamadosPorDepartamento
            .Where(x => x.DepartamentoId.HasValue)
            .ToDictionary(x => x.DepartamentoId!.Value, x => x.TotalComChamados);

        return ativosPorDepartamento
            .Select(item => new RelatorioInventarioAtivosPorDepartamentoDto(
                item.DepartamentoId,
                item.Nome,
                item.TotalAtivos,
                item.AtivosAtivos,
                item.AtivosInativos,
                item.DepartamentoId.HasValue
                    ? mapaChamados.GetValueOrDefault(item.DepartamentoId.Value)
                    : chamadosSemDepartamento,
                item.Criticos))
            .OrderByDescending(x => x.TotalAtivos)
            .ThenBy(x => x.DepartamentoNome)
            .ToArray();
    }

    public async Task<RelatorioBaseConhecimentoResumoDto> ObterResumoBaseConhecimentoAsync(
        FiltroRelatorioBaseConhecimentoRequest request,
        CancellationToken cancellationToken = default)
    {
        await GarantirAcessoAdminAsync(cancellationToken);

        var periodo = NormalizarPeriodo(request);
        var queryArtigos = CriarQueryBaseConhecimento(request, periodo);
        var artigosIds = queryArtigos.Select(x => x.Id);

        var totalArtigos = await queryArtigos.CountAsync(cancellationToken);
        var artigosPublicados = await queryArtigos.CountAsync(x => x.Status == StatusArtigoConhecimento.Publicado, cancellationToken);
        var artigosRascunho = await queryArtigos.CountAsync(x => x.Status == StatusArtigoConhecimento.Rascunho, cancellationToken);
        var artigosArquivados = await queryArtigos.CountAsync(x => x.Status == StatusArtigoConhecimento.Arquivado, cancellationToken);
        var artigosAtivos = await queryArtigos.CountAsync(x => x.Ativo, cancellationToken);
        var artigosInativos = await queryArtigos.CountAsync(x => !x.Ativo, cancellationToken);

        var totalPorVisibilidadeBruto = await queryArtigos
            .GroupBy(x => x.Visibilidade)
            .Select(grupo => new { Chave = grupo.Key, Quantidade = grupo.Count() })
            .ToListAsync(cancellationToken);
        var totalPorVisibilidade = totalPorVisibilidadeBruto
            .Select(item => new IndicadorRelatorioDto(
                ((int)item.Chave).ToString(),
                item.Chave.ToString(),
                item.Quantidade,
                CalcularPercentual(item.Quantidade, totalArtigos)))
            .OrderByDescending(x => x.Quantidade)
            .ThenBy(x => x.Nome)
            .ToArray();

        var vinculos = await chamadoArtigoConhecimentoRepository.Query()
            .AsNoTracking()
            .Where(x =>
                x.VinculadoEm >= periodo.Inicio &&
                x.VinculadoEm < periodo.FimExclusivo &&
                artigosIds.Contains(x.ArtigoId))
            .Select(x => new { x.ArtigoId, x.ChamadoId })
            .ToListAsync(cancellationToken);

        var artigosVinculadosChamados = vinculos.Select(x => x.ArtigoId).Distinct().Count();
        var chamadosComArtigoVinculado = vinculos.Select(x => x.ChamadoId).Distinct().Count();

        return new RelatorioBaseConhecimentoResumoDto(
            totalArtigos,
            artigosPublicados,
            artigosRascunho,
            artigosArquivados,
            artigosAtivos,
            artigosInativos,
            totalPorVisibilidade,
            artigosVinculadosChamados,
            chamadosComArtigoVinculado);
    }

    public async Task<RelatorioBaseConhecimentoPorStatusDto> ObterBaseConhecimentoPorStatusAsync(
        FiltroRelatorioBaseConhecimentoRequest request,
        CancellationToken cancellationToken = default)
    {
        await GarantirAcessoAdminAsync(cancellationToken);

        var periodo = NormalizarPeriodo(request);
        var queryArtigos = CriarQueryBaseConhecimento(request, periodo);
        var totalArtigos = await queryArtigos.CountAsync(cancellationToken);

        var porStatus = MapDistribuicao(
            (await queryArtigos
                .GroupBy(x => x.Status)
                .Select(grupo => new { Chave = grupo.Key, Quantidade = grupo.Count() })
                .ToListAsync(cancellationToken))
                .Select(item => new DistribuicaoBase(((int)item.Chave).ToString(), item.Chave.ToString(), item.Quantidade)),
            totalArtigos);

        var porVisibilidade = MapDistribuicao(
            (await queryArtigos
                .GroupBy(x => x.Visibilidade)
                .Select(grupo => new { Chave = grupo.Key, Quantidade = grupo.Count() })
                .ToListAsync(cancellationToken))
                .Select(item => new DistribuicaoBase(((int)item.Chave).ToString(), item.Chave.ToString(), item.Quantidade)),
            totalArtigos);

        return new RelatorioBaseConhecimentoPorStatusDto(porStatus, porVisibilidade);
    }

    public async Task<IReadOnlyCollection<RelatorioBaseConhecimentoVinculosChamadosDto>> ObterBaseConhecimentoVinculosChamadosAsync(
        FiltroRelatorioBaseConhecimentoRequest request,
        CancellationToken cancellationToken = default)
    {
        await GarantirAcessoAdminAsync(cancellationToken);

        var periodo = NormalizarPeriodo(request);
        var limite = NormalizarLimiteRanking(request.LimiteRanking);
        var queryArtigos = CriarQueryBaseConhecimento(request, periodo);
        var artigosIds = queryArtigos.Select(x => x.Id);

        var vinculos = await chamadoArtigoConhecimentoRepository.Query()
            .AsNoTracking()
            .Where(x =>
                x.VinculadoEm >= periodo.Inicio &&
                x.VinculadoEm < periodo.FimExclusivo &&
                artigosIds.Contains(x.ArtigoId))
            .Select(x => new
            {
                x.ArtigoId,
                x.ChamadoId,
                x.VinculadoEm,
                x.Artigo.Titulo,
                x.Artigo.Status,
                x.Artigo.Visibilidade
            })
            .ToListAsync(cancellationToken);

        return vinculos
            .GroupBy(x => new { x.ArtigoId, x.Titulo, x.Status, x.Visibilidade })
            .Select(grupo => new RelatorioBaseConhecimentoVinculosChamadosDto(
                grupo.Key.ArtigoId,
                grupo.Key.Titulo,
                grupo.Key.Status.ToString(),
                grupo.Key.Visibilidade.ToString(),
                grupo.Select(x => x.ChamadoId).Distinct().Count(),
                grupo.Max(x => (DateTime?)x.VinculadoEm)))
            .OrderByDescending(x => x.TotalChamadosVinculados)
            .ThenBy(x => x.Titulo)
            .Take(limite)
            .ToArray();
    }

    public async Task<RelatorioAuditoriaResumoDto> ObterResumoAuditoriaAsync(
        FiltroRelatorioAuditoriaRequest request,
        CancellationToken cancellationToken = default)
    {
        await GarantirAcessoAdminAsync(cancellationToken);

        var periodo = NormalizarPeriodo(request);
        var queryAuditoria = CriarQueryAuditoria(request, periodo);

        var totalAcoesAuditadas = await queryAuditoria.CountAsync(cancellationToken);
        var usuariosComAcoes = await queryAuditoria
            .Where(x => x.UsuarioId.HasValue)
            .Select(x => x.UsuarioId!.Value)
            .Distinct()
            .CountAsync(cancellationToken);
        var entidadesAfetadas = await queryAuditoria
            .Select(x => x.Entidade)
            .Distinct()
            .CountAsync(cancellationToken);

        var totalPorTipoAcaoBruto = await queryAuditoria
            .GroupBy(x => x.Acao)
            .Select(grupo => new { Chave = grupo.Key, Quantidade = grupo.Count() })
            .ToListAsync(cancellationToken);
        var totalPorTipoAcao = totalPorTipoAcaoBruto
            .Select(item => new IndicadorRelatorioDto(
                ((int)item.Chave).ToString(),
                item.Chave.ToString(),
                item.Quantidade,
                CalcularPercentual(item.Quantidade, totalAcoesAuditadas)))
            .OrderByDescending(x => x.Quantidade)
            .ThenBy(x => x.Nome)
            .ToArray();

        var totalPorEntidadeBruto = await queryAuditoria
            .GroupBy(x => x.Entidade)
            .Select(grupo => new { Chave = grupo.Key, Quantidade = grupo.Count() })
            .ToListAsync(cancellationToken);
        var totalPorEntidade = totalPorEntidadeBruto
            .Select(item => new IndicadorRelatorioDto(
                item.Chave,
                item.Chave,
                item.Quantidade,
                CalcularPercentual(item.Quantidade, totalAcoesAuditadas)))
            .OrderByDescending(x => x.Quantidade)
            .ThenBy(x => x.Nome)
            .ToArray();

        var totalPorDiaBruto = await queryAuditoria
            .GroupBy(x => x.DataEvento.Date)
            .Select(grupo => new
            {
                Referencia = grupo.Key,
                Quantidade = grupo.Count()
            })
            .OrderBy(x => x.Referencia)
            .ToListAsync(cancellationToken);

        var totalPorDia = totalPorDiaBruto
            .Select(item => new PontoSerieTemporalDto(
                item.Referencia,
                item.Quantidade,
                item.Referencia.ToString("yyyy-MM-dd")))
            .ToArray();

        return new RelatorioAuditoriaResumoDto(
            totalAcoesAuditadas,
            usuariosComAcoes,
            entidadesAfetadas,
            totalPorTipoAcao,
            totalPorEntidade,
            totalPorDia);
    }

    public async Task<IReadOnlyCollection<RelatorioAuditoriaPorUsuarioDto>> ObterAuditoriaPorUsuarioAsync(
        FiltroRelatorioAuditoriaRequest request,
        CancellationToken cancellationToken = default)
    {
        await GarantirAcessoAdminAsync(cancellationToken);

        var periodo = NormalizarPeriodo(request);
        var limite = NormalizarLimiteRanking(request.LimiteRanking);
        var itens = await CriarQueryAuditoria(request, periodo)
            .Select(x => new
            {
                x.UsuarioId,
                UsuarioNome = x.UsuarioNome ?? x.UsuarioEmail ?? "Usuario nao identificado",
                x.Acao,
                x.DataEvento
            })
            .ToListAsync(cancellationToken);

        return itens
            .GroupBy(x => new { x.UsuarioId, x.UsuarioNome })
            .Select(grupo =>
            {
                var acoesPorTipo = grupo
                    .GroupBy(x => x.Acao)
                    .Select(item => new IndicadorRelatorioDto(
                        ((int)item.Key).ToString(),
                        item.Key.ToString(),
                        item.Count(),
                        CalcularPercentual(item.Count(), grupo.Count())))
                    .OrderByDescending(x => x.Quantidade)
                    .ThenBy(x => x.Nome)
                    .ToArray();

                return new RelatorioAuditoriaPorUsuarioDto(
                    grupo.Key.UsuarioId,
                    grupo.Key.UsuarioNome,
                    grupo.Count(),
                    grupo.Max(x => (DateTime?)x.DataEvento),
                    acoesPorTipo);
            })
            .OrderByDescending(x => x.TotalAcoes)
            .ThenBy(x => x.UsuarioNome)
            .Take(limite)
            .ToArray();
    }

    public async Task<IReadOnlyCollection<RelatorioAuditoriaPorEntidadeDto>> ObterAuditoriaPorEntidadeAsync(
        FiltroRelatorioAuditoriaRequest request,
        CancellationToken cancellationToken = default)
    {
        await GarantirAcessoAdminAsync(cancellationToken);

        var periodo = NormalizarPeriodo(request);
        var limite = NormalizarLimiteRanking(request.LimiteRanking);
        var itens = await CriarQueryAuditoria(request, periodo)
            .Select(x => new
            {
                x.Entidade,
                x.Acao,
                x.UsuarioId,
                x.DataEvento
            })
            .ToListAsync(cancellationToken);

        return itens
            .GroupBy(x => x.Entidade)
            .Select(grupo =>
            {
                var acoesPorTipo = grupo
                    .GroupBy(x => x.Acao)
                    .Select(item => new IndicadorRelatorioDto(
                        ((int)item.Key).ToString(),
                        item.Key.ToString(),
                        item.Count(),
                        CalcularPercentual(item.Count(), grupo.Count())))
                    .OrderByDescending(x => x.Quantidade)
                    .ThenBy(x => x.Nome)
                    .ToArray();

                return new RelatorioAuditoriaPorEntidadeDto(
                    grupo.Key,
                    grupo.Count(),
                    grupo.Where(x => x.UsuarioId.HasValue).Select(x => x.UsuarioId!.Value).Distinct().Count(),
                    grupo.Max(x => (DateTime?)x.DataEvento),
                    acoesPorTipo);
            })
            .OrderByDescending(x => x.TotalAcoes)
            .ThenBy(x => x.Entidade)
            .Take(limite)
            .ToArray();
    }

    private async Task GarantirAcessoAdminAsync(CancellationToken cancellationToken)
    {
        var usuario = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        if (!AdminUseCaseHelpers.PodeOperarAdmin(usuario))
        {
            throw new UnauthorizedAccessException("Acesso administrativo negado.");
        }
    }

    private static (DateTime Inicio, DateTime FimExclusivo) NormalizarPeriodo(FiltroPeriodoRelatorioRequest request)
    {
        var dataInicial = (request.ObterDataInicial() ?? DateTime.UtcNow.Date.AddDays(-30)).Date;
        var dataFinal = (request.ObterDataFinal() ?? DateTime.UtcNow.Date).Date;

        if (dataInicial > dataFinal)
        {
            throw new ArgumentException("DataInicial nao pode ser maior que DataFinal.");
        }

        if ((dataFinal - dataInicial).TotalDays > MaxDiasPeriodo)
        {
            throw new ArgumentException($"Periodo informado excede o limite maximo de {MaxDiasPeriodo} dias.");
        }

        return (dataInicial, dataFinal.AddDays(1));
    }

    private static AgrupamentoRelatorio NormalizarAgrupamentoTemporal(AgrupamentoRelatorio agrupamento)
    {
        if (agrupamento is AgrupamentoRelatorio.Dia or AgrupamentoRelatorio.Semana or AgrupamentoRelatorio.Mes)
        {
            return agrupamento;
        }

        throw new ArgumentException("Agrupamento temporal invalido. Use Dia, Semana ou Mes.", nameof(agrupamento));
    }

    private static int NormalizarLimiteRanking(int limite)
    {
        if (limite <= 0)
        {
            return LimitePadraoRanking;
        }

        return Math.Min(limite, LimiteMaximoRanking);
    }

    private IQueryable<Chamado> CriarQueryChamados(
        FiltroRelatorioChamadosRequest request,
        (DateTime Inicio, DateTime FimExclusivo) periodo)
    {
        var query = chamadoRepository.Query()
            .AsNoTracking()
            .Where(x => x.AbertoEm >= periodo.Inicio && x.AbertoEm < periodo.FimExclusivo)
            .AsQueryable();

        if (request.ApenasAtivos.HasValue)
        {
            query = query.Where(x => x.Ativo == request.ApenasAtivos.Value);
        }

        if (request.DepartamentoId.HasValue)
        {
            query = query.Where(x => x.DepartamentoId == request.DepartamentoId.Value);
        }

        if (request.CategoriaId.HasValue)
        {
            query = query.Where(x => x.CategoriaId == request.CategoriaId.Value);
        }

        if (request.SubcategoriaId.HasValue)
        {
            query = query.Where(x => x.SubcategoriaId == request.SubcategoriaId.Value);
        }

        if (request.PrioridadeId.HasValue)
        {
            query = query.Where(x => x.PrioridadeId == request.PrioridadeId.Value);
        }

        if (request.StatusId.HasValue)
        {
            query = query.Where(x => x.StatusId == request.StatusId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            var statusTexto = request.Status.Trim();
            if (Enum.TryParse<StatusChamadoEnum>(statusTexto, true, out var statusCodigo))
            {
                query = query.Where(x => x.Status.Codigo == statusCodigo);
            }
            else
            {
                var nomeStatus = statusTexto.ToLowerInvariant();
                query = query.Where(x => x.Status.Nome.ToLower() == nomeStatus);
            }
        }

        var atendenteId = request.ObterAtendenteId();
        if (atendenteId.HasValue)
        {
            query = query.Where(x => x.ResponsavelId == atendenteId.Value);
        }

        if (request.SolicitanteId.HasValue)
        {
            query = query.Where(x => x.SolicitanteId == request.SolicitanteId.Value);
        }

        if (request.CatalogoServicoId.HasValue)
        {
            query = query.Where(x => x.CatalogoServicoId == request.CatalogoServicoId.Value);
        }

        if (request.InventarioAtivoId.HasValue)
        {
            query = query.Where(x => x.InventarioAtivoId == request.InventarioAtivoId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Origem))
        {
            if (!Enum.TryParse<OrigemChamado>(request.Origem.Trim(), true, out var origem))
            {
                throw new ArgumentException("Origem informada invalida.", nameof(request.Origem));
            }

            query = query.Where(x => x.Origem == origem);
        }

        if (request.NaturezaChamado.HasValue)
        {
            query = query.Where(x => x.NaturezaChamado == request.NaturezaChamado.Value);
        }

        return query;
    }

    private IQueryable<Chamado> CriarQueryChamadosAtendimento(
        FiltroRelatorioAtendimentoRequest request,
        (DateTime Inicio, DateTime FimExclusivo) periodo)
    {
        var query = chamadoRepository.Query()
            .AsNoTracking()
            .Where(x => x.AbertoEm >= periodo.Inicio && x.AbertoEm < periodo.FimExclusivo)
            .AsQueryable();

        if (request.ApenasAtivos.HasValue)
        {
            query = query.Where(x => x.Ativo == request.ApenasAtivos.Value);
        }

        if (request.DepartamentoId.HasValue)
        {
            query = query.Where(x => x.DepartamentoId == request.DepartamentoId.Value);
        }

        if (request.CategoriaId.HasValue)
        {
            query = query.Where(x => x.CategoriaId == request.CategoriaId.Value);
        }

        if (request.SubcategoriaId.HasValue)
        {
            query = query.Where(x => x.SubcategoriaId == request.SubcategoriaId.Value);
        }

        if (request.PrioridadeId.HasValue)
        {
            query = query.Where(x => x.PrioridadeId == request.PrioridadeId.Value);
        }

        if (request.StatusId.HasValue)
        {
            query = query.Where(x => x.StatusId == request.StatusId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            var statusTexto = request.Status.Trim();
            if (Enum.TryParse<StatusChamadoEnum>(statusTexto, true, out var statusCodigo))
            {
                query = query.Where(x => x.Status.Codigo == statusCodigo);
            }
            else
            {
                var nomeStatus = statusTexto.ToLowerInvariant();
                query = query.Where(x => x.Status.Nome.ToLower() == nomeStatus);
            }
        }

        var atendenteId = request.ObterAtendenteId();
        if (atendenteId.HasValue)
        {
            query = query.Where(x => x.ResponsavelId == atendenteId.Value);
        }

        if (request.SolicitanteId.HasValue)
        {
            query = query.Where(x => x.SolicitanteId == request.SolicitanteId.Value);
        }

        if (request.CatalogoServicoId.HasValue)
        {
            query = query.Where(x => x.CatalogoServicoId == request.CatalogoServicoId.Value);
        }

        if (request.InventarioAtivoId.HasValue)
        {
            query = query.Where(x => x.InventarioAtivoId == request.InventarioAtivoId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Origem))
        {
            if (!Enum.TryParse<OrigemChamado>(request.Origem.Trim(), true, out var origem))
            {
                throw new ArgumentException("Origem informada invalida.", nameof(request.Origem));
            }

            query = query.Where(x => x.Origem == origem);
        }

        return query;
    }

    private IQueryable<Chamado> CriarQueryChamadosSla(
        FiltroRelatorioSlaRequest request,
        (DateTime Inicio, DateTime FimExclusivo) periodo)
    {
        var query = chamadoRepository.Query()
            .AsNoTracking()
            .Where(x => x.AbertoEm >= periodo.Inicio && x.AbertoEm < periodo.FimExclusivo)
            .AsQueryable();

        if (request.ApenasAtivos.HasValue)
        {
            query = query.Where(x => x.Ativo == request.ApenasAtivos.Value);
        }

        if (request.DepartamentoId.HasValue)
        {
            query = query.Where(x => x.DepartamentoId == request.DepartamentoId.Value);
        }

        if (request.CategoriaId.HasValue)
        {
            query = query.Where(x => x.CategoriaId == request.CategoriaId.Value);
        }

        if (request.SubcategoriaId.HasValue)
        {
            query = query.Where(x => x.SubcategoriaId == request.SubcategoriaId.Value);
        }

        if (request.PrioridadeId.HasValue)
        {
            query = query.Where(x => x.PrioridadeId == request.PrioridadeId.Value);
        }

        if (request.StatusId.HasValue)
        {
            query = query.Where(x => x.StatusId == request.StatusId.Value);
        }

        query = AplicarFiltroStatusChamado(query, request.Status);

        if (request.AtendenteId.HasValue)
        {
            query = query.Where(x => x.ResponsavelId == request.AtendenteId.Value);
        }

        if (request.SolicitanteId.HasValue)
        {
            query = query.Where(x => x.SolicitanteId == request.SolicitanteId.Value);
        }

        if (request.CatalogoServicoId.HasValue)
        {
            query = query.Where(x => x.CatalogoServicoId == request.CatalogoServicoId.Value);
        }

        if (request.PoliticaSlaId.HasValue)
        {
            query = query.Where(x => x.ChamadoSla != null && x.ChamadoSla.PoliticaSlaId == request.PoliticaSlaId.Value);
        }

        if (request.NaturezaChamado.HasValue)
        {
            query = query.Where(x => x.NaturezaChamado == request.NaturezaChamado.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SituacaoSla))
        {
            if (!Enum.TryParse<SituacaoSlaChamadoEnum>(request.SituacaoSla.Trim(), true, out var situacao))
            {
                throw new ArgumentException("SituacaoSla informada invalida.", nameof(request.SituacaoSla));
            }

            query = situacao switch
            {
                SituacaoSlaChamadoEnum.NaoAplicavel => query.Where(x => x.ChamadoSla == null),
                SituacaoSlaChamadoEnum.DentroDoPrazo or SituacaoSlaChamadoEnum.Cumprido => query.Where(x => x.ChamadoSla != null && x.ChamadoSla.ResolucaoCumprida == true),
                SituacaoSlaChamadoEnum.Pausado => query.Where(x => x.ChamadoSla != null && x.ChamadoSla.Pausado),
                SituacaoSlaChamadoEnum.ProximoDoVencimento => query.Where(
                    x => x.ChamadoSla != null
                        && x.ChamadoSla.ResolucaoCumprida == null
                        && !x.ChamadoSla.ResolucaoViolada
                        && x.ChamadoSla.PrazoResolucao >= DateTime.UtcNow
                        && x.ChamadoSla.PrazoResolucao <= DateTime.UtcNow.AddHours(24)),
                SituacaoSlaChamadoEnum.Vencido or SituacaoSlaChamadoEnum.Violado => query.Where(
                    x => x.ChamadoSla != null
                        && (x.ChamadoSla.ResolucaoViolada || x.ChamadoSla.ResolucaoCumprida == false || x.ChamadoSla.PrazoResolucao < DateTime.UtcNow)),
                _ => query
            };
        }

        return query;
    }

    private IQueryable<AprovacaoChamado> CriarQueryAprovacoes(
        FiltroRelatorioAprovacoesRequest request,
        (DateTime Inicio, DateTime FimExclusivo) periodo)
    {
        var query = aprovacaoChamadoRepository.Query()
            .AsNoTracking()
            .Where(x => x.SolicitadaEm >= periodo.Inicio && x.SolicitadaEm < periodo.FimExclusivo)
            .AsQueryable();

        if (request.ApenasAtivos.HasValue)
        {
            query = query.Where(x => x.Chamado.Ativo == request.ApenasAtivos.Value);
        }

        if (request.DepartamentoId.HasValue)
        {
            query = query.Where(x => x.Chamado.DepartamentoId == request.DepartamentoId.Value);
        }

        if (request.CategoriaId.HasValue)
        {
            query = query.Where(x => x.Chamado.CategoriaId == request.CategoriaId.Value);
        }

        if (request.SubcategoriaId.HasValue)
        {
            query = query.Where(x => x.Chamado.SubcategoriaId == request.SubcategoriaId.Value);
        }

        if (request.PrioridadeId.HasValue)
        {
            query = query.Where(x => x.Chamado.PrioridadeId == request.PrioridadeId.Value);
        }

        if (request.StatusId.HasValue)
        {
            query = query.Where(x => x.Chamado.StatusId == request.StatusId.Value);
        }

        query = AplicarFiltroStatusChamadoAprovacao(query, request.Status);

        if (request.AtendenteId.HasValue)
        {
            query = query.Where(x => x.Chamado.ResponsavelId == request.AtendenteId.Value);
        }

        if (request.SolicitanteId.HasValue)
        {
            query = query.Where(x => x.Chamado.SolicitanteId == request.SolicitanteId.Value);
        }

        if (request.CatalogoServicoId.HasValue)
        {
            query = query.Where(x => x.Chamado.CatalogoServicoId == request.CatalogoServicoId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.TipoOrigemAprovacao))
        {
            if (!Enum.TryParse<TipoOrigemAprovacaoChamado>(request.TipoOrigemAprovacao.Trim(), true, out var tipoOrigem))
            {
                throw new ArgumentException("TipoOrigemAprovacao informado invalido.", nameof(request.TipoOrigemAprovacao));
            }

            query = query.Where(x => x.TipoOrigem == tipoOrigem);
        }

        if (!string.IsNullOrWhiteSpace(request.StatusAprovacao))
        {
            if (!Enum.TryParse<StatusAprovacaoChamado>(request.StatusAprovacao.Trim(), true, out var statusAprovacao))
            {
                throw new ArgumentException("StatusAprovacao informado invalido.", nameof(request.StatusAprovacao));
            }

            query = query.Where(x => x.Status == statusAprovacao);
        }

        return query;
    }

    private IQueryable<CatalogoServico> CriarQueryCatalogoServicos(FiltroRelatorioCatalogoServicosRequest request)
    {
        var query = catalogoServicoRepository.Query()
            .AsNoTracking()
            .AsQueryable();

        if (request.ApenasAtivos.HasValue)
        {
            query = query.Where(x => x.Ativo == request.ApenasAtivos.Value);
        }

        if (request.CatalogoServicoId.HasValue)
        {
            query = query.Where(x => x.Id == request.CatalogoServicoId.Value);
        }

        if (request.DepartamentoId.HasValue)
        {
            query = query.Where(x => x.DepartamentoResponsavelId == request.DepartamentoId.Value);
        }

        if (request.CategoriaId.HasValue)
        {
            query = query.Where(x => x.CategoriaId == request.CategoriaId.Value);
        }

        if (request.SubcategoriaId.HasValue)
        {
            query = query.Where(x => x.SubcategoriaId == request.SubcategoriaId.Value);
        }

        if (request.PrioridadeId.HasValue)
        {
            query = query.Where(x => x.PrioridadePadraoId == request.PrioridadeId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (Enum.TryParse<StatusCatalogoServico>(request.Status.Trim(), true, out var statusCatalogo))
            {
                query = query.Where(x => x.Status == statusCatalogo);
            }
            else
            {
                var statusTexto = request.Status.Trim().ToLowerInvariant();
                query = query.Where(x => x.Status.ToString().ToLower() == statusTexto);
            }
        }

        return query;
    }

    private IQueryable<Chamado> CriarQueryChamadosCatalogo(
        FiltroRelatorioCatalogoServicosRequest request,
        (DateTime Inicio, DateTime FimExclusivo) periodo)
    {
        var query = chamadoRepository.Query()
            .AsNoTracking()
            .Where(x => x.AbertoEm >= periodo.Inicio && x.AbertoEm < periodo.FimExclusivo)
            .AsQueryable();

        if (request.ApenasAtivos.HasValue)
        {
            query = query.Where(x => x.Ativo == request.ApenasAtivos.Value);
        }

        if (request.DepartamentoId.HasValue)
        {
            query = query.Where(x => x.DepartamentoId == request.DepartamentoId.Value);
        }

        if (request.CategoriaId.HasValue)
        {
            query = query.Where(x => x.CategoriaId == request.CategoriaId.Value);
        }

        if (request.SubcategoriaId.HasValue)
        {
            query = query.Where(x => x.SubcategoriaId == request.SubcategoriaId.Value);
        }

        if (request.PrioridadeId.HasValue)
        {
            query = query.Where(x => x.PrioridadeId == request.PrioridadeId.Value);
        }

        if (request.StatusId.HasValue)
        {
            query = query.Where(x => x.StatusId == request.StatusId.Value);
        }

        query = AplicarFiltroStatusChamado(query, request.Status);

        if (request.AtendenteId.HasValue)
        {
            query = query.Where(x => x.ResponsavelId == request.AtendenteId.Value);
        }

        if (request.SolicitanteId.HasValue)
        {
            query = query.Where(x => x.SolicitanteId == request.SolicitanteId.Value);
        }

        if (request.CatalogoServicoId.HasValue)
        {
            query = query.Where(x => x.CatalogoServicoId == request.CatalogoServicoId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.TipoOrigemAprovacao))
        {
            if (!Enum.TryParse<TipoOrigemAprovacaoChamado>(request.TipoOrigemAprovacao.Trim(), true, out var tipoOrigem))
            {
                throw new ArgumentException("TipoOrigemAprovacao informado invalido.", nameof(request.TipoOrigemAprovacao));
            }

            query = query.Where(x => x.Aprovacoes.Any(a => a.TipoOrigem == tipoOrigem));
        }

        if (!string.IsNullOrWhiteSpace(request.StatusAprovacao))
        {
            if (!Enum.TryParse<StatusAprovacaoChamado>(request.StatusAprovacao.Trim(), true, out var statusAprovacao))
            {
                throw new ArgumentException("StatusAprovacao informado invalido.", nameof(request.StatusAprovacao));
            }

            query = query.Where(x => x.Aprovacoes.Any(a => a.Status == statusAprovacao));
        }

        return query;
    }

    private IQueryable<InventarioAtivo> CriarQueryInventarioAtivos(
        FiltroRelatorioInventarioAtivosRequest request,
        (DateTime Inicio, DateTime FimExclusivo) periodo)
    {
        var query = inventarioAtivoRepository.Query()
            .AsNoTracking()
            .Where(x => x.CriadoEm >= periodo.Inicio && x.CriadoEm < periodo.FimExclusivo)
            .AsQueryable();

        if (request.Ativo.HasValue)
        {
            query = query.Where(x => x.Ativo == request.Ativo.Value);
        }

        if (request.DepartamentoId.HasValue)
        {
            query = query.Where(x => x.DepartamentoId == request.DepartamentoId.Value);
        }

        if (request.LocalUnidadeId.HasValue)
        {
            query = query.Where(x => x.LocalUnidadeId == request.LocalUnidadeId.Value);
        }

        if (request.UsuarioResponsavelId.HasValue)
        {
            query = query.Where(x => x.UsuarioResponsavelId == request.UsuarioResponsavelId.Value);
        }

        if (request.TipoAtivoInventarioId.HasValue)
        {
            query = query.Where(x => x.TipoAtivoInventarioId == request.TipoAtivoInventarioId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.StatusOperacional))
        {
            if (!Enum.TryParse<StatusOperacionalAtivo>(request.StatusOperacional.Trim(), true, out var statusOperacional))
            {
                throw new ArgumentException("StatusOperacional informado invalido.", nameof(request.StatusOperacional));
            }

            query = query.Where(x => x.StatusOperacional == statusOperacional);
        }

        if (!string.IsNullOrWhiteSpace(request.StatusPatrimonial))
        {
            if (!Enum.TryParse<StatusPatrimonialAtivo>(request.StatusPatrimonial.Trim(), true, out var statusPatrimonial))
            {
                throw new ArgumentException("StatusPatrimonial informado invalido.", nameof(request.StatusPatrimonial));
            }

            query = query.Where(x => x.StatusPatrimonial == statusPatrimonial);
        }

        if (!string.IsNullOrWhiteSpace(request.Criticidade))
        {
            if (!Enum.TryParse<CriticidadeAtivo>(request.Criticidade.Trim(), true, out var criticidade))
            {
                throw new ArgumentException("Criticidade informada invalida.", nameof(request.Criticidade));
            }

            query = query.Where(x => x.Criticidade == criticidade);
        }

        return query;
    }

    private IQueryable<BaseConhecimentoArtigo> CriarQueryBaseConhecimento(
        FiltroRelatorioBaseConhecimentoRequest request,
        (DateTime Inicio, DateTime FimExclusivo) periodo)
    {
        var query = baseConhecimentoArtigoRepository.Query()
            .AsNoTracking()
            .Where(x => x.CriadoEm >= periodo.Inicio && x.CriadoEm < periodo.FimExclusivo)
            .AsQueryable();

        if (request.Ativo.HasValue)
        {
            query = query.Where(x => x.Ativo == request.Ativo.Value);
        }

        if (request.CategoriaId.HasValue)
        {
            query = query.Where(x => x.CategoriaId == request.CategoriaId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.StatusArtigo))
        {
            if (!Enum.TryParse<StatusArtigoConhecimento>(request.StatusArtigo.Trim(), true, out var statusArtigo))
            {
                throw new ArgumentException("StatusArtigo informado invalido.", nameof(request.StatusArtigo));
            }

            query = query.Where(x => x.Status == statusArtigo);
        }

        if (!string.IsNullOrWhiteSpace(request.VisibilidadeArtigo))
        {
            if (!Enum.TryParse<VisibilidadeArtigoConhecimento>(request.VisibilidadeArtigo.Trim(), true, out var visibilidadeArtigo))
            {
                throw new ArgumentException("VisibilidadeArtigo informada invalida.", nameof(request.VisibilidadeArtigo));
            }

            query = query.Where(x => x.Visibilidade == visibilidadeArtigo);
        }

        return query;
    }

    private IQueryable<EventoAuditoria> CriarQueryAuditoria(
        FiltroRelatorioAuditoriaRequest request,
        (DateTime Inicio, DateTime FimExclusivo) periodo)
    {
        var query = eventoAuditoriaRepository.Query()
            .AsNoTracking()
            .Where(x => x.DataEvento >= periodo.Inicio && x.DataEvento < periodo.FimExclusivo)
            .AsQueryable();

        if (request.UsuarioId.HasValue)
        {
            query = query.Where(x => x.UsuarioId == request.UsuarioId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Entidade))
        {
            var entidade = request.Entidade.Trim().ToLowerInvariant();
            query = query.Where(x => x.Entidade.ToLower().Contains(entidade));
        }

        if (!string.IsNullOrWhiteSpace(request.TipoAcao))
        {
            if (!Enum.TryParse<TipoAcaoAuditoria>(request.TipoAcao.Trim(), true, out var tipoAcao))
            {
                throw new ArgumentException("TipoAcao informado invalido.", nameof(request.TipoAcao));
            }

            query = query.Where(x => x.Acao == tipoAcao);
        }

        if (!string.IsNullOrWhiteSpace(request.Termo))
        {
            var termo = request.Termo.Trim().ToLowerInvariant();
            query = query.Where(x =>
                x.Descricao.ToLower().Contains(termo) ||
                x.Modulo.ToLower().Contains(termo) ||
                x.Entidade.ToLower().Contains(termo) ||
                (x.UsuarioNome != null && x.UsuarioNome.ToLower().Contains(termo)) ||
                (x.UsuarioEmail != null && x.UsuarioEmail.ToLower().Contains(termo)));
        }

        return query;
    }

    private static IQueryable<Chamado> AplicarFiltroStatusChamado(IQueryable<Chamado> query, string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            return query;
        }

        var statusTexto = status.Trim();
        if (Enum.TryParse<StatusChamadoEnum>(statusTexto, true, out var statusCodigo))
        {
            return query.Where(x => x.Status.Codigo == statusCodigo);
        }

        var nomeStatus = statusTexto.ToLowerInvariant();
        return query.Where(x => x.Status.Nome.ToLower() == nomeStatus);
    }

    private static IQueryable<AprovacaoChamado> AplicarFiltroStatusChamadoAprovacao(IQueryable<AprovacaoChamado> query, string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            return query;
        }

        var statusTexto = status.Trim();
        if (Enum.TryParse<StatusChamadoEnum>(statusTexto, true, out var statusCodigo))
        {
            return query.Where(x => x.Chamado.Status.Codigo == statusCodigo);
        }

        var nomeStatus = statusTexto.ToLowerInvariant();
        return query.Where(x => x.Chamado.Status.Nome.ToLower() == nomeStatus);
    }

    private static DistribuicaoRelatorioDto[] MapDistribuicao(IEnumerable<DistribuicaoBase> itens, int totalChamados)
    {
        return itens
            .OrderByDescending(x => x.Quantidade)
            .ThenBy(x => x.Nome)
            .Select(item => new DistribuicaoRelatorioDto(
                item.Chave,
                item.Nome,
                item.Quantidade,
                totalChamados == 0 ? 0 : Math.Round((decimal)item.Quantidade * 100 / totalChamados, 2)))
            .ToArray();
    }

    private static decimal? CalcularPercentual(int quantidade, int totalChamados)
    {
        if (totalChamados == 0)
        {
            return null;
        }

        return Math.Round((decimal)quantidade * 100 / totalChamados, 2);
    }

    private static async Task<double?> CalcularTempoMedioAtendimentoAsync(IQueryable<Chamado> queryChamados, CancellationToken cancellationToken)
    {
        var tempos = await queryChamados
            .Where(x => x.EncerradoEm.HasValue)
            .Select(x => new { x.AbertoEm, EncerradoEm = x.EncerradoEm!.Value })
            .ToListAsync(cancellationToken);

        if (tempos.Count == 0)
        {
            return null;
        }

        var horas = tempos
            .Select(item => (item.EncerradoEm - item.AbertoEm).TotalHours)
            .Where(valor => valor >= 0)
            .ToArray();

        if (horas.Length == 0)
        {
            return null;
        }

        return Math.Round(horas.Average(), 2);
    }

    private static async Task<double?> CalcularTempoMedioPrimeiraAcaoAsync(IQueryable<Chamado> queryChamados, CancellationToken cancellationToken)
    {
        var tempos = await queryChamados
            .Where(x => x.ChamadoSla != null && x.ChamadoSla.DataPrimeiraResposta.HasValue)
            .Select(x => new
            {
                Inicio = x.ChamadoSla!.DataInicio,
                PrimeiraResposta = x.ChamadoSla.DataPrimeiraResposta!.Value,
                x.ChamadoSla.MinutosPausados
            })
            .ToListAsync(cancellationToken);

        if (tempos.Count == 0)
        {
            return null;
        }

        var horas = tempos
            .Select(item => Math.Max(0, (item.PrimeiraResposta - item.Inicio).TotalMinutes - item.MinutosPausados) / 60d)
            .ToArray();

        if (horas.Length == 0)
        {
            return null;
        }

        return Math.Round(horas.Average(), 2);
    }

    private static DateTime ObterChaveAgrupamento(DateTime data, AgrupamentoRelatorio agrupamento)
        => agrupamento switch
        {
            AgrupamentoRelatorio.Dia => data.Date,
            AgrupamentoRelatorio.Semana => ObterInicioSemana(data),
            AgrupamentoRelatorio.Mes => new DateTime(data.Year, data.Month, 1),
            _ => data.Date
        };

    private static string FormatarPeriodo(DateTime referencia, AgrupamentoRelatorio agrupamento)
        => agrupamento switch
        {
            AgrupamentoRelatorio.Dia => referencia.ToString("yyyy-MM-dd"),
            AgrupamentoRelatorio.Semana => $"Semana de {referencia:yyyy-MM-dd}",
            AgrupamentoRelatorio.Mes => referencia.ToString("yyyy-MM"),
            _ => referencia.ToString("yyyy-MM-dd")
        };

    private static DateTime ObterInicioSemana(DateTime data)
    {
        var dia = data.Date;
        var deslocamento = ((int)dia.DayOfWeek + 6) % 7;
        return dia.AddDays(-deslocamento);
    }

    private sealed record TotalDia(DateTime Data, int Total);

    private sealed class SerieTemporalValores
    {
        public int Abertos { get; set; }
        public int Encerrados { get; set; }
        public int Reabertos { get; set; }
    }

    private sealed record DistribuicaoBase(string Chave, string Nome, int Quantidade);

    private sealed record ProducaoAtendenteBase(
        Guid AtendenteId,
        string AtendenteNome,
        int ChamadosAssumidos,
        int ChamadosConcluidos,
        int ChamadosEmAberto);

    private sealed record TempoConclusaoBase(Guid AtendenteId, DateTime AbertoEm, DateTime EncerradoEm);
}
