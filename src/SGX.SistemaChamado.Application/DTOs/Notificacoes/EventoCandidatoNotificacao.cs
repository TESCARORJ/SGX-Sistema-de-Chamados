using System.Collections.ObjectModel;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.DTOs.Notificacoes;

public sealed record EventoCandidatoNotificacao
{
    private const int MaximoChaveCorrelacao = 200;
    private const int MaximoChaveIdempotencia = 200;

    public TipoEventoNotificacao TipoEvento { get; init; }
    public Guid? ChamadoId { get; init; }
    public Guid? UsuarioOriginadorId { get; init; }
    public DateTime OcorridoEm { get; init; }
    public string ChaveCorrelacao { get; init; }
    public string ChaveIdempotencia { get; init; }
    public IReadOnlyDictionary<string, string> Metadados { get; init; }

    public EventoCandidatoNotificacao(
        TipoEventoNotificacao tipoEvento,
        Guid? chamadoId,
        Guid? usuarioOriginadorId,
        DateTime ocorridoEm,
        string chaveCorrelacao,
        string chaveIdempotencia,
        IReadOnlyDictionary<string, string>? metadados = null)
    {
        if (!Enum.IsDefined(tipoEvento))
        {
            throw new ArgumentException("O tipo de evento candidato a notificacao e invalido.", nameof(tipoEvento));
        }

        if (chamadoId.HasValue && chamadoId.Value == Guid.Empty)
        {
            throw new ArgumentException("O chamado informado para o evento candidato e invalido.", nameof(chamadoId));
        }

        if (usuarioOriginadorId.HasValue && usuarioOriginadorId.Value == Guid.Empty)
        {
            throw new ArgumentException("O usuario originador informado para o evento candidato e invalido.", nameof(usuarioOriginadorId));
        }

        TipoEvento = tipoEvento;
        ChamadoId = chamadoId;
        UsuarioOriginadorId = usuarioOriginadorId;
        OcorridoEm = NormalizarDataUtcObrigatoria(ocorridoEm, nameof(ocorridoEm));
        ChaveCorrelacao = NormalizarTextoObrigatorio(
            chaveCorrelacao,
            MaximoChaveCorrelacao,
            "A chave de correlacao do evento candidato e obrigatoria.",
            nameof(chaveCorrelacao));
        ChaveIdempotencia = NormalizarTextoObrigatorio(
            chaveIdempotencia,
            MaximoChaveIdempotencia,
            "A chave de idempotencia do evento candidato e obrigatoria.",
            nameof(chaveIdempotencia));
        Metadados = CriarMetadadosSomenteLeitura(metadados);
    }

    private static IReadOnlyDictionary<string, string> CriarMetadadosSomenteLeitura(IReadOnlyDictionary<string, string>? metadados)
    {
        if (metadados is null || metadados.Count == 0)
        {
            return new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());
        }

        var copia = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var par in metadados)
        {
            if (string.IsNullOrWhiteSpace(par.Key))
            {
                throw new ArgumentException("Metadados do evento candidato nao podem possuir chave vazia.", nameof(metadados));
            }

            copia[par.Key.Trim()] = par.Value?.Trim() ?? string.Empty;
        }

        return new ReadOnlyDictionary<string, string>(copia);
    }

    private static string NormalizarTextoObrigatorio(string? valor, int tamanhoMaximo, string mensagemObrigatorio, string paramName)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            throw new ArgumentException(mensagemObrigatorio, paramName);
        }

        var textoNormalizado = valor.Trim();
        if (textoNormalizado.Length > tamanhoMaximo)
        {
            throw new ArgumentException($"O valor informado deve possuir no maximo {tamanhoMaximo} caracteres.", paramName);
        }

        return textoNormalizado;
    }

    private static DateTime NormalizarDataUtcObrigatoria(DateTime valor, string paramName)
    {
        if (valor == default)
        {
            throw new ArgumentException("A data informada e obrigatoria.", paramName);
        }

        return valor.Kind switch
        {
            DateTimeKind.Utc => valor,
            DateTimeKind.Local => valor.ToUniversalTime(),
            _ => DateTime.SpecifyKind(valor, DateTimeKind.Utc)
        };
    }
}
