using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces.Notificacoes;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Notificacoes;

public sealed class ProcessarEventoCandidatoNotificacaoUseCase(
    IResolverDestinatariosNotificacaoUseCase resolverDestinatariosNotificacaoUseCase,
    IAvaliarPreferenciaNotificacaoUseCase avaliarPreferenciaNotificacaoUseCase,
    IMaterializarConteudoNotificacaoUseCase materializarConteudoNotificacaoUseCase,
    IGerarNotificacaoUseCase gerarNotificacaoUseCase,
    ILogger<ProcessarEventoCandidatoNotificacaoUseCase> logger) : IProcessarEventoCandidatoNotificacaoUseCase
{
    public async Task<ProcessarEventoCandidatoNotificacaoResponse> ExecutarAsync(
        ProcessarEventoCandidatoNotificacaoRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.Evento);
        ArgumentNullException.ThrowIfNull(request.Variaveis);

        var eventoId = string.IsNullOrWhiteSpace(request.EventoId)
            ? request.Evento.ChaveIdempotencia
            : request.EventoId.Trim();

        var canais = request.Canais
            .Where(Enum.IsDefined)
            .Distinct()
            .ToArray();
        var participacoes = request.Participacoes
            .Where(Enum.IsDefined)
            .Distinct()
            .ToArray();

        if (canais.Length == 0)
        {
            throw new InvalidOperationException("Ao menos um canal de notificacao deve ser informado.");
        }

        if (participacoes.Length == 0)
        {
            throw new InvalidOperationException("Ao menos uma participacao de destinatario deve ser informada.");
        }

        var avisos = new List<string>();
        var resolverResponse = await resolverDestinatariosNotificacaoUseCase.ExecutarAsync(
            new ResolverDestinatariosNotificacaoRequest(
                request.Evento,
                participacoes,
                request.AprovacaoChamadoId,
                request.InstanciaAprovacaoChamadoId,
                request.GrupoTecnicoId,
                request.PerfilAcessoId,
                request.ExcluirUsuarioOriginador),
            cancellationToken);

        avisos.AddRange(resolverResponse.Avisos);

        var destinatarios = resolverResponse.Destinatarios.ToArray();
        var destinatariosPermitidos = 0;
        var notificacoesCriadas = 0;
        var notificacoesDuplicadas = 0;
        var ignoradas = 0;

        foreach (var destinatario in destinatarios)
        {
            foreach (var canal in canais)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var avaliacao = await avaliarPreferenciaNotificacaoUseCase.ExecutarAsync(
                    new AvaliarPreferenciaNotificacaoRequest(
                        destinatario.UsuarioId,
                        request.Evento.TipoEvento,
                        canal),
                    cancellationToken);

                if (!avaliacao.Permitida)
                {
                    ignoradas++;
                    avisos.Add($"Canal {canal} bloqueado para o destinatario {destinatario.UsuarioId}: {avaliacao.DescricaoMotivo}");
                    continue;
                }

                destinatariosPermitidos++;

                MaterializarConteudoNotificacaoResponse conteudo;
                try
                {
                    conteudo = await materializarConteudoNotificacaoUseCase.ExecutarAsync(
                        new MaterializarConteudoNotificacaoRequest(
                            request.Evento.TipoEvento,
                            canal,
                            request.Evento.OcorridoEm,
                            request.Variaveis),
                        cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    ignoradas++;
                    avisos.Add($"Template/materializacao indisponivel para o evento {eventoId} no canal {canal}: {ex.Message}");
                    logger.LogWarning(
                        ex,
                        "Materializacao de notificacao ignorada. EventoId={EventoId} ChamadoId={ChamadoId} UsuarioId={UsuarioId} Canal={Canal}",
                        eventoId,
                        request.Evento.ChamadoId,
                        destinatario.UsuarioId,
                        canal);
                    continue;
                }

                var chaveIdempotencia = CriarChaveIdempotencia(
                    request.Evento.ChaveIdempotencia,
                    destinatario,
                    canal);

                var eventoPorDestinatario = new EventoCandidatoNotificacao(
                    request.Evento.TipoEvento,
                    request.Evento.ChamadoId,
                    request.Evento.UsuarioOriginadorId,
                    request.Evento.OcorridoEm,
                    request.Evento.ChaveCorrelacao,
                    chaveIdempotencia,
                    request.Evento.Metadados);

                var notificacao = await gerarNotificacaoUseCase.ExecutarAsync(
                    new GerarNotificacaoRequest(
                        eventoPorDestinatario,
                        canal,
                        destinatario.UsuarioId,
                        canal == CanalNotificacao.Email ? destinatario.Email : null,
                        conteudo.Assunto,
                        conteudo.Conteudo),
                    cancellationToken);

                if (notificacao.Criada)
                {
                    notificacoesCriadas++;
                }
                else if (notificacao.JaExistia)
                {
                    notificacoesDuplicadas++;
                }
            }
        }

        logger.LogInformation(
            "Integracao de notificacao processada. EventoId={EventoId} ChamadoId={ChamadoId} DestinatariosResolvidos={DestinatariosResolvidos} DestinatariosPermitidos={DestinatariosPermitidos} NotificacoesCriadas={NotificacoesCriadas} NotificacoesDuplicadas={NotificacoesDuplicadas} Ignoradas={Ignoradas}",
            eventoId,
            request.Evento.ChamadoId,
            destinatarios.Length,
            destinatariosPermitidos,
            notificacoesCriadas,
            notificacoesDuplicadas,
            ignoradas);

        return new ProcessarEventoCandidatoNotificacaoResponse(
            eventoId,
            destinatarios.Length,
            destinatariosPermitidos,
            notificacoesCriadas,
            notificacoesDuplicadas,
            ignoradas,
            avisos);
    }

    private static string CriarChaveIdempotencia(
        string chaveEventoBase,
        DestinatarioNotificacaoResolvido destinatario,
        CanalNotificacao canal)
    {
        var identificadorDestino = canal == CanalNotificacao.Email
            ? destinatario.Email ?? destinatario.UsuarioId.ToString("N")
            : destinatario.UsuarioId.ToString("N");

        var bruto = $"{chaveEventoBase.Trim()}:{identificadorDestino}:{canal}";
        Span<byte> hash = stackalloc byte[32];
        SHA256.HashData(Encoding.UTF8.GetBytes(bruto), hash);
        var sufixo = Convert.ToHexString(hash[..12]).ToLowerInvariant();
        var prefixo = $"{chaveEventoBase.Trim()}:{(int)canal}:";
        var chave = $"{prefixo}{sufixo}";

        return chave.Length <= 200
            ? chave
            : chave[..200];
    }
}
