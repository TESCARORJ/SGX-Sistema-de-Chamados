using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class FiltroRelatorioChamadosRequestValidator : AbstractValidator<FiltroRelatorioChamadosRequest>
{
    public FiltroRelatorioChamadosRequestValidator()
    {
        RuleFor(x => x)
            .Custom((request, context) =>
            {
                ValidarPeriodo(request, context);
            });

        RuleFor(x => x.Agrupamento)
            .Must(x => x is AgrupamentoRelatorio.Dia or AgrupamentoRelatorio.Semana or AgrupamentoRelatorio.Mes)
            .WithMessage("Agrupamento temporal invalido. Use Dia, Semana ou Mes.");

        RuleFor(x => x.LimiteRanking)
            .InclusiveBetween(1, 100)
            .WithMessage("LimiteRanking deve estar entre 1 e 100.");

        RuleFor(x => x.Origem)
            .Must(OrigemValida)
            .When(x => !string.IsNullOrWhiteSpace(x.Origem))
            .WithMessage("Origem informada invalida.");
    }

    private static void ValidarPeriodo(FiltroPeriodoRelatorioRequest request, ValidationContext<FiltroRelatorioChamadosRequest> context)
    {
        var dataInicial = (request.ObterDataInicial() ?? DateTime.UtcNow.Date.AddDays(-30)).Date;
        var dataFinal = (request.ObterDataFinal() ?? DateTime.UtcNow.Date).Date;

        if (dataInicial > dataFinal)
        {
            context.AddFailure("DataInicial", "DataInicial nao pode ser maior que DataFinal.");
            return;
        }

        if ((dataFinal - dataInicial).TotalDays > 366)
        {
            context.AddFailure("DataFinal", "Periodo informado excede o limite maximo de 366 dias.");
        }
    }

    private static bool OrigemValida(string? origem)
        => Enum.TryParse<OrigemChamado>(origem?.Trim(), true, out _);
}

public sealed class FiltroRelatorioAtendimentoRequestValidator : AbstractValidator<FiltroRelatorioAtendimentoRequest>
{
    public FiltroRelatorioAtendimentoRequestValidator()
    {
        RuleFor(x => x)
            .Custom((request, context) =>
            {
                var dataInicial = (request.ObterDataInicial() ?? DateTime.UtcNow.Date.AddDays(-30)).Date;
                var dataFinal = (request.ObterDataFinal() ?? DateTime.UtcNow.Date).Date;

                if (dataInicial > dataFinal)
                {
                    context.AddFailure("DataInicial", "DataInicial nao pode ser maior que DataFinal.");
                    return;
                }

                if ((dataFinal - dataInicial).TotalDays > 366)
                {
                    context.AddFailure("DataFinal", "Periodo informado excede o limite maximo de 366 dias.");
                }
            });

        RuleFor(x => x.LimiteRanking)
            .InclusiveBetween(1, 100)
            .WithMessage("LimiteRanking deve estar entre 1 e 100.");

        RuleFor(x => x.Origem)
            .Must(origem => Enum.TryParse<OrigemChamado>(origem?.Trim(), true, out _))
            .When(x => !string.IsNullOrWhiteSpace(x.Origem))
            .WithMessage("Origem informada invalida.");
    }
}

public sealed class FiltroRelatorioSlaRequestValidator : AbstractValidator<FiltroRelatorioSlaRequest>
{
    public FiltroRelatorioSlaRequestValidator()
    {
        RuleFor(x => x)
            .Custom((request, context) =>
            {
                var dataInicial = (request.ObterDataInicial() ?? DateTime.UtcNow.Date.AddDays(-30)).Date;
                var dataFinal = (request.ObterDataFinal() ?? DateTime.UtcNow.Date).Date;

                if (dataInicial > dataFinal)
                {
                    context.AddFailure("DataInicial", "DataInicial nao pode ser maior que DataFinal.");
                    return;
                }

                if ((dataFinal - dataInicial).TotalDays > 366)
                {
                    context.AddFailure("DataFinal", "Periodo informado excede o limite maximo de 366 dias.");
                }
            });

        RuleFor(x => x.LimiteRanking)
            .InclusiveBetween(1, 100)
            .WithMessage("LimiteRanking deve estar entre 1 e 100.");

        RuleFor(x => x.SituacaoSla)
            .Must(valor => Enum.TryParse<SituacaoSlaChamadoEnum>(valor?.Trim(), true, out _))
            .When(x => !string.IsNullOrWhiteSpace(x.SituacaoSla))
            .WithMessage("SituacaoSla informada invalida.");
    }
}

public sealed class FiltroRelatorioAprovacoesRequestValidator : AbstractValidator<FiltroRelatorioAprovacoesRequest>
{
    public FiltroRelatorioAprovacoesRequestValidator()
    {
        RuleFor(x => x)
            .Custom((request, context) =>
            {
                var dataInicial = (request.ObterDataInicial() ?? DateTime.UtcNow.Date.AddDays(-30)).Date;
                var dataFinal = (request.ObterDataFinal() ?? DateTime.UtcNow.Date).Date;

                if (dataInicial > dataFinal)
                {
                    context.AddFailure("DataInicial", "DataInicial nao pode ser maior que DataFinal.");
                    return;
                }

                if ((dataFinal - dataInicial).TotalDays > 366)
                {
                    context.AddFailure("DataFinal", "Periodo informado excede o limite maximo de 366 dias.");
                }

                if (request.Agrupamento.HasValue && request.Agrupamento.Value is not (AgrupamentoRelatorio.Dia or AgrupamentoRelatorio.Mes))
                {
                    context.AddFailure("Agrupamento", "Agrupamento temporal invalido. Use Dia ou Mes.");
                }

                if (request.AgruparPor == AgruparTempoMedioAprovacoesPor.Periodo && !request.Agrupamento.HasValue)
                {
                    context.AddFailure("Agrupamento", "Agrupamento e obrigatorio quando AgruparPor for Periodo.");
                }
            });

        RuleFor(x => x.TipoOrigemAprovacao)
            .Must(valor => Enum.TryParse<TipoOrigemAprovacaoChamado>(valor?.Trim(), true, out _))
            .When(x => !string.IsNullOrWhiteSpace(x.TipoOrigemAprovacao))
            .WithMessage("TipoOrigemAprovacao informado invalido.");

        RuleFor(x => x.StatusAprovacao)
            .Must(valor => Enum.TryParse<StatusAprovacaoChamado>(valor?.Trim(), true, out _))
            .When(x => !string.IsNullOrWhiteSpace(x.StatusAprovacao))
            .WithMessage("StatusAprovacao informado invalido.");
    }
}

public sealed class FiltroRelatorioCatalogoServicosRequestValidator : AbstractValidator<FiltroRelatorioCatalogoServicosRequest>
{
    public FiltroRelatorioCatalogoServicosRequestValidator()
    {
        RuleFor(x => x)
            .Custom((request, context) =>
            {
                var dataInicial = (request.ObterDataInicial() ?? DateTime.UtcNow.Date.AddDays(-30)).Date;
                var dataFinal = (request.ObterDataFinal() ?? DateTime.UtcNow.Date).Date;

                if (dataInicial > dataFinal)
                {
                    context.AddFailure("DataInicial", "DataInicial nao pode ser maior que DataFinal.");
                    return;
                }

                if ((dataFinal - dataInicial).TotalDays > 366)
                {
                    context.AddFailure("DataFinal", "Periodo informado excede o limite maximo de 366 dias.");
                }
            });

        RuleFor(x => x.LimiteRanking)
            .InclusiveBetween(1, 100)
            .WithMessage("LimiteRanking deve estar entre 1 e 100.");

        RuleFor(x => x.TipoOrigemAprovacao)
            .Must(valor => Enum.TryParse<TipoOrigemAprovacaoChamado>(valor?.Trim(), true, out _))
            .When(x => !string.IsNullOrWhiteSpace(x.TipoOrigemAprovacao))
            .WithMessage("TipoOrigemAprovacao informado invalido.");

        RuleFor(x => x.StatusAprovacao)
            .Must(valor => Enum.TryParse<StatusAprovacaoChamado>(valor?.Trim(), true, out _))
            .When(x => !string.IsNullOrWhiteSpace(x.StatusAprovacao))
            .WithMessage("StatusAprovacao informado invalido.");
    }
}

public sealed class FiltroRelatorioInventarioAtivosRequestValidator : AbstractValidator<FiltroRelatorioInventarioAtivosRequest>
{
    public FiltroRelatorioInventarioAtivosRequestValidator()
    {
        RuleFor(x => x)
            .Custom((request, context) =>
            {
                var dataInicial = (request.ObterDataInicial() ?? DateTime.UtcNow.Date.AddDays(-30)).Date;
                var dataFinal = (request.ObterDataFinal() ?? DateTime.UtcNow.Date).Date;

                if (dataInicial > dataFinal)
                {
                    context.AddFailure("DataInicial", "DataInicial nao pode ser maior que DataFinal.");
                    return;
                }

                if ((dataFinal - dataInicial).TotalDays > 366)
                {
                    context.AddFailure("DataFinal", "Periodo informado excede o limite maximo de 366 dias.");
                }
            });

        RuleFor(x => x.LimiteRanking)
            .InclusiveBetween(1, 100)
            .WithMessage("LimiteRanking deve estar entre 1 e 100.");

        RuleFor(x => x.StatusOperacional)
            .Must(valor => Enum.TryParse<StatusOperacionalAtivo>(valor?.Trim(), true, out _))
            .When(x => !string.IsNullOrWhiteSpace(x.StatusOperacional))
            .WithMessage("StatusOperacional informado invalido.");

        RuleFor(x => x.StatusPatrimonial)
            .Must(valor => Enum.TryParse<StatusPatrimonialAtivo>(valor?.Trim(), true, out _))
            .When(x => !string.IsNullOrWhiteSpace(x.StatusPatrimonial))
            .WithMessage("StatusPatrimonial informado invalido.");

        RuleFor(x => x.Criticidade)
            .Must(valor => Enum.TryParse<CriticidadeAtivo>(valor?.Trim(), true, out _))
            .When(x => !string.IsNullOrWhiteSpace(x.Criticidade))
            .WithMessage("Criticidade informada invalida.");
    }
}

public sealed class FiltroRelatorioBaseConhecimentoRequestValidator : AbstractValidator<FiltroRelatorioBaseConhecimentoRequest>
{
    public FiltroRelatorioBaseConhecimentoRequestValidator()
    {
        RuleFor(x => x)
            .Custom((request, context) =>
            {
                var dataInicial = (request.ObterDataInicial() ?? DateTime.UtcNow.Date.AddDays(-30)).Date;
                var dataFinal = (request.ObterDataFinal() ?? DateTime.UtcNow.Date).Date;

                if (dataInicial > dataFinal)
                {
                    context.AddFailure("DataInicial", "DataInicial nao pode ser maior que DataFinal.");
                    return;
                }

                if ((dataFinal - dataInicial).TotalDays > 366)
                {
                    context.AddFailure("DataFinal", "Periodo informado excede o limite maximo de 366 dias.");
                }
            });

        RuleFor(x => x.LimiteRanking)
            .InclusiveBetween(1, 100)
            .WithMessage("LimiteRanking deve estar entre 1 e 100.");

        RuleFor(x => x.StatusArtigo)
            .Must(valor => Enum.TryParse<StatusArtigoConhecimento>(valor?.Trim(), true, out _))
            .When(x => !string.IsNullOrWhiteSpace(x.StatusArtigo))
            .WithMessage("StatusArtigo informado invalido.");

        RuleFor(x => x.VisibilidadeArtigo)
            .Must(valor => Enum.TryParse<VisibilidadeArtigoConhecimento>(valor?.Trim(), true, out _))
            .When(x => !string.IsNullOrWhiteSpace(x.VisibilidadeArtigo))
            .WithMessage("VisibilidadeArtigo informada invalida.");
    }
}

public sealed class FiltroRelatorioAuditoriaRequestValidator : AbstractValidator<FiltroRelatorioAuditoriaRequest>
{
    public FiltroRelatorioAuditoriaRequestValidator()
    {
        RuleFor(x => x)
            .Custom((request, context) =>
            {
                var dataInicial = (request.ObterDataInicial() ?? DateTime.UtcNow.Date.AddDays(-30)).Date;
                var dataFinal = (request.ObterDataFinal() ?? DateTime.UtcNow.Date).Date;

                if (dataInicial > dataFinal)
                {
                    context.AddFailure("DataInicial", "DataInicial nao pode ser maior que DataFinal.");
                    return;
                }

                if ((dataFinal - dataInicial).TotalDays > 366)
                {
                    context.AddFailure("DataFinal", "Periodo informado excede o limite maximo de 366 dias.");
                }
            });

        RuleFor(x => x.LimiteRanking)
            .InclusiveBetween(1, 100)
            .WithMessage("LimiteRanking deve estar entre 1 e 100.");

        RuleFor(x => x.TipoAcao)
            .Must(valor => Enum.TryParse<TipoAcaoAuditoria>(valor?.Trim(), true, out _))
            .When(x => !string.IsNullOrWhiteSpace(x.TipoAcao))
            .WithMessage("TipoAcao informado invalido.");

        RuleFor(x => x.Termo)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Termo))
            .WithMessage("Termo deve possuir no maximo 200 caracteres.");
    }
}
