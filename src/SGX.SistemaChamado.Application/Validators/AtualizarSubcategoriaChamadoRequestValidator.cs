using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class AtualizarSubcategoriaChamadoRequestValidator : AbstractValidator<AtualizarSubcategoriaChamadoRequest>
{
    public AtualizarSubcategoriaChamadoRequestValidator()
    {
        RuleFor(x => x.CategoriaChamadoId)
            .NotEqual(Guid.Empty).WithMessage("CategoriaChamadoId obrigatorio.");

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome obrigatorio.")
            .MaximumLength(180);
    }
}
