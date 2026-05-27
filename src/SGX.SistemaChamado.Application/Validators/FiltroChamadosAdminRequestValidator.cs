using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class FiltroChamadosAdminRequestValidator : AbstractValidator<FiltroChamadosAdminRequest>
{
    private static readonly string[] CamposOrdenacaoPermitidos =
    [
        "atualizadoEm",
        "abertoEm",
        "encerradoEm",
        "codigo",
        "titulo",
        "status",
        "prioridade"
    ];

    public FiltroChamadosAdminRequestValidator()
    {
        RuleFor(x => x.Pagina)
            .GreaterThanOrEqualTo(1).WithMessage("Pagina deve ser maior ou igual a 1.");

        RuleFor(x => x.TamanhoPagina)
            .InclusiveBetween(1, 100).WithMessage("TamanhoPagina deve estar entre 1 e 100.");

        RuleFor(x => x.OrdenarPor)
            .Must(campo => CamposOrdenacaoPermitidos.Contains(campo, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Campo de ordenacao invalido.");

        RuleFor(x => x.DirecaoOrdenacao)
            .Must(direcao => string.Equals(direcao, "asc", StringComparison.OrdinalIgnoreCase) || string.Equals(direcao, "desc", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Direcao de ordenacao invalida. Use 'asc' ou 'desc'.");

        RuleFor(x => x.NaturezaChamado)
            .IsInEnum()
            .When(x => x.NaturezaChamado.HasValue)
            .WithMessage("Natureza do chamado invalida.");
    }
}
