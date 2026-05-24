using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Helpers;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Auditoria;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public sealed class VincularInventarioAtivoChamadoUseCase(
    IRepository<Chamado> chamadoRepository,
    IRepository<InventarioAtivo> inventarioAtivoRepository,
    IRepository<HistoricoChamado> historicoChamadoRepository,
    IRepository<HistoricoInventarioAtivo> historicoInventarioAtivoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null) : IVincularInventarioAtivoChamadoUseCase
{
    public async Task<ChamadoAdminDetalheResponse> ExecutarAsync(Guid chamadoId, Guid ativoId, CancellationToken cancellationToken = default)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("Id do chamado invalido.", nameof(chamadoId));
        }

        if (ativoId == Guid.Empty)
        {
            throw new ArgumentException("Id do ativo invalido.", nameof(ativoId));
        }

        var usuario = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        if (!AdminUseCaseHelpers.PodeOperarAdmin(usuario))
        {
            throw new UnauthorizedAccessException("Acesso administrativo negado.");
        }

        var chamado = await chamadoRepository.Query()
            .Include(x => x.InventarioAtivo)
            .FirstOrDefaultAsync(x => x.Id == chamadoId && x.Ativo, cancellationToken)
            ?? throw new KeyNotFoundException("Chamado nao encontrado.");

        var ativo = await inventarioAtivoRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == ativoId, cancellationToken)
            ?? throw new KeyNotFoundException("Ativo de inventario nao encontrado.");

        if (!ativo.Ativo)
        {
            throw new InvalidOperationException("Ativo de inventario inativo nao pode ser vinculado ao chamado.");
        }

        if (chamado.InventarioAtivoId == ativoId)
        {
            var semMudanca = await AdminChamadoLoader.QueryDetalhe(chamadoRepository.Query().AsNoTracking())
                .FirstAsync(x => x.Id == chamadoId, cancellationToken);

            return AdminUseCaseHelpers.MapDetalhe(semMudanca);
        }

        var (dadosAntes, dadosDepois) = AuditoriaDiffHelper.CriarDiff(
            new
            {
                chamado.InventarioAtivoId,
                InventarioAtivoCodigo = chamado.InventarioAtivo?.Codigo,
                InventarioAtivoNome = chamado.InventarioAtivo?.Nome
            },
            new
            {
                InventarioAtivoId = ativo.Id,
                InventarioAtivoCodigo = ativo.Codigo,
                InventarioAtivoNome = ativo.Nome
            });

        chamado.VincularInventarioAtivo(ativo.Id, usuario.Login);
        chamadoRepository.Update(chamado);

        var descricao = CriarDescricaoHistoricoAtivoVinculado(ativo);
        var historicoChamado = new HistoricoChamado(
            chamado.Id,
            TipoHistoricoChamado.AtivoVinculado,
            descricao,
            usuario.Id,
            usuario.Login);

        await historicoChamadoRepository.AddAsync(historicoChamado, cancellationToken);

        var historicoAtivo = new HistoricoInventarioAtivo(
            ativo.Id,
            TipoMovimentacaoAtivo.VinculoChamado,
            usuario.Id,
            usuario.Login,
            $"Chamado {chamado.Codigo} vinculado ao ativo ({chamado.Titulo}).");

        await historicoInventarioAtivoRepository.AddAsync(historicoAtivo, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var atualizado = await AdminChamadoLoader.QueryDetalhe(chamadoRepository.Query().AsNoTracking())
            .FirstAsync(x => x.Id == chamadoId, cancellationToken);

        if (auditoriaService is not null)
        {
            await auditoriaService.RegistrarEdicaoAsync(
                "Chamados",
                "Chamado",
                chamadoId.ToString(),
                "Ativo vinculado ao chamado.",
                dadosAntes: dadosAntes,
                dadosDepois: dadosDepois,
                metadados: AuditoriaDiffHelper.CriarMetadadosPadrao(
                    origem: "api",
                    modulo: "Chamados",
                    entidade: "Chamado",
                    entidadeId: chamadoId.ToString(),
                    codigo: atualizado.Codigo,
                    nome: atualizado.Titulo,
                    operacao: "VincularAtivoInventario",
                    resultado: "Sucesso",
                    observacao: $"Ativo vinculado: {ativo.Codigo} - {ativo.Nome}"),
                cancellationToken: cancellationToken);
        }

        return AdminUseCaseHelpers.MapDetalhe(atualizado);
    }

    private static string CriarDescricaoHistoricoAtivoVinculado(InventarioAtivo ativo)
    {
        var descricao = $"Ativo vinculado ao chamado: {ativo.Codigo} - {ativo.Nome}";
        if (!string.IsNullOrWhiteSpace(ativo.NumeroPatrimonio))
        {
            descricao += $" (Patrimonio: {ativo.NumeroPatrimonio})";
        }

        return descricao;
    }
}

public sealed class RemoverInventarioAtivoChamadoUseCase(
    IRepository<Chamado> chamadoRepository,
    IRepository<InventarioAtivo> inventarioAtivoRepository,
    IRepository<HistoricoChamado> historicoChamadoRepository,
    IRepository<HistoricoInventarioAtivo> historicoInventarioAtivoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null) : IRemoverInventarioAtivoChamadoUseCase
{
    public async Task<ChamadoAdminDetalheResponse> ExecutarAsync(Guid chamadoId, CancellationToken cancellationToken = default)
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
            .Include(x => x.InventarioAtivo)
            .FirstOrDefaultAsync(x => x.Id == chamadoId && x.Ativo, cancellationToken)
            ?? throw new KeyNotFoundException("Chamado nao encontrado.");

        if (!chamado.InventarioAtivoId.HasValue)
        {
            var semVinculo = await AdminChamadoLoader.QueryDetalhe(chamadoRepository.Query().AsNoTracking())
                .FirstAsync(x => x.Id == chamadoId, cancellationToken);

            return AdminUseCaseHelpers.MapDetalhe(semVinculo);
        }

        var ativoAnterior = await inventarioAtivoRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == chamado.InventarioAtivoId.Value, cancellationToken);

        var (dadosAntes, dadosDepois) = AuditoriaDiffHelper.CriarDiff(
            new
            {
                chamado.InventarioAtivoId,
                InventarioAtivoCodigo = chamado.InventarioAtivo?.Codigo,
                InventarioAtivoNome = chamado.InventarioAtivo?.Nome
            },
            new
            {
                InventarioAtivoId = (Guid?)null,
                InventarioAtivoCodigo = (string?)null,
                InventarioAtivoNome = (string?)null
            });

        chamado.RemoverVinculoInventarioAtivo(usuario.Login);
        chamadoRepository.Update(chamado);

        var descricao = CriarDescricaoHistoricoAtivoRemovido(ativoAnterior);
        var historicoChamado = new HistoricoChamado(
            chamado.Id,
            TipoHistoricoChamado.AtivoRemovido,
            descricao,
            usuario.Id,
            usuario.Login);

        await historicoChamadoRepository.AddAsync(historicoChamado, cancellationToken);

        if (ativoAnterior is not null)
        {
            var historicoAtivo = new HistoricoInventarioAtivo(
                ativoAnterior.Id,
                TipoMovimentacaoAtivo.RemocaoVinculoChamado,
                usuario.Id,
                usuario.Login,
                $"Vinculo com chamado {chamado.Codigo} removido ({chamado.Titulo}).");

            await historicoInventarioAtivoRepository.AddAsync(historicoAtivo, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var atualizado = await AdminChamadoLoader.QueryDetalhe(chamadoRepository.Query().AsNoTracking())
            .FirstAsync(x => x.Id == chamadoId, cancellationToken);

        if (auditoriaService is not null)
        {
            await auditoriaService.RegistrarEdicaoAsync(
                "Chamados",
                "Chamado",
                chamadoId.ToString(),
                "Vinculo de ativo removido do chamado.",
                dadosAntes: dadosAntes,
                dadosDepois: dadosDepois,
                metadados: AuditoriaDiffHelper.CriarMetadadosPadrao(
                    origem: "api",
                    modulo: "Chamados",
                    entidade: "Chamado",
                    entidadeId: chamadoId.ToString(),
                    codigo: atualizado.Codigo,
                    nome: atualizado.Titulo,
                    operacao: "RemoverVinculoAtivoInventario",
                    resultado: "Sucesso",
                    observacao: ativoAnterior is null
                        ? "Vinculo removido."
                        : $"Ativo removido: {ativoAnterior.Codigo} - {ativoAnterior.Nome}"),
                cancellationToken: cancellationToken);
        }

        return AdminUseCaseHelpers.MapDetalhe(atualizado);
    }

    private static string CriarDescricaoHistoricoAtivoRemovido(InventarioAtivo? ativoAnterior)
    {
        if (ativoAnterior is null)
        {
            return "Vinculo de ativo removido do chamado.";
        }

        var descricao = $"Ativo removido do chamado: {ativoAnterior.Codigo} - {ativoAnterior.Nome}";
        if (!string.IsNullOrWhiteSpace(ativoAnterior.NumeroPatrimonio))
        {
            descricao += $" (Patrimonio: {ativoAnterior.NumeroPatrimonio})";
        }

        return descricao;
    }
}
