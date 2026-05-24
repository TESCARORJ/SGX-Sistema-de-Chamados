namespace SGX.SistemaChamado.Domain.Enums;

public enum TipoMovimentacaoAtivo
{
    Criacao = 1,
    Edicao = 2,
    TransferenciaDepartamento = 3,
    TransferenciaLocal = 4,
    AlteracaoResponsavel = 5,
    AlteracaoStatusOperacional = 6,
    AlteracaoStatusPatrimonial = 7,
    Manutencao = 8,
    Inativacao = 9,
    Reativacao = 10,
    VinculoChamado = 11,
    RemocaoVinculoChamado = 12
}
