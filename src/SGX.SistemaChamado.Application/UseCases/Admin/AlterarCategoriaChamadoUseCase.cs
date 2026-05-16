using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Helpers;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Auditoria;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Interfaces.Sla;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public sealed class AlterarCategoriaChamadoUseCase(
    IRepository<Chamado> chamadoRepository,
    IRepository<CategoriaChamado> categoriaRepository,
    IRepository<SubcategoriaChamado> subcategoriaRepository,
    IRepository<TipoSolicitacao> tipoSolicitacaoRepository,
    IRepository<LocalUnidade> localUnidadeRepository,
    IRepository<Departamento> departamentoRepository,
    IRepository<HistoricoChamado> historicoRepository,
    ISlaService slaService,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null) : IAlterarCategoriaChamadoUseCase
{
    public async Task<ChamadoAdminDetalheResponse> ExecutarAsync(Guid chamadoId, AlterarCategoriaChamadoRequest request, CancellationToken cancellationToken = default)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("Id do chamado invalido.", nameof(chamadoId));
        }

        var usuario = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        if (!AdminUseCaseHelpers.PodeOperarAdmin(usuario))
        {
            throw new UnauthorizedAccessException("Acesso administrativo negado.");
        }

        var chamado = await chamadoRepository.Query()
            .Include(x => x.Categoria)
            .Include(x => x.Subcategoria)
            .Include(x => x.TipoSolicitacao)
            .Include(x => x.LocalUnidade)
            .Include(x => x.Departamento)
            .Include(x => x.ChamadoSla)
            .FirstOrDefaultAsync(x => x.Id == chamadoId && x.Ativo, cancellationToken)
            ?? throw new KeyNotFoundException("Chamado nao encontrado.");
        var categoriaAnterior = chamado.Categoria?.Nome;
        var subcategoriaAnterior = chamado.Subcategoria?.Nome;
        var tipoSolicitacaoAnterior = chamado.TipoSolicitacao?.Nome;
        var localUnidadeAnterior = chamado.LocalUnidade?.Nome;
        var departamentoAnterior = chamado.Departamento?.Nome;

        var categoria = await categoriaRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.CategoriaId && x.Ativo, cancellationToken)
            ?? throw new InvalidOperationException("Categoria informada nao encontrada ou inativa.");

        var departamentoId = request.DepartamentoId ?? categoria.DepartamentoId;
        if (departamentoId.HasValue)
        {
            _ = await departamentoRepository.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == departamentoId.Value && x.Ativo, cancellationToken)
                ?? throw new InvalidOperationException("Departamento informado nao encontrado ou inativo.");
        }

        Guid? subcategoriaId = null;
        if (request.SubcategoriaId.HasValue)
        {
            var subcategoria = await subcategoriaRepository.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.SubcategoriaId.Value && x.Ativo, cancellationToken)
                ?? throw new InvalidOperationException("Subcategoria informada nao encontrada ou inativa.");

            if (subcategoria.CategoriaChamadoId != categoria.Id)
            {
                throw new InvalidOperationException("A subcategoria selecionada nao pertence a categoria informada.");
            }

            subcategoriaId = subcategoria.Id;
        }

        if (request.TipoSolicitacaoId.HasValue)
        {
            _ = await tipoSolicitacaoRepository.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.TipoSolicitacaoId.Value && x.Ativo, cancellationToken)
                ?? throw new InvalidOperationException("Tipo de solicitacao informado nao encontrado ou inativo.");
        }

        if (request.LocalUnidadeId.HasValue)
        {
            _ = await localUnidadeRepository.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.LocalUnidadeId.Value && x.Ativo, cancellationToken)
                ?? throw new InvalidOperationException("Local/unidade informado nao encontrado ou inativo.");
        }

        chamado.AlterarCategoria(categoria.Id, departamentoId, usuario.Login);
        chamado.AlterarClassificacaoOperacional(
            subcategoriaId,
            request.TipoSolicitacaoId,
            request.LocalUnidadeId,
            departamentoId,
            usuario.Login);
        await slaService.AplicarMudancaCategoriaAsync(chamado, usuario.Login, DateTime.UtcNow, cancellationToken);
        chamadoRepository.Update(chamado);

        var historico = new HistoricoChamado(
            chamado.Id,
            TipoHistoricoChamado.CategoriaAlterada,
            AdminUseCaseHelpers.ObterDescricaoHistorico(TipoHistoricoChamado.CategoriaAlterada, $"Categoria alterada para {categoria.Nome}"),
            usuario.Id,
            usuario.Login);

        await historicoRepository.AddAsync(historico, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var atualizado = await AdminChamadoLoader.QueryDetalhe(chamadoRepository.Query().AsNoTracking())
            .FirstAsync(x => x.Id == chamadoId, cancellationToken);

        if (auditoriaService is not null)
        {
            var (dadosAntes, dadosDepois) = AuditoriaDiffHelper.CriarDiff(
                new
                {
                    Categoria = categoriaAnterior,
                    Subcategoria = subcategoriaAnterior,
                    TipoSolicitacao = tipoSolicitacaoAnterior,
                    LocalUnidade = localUnidadeAnterior,
                    Departamento = departamentoAnterior
                },
                new
                {
                    Categoria = atualizado.Categoria,
                    atualizado.Subcategoria,
                    atualizado.TipoSolicitacao,
                    atualizado.LocalUnidade,
                    atualizado.Departamento
                });

            await auditoriaService.RegistrarEdicaoAsync(
                "Chamados",
                "Chamado",
                chamadoId.ToString(),
                "Categoria do chamado alterada.",
                dadosAntes: dadosAntes,
                dadosDepois: dadosDepois,
                metadados: AuditoriaDiffHelper.CriarMetadadosPadrao(
                    origem: "api",
                    modulo: "Chamados",
                    entidade: "Chamado",
                    entidadeId: chamadoId.ToString(),
                    codigo: atualizado.Codigo,
                    nome: atualizado.Titulo,
                    operacao: "AlteracaoCategoria",
                    resultado: "Sucesso",
                    observacao: $"Categoria atual: {atualizado.Categoria}"),
                cancellationToken: cancellationToken);
        }

        return AdminUseCaseHelpers.MapDetalhe(atualizado);
    }
}
