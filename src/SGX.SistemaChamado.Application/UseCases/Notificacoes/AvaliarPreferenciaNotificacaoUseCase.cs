using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Validators;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Notificacoes;

public sealed class AvaliarPreferenciaNotificacaoUseCase(
    IRepository<Usuario> usuarioRepository,
    IRepository<PreferenciaNotificacaoUsuario> preferenciaRepository) : IAvaliarPreferenciaNotificacaoUseCase
{
    public async Task<AvaliarPreferenciaNotificacaoResponse> ExecutarAsync(
        AvaliarPreferenciaNotificacaoRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var validator = new AvaliarPreferenciaNotificacaoRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var usuario = await usuarioRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.UsuarioId, cancellationToken);

        if (usuario is null)
        {
            return CriarResposta(request, false, false, null, MotivoDecisaoPreferenciaNotificacao.UsuarioInexistente);
        }

        if (!usuario.Ativo || usuario.Situacao == SituacaoUsuario.Inativo)
        {
            return CriarResposta(request, false, false, null, MotivoDecisaoPreferenciaNotificacao.UsuarioInativo);
        }

        if (usuario.Situacao == SituacaoUsuario.Bloqueado
            || (usuario.BloqueadoAte.HasValue && usuario.BloqueadoAte.Value > DateTime.UtcNow))
        {
            return CriarResposta(request, false, false, null, MotivoDecisaoPreferenciaNotificacao.UsuarioBloqueado);
        }

        if (request.Canal == CanalNotificacao.Email && string.IsNullOrWhiteSpace(usuario.Email))
        {
            return CriarResposta(request, false, false, null, MotivoDecisaoPreferenciaNotificacao.CanalSemEndereco);
        }

        var preferencia = await preferenciaRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.UsuarioId == request.UsuarioId
                     && x.TipoEvento == request.TipoEvento
                     && x.Canal == request.Canal,
                cancellationToken);

        if (preferencia is not null)
        {
            return CriarResposta(
                request,
                preferencia.Habilitada,
                true,
                preferencia.Habilitada,
                preferencia.Habilitada
                    ? MotivoDecisaoPreferenciaNotificacao.PreferenciaHabilitada
                    : MotivoDecisaoPreferenciaNotificacao.PreferenciaDesabilitada);
        }

        return CriarResposta(request, true, false, null, MotivoDecisaoPreferenciaNotificacao.FallbackPermitido);
    }

    private static AvaliarPreferenciaNotificacaoResponse CriarResposta(
        AvaliarPreferenciaNotificacaoRequest request,
        bool permitida,
        bool preferenciaExplicita,
        bool? habilitadaConfigurada,
        MotivoDecisaoPreferenciaNotificacao motivo)
    {
        return new AvaliarPreferenciaNotificacaoResponse(
            request.UsuarioId,
            request.TipoEvento,
            request.Canal,
            permitida,
            preferenciaExplicita,
            habilitadaConfigurada,
            motivo,
            DescreverMotivo(motivo));
    }

    private static string DescreverMotivo(MotivoDecisaoPreferenciaNotificacao motivo)
    {
        return motivo switch
        {
            MotivoDecisaoPreferenciaNotificacao.PreferenciaHabilitada => "Permitido por preferencia explicita habilitada.",
            MotivoDecisaoPreferenciaNotificacao.PreferenciaDesabilitada => "Bloqueado por preferencia explicita desabilitada.",
            MotivoDecisaoPreferenciaNotificacao.FallbackPermitido => "Permitido por ausencia de preferencia explicita.",
            MotivoDecisaoPreferenciaNotificacao.UsuarioInexistente => "Bloqueado porque o usuario nao foi encontrado.",
            MotivoDecisaoPreferenciaNotificacao.UsuarioInativo => "Bloqueado porque o usuario esta inativo.",
            MotivoDecisaoPreferenciaNotificacao.UsuarioBloqueado => "Bloqueado porque o usuario esta bloqueado.",
            MotivoDecisaoPreferenciaNotificacao.CanalSemEndereco => "Bloqueado porque o canal exige endereco eletronico valido.",
            _ => "Motivo de preferencia de notificacao nao mapeado."
        };
    }
}
