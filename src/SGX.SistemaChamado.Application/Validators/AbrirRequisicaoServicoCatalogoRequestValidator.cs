using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Portal;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class AbrirRequisicaoServicoCatalogoRequestValidator : AbstractValidator<AbrirRequisicaoServicoCatalogoRequest>
{
    public AbrirRequisicaoServicoCatalogoRequestValidator()
    {
        var respostaFormularioValidator = new RespostaFormularioAberturaRequestValidator();

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

        RuleForEach(x => x.RespostasFormulario)
            .SetValidator(respostaFormularioValidator)
            .When(x => x.RespostasFormulario is not null);

        RuleFor(x => x.RespostasFormulario)
            .Must(respostas => !PossuiCamposDuplicados(respostas))
            .When(x => x.RespostasFormulario is not null)
            .WithMessage("RespostasFormulario nao pode conter CampoFormularioServicoId duplicado.");
    }

    private static bool PossuiCamposDuplicados(IEnumerable<RespostaFormularioAberturaRequest>? respostas)
        => respostas?
            .GroupBy(x => x.CampoFormularioServicoId)
            .Any(x => x.Key != Guid.Empty && x.Count() > 1)
            == true;
}
