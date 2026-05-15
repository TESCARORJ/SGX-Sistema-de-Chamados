using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Auditoria;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class FiltroEventosAuditoriaRequestValidator : AbstractValidator<FiltroEventosAuditoriaRequest>
{
    public FiltroEventosAuditoriaRequestValidator()
    {
        RuleFor(x => x.Pagina)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Pagina deve ser maior ou igual a 1.");

        RuleFor(x => x.TamanhoPagina)
            .InclusiveBetween(1, 100)
            .WithMessage("TamanhoPagina deve estar entre 1 e 100.");

        RuleFor(x => x.Texto)
            .MaximumLength(200)
            .WithMessage("Texto deve ter no maximo 200 caracteres.");

        RuleFor(x => x.UsuarioEmail)
            .MaximumLength(320)
            .WithMessage("UsuarioEmail deve ter no maximo 320 caracteres.");

        RuleFor(x => x.CorrelacaoId)
            .MaximumLength(120)
            .WithMessage("CorrelacaoId deve ter no maximo 120 caracteres.");

        RuleFor(x => x)
            .Must(x => !x.DataInicio.HasValue || !x.DataFim.HasValue || x.DataFim.Value.Date >= x.DataInicio.Value.Date)
            .WithMessage("DataFim deve ser maior ou igual a DataInicio.");
    }
}

public sealed class FiltroDashboardAuditoriaRequestValidator : AbstractValidator<FiltroDashboardAuditoriaRequest>
{
    public FiltroDashboardAuditoriaRequestValidator()
    {
        RuleFor(x => x.UsuarioEmail)
            .MaximumLength(320)
            .WithMessage("UsuarioEmail deve ter no maximo 320 caracteres.");

        RuleFor(x => x.Modulo)
            .MaximumLength(120)
            .WithMessage("Modulo deve ter no maximo 120 caracteres.");

        RuleFor(x => x)
            .Must(x => !x.DataInicio.HasValue || !x.DataFim.HasValue || x.DataFim.Value.Date >= x.DataInicio.Value.Date)
            .WithMessage("DataFim deve ser maior ou igual a DataInicio.");
    }
}
