using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class AtualizarPermissoesPerfilRequestValidator : AbstractValidator<AtualizarPermissoesPerfilRequest>
{
    public AtualizarPermissoesPerfilRequestValidator()
    {
        RuleFor(x => x.CodigosPermissoes)
            .NotNull().WithMessage("A lista de codigos de permissoes deve ser informada.");

        RuleForEach(x => x.CodigosPermissoes)
            .NotEmpty().WithMessage("Codigo de permissao invalido.");
    }
}
