using SGX.SistemaChamado.Domain.Abstractions;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class ConfiguracaoAlertaSla : AuditableEntity
{
    public int MinutosAntesVencimentoPrimeiraResposta { get; private set; }
    public int MinutosAntesVencimentoResolucao { get; private set; }
    public bool NotificarAtendente { get; private set; }
    public bool NotificarGestor { get; private set; }
    public bool NotificarDepartamento { get; private set; }

    private ConfiguracaoAlertaSla()
    {
    }

    public ConfiguracaoAlertaSla(
        int minutosAntesVencimentoPrimeiraResposta,
        int minutosAntesVencimentoResolucao,
        bool notificarAtendente,
        bool notificarGestor,
        bool notificarDepartamento,
        string criadoPor)
    {
        ValidarMinutos(minutosAntesVencimentoPrimeiraResposta, nameof(minutosAntesVencimentoPrimeiraResposta));
        ValidarMinutos(minutosAntesVencimentoResolucao, nameof(minutosAntesVencimentoResolucao));

        MinutosAntesVencimentoPrimeiraResposta = minutosAntesVencimentoPrimeiraResposta;
        MinutosAntesVencimentoResolucao = minutosAntesVencimentoResolucao;
        NotificarAtendente = notificarAtendente;
        NotificarGestor = notificarGestor;
        NotificarDepartamento = notificarDepartamento;
        DefinirCriacao(criadoPor);
    }

    public void Atualizar(
        bool ativo,
        int minutosAntesVencimentoPrimeiraResposta,
        int minutosAntesVencimentoResolucao,
        bool notificarAtendente,
        bool notificarGestor,
        bool notificarDepartamento,
        string atualizadoPor)
    {
        ValidarMinutos(minutosAntesVencimentoPrimeiraResposta, nameof(minutosAntesVencimentoPrimeiraResposta));
        ValidarMinutos(minutosAntesVencimentoResolucao, nameof(minutosAntesVencimentoResolucao));

        MinutosAntesVencimentoPrimeiraResposta = minutosAntesVencimentoPrimeiraResposta;
        MinutosAntesVencimentoResolucao = minutosAntesVencimentoResolucao;
        NotificarAtendente = notificarAtendente;
        NotificarGestor = notificarGestor;
        NotificarDepartamento = notificarDepartamento;

        if (ativo)
        {
            Ativar(atualizadoPor);
        }
        else
        {
            Desativar(atualizadoPor);
        }
    }

    private static void ValidarMinutos(int valor, string parametro)
    {
        if (valor < 0)
        {
            throw new ArgumentOutOfRangeException(parametro, "Os minutos de alerta de SLA nao podem ser negativos.");
        }
    }
}
