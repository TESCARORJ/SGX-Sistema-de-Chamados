using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class ListarDecisoesAprovacaoChamadoRequestValidator : AbstractValidator<ListarDecisoesAprovacaoChamadoRequest>
{
    private static readonly string[] CamposOrdenacao =
    [
        "datadecisao",
        "tipodecisao",
        "resultado",
        "criadoem",
        "instanciaaprovacaochamadoid"
    ];

    public ListarDecisoesAprovacaoChamadoRequestValidator()
    {
        RuleFor(x => x.InstanciaAprovacaoChamadoId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("InstanciaAprovacaoChamadoId informado e invalido.");

        RuleFor(x => x.EtapaAprovacaoChamadoId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("EtapaAprovacaoChamadoId informado e invalido.");

        RuleFor(x => x.ChamadoId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("ChamadoId informado e invalido.");

        RuleFor(x => x.DecisorUsuarioId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("DecisorUsuarioId informado e invalido.");

        RuleFor(x => x.TipoDecisao)
            .IsInEnum()
            .When(x => x.TipoDecisao.HasValue)
            .WithMessage("TipoDecisao informado e invalido.");

        RuleFor(x => x.Resultado)
            .IsInEnum()
            .When(x => x.Resultado.HasValue)
            .WithMessage("Resultado informado e invalido.");

        RuleFor(x => x.EfeitoOperacional)
            .IsInEnum()
            .When(x => x.EfeitoOperacional.HasValue)
            .WithMessage("EfeitoOperacional informado e invalido.");

        RuleFor(x => x.DataDecisaoAte)
            .GreaterThanOrEqualTo(x => x.DataDecisaoDe!.Value.Date)
            .When(x => x.DataDecisaoDe.HasValue && x.DataDecisaoAte.HasValue)
            .WithMessage("DataDecisaoAte nao pode ser anterior a DataDecisaoDe.");

        RuleFor(x => x.Termo)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Termo))
            .WithMessage("Termo deve ter no maximo 500 caracteres.");

        RuleFor(x => x.Pagina)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Pagina deve ser maior ou igual a 1.");

        RuleFor(x => x.TamanhoPagina)
            .InclusiveBetween(1, 100)
            .WithMessage("TamanhoPagina deve estar entre 1 e 100.");

        RuleFor(x => x.OrdenarPor)
            .Must(valor => string.IsNullOrWhiteSpace(valor) || CamposOrdenacao.Contains(valor.Trim().ToLowerInvariant()))
            .WithMessage("OrdenarPor deve ser dataDecisao, tipoDecisao, resultado, criadoEm ou instanciaAprovacaoChamadoId.");

        RuleFor(x => x.DirecaoOrdenacao)
            .Must(valor => string.IsNullOrWhiteSpace(valor) ||
                           string.Equals(valor.Trim(), "asc", StringComparison.OrdinalIgnoreCase) ||
                           string.Equals(valor.Trim(), "desc", StringComparison.OrdinalIgnoreCase))
            .WithMessage("DirecaoOrdenacao deve ser asc ou desc.");
    }
}

public sealed class RegistrarDecisaoAprovacaoChamadoRequestValidator : AbstractValidator<RegistrarDecisaoAprovacaoChamadoRequest>
{
    public RegistrarDecisaoAprovacaoChamadoRequestValidator()
    {
        RuleFor(x => x.InstanciaAprovacaoChamadoId)
            .NotEqual(Guid.Empty)
            .WithMessage("InstanciaAprovacaoChamadoId e obrigatorio.");

        RuleFor(x => x.EtapaAprovacaoChamadoId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("EtapaAprovacaoChamadoId informado e invalido.");

        RuleFor(x => x.DecisorUsuarioId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("DecisorUsuarioId informado e invalido.");

        RuleFor(x => x.TipoDecisao)
            .IsInEnum()
            .WithMessage("TipoDecisao informado e invalido.");

        RuleFor(x => x.Resultado)
            .IsInEnum()
            .WithMessage("Resultado informado e invalido.");

        RuleFor(x => x.EfeitoOperacional)
            .IsInEnum()
            .WithMessage("EfeitoOperacional informado e invalido.");

        RuleFor(x => x.StatusInstanciaAnterior)
            .IsInEnum()
            .WithMessage("StatusInstanciaAnterior informado e invalido.");

        RuleFor(x => x.StatusInstanciaNovo)
            .IsInEnum()
            .WithMessage("StatusInstanciaNovo informado e invalido.");

        RuleFor(x => x.StatusEtapaAnterior)
            .IsInEnum()
            .When(x => x.StatusEtapaAnterior.HasValue)
            .WithMessage("StatusEtapaAnterior informado e invalido.");

        RuleFor(x => x.StatusEtapaNovo)
            .IsInEnum()
            .When(x => x.StatusEtapaNovo.HasValue)
            .WithMessage("StatusEtapaNovo informado e invalido.");

        RuleFor(x => x.PapelDecisorSnapshot)
            .MaximumLength(120)
            .When(x => !string.IsNullOrWhiteSpace(x.PapelDecisorSnapshot))
            .WithMessage("PapelDecisorSnapshot deve ter no maximo 120 caracteres.");

        RuleFor(x => x.AutoridadeDecisorSnapshot)
            .MaximumLength(180)
            .When(x => !string.IsNullOrWhiteSpace(x.AutoridadeDecisorSnapshot))
            .WithMessage("AutoridadeDecisorSnapshot deve ter no maximo 180 caracteres.");

        RuleFor(x => x.GrupoAprovadorSnapshot)
            .MaximumLength(180)
            .When(x => !string.IsNullOrWhiteSpace(x.GrupoAprovadorSnapshot))
            .WithMessage("GrupoAprovadorSnapshot deve ter no maximo 180 caracteres.");

        RuleFor(x => x.Justificativa)
            .MaximumLength(4000)
            .When(x => !string.IsNullOrWhiteSpace(x.Justificativa))
            .WithMessage("Justificativa deve ter no maximo 4000 caracteres.");

        RuleFor(x => x.Observacao)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.Observacao))
            .WithMessage("Observacao deve ter no maximo 2000 caracteres.");

        RuleFor(x => x.EscopoDecididoSnapshot)
            .MaximumLength(4000)
            .When(x => !string.IsNullOrWhiteSpace(x.EscopoDecididoSnapshot))
            .WithMessage("EscopoDecididoSnapshot deve ter no maximo 4000 caracteres.");

        RuleFor(x => x.RegraNomeSnapshot)
            .MaximumLength(180)
            .When(x => !string.IsNullOrWhiteSpace(x.RegraNomeSnapshot))
            .WithMessage("RegraNomeSnapshot deve ter no maximo 180 caracteres.");

        RuleFor(x => x.RegraCriterioSnapshot)
            .MaximumLength(4000)
            .When(x => !string.IsNullOrWhiteSpace(x.RegraCriterioSnapshot))
            .WithMessage("RegraCriterioSnapshot deve ter no maximo 4000 caracteres.");

        RuleFor(x => x.RamoSnapshot)
            .MaximumLength(80)
            .When(x => !string.IsNullOrWhiteSpace(x.RamoSnapshot))
            .WithMessage("RamoSnapshot deve ter no maximo 80 caracteres.");

        RuleFor(x => x.QuorumEsperado)
            .GreaterThan(0)
            .When(x => x.QuorumEsperado.HasValue)
            .WithMessage("QuorumEsperado deve ser maior que zero.");

        RuleFor(x => x.QuorumAtingido)
            .GreaterThan(0)
            .When(x => x.QuorumAtingido.HasValue)
            .WithMessage("QuorumAtingido deve ser maior que zero.");

        RuleFor(x => x)
            .Must(x => !x.QuorumAtingido.HasValue || x.QuorumEsperado.HasValue)
            .WithMessage("QuorumAtingido nao pode ser informado sem QuorumEsperado.");

        RuleFor(x => x.RegraVersaoSnapshot)
            .GreaterThan(0)
            .When(x => x.RegraVersaoSnapshot.HasValue)
            .WithMessage("RegraVersaoSnapshot deve ser maior que zero.");

        RuleFor(x => x.NivelSnapshot)
            .GreaterThan(0)
            .When(x => x.NivelSnapshot.HasValue)
            .WithMessage("NivelSnapshot deve ser maior que zero.");

        RuleFor(x => x.OrdemSnapshot)
            .GreaterThanOrEqualTo(0)
            .When(x => x.OrdemSnapshot.HasValue)
            .WithMessage("OrdemSnapshot nao pode ser negativa.");

        RuleFor(x => x)
            .Must(x => !x.LiberaAvanco || !x.MantemBloqueio)
            .WithMessage("LiberaAvanco e MantemBloqueio nao podem ser verdadeiros ao mesmo tempo.");

        RuleFor(x => x)
            .Must(x => !x.DecisaoParcial || !x.DecisaoFinal)
            .WithMessage("DecisaoParcial e DecisaoFinal nao devem ser verdadeiros ao mesmo tempo.");

        RuleFor(x => x)
            .Must(x => x.EtapaAprovacaoChamadoId.HasValue || (!x.StatusEtapaAnterior.HasValue && !x.StatusEtapaNovo.HasValue))
            .WithMessage("Status de etapa so pode ser informado quando EtapaAprovacaoChamadoId existir.");

        RuleFor(x => x)
            .Must(x => x.EtapaAprovacaoChamadoId.HasValue || (!x.NivelSnapshot.HasValue && !x.OrdemSnapshot.HasValue && string.IsNullOrWhiteSpace(x.RamoSnapshot)))
            .WithMessage("Snapshots estruturais de etapa so podem ser informados quando EtapaAprovacaoChamadoId existir.");

        RuleFor(x => x)
            .Must(ValidarCompatibilidadeTipoResultado)
            .WithMessage("Resultado informado nao e compativel com o TipoDecisao.");
    }

    private static bool ValidarCompatibilidadeTipoResultado(RegistrarDecisaoAprovacaoChamadoRequest request)
        => request.TipoDecisao switch
        {
            TipoDecisaoAprovacaoChamado.Aprovacao => request.Resultado == ResultadoDecisaoAprovacaoChamado.Aprovada,
            TipoDecisaoAprovacaoChamado.Rejeicao => request.Resultado == ResultadoDecisaoAprovacaoChamado.Reprovada ||
                                                    request.Resultado == ResultadoDecisaoAprovacaoChamado.RequerAjuste ||
                                                    request.Resultado == ResultadoDecisaoAprovacaoChamado.RequerNovaAprovacao,
            TipoDecisaoAprovacaoChamado.Cancelamento => request.Resultado == ResultadoDecisaoAprovacaoChamado.Cancelada,
            TipoDecisaoAprovacaoChamado.Expiracao => request.Resultado == ResultadoDecisaoAprovacaoChamado.Expirada,
            TipoDecisaoAprovacaoChamado.Reavaliacao => request.Resultado == ResultadoDecisaoAprovacaoChamado.RequerAjuste ||
                                                       request.Resultado == ResultadoDecisaoAprovacaoChamado.RequerNovaAprovacao,
            TipoDecisaoAprovacaoChamado.Substituicao => request.Resultado == ResultadoDecisaoAprovacaoChamado.SemEfeitoOperacional,
            TipoDecisaoAprovacaoChamado.RegistroManual => true,
            _ => false
        };
}

public sealed class AprovarAprovacaoChamadoRequestValidator : AbstractValidator<AprovarAprovacaoChamadoRequest>
{
    public AprovarAprovacaoChamadoRequestValidator()
    {
        RuleFor(x => x.InstanciaAprovacaoChamadoId)
            .NotEqual(Guid.Empty)
            .WithMessage("InstanciaAprovacaoChamadoId e obrigatorio.");

        RuleFor(x => x.EtapaAprovacaoChamadoId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("EtapaAprovacaoChamadoId informado e invalido.");

        RuleFor(x => x.DecisorUsuarioId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("DecisorUsuarioId informado e invalido.");

        RuleFor(x => x.PapelDecisorSnapshot)
            .MaximumLength(120)
            .When(x => !string.IsNullOrWhiteSpace(x.PapelDecisorSnapshot))
            .WithMessage("PapelDecisorSnapshot deve ter no maximo 120 caracteres.");

        RuleFor(x => x.AutoridadeDecisorSnapshot)
            .MaximumLength(180)
            .When(x => !string.IsNullOrWhiteSpace(x.AutoridadeDecisorSnapshot))
            .WithMessage("AutoridadeDecisorSnapshot deve ter no maximo 180 caracteres.");

        RuleFor(x => x.Justificativa)
            .MaximumLength(4000)
            .When(x => !string.IsNullOrWhiteSpace(x.Justificativa))
            .WithMessage("Justificativa deve ter no maximo 4000 caracteres.");

        RuleFor(x => x.Observacao)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.Observacao))
            .WithMessage("Observacao deve ter no maximo 2000 caracteres.");

        RuleFor(x => x.EscopoDecididoSnapshot)
            .MaximumLength(4000)
            .When(x => !string.IsNullOrWhiteSpace(x.EscopoDecididoSnapshot))
            .WithMessage("EscopoDecididoSnapshot deve ter no maximo 4000 caracteres.");

        RuleFor(x => x)
            .Must(x => !x.LiberaAvanco || !x.MantemBloqueio)
            .WithMessage("LiberaAvanco e MantemBloqueio nao podem ser verdadeiros ao mesmo tempo.");

        RuleFor(x => x)
            .Must(x => !x.DecisaoParcial || !x.DecisaoFinal)
            .WithMessage("DecisaoParcial e DecisaoFinal nao devem ser verdadeiros ao mesmo tempo.");
    }
}

public sealed class ReprovarAprovacaoChamadoRequestValidator : AbstractValidator<ReprovarAprovacaoChamadoRequest>
{
    public ReprovarAprovacaoChamadoRequestValidator()
    {
        RuleFor(x => x.InstanciaAprovacaoChamadoId)
            .NotEqual(Guid.Empty)
            .WithMessage("InstanciaAprovacaoChamadoId e obrigatorio.");

        RuleFor(x => x.EtapaAprovacaoChamadoId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("EtapaAprovacaoChamadoId informado e invalido.");

        RuleFor(x => x.DecisorUsuarioId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("DecisorUsuarioId informado e invalido.");

        RuleFor(x => x.PapelDecisorSnapshot)
            .MaximumLength(120)
            .When(x => !string.IsNullOrWhiteSpace(x.PapelDecisorSnapshot))
            .WithMessage("PapelDecisorSnapshot deve ter no maximo 120 caracteres.");

        RuleFor(x => x.AutoridadeDecisorSnapshot)
            .MaximumLength(180)
            .When(x => !string.IsNullOrWhiteSpace(x.AutoridadeDecisorSnapshot))
            .WithMessage("AutoridadeDecisorSnapshot deve ter no maximo 180 caracteres.");

        RuleFor(x => x.Justificativa)
            .NotEmpty().WithMessage("Justificativa e obrigatoria para reprovacao.")
            .MaximumLength(4000).WithMessage("Justificativa deve ter no maximo 4000 caracteres.");

        RuleFor(x => x.Observacao)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.Observacao))
            .WithMessage("Observacao deve ter no maximo 2000 caracteres.");

        RuleFor(x => x.EscopoDecididoSnapshot)
            .MaximumLength(4000)
            .When(x => !string.IsNullOrWhiteSpace(x.EscopoDecididoSnapshot))
            .WithMessage("EscopoDecididoSnapshot deve ter no maximo 4000 caracteres.");

        RuleFor(x => x)
            .Must(x => !x.DecisaoParcial || !x.DecisaoFinal)
            .WithMessage("DecisaoParcial e DecisaoFinal nao devem ser verdadeiros ao mesmo tempo.");
    }
}

public sealed class CancelarDecisaoAprovacaoChamadoRequestValidator : AbstractValidator<CancelarDecisaoAprovacaoChamadoRequest>
{
    public CancelarDecisaoAprovacaoChamadoRequestValidator()
    {
        RuleFor(x => x.InstanciaAprovacaoChamadoId)
            .NotEqual(Guid.Empty)
            .WithMessage("InstanciaAprovacaoChamadoId e obrigatorio.");

        RuleFor(x => x.EtapaAprovacaoChamadoId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("EtapaAprovacaoChamadoId informado e invalido.");

        RuleFor(x => x.DecisorUsuarioId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("DecisorUsuarioId informado e invalido.");

        RuleFor(x => x.Motivo)
            .NotEmpty().WithMessage("Motivo e obrigatorio para cancelamento.")
            .MaximumLength(4000).WithMessage("Motivo deve ter no maximo 4000 caracteres.");

        RuleFor(x => x.Observacao)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.Observacao))
            .WithMessage("Observacao deve ter no maximo 2000 caracteres.");
    }
}

public sealed class RegistrarExpiracaoAprovacaoChamadoRequestValidator : AbstractValidator<RegistrarExpiracaoAprovacaoChamadoRequest>
{
    public RegistrarExpiracaoAprovacaoChamadoRequestValidator()
    {
        RuleFor(x => x.InstanciaAprovacaoChamadoId)
            .NotEqual(Guid.Empty)
            .WithMessage("InstanciaAprovacaoChamadoId e obrigatorio.");

        RuleFor(x => x.EtapaAprovacaoChamadoId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("EtapaAprovacaoChamadoId informado e invalido.");

        RuleFor(x => x.ResponsavelUsuarioId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("ResponsavelUsuarioId informado e invalido.");

        RuleFor(x => x.ComponenteResponsavel)
            .MaximumLength(180)
            .When(x => !string.IsNullOrWhiteSpace(x.ComponenteResponsavel))
            .WithMessage("ComponenteResponsavel deve ter no maximo 180 caracteres.");

        RuleFor(x => x.Motivo)
            .NotEmpty().WithMessage("Motivo e obrigatorio para expiracao.")
            .MaximumLength(4000).WithMessage("Motivo deve ter no maximo 4000 caracteres.");

        RuleFor(x => x.DataExpiracao)
            .NotEqual(default(DateTime))
            .WithMessage("DataExpiracao e obrigatoria.");
    }
}

public sealed class SolicitarReavaliacaoAprovacaoChamadoRequestValidator : AbstractValidator<SolicitarReavaliacaoAprovacaoChamadoRequest>
{
    public SolicitarReavaliacaoAprovacaoChamadoRequestValidator()
    {
        RuleFor(x => x.InstanciaAprovacaoChamadoId)
            .NotEqual(Guid.Empty)
            .WithMessage("InstanciaAprovacaoChamadoId e obrigatorio.");

        RuleFor(x => x.EtapaAprovacaoChamadoId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("EtapaAprovacaoChamadoId informado e invalido.");

        RuleFor(x => x.DecisorUsuarioId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("DecisorUsuarioId informado e invalido.");

        RuleFor(x => x.Justificativa)
            .MaximumLength(4000)
            .When(x => !string.IsNullOrWhiteSpace(x.Justificativa))
            .WithMessage("Justificativa deve ter no maximo 4000 caracteres.");

        RuleFor(x => x.Observacao)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.Observacao))
            .WithMessage("Observacao deve ter no maximo 2000 caracteres.");

        RuleFor(x => x.EscopoDecididoSnapshot)
            .MaximumLength(4000)
            .When(x => !string.IsNullOrWhiteSpace(x.EscopoDecididoSnapshot))
            .WithMessage("EscopoDecididoSnapshot deve ter no maximo 4000 caracteres.");
    }
}

public sealed class ValidarDecisaoAprovacaoChamadoRequestValidator : AbstractValidator<ValidarDecisaoAprovacaoChamadoRequest>
{
    public ValidarDecisaoAprovacaoChamadoRequestValidator()
    {
        RuleFor(x => x.DecisaoAprovacaoChamadoId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("DecisaoAprovacaoChamadoId informado e invalido.");

        RuleFor(x => x.Decisao)
            .NotNull()
            .WithMessage("Decisao e obrigatoria.");

        RuleFor(x => x.Decisao)
            .SetValidator(new RegistrarDecisaoAprovacaoChamadoRequestValidator());
    }
}
