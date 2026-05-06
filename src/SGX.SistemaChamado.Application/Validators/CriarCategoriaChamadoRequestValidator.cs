using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class CriarCategoriaChamadoRequestValidator : AbstractValidator<CriarCategoriaChamadoRequest>
{
    public CriarCategoriaChamadoRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome obrigatorio.")
            .MaximumLength(180);

        RuleFor(x => x.DepartamentoId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("DepartamentoId invalido.");
    }
}
