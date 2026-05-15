using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Chamados;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class CriarComentarioChamadoRequestValidator : AbstractValidator<CriarComentarioChamadoRequest>
{
    public CriarComentarioChamadoRequestValidator()
    {
        RuleFor(x => x.Mensagem)
            .NotEmpty().WithMessage("Mensagem obrigatoria.")
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Mensagem nao pode ser vazia.")
            .MaximumLength(4000).WithMessage("Mensagem deve ter no maximo 4000 caracteres.");
    }
}
