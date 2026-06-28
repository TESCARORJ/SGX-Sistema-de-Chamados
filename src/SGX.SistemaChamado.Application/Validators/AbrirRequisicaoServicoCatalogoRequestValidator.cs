using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Portal;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class AbrirRequisicaoServicoCatalogoRequestValidator : AbstractValidator<AbrirRequisicaoServicoCatalogoRequest>
{
    public AbrirRequisicaoServicoCatalogoRequestValidator()
    {
        RuleFor(x => x.CatalogoServicoId)
            .NotEmpty()
            .WithMessage("CatalogoServicoId obrigatorio.");

        RuleFor(x => x.Titulo)
            .NotEmpty()
            .WithMessage("Titulo obrigatorio.")
            .MaximumLength(180)
            .WithMessage("Titulo deve ter no maximo 180 caracteres.");

        RuleFor(x => x.Descricao)
            .MaximumLength(4000)
            .WithMessage("Descricao deve ter no maximo 4000 caracteres.");
    }
}
