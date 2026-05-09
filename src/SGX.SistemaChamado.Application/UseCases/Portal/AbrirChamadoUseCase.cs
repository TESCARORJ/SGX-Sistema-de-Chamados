using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Portal;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Interfaces.Portal;
using SGX.SistemaChamado.Application.Interfaces.Sla;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Portal;

public sealed class AbrirChamadoUseCase(
    IRepository<Chamado> chamadoRepository,
    IRepository<CategoriaChamado> categoriaRepository,
    IRepository<PrioridadeChamado> prioridadeRepository,
    IRepository<Departamento> departamentoRepository,
    IRepository<StatusChamado> statusRepository,
    IRepository<HistoricoChamado> historicoRepository,
    ISlaService slaService,
    ICodigoChamadoService codigoChamadoService,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IAbrirChamadoUseCase
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
            request.DepartamentoId);

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
            .Include(x => x.Departamento)
            .Include(x => x.Solicitante)
            .Include(x => x.Responsavel)
            .Include(x => x.Comentarios).ThenInclude(x => x.Usuario)
            .Include(x => x.Anexos).ThenInclude(x => x.Usuario)
            .Include(x => x.Historicos).ThenInclude(x => x.Usuario)
            .Include(x => x.SlaControle)
            .FirstAsync(x => x.Id == chamado.Id, cancellationToken);

        return PortalUseCaseHelpers.MapDetalhe(chamadoCriado, usuarioAtual);
    }
}
