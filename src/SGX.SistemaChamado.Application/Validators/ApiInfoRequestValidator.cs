using FluentValidation;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class ApiInfoRequest
{
    public string Ambiente { get; init; } = string.Empty;
}

public sealed class ApiInfoRequestValidator : AbstractValidator<ApiInfoRequest>
{
    public ApiInfoRequestValidator()
    {
        RuleFor(x => x.Ambiente)
            .NotEmpty()
            .WithMessage("O ambiente deve ser informado.")
            .MaximumLength(64);
    }
}
