using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.Interfaces.Email;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Application.Services.Email;

public sealed class EmailCorrelationService(
    IRepository<Chamado> chamadoRepository,
    IRepository<LogIntegracaoEmail> logIntegracaoEmailRepository) : IEmailCorrelationService
{
    private static readonly System.Text.RegularExpressions.Regex CodigoChamadoRegex =
        new(@"SGX-\d{4}-\d{6}", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Compiled);

    public async Task<Chamado?> TryFindChamadoAsync(EmailMessageData emailMessage, CancellationToken cancellationToken = default)
    {
        var codigoMatch = CodigoChamadoRegex.Match(emailMessage.Assunto ?? string.Empty);
        if (codigoMatch.Success)
        {
            var codigo = codigoMatch.Value.ToUpperInvariant();
            var chamadoPorCodigo = await chamadoRepository.Query()
                .FirstOrDefaultAsync(x => x.Ativo && x.Codigo == codigo, cancellationToken);

            if (chamadoPorCodigo is not null)
            {
                return chamadoPorCodigo;
            }
        }

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

        if (headersRelacionados.Count == 0)
        {
            return null;
        }

        var logRelacionado = await logIntegracaoEmailRepository.Query()
            .AsNoTracking()
            .Where(x => x.ChamadoId.HasValue && x.MessageId != null && headersRelacionados.Contains(x.MessageId))
            .OrderByDescending(x => x.DataProcessamento ?? x.CriadoEm)
            .FirstOrDefaultAsync(cancellationToken);

        if (logRelacionado?.ChamadoId is null)
        {
            return null;
        }

        return await chamadoRepository.Query()
            .FirstOrDefaultAsync(x => x.Ativo && x.Id == logRelacionado.ChamadoId.Value, cancellationToken);
    }

    private static string NormalizarHeader(string value)
        => value.Trim().Trim('<', '>');
}
