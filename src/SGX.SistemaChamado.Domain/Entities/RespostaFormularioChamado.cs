using System.Text.Json;
using SGX.SistemaChamado.Domain.Abstractions;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class RespostaFormularioChamado : AuditableEntity
{
    public const int TamanhoMaximoValor = 4000;

    public Guid ChamadoId { get; private set; }
    public Guid FormularioServicoVersaoId { get; private set; }
    public Guid CampoFormularioServicoId { get; private set; }
    public string? Valor { get; private set; }
    public string? ValoresJson { get; private set; }

    public Chamado Chamado { get; private set; } = default!;
    public FormularioServicoVersao FormularioServicoVersao { get; private set; } = default!;
    public CampoFormularioServico CampoFormularioServico { get; private set; } = default!;

    private RespostaFormularioChamado()
    {
    }

    public RespostaFormularioChamado(
        Guid chamadoId,
        Guid formularioServicoVersaoId,
        Guid campoFormularioServicoId,
        string? valor,
        IReadOnlyCollection<string>? valores,
        string criadoPor)
    {
        DefinirChamado(chamadoId);
        DefinirFormularioServicoVersao(formularioServicoVersaoId);
        DefinirCampoFormularioServico(campoFormularioServicoId);
        DefinirConteudo(valor, valores);
        DefinirCriacao(criadoPor);
    }

    public IReadOnlyCollection<string> ObterValores()
    {
        if (string.IsNullOrWhiteSpace(ValoresJson))
        {
            return [];
        }

        return JsonSerializer.Deserialize<string[]>(ValoresJson) ?? [];
    }

    private void DefinirChamado(Guid chamadoId)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("O chamado da resposta e obrigatorio.", nameof(chamadoId));
        }

        ChamadoId = chamadoId;
    }

    private void DefinirFormularioServicoVersao(Guid formularioServicoVersaoId)
    {
        if (formularioServicoVersaoId == Guid.Empty)
        {
            throw new ArgumentException("A versao do formulario da resposta e obrigatoria.", nameof(formularioServicoVersaoId));
        }

        FormularioServicoVersaoId = formularioServicoVersaoId;
    }

    private void DefinirCampoFormularioServico(Guid campoFormularioServicoId)
    {
        if (campoFormularioServicoId == Guid.Empty)
        {
            throw new ArgumentException("O campo do formulario da resposta e obrigatorio.", nameof(campoFormularioServicoId));
        }

        CampoFormularioServicoId = campoFormularioServicoId;
    }

    private void DefinirConteudo(string? valor, IReadOnlyCollection<string>? valores)
    {
        var valorNormalizado = string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
        var possuiValor = valorNormalizado is not null;
        var valoresNormalizados = valores?
            .Select(item => item?.Trim() ?? string.Empty)
            .ToArray() ?? [];
        var possuiValores = valoresNormalizados.Length > 0;

        if (!possuiValor && !possuiValores)
        {
            throw new ArgumentException("A resposta deve informar Valor ou Valores.", nameof(valor));
        }

        if (possuiValor && possuiValores)
        {
            throw new ArgumentException("A resposta deve informar apenas Valor ou Valores.", nameof(valor));
        }

        if (possuiValor)
        {
            if (valorNormalizado!.Length > TamanhoMaximoValor)
            {
                throw new ArgumentException($"O valor da resposta deve possuir no maximo {TamanhoMaximoValor} caracteres.", nameof(valor));
            }

            Valor = valorNormalizado;
            ValoresJson = null;
            return;
        }

        if (valoresNormalizados.Any(item => string.IsNullOrWhiteSpace(item)))
        {
            throw new ArgumentException("Valores nao pode conter itens vazios.", nameof(valores));
        }

        if (valoresNormalizados.Any(item => item.Length > TamanhoMaximoValor))
        {
            throw new ArgumentException($"Cada item de Valores deve possuir no maximo {TamanhoMaximoValor} caracteres.", nameof(valores));
        }

        Valor = null;
        ValoresJson = JsonSerializer.Serialize(valoresNormalizados);
    }
}
