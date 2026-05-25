using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class SolicitarAprovacaoChamadoRequestValidator : AbstractValidator<SolicitarAprovacaoChamadoRequest>
{
    public SolicitarAprovacaoChamadoRequestValidator()
    {
        RuleFor(x => x.TipoOrigem)
            .IsInEnum().WithMessage("TipoOrigem informado e invalido.")
            .Must(x => x != 0).WithMessage("TipoOrigem e obrigatorio.");

        RuleFor(x => x.OrigemDescricao)
            .MaximumLength(300)
            .When(x => !string.IsNullOrWhiteSpace(x.OrigemDescricao))
            .WithMessage("OrigemDescricao deve ter no maximo 300 caracteres.");

        RuleFor(x => x.JustificativaSolicitacao)
            .MaximumLength(4000)
            .When(x => !string.IsNullOrWhiteSpace(x.JustificativaSolicitacao))
            .WithMessage("JustificativaSolicitacao deve ter no maximo 4000 caracteres.");
    }
}

public sealed class DecidirAprovacaoChamadoRequestValidator : AbstractValidator<DecidirAprovacaoChamadoRequest>
{
    public DecidirAprovacaoChamadoRequestValidator()
    {
        RuleFor(x => x.JustificativaDecisao)
            .MaximumLength(4000)
            .When(x => !string.IsNullOrWhiteSpace(x.JustificativaDecisao))
            .WithMessage("JustificativaDecisao deve ter no maximo 4000 caracteres.");
    }
}

public sealed class CancelarAprovacaoChamadoRequestValidator : AbstractValidator<CancelarAprovacaoChamadoRequest>
{
    public CancelarAprovacaoChamadoRequestValidator()
    {
        RuleFor(x => x.JustificativaDecisao)
            .NotEmpty().WithMessage("JustificativaDecisao e obrigatoria para cancelamento.")
            .MaximumLength(4000).WithMessage("JustificativaDecisao deve ter no maximo 4000 caracteres.");
    }
}

public sealed class FiltroAprovacaoChamadoRequestValidator : AbstractValidator<FiltroAprovacaoChamadoRequest>
{
    private static readonly string[] CamposOrdenacao =
    [
        "solicitadaem",
        "decididaem",
        "status",
        "tipoorigem",
        "codigo",
        "titulo"
    ];

    public FiltroAprovacaoChamadoRequestValidator()
    {
        RuleFor(x => x.ChamadoId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("ChamadoId informado e invalido.");

        RuleFor(x => x.SolicitanteId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("SolicitanteId informado e invalido.");

        RuleFor(x => x.AprovadorId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("AprovadorId informado e invalido.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .When(x => x.Status.HasValue)
            .WithMessage("Status informado e invalido.");

        RuleFor(x => x.TipoOrigem)
            .IsInEnum()
            .When(x => x.TipoOrigem.HasValue)
            .WithMessage("TipoOrigem informado e invalido.");

        RuleFor(x => x.DataSolicitacaoFinal)
            .GreaterThanOrEqualTo(x => x.DataSolicitacaoInicial!.Value.Date)
            .When(x => x.DataSolicitacaoInicial.HasValue && x.DataSolicitacaoFinal.HasValue)
            .WithMessage("DataSolicitacaoFinal nao pode ser anterior a DataSolicitacaoInicial.");

        RuleFor(x => x.DataDecisaoFinal)
            .GreaterThanOrEqualTo(x => x.DataDecisaoInicial!.Value.Date)
            .When(x => x.DataDecisaoInicial.HasValue && x.DataDecisaoFinal.HasValue)
            .WithMessage("DataDecisaoFinal nao pode ser anterior a DataDecisaoInicial.");

        RuleFor(x => x.Termo)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Termo))
            .WithMessage("Termo deve ter no maximo 200 caracteres.");

        RuleFor(x => x.Pagina)
            .GreaterThanOrEqualTo(1).WithMessage("Pagina deve ser maior ou igual a 1.");

        RuleFor(x => x.TamanhoPagina)
            .InclusiveBetween(1, 100).WithMessage("TamanhoPagina deve estar entre 1 e 100.");

        RuleFor(x => x.OrdenarPor)
            .Must(valor => string.IsNullOrWhiteSpace(valor) || CamposOrdenacao.Contains(valor.Trim().ToLowerInvariant()))
            .WithMessage("OrdenarPor deve ser solicitadaEm, decididaEm, status, tipoOrigem, codigo ou titulo.");

        RuleFor(x => x.DirecaoOrdenacao)
            .Must(valor => string.IsNullOrWhiteSpace(valor) ||
                           string.Equals(valor.Trim(), "asc", StringComparison.OrdinalIgnoreCase) ||
                           string.Equals(valor.Trim(), "desc", StringComparison.OrdinalIgnoreCase))
            .WithMessage("DirecaoOrdenacao deve ser asc ou desc.");
    }
}
