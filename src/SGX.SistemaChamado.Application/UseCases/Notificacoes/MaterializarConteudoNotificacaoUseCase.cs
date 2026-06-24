using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Validators;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Notificacoes;

public sealed class MaterializarConteudoNotificacaoUseCase(
    IRepository<TemplateNotificacao> templateNotificacaoRepository) : IMaterializarConteudoNotificacaoUseCase
{
    private const int MaximoAssuntoNotificacao = 300;
    private const int MaximoConteudoNotificacao = 10000;

    private static readonly Regex RegexPlaceholderValido = new(
        @"\{\{\s*([a-z0-9]+(?:[._-][a-z0-9]+)*)\s*\}\}",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public async Task<MaterializarConteudoNotificacaoResponse> ExecutarAsync(
        MaterializarConteudoNotificacaoRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var validator = new MaterializarConteudoNotificacaoRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var variaveisNormalizadas = NormalizarVariaveis(request.Variaveis);
        var template = await SelecionarTemplateAsync(request, cancellationToken);

        if (!template.Ativo)
        {
            throw new InvalidOperationException("O template de notificacao selecionado esta inativo.");
        }

        if (!template.EstaVigenteEm(request.DataReferencia))
        {
            throw new InvalidOperationException("O template de notificacao selecionado nao esta vigente na data de referencia informada.");
        }

        if (template.TipoEvento != request.TipoEvento || template.Canal != request.Canal)
        {
            throw new InvalidOperationException("O template de notificacao selecionado nao corresponde ao tipo de evento ou canal informados.");
        }

        var variaveisPermitidas = new HashSet<string>(template.VariaveisPermitidas, StringComparer.Ordinal);
        foreach (var variavelInformada in variaveisNormalizadas.Keys)
        {
            if (!variaveisPermitidas.Contains(variavelInformada))
            {
                throw new InvalidOperationException($"A variavel '{variavelInformada}' nao e permitida pelo template selecionado.");
            }
        }

        var placeholdersAssunto = ExtrairPlaceholders(template.AssuntoTemplate);
        var placeholdersConteudo = ExtrairPlaceholders(template.ConteudoTemplate);
        var placeholdersUtilizados = placeholdersAssunto
            .Concat(placeholdersConteudo)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(x => x, StringComparer.Ordinal)
            .ToArray();

        foreach (var placeholder in placeholdersUtilizados)
        {
            if (!variaveisPermitidas.Contains(placeholder))
            {
                throw new InvalidOperationException($"O placeholder '{placeholder}' nao esta declarado nas variaveis permitidas do template.");
            }

            if (!variaveisNormalizadas.ContainsKey(placeholder))
            {
                throw new InvalidOperationException($"A variavel obrigatoria '{placeholder}' nao foi informada para a materializacao.");
            }
        }

        var assunto = MaterializarAssunto(template.AssuntoTemplate, variaveisNormalizadas);
        var conteudo = MaterializarConteudo(template.ConteudoTemplate, request.Canal, variaveisNormalizadas);

        if (assunto is not null && assunto.Length > MaximoAssuntoNotificacao)
        {
            throw new InvalidOperationException($"O assunto materializado excede o limite de {MaximoAssuntoNotificacao} caracteres da notificacao.");
        }

        if (conteudo.Length > MaximoConteudoNotificacao)
        {
            throw new InvalidOperationException($"O conteudo materializado excede o limite de {MaximoConteudoNotificacao} caracteres da notificacao.");
        }

        return new MaterializarConteudoNotificacaoResponse(
            template.Id,
            template.Nome,
            template.Versao,
            assunto,
            conteudo,
            placeholdersUtilizados);
    }

    private async Task<TemplateNotificacao> SelecionarTemplateAsync(
        MaterializarConteudoNotificacaoRequest request,
        CancellationToken cancellationToken)
    {
        if (request.TemplateNotificacaoId.HasValue)
        {
            var templateExplcito = await templateNotificacaoRepository.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.TemplateNotificacaoId.Value, cancellationToken);

            return templateExplcito
                ?? throw new InvalidOperationException("O template de notificacao informado nao foi encontrado.");
        }

        var template = await templateNotificacaoRepository.Query()
            .AsNoTracking()
            .Where(x =>
                x.TipoEvento == request.TipoEvento
                && x.Canal == request.Canal
                && x.Ativo
                && (!x.VigenteDe.HasValue || x.VigenteDe.Value <= request.DataReferencia)
                && (!x.VigenteAte.HasValue || x.VigenteAte.Value >= request.DataReferencia))
            .OrderByDescending(x => x.Versao)
            .ThenBy(x => x.Nome)
            .FirstOrDefaultAsync(cancellationToken);

        return template
            ?? throw new InvalidOperationException("Nenhum template de notificacao ativo e vigente foi encontrado para o evento e canal informados.");
    }

    private static Dictionary<string, string> NormalizarVariaveis(IReadOnlyDictionary<string, string> variaveis)
    {
        var normalizadas = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var variavel in variaveis)
        {
            var chave = variavel.Key.Trim().ToLowerInvariant();
            if (!normalizadas.TryAdd(chave, variavel.Value.Trim()))
            {
                throw new InvalidOperationException($"A variavel '{chave}' foi informada mais de uma vez para a materializacao.");
            }
        }

        return normalizadas;
    }

    private static string? MaterializarAssunto(string? assuntoTemplate, IReadOnlyDictionary<string, string> variaveis)
    {
        if (string.IsNullOrWhiteSpace(assuntoTemplate))
        {
            return null;
        }

        return SubstituirPlaceholders(assuntoTemplate, variaveis, escapeHtml: false);
    }

    private static string MaterializarConteudo(
        string conteudoTemplate,
        CanalNotificacao canal,
        IReadOnlyDictionary<string, string> variaveis)
    {
        var escapeHtml = canal == CanalNotificacao.Email;
        return SubstituirPlaceholders(conteudoTemplate, variaveis, escapeHtml);
    }

    private static string[] ExtrairPlaceholders(string? template)
    {
        if (string.IsNullOrWhiteSpace(template))
        {
            return [];
        }

        ValidarMarcadoresBemFormados(template);

        return RegexPlaceholderValido
            .Matches(template)
            .Select(x => x.Groups[1].Value)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(x => x, StringComparer.Ordinal)
            .ToArray();
    }

    private static void ValidarMarcadoresBemFormados(string template)
    {
        var restante = RegexPlaceholderValido.Replace(template, string.Empty);
        if (restante.Contains("{{", StringComparison.Ordinal) || restante.Contains("}}", StringComparison.Ordinal))
        {
            throw new InvalidOperationException("O template de notificacao possui placeholder malformado ou nao suportado.");
        }
    }

    private static string SubstituirPlaceholders(
        string template,
        IReadOnlyDictionary<string, string> variaveis,
        bool escapeHtml)
    {
        ValidarMarcadoresBemFormados(template);

        var builder = new StringBuilder();
        var ultimoIndice = 0;

        foreach (Match match in RegexPlaceholderValido.Matches(template))
        {
            builder.Append(template, ultimoIndice, match.Index - ultimoIndice);

            var chave = match.Groups[1].Value;
            if (!variaveis.TryGetValue(chave, out var valor))
            {
                throw new InvalidOperationException($"A variavel obrigatoria '{chave}' nao foi informada para a materializacao.");
            }

            builder.Append(escapeHtml ? HtmlEncoder.Default.Encode(valor) : valor);
            ultimoIndice = match.Index + match.Length;
        }

        builder.Append(template, ultimoIndice, template.Length - ultimoIndice);
        return builder.ToString();
    }
}
