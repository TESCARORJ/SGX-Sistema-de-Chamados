using SGX.SistemaChamado.Application.DTOs.Chamados;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.Services;

public sealed class CamposObrigatoriosChamadoService : ICamposObrigatoriosChamadoService
{
    private const int MinimoCaracteresDetalhamentoMudanca = 30;
    private const int MinimoCaracteresDetalhamentoProblema = 30;

    public IReadOnlyCollection<ErroCampoObrigatorioChamado> ValidarCriacao(CamposObrigatoriosChamadoInput input)
    {
        ArgumentNullException.ThrowIfNull(input);

        var erros = new List<ErroCampoObrigatorioChamado>();
        var origemEmail = string.Equals(input.Origem, "Email", StringComparison.OrdinalIgnoreCase);

        if (!input.NaturezaChamado.HasValue || !Enum.IsDefined(input.NaturezaChamado.Value))
        {
            erros.Add(new ErroCampoObrigatorioChamado("NaturezaChamado", "Natureza do chamado obrigatoria."));
            return erros;
        }

        if (string.IsNullOrWhiteSpace(input.Titulo))
        {
            erros.Add(new ErroCampoObrigatorioChamado("Titulo", "Titulo obrigatorio."));
        }

        if (string.IsNullOrWhiteSpace(input.Descricao))
        {
            erros.Add(new ErroCampoObrigatorioChamado("Descricao", "Descricao obrigatoria."));
        }

        var naturezaExigeImpactoEUrgencia = input.NaturezaChamado.Value is
            NaturezaChamadoEnum.Incidente or
            NaturezaChamadoEnum.Mudanca or
            NaturezaChamadoEnum.Problema or
            NaturezaChamadoEnum.EventoAlerta or
            NaturezaChamadoEnum.TarefaOperacional;

        if (!origemEmail && naturezaExigeImpactoEUrgencia && !input.ImpactoChamado.HasValue)
        {
            erros.Add(new ErroCampoObrigatorioChamado("ImpactoChamado", "Impacto do chamado obrigatorio."));
        }

        if (!origemEmail && naturezaExigeImpactoEUrgencia && !input.UrgenciaChamado.HasValue)
        {
            erros.Add(new ErroCampoObrigatorioChamado("UrgenciaChamado", "Urgencia do chamado obrigatoria."));
        }

        var possuiCategoria = input.CategoriaId.HasValue && input.CategoriaId.Value != Guid.Empty;
        var possuiTipoSolicitacao = input.TipoSolicitacaoId.HasValue && input.TipoSolicitacaoId.Value != Guid.Empty;
        var possuiCatalogoServico = (input.CatalogoServicoId.HasValue && input.CatalogoServicoId.Value != Guid.Empty)
            || !string.IsNullOrWhiteSpace(input.CatalogoServicoSlug);

        if (!possuiCategoria && !possuiTipoSolicitacao && !possuiCatalogoServico)
        {
            erros.Add(new ErroCampoObrigatorioChamado("CategoriaId", "Categoria ou tipo de solicitacao obrigatorio."));
        }

        if (input.NaturezaChamado is NaturezaChamadoEnum.Mudanca &&
            !PossuiDetalhamentoMinimo(input.Descricao, MinimoCaracteresDetalhamentoMudanca))
        {
            erros.Add(new ErroCampoObrigatorioChamado(
                "Descricao",
                "Mudanca exige detalhamento minimo na descricao."));
        }

        if (input.NaturezaChamado is NaturezaChamadoEnum.Problema &&
            !PossuiDetalhamentoMinimo(input.Descricao, MinimoCaracteresDetalhamentoProblema))
        {
            erros.Add(new ErroCampoObrigatorioChamado(
                "Descricao",
                "Problema exige evidencias de recorrencia ou detalhamento minimo na descricao."));
        }

        return erros;
    }

    private static bool PossuiDetalhamentoMinimo(string? descricao, int minimoCaracteres)
    {
        if (string.IsNullOrWhiteSpace(descricao))
        {
            return false;
        }

        var texto = descricao.Trim();
        if (texto.Length < minimoCaracteres)
        {
            return false;
        }

        var quantidadePalavras = texto
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Length;

        return quantidadePalavras >= 5;
    }
}
