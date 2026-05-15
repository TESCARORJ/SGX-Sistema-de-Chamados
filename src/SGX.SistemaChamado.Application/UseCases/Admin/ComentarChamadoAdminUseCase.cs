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

public sealed class ComentarChamadoAdminUseCase(
    IRepository<Chamado> chamadoRepository,
    IRepository<ComentarioChamado> comentarioRepository,
    IRepository<HistoricoChamado> historicoRepository,
    ISlaService slaService,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null) : IComentarChamadoAdminUseCase
{
    public async Task<ComentarioAdminResponse> ExecutarAsync(Guid chamadoId, ComentarioAdminChamadoRequest request, CancellationToken cancellationToken = default)
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
            .Include(x => x.ChamadoSla)
            .FirstOrDefaultAsync(x => x.Id == chamadoId && x.Ativo, cancellationToken)
            ?? throw new KeyNotFoundException("Chamado nao encontrado.");

        var comentario = new ComentarioChamado(
            chamado.Id,
            usuario.Id,
            request.Mensagem,
            request.Interno,
            usuario.Login);

        await comentarioRepository.AddAsync(comentario, cancellationToken);

        var descricaoHistorico = request.Interno
            ? "Comentario administrativo interno adicionado"
            : "Comentario administrativo publico adicionado";

        var historico = new HistoricoChamado(
            chamado.Id,
            TipoHistoricoChamado.ComentarioAdicionado,
            AdminUseCaseHelpers.ObterDescricaoHistorico(TipoHistoricoChamado.ComentarioAdicionado, descricaoHistorico),
            usuario.Id,
            usuario.Login);

        await historicoRepository.AddAsync(historico, cancellationToken);

        if (!request.Interno)
        {
            await slaService.RegistrarPrimeiraRespostaAsync(chamado, usuario.Login, DateTime.UtcNow, cancellationToken);
        }

        chamado.AtualizarAuditoria(usuario.Login);
        chamadoRepository.Update(chamado);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (auditoriaService is not null)
        {
            await auditoriaService.RegistrarAsync(new RegistrarEventoAuditoriaRequest
            {
                Modulo = "Chamados",
                Entidade = "Chamado",
                EntidadeId = chamadoId.ToString(),
                Acao = TipoAcaoAuditoria.Edicao,
                Descricao = request.Interno
                    ? "Comentario interno registrado."
                    : "Comentario publico administrativo registrado.",
                DadosDepois = AuditoriaDiffHelper.SerializarSeguro(new
                {
                    ComentarioId = comentario.Id,
                    comentario.Interno,
                    TamanhoMensagem = comentario.Mensagem.Length
                }),
                Metadados = AuditoriaDiffHelper.CriarMetadadosPadrao(
                    origem: "api",
                    modulo: "Chamados",
                    entidade: "Chamado",
                    entidadeId: chamadoId.ToString(),
                    codigo: chamado.Codigo,
                    nome: chamado.Titulo,
                    operacao: request.Interno ? "ComentarioInterno" : "ComentarioPublico",
                    resultado: "Sucesso",
                    observacao: $"ComentarioId: {comentario.Id}"),
                Nivel = NivelAuditoria.Informacao,
                Sucesso = true
            }, cancellationToken);
        }

        return new ComentarioAdminResponse(
            comentario.Id,
            comentario.UsuarioId,
            usuario.Nome,
            comentario.Mensagem,
            comentario.Interno,
            comentario.CriadoEm);
    }
}
