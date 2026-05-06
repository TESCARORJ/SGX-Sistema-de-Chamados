using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class FiltroIndicadoresRequestValidator : AbstractValidator<FiltroIndicadoresRequest>
{
    public FiltroIndicadoresRequestValidator()
    {
        RuleFor(x => x.DataFim)
            .GreaterThanOrEqualTo(x => x.DataInicio!.Value)
            .When(x => x.DataInicio.HasValue && x.DataFim.HasValue)
            .WithMessage("DataFim deve ser maior ou igual a DataInicio.");
    }
}
