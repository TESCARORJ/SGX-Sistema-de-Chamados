using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class MaterializarConteudoNotificacaoRequestValidator : AbstractValidator<MaterializarConteudoNotificacaoRequest>
{
    private const int MaximoQuantidadeVariaveis = 100;
    private const int MaximoChaveVariavel = 120;
    private const int MaximoValorVariavel = 5000;

    public MaterializarConteudoNotificacaoRequestValidator()
    {
        RuleFor(x => x.TipoEvento)
            .Must(Enum.IsDefined)
            .WithMessage("O tipo de evento da materializacao de notificacao e invalido.");

        RuleFor(x => x.Canal)
            .Must(Enum.IsDefined)
            .WithMessage("O canal da materializacao de notificacao e invalido.");

        RuleFor(x => x.DataReferencia)
            .Must(x => x != default)
            .WithMessage("A data de referencia da materializacao e obrigatoria.");

        RuleFor(x => x.TemplateNotificacaoId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("O template de notificacao informado e invalido.");

        RuleFor(x => x.Variaveis)
            .NotNull()
            .WithMessage("As variaveis da materializacao de notificacao sao obrigatorias.")
            .Must(x => x is null || x.Count <= MaximoQuantidadeVariaveis)
            .WithMessage($"A materializacao deve possuir no maximo {MaximoQuantidadeVariaveis} variaveis.");

        RuleForEach(x => x.Variaveis)
            .ChildRules(variavel =>
            {
                variavel.RuleFor(x => x.Key)
                    .NotEmpty()
                    .WithMessage("As variaveis da materializacao nao podem possuir chave vazia.")
                    .MaximumLength(MaximoChaveVariavel)
                    .WithMessage($"Cada chave de variavel deve possuir no maximo {MaximoChaveVariavel} caracteres.");

                variavel.RuleFor(x => x.Value)
                    .NotNull()
                    .WithMessage("As variaveis da materializacao nao podem possuir valor nulo.")
                    .MaximumLength(MaximoValorVariavel)
                    .WithMessage($"Cada valor de variavel deve possuir no maximo {MaximoValorVariavel} caracteres.");
            });
    }
}
