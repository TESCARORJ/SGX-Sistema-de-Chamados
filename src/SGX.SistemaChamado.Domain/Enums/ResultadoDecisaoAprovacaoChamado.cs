namespace SGX.SistemaChamado.Domain.Enums;

public enum ResultadoDecisaoAprovacaoChamado
{
    Aprovada = 1,
    Reprovada = 2,
    Cancelada = 3,
    Expirada = 4,
    RequerAjuste = 5,
    RequerNovaAprovacao = 6,
    SemEfeitoOperacional = 7
}
