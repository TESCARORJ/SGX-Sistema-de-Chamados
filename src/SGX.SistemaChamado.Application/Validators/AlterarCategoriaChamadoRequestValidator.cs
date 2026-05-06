using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class AlterarCategoriaChamadoRequestValidator : AbstractValidator<AlterarCategoriaChamadoRequest>
{
    public AlterarCategoriaChamadoRequestValidator()
    {
        RuleFor(x => x.CategoriaId)
            .NotEmpty().WithMessage("Categoria obrigatoria.");
    }
}
