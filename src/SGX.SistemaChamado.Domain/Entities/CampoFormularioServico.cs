using System.Text.RegularExpressions;
using SGX.SistemaChamado.Domain.Abstractions;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class CampoFormularioServico : AuditableEntity
{
    private static readonly Regex NomeTecnicoRegex = new("^[a-zA-Z][a-zA-Z0-9_]*$", RegexOptions.Compiled);

    public Guid FormularioServicoVersaoId { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Rotulo { get; private set; } = string.Empty;
    public TipoCampoFormularioServico Tipo { get; private set; }
    public bool Obrigatorio { get; private set; }
    public int Ordem { get; private set; }
    public string? TextoAjuda { get; private set; }
    public bool Visivel { get; private set; } = true;

    public FormularioServicoVersao FormularioServicoVersao { get; private set; } = default!;
    public ICollection<OpcaoCampoFormularioServico> Opcoes { get; private set; } = [];

    private CampoFormularioServico()
    {
    }

    public CampoFormularioServico(
        Guid formularioServicoVersaoId,
        string nome,
        string rotulo,
        TipoCampoFormularioServico tipo,
        bool obrigatorio,
        int ordem,
        string? textoAjuda,
        bool visivel,
        string criadoPor)
    {
        DefinirFormularioServicoVersao(formularioServicoVersaoId);
        DefinirNome(nome);
        DefinirRotulo(rotulo);
        DefinirTipo(tipo);
        DefinirObrigatorio(obrigatorio);
        DefinirOrdem(ordem);
        DefinirTextoAjuda(textoAjuda);
        DefinirVisivel(visivel);
        DefinirCriacao(criadoPor);
    }

    public void AlterarDados(
        string nome,
        string rotulo,
        TipoCampoFormularioServico tipo,
        bool obrigatorio,
        int ordem,
        string? textoAjuda,
        bool visivel,
        string atualizadoPor)
    {
        DefinirNome(nome);
        DefinirRotulo(rotulo);
        DefinirTipo(tipo);
        DefinirObrigatorio(obrigatorio);
        DefinirOrdem(ordem);
        DefinirTextoAjuda(textoAjuda);
        DefinirVisivel(visivel);
        AtualizarAuditoria(atualizadoPor);
    }

    public void Inativar(string atualizadoPor)
        => Desativar(atualizadoPor);

    public void Reativar(string atualizadoPor)
        => Ativar(atualizadoPor);

    private void DefinirFormularioServicoVersao(Guid formularioServicoVersaoId)
    {
        if (formularioServicoVersaoId == Guid.Empty)
        {
            throw new ArgumentException("A versao do formulario do campo e obrigatoria.", nameof(formularioServicoVersaoId));
        }

        FormularioServicoVersaoId = formularioServicoVersaoId;
    }

    private void DefinirNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ArgumentException("O nome tecnico do campo e obrigatorio.", nameof(nome));
        }

        var nomeNormalizado = nome.Trim();
        if (nomeNormalizado.Length > 120)
        {
            throw new ArgumentException("O nome tecnico do campo deve possuir no maximo 120 caracteres.", nameof(nome));
        }

        if (!NomeTecnicoRegex.IsMatch(nomeNormalizado))
        {
            throw new ArgumentException("O nome tecnico do campo deve iniciar com letra e conter apenas letras, numeros ou underscore.", nameof(nome));
        }

        Nome = nomeNormalizado;
    }

    private void DefinirRotulo(string rotulo)
    {
        if (string.IsNullOrWhiteSpace(rotulo))
        {
            throw new ArgumentException("O rotulo do campo e obrigatorio.", nameof(rotulo));
        }

        var rotuloNormalizado = rotulo.Trim();
        if (rotuloNormalizado.Length > 180)
        {
            throw new ArgumentException("O rotulo do campo deve possuir no maximo 180 caracteres.", nameof(rotulo));
        }

        Rotulo = rotuloNormalizado;
    }

    private void DefinirTipo(TipoCampoFormularioServico tipo)
    {
        if (!Enum.IsDefined(tipo))
        {
            throw new ArgumentException("O tipo do campo e invalido.", nameof(tipo));
        }

        Tipo = tipo;
    }

    private void DefinirObrigatorio(bool obrigatorio)
    {
        Obrigatorio = obrigatorio;
    }

    private void DefinirOrdem(int ordem)
    {
        if (ordem <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(ordem), "A ordem do campo deve ser maior que zero.");
        }

        Ordem = ordem;
    }

    private void DefinirTextoAjuda(string? textoAjuda)
    {
        if (textoAjuda is not null && textoAjuda.Trim().Length > 500)
        {
            throw new ArgumentException("O texto de ajuda do campo deve possuir no maximo 500 caracteres.", nameof(textoAjuda));
        }

        TextoAjuda = string.IsNullOrWhiteSpace(textoAjuda) ? null : textoAjuda.Trim();
    }

    private void DefinirVisivel(bool visivel)
    {
        Visivel = visivel;
    }
}
