using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class ListarInstanciasAprovacaoChamadoRequestValidator : AbstractValidator<ListarInstanciasAprovacaoChamadoRequest>
{
    private static readonly string[] CamposOrdenacao =
    [
        "titulo",
        "status",
        "origem",
        "solicitadaem",
        "deveexpirarem",
        "decididaem",
        "criadoem",
        "atualizadoem"
    ];

    public ListarInstanciasAprovacaoChamadoRequestValidator()
    {
        RuleFor(x => x.ChamadoId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("ChamadoId informado e invalido.");

        RuleFor(x => x.ConfiguracaoRegraAprovacaoId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("ConfiguracaoRegraAprovacaoId informado e invalido.");

        RuleFor(x => x.AprovacaoChamadoLegadaId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("AprovacaoChamadoLegadaId informado e invalido.");

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

        RuleFor(x => x.AprovadorResolvidoUsuarioId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("AprovadorResolvidoUsuarioId informado e invalido.");

        RuleFor(x => x.SolicitanteId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("SolicitanteId informado e invalido.");

        RuleFor(x => x.Termo)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Termo))
            .WithMessage("Termo deve ter no maximo 500 caracteres.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .When(x => x.Status.HasValue)
            .WithMessage("Status informado e invalido.");

        RuleFor(x => x.Origem)
            .IsInEnum()
            .When(x => x.Origem.HasValue)
            .WithMessage("Origem informada e invalida.");

        RuleFor(x => x.TipoFluxoAprovacao)
            .IsInEnum()
            .When(x => x.TipoFluxoAprovacao.HasValue)
            .WithMessage("TipoFluxoAprovacao informado e invalido.");

        RuleFor(x => x.EfeitoOperacional)
            .IsInEnum()
            .When(x => x.EfeitoOperacional.HasValue)
            .WithMessage("EfeitoOperacional informado e invalido.");

        RuleFor(x => x.EscopoRegra)
            .IsInEnum()
            .When(x => x.EscopoRegra.HasValue)
            .WithMessage("EscopoRegra informado e invalido.");

        RuleFor(x => x.TipoRegra)
            .IsInEnum()
            .When(x => x.TipoRegra.HasValue)
            .WithMessage("TipoRegra informado e invalido.");

        RuleFor(x => x.NaturezaChamado)
            .IsInEnum()
            .When(x => x.NaturezaChamado.HasValue)
            .WithMessage("NaturezaChamado informada e invalida.");

        RuleFor(x => x.ImpactoAvaliado)
            .IsInEnum()
            .When(x => x.ImpactoAvaliado.HasValue)
            .WithMessage("ImpactoAvaliado informado e invalido.");

        RuleFor(x => x.UrgenciaAvaliada)
            .IsInEnum()
            .When(x => x.UrgenciaAvaliada.HasValue)
            .WithMessage("UrgenciaAvaliada informada e invalida.");

        RuleFor(x => x.PrioridadeAvaliada)
            .IsInEnum()
            .When(x => x.PrioridadeAvaliada.HasValue)
            .WithMessage("PrioridadeAvaliada informada e invalida.");

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
            .WithMessage("OrdenarPor deve ser titulo, status, origem, solicitadaEm, deveExpirarEm, decididaEm, criadoEm ou atualizadoEm.");

        RuleFor(x => x.DirecaoOrdenacao)
            .Must(valor => string.IsNullOrWhiteSpace(valor) ||
                           string.Equals(valor.Trim(), "asc", StringComparison.OrdinalIgnoreCase) ||
                           string.Equals(valor.Trim(), "desc", StringComparison.OrdinalIgnoreCase))
            .WithMessage("DirecaoOrdenacao deve ser asc ou desc.");

        RuleFor(x => x.SolicitadaAte)
            .GreaterThanOrEqualTo(x => x.SolicitadaDe!.Value)
            .When(x => x.SolicitadaDe.HasValue && x.SolicitadaAte.HasValue)
            .WithMessage("SolicitadaAte nao pode ser anterior a SolicitadaDe.");

        RuleFor(x => x.DeveExpirarAte)
            .GreaterThanOrEqualTo(x => x.DeveExpirarDe!.Value)
            .When(x => x.DeveExpirarDe.HasValue && x.DeveExpirarAte.HasValue)
            .WithMessage("DeveExpirarAte nao pode ser anterior a DeveExpirarDe.");
    }
}

public sealed class PrepararInstanciaAprovacaoChamadoRequestValidator : AbstractValidator<PrepararInstanciaAprovacaoChamadoRequest>
{
    public PrepararInstanciaAprovacaoChamadoRequestValidator()
    {
        RuleFor(x => x.ChamadoId)
            .NotEmpty()
            .WithMessage("ChamadoId e obrigatorio.");

        RuleFor(x => x.SolicitanteId)
            .NotEmpty()
            .WithMessage("SolicitanteId e obrigatorio.");

        RuleFor(x => x.ConfiguracaoRegraAprovacaoId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("ConfiguracaoRegraAprovacaoId informado e invalido.");

        RuleFor(x => x.AprovacaoChamadoLegadaId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("AprovacaoChamadoLegadaId informado e invalido.");

        RuleFor(x => x.Titulo)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Titulo))
            .WithMessage("Titulo deve ter no maximo 200 caracteres.");

        RuleFor(x => x.Descricao)
            .MaximumLength(4000)
            .When(x => !string.IsNullOrWhiteSpace(x.Descricao))
            .WithMessage("Descricao deve ter no maximo 4000 caracteres.");

        RuleFor(x => x.Origem)
            .IsInEnum()
            .WithMessage("Origem informada e invalida.");

        RuleFor(x => x.TipoFluxoAprovacao)
            .IsInEnum()
            .When(x => x.TipoFluxoAprovacao.HasValue)
            .WithMessage("TipoFluxoAprovacao informado e invalido.");

        RuleFor(x => x.EfeitoOperacional)
            .IsInEnum()
            .When(x => x.EfeitoOperacional.HasValue)
            .WithMessage("EfeitoOperacional informado e invalido.");

        RuleFor(x => x.EscopoRegra)
            .IsInEnum()
            .When(x => x.EscopoRegra.HasValue)
            .WithMessage("EscopoRegra informado e invalido.");

        RuleFor(x => x.TipoRegra)
            .IsInEnum()
            .When(x => x.TipoRegra.HasValue)
            .WithMessage("TipoRegra informado e invalido.");

        RuleFor(x => x.NaturezaChamado)
            .IsInEnum()
            .When(x => x.NaturezaChamado.HasValue)
            .WithMessage("NaturezaChamado informada e invalida.");

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

        RuleFor(x => x.ImpactoAvaliado)
            .IsInEnum()
            .When(x => x.ImpactoAvaliado.HasValue)
            .WithMessage("ImpactoAvaliado informado e invalido.");

        RuleFor(x => x.UrgenciaAvaliada)
            .IsInEnum()
            .When(x => x.UrgenciaAvaliada.HasValue)
            .WithMessage("UrgenciaAvaliada informada e invalida.");

        RuleFor(x => x.PrioridadeAvaliada)
            .IsInEnum()
            .When(x => x.PrioridadeAvaliada.HasValue)
            .WithMessage("PrioridadeAvaliada informada e invalida.");

        RuleFor(x => x.CustoAvaliado)
            .GreaterThanOrEqualTo(0m)
            .When(x => x.CustoAvaliado.HasValue)
            .WithMessage("CustoAvaliado nao pode ser negativo.");

        RuleFor(x => x.NivelRiscoAvaliado)
            .GreaterThan(0)
            .When(x => x.NivelRiscoAvaliado.HasValue)
            .WithMessage("NivelRiscoAvaliado deve ser maior que zero.");

        RuleFor(x => x.TipoResolucaoAprovador)
            .IsInEnum()
            .When(x => x.TipoResolucaoAprovador.HasValue)
            .WithMessage("TipoResolucaoAprovador informado e invalido.");

        RuleFor(x => x.AprovadorEspecificoUsuarioId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("AprovadorEspecificoUsuarioId informado e invalido.");

        RuleFor(x => x.AprovadorPadraoUsuarioId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("AprovadorPadraoUsuarioId informado e invalido.");

        RuleFor(x => x.AprovadorResolvidoUsuarioId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("AprovadorResolvidoUsuarioId informado e invalido.");

        RuleFor(x => x.PrazoDecisaoHoras)
            .GreaterThan(0)
            .When(x => x.PrazoDecisaoHoras.HasValue)
            .WithMessage("PrazoDecisaoHoras deve ser maior que zero.");

        RuleFor(x => x.DeveExpirarEm)
            .Must((x, deveExpirarEm) => !deveExpirarEm.HasValue || !x.SolicitadaEm.HasValue || deveExpirarEm.Value >= x.SolicitadaEm.Value)
            .WithMessage("DeveExpirarEm nao pode ser anterior a SolicitadaEm.");

        RuleFor(x => x.RegraNomeSnapshot)
            .MaximumLength(180)
            .When(x => !string.IsNullOrWhiteSpace(x.RegraNomeSnapshot))
            .WithMessage("RegraNomeSnapshot deve ter no maximo 180 caracteres.");

        RuleFor(x => x.RegraVersaoSnapshot)
            .GreaterThan(0)
            .When(x => x.RegraVersaoSnapshot.HasValue)
            .WithMessage("RegraVersaoSnapshot deve ser maior que zero.");

        RuleFor(x => x.RegraCriterioSnapshot)
            .MaximumLength(4000)
            .When(x => !string.IsNullOrWhiteSpace(x.RegraCriterioSnapshot))
            .WithMessage("RegraCriterioSnapshot deve ter no maximo 4000 caracteres.");

        RuleFor(x => x)
            .Must(x => x.EfeitoOperacional != EfeitoOperacionalRegraAprovacao.Sinalizar || x.Bloqueante != true)
            .WithMessage("Instancia informativa nao deve ser bloqueante.");

        RuleFor(x => x)
            .Must(x => x.Bloqueante != true || x.ExigeAprovacao != false)
            .WithMessage("Instancia bloqueante deve exigir aprovacao.");

        RuleFor(x => x)
            .Must(x => x.TipoResolucaoAprovador != TipoResolucaoAprovadorRegraAprovacao.AprovadorEspecifico || x.AprovadorEspecificoUsuarioId.HasValue)
            .WithMessage("AprovadorEspecificoUsuarioId e obrigatorio quando TipoResolucaoAprovador for AprovadorEspecifico.");

        RuleFor(x => x)
            .Must(x => x.TipoResolucaoAprovador != TipoResolucaoAprovadorRegraAprovacao.AprovadorPadrao || x.AprovadorPadraoUsuarioId.HasValue)
            .WithMessage("AprovadorPadraoUsuarioId e obrigatorio quando TipoResolucaoAprovador for AprovadorPadrao.");
    }
}

public sealed class CriarInstanciaAprovacaoChamadoManualRequestValidator : AbstractValidator<CriarInstanciaAprovacaoChamadoManualRequest>
{
    public CriarInstanciaAprovacaoChamadoManualRequestValidator()
    {
        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("Titulo e obrigatorio.")
            .MaximumLength(200).WithMessage("Titulo deve ter no maximo 200 caracteres.");

        RuleFor(x => x.Descricao)
            .MaximumLength(4000)
            .When(x => !string.IsNullOrWhiteSpace(x.Descricao))
            .WithMessage("Descricao deve ter no maximo 4000 caracteres.");

        RuleFor(x => x.ChamadoId)
            .NotEmpty()
            .WithMessage("ChamadoId e obrigatorio.");

        RuleFor(x => x.SolicitanteId)
            .NotEmpty()
            .WithMessage("SolicitanteId e obrigatorio.");

        RuleFor(x => x.ConfiguracaoRegraAprovacaoId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("ConfiguracaoRegraAprovacaoId informado e invalido.");

        RuleFor(x => x.AprovacaoChamadoLegadaId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("AprovacaoChamadoLegadaId informado e invalido.");

        RuleFor(x => x.Origem)
            .IsInEnum()
            .WithMessage("Origem informada e invalida.");

        RuleFor(x => x.TipoFluxoAprovacao)
            .IsInEnum()
            .WithMessage("TipoFluxoAprovacao informado e invalido.");

        RuleFor(x => x.EfeitoOperacional)
            .IsInEnum()
            .WithMessage("EfeitoOperacional informado e invalido.");

        RuleFor(x => x.EscopoRegra)
            .IsInEnum()
            .WithMessage("EscopoRegra informado e invalido.");

        RuleFor(x => x.TipoRegra)
            .IsInEnum()
            .WithMessage("TipoRegra informado e invalido.");

        RuleFor(x => x.NaturezaChamado)
            .IsInEnum()
            .When(x => x.NaturezaChamado.HasValue)
            .WithMessage("NaturezaChamado informada e invalida.");

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

        RuleFor(x => x.ImpactoAvaliado)
            .IsInEnum()
            .When(x => x.ImpactoAvaliado.HasValue)
            .WithMessage("ImpactoAvaliado informado e invalido.");

        RuleFor(x => x.UrgenciaAvaliada)
            .IsInEnum()
            .When(x => x.UrgenciaAvaliada.HasValue)
            .WithMessage("UrgenciaAvaliada informada e invalida.");

        RuleFor(x => x.PrioridadeAvaliada)
            .IsInEnum()
            .When(x => x.PrioridadeAvaliada.HasValue)
            .WithMessage("PrioridadeAvaliada informada e invalida.");

        RuleFor(x => x.CustoAvaliado)
            .GreaterThanOrEqualTo(0m)
            .When(x => x.CustoAvaliado.HasValue)
            .WithMessage("CustoAvaliado nao pode ser negativo.");

        RuleFor(x => x.NivelRiscoAvaliado)
            .GreaterThan(0)
            .When(x => x.NivelRiscoAvaliado.HasValue)
            .WithMessage("NivelRiscoAvaliado deve ser maior que zero.");

        RuleFor(x => x.TipoResolucaoAprovador)
            .IsInEnum()
            .WithMessage("TipoResolucaoAprovador informado e invalido.");

        RuleFor(x => x.AprovadorEspecificoUsuarioId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("AprovadorEspecificoUsuarioId informado e invalido.");

        RuleFor(x => x.AprovadorPadraoUsuarioId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("AprovadorPadraoUsuarioId informado e invalido.");

        RuleFor(x => x.AprovadorResolvidoUsuarioId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("AprovadorResolvidoUsuarioId informado e invalido.");

        RuleFor(x => x.PrazoDecisaoHoras)
            .GreaterThan(0)
            .When(x => x.PrazoDecisaoHoras.HasValue)
            .WithMessage("PrazoDecisaoHoras deve ser maior que zero.");

        RuleFor(x => x.RegraNomeSnapshot)
            .MaximumLength(180)
            .When(x => !string.IsNullOrWhiteSpace(x.RegraNomeSnapshot))
            .WithMessage("RegraNomeSnapshot deve ter no maximo 180 caracteres.");

        RuleFor(x => x.RegraVersaoSnapshot)
            .GreaterThan(0)
            .When(x => x.RegraVersaoSnapshot.HasValue)
            .WithMessage("RegraVersaoSnapshot deve ser maior que zero.");

        RuleFor(x => x.RegraCriterioSnapshot)
            .MaximumLength(4000)
            .When(x => !string.IsNullOrWhiteSpace(x.RegraCriterioSnapshot))
            .WithMessage("RegraCriterioSnapshot deve ter no maximo 4000 caracteres.");

        RuleFor(x => x)
            .Must(x => x.EfeitoOperacional != EfeitoOperacionalRegraAprovacao.Sinalizar || !x.Bloqueante)
            .WithMessage("Instancia informativa nao deve ser bloqueante.");

        RuleFor(x => x)
            .Must(x => !x.Bloqueante || x.ExigeAprovacao)
            .WithMessage("Instancia bloqueante deve exigir aprovacao.");

        RuleFor(x => x)
            .Must(x => x.TipoResolucaoAprovador != TipoResolucaoAprovadorRegraAprovacao.AprovadorEspecifico || x.AprovadorEspecificoUsuarioId.HasValue)
            .WithMessage("AprovadorEspecificoUsuarioId e obrigatorio quando TipoResolucaoAprovador for AprovadorEspecifico.");

        RuleFor(x => x)
            .Must(x => x.TipoResolucaoAprovador != TipoResolucaoAprovadorRegraAprovacao.AprovadorPadrao || x.AprovadorPadraoUsuarioId.HasValue)
            .WithMessage("AprovadorPadraoUsuarioId e obrigatorio quando TipoResolucaoAprovador for AprovadorPadrao.");
    }
}

public sealed class ValidarInstanciaAprovacaoChamadoRequestValidator : AbstractValidator<ValidarInstanciaAprovacaoChamadoRequest>
{
    public ValidarInstanciaAprovacaoChamadoRequestValidator()
    {
        RuleFor(x => x.InstanciaAprovacaoChamadoId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("InstanciaAprovacaoChamadoId informado e invalido.");

        RuleFor(x => x.Instancia)
            .NotNull()
            .WithMessage("Instancia e obrigatoria.");

        RuleFor(x => x.Instancia)
            .SetValidator(new PrepararInstanciaAprovacaoChamadoRequestValidator());
    }
}
