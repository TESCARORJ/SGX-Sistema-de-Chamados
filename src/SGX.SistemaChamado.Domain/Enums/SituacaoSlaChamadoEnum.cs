namespace SGX.SistemaChamado.Domain.Enums;

public enum SituacaoSlaChamadoEnum
{
    NaoAplicavel = 0,
    DentroDoPrazo = 1,
    ProximoDoVencimento = 2,
    Vencido = 3,
    Cumprido = 4,
    Violado = 5,
    Pausado = 6
}
