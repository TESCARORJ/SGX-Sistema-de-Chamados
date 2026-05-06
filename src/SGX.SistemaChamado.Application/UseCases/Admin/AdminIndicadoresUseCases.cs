using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Services.Sla;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public sealed class AdminIndicadoresUseCases(
    IRepository<Chamado> chamadoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) :
    IObterDashboardAdminUseCase,
    IObterIndicadoresChamadosPorStatusUseCase,
    IObterIndicadoresChamadosPorPrioridadeUseCase,
    IObterIndicadoresChamadosPorCategoriaUseCase,
    IObterIndicadoresSlaUseCase,
    IObterIndicadoresProdutividadeUseCase
{
    public async Task<DashboardAdminResponse> ExecutarAsync(FiltroIndicadoresRequest request, CancellationToken cancellationToken = default)
    {
        var chamados = await CarregarChamadosAsync(request, cancellationToken);

        var porStatus = MapChamadosPorStatus(chamados);
        var porPrioridade = MapChamadosPorPrioridade(chamados);
        var porCategoria = MapChamadosPorCategoria(chamados);
        var indicadoresSla = MapIndicadoresSla(chamados);
        var produtividade = MapProdutividade(chamados);

        var totalAbertos = chamados.Count(x => x.Status.Codigo == StatusChamadoEnum.Aberto);
        var totalEmAtendimento = chamados.Count(x => x.Status.Codigo == StatusChamadoEnum.EmAtendimento);
        var totalAguardandoSolicitante = chamados.Count(x => x.Status.Codigo == StatusChamadoEnum.AguardandoSolicitante);
        var totalResolvidosPeriodo = chamados.Count(x => x.Status.Codigo == StatusChamadoEnum.Resolvido);
        var totalEncerradosPeriodo = chamados.Count(x => x.Status.Codigo == StatusChamadoEnum.Encerrado);
        var totalVencidos = chamados.Count(x => x.SlaControle?.EstaVencido == true);
        var totalProximosDoVencimento = chamados.Count(x => SlaRules.EstaProximoDoVencimento(x.SlaControle, DateTime.UtcNow));
        var totalSemResponsavel = chamados.Count(x => !x.ResponsavelId.HasValue);

        return new DashboardAdminResponse
        {
            TotalAbertos = totalAbertos,
            TotalEmAtendimento = totalEmAtendimento,
            TotalAguardandoSolicitante = totalAguardandoSolicitante,
            TotalResolvidosPeriodo = totalResolvidosPeriodo,
            TotalEncerradosPeriodo = totalEncerradosPeriodo,
            TotalVencidos = totalVencidos,
            TotalProximosDoVencimento = totalProximosDoVencimento,
            TotalSemResponsavel = totalSemResponsavel,
            Cards =
            [
                new IndicadorCardResponse("abertos", "Chamados abertos", totalAbertos),
                new IndicadorCardResponse("em_atendimento", "Em atendimento", totalEmAtendimento),
                new IndicadorCardResponse("aguardando_solicitante", "Aguardando solicitante", totalAguardandoSolicitante),
                new IndicadorCardResponse("vencidos", "SLA vencido", totalVencidos),
                new IndicadorCardResponse("proximos_vencimento", "Proximos do vencimento", totalProximosDoVencimento),
                new IndicadorCardResponse("resolvidos_periodo", "Resolvidos no periodo", totalResolvidosPeriodo),
                new IndicadorCardResponse("sem_responsavel", "Sem responsavel", totalSemResponsavel)
            ],
            ChamadosPorStatus = porStatus,
            ChamadosPorPrioridade = porPrioridade,
            ChamadosPorCategoria = porCategoria,
            IndicadoresSla = indicadoresSla,
            ProdutividadePorAtendente = produtividade
        };
    }

    Task<IReadOnlyCollection<ChamadosPorStatusResponse>> IObterIndicadoresChamadosPorStatusUseCase.ExecutarAsync(FiltroIndicadoresRequest request, CancellationToken cancellationToken)
        => ObterChamadosPorStatusAsync(request, cancellationToken);

    Task<IReadOnlyCollection<ChamadosPorPrioridadeResponse>> IObterIndicadoresChamadosPorPrioridadeUseCase.ExecutarAsync(FiltroIndicadoresRequest request, CancellationToken cancellationToken)
        => ObterChamadosPorPrioridadeAsync(request, cancellationToken);

    Task<IReadOnlyCollection<ChamadosPorCategoriaResponse>> IObterIndicadoresChamadosPorCategoriaUseCase.ExecutarAsync(FiltroIndicadoresRequest request, CancellationToken cancellationToken)
        => ObterChamadosPorCategoriaAsync(request, cancellationToken);

    Task<IndicadoresSlaResponse> IObterIndicadoresSlaUseCase.ExecutarAsync(FiltroIndicadoresRequest request, CancellationToken cancellationToken)
        => ObterIndicadoresSlaAsync(request, cancellationToken);

    Task<IReadOnlyCollection<ProdutividadeAtendenteResponse>> IObterIndicadoresProdutividadeUseCase.ExecutarAsync(FiltroIndicadoresRequest request, CancellationToken cancellationToken)
        => ObterProdutividadeAsync(request, cancellationToken);

    private async Task<IReadOnlyCollection<ChamadosPorStatusResponse>> ObterChamadosPorStatusAsync(FiltroIndicadoresRequest request, CancellationToken cancellationToken)
    {
        var chamados = await CarregarChamadosAsync(request, cancellationToken);
        return MapChamadosPorStatus(chamados);
    }

    private async Task<IReadOnlyCollection<ChamadosPorPrioridadeResponse>> ObterChamadosPorPrioridadeAsync(FiltroIndicadoresRequest request, CancellationToken cancellationToken)
    {
        var chamados = await CarregarChamadosAsync(request, cancellationToken);
        return MapChamadosPorPrioridade(chamados);
    }

    private async Task<IReadOnlyCollection<ChamadosPorCategoriaResponse>> ObterChamadosPorCategoriaAsync(FiltroIndicadoresRequest request, CancellationToken cancellationToken)
    {
        var chamados = await CarregarChamadosAsync(request, cancellationToken);
        return MapChamadosPorCategoria(chamados);
    }

    private async Task<IndicadoresSlaResponse> ObterIndicadoresSlaAsync(FiltroIndicadoresRequest request, CancellationToken cancellationToken)
    {
        var chamados = await CarregarChamadosAsync(request, cancellationToken);
        return MapIndicadoresSla(chamados);
    }

    private async Task<IReadOnlyCollection<ProdutividadeAtendenteResponse>> ObterProdutividadeAsync(FiltroIndicadoresRequest request, CancellationToken cancellationToken)
    {
        var chamados = await CarregarChamadosAsync(request, cancellationToken);
        return MapProdutividade(chamados);
    }

    private async Task<List<Chamado>> CarregarChamadosAsync(FiltroIndicadoresRequest request, CancellationToken cancellationToken)
    {
        var usuario = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        if (!AdminUseCaseHelpers.PodeOperarAdmin(usuario))
        {
            throw new UnauthorizedAccessException("Acesso administrativo negado.");
        }

        var query = chamadoRepository.Query()
            .AsNoTracking()
            .Include(x => x.Status)
            .Include(x => x.Prioridade)
            .Include(x => x.Categoria)
            .Include(x => x.Responsavel)
            .Include(x => x.SlaControle)
            .Where(x => x.Ativo)
            .AsQueryable();

        if (request.DataInicio.HasValue)
        {
            query = query.Where(x => x.AbertoEm >= request.DataInicio.Value);
        }

        if (request.DataFim.HasValue)
        {
            var dataFinalExclusiva = request.DataFim.Value.Date.AddDays(1);
            query = query.Where(x => x.AbertoEm < dataFinalExclusiva);
        }

        if (request.DepartamentoId.HasValue)
        {
            query = query.Where(x => x.DepartamentoId == request.DepartamentoId.Value);
        }

        if (request.CategoriaId.HasValue)
        {
            query = query.Where(x => x.CategoriaId == request.CategoriaId.Value);
        }

        if (request.ResponsavelId.HasValue)
        {
            query = query.Where(x => x.ResponsavelId == request.ResponsavelId.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }

    private static IReadOnlyCollection<ChamadosPorStatusResponse> MapChamadosPorStatus(IReadOnlyCollection<Chamado> chamados)
        => chamados
            .GroupBy(x => x.Status.Nome)
            .OrderByDescending(x => x.Count())
            .Select(x => new ChamadosPorStatusResponse(x.Key, x.Count()))
            .ToArray();

    private static IReadOnlyCollection<ChamadosPorPrioridadeResponse> MapChamadosPorPrioridade(IReadOnlyCollection<Chamado> chamados)
        => chamados
            .GroupBy(x => x.Prioridade.Nome)
            .OrderByDescending(x => x.Count())
            .Select(x => new ChamadosPorPrioridadeResponse(x.Key, x.Count()))
            .ToArray();

    private static IReadOnlyCollection<ChamadosPorCategoriaResponse> MapChamadosPorCategoria(IReadOnlyCollection<Chamado> chamados)
        => chamados
            .GroupBy(x => x.Categoria.Nome)
            .OrderByDescending(x => x.Count())
            .Select(x => new ChamadosPorCategoriaResponse(x.Key, x.Count()))
            .ToArray();

    private static IndicadoresSlaResponse MapIndicadoresSla(IReadOnlyCollection<Chamado> chamados)
    {
        var comSla = chamados.Where(x => x.SlaControle is not null).ToArray();
        var totalChamados = comSla.Length;
        var totalVencidos = comSla.Count(x => x.SlaControle!.EstaVencido);
        var totalDentroDoPrazo = totalChamados - totalVencidos;
        var totalProximos = comSla.Count(x => SlaRules.EstaProximoDoVencimento(x.SlaControle, DateTime.UtcNow));

        var horasResolucao = comSla
            .Where(x => x.SlaControle!.ResolvidoEm.HasValue)
            .Select(CalcularHorasResolucao)
            .ToArray();

        var horasPrimeiraResposta = comSla
            .Where(x => x.SlaControle!.PrimeiraRespostaEm.HasValue)
            .Select(CalcularHorasPrimeiraResposta)
            .ToArray();

        var percentualCumprimento = totalChamados == 0
            ? 0
            : Math.Round((decimal)totalDentroDoPrazo * 100 / totalChamados, 2);

        return new IndicadoresSlaResponse
        {
            TotalChamados = totalChamados,
            TotalDentroDoPrazo = totalDentroDoPrazo,
            TotalVencidos = totalVencidos,
            PercentualCumprimento = percentualCumprimento,
            TotalProximosDoVencimento = totalProximos,
            MediaHorasResolucao = CalcularMedia(horasResolucao),
            MediaHorasPrimeiraResposta = CalcularMedia(horasPrimeiraResposta)
        };
    }

    private static IReadOnlyCollection<ProdutividadeAtendenteResponse> MapProdutividade(IReadOnlyCollection<Chamado> chamados)
    {
        return chamados
            .Where(x => x.ResponsavelId.HasValue && x.Responsavel is not null)
            .GroupBy(x => new { Id = x.ResponsavelId!.Value, x.Responsavel!.Nome })
            .Select(grupo =>
            {
                var resolvidos = grupo
                    .Where(x => x.SlaControle?.ResolvidoEm.HasValue == true)
                    .Select(CalcularHorasResolucao)
                    .ToArray();

                return new ProdutividadeAtendenteResponse
                {
                    ResponsavelId = grupo.Key.Id,
                    ResponsavelNome = grupo.Key.Nome,
                    TotalAtendidos = grupo.Count(),
                    TotalEncerrados = grupo.Count(x => x.Status.Codigo is StatusChamadoEnum.Encerrado or StatusChamadoEnum.Resolvido),
                    TotalVencidos = grupo.Count(x => x.SlaControle?.EstaVencido == true),
                    MediaHorasResolucao = CalcularMedia(resolvidos)
                };
            })
            .OrderByDescending(x => x.TotalAtendidos)
            .ThenBy(x => x.ResponsavelNome)
            .ToArray();
    }

    private static double CalcularHorasResolucao(Chamado chamado)
    {
        if (chamado.SlaControle?.ResolvidoEm is null)
        {
            return 0;
        }

        var minutos = (chamado.SlaControle.ResolvidoEm.Value - chamado.AbertoEm).TotalMinutes - chamado.SlaControle.TotalMinutosPausado;
        return Math.Max(0, minutos) / 60d;
    }

    private static double CalcularHorasPrimeiraResposta(Chamado chamado)
    {
        if (chamado.SlaControle?.PrimeiraRespostaEm is null)
        {
            return 0;
        }

        var minutos = (chamado.SlaControle.PrimeiraRespostaEm.Value - chamado.AbertoEm).TotalMinutes - chamado.SlaControle.TotalMinutosPausado;
        return Math.Max(0, minutos) / 60d;
    }

    private static double? CalcularMedia(IReadOnlyCollection<double> valores)
    {
        if (valores.Count == 0)
        {
            return null;
        }

        return Math.Round(valores.Average(), 2);
    }
}
