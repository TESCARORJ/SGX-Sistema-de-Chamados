using System.Text.RegularExpressions;
using SGX.SistemaChamado.Domain.Abstractions;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class TemplateNotificacao : AuditableEntity
{
    public const int MaximoNome = 180;
    public const int MaximoDescricao = 4000;
    public const int MaximoAssuntoTemplate = 300;
    public const int MaximoConteudoTemplate = 10000;
    public const int MaximoVariavelPermitida = 120;
    public const int MaximoQuantidadeVariaveisPermitidas = 100;

    private static readonly Regex RegexVariavelPermitida = new(
        "^[a-z0-9]+(?:[._-][a-z0-9]+)*$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private List<string> _variaveisPermitidas = [];

    public string Nome { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }
    public TipoEventoNotificacao TipoEvento { get; private set; }
    public CanalNotificacao Canal { get; private set; }
    public int Versao { get; private set; }
    public string? AssuntoTemplate { get; private set; }
    public string ConteudoTemplate { get; private set; } = string.Empty;
    public IReadOnlyCollection<string> VariaveisPermitidas => _variaveisPermitidas.AsReadOnly();
    public DateTime? VigenteDe { get; private set; }
    public DateTime? VigenteAte { get; private set; }
    public Guid CriadoPorUsuarioId { get; private set; }
    public Guid? AtualizadoPorUsuarioId { get; private set; }

    public Usuario CriadoPorUsuario { get; private set; } = default!;
    public Usuario? AtualizadoPorUsuario { get; private set; }

    private List<string> VariaveisPermitidasPersistidas
    {
        get => _variaveisPermitidas;
        set => _variaveisPermitidas = value ?? [];
    }

    private TemplateNotificacao()
    {
    }

    public TemplateNotificacao(
        string nome,
        TipoEventoNotificacao tipoEvento,
        CanalNotificacao canal,
        int versao,
        string conteudoTemplate,
        Guid criadoPorUsuarioId,
        string criadoPor,
        IReadOnlyCollection<string>? variaveisPermitidas = null,
        string? assuntoTemplate = null,
        string? descricao = null,
        DateTime? vigenteDe = null,
        DateTime? vigenteAte = null)
    {
        if (criadoPorUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuario criador do template de notificacao e obrigatorio.", nameof(criadoPorUsuarioId));
        }

        DefinirNome(nome);
        DefinirDescricao(descricao);
        DefinirTipoEvento(tipoEvento);
        DefinirCanal(canal);
        DefinirVersao(versao);
        DefinirAssuntoTemplate(assuntoTemplate);
        DefinirConteudoTemplate(conteudoTemplate);
        DefinirVariaveisPermitidas(variaveisPermitidas);
        DefinirVigencia(vigenteDe, vigenteAte);

        CriadoPorUsuarioId = criadoPorUsuarioId;
        DefinirCriacao(criadoPor);
    }

    public bool EstaVigenteEm(DateTime dataReferenciaUtc)
    {
        var referenciaUtc = NormalizarDataUtcObrigatoria(dataReferenciaUtc, nameof(dataReferenciaUtc));
        if (!Ativo)
        {
            return false;
        }

        if (VigenteDe.HasValue && referenciaUtc < VigenteDe.Value)
        {
            return false;
        }

        if (VigenteAte.HasValue && referenciaUtc > VigenteAte.Value)
        {
            return false;
        }

        return true;
    }

    public void AtivarTemplate(Guid atualizadoPorUsuarioId, string atualizadoPor)
    {
        RegistrarAtualizacao(atualizadoPorUsuarioId, atualizadoPor);
        Ativar(atualizadoPor);
    }

    public void DesativarTemplate(Guid atualizadoPorUsuarioId, string atualizadoPor)
    {
        RegistrarAtualizacao(atualizadoPorUsuarioId, atualizadoPor);
        Desativar(atualizadoPor);
    }

    private void DefinirNome(string nome)
    {
        Nome = NormalizarTextoObrigatorio(
            nome,
            MaximoNome,
            "O nome do template de notificacao e obrigatorio.",
            nameof(nome));
    }

    private void DefinirDescricao(string? descricao)
    {
        Descricao = NormalizarTextoOpcional(descricao, MaximoDescricao, nameof(descricao));
    }

    private void DefinirTipoEvento(TipoEventoNotificacao tipoEvento)
    {
        if (!Enum.IsDefined(tipoEvento))
        {
            throw new ArgumentException("O tipo de evento do template de notificacao e invalido.", nameof(tipoEvento));
        }

        TipoEvento = tipoEvento;
    }

    private void DefinirCanal(CanalNotificacao canal)
    {
        if (!Enum.IsDefined(canal))
        {
            throw new ArgumentException("O canal do template de notificacao e invalido.", nameof(canal));
        }

        Canal = canal;
    }

    private void DefinirVersao(int versao)
    {
        if (versao <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(versao), "A versao do template de notificacao deve ser maior que zero.");
        }

        Versao = versao;
    }

    private void DefinirAssuntoTemplate(string? assuntoTemplate)
    {
        AssuntoTemplate = NormalizarTextoOpcional(assuntoTemplate, MaximoAssuntoTemplate, nameof(assuntoTemplate));
    }

    private void DefinirConteudoTemplate(string conteudoTemplate)
    {
        ConteudoTemplate = NormalizarTextoObrigatorio(
            conteudoTemplate,
            MaximoConteudoTemplate,
            "O conteudo do template de notificacao e obrigatorio.",
            nameof(conteudoTemplate));
    }

    private void DefinirVariaveisPermitidas(IReadOnlyCollection<string>? variaveisPermitidas)
    {
        if (variaveisPermitidas is null || variaveisPermitidas.Count == 0)
        {
            _variaveisPermitidas = [];
            return;
        }

        if (variaveisPermitidas.Count > MaximoQuantidadeVariaveisPermitidas)
        {
            throw new ArgumentException(
                $"O template de notificacao deve possuir no maximo {MaximoQuantidadeVariaveisPermitidas} variaveis permitidas.",
                nameof(variaveisPermitidas));
        }

        var normalizadas = new HashSet<string>(StringComparer.Ordinal);
        foreach (var variavel in variaveisPermitidas)
        {
            if (string.IsNullOrWhiteSpace(variavel))
            {
                throw new ArgumentException("As variaveis permitidas do template nao podem ser vazias.", nameof(variaveisPermitidas));
            }

            var normalizada = variavel.Trim().ToLowerInvariant();
            if (normalizada.Length > MaximoVariavelPermitida)
            {
                throw new ArgumentException(
                    $"Cada variavel permitida deve possuir no maximo {MaximoVariavelPermitida} caracteres.",
                    nameof(variaveisPermitidas));
            }

            if (!RegexVariavelPermitida.IsMatch(normalizada))
            {
                throw new ArgumentException(
                    "As variaveis permitidas do template devem usar apenas letras minusculas, numeros, ponto, underline ou hifen.",
                    nameof(variaveisPermitidas));
            }

            if (!normalizadas.Add(normalizada))
            {
                throw new ArgumentException("O template de notificacao nao pode possuir variaveis permitidas duplicadas.", nameof(variaveisPermitidas));
            }
        }

        _variaveisPermitidas = normalizadas.OrderBy(x => x, StringComparer.Ordinal).ToList();
    }

    private void DefinirVigencia(DateTime? vigenteDe, DateTime? vigenteAte)
    {
        DateTime? vigenteDeUtc = vigenteDe.HasValue ? NormalizarDataUtcObrigatoria(vigenteDe.Value, nameof(vigenteDe)) : null;
        DateTime? vigenteAteUtc = vigenteAte.HasValue ? NormalizarDataUtcObrigatoria(vigenteAte.Value, nameof(vigenteAte)) : null;

        if (vigenteDeUtc.HasValue && vigenteAteUtc.HasValue && vigenteAteUtc.Value < vigenteDeUtc.Value)
        {
            throw new InvalidOperationException("A vigencia final do template de notificacao nao pode ser anterior ao inicio.");
        }

        VigenteDe = vigenteDeUtc;
        VigenteAte = vigenteAteUtc;
    }

    private void RegistrarAtualizacao(Guid atualizadoPorUsuarioId, string atualizadoPor)
    {
        if (atualizadoPorUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuario de atualizacao do template de notificacao e obrigatorio.", nameof(atualizadoPorUsuarioId));
        }

        AtualizadoPorUsuarioId = atualizadoPorUsuarioId;
        AtualizarAuditoria(atualizadoPor);
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

    private static string? NormalizarTextoOpcional(string? valor, int tamanhoMaximo, string paramName)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return null;
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
