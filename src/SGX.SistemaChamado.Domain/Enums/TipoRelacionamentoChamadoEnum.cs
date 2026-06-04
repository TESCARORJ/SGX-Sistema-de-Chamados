namespace SGX.SistemaChamado.Domain.Enums;

public enum TipoRelacionamentoChamadoEnum
{
    Relacionado = 1,
    Pai = 2,
    Filho = 3,
    Duplicado = 4,
    Bloqueia = 5,
    BloqueadoPor = 6,
    DerivadoDe = 7,
    Origina = 8
}
