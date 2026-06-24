using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Validators;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Notificacoes;

public sealed class DefinirPreferenciaNotificacaoUsuarioUseCase(
    IRepository<Usuario> usuarioRepository,
    IRepository<PreferenciaNotificacaoUsuario> preferenciaRepository,
    IUnitOfWork unitOfWork,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IDefinirPreferenciaNotificacaoUsuarioUseCase
{
    public async Task<PreferenciaNotificacaoUsuarioResponse> ExecutarAsync(
        DefinirPreferenciaNotificacaoUsuarioRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var validator = new DefinirPreferenciaNotificacaoUsuarioRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var usuarioExiste = await usuarioRepository.Query()
            .AsNoTracking()
            .AnyAsync(x => x.Id == request.UsuarioId, cancellationToken);

        if (!usuarioExiste)
        {
            throw new InvalidOperationException("O usuario informado para preferencia de notificacao nao foi encontrado.");
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);

        var preferencia = await preferenciaRepository.Query()
            .FirstOrDefaultAsync(
                x => x.UsuarioId == request.UsuarioId
                     && x.TipoEvento == request.TipoEvento
                     && x.Canal == request.Canal,
                cancellationToken);

        var criada = false;
        if (preferencia is null)
        {
            preferencia = new PreferenciaNotificacaoUsuario(
                request.UsuarioId,
                request.TipoEvento,
                request.Canal,
                request.Habilitada,
                usuarioAtual.Id,
                usuarioAtual.Login);

            await preferenciaRepository.AddAsync(preferencia, cancellationToken);
            criada = true;
        }
        else
        {
            if (request.Habilitada)
            {
                preferencia.Habilitar(usuarioAtual.Id, usuarioAtual.Login);
            }
            else
            {
                preferencia.Desabilitar(usuarioAtual.Id, usuarioAtual.Login);
            }

            preferenciaRepository.Update(preferencia);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new PreferenciaNotificacaoUsuarioResponse(
            preferencia.Id,
            preferencia.UsuarioId,
            preferencia.TipoEvento,
            preferencia.Canal,
            preferencia.Habilitada,
            criada,
            !criada);
    }
}
