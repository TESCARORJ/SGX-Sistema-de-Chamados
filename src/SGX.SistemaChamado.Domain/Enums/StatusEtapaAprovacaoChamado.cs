namespace SGX.SistemaChamado.Domain.Enums;

public enum StatusEtapaAprovacaoChamado
{
    Pendente = 1,
    Aprovada = 2,
    Reprovada = 3,
    Cancelada = 4,
    Expirada = 5,
    AguardandoEtapaAnterior = 6,
    EmReavaliacao = 7,
    Substituida = 8,
    Ignorada = 9
}
