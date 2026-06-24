using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Validators;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Application.UseCases.Notificacoes;

public sealed class GerarNotificacaoUseCase(
    IRepository<Notificacao> notificacaoRepository,
    IUnitOfWork unitOfWork,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IGerarNotificacaoUseCase
{
    private const string NomeIndiceIdempotencia = "ux_notificacoes_chave_idempotencia";

    public async Task<GerarNotificacaoResponse> ExecutarAsync(
        GerarNotificacaoRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var validator = new GerarNotificacaoRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var chaveIdempotencia = request.Evento.ChaveIdempotencia.Trim();
        var existente = await ObterPorChaveIdempotenciaAsync(chaveIdempotencia, cancellationToken);
        if (existente is not null)
        {
            return CriarResponseJaExistente(existente);
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        var criadoPorUsuarioId = request.Evento.UsuarioOriginadorId ?? usuarioAtual.Id;

        var notificacao = new Notificacao(
            request.Evento.TipoEvento,
            request.Canal,
            request.Conteudo,
            chaveIdempotencia,
            usuarioAtual.Login,
            request.DestinatarioUsuarioId,
            request.DestinatarioEndereco,
            request.Evento.ChamadoId,
            request.Assunto,
            request.Evento.ChaveCorrelacao,
            criadoPorUsuarioId);

        if (request.AgendadaEm.HasValue)
        {
            notificacao.Agendar(request.AgendadaEm.Value, usuarioAtual.Login, usuarioAtual.Id);
        }

        await notificacaoRepository.AddAsync(notificacao, cancellationToken);

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return new GerarNotificacaoResponse(
                notificacao.Id,
                true,
                false,
                notificacao.Status,
                notificacao.ChaveIdempotencia);
        }
        catch (DbUpdateException ex) when (EhViolacaoDuplicidadeIdempotencia(ex))
        {
            var concorrente = await ObterPorChaveIdempotenciaAsync(chaveIdempotencia, cancellationToken);
            if (concorrente is not null)
            {
                return CriarResponseJaExistente(concorrente);
            }

            throw;
        }
    }

    private async Task<Notificacao?> ObterPorChaveIdempotenciaAsync(string chaveIdempotencia, CancellationToken cancellationToken)
    {
        return await notificacaoRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ChaveIdempotencia == chaveIdempotencia, cancellationToken);
    }

    private static GerarNotificacaoResponse CriarResponseJaExistente(Notificacao notificacao)
        => new(
            notificacao.Id,
            false,
            true,
            notificacao.Status,
            notificacao.ChaveIdempotencia);

    private static bool EhViolacaoDuplicidadeIdempotencia(DbUpdateException ex)
        => ex.InnerException?.Message.Contains(NomeIndiceIdempotencia, StringComparison.OrdinalIgnoreCase) == true
           || ex.Message.Contains(NomeIndiceIdempotencia, StringComparison.OrdinalIgnoreCase);
}
