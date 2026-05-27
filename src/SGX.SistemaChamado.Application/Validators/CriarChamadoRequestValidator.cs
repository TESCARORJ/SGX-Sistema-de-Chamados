using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Chamados;
using SGX.SistemaChamado.Application.DTOs.Portal;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class CriarChamadoRequestValidator : AbstractValidator<CriarChamadoRequest>
{
    public CriarChamadoRequestValidator(ICamposObrigatoriosChamadoService camposObrigatoriosChamadoService)
    {
        RuleFor(x => x.Titulo)
            .MaximumLength(180).WithMessage("Titulo deve ter no maximo 180 caracteres.");

        RuleFor(x => x.Descricao)
            .MaximumLength(4000).WithMessage("Descricao deve ter no maximo 4000 caracteres.");

        RuleFor(x => x.CategoriaId)
            .Must((request, valor) => request.CatalogoServicoId.HasValue || !string.IsNullOrWhiteSpace(request.CatalogoServicoSlug) || valor.HasValue)
            .WithMessage("Categoria obrigatoria quando nao houver servico de catalogo.")
            .Must(valor => !valor.HasValue || valor.Value != Guid.Empty)
            .WithMessage("Categoria invalida.");

        RuleFor(x => x.PrioridadeId)
            .Must((request, valor) => request.CatalogoServicoId.HasValue || !string.IsNullOrWhiteSpace(request.CatalogoServicoSlug) || valor.HasValue)
            .WithMessage("Prioridade obrigatoria quando nao houver servico de catalogo.")
            .Must(valor => !valor.HasValue || valor.Value != Guid.Empty)
            .WithMessage("Prioridade invalida.");

        RuleFor(x => x.NaturezaChamado)
            .Must(valor => !valor.HasValue || Enum.IsDefined(typeof(NaturezaChamadoEnum), valor.Value))
            .WithMessage("Natureza do chamado invalida.");

        RuleFor(x => x.ImpactoChamado)
            .Must(valor => !valor.HasValue || Enum.IsDefined(typeof(ImpactoChamadoEnum), valor.Value))
            .WithMessage("Impacto do chamado invalido.");

        RuleFor(x => x.UrgenciaChamado)
            .Must(valor => !valor.HasValue || Enum.IsDefined(typeof(UrgenciaChamadoEnum), valor.Value))
            .WithMessage("Urgencia do chamado invalida.");

        RuleFor(x => x.CatalogoServicoId)
            .Must(valor => !valor.HasValue || valor.Value != Guid.Empty)
            .WithMessage("Servico de catalogo invalido.");

        RuleFor(x => x.CatalogoServicoSlug)
            .MaximumLength(160)
            .WithMessage("Slug do servico de catalogo deve ter no maximo 160 caracteres.");

        RuleFor(x => x.SubcategoriaId)
            .Must(valor => !valor.HasValue || valor.Value != Guid.Empty)
            .WithMessage("Subcategoria invalida.");

        RuleFor(x => x.TipoSolicitacaoId)
            .Must(valor => !valor.HasValue || valor.Value != Guid.Empty)
            .WithMessage("Tipo de solicitacao invalido.");

        RuleFor(x => x.LocalUnidadeId)
            .Must(valor => !valor.HasValue || valor.Value != Guid.Empty)
            .WithMessage("Local/unidade invalido.");

        RuleFor(x => x.DepartamentoId)
            .Must(valor => !valor.HasValue || valor.Value != Guid.Empty)
            .WithMessage("Departamento invalido.");

        RuleFor(x => x.InventarioAtivoId)
            .Must(valor => !valor.HasValue || valor.Value != Guid.Empty)
            .WithMessage("InventarioAtivoId invalido.");

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
                CatalogoServicoId = request.CatalogoServicoId,
                CatalogoServicoSlug = request.CatalogoServicoSlug,
                Origem = "Portal"
            };

            var erros = camposObrigatoriosChamadoService.ValidarCriacao(input);
            foreach (var erro in erros)
            {
                context.AddFailure(erro.Campo, erro.Mensagem);
            }
        });
    }
}
