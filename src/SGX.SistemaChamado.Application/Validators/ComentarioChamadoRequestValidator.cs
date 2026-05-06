using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Portal;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class ComentarioChamadoRequestValidator : AbstractValidator<ComentarioChamadoRequest>
{
    public ComentarioChamadoRequestValidator()
    {
        RuleFor(x => x.Mensagem)
            .NotEmpty().WithMessage("Mensagem obrigatoria.")
            .MaximumLength(3000).WithMessage("Mensagem deve ter no maximo 3000 caracteres.")
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Mensagem nao pode ser vazia.");
    }
}
