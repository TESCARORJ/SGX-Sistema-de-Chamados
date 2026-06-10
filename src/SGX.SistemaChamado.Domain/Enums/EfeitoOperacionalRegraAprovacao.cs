namespace SGX.SistemaChamado.Domain.Enums;

public enum EfeitoOperacionalRegraAprovacao
{
    Permitir = 1,
    Sinalizar = 2,
    ExigirAprovacao = 3,
    ExigirAprovacaoEBloquearAvanco = 4,
    RequerReavaliacao = 5
}
