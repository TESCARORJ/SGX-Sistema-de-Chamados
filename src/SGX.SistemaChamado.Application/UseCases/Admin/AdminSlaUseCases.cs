using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Helpers;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Auditoria;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Services.Sla;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public sealed class ListarPoliticasSlaUseCase(
    IRepository<PoliticaSla> politicaRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IListarPoliticasSlaUseCase
{
    public async Task<IReadOnlyCollection<PoliticaSlaResponse>> ExecutarAsync(
        FiltroPoliticaSlaRequest request,
        CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var query = politicaRepository.Query()
            .AsNoTracking()
            .Include(x => x.Categoria)
            .Include(x => x.Departamento)
            .Include(x => x.CalendarioCorporativo)
            .Include(x => x.Metas.Where(meta => meta.Ativo))
            .ThenInclude(x => x.Prioridade)
            .AsQueryable();

        if (request.Ativo.HasValue)
        {
            query = query.Where(x => x.Ativo == request.Ativo.Value);
        }

        if (request.CategoriaId.HasValue)
        {
            query = query.Where(x => x.CategoriaId == request.CategoriaId.Value);
        }

        if (request.DepartamentoId.HasValue)
        {
            query = query.Where(x => x.DepartamentoId == request.DepartamentoId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Texto))
        {
            var texto = request.Texto.Trim().ToLower();
            query = query.Where(x =>
                x.Nome.ToLower().Contains(texto) ||
                (x.Descricao != null && x.Descricao.ToLower().Contains(texto)));
        }

        var politicas = await query
            .OrderBy(x => x.Ordem)
            .ThenBy(x => x.Nome)
            .ToListAsync(cancellationToken);

        return politicas.Select(PoliticasSlaMapper.Map).ToArray();
    }
}

public sealed class ObterPoliticaSlaUseCase(
    IRepository<PoliticaSla> politicaRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IObterPoliticaSlaUseCase
{
    public async Task<PoliticaSlaResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id da politica de SLA invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var politica = await PoliticasSlaMapper.CarregarPoliticaPorIdAsync(politicaRepository, id, cancellationToken)
            ?? throw new KeyNotFoundException("Politica de SLA nao encontrada.");

        return PoliticasSlaMapper.Map(politica);
    }
}

public sealed class CriarPoliticaSlaUseCase(
    IRepository<PoliticaSla> politicaRepository,
    IRepository<MetaSla> metaRepository,
    IRepository<PrioridadeChamado> prioridadeRepository,
    IRepository<CategoriaChamado> categoriaRepository,
    IRepository<Departamento> departamentoRepository,
    IRepository<CalendarioCorporativo> calendarioRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null) : ICriarPoliticaSlaUseCase
{
    public async Task<PoliticaSlaResponse> ExecutarAsync(CriarPoliticaSlaRequest request, CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        await PoliticasSlaMapper.ValidarReferenciasAsync(
            request.CategoriaId,
            request.DepartamentoId,
            request.UsarHorarioComercial ? request.CalendarioCorporativoId : null,
            request.Metas,
            prioridadeRepository,
            categoriaRepository,
            departamentoRepository,
            calendarioRepository,
            cancellationToken);

        PoliticasSlaMapper.ValidarDuplicidadePrioridade(request.Metas);

        var politica = new PoliticaSla(
            request.Nome,
            request.Descricao,
            request.Ordem,
            request.CategoriaId,
            request.DepartamentoId,
            request.UsarHorarioComercial ? request.CalendarioCorporativoId : null,
            request.UsarHorarioComercial,
            request.PausarQuandoAguardandoSolicitante,
            usuarioAtual.Login);

        if (!request.Ativo)
        {
            politica.Desativar(usuarioAtual.Login);
        }

        await politicaRepository.AddAsync(politica, cancellationToken);

        foreach (var metaRequest in request.Metas)
        {
            var meta = new MetaSla(
                politica.Id,
                metaRequest.PrioridadeId,
                metaRequest.TempoPrimeiraRespostaMinutos,
                metaRequest.TempoResolucaoMinutos,
                metaRequest.TempoAtualizacaoMinutos,
                metaRequest.TempoRespostaSubsequenteMinutos,
                usuarioAtual.Login);

            if (!metaRequest.Ativo)
            {
                meta.Desativar(usuarioAtual.Login);
            }

            await metaRepository.AddAsync(meta, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        var politicaCompleta = await PoliticasSlaMapper.CarregarPoliticaPorIdAsync(politicaRepository, politica.Id, cancellationToken)
            ?? throw new InvalidOperationException("Falha ao recarregar a politica de SLA criada.");

        if (auditoriaService is not null)
        {
            await auditoriaService.RegistrarCriacaoAsync(
                "SLA",
                "PoliticaSla",
                politica.Id.ToString(),
                "Politica de SLA criada.",
                dadosDepois: PoliticasSlaMapper.SerializarPoliticaAuditoria(politicaCompleta),
                metadados: AuditoriaDiffHelper.CriarMetadadosPadrao(
                    origem: "api",
                    modulo: "SLA",
                    entidade: "PoliticaSla",
                    entidadeId: politica.Id.ToString(),
                    codigo: politicaCompleta.Nome,
                    nome: politicaCompleta.Descricao,
                    operacao: "CriacaoPoliticaSla",
                    resultado: "Sucesso"),
                cancellationToken: cancellationToken);
        }

        return PoliticasSlaMapper.Map(politicaCompleta);
    }
}

public sealed class AtualizarPoliticaSlaUseCase(
    IRepository<PoliticaSla> politicaRepository,
    IRepository<MetaSla> metaRepository,
    IRepository<PrioridadeChamado> prioridadeRepository,
    IRepository<CategoriaChamado> categoriaRepository,
    IRepository<Departamento> departamentoRepository,
    IRepository<CalendarioCorporativo> calendarioRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null) : IAtualizarPoliticaSlaUseCase
{
    public async Task<PoliticaSlaResponse> ExecutarAsync(Guid id, AtualizarPoliticaSlaRequest request, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id da politica de SLA invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var politica = await politicaRepository.Query()
            .Include(x => x.Metas)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Politica de SLA nao encontrada.");
        var dadosAntesPolitica = PoliticasSlaMapper.SerializarPoliticaAuditoria(politica);

        await PoliticasSlaMapper.ValidarReferenciasAsync(
            request.CategoriaId,
            request.DepartamentoId,
            request.UsarHorarioComercial ? request.CalendarioCorporativoId : null,
            request.Metas,
            prioridadeRepository,
            categoriaRepository,
            departamentoRepository,
            calendarioRepository,
            cancellationToken);

        PoliticasSlaMapper.ValidarDuplicidadePrioridade(request.Metas);

        politica.Atualizar(
            request.Nome,
            request.Descricao,
            request.Ordem,
            request.CategoriaId,
            request.DepartamentoId,
            request.CalendarioCorporativoId,
            request.UsarHorarioComercial,
            request.PausarQuandoAguardandoSolicitante,
            usuarioAtual.Login);

        if (request.Ativo)
        {
            politica.Ativar(usuarioAtual.Login);
        }
        else
        {
            politica.Desativar(usuarioAtual.Login);
        }

        var metasExistentes = politica.Metas.ToDictionary(x => x.Id, x => x);
        var metasRecebidasComId = request.Metas.Where(x => x.Id.HasValue).Select(x => x.Id!.Value).ToHashSet();

        foreach (var meta in politica.Metas.Where(x => !metasRecebidasComId.Contains(x.Id)).ToArray())
        {
            meta.Desativar(usuarioAtual.Login);
            metaRepository.Update(meta);
        }

        foreach (var metaRequest in request.Metas)
        {
            if (metaRequest.Id.HasValue && metasExistentes.TryGetValue(metaRequest.Id.Value, out var metaExistente))
            {
                metaExistente.Atualizar(
                    metaRequest.PrioridadeId,
                    metaRequest.TempoPrimeiraRespostaMinutos,
                    metaRequest.TempoResolucaoMinutos,
                    metaRequest.TempoAtualizacaoMinutos,
                    metaRequest.TempoRespostaSubsequenteMinutos,
                    metaRequest.Ativo,
                    usuarioAtual.Login);

                metaRepository.Update(metaExistente);
                continue;
            }

            var metaNova = new MetaSla(
                politica.Id,
                metaRequest.PrioridadeId,
                metaRequest.TempoPrimeiraRespostaMinutos,
                metaRequest.TempoResolucaoMinutos,
                metaRequest.TempoAtualizacaoMinutos,
                metaRequest.TempoRespostaSubsequenteMinutos,
                usuarioAtual.Login);

            if (!metaRequest.Ativo)
            {
                metaNova.Desativar(usuarioAtual.Login);
            }

            await metaRepository.AddAsync(metaNova, cancellationToken);
        }

        politicaRepository.Update(politica);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var politicaCompleta = await PoliticasSlaMapper.CarregarPoliticaPorIdAsync(politicaRepository, id, cancellationToken)
            ?? throw new InvalidOperationException("Falha ao recarregar a politica de SLA atualizada.");

        if (auditoriaService is not null)
        {
            var dadosDepoisPolitica = PoliticasSlaMapper.SerializarPoliticaAuditoria(politicaCompleta);

            await auditoriaService.RegistrarEdicaoAsync(
                "SLA",
                "PoliticaSla",
                id.ToString(),
                "Politica de SLA atualizada.",
                dadosAntes: dadosAntesPolitica,
                dadosDepois: dadosDepoisPolitica,
                metadados: AuditoriaDiffHelper.CriarMetadadosPadrao(
                    origem: "api",
                    modulo: "SLA",
                    entidade: "PoliticaSla",
                    entidadeId: id.ToString(),
                    codigo: politicaCompleta.Nome,
                    nome: politicaCompleta.Descricao,
                    operacao: "AtualizacaoPoliticaSla",
                    resultado: "Sucesso"),
                cancellationToken: cancellationToken);
        }

        return PoliticasSlaMapper.Map(politicaCompleta);
    }
}

public sealed class AtualizarStatusPoliticaSlaUseCase(
    IRepository<PoliticaSla> politicaRepository,
    IRepository<MetaSla> metaRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null) : IAtualizarStatusPoliticaSlaUseCase
{
    public async Task<PoliticaSlaResponse> ExecutarAsync(
        Guid id,
        AtualizarStatusPoliticaSlaRequest request,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id da politica de SLA invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var politica = await politicaRepository.Query()
            .Include(x => x.Metas)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Politica de SLA nao encontrada.");
        var ativoAntes = politica.Ativo;

        if (request.Ativo)
        {
            politica.Ativar(usuarioAtual.Login);
            foreach (var meta in politica.Metas)
            {
                meta.Ativar(usuarioAtual.Login);
                metaRepository.Update(meta);
            }
        }
        else
        {
            politica.Desativar(usuarioAtual.Login);
            foreach (var meta in politica.Metas)
            {
                meta.Desativar(usuarioAtual.Login);
                metaRepository.Update(meta);
            }
        }

        politicaRepository.Update(politica);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var politicaCompleta = await PoliticasSlaMapper.CarregarPoliticaPorIdAsync(politicaRepository, id, cancellationToken)
            ?? throw new InvalidOperationException("Falha ao recarregar a politica de SLA.");

        if (auditoriaService is not null)
        {
            var descricao = request.Ativo
                ? "Politica de SLA ativada."
                : "Politica de SLA inativada.";

            var metadados = AuditoriaDiffHelper.CriarMetadadosPadrao(
                origem: "api",
                modulo: "SLA",
                entidade: "PoliticaSla",
                entidadeId: id.ToString(),
                codigo: politicaCompleta.Nome,
                nome: politicaCompleta.Descricao,
                operacao: request.Ativo ? "AtivacaoPoliticaSla" : "InativacaoPoliticaSla",
                resultado: "Sucesso",
                observacao: $"Ativo: {ativoAntes} -> {politicaCompleta.Ativo}");

            if (request.Ativo)
            {
                await auditoriaService.RegistrarAtivacaoAsync(
                    "SLA",
                    "PoliticaSla",
                    id.ToString(),
                    descricao,
                    metadados,
                    cancellationToken);
            }
            else
            {
                await auditoriaService.RegistrarInativacaoAsync(
                    "SLA",
                    "PoliticaSla",
                    id.ToString(),
                    descricao,
                    metadados,
                    cancellationToken);
            }
        }

        return PoliticasSlaMapper.Map(politicaCompleta);
    }
}

public sealed class InativarPoliticaSlaUseCase(
    IAtualizarStatusPoliticaSlaUseCase atualizarStatusPoliticaSlaUseCase) : IInativarPoliticaSlaUseCase
{
    public Task<PoliticaSlaResponse> ExecutarAsync(Guid id, CancellationToken cancellationToken = default)
        => atualizarStatusPoliticaSlaUseCase.ExecutarAsync(id, new AtualizarStatusPoliticaSlaRequest { Ativo = false }, cancellationToken);
}

public sealed class ObterConfiguracaoAlertaSlaUseCase(
    IRepository<ConfiguracaoAlertaSla> configuracaoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IObterConfiguracaoAlertaSlaUseCase
{
    public async Task<ConfiguracaoAlertaSlaResponse> ExecutarAsync(CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var configuracao = await CarregarConfiguracaoAsync(configuracaoRepository, cancellationToken)
            ?? throw new KeyNotFoundException("Configuracao de alerta de SLA nao encontrada.");

        return MapConfiguracao(configuracao);
    }

    internal static async Task<ConfiguracaoAlertaSla?> CarregarConfiguracaoAsync(
        IRepository<ConfiguracaoAlertaSla> configuracaoRepository,
        CancellationToken cancellationToken)
        => await configuracaoRepository.Query()
            .OrderByDescending(x => x.Ativo)
            .ThenBy(x => x.CriadoEm)
            .FirstOrDefaultAsync(cancellationToken);

    internal static ConfiguracaoAlertaSlaResponse MapConfiguracao(ConfiguracaoAlertaSla configuracao)
        => new(
            configuracao.Id,
            configuracao.Ativo,
            configuracao.MinutosAntesVencimentoPrimeiraResposta,
            configuracao.MinutosAntesVencimentoResolucao,
            configuracao.NotificarAtendente,
            configuracao.NotificarGestor,
            configuracao.NotificarDepartamento,
            configuracao.CriadoEm,
            configuracao.CriadoPor,
            configuracao.AtualizadoEm,
            configuracao.AtualizadoPor);
}

public sealed class AtualizarConfiguracaoAlertaSlaUseCase(
    IRepository<ConfiguracaoAlertaSla> configuracaoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null) : IAtualizarConfiguracaoAlertaSlaUseCase
{
    public async Task<ConfiguracaoAlertaSlaResponse> ExecutarAsync(
        AtualizarConfiguracaoAlertaSlaRequest request,
        CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var configuracao = await ObterConfiguracaoAlertaSlaUseCase.CarregarConfiguracaoAsync(configuracaoRepository, cancellationToken);
        var nova = configuracao is null;
        configuracao ??= new ConfiguracaoAlertaSla(30, 120, true, false, false, usuarioAtual.Login);
        var dadosAntes = nova ? null : AuditoriaDiffHelper.SerializarSeguro(new
        {
            configuracao.Ativo,
            configuracao.MinutosAntesVencimentoPrimeiraResposta,
            configuracao.MinutosAntesVencimentoResolucao,
            configuracao.NotificarAtendente,
            configuracao.NotificarGestor,
            configuracao.NotificarDepartamento
        });

        if (nova)
        {
            await configuracaoRepository.AddAsync(configuracao, cancellationToken);
        }

        configuracao.Atualizar(
            request.Ativo,
            request.MinutosAntesVencimentoPrimeiraResposta,
            request.MinutosAntesVencimentoResolucao,
            request.NotificarAtendente,
            request.NotificarGestor,
            request.NotificarDepartamento,
            usuarioAtual.Login);

        configuracaoRepository.Update(configuracao);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (auditoriaService is not null)
        {
            var dadosDepois = AuditoriaDiffHelper.SerializarSeguro(new
            {
                configuracao.Ativo,
                configuracao.MinutosAntesVencimentoPrimeiraResposta,
                configuracao.MinutosAntesVencimentoResolucao,
                configuracao.NotificarAtendente,
                configuracao.NotificarGestor,
                configuracao.NotificarDepartamento
            });

            if (nova)
            {
                await auditoriaService.RegistrarCriacaoAsync(
                    "SLA",
                    "ConfiguracaoAlertaSla",
                    configuracao.Id.ToString(),
                    "Configuracao de alerta de SLA criada.",
                    dadosDepois: dadosDepois,
                    metadados: AuditoriaDiffHelper.CriarMetadadosPadrao(
                        origem: "api",
                        modulo: "SLA",
                        entidade: "ConfiguracaoAlertaSla",
                        entidadeId: configuracao.Id.ToString(),
                        operacao: "CriacaoConfiguracaoAlertaSla",
                        resultado: "Sucesso"),
                    cancellationToken: cancellationToken);
            }
            else
            {
                await auditoriaService.RegistrarEdicaoAsync(
                    "SLA",
                    "ConfiguracaoAlertaSla",
                    configuracao.Id.ToString(),
                    "Configuracao de alerta de SLA alterada.",
                    dadosAntes: dadosAntes,
                    dadosDepois: dadosDepois,
                    metadados: AuditoriaDiffHelper.CriarMetadadosPadrao(
                        origem: "api",
                        modulo: "SLA",
                        entidade: "ConfiguracaoAlertaSla",
                        entidadeId: configuracao.Id.ToString(),
                        operacao: "AtualizacaoConfiguracaoAlertaSla",
                        resultado: "Sucesso"),
                    cancellationToken: cancellationToken);
            }
        }

        return ObterConfiguracaoAlertaSlaUseCase.MapConfiguracao(configuracao);
    }
}

public sealed class ObterDashboardSlaUseCase(
    IRepository<Chamado> chamadoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IObterDashboardSlaUseCase
{
    public async Task<SlaDashboardResponse> ExecutarAsync(FiltroDashboardSlaRequest request, CancellationToken cancellationToken = default)
    {
        var chamados = await SlaConsultas.CarregarChamadosComSlaAsync(chamadoRepository, usuarioContextoAplicacaoService, request, cancellationToken);
        var agora = DateTime.UtcNow;
        var total = chamados.Count;
        var vencidos = chamados.Count(x => SlaRules.CalcularSituacao(x.ChamadoSla, agora) is SituacaoSlaChamadoEnum.Vencido or SituacaoSlaChamadoEnum.Violado);
        var proximos = chamados.Count(x => SlaRules.EstaProximoDoVencimento(x.ChamadoSla, agora));
        var cumpridos = chamados.Count(x => SlaRules.CalcularSituacao(x.ChamadoSla, agora) == SituacaoSlaChamadoEnum.Cumprido);
        var violados = chamados.Count(x => SlaRules.CalcularSituacao(x.ChamadoSla, agora) == SituacaoSlaChamadoEnum.Violado);
        var dentro = chamados.Count(x => SlaRules.CalcularSituacao(x.ChamadoSla, agora) == SituacaoSlaChamadoEnum.DentroDoPrazo);
        var resolvidos = chamados.Where(x => x.ChamadoSla!.DataResolucao.HasValue).ToArray();
        var percentualCumprimento = resolvidos.Length == 0
            ? 0
            : Math.Round((decimal)resolvidos.Count(x => x.ChamadoSla!.ResolucaoCumprida == true) * 100 / resolvidos.Length, 2);

        return new SlaDashboardResponse
        {
            TotalComSlaAplicado = total,
            TotalVencidos = vencidos,
            TotalProximosDoVencimento = proximos,
            TotalDentroDoPrazo = dentro,
            TotalCumpridos = cumpridos,
            TotalViolados = violados,
            PercentualCumprimento = percentualCumprimento,
            TempoMedioPrimeiraRespostaMinutos = Media(chamados.Select(x => x.ChamadoSla!.MinutosPrimeiraResposta)),
            TempoMedioResolucaoMinutos = Media(chamados.Select(x => x.ChamadoSla!.MinutosResolucao)),
            PorPrioridade = Agrupar(chamados, x => x.PrioridadeId, x => x.Prioridade.Nome, agora),
            PorCategoria = Agrupar(chamados, x => x.CategoriaId, x => x.Categoria.Nome, agora),
            PorDepartamento = Agrupar(chamados, x => x.DepartamentoId, x => x.Departamento?.Nome ?? "Sem departamento", agora)
        };
    }

    private static double? Media(IEnumerable<int?> valores)
    {
        var materializado = valores.Where(x => x.HasValue).Select(x => (double)x!.Value).ToArray();
        return materializado.Length == 0 ? null : Math.Round(materializado.Average(), 2);
    }

    private static IReadOnlyCollection<SlaAgrupamentoResponse> Agrupar(
        IReadOnlyCollection<Chamado> chamados,
        Func<Chamado, Guid?> idSelector,
        Func<Chamado, string> nomeSelector,
        DateTime agora)
        => chamados
            .GroupBy(x => new { Id = idSelector(x), Nome = nomeSelector(x) })
            .OrderByDescending(x => x.Count())
            .ThenBy(x => x.Key.Nome)
            .Select(x => new SlaAgrupamentoResponse(
                x.Key.Id,
                x.Key.Nome,
                x.Count(),
                x.Count(chamado => SlaRules.CalcularSituacao(chamado.ChamadoSla, agora) is SituacaoSlaChamadoEnum.Vencido or SituacaoSlaChamadoEnum.Violado),
                x.Count(chamado => SlaRules.EstaProximoDoVencimento(chamado.ChamadoSla, agora)),
                x.Count(chamado => SlaRules.CalcularSituacao(chamado.ChamadoSla, agora) == SituacaoSlaChamadoEnum.Cumprido),
                x.Count(chamado => SlaRules.CalcularSituacao(chamado.ChamadoSla, agora) == SituacaoSlaChamadoEnum.Violado)))
            .ToArray();
}

public sealed class ListarRelatorioSlaUseCase(
    IRepository<Chamado> chamadoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IListarRelatorioSlaUseCase
{
    public async Task<IReadOnlyCollection<SlaRelatorioItemResponse>> ExecutarAsync(
        FiltroDashboardSlaRequest request,
        CancellationToken cancellationToken = default)
    {
        var chamados = await SlaConsultas.CarregarChamadosComSlaAsync(chamadoRepository, usuarioContextoAplicacaoService, request, cancellationToken);
        var agora = DateTime.UtcNow;

        return chamados
            .OrderBy(x => x.Codigo)
            .Select(x => new SlaRelatorioItemResponse(
                x.Id,
                x.Codigo,
                x.Titulo,
                x.Prioridade.Nome,
                x.Categoria.Nome,
                x.Departamento?.Nome,
                x.ChamadoSla!.PoliticaSla?.Nome,
                x.ChamadoSla.PrazoPrimeiraResposta,
                x.ChamadoSla.DataPrimeiraResposta,
                x.ChamadoSla.PrimeiraRespostaCumprida,
                x.ChamadoSla.PrazoResolucao,
                x.ChamadoSla.DataResolucao,
                x.ChamadoSla.ResolucaoCumprida,
                SlaRules.CalcularSituacao(x.ChamadoSla, agora),
                x.ChamadoSla.MinutosPrimeiraResposta,
                x.ChamadoSla.MinutosResolucao,
                x.ChamadoSla.MinutosPausados))
            .ToArray();
    }
}

internal static class SlaConsultas
{
    public static async Task<IReadOnlyCollection<Chamado>> CarregarChamadosComSlaAsync(
        IRepository<Chamado> chamadoRepository,
        IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
        FiltroDashboardSlaRequest request,
        CancellationToken cancellationToken)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var query = chamadoRepository.Query()
            .AsNoTracking()
            .Include(x => x.Prioridade)
            .Include(x => x.Categoria)
            .Include(x => x.Departamento)
            .Include(x => x.ChamadoSla).ThenInclude(x => x.PoliticaSla)
            .Include(x => x.ChamadoSla).ThenInclude(x => x.CalendarioCorporativo)
            .Where(x => x.Ativo && x.ChamadoSla != null)
            .AsQueryable();

        if (request.DataInicio.HasValue)
        {
            query = query.Where(x => x.AbertoEm >= request.DataInicio.Value);
        }

        if (request.DataFim.HasValue)
        {
            var fimExclusivo = request.DataFim.Value.Date.AddDays(1);
            query = query.Where(x => x.AbertoEm < fimExclusivo);
        }

        if (request.PrioridadeId.HasValue)
        {
            query = query.Where(x => x.PrioridadeId == request.PrioridadeId.Value);
        }

        if (request.CategoriaId.HasValue)
        {
            query = query.Where(x => x.CategoriaId == request.CategoriaId.Value);
        }

        if (request.DepartamentoId.HasValue)
        {
            query = query.Where(x => x.DepartamentoId == request.DepartamentoId.Value);
        }

        var chamados = await query.ToListAsync(cancellationToken);
        if (request.SituacaoSla.HasValue)
        {
            var agora = DateTime.UtcNow;
            chamados = chamados
                .Where(x => SlaRules.CalcularSituacao(x.ChamadoSla, agora) == request.SituacaoSla.Value)
                .ToList();
        }

        return chamados;
    }
}

internal static class PoliticasSlaMapper
{
    public static PoliticaSlaResponse Map(PoliticaSla politica)
        => new(
            politica.Id,
            politica.Nome,
            politica.Descricao,
            politica.Ativo,
            politica.Ordem,
            politica.CategoriaId,
            politica.Categoria?.Nome,
            politica.DepartamentoId,
            politica.Departamento?.Nome,
            politica.UsarHorarioComercial,
            politica.CalendarioCorporativoId,
            politica.CalendarioCorporativo?.Nome,
            politica.PausarQuandoAguardandoSolicitante,
            politica.CriadoEm,
            politica.CriadoPor,
            politica.AtualizadoEm,
            politica.AtualizadoPor,
            politica.Metas
                .OrderBy(x => x.Prioridade.Nivel)
                .ThenBy(x => x.Prioridade.Nome)
                .Select(MapMeta)
                .ToArray());

    public static MetaSlaResponse MapMeta(MetaSla meta)
        => new(
            meta.Id,
            meta.PrioridadeId,
            meta.Prioridade.Nome,
            (int)meta.Prioridade.Nivel,
            meta.TempoPrimeiraRespostaMinutos,
            meta.TempoResolucaoMinutos,
            meta.TempoAtualizacaoMinutos,
            meta.TempoRespostaSubsequenteMinutos,
            meta.Ativo);

    public static async Task<PoliticaSla?> CarregarPoliticaPorIdAsync(
        IRepository<PoliticaSla> politicaRepository,
        Guid id,
        CancellationToken cancellationToken)
    {
        return await politicaRepository.Query()
            .AsNoTracking()
            .Include(x => x.Categoria)
            .Include(x => x.Departamento)
            .Include(x => x.CalendarioCorporativo)
            .Include(x => x.Metas)
            .ThenInclude(x => x.Prioridade)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public static string? SerializarPoliticaAuditoria(PoliticaSla politica)
        => AuditoriaDiffHelper.SerializarSeguro(new
        {
            politica.Nome,
            politica.Descricao,
            politica.Ativo,
            politica.Ordem,
            politica.CategoriaId,
            politica.DepartamentoId,
            politica.CalendarioCorporativoId,
            politica.UsarHorarioComercial,
            politica.PausarQuandoAguardandoSolicitante,
            Metas = politica.Metas
                .OrderBy(x => x.PrioridadeId)
                .Select(x => new
                {
                    x.Id,
                    x.PrioridadeId,
                    x.TempoPrimeiraRespostaMinutos,
                    x.TempoResolucaoMinutos,
                    x.TempoAtualizacaoMinutos,
                    x.TempoRespostaSubsequenteMinutos,
                    x.Ativo
                })
                .ToArray()
        });

    public static void ValidarDuplicidadePrioridade(IReadOnlyCollection<MetaSlaUpsertRequest> metas)
    {
        if (metas.Count == 0)
        {
            throw new InvalidOperationException("A politica de SLA deve possuir ao menos uma meta.");
        }

        var prioridades = metas
            .Where(x => x.Ativo)
            .Select(x => x.PrioridadeId)
            .ToArray();

        var duplicadas = prioridades
            .GroupBy(x => x)
            .Where(x => x.Count() > 1)
            .Select(x => x.Key)
            .ToArray();

        if (duplicadas.Length > 0)
        {
            throw new InvalidOperationException("Nao e permitido repetir prioridade na mesma politica de SLA.");
        }
    }

    public static async Task ValidarReferenciasAsync(
        Guid? categoriaId,
        Guid? departamentoId,
        Guid? calendarioCorporativoId,
        IReadOnlyCollection<MetaSlaUpsertRequest> metas,
        IRepository<PrioridadeChamado> prioridadeRepository,
        IRepository<CategoriaChamado> categoriaRepository,
        IRepository<Departamento> departamentoRepository,
        IRepository<CalendarioCorporativo> calendarioRepository,
        CancellationToken cancellationToken)
    {
        if (categoriaId.HasValue)
        {
            var categoriaExiste = await categoriaRepository.Query()
                .AsNoTracking()
                .AnyAsync(x => x.Id == categoriaId.Value && x.Ativo, cancellationToken);

            if (!categoriaExiste)
            {
                throw new InvalidOperationException("Categoria informada para a politica de SLA nao existe ou esta inativa.");
            }
        }

        if (departamentoId.HasValue)
        {
            var departamentoExiste = await departamentoRepository.Query()
                .AsNoTracking()
                .AnyAsync(x => x.Id == departamentoId.Value && x.Ativo, cancellationToken);

            if (!departamentoExiste)
            {
                throw new InvalidOperationException("Departamento informado para a politica de SLA nao existe ou esta inativo.");
            }
        }

        if (calendarioCorporativoId.HasValue)
        {
            var calendarioExiste = await calendarioRepository.Query()
                .AsNoTracking()
                .AnyAsync(x => x.Id == calendarioCorporativoId.Value && x.Ativo, cancellationToken);

            if (!calendarioExiste)
            {
                throw new InvalidOperationException("Calendario corporativo informado para a politica de SLA nao existe ou esta inativo.");
            }
        }

        var prioridadeIds = metas.Select(x => x.PrioridadeId).Distinct().ToArray();
        var prioridadesAtivas = await prioridadeRepository.Query()
            .AsNoTracking()
            .Where(x => x.Ativo && prioridadeIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        if (prioridadesAtivas.Count != prioridadeIds.Length)
        {
            throw new InvalidOperationException("Uma ou mais prioridades informadas para metas de SLA nao existem ou estao inativas.");
        }
    }
}
