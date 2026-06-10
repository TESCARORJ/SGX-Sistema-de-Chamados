using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Chamados;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class ValidarBloqueioMovimentacaoAprovacaoPendenteRequestValidator : AbstractValidator<ValidarBloqueioMovimentacaoAprovacaoPendenteRequest>
{
    public ValidarBloqueioMovimentacaoAprovacaoPendenteRequestValidator()
    {
        RuleFor(x => x.ChamadoId)
            .NotEmpty()
            .WithMessage("O chamado informado para validacao de bloqueio e obrigatorio.");

        RuleFor(x => x.TipoAcao)
            .IsInEnum()
            .WithMessage("O tipo de acao informado para validacao de bloqueio e invalido.");

        RuleFor(x => x.StatusDestinoId)
            .NotEmpty()
            .When(x => x.TipoAcao == TipoAcaoMovimentacaoChamado.AlterarStatus)
            .WithMessage("O status de destino e obrigatorio quando a acao for alterar status.");

        RuleFor(x => x.UsuarioId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("O usuario informado para validacao de bloqueio e invalido.");

        RuleFor(x => x.Contexto)
            .MaximumLength(500)
            .WithMessage("O contexto informado para validacao de bloqueio deve possuir no maximo 500 caracteres.");
    }
}
