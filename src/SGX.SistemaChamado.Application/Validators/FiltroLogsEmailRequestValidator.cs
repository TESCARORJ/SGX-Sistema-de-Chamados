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

        RuleFor(x => x.DataFim)
            .GreaterThanOrEqualTo(x => x.DataInicio!.Value)
            .When(x => x.DataInicio.HasValue && x.DataFim.HasValue)
            .WithMessage("DataFim deve ser maior ou igual a DataInicio.");
    }
}
