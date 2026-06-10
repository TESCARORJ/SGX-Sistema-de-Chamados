using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class ListarConfiguracoesRegrasAprovacaoRequestValidator : AbstractValidator<ListarConfiguracoesRegrasAprovacaoRequest>
{
    private static readonly string[] CamposOrdenacao =
    [
        "nome",
        "prioridade",
        "ordem",
        "versao",
        "criadoem",
        "atualizadoem",
        "vigentede",
        "vigenteate"
    ];

    public ListarConfiguracoesRegrasAprovacaoRequestValidator()
    {
        RuleFor(x => x.Termo)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Termo))
            .WithMessage("Termo deve ter no maximo 500 caracteres.");

        RuleFor(x => x.TipoSolicitacaoId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("TipoSolicitacaoId informado e invalido.");

        RuleFor(x => x.CatalogoServicoId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("CatalogoServicoId informado e invalido.");

        RuleFor(x => x.CategoriaId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("CategoriaId informado e invalido.");

        RuleFor(x => x.SubcategoriaId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("SubcategoriaId informado e invalido.");

        RuleFor(x => x.TipoRegra)
            .IsInEnum()
            .When(x => x.TipoRegra.HasValue)
            .WithMessage("TipoRegra informado e invalido.");

        RuleFor(x => x.EscopoRegra)
            .IsInEnum()
            .When(x => x.EscopoRegra.HasValue)
            .WithMessage("EscopoRegra informado e invalido.");

        RuleFor(x => x.NaturezaChamado)
            .IsInEnum()
            .When(x => x.NaturezaChamado.HasValue)
            .WithMessage("NaturezaChamado informada e invalida.");

        RuleFor(x => x.EfeitoOperacional)
            .IsInEnum()
            .When(x => x.EfeitoOperacional.HasValue)
            .WithMessage("EfeitoOperacional informado e invalido.");

        RuleFor(x => x.TipoFluxoAprovacao)
            .IsInEnum()
            .When(x => x.TipoFluxoAprovacao.HasValue)
            .WithMessage("TipoFluxoAprovacao informado e invalido.");

        RuleFor(x => x.TipoResolucaoAprovador)
            .IsInEnum()
            .When(x => x.TipoResolucaoAprovador.HasValue)
            .WithMessage("TipoResolucaoAprovador informado e invalido.");

        RuleFor(x => x.Pagina)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Pagina deve ser maior ou igual a 1.");

        RuleFor(x => x.TamanhoPagina)
            .InclusiveBetween(1, 100)
            .WithMessage("TamanhoPagina deve estar entre 1 e 100.");

        RuleFor(x => x.OrdenarPor)
            .Must(valor => string.IsNullOrWhiteSpace(valor) || CamposOrdenacao.Contains(valor.Trim().ToLowerInvariant()))
            .WithMessage("OrdenarPor deve ser nome, prioridade, ordem, versao, criadoEm, atualizadoEm, vigenteDe ou vigenteAte.");

        RuleFor(x => x.DirecaoOrdenacao)
            .Must(valor => string.IsNullOrWhiteSpace(valor) ||
                           string.Equals(valor.Trim(), "asc", StringComparison.OrdinalIgnoreCase) ||
                           string.Equals(valor.Trim(), "desc", StringComparison.OrdinalIgnoreCase))
            .WithMessage("DirecaoOrdenacao deve ser asc ou desc.");
    }
}

public sealed class CriarConfiguracaoRegraAprovacaoRequestValidator : AbstractValidator<CriarConfiguracaoRegraAprovacaoRequest>
{
    public CriarConfiguracaoRegraAprovacaoRequestValidator()
    {
        AplicarRegrasComuns();
    }

    private void AplicarRegrasComuns()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome e obrigatorio.")
            .MaximumLength(180).WithMessage("Nome deve ter no maximo 180 caracteres.");

        RuleFor(x => x.Descricao)
            .MaximumLength(4000)
            .When(x => !string.IsNullOrWhiteSpace(x.Descricao))
            .WithMessage("Descricao deve ter no maximo 4000 caracteres.");

        RuleFor(x => x.TipoRegra)
            .IsInEnum()
            .WithMessage("TipoRegra informado e invalido.");

        RuleFor(x => x.EscopoRegra)
            .IsInEnum()
            .WithMessage("EscopoRegra informado e invalido.");

        RuleFor(x => x.Ordem)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Ordem nao pode ser negativa.");

        RuleFor(x => x.Prioridade)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Prioridade nao pode ser negativa.");

        RuleFor(x => x.Versao)
            .GreaterThan(0)
            .WithMessage("Versao deve ser maior que zero.");

        RuleFor(x => x.TipoSolicitacaoId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("TipoSolicitacaoId informado e invalido.");

        RuleFor(x => x.CatalogoServicoId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("CatalogoServicoId informado e invalido.");

        RuleFor(x => x.CategoriaId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("CategoriaId informado e invalido.");

        RuleFor(x => x.SubcategoriaId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("SubcategoriaId informada e invalida.");

        RuleFor(x => x)
            .Must(x => !x.SubcategoriaId.HasValue || x.CategoriaId.HasValue)
            .WithMessage("CategoriaId e obrigatoria quando SubcategoriaId for informada.");

        RuleFor(x => x.CustoMinimo)
            .GreaterThanOrEqualTo(0m)
            .When(x => x.CustoMinimo.HasValue)
            .WithMessage("CustoMinimo nao pode ser negativo.");

        RuleFor(x => x.NivelRiscoMinimo)
            .GreaterThan(0)
            .When(x => x.NivelRiscoMinimo.HasValue)
            .WithMessage("NivelRiscoMinimo deve ser maior que zero.");

        RuleFor(x => x.PrazoDecisaoHoras)
            .GreaterThan(0)
            .When(x => x.PrazoDecisaoHoras.HasValue)
            .WithMessage("PrazoDecisaoHoras deve ser maior que zero.");

        RuleFor(x => x.VigenteAte)
            .GreaterThanOrEqualTo(x => x.VigenteDe!.Value)
            .When(x => x.VigenteDe.HasValue && x.VigenteAte.HasValue)
            .WithMessage("VigenteAte nao pode ser anterior a VigenteDe.");

        RuleFor(x => x.EfeitoOperacional)
            .IsInEnum()
            .WithMessage("EfeitoOperacional informado e invalido.");

        RuleFor(x => x.TipoFluxoAprovacao)
            .IsInEnum()
            .WithMessage("TipoFluxoAprovacao informado e invalido.");

        RuleFor(x => x.TipoResolucaoAprovador)
            .IsInEnum()
            .WithMessage("TipoResolucaoAprovador informado e invalido.");

        RuleFor(x => x.AprovadorEspecificoUsuarioId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("AprovadorEspecificoUsuarioId informado e invalido.");

        RuleFor(x => x.AprovadorPadraoUsuarioId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("AprovadorPadraoUsuarioId informado e invalido.");

        RuleFor(x => x)
            .Must(x => x.EfeitoOperacional != EfeitoOperacionalRegraAprovacao.Sinalizar || !x.Bloqueante)
            .WithMessage("Regra informativa nao deve ser bloqueante.");

        RuleFor(x => x)
            .Must(x => !x.Bloqueante || x.ExigeAprovacao)
            .WithMessage("Regra bloqueante deve exigir aprovacao.");

        RuleFor(x => x)
            .Must(x => x.TipoResolucaoAprovador != TipoResolucaoAprovadorRegraAprovacao.AprovadorEspecifico || x.AprovadorEspecificoUsuarioId.HasValue)
            .WithMessage("AprovadorEspecificoUsuarioId e obrigatorio quando TipoResolucaoAprovador for AprovadorEspecifico.");

        RuleFor(x => x)
            .Must(x => x.TipoResolucaoAprovador != TipoResolucaoAprovadorRegraAprovacao.AprovadorPadrao || x.AprovadorPadraoUsuarioId.HasValue)
            .WithMessage("AprovadorPadraoUsuarioId e obrigatorio quando TipoResolucaoAprovador for AprovadorPadrao.");
    }
}

public sealed class AtualizarConfiguracaoRegraAprovacaoRequestValidator : AbstractValidator<AtualizarConfiguracaoRegraAprovacaoRequest>
{
    public AtualizarConfiguracaoRegraAprovacaoRequestValidator()
    {
        Include(new AtualizarConfiguracaoRegraAprovacaoInternalValidator());
    }

    private sealed class AtualizarConfiguracaoRegraAprovacaoInternalValidator : AbstractValidator<AtualizarConfiguracaoRegraAprovacaoRequest>
    {
        public AtualizarConfiguracaoRegraAprovacaoInternalValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("Nome e obrigatorio.")
                .MaximumLength(180).WithMessage("Nome deve ter no maximo 180 caracteres.");

            RuleFor(x => x.Descricao)
                .MaximumLength(4000)
                .When(x => !string.IsNullOrWhiteSpace(x.Descricao))
                .WithMessage("Descricao deve ter no maximo 4000 caracteres.");

            RuleFor(x => x.TipoRegra)
                .IsInEnum()
                .WithMessage("TipoRegra informado e invalido.");

            RuleFor(x => x.EscopoRegra)
                .IsInEnum()
                .WithMessage("EscopoRegra informado e invalido.");

            RuleFor(x => x.Ordem)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Ordem nao pode ser negativa.");

            RuleFor(x => x.Prioridade)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Prioridade nao pode ser negativa.");

            RuleFor(x => x.Versao)
                .GreaterThan(0)
                .WithMessage("Versao deve ser maior que zero.");

            RuleFor(x => x.TipoSolicitacaoId)
                .Must(x => !x.HasValue || x.Value != Guid.Empty)
                .WithMessage("TipoSolicitacaoId informado e invalido.");

            RuleFor(x => x.CatalogoServicoId)
                .Must(x => !x.HasValue || x.Value != Guid.Empty)
                .WithMessage("CatalogoServicoId informado e invalido.");

            RuleFor(x => x.CategoriaId)
                .Must(x => !x.HasValue || x.Value != Guid.Empty)
                .WithMessage("CategoriaId informado e invalido.");

            RuleFor(x => x.SubcategoriaId)
                .Must(x => !x.HasValue || x.Value != Guid.Empty)
                .WithMessage("SubcategoriaId informada e invalida.");

            RuleFor(x => x)
                .Must(x => !x.SubcategoriaId.HasValue || x.CategoriaId.HasValue)
                .WithMessage("CategoriaId e obrigatoria quando SubcategoriaId for informada.");

            RuleFor(x => x.CustoMinimo)
                .GreaterThanOrEqualTo(0m)
                .When(x => x.CustoMinimo.HasValue)
                .WithMessage("CustoMinimo nao pode ser negativo.");

            RuleFor(x => x.NivelRiscoMinimo)
                .GreaterThan(0)
                .When(x => x.NivelRiscoMinimo.HasValue)
                .WithMessage("NivelRiscoMinimo deve ser maior que zero.");

            RuleFor(x => x.PrazoDecisaoHoras)
                .GreaterThan(0)
                .When(x => x.PrazoDecisaoHoras.HasValue)
                .WithMessage("PrazoDecisaoHoras deve ser maior que zero.");

            RuleFor(x => x.VigenteAte)
                .GreaterThanOrEqualTo(x => x.VigenteDe!.Value)
                .When(x => x.VigenteDe.HasValue && x.VigenteAte.HasValue)
                .WithMessage("VigenteAte nao pode ser anterior a VigenteDe.");

            RuleFor(x => x.EfeitoOperacional)
                .IsInEnum()
                .WithMessage("EfeitoOperacional informado e invalido.");

            RuleFor(x => x.TipoFluxoAprovacao)
                .IsInEnum()
                .WithMessage("TipoFluxoAprovacao informado e invalido.");

            RuleFor(x => x.TipoResolucaoAprovador)
                .IsInEnum()
                .WithMessage("TipoResolucaoAprovador informado e invalido.");

            RuleFor(x => x.AprovadorEspecificoUsuarioId)
                .Must(x => !x.HasValue || x.Value != Guid.Empty)
                .WithMessage("AprovadorEspecificoUsuarioId informado e invalido.");

            RuleFor(x => x.AprovadorPadraoUsuarioId)
                .Must(x => !x.HasValue || x.Value != Guid.Empty)
                .WithMessage("AprovadorPadraoUsuarioId informado e invalido.");

            RuleFor(x => x)
                .Must(x => x.EfeitoOperacional != EfeitoOperacionalRegraAprovacao.Sinalizar || !x.Bloqueante)
                .WithMessage("Regra informativa nao deve ser bloqueante.");

            RuleFor(x => x)
                .Must(x => !x.Bloqueante || x.ExigeAprovacao)
                .WithMessage("Regra bloqueante deve exigir aprovacao.");

            RuleFor(x => x)
                .Must(x => x.TipoResolucaoAprovador != TipoResolucaoAprovadorRegraAprovacao.AprovadorEspecifico || x.AprovadorEspecificoUsuarioId.HasValue)
                .WithMessage("AprovadorEspecificoUsuarioId e obrigatorio quando TipoResolucaoAprovador for AprovadorEspecifico.");

            RuleFor(x => x)
                .Must(x => x.TipoResolucaoAprovador != TipoResolucaoAprovadorRegraAprovacao.AprovadorPadrao || x.AprovadorPadraoUsuarioId.HasValue)
                .WithMessage("AprovadorPadraoUsuarioId e obrigatorio quando TipoResolucaoAprovador for AprovadorPadrao.");
        }
    }
}

public sealed class AlterarStatusConfiguracaoRegraAprovacaoRequestValidator : AbstractValidator<AlterarStatusConfiguracaoRegraAprovacaoRequest>
{
    public AlterarStatusConfiguracaoRegraAprovacaoRequestValidator()
    {
    }
}

public sealed class ValidarConfiguracaoRegraAprovacaoRequestValidator : AbstractValidator<ValidarConfiguracaoRegraAprovacaoRequest>
{
    public ValidarConfiguracaoRegraAprovacaoRequestValidator()
    {
        RuleFor(x => x.ConfiguracaoRegraAprovacaoId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("ConfiguracaoRegraAprovacaoId informado e invalido.");

        RuleFor(x => x.Configuracao)
            .NotNull()
            .WithMessage("Configuracao e obrigatoria.");

        RuleFor(x => x.Configuracao)
            .SetValidator(new CriarConfiguracaoRegraAprovacaoRequestValidator());
    }
}

public sealed class ContextoAvaliacaoRegraAprovacaoRequestValidator : AbstractValidator<ContextoAvaliacaoRegraAprovacaoRequest>
{
    public ContextoAvaliacaoRegraAprovacaoRequestValidator()
    {
        RuleFor(x => x.TipoSolicitacaoId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("TipoSolicitacaoId informado e invalido.");

        RuleFor(x => x.CatalogoServicoId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("CatalogoServicoId informado e invalido.");

        RuleFor(x => x.CategoriaId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("CategoriaId informado e invalido.");

        RuleFor(x => x.SubcategoriaId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("SubcategoriaId informada e invalida.");

        RuleFor(x => x)
            .Must(x => !x.SubcategoriaId.HasValue || x.CategoriaId.HasValue)
            .WithMessage("CategoriaId e obrigatoria quando SubcategoriaId for informada.");

        RuleFor(x => x.NaturezaChamado)
            .IsInEnum()
            .When(x => x.NaturezaChamado.HasValue)
            .WithMessage("NaturezaChamado informada e invalida.");

        RuleFor(x => x.ImpactoChamado)
            .IsInEnum()
            .When(x => x.ImpactoChamado.HasValue)
            .WithMessage("ImpactoChamado informado e invalido.");

        RuleFor(x => x.UrgenciaChamado)
            .IsInEnum()
            .When(x => x.UrgenciaChamado.HasValue)
            .WithMessage("UrgenciaChamado informada e invalida.");

        RuleFor(x => x.PrioridadeChamado)
            .IsInEnum()
            .When(x => x.PrioridadeChamado.HasValue)
            .WithMessage("PrioridadeChamado informada e invalida.");

        RuleFor(x => x.Custo)
            .GreaterThanOrEqualTo(0m)
            .When(x => x.Custo.HasValue)
            .WithMessage("Custo nao pode ser negativo.");

        RuleFor(x => x.NivelRisco)
            .GreaterThan(0)
            .When(x => x.NivelRisco.HasValue)
            .WithMessage("NivelRisco deve ser maior que zero.");
    }
}
