using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Chamados;
using SGX.SistemaChamado.Application.DTOs;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class CriarChamadoDtoValidator : AbstractValidator<CriarChamadoDto>
{
    public CriarChamadoDtoValidator(ICamposObrigatoriosChamadoService camposObrigatoriosChamadoService)
    {
        RuleFor(x => x.Titulo)
            .MaximumLength(180);

        RuleFor(x => x.Descricao)
            .MaximumLength(4000);

        RuleFor(x => x.SolicitanteId)
            .NotEqual(Guid.Empty);

        RuleFor(x => x.CategoriaId)
            .NotEqual(Guid.Empty);

        RuleFor(x => x.PrioridadeId)
            .NotEqual(Guid.Empty);

        RuleFor(x => x.NaturezaChamado)
            .Must(valor => Enum.IsDefined(typeof(NaturezaChamadoEnum), valor))
            .WithMessage("Natureza do chamado invalida.");
        
        RuleFor(x => x.ImpactoChamado)
            .Must(valor => !valor.HasValue || Enum.IsDefined(typeof(ImpactoChamadoEnum), valor.Value))
            .WithMessage("Impacto do chamado invalido.");

        RuleFor(x => x.UrgenciaChamado)
            .Must(valor => !valor.HasValue || Enum.IsDefined(typeof(UrgenciaChamadoEnum), valor.Value))
            .WithMessage("Urgencia do chamado invalida.");

        RuleFor(x => x.SubcategoriaId)
            .Must(valor => !valor.HasValue || valor.Value != Guid.Empty);

        RuleFor(x => x.TipoSolicitacaoId)
            .Must(valor => !valor.HasValue || valor.Value != Guid.Empty);

        RuleFor(x => x.LocalUnidadeId)
            .Must(valor => !valor.HasValue || valor.Value != Guid.Empty);

        RuleFor(x => x.DepartamentoId)
            .Must(valor => !valor.HasValue || valor.Value != Guid.Empty);

        RuleFor(x => x.Origem)
            .NotEmpty()
            .Must(origem => origem is "Portal" or "Email" or "Admin")
            .WithMessage("Origem invalida. Valores permitidos: Portal, Email ou Admin.");

        RuleFor(x => x).Custom((request, context) =>
        {
            var input = new CamposObrigatoriosChamadoInput
            {
                NaturezaChamado = request.NaturezaChamado,
                ImpactoChamado = request.ImpactoChamado,
                UrgenciaChamado = request.UrgenciaChamado,
                Titulo = request.Titulo,
                Descricao = request.Descricao,
                CategoriaId = request.CategoriaId,
                TipoSolicitacaoId = request.TipoSolicitacaoId,
                Origem = request.Origem
            };

            var erros = camposObrigatoriosChamadoService.ValidarCriacao(input);
            foreach (var erro in erros)
            {
                context.AddFailure(erro.Campo, erro.Mensagem);
            }
        });
    }
}
