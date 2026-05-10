using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.Interfaces.Email;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Application.Services.Email;

public sealed class EmailCorrelationService(
    IRepository<Chamado> chamadoRepository,
    IRepository<LogIntegracaoEmail> logIntegracaoEmailRepository) : IEmailCorrelationService
{
    private static readonly Regex CodigoChamadoRegex =
        new(@"(?:#)?\b(?:SGX|CHM)-\d{4}-\d{6}\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public async Task<Chamado?> TryFindChamadoAsync(EmailMessageData emailMessage, CancellationToken cancellationToken = default)
        => (await CorrelacionarAsync(emailMessage, cancellationToken)).Chamado;

    public async Task<EmailCorrelationResult> CorrelacionarAsync(EmailMessageData emailMessage, CancellationToken cancellationToken = default)
    {
        var codigoDetectado = ExtrairCodigoChamado(emailMessage.Assunto);
        var headersRelacionados = ObterHeadersRelacionados(emailMessage);
        var possuiIndicadorResposta = codigoDetectado is not null || headersRelacionados.Count > 0;

        if (codigoDetectado is not null)
        {
            var chamadoPorCodigo = await chamadoRepository.Query()
                .FirstOrDefaultAsync(x => x.Ativo && x.Codigo == codigoDetectado, cancellationToken);

            if (chamadoPorCodigo is not null)
            {
                return new EmailCorrelationResult(chamadoPorCodigo, true, codigoDetectado, headersRelacionados);
            }
        }

        if (headersRelacionados.Count > 0)
        {
            var headersLower = headersRelacionados.Select(x => x.ToLowerInvariant()).ToArray();

            var logRelacionado = await logIntegracaoEmailRepository.Query()
                .AsNoTracking()
                .Where(x => x.ChamadoId.HasValue && x.MessageId != null)
                .Where(x => headersLower.Contains(x.MessageId!.ToLower()))
                .OrderByDescending(x => x.DataProcessamento ?? x.CriadoEm)
                .FirstOrDefaultAsync(cancellationToken);

            if (logRelacionado?.ChamadoId is not null)
            {
                var chamadoPorHeader = await chamadoRepository.Query()
                    .FirstOrDefaultAsync(x => x.Ativo && x.Id == logRelacionado.ChamadoId.Value, cancellationToken);

                if (chamadoPorHeader is not null)
                {
                    return new EmailCorrelationResult(chamadoPorHeader, true, codigoDetectado, headersRelacionados);
                }
            }
        }

        return new EmailCorrelationResult(null, possuiIndicadorResposta, codigoDetectado, headersRelacionados);
    }

    private static string? ExtrairCodigoChamado(string? assunto)
    {
        if (string.IsNullOrWhiteSpace(assunto))
        {
            return null;
        }

        var match = CodigoChamadoRegex.Match(assunto);
        if (!match.Success)
        {
            return null;
        }

        var codigo = match.Value.Trim().TrimStart('#').ToUpperInvariant();
        return codigo;
    }

    private static IReadOnlyCollection<string> ObterHeadersRelacionados(EmailMessageData emailMessage)
    {
        var headersRelacionados = new List<string>();

        if (!string.IsNullOrWhiteSpace(emailMessage.InReplyTo))
        {
            headersRelacionados.Add(NormalizarHeader(emailMessage.InReplyTo));
        }

        foreach (var header in emailMessage.References)
        {
            if (!string.IsNullOrWhiteSpace(header))
            {
                headersRelacionados.Add(NormalizarHeader(header));
            }
        }

        return headersRelacionados
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static string NormalizarHeader(string value)
        => value.Trim().Trim('<', '>');
}
