using SGX.SistemaChamado.Application.DTOs.Portal;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Services.Sla;
using SGX.SistemaChamado.Application.UseCases.Chamados;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Portal;

internal static class PortalUseCaseHelpers
{
    public static bool PodeVisaoAmpliada(UsuarioContextoAplicacao usuario)
        => usuario.PossuiQualquerPerfil("Administrador", "Atendente");

    public static bool PodeAcessarChamado(UsuarioContextoAplicacao usuario, Chamado chamado)
        => PodeVisaoAmpliada(usuario) || chamado.SolicitanteId == usuario.Id;

    public static ChamadoResumoResponse MapResumo(Chamado chamado)
    {
        var agora = DateTime.UtcNow;
        var situacao = SlaRules.CalcularSituacao(chamado.ChamadoSla, agora);
        var aprovacao = AprovacaoChamadoHelper.ObterEstado(chamado);

        return new ChamadoResumoResponse
        {
            Id = chamado.Id,
            Codigo = chamado.Codigo,
            Titulo = chamado.Titulo,
            Status = chamado.Status.Nome,
            Prioridade = chamado.Prioridade.Nome,
            Categoria = chamado.Categoria.Nome,
            Subcategoria = chamado.Subcategoria?.Nome,
            TipoSolicitacao = chamado.TipoSolicitacao?.Nome,
            LocalUnidade = chamado.LocalUnidade?.Nome,
            Departamento = chamado.Departamento?.Nome,
            CategoriaId = chamado.CategoriaId,
            SubcategoriaId = chamado.SubcategoriaId,
            PrioridadeId = chamado.PrioridadeId,
            TipoSolicitacaoId = chamado.TipoSolicitacaoId,
            LocalUnidadeId = chamado.LocalUnidadeId,
            DepartamentoId = chamado.DepartamentoId,
            InventarioAtivoId = chamado.InventarioAtivoId,
            InventarioAtivoCodigo = chamado.InventarioAtivo?.Codigo,
            InventarioAtivoNome = chamado.InventarioAtivo?.Nome,
            AbertoEm = chamado.AbertoEm,
            AtualizadoEm = chamado.AtualizadoEm,
            SlaVencido = situacao is SituacaoSlaChamadoEnum.Vencido or SituacaoSlaChamadoEnum.Violado,
            SlaProximoVencimento = situacao == SituacaoSlaChamadoEnum.ProximoDoVencimento,
            SituacaoSla = situacao,
            PoliticaSlaNome = chamado.ChamadoSla?.PoliticaSla?.Nome,
            TempoRestanteMinutos = SlaRules.CalcularTempoRestanteMinutos(chamado.ChamadoSla, agora),
            TempoExcedidoMinutos = SlaRules.CalcularTempoExcedidoMinutos(chamado.ChamadoSla, agora),
            PrazoPrimeiraRespostaEm = chamado.ChamadoSla?.PrazoPrimeiraResposta,
            PrimeiraRespostaEm = chamado.ChamadoSla?.DataPrimeiraResposta,
            PrazoResolucaoEm = chamado.ChamadoSla?.PrazoResolucao,
            ResolvidoEm = chamado.ChamadoSla?.DataResolucao,
            EstaPausado = chamado.ChamadoSla?.Pausado ?? false,
            TotalMinutosPausado = chamado.ChamadoSla?.MinutosPausados ?? 0,
            RequerAprovacao = aprovacao.RequerAprovacao,
            AprovacaoPendente = aprovacao.AprovacaoPendente,
            StatusAprovacao = aprovacao.StatusAprovacao,
            AprovacaoChamadoId = aprovacao.AprovacaoChamadoId,
            AprovacaoSolicitadaEm = aprovacao.AprovacaoSolicitadaEm,
            AprovacaoDecididaEm = aprovacao.AprovacaoDecididaEm,
            JustificativaAprovacao = aprovacao.JustificativaAprovacao,
            JustificativaReprovacao = aprovacao.JustificativaReprovacao,
            MensagemOrientativaAprovacao = aprovacao.MensagemOrientativa
        };
    }

    public static ChamadoDetalheResponse MapDetalhe(Chamado chamado, UsuarioContextoAplicacao? usuarioContexto = null)
    {
        var historico = chamado.Historicos.AsEnumerable();
        if (usuarioContexto is not null && !PodeVisaoAmpliada(usuarioContexto))
        {
            historico = historico.Where(x =>
                !(x.Tipo == TipoHistoricoChamado.ComentarioAdicionado &&
                  x.Descricao.Contains("interno", StringComparison.OrdinalIgnoreCase)));
        }

        var agora = DateTime.UtcNow;
        var situacao = SlaRules.CalcularSituacao(chamado.ChamadoSla, agora);
        var aprovacao = AprovacaoChamadoHelper.ObterEstado(chamado);

        return new ChamadoDetalheResponse
        {
            Id = chamado.Id,
            Codigo = chamado.Codigo,
            Titulo = chamado.Titulo,
            Descricao = chamado.Descricao,
            Status = chamado.Status.Nome,
            Prioridade = chamado.Prioridade.Nome,
            Categoria = chamado.Categoria.Nome,
            Subcategoria = chamado.Subcategoria?.Nome,
            TipoSolicitacao = chamado.TipoSolicitacao?.Nome,
            LocalUnidade = chamado.LocalUnidade?.Nome,
            Departamento = chamado.Departamento?.Nome,
            CategoriaId = chamado.CategoriaId,
            SubcategoriaId = chamado.SubcategoriaId,
            PrioridadeId = chamado.PrioridadeId,
            TipoSolicitacaoId = chamado.TipoSolicitacaoId,
            LocalUnidadeId = chamado.LocalUnidadeId,
            DepartamentoId = chamado.DepartamentoId,
            InventarioAtivoId = chamado.InventarioAtivoId,
            InventarioAtivoCodigo = chamado.InventarioAtivo?.Codigo,
            InventarioAtivoNome = chamado.InventarioAtivo?.Nome,
            Solicitante = chamado.Solicitante.Nome,
            Responsavel = chamado.Responsavel?.Nome,
            AbertoEm = chamado.AbertoEm,
            EncerradoEm = chamado.EncerradoEm,
            RequerAprovacao = aprovacao.RequerAprovacao,
            AprovacaoPendente = aprovacao.AprovacaoPendente,
            StatusAprovacao = aprovacao.StatusAprovacao,
            AprovacaoChamadoId = aprovacao.AprovacaoChamadoId,
            AprovacaoSolicitadaEm = aprovacao.AprovacaoSolicitadaEm,
            AprovacaoDecididaEm = aprovacao.AprovacaoDecididaEm,
            JustificativaAprovacao = aprovacao.JustificativaAprovacao,
            JustificativaReprovacao = aprovacao.JustificativaReprovacao,
            MensagemOrientativaAprovacao = aprovacao.MensagemOrientativa,
            Comentarios = chamado.Comentarios
                .Where(x => x.Ativo && !x.Interno)
                .OrderBy(x => x.CriadoEm)
                .Select(x => new ComentarioChamadoResponse(x.Id, x.UsuarioId, x.Usuario.Nome, x.Mensagem, x.CriadoEm))
                .ToArray(),
            Anexos = chamado.Anexos
                .Where(x => x.Ativo)
                .OrderByDescending(x => x.CriadoEm)
                .Select(x => new AnexoChamadoResponse(x.Id, x.NomeArquivo, x.ContentType, x.TamanhoBytes, x.CriadoEm, x.UsuarioId, x.Usuario.Nome))
                .ToArray(),
            Historico = historico
                .OrderByDescending(x => x.CriadoEm)
                .Select(x => new HistoricoChamadoResponse(
                    x.Id,
                    (int)x.Tipo,
                    x.Tipo.ToString(),
                    x.Descricao,
                    x.CriadoEm,
                    x.UsuarioId,
                    x.Usuario?.Nome))
                .ToArray(),
            Sla = chamado.ChamadoSla is null
                ? null
                : new SlaResumoResponse(
                    chamado.ChamadoSla.PoliticaSla?.Nome,
                    chamado.Prioridade.Nome,
                    chamado.ChamadoSla.DataInicio,
                    chamado.ChamadoSla.PrazoPrimeiraResposta,
                    chamado.ChamadoSla.DataPrimeiraResposta,
                    chamado.ChamadoSla.PrazoResolucao,
                    chamado.ChamadoSla.DataResolucao,
                    chamado.ChamadoSla.PrimeiraRespostaCumprida,
                    chamado.ChamadoSla.ResolucaoCumprida,
                    chamado.ChamadoSla.PrimeiraRespostaViolada,
                    chamado.ChamadoSla.ResolucaoViolada,
                    situacao is SituacaoSlaChamadoEnum.Vencido or SituacaoSlaChamadoEnum.Violado,
                    chamado.ChamadoSla.Pausado,
                    situacao,
                    chamado.ChamadoSla.MinutosPrimeiraResposta,
                    chamado.ChamadoSla.MinutosResolucao,
                    SlaRules.CalcularTempoRestanteMinutos(chamado.ChamadoSla, agora),
                    SlaRules.CalcularTempoExcedidoMinutos(chamado.ChamadoSla, agora),
                    chamado.ChamadoSla.MinutosPausados,
                    chamado.ChamadoSla.UsarHorarioComercial,
                    chamado.ChamadoSla.CalendarioCorporativo?.Nome)
        };
    }

    public static PortalStatusAprovacaoChamadoDto MapStatusAprovacao(Chamado chamado)
    {
        var aprovacao = AprovacaoChamadoHelper.ObterEstado(chamado);
        return new PortalStatusAprovacaoChamadoDto
        {
            ChamadoId = chamado.Id,
            RequerAprovacao = aprovacao.RequerAprovacao,
            AprovacaoPendente = aprovacao.AprovacaoPendente,
            StatusAprovacao = aprovacao.StatusAprovacao,
            AprovacaoChamadoId = aprovacao.AprovacaoChamadoId,
            SolicitadaEm = aprovacao.AprovacaoSolicitadaEm,
            DecididaEm = aprovacao.AprovacaoDecididaEm,
            JustificativaDecisao = aprovacao.JustificativaDecisao,
            MensagemOrientativa = aprovacao.MensagemOrientativa
        };
    }

    public static string ObterDescricaoHistorico(TipoHistoricoChamado tipo)
        => tipo switch
        {
            TipoHistoricoChamado.Criado => "Chamado criado",
            TipoHistoricoChamado.ComentarioAdicionado => "Comentario adicionado",
            TipoHistoricoChamado.AnexoAdicionado => "Anexo adicionado",
            TipoHistoricoChamado.AtivoVinculado => "Ativo vinculado ao chamado",
            TipoHistoricoChamado.AtivoRemovido => "Vinculo de ativo removido do chamado",
            TipoHistoricoChamado.AprovacaoSolicitada => "Aprovacao solicitada",
            TipoHistoricoChamado.ChamadoAprovado => "Chamado aprovado",
            TipoHistoricoChamado.ChamadoReprovado => "Chamado reprovado",
            TipoHistoricoChamado.AprovacaoCancelada => "Aprovacao cancelada",
            _ => tipo.ToString()
        };
}
