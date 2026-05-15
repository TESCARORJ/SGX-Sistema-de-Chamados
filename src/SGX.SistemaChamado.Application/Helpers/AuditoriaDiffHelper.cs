using System.Text.Json;
using System.Text.Json.Nodes;

namespace SGX.SistemaChamado.Application.Helpers;

public static class AuditoriaDiffHelper
{
    private static readonly string[] CamposSensiveis =
    [
        "senha",
        "password",
        "token",
        "jwt",
        "secret",
        "clientsecret",
        "refreshtoken",
        "accesstoken",
        "connectionstring",
        "chave",
        "apikey",
        "authorization",
        "bearer"
    ];

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false
    };

    public static string? SerializarSeguro(object? dados, bool omitirNulos = true)
    {
        if (dados is null)
        {
            return null;
        }

        var node = JsonSerializer.SerializeToNode(dados, JsonOptions);
        if (node is null)
        {
            return null;
        }

        var sanitizado = SanitizarNode(node, propriedadePai: null, omitirNulos);
        if (sanitizado is null)
        {
            return null;
        }

        return sanitizado.ToJsonString(JsonOptions);
    }

    public static (string? dadosAntes, string? dadosDepois) CriarDiff(
        object? antes,
        object? depois,
        bool somenteAlterados = true,
        bool omitirNulos = true)
    {
        var nodeAntes = JsonSerializer.SerializeToNode(antes, JsonOptions) as JsonObject ?? [];
        var nodeDepois = JsonSerializer.SerializeToNode(depois, JsonOptions) as JsonObject ?? [];

        var sanitizadoAntes = SanitizarNode(nodeAntes, null, omitirNulos) as JsonObject ?? [];
        var sanitizadoDepois = SanitizarNode(nodeDepois, null, omitirNulos) as JsonObject ?? [];

        if (!somenteAlterados)
        {
            return (ToJsonOuNulo(sanitizadoAntes), ToJsonOuNulo(sanitizadoDepois));
        }

        var chaves = sanitizadoAntes.Select(x => x.Key)
            .Union(sanitizadoDepois.Select(x => x.Key), StringComparer.OrdinalIgnoreCase)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        JsonObject? antesAlterado = [];
        JsonObject? depoisAlterado = [];

        foreach (var chave in chaves)
        {
            sanitizadoAntes.TryGetPropertyValue(chave, out var valorAntes);
            sanitizadoDepois.TryGetPropertyValue(chave, out var valorDepois);

            if (JsonNodesIguais(valorAntes, valorDepois))
            {
                continue;
            }

            antesAlterado[chave] = valorAntes?.DeepClone();
            depoisAlterado[chave] = valorDepois?.DeepClone();
        }

        return (ToJsonOuNulo(antesAlterado), ToJsonOuNulo(depoisAlterado));
    }

    public static string CriarMetadadosPadrao(
        string origem,
        string modulo,
        string entidade,
        string? entidadeId,
        string operacao,
        string resultado,
        string? codigo = null,
        string? nome = null,
        string? observacao = null)
        => SerializarSeguro(new
        {
            origem,
            modulo,
            entidade,
            entidadeId,
            codigo,
            nome,
            operacao,
            resultado,
            observacao
        }) ?? "{}";

    private static JsonNode? SanitizarNode(JsonNode? node, string? propriedadePai, bool omitirNulos)
    {
        if (node is null)
        {
            return omitirNulos ? null : JsonValue.Create((string?)null);
        }

        if (EhCampoSensivel(propriedadePai))
        {
            return JsonValue.Create("***");
        }

        return node switch
        {
            JsonObject obj => SanitizarObjeto(obj, omitirNulos),
            JsonArray arr => SanitizarArray(arr, omitirNulos),
            _ => node.DeepClone()
        };
    }

    private static JsonNode? SanitizarObjeto(JsonObject objeto, bool omitirNulos)
    {
        var resultado = new JsonObject();

        foreach (var kv in objeto)
        {
            var valor = SanitizarNode(kv.Value, kv.Key, omitirNulos);
            if (valor is null && omitirNulos)
            {
                continue;
            }

            resultado[kv.Key] = valor;
        }

        return resultado.Count == 0 && omitirNulos ? null : resultado;
    }

    private static JsonNode? SanitizarArray(JsonArray array, bool omitirNulos)
    {
        var resultado = new JsonArray();

        foreach (var item in array)
        {
            var valor = SanitizarNode(item, null, omitirNulos);
            if (valor is null && omitirNulos)
            {
                continue;
            }

            resultado.Add(valor);
        }

        return resultado.Count == 0 && omitirNulos ? null : resultado;
    }

    private static bool EhCampoSensivel(string? nomeCampo)
    {
        if (string.IsNullOrWhiteSpace(nomeCampo))
        {
            return false;
        }

        var normalizado = nomeCampo.Trim().Replace("_", string.Empty).Replace("-", string.Empty).ToLowerInvariant();
        return CamposSensiveis.Any(campo => normalizado.Contains(campo, StringComparison.OrdinalIgnoreCase));
    }

    private static bool JsonNodesIguais(JsonNode? antes, JsonNode? depois)
    {
        var a = antes?.ToJsonString(JsonOptions) ?? "null";
        var b = depois?.ToJsonString(JsonOptions) ?? "null";
        return string.Equals(a, b, StringComparison.Ordinal);
    }

    private static string? ToJsonOuNulo(JsonObject? obj)
    {
        if (obj is null || obj.Count == 0)
        {
            return null;
        }

        return obj.ToJsonString(JsonOptions);
    }
}
