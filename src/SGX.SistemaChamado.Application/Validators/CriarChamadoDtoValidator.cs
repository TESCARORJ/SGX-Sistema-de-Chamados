using FluentValidation;
using SGX.SistemaChamado.Application.DTOs;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class CriarChamadoDtoValidator : AbstractValidator<CriarChamadoDto>
{
    public CriarChamadoDtoValidator()
    {
        RuleFor(x => x.Titulo)
            .NotEmpty()
            .MaximumLength(180);

        RuleFor(x => x.Descricao)
            .NotEmpty()
            .MaximumLength(4000);

        RuleFor(x => x.SolicitanteId)
            .NotEqual(Guid.Empty);

        RuleFor(x => x.CategoriaId)
            .NotEqual(Guid.Empty);

        RuleFor(x => x.PrioridadeId)
            .NotEqual(Guid.Empty);

        RuleFor(x => x.Origem)
            .NotEmpty()
            .Must(origem => origem is "Portal" or "Email" or "Admin")
            .WithMessage("Origem invalida. Valores permitidos: Portal, Email ou Admin.");
    }
}
