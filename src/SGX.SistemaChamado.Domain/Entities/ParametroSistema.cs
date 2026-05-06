using SGX.SistemaChamado.Domain.Abstractions;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class ParametroSistema : AuditableEntity
{
    public string Chave { get; private set; } = string.Empty;
    public string Valor { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }
    public bool Sensivel { get; private set; }

    private ParametroSistema()
    {
    }

    public ParametroSistema(string chave, string valor, string? descricao, bool sensivel, string criadoPor)
    {
        DefinirChave(chave);
        DefinirValor(valor);
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
        Sensivel = sensivel;
        DefinirCriacao(criadoPor);
    }

    public void DefinirChave(string chave)
    {
        if (string.IsNullOrWhiteSpace(chave))
        {
            throw new ArgumentException("A chave do parametro e obrigatoria.", nameof(chave));
        }

        Chave = chave.Trim();
    }

    public void DefinirValor(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            throw new ArgumentException("O valor do parametro e obrigatorio.", nameof(valor));
        }

        Valor = valor.Trim();
    }

    public void AtualizarValor(string valor, string atualizadoPor)
    {
        DefinirValor(valor);
        AtualizarAuditoria(atualizadoPor);
    }

    public void DefinirDescricao(string? descricao, string atualizadoPor)
    {
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
        AtualizarAuditoria(atualizadoPor);
    }

    public void DefinirSensivel(bool sensivel, string atualizadoPor)
    {
        Sensivel = sensivel;
        AtualizarAuditoria(atualizadoPor);
    }
}
