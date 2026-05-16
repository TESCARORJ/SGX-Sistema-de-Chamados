using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Portal;
using SGX.SistemaChamado.Application.Helpers;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Auditoria;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Interfaces.Portal;
using SGX.SistemaChamado.Application.Interfaces.Sla;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Portal;

public sealed class AbrirChamadoUseCase(
    IRepository<Chamado> chamadoRepository,
    IRepository<CategoriaChamado> categoriaRepository,
    IRepository<SubcategoriaChamado> subcategoriaRepository,
    IRepository<PrioridadeChamado> prioridadeRepository,
    IRepository<TipoSolicitacao> tipoSolicitacaoRepository,
    IRepository<LocalUnidade> localUnidadeRepository,
    IRepository<Departamento> departamentoRepository,
    IRepository<StatusChamado> statusRepository,
    IRepository<HistoricoChamado> historicoRepository,
    ISlaService slaService,
    ICodigoChamadoService codigoChamadoService,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null) : IAbrirChamadoUseCase
{
    private const string DescricaoHistoricoCriacaoPortal = "Chamado criado pelo portal";

    public async Task<ChamadoDetalheResponse> ExecutarAsync(CriarChamadoRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.CategoriaId == Guid.Empty)
        {
            throw new InvalidOperationException("Categoria obrigatoria.");
        }

        if (request.PrioridadeId == Guid.Empty)
        {
            throw new InvalidOperationException("Prioridade obrigatoria.");
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);

        var categoria = await categoriaRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.CategoriaId && x.Ativo, cancellationToken)
            ?? throw new InvalidOperationException("Categoria nao encontrada ou inativa.");

        var prioridade = await prioridadeRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.PrioridadeId && x.Ativo, cancellationToken)
            ?? throw new InvalidOperationException("Prioridade nao encontrada ou inativa.");

        SubcategoriaChamado? subcategoria = null;
        if (request.SubcategoriaId.HasValue)
        {
            subcategoria = await subcategoriaRepository.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.SubcategoriaId.Value && x.Ativo, cancellationToken)
                ?? throw new InvalidOperationException("Subcategoria nao encontrada ou inativa.");

            if (subcategoria.CategoriaChamadoId != categoria.Id)
            {
                throw new InvalidOperationException("A subcategoria selecionada nao pertence a categoria informada.");
            }
        }

        if (request.TipoSolicitacaoId.HasValue)
        {
            _ = await tipoSolicitacaoRepository.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.TipoSolicitacaoId.Value && x.Ativo, cancellationToken)
                ?? throw new InvalidOperationException("Tipo de solicitacao nao encontrado ou inativo.");
        }

        if (request.LocalUnidadeId.HasValue)
        {
            _ = await localUnidadeRepository.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.LocalUnidadeId.Value && x.Ativo, cancellationToken)
                ?? throw new InvalidOperationException("Local/unidade nao encontrado ou inativo.");
        }

        if (request.DepartamentoId.HasValue)
        {
            _ = await departamentoRepository.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.DepartamentoId.Value && x.Ativo, cancellationToken)
                ?? throw new InvalidOperationException("Departamento nao encontrado ou inativo.");
        }

        var statusAberto = await statusRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Ativo && x.Codigo == StatusChamadoEnum.Aberto, cancellationToken)
            ?? throw new InvalidOperationException("Status inicial 'Aberto' nao encontrado.");

        var codigo = await codigoChamadoService.GerarAsync(cancellationToken);
        var chamado = new Chamado(
            codigo,
            request.Titulo,
            request.Descricao,
            usuarioAtual.Id,
            request.CategoriaId,
            request.PrioridadeId,
            statusAberto.Id,
            OrigemChamado.Portal,
            usuarioAtual.Login,
            request.DepartamentoId,
            subcategoria?.Id,
            request.TipoSolicitacaoId,
            request.LocalUnidadeId);

        await chamadoRepository.AddAsync(chamado, cancellationToken);

        var historicoCriado = new HistoricoChamado(
            chamado.Id,
            TipoHistoricoChamado.Criado,
            DescricaoHistoricoCriacaoPortal,
            usuarioAtual.Id,
            usuarioAtual.Login);

        await historicoRepository.AddAsync(historicoCriado, cancellationToken);

        await slaService.InicializarNaAberturaAsync(chamado, usuarioAtual.Login, DateTime.UtcNow, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var chamadoCriado = await chamadoRepository.Query()
            .Include(x => x.Status)
            .Include(x => x.Prioridade)
            .Include(x => x.Categoria)
            .Include(x => x.Subcategoria)
            .Include(x => x.TipoSolicitacao)
            .Include(x => x.LocalUnidade)
            .Include(x => x.Departamento)
            .Include(x => x.Solicitante)
            .Include(x => x.Responsavel)
            .Include(x => x.Comentarios).ThenInclude(x => x.Usuario)
            .Include(x => x.Anexos).ThenInclude(x => x.Usuario)
            .Include(x => x.Historicos).ThenInclude(x => x.Usuario)
            .Include(x => x.ChamadoSla).ThenInclude(x => x.PoliticaSla)
            .Include(x => x.ChamadoSla).ThenInclude(x => x.CalendarioCorporativo)
            .FirstAsync(x => x.Id == chamado.Id, cancellationToken);

        if (auditoriaService is not null)
        {
            var dadosDepois = AuditoriaDiffHelper.SerializarSeguro(new
            {
                chamadoCriado.Id,
                chamadoCriado.Codigo,
                chamadoCriado.Titulo,
                Status = chamadoCriado.Status.Nome,
                Prioridade = chamadoCriado.Prioridade.Nome,
                Categoria = chamadoCriado.Categoria.Nome,
                Subcategoria = chamadoCriado.Subcategoria?.Nome,
                TipoSolicitacao = chamadoCriado.TipoSolicitacao?.Nome,
                LocalUnidade = chamadoCriado.LocalUnidade?.Nome,
                chamadoCriado.DepartamentoId,
                SolicitanteId = chamadoCriado.SolicitanteId
            });

            await auditoriaService.RegistrarCriacaoAsync(
                "Chamados",
                "Chamado",
                chamadoCriado.Id.ToString(),
                "Chamado aberto.",
                dadosDepois: dadosDepois,
                metadados: AuditoriaDiffHelper.CriarMetadadosPadrao(
                    origem: "api",
                    modulo: "Chamados",
                    entidade: "Chamado",
                    entidadeId: chamadoCriado.Id.ToString(),
                    codigo: chamadoCriado.Codigo,
                    nome: chamadoCriado.Titulo,
                    operacao: "Abertura",
                    resultado: "Sucesso",
                    observacao: "Abertura via portal"),
                cancellationToken: cancellationToken);
        }

        return PortalUseCaseHelpers.MapDetalhe(chamadoCriado, usuarioAtual);
    }
}
