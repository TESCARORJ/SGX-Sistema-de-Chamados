using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class CriarPerfilAcessoRequestValidator : AbstractValidator<CriarPerfilAcessoRequest>
{
    public CriarPerfilAcessoRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome obrigatorio.")
            .MaximumLength(120);

        RuleFor(x => x.TipoPerfil)
            .IsInEnum().WithMessage("TipoPerfil invalido.");
    }
}
