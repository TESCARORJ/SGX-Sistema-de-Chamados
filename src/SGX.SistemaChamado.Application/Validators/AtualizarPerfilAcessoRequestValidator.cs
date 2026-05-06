using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class AtualizarPerfilAcessoRequestValidator : AbstractValidator<AtualizarPerfilAcessoRequest>
{
    public AtualizarPerfilAcessoRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome obrigatorio.")
            .MaximumLength(120);

        RuleFor(x => x.TipoPerfil)
            .IsInEnum().WithMessage("TipoPerfil invalido.");
    }
}
