using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class CriarSubcategoriaChamadoRequestValidator : AbstractValidator<CriarSubcategoriaChamadoRequest>
{
    public CriarSubcategoriaChamadoRequestValidator()
    {
        RuleFor(x => x.CategoriaChamadoId)
            .NotEqual(Guid.Empty).WithMessage("CategoriaChamadoId obrigatorio.");

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome obrigatorio.")
            .MaximumLength(180);
    }
}
