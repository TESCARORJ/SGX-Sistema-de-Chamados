using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Portal;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class AceitarSolucaoChamadoRequestValidator : AbstractValidator<AceitarSolucaoChamadoRequest>
{
    public AceitarSolucaoChamadoRequestValidator()
    {
        RuleFor(x => x.ObservacaoAceite)
            .MaximumLength(2000).WithMessage("A observacao do aceite nao pode exceder 2000 caracteres.");
    }
}
