using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class FiltroLogsEmailRequestValidator : AbstractValidator<FiltroLogsEmailRequest>
{
    public FiltroLogsEmailRequestValidator()
    {
        RuleFor(x => x.Pagina)
            .GreaterThan(0)
            .WithMessage("Pagina deve ser maior que zero.");

        RuleFor(x => x.TamanhoPagina)
            .InclusiveBetween(1, 200)
            .WithMessage("TamanhoPagina deve estar entre 1 e 200.");

        RuleFor(x => x)
            .Must(x => !x.DataInicialEfetiva.HasValue || !x.DataFinalEfetiva.HasValue || x.DataFinalEfetiva.Value >= x.DataInicialEfetiva.Value)
            .WithMessage("Data final deve ser maior ou igual a data inicial.");

        RuleFor(x => x.Direcao)
            .Must(v => string.IsNullOrWhiteSpace(v) || string.Equals(v, "asc", StringComparison.OrdinalIgnoreCase) || string.Equals(v, "desc", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Direcao deve ser asc ou desc.");
    }
}
