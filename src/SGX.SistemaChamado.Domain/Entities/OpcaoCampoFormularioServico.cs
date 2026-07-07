using SGX.SistemaChamado.Domain.Abstractions;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class OpcaoCampoFormularioServico : AuditableEntity
{
    public Guid CampoFormularioServicoId { get; private set; }
    public string Valor { get; private set; } = string.Empty;
    public string Rotulo { get; private set; } = string.Empty;
    public int Ordem { get; private set; }

    public CampoFormularioServico CampoFormularioServico { get; private set; } = default!;

    private OpcaoCampoFormularioServico()
    {
    }

    public OpcaoCampoFormularioServico(
        Guid campoFormularioServicoId,
        string valor,
        string rotulo,
        int ordem,
        string criadoPor)
    {
        DefinirCampoFormularioServico(campoFormularioServicoId);
        DefinirValor(valor);
        DefinirRotulo(rotulo);
        DefinirOrdem(ordem);
        DefinirCriacao(criadoPor);
    }

    public void AlterarDados(string valor, string rotulo, int ordem, string atualizadoPor)
    {
        DefinirValor(valor);
        DefinirRotulo(rotulo);
        DefinirOrdem(ordem);
        AtualizarAuditoria(atualizadoPor);
    }

    public void Inativar(string atualizadoPor)
        => Desativar(atualizadoPor);

    public void Reativar(string atualizadoPor)
        => Ativar(atualizadoPor);

    private void DefinirCampoFormularioServico(Guid campoFormularioServicoId)
    {
        if (campoFormularioServicoId == Guid.Empty)
        {
            throw new ArgumentException("O campo da opcao e obrigatorio.", nameof(campoFormularioServicoId));
        }

        CampoFormularioServicoId = campoFormularioServicoId;
    }

    private void DefinirValor(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            throw new ArgumentException("O valor da opcao e obrigatorio.", nameof(valor));
        }

        var valorNormalizado = valor.Trim();
        if (valorNormalizado.Length > 120)
        {
            throw new ArgumentException("O valor da opcao deve possuir no maximo 120 caracteres.", nameof(valor));
        }

        Valor = valorNormalizado;
    }

    private void DefinirRotulo(string rotulo)
    {
        if (string.IsNullOrWhiteSpace(rotulo))
        {
            throw new ArgumentException("O rotulo da opcao e obrigatorio.", nameof(rotulo));
        }

        var rotuloNormalizado = rotulo.Trim();
        if (rotuloNormalizado.Length > 180)
        {
            throw new ArgumentException("O rotulo da opcao deve possuir no maximo 180 caracteres.", nameof(rotulo));
        }

        Rotulo = rotuloNormalizado;
    }

    private void DefinirOrdem(int ordem)
    {
        if (ordem <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(ordem), "A ordem da opcao deve ser maior que zero.");
        }

        Ordem = ordem;
    }
}
