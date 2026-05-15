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

public sealed class ReabrirChamadoUseCase(
    IRepository<Chamado> chamadoRepository,
    IRepository<StatusChamado> statusRepository,
    IRepository<HistoricoChamado> historicoRepository,
    IRepository<ComentarioChamado> comentarioRepository,
    ISlaService slaService,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null) : IReabrirChamadoUseCase
{
    public async Task<ChamadoAdminDetalheResponse> ExecutarAsync(Guid chamadoId, ReabrirChamadoRequest request, CancellationToken cancellationToken = default)
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
            .Include(x => x.Status)
            .Include(x => x.ChamadoSla)
            .FirstOrDefaultAsync(x => x.Id == chamadoId && x.Ativo, cancellationToken)
            ?? throw new KeyNotFoundException("Chamado nao encontrado.");
        var statusAnterior = chamado.Status.Nome;
        var encerradoAnterior = chamado.EncerradoEm;

        var podeReabrir = chamado.EncerradoEm.HasValue ||
            chamado.Status.Codigo is StatusChamadoEnum.Encerrado or StatusChamadoEnum.Resolvido;

        if (!podeReabrir)
        {
            throw new InvalidOperationException("Somente chamados encerrados ou resolvidos podem ser reabertos.");
        }

        var statusEmAtendimento = await statusRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Ativo && x.Codigo == StatusChamadoEnum.EmAtendimento, cancellationToken)
            ?? throw new InvalidOperationException("Status Em Atendimento nao configurado.");

        chamado.Reabrir(statusEmAtendimento.Id, usuario.Login);
        await slaService.ReabrirAsync(chamado, usuario.Login, DateTime.UtcNow, cancellationToken);
        chamadoRepository.Update(chamado);

        if (!string.IsNullOrWhiteSpace(request.Mensagem))
        {
            var comentario = new ComentarioChamado(
                chamado.Id,
                usuario.Id,
                request.Mensagem,
                false,
                usuario.Login);

            await comentarioRepository.AddAsync(comentario, cancellationToken);
        }

        var historico = new HistoricoChamado(
            chamado.Id,
            TipoHistoricoChamado.Reaberto,
            AdminUseCaseHelpers.ObterDescricaoHistorico(TipoHistoricoChamado.Reaberto, "Chamado reaberto"),
            usuario.Id,
            usuario.Login);

        await historicoRepository.AddAsync(historico, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var atualizado = await AdminChamadoLoader.QueryDetalhe(chamadoRepository.Query().AsNoTracking())
            .FirstAsync(x => x.Id == chamadoId, cancellationToken);

        if (auditoriaService is not null)
        {
            await auditoriaService.RegistrarEdicaoAsync(
                "Chamados",
                "Chamado",
                chamadoId.ToString(),
                "Chamado reaberto.",
                dadosAntes: AuditoriaDiffHelper.SerializarSeguro(new
                {
                    Status = statusAnterior,
                    EncerradoEm = encerradoAnterior
                }),
                dadosDepois: AuditoriaDiffHelper.SerializarSeguro(new
                {
                    Status = atualizado.Status,
                    EncerradoEm = atualizado.EncerradoEm,
                    TamanhoMensagem = request.Mensagem?.Length ?? 0
                }),
                metadados: AuditoriaDiffHelper.CriarMetadadosPadrao(
                    origem: "api",
                    modulo: "Chamados",
                    entidade: "Chamado",
                    entidadeId: chamadoId.ToString(),
                    codigo: atualizado.Codigo,
                    nome: atualizado.Titulo,
                    operacao: "Reabertura",
                    resultado: "Sucesso",
                    observacao: $"Status atual: {atualizado.Status}"),
                cancellationToken: cancellationToken);
        }

        return AdminUseCaseHelpers.MapDetalhe(atualizado);
    }
}
