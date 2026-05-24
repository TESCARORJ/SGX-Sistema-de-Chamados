using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Admin;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class FiltroInventarioAtivoRequestValidator : AbstractValidator<FiltroInventarioAtivoRequest>
{
    private static readonly string[] CamposOrdenacao =
    [
        "codigo",
        "nome",
        "dataaquisicao",
        "datafimgarantia",
        "criadoem",
        "atualizadoem"
    ];

    public FiltroInventarioAtivoRequestValidator()
    {
        RuleFor(x => x.Termo)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Termo))
            .WithMessage("Termo deve ter no maximo 500 caracteres.");

        RuleFor(x => x.TipoAtivoInventarioId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("TipoAtivoInventarioId informado e invalido.");

        RuleFor(x => x.DepartamentoId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("DepartamentoId informado e invalido.");

        RuleFor(x => x.LocalUnidadeId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("LocalUnidadeId informado e invalido.");

        RuleFor(x => x.UsuarioResponsavelId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("UsuarioResponsavelId informado e invalido.");

        RuleFor(x => x.StatusOperacional)
            .IsInEnum()
            .When(x => x.StatusOperacional.HasValue)
            .WithMessage("StatusOperacional informado e invalido.");

        RuleFor(x => x.StatusPatrimonial)
            .IsInEnum()
            .When(x => x.StatusPatrimonial.HasValue)
            .WithMessage("StatusPatrimonial informado e invalido.");

        RuleFor(x => x.Criticidade)
            .IsInEnum()
            .When(x => x.Criticidade.HasValue)
            .WithMessage("Criticidade informada e invalida.");

        RuleFor(x => x.DataAquisicaoFinal)
            .GreaterThanOrEqualTo(x => x.DataAquisicaoInicial!.Value.Date)
            .When(x => x.DataAquisicaoInicial.HasValue && x.DataAquisicaoFinal.HasValue)
            .WithMessage("DataAquisicaoFinal nao pode ser anterior a DataAquisicaoInicial.");

        RuleFor(x => x.DataFimGarantiaFinal)
            .GreaterThanOrEqualTo(x => x.DataFimGarantiaInicial!.Value.Date)
            .When(x => x.DataFimGarantiaInicial.HasValue && x.DataFimGarantiaFinal.HasValue)
            .WithMessage("DataFimGarantiaFinal nao pode ser anterior a DataFimGarantiaInicial.");

        RuleFor(x => x.OrdenarPor)
            .Must(valor => string.IsNullOrWhiteSpace(valor) || CamposOrdenacao.Contains(valor.Trim().ToLowerInvariant()))
            .WithMessage("OrdenarPor deve ser codigo, nome, dataAquisicao, dataFimGarantia, criadoEm ou atualizadoEm.");

        RuleFor(x => x.DirecaoOrdenacao)
            .Must(valor => string.IsNullOrWhiteSpace(valor) ||
                           string.Equals(valor.Trim(), "asc", StringComparison.OrdinalIgnoreCase) ||
                           string.Equals(valor.Trim(), "desc", StringComparison.OrdinalIgnoreCase))
            .WithMessage("DirecaoOrdenacao deve ser asc ou desc.");
    }
}

public sealed class CriarInventarioAtivoRequestValidator : AbstractValidator<CriarInventarioAtivoRequest>
{
    public CriarInventarioAtivoRequestValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("Codigo e obrigatorio.")
            .MaximumLength(60).WithMessage("Codigo deve ter no maximo 60 caracteres.");

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome e obrigatorio.")
            .MaximumLength(180).WithMessage("Nome deve ter no maximo 180 caracteres.");

        RuleFor(x => x.Descricao)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.Descricao))
            .WithMessage("Descricao deve ter no maximo 2000 caracteres.");

        RuleFor(x => x.NumeroPatrimonio)
            .MaximumLength(120)
            .When(x => !string.IsNullOrWhiteSpace(x.NumeroPatrimonio))
            .WithMessage("NumeroPatrimonio deve ter no maximo 120 caracteres.");

        RuleFor(x => x.NumeroSerie)
            .MaximumLength(180)
            .When(x => !string.IsNullOrWhiteSpace(x.NumeroSerie))
            .WithMessage("NumeroSerie deve ter no maximo 180 caracteres.");

        RuleFor(x => x.TipoAtivoInventarioId)
            .NotEqual(Guid.Empty)
            .WithMessage("TipoAtivoInventarioId e obrigatorio.");

        RuleFor(x => x.Fabricante)
            .MaximumLength(180)
            .When(x => !string.IsNullOrWhiteSpace(x.Fabricante))
            .WithMessage("Fabricante deve ter no maximo 180 caracteres.");

        RuleFor(x => x.Modelo)
            .MaximumLength(180)
            .When(x => !string.IsNullOrWhiteSpace(x.Modelo))
            .WithMessage("Modelo deve ter no maximo 180 caracteres.");

        RuleFor(x => x.Fornecedor)
            .MaximumLength(180)
            .When(x => !string.IsNullOrWhiteSpace(x.Fornecedor))
            .WithMessage("Fornecedor deve ter no maximo 180 caracteres.");

        RuleFor(x => x.Observacoes)
            .MaximumLength(4000)
            .When(x => !string.IsNullOrWhiteSpace(x.Observacoes))
            .WithMessage("Observacoes deve ter no maximo 4000 caracteres.");

        RuleFor(x => x.DepartamentoId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("DepartamentoId informado e invalido.");

        RuleFor(x => x.LocalUnidadeId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("LocalUnidadeId informado e invalido.");

        RuleFor(x => x.UsuarioResponsavelId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("UsuarioResponsavelId informado e invalido.");

        RuleFor(x => x.StatusOperacional)
            .IsInEnum()
            .When(x => x.StatusOperacional.HasValue)
            .WithMessage("StatusOperacional informado e invalido.");

        RuleFor(x => x.StatusPatrimonial)
            .IsInEnum()
            .When(x => x.StatusPatrimonial.HasValue)
            .WithMessage("StatusPatrimonial informado e invalido.");

        RuleFor(x => x.Criticidade)
            .IsInEnum()
            .When(x => x.Criticidade.HasValue)
            .WithMessage("Criticidade informada e invalida.");

        RuleFor(x => x.DataFimGarantia)
            .GreaterThanOrEqualTo(x => x.DataAquisicao!.Value.Date)
            .When(x => x.DataAquisicao.HasValue && x.DataFimGarantia.HasValue)
            .WithMessage("DataFimGarantia nao pode ser anterior a DataAquisicao.");

        RuleFor(x => x.ValorAquisicao)
            .GreaterThanOrEqualTo(0m)
            .When(x => x.ValorAquisicao.HasValue)
            .WithMessage("ValorAquisicao nao pode ser negativo.");
    }
}

public sealed class AtualizarInventarioAtivoRequestValidator : AbstractValidator<AtualizarInventarioAtivoRequest>
{
    public AtualizarInventarioAtivoRequestValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("Codigo e obrigatorio.")
            .MaximumLength(60).WithMessage("Codigo deve ter no maximo 60 caracteres.");

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome e obrigatorio.")
            .MaximumLength(180).WithMessage("Nome deve ter no maximo 180 caracteres.");

        RuleFor(x => x.Descricao)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.Descricao))
            .WithMessage("Descricao deve ter no maximo 2000 caracteres.");

        RuleFor(x => x.NumeroPatrimonio)
            .MaximumLength(120)
            .When(x => !string.IsNullOrWhiteSpace(x.NumeroPatrimonio))
            .WithMessage("NumeroPatrimonio deve ter no maximo 120 caracteres.");

        RuleFor(x => x.NumeroSerie)
            .MaximumLength(180)
            .When(x => !string.IsNullOrWhiteSpace(x.NumeroSerie))
            .WithMessage("NumeroSerie deve ter no maximo 180 caracteres.");

        RuleFor(x => x.TipoAtivoInventarioId)
            .NotEqual(Guid.Empty)
            .WithMessage("TipoAtivoInventarioId e obrigatorio.");

        RuleFor(x => x.Fabricante)
            .MaximumLength(180)
            .When(x => !string.IsNullOrWhiteSpace(x.Fabricante))
            .WithMessage("Fabricante deve ter no maximo 180 caracteres.");

        RuleFor(x => x.Modelo)
            .MaximumLength(180)
            .When(x => !string.IsNullOrWhiteSpace(x.Modelo))
            .WithMessage("Modelo deve ter no maximo 180 caracteres.");

        RuleFor(x => x.Fornecedor)
            .MaximumLength(180)
            .When(x => !string.IsNullOrWhiteSpace(x.Fornecedor))
            .WithMessage("Fornecedor deve ter no maximo 180 caracteres.");

        RuleFor(x => x.Observacoes)
            .MaximumLength(4000)
            .When(x => !string.IsNullOrWhiteSpace(x.Observacoes))
            .WithMessage("Observacoes deve ter no maximo 4000 caracteres.");

        RuleFor(x => x.DepartamentoId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("DepartamentoId informado e invalido.");

        RuleFor(x => x.LocalUnidadeId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("LocalUnidadeId informado e invalido.");

        RuleFor(x => x.UsuarioResponsavelId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("UsuarioResponsavelId informado e invalido.");

        RuleFor(x => x.StatusOperacional)
            .IsInEnum()
            .WithMessage("StatusOperacional informado e invalido.");

        RuleFor(x => x.StatusPatrimonial)
            .IsInEnum()
            .WithMessage("StatusPatrimonial informado e invalido.");

        RuleFor(x => x.Criticidade)
            .IsInEnum()
            .WithMessage("Criticidade informada e invalida.");

        RuleFor(x => x.DataFimGarantia)
            .GreaterThanOrEqualTo(x => x.DataAquisicao!.Value.Date)
            .When(x => x.DataAquisicao.HasValue && x.DataFimGarantia.HasValue)
            .WithMessage("DataFimGarantia nao pode ser anterior a DataAquisicao.");

        RuleFor(x => x.ValorAquisicao)
            .GreaterThanOrEqualTo(0m)
            .When(x => x.ValorAquisicao.HasValue)
            .WithMessage("ValorAquisicao nao pode ser negativo.");
    }
}

public sealed class FiltroHistoricoInventarioAtivoRequestValidator : AbstractValidator<FiltroHistoricoInventarioAtivoRequest>
{
    public FiltroHistoricoInventarioAtivoRequestValidator()
    {
        RuleFor(x => x.Pagina)
            .GreaterThan(0)
            .WithMessage("Pagina deve ser maior que zero.");

        RuleFor(x => x.TamanhoPagina)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("TamanhoPagina deve estar entre 1 e 100.");
    }
}

public sealed class FiltroChamadosRelacionadosInventarioAtivoRequestValidator : AbstractValidator<FiltroChamadosRelacionadosInventarioAtivoRequest>
{
    public FiltroChamadosRelacionadosInventarioAtivoRequestValidator()
    {
        RuleFor(x => x.Pagina)
            .GreaterThan(0)
            .WithMessage("Pagina deve ser maior que zero.");

        RuleFor(x => x.TamanhoPagina)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("TamanhoPagina deve estar entre 1 e 100.");
    }
}

public sealed class MovimentarInventarioAtivoRequestValidator : AbstractValidator<MovimentarInventarioAtivoRequest>
{
    public MovimentarInventarioAtivoRequestValidator()
    {
        RuleFor(x => x.DepartamentoId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("DepartamentoId informado e invalido.");

        RuleFor(x => x.LocalUnidadeId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("LocalUnidadeId informado e invalido.");

        RuleFor(x => x.UsuarioResponsavelId)
            .Must(x => !x.HasValue || x.Value != Guid.Empty)
            .WithMessage("UsuarioResponsavelId informado e invalido.");

        RuleFor(x => x.StatusOperacional)
            .IsInEnum()
            .When(x => x.StatusOperacional.HasValue)
            .WithMessage("StatusOperacional informado e invalido.");

        RuleFor(x => x.StatusPatrimonial)
            .IsInEnum()
            .When(x => x.StatusPatrimonial.HasValue)
            .WithMessage("StatusPatrimonial informado e invalido.");

        RuleFor(x => x.Observacao)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.Observacao))
            .WithMessage("Observacao deve ter no maximo 2000 caracteres.");

        RuleFor(x => x)
            .Must(x => x.DepartamentoId.HasValue ||
                       x.LocalUnidadeId.HasValue ||
                       x.UsuarioResponsavelId.HasValue ||
                       x.StatusOperacional.HasValue ||
                       x.StatusPatrimonial.HasValue)
            .WithMessage("Informe pelo menos uma alteracao para movimentar o ativo.");
    }
}
