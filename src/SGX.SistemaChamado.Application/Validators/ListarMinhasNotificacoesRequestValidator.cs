using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class ListarMinhasNotificacoesRequestValidator : AbstractValidator<ListarMinhasNotificacoesRequest>
{
    public ListarMinhasNotificacoesRequestValidator()
    {
        RuleFor(x => x.Pagina)
            .GreaterThanOrEqualTo(1)
            .WithMessage("A pagina deve ser maior ou igual a 1.");

        RuleFor(x => x.TamanhoPagina)
            .InclusiveBetween(1, 100)
            .WithMessage("O tamanho da pagina deve estar entre 1 e 100.");
    }
}
