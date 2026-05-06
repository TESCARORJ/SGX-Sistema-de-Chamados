using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Portal;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class CriarChamadoRequestValidator : AbstractValidator<CriarChamadoRequest>
{
    public CriarChamadoRequestValidator()
    {
        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("Titulo obrigatorio.")
            .MaximumLength(180).WithMessage("Titulo deve ter no maximo 180 caracteres.");

        RuleFor(x => x.Descricao)
            .NotEmpty().WithMessage("Descricao obrigatoria.")
            .MaximumLength(4000).WithMessage("Descricao deve ter no maximo 4000 caracteres.");

        RuleFor(x => x.CategoriaId)
            .NotEmpty().WithMessage("Categoria obrigatoria.");

        RuleFor(x => x.PrioridadeId)
            .NotEmpty().WithMessage("Prioridade obrigatoria.");
    }
}
