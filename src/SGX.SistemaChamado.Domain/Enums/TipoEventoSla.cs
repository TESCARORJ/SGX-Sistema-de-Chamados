namespace SGX.SistemaChamado.Domain.Enums;

public enum TipoEventoSla
{
    SlaAplicado = 1,
    PrimeiraRespostaDentroDoPrazo = 2,
    PrimeiraRespostaVencida = 3,
    ResolucaoDentroDoPrazo = 4,
    ResolucaoVencida = 5,
    SlaPausado = 6,
    SlaRetomado = 7,
    AlertaPrimeiraRespostaProximoVencimento = 8,
    AlertaResolucaoProximoVencimento = 9,
    AlertaPrimeiraRespostaVencida = 10,
    AlertaResolucaoVencida = 11,
    AlertaEnviado = 12
}
