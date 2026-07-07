using SGX.SistemaChamado.Domain.Abstractions;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class FormularioServicoVersao : AuditableEntity
{
    public Guid FormularioServicoId { get; private set; }
    public int Numero { get; private set; }
    public bool Publicada { get; private set; }
    public DateTime? PublicadoEm { get; private set; }

    public FormularioServico FormularioServico { get; private set; } = default!;
    public ICollection<CampoFormularioServico> Campos { get; private set; } = [];

    private FormularioServicoVersao()
    {
    }

    public FormularioServicoVersao(
        Guid formularioServicoId,
        int numero,
        bool publicada,
        DateTime? publicadoEm,
        string criadoPor)
    {
        DefinirFormularioServico(formularioServicoId);
        DefinirNumero(numero);
        DefinirPublicacao(publicada, publicadoEm);
        DefinirCriacao(criadoPor);
    }

    public void AlterarDados(int numero, bool publicada, DateTime? publicadoEm, string atualizadoPor)
    {
        DefinirNumero(numero);
        DefinirPublicacao(publicada, publicadoEm);
        AtualizarAuditoria(atualizadoPor);
    }

    public void Inativar(string atualizadoPor)
        => Desativar(atualizadoPor);

    public void Reativar(string atualizadoPor)
        => Ativar(atualizadoPor);

    private void DefinirFormularioServico(Guid formularioServicoId)
    {
        if (formularioServicoId == Guid.Empty)
        {
            throw new ArgumentException("O formulario da versao e obrigatorio.", nameof(formularioServicoId));
        }

        FormularioServicoId = formularioServicoId;
    }

    private void DefinirNumero(int numero)
    {
        if (numero <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(numero), "O numero da versao deve ser maior que zero.");
        }

        Numero = numero;
    }

    private void DefinirPublicacao(bool publicada, DateTime? publicadoEm)
    {
        if (!publicada && publicadoEm.HasValue)
        {
            throw new ArgumentException("A data de publicacao so pode ser informada quando a versao estiver publicada.", nameof(publicadoEm));
        }

        Publicada = publicada;
        PublicadoEm = publicada ? publicadoEm : null;
    }
}
