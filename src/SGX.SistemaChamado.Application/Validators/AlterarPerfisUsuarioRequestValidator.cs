using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class AlterarPerfisUsuarioRequestValidator : AbstractValidator<AlterarPerfisUsuarioRequest>
{
    public AlterarPerfisUsuarioRequestValidator()
    {
        RuleFor(x => x.PerfilIds)
            .NotEmpty().WithMessage("Ao menos um perfil deve ser informado.");

        RuleForEach(x => x.PerfilIds)
            .NotEmpty().WithMessage("PerfilId invalido.");
    }
}
