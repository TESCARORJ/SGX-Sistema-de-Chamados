using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class CriarFormularioServicoRequestValidator : AbstractValidator<CriarFormularioServicoRequest>
{
    public CriarFormularioServicoRequestValidator()
    {
        RuleFor(x => x.CatalogoServicoId)
            .NotEqual(Guid.Empty)
            .WithMessage("CatalogoServicoId e obrigatorio.");

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome e obrigatorio.")
            .MaximumLength(180).WithMessage("Nome deve ter no maximo 180 caracteres.");

        RuleFor(x => x.Descricao)
            .MaximumLength(4000)
            .When(x => !string.IsNullOrWhiteSpace(x.Descricao))
            .WithMessage("Descricao deve ter no maximo 4000 caracteres.");
    }
}

public sealed class AtualizarFormularioServicoRequestValidator : AbstractValidator<AtualizarFormularioServicoRequest>
{
    public AtualizarFormularioServicoRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome e obrigatorio.")
            .MaximumLength(180).WithMessage("Nome deve ter no maximo 180 caracteres.");

        RuleFor(x => x.Descricao)
            .MaximumLength(4000)
            .When(x => !string.IsNullOrWhiteSpace(x.Descricao))
            .WithMessage("Descricao deve ter no maximo 4000 caracteres.");
    }
}

public sealed class CriarFormularioServicoVersaoRequestValidator : AbstractValidator<CriarFormularioServicoVersaoRequest>
{
    public CriarFormularioServicoVersaoRequestValidator()
    {
        RuleFor(x => x.FormularioServicoId)
            .NotEqual(Guid.Empty)
            .WithMessage("FormularioServicoId e obrigatorio.");

        RuleFor(x => x.Numero)
            .GreaterThan(0)
            .WithMessage("Numero deve ser maior que zero.");

        RuleFor(x => x)
            .Must(x => !x.PublicadoEm.HasValue || x.Publicada)
            .WithMessage("PublicadoEm so pode ser informado quando Publicada for true.");
    }
}

public sealed class AtualizarFormularioServicoVersaoRequestValidator : AbstractValidator<AtualizarFormularioServicoVersaoRequest>
{
    public AtualizarFormularioServicoVersaoRequestValidator()
    {
        RuleFor(x => x.Numero)
            .GreaterThan(0)
            .WithMessage("Numero deve ser maior que zero.");

        RuleFor(x => x)
            .Must(x => !x.PublicadoEm.HasValue || x.Publicada)
            .WithMessage("PublicadoEm so pode ser informado quando Publicada for true.");
    }
}

public sealed class CriarCampoFormularioServicoRequestValidator : AbstractValidator<CriarCampoFormularioServicoRequest>
{
    public CriarCampoFormularioServicoRequestValidator()
    {
        RuleFor(x => x.FormularioServicoVersaoId)
            .NotEqual(Guid.Empty)
            .WithMessage("FormularioServicoVersaoId e obrigatorio.");

        ConfigurarRegrasComuns();
    }

    private void ConfigurarRegrasComuns()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome e obrigatorio.")
            .MaximumLength(120).WithMessage("Nome deve ter no maximo 120 caracteres.")
            .Matches("^[A-Za-z][A-Za-z0-9_]*$").WithMessage("Nome deve comecar com letra e conter apenas letras, numeros ou underscore.");

        RuleFor(x => x.Rotulo)
            .NotEmpty().WithMessage("Rotulo e obrigatorio.")
            .MaximumLength(180).WithMessage("Rotulo deve ter no maximo 180 caracteres.");

        RuleFor(x => x.Tipo)
            .IsInEnum()
            .Must(x => Enum.IsDefined(typeof(TipoCampoFormularioServico), x))
            .WithMessage("Tipo informado e invalido.");

        RuleFor(x => x.Ordem)
            .GreaterThan(0)
            .WithMessage("Ordem deve ser maior que zero.");

        RuleFor(x => x.TextoAjuda)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.TextoAjuda))
            .WithMessage("TextoAjuda deve ter no maximo 500 caracteres.");
    }
}

public sealed class AtualizarCampoFormularioServicoRequestValidator : AbstractValidator<AtualizarCampoFormularioServicoRequest>
{
    public AtualizarCampoFormularioServicoRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome e obrigatorio.")
            .MaximumLength(120).WithMessage("Nome deve ter no maximo 120 caracteres.")
            .Matches("^[A-Za-z][A-Za-z0-9_]*$").WithMessage("Nome deve comecar com letra e conter apenas letras, numeros ou underscore.");

        RuleFor(x => x.Rotulo)
            .NotEmpty().WithMessage("Rotulo e obrigatorio.")
            .MaximumLength(180).WithMessage("Rotulo deve ter no maximo 180 caracteres.");

        RuleFor(x => x.Tipo)
            .IsInEnum()
            .Must(x => Enum.IsDefined(typeof(TipoCampoFormularioServico), x))
            .WithMessage("Tipo informado e invalido.");

        RuleFor(x => x.Ordem)
            .GreaterThan(0)
            .WithMessage("Ordem deve ser maior que zero.");

        RuleFor(x => x.TextoAjuda)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.TextoAjuda))
            .WithMessage("TextoAjuda deve ter no maximo 500 caracteres.");
    }
}

public sealed class CriarOpcaoCampoFormularioServicoRequestValidator : AbstractValidator<CriarOpcaoCampoFormularioServicoRequest>
{
    public CriarOpcaoCampoFormularioServicoRequestValidator()
    {
        RuleFor(x => x.CampoFormularioServicoId)
            .NotEqual(Guid.Empty)
            .WithMessage("CampoFormularioServicoId e obrigatorio.");

        ConfigurarRegrasComuns();
    }

    private void ConfigurarRegrasComuns()
    {
        RuleFor(x => x.Valor)
            .NotEmpty().WithMessage("Valor e obrigatorio.")
            .MaximumLength(120).WithMessage("Valor deve ter no maximo 120 caracteres.");

        RuleFor(x => x.Rotulo)
            .NotEmpty().WithMessage("Rotulo e obrigatorio.")
            .MaximumLength(180).WithMessage("Rotulo deve ter no maximo 180 caracteres.");

        RuleFor(x => x.Ordem)
            .GreaterThan(0)
            .WithMessage("Ordem deve ser maior que zero.");
    }
}

public sealed class AtualizarOpcaoCampoFormularioServicoRequestValidator : AbstractValidator<AtualizarOpcaoCampoFormularioServicoRequest>
{
    public AtualizarOpcaoCampoFormularioServicoRequestValidator()
    {
        RuleFor(x => x.Valor)
            .NotEmpty().WithMessage("Valor e obrigatorio.")
            .MaximumLength(120).WithMessage("Valor deve ter no maximo 120 caracteres.");

        RuleFor(x => x.Rotulo)
            .NotEmpty().WithMessage("Rotulo e obrigatorio.")
            .MaximumLength(180).WithMessage("Rotulo deve ter no maximo 180 caracteres.");

        RuleFor(x => x.Ordem)
            .GreaterThan(0)
            .WithMessage("Ordem deve ser maior que zero.");
    }
}
