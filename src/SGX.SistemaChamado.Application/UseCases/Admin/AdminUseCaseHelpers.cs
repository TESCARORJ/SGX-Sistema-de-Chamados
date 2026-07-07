using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Services.Sla;
using SGX.SistemaChamado.Application.UseCases.Chamados;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

internal static class AdminUseCaseHelpers
{
    public static bool PodeOperarAdmin(UsuarioContextoAplicacao usuario)
        => usuario.PossuiQualquerPerfil("Administrador", "Atendente");

    public static bool EhAdministrador(UsuarioContextoAplicacao usuario)
        => usuario.PossuiPerfil("Administrador");

    public static string ObterDescricaoHistorico(TipoHistoricoChamado tipo, string? detalhe = null)
        => tipo switch
        {
            TipoHistoricoChamado.ResponsavelAlterado => string.IsNullOrWhiteSpace(detalhe)
                ? "Responsavel alterado"
                : detalhe,
            TipoHistoricoChamado.StatusAlterado => string.IsNullOrWhiteSpace(detalhe)
                ? "Status alterado"
                : detalhe,
            TipoHistoricoChamado.PrioridadeAlterada => string.IsNullOrWhiteSpace(detalhe)
                ? "Prioridade alterada"
                : detalhe,
            TipoHistoricoChamado.CategoriaAlterada => string.IsNullOrWhiteSpace(detalhe)
                ? "Categoria alterada"
                : detalhe,
            TipoHistoricoChamado.ComentarioAdicionado => string.IsNullOrWhiteSpace(detalhe)
                ? "Comentario administrativo adicionado"
                : detalhe,
            TipoHistoricoChamado.Encerrado => string.IsNullOrWhiteSpace(detalhe)
                ? "Chamado encerrado"
                : detalhe,
            TipoHistoricoChamado.Reaberto => string.IsNullOrWhiteSpace(detalhe)
                ? "Chamado reaberto"
                : detalhe,
            TipoHistoricoChamado.ArtigoConhecimentoVinculado => string.IsNullOrWhiteSpace(detalhe)
                ? "Artigo da base de conhecimento vinculado ao chamado"
                : detalhe,
            TipoHistoricoChamado.ArtigoConhecimentoDesvinculado => string.IsNullOrWhiteSpace(detalhe)
                ? "Artigo da base de conhecimento removido do chamado"
                : detalhe,
            TipoHistoricoChamado.AtivoVinculado => string.IsNullOrWhiteSpace(detalhe)
                ? "Ativo vinculado ao chamado"
                : detalhe,
            TipoHistoricoChamado.AtivoRemovido => string.IsNullOrWhiteSpace(detalhe)
                ? "Vinculo de ativo removido do chamado"
                : detalhe,
            TipoHistoricoChamado.AprovacaoSolicitada => string.IsNullOrWhiteSpace(detalhe)
                ? "Aprovacao solicitada"
                : detalhe,
            TipoHistoricoChamado.ChamadoAprovado => string.IsNullOrWhiteSpace(detalhe)
                ? "Chamado aprovado"
                : detalhe,
            TipoHistoricoChamado.ChamadoReprovado => string.IsNullOrWhiteSpace(detalhe)
                ? "Chamado reprovado"
                : detalhe,
            TipoHistoricoChamado.AprovacaoCancelada => string.IsNullOrWhiteSpace(detalhe)
                ? "Aprovacao cancelada"
                : detalhe,
            TipoHistoricoChamado.RelacionamentoCriado => string.IsNullOrWhiteSpace(detalhe)
                ? "Vinculo criado com outro chamado"
                : detalhe,
            TipoHistoricoChamado.RelacionamentoRecebido => string.IsNullOrWhiteSpace(detalhe)
                ? "Vinculo recebido de outro chamado"
                : detalhe,
            TipoHistoricoChamado.ChamadoDerivadoCriado => string.IsNullOrWhiteSpace(detalhe)
                ? "Chamado derivado criado"
                : detalhe,
            TipoHistoricoChamado.CriadoAPartirDeChamado => string.IsNullOrWhiteSpace(detalhe)
                ? "Chamado criado a partir de outro chamado"
                : detalhe,
            TipoHistoricoChamado.TarefaCriada => string.IsNullOrWhiteSpace(detalhe)
                ? "Tarefa vinculada criada"
                : detalhe,
            TipoHistoricoChamado.TarefaStatusAlterado => string.IsNullOrWhiteSpace(detalhe)
                ? "Status da tarefa vinculada alterado"
                : detalhe,
            TipoHistoricoChamado.TarefaConcluida => string.IsNullOrWhiteSpace(detalhe)
                ? "Tarefa vinculada concluida"
                : detalhe,
            TipoHistoricoChamado.TarefaCancelada => string.IsNullOrWhiteSpace(detalhe)
                ? "Tarefa vinculada cancelada"
                : detalhe,
            TipoHistoricoChamado.AprovacaoCriada => string.IsNullOrWhiteSpace(detalhe)
                ? "Aprovacao vinculada criada"
                : detalhe,
            TipoHistoricoChamado.AprovacaoAprovada => string.IsNullOrWhiteSpace(detalhe)
                ? "Aprovacao vinculada aprovada"
                : detalhe,
            TipoHistoricoChamado.AprovacaoReprovada => string.IsNullOrWhiteSpace(detalhe)
                ? "Aprovacao vinculada reprovada"
                : detalhe,
            TipoHistoricoChamado.GrupoTecnicoTransferido => string.IsNullOrWhiteSpace(detalhe)
                ? "Grupo tecnico transferido"
                : detalhe,
            TipoHistoricoChamado.GrupoTecnicoDefinido => string.IsNullOrWhiteSpace(detalhe)
                ? "Grupo tecnico definido"
                : detalhe,
            TipoHistoricoChamado.FilaAtendimentoDefinida => string.IsNullOrWhiteSpace(detalhe)
                ? "Fila de atendimento definida"
                : detalhe,
            TipoHistoricoChamado.FilaAtendimentoRemovida => string.IsNullOrWhiteSpace(detalhe)
                ? "Fila de atendimento removida"
                : detalhe,
            TipoHistoricoChamado.FilaAtendimentoTransferida => string.IsNullOrWhiteSpace(detalhe)
                ? "Fila de atendimento transferida"
                : detalhe,
            TipoHistoricoChamado.ResponsavelRemovidoPorTransferenciaGrupo => string.IsNullOrWhiteSpace(detalhe)
                ? "Responsavel removido por transferencia de grupo tecnico"
                : detalhe,
            TipoHistoricoChamado.ChamadoAssumidoDaFila => string.IsNullOrWhiteSpace(detalhe)
                ? "Chamado assumido da fila"
                : detalhe,
            _ => detalhe ?? tipo.ToString()
        };

    public static ChamadoAdminResumoResponse MapResumo(Chamado chamado)
    {
        var agora = DateTime.UtcNow;
        var situacao = SlaRules.CalcularSituacao(chamado.ChamadoSla, agora);
        var aprovacao = AprovacaoChamadoHelper.ObterEstado(chamado);

        return new ChamadoAdminResumoResponse
        {
            Id = chamado.Id,
            Codigo = chamado.Codigo,
            Titulo = chamado.Titulo,
            SolicitanteNome = chamado.Solicitante.Nome,
            SolicitanteEmail = chamado.Solicitante.Email,
            ResponsavelNome = chamado.Responsavel?.Nome,
            Status = chamado.Status.Nome,
            Prioridade = chamado.Prioridade.Nome,
            NaturezaChamado = chamado.NaturezaChamado,
            ImpactoChamado = chamado.ImpactoChamado,
            UrgenciaChamado = chamado.UrgenciaChamado,
            Categoria = chamado.Categoria.Nome,
            Subcategoria = chamado.Subcategoria?.Nome,
            TipoSolicitacao = chamado.TipoSolicitacao?.Nome,
            LocalUnidade = chamado.LocalUnidade?.Nome,
            Departamento = chamado.Departamento?.Nome,
            GrupoTecnicoId = chamado.GrupoTecnicoId,
            GrupoTecnicoNome = chamado.GrupoTecnico?.Nome,
            FilaAtendimentoId = chamado.FilaAtendimentoId,
            FilaAtendimentoNome = chamado.FilaAtendimento?.Nome,
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
            EncerradoEm = chamado.EncerradoEm,
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
            AprovacaoChamadoId = aprovacao.AprovacaoChamadoId
        };
    }

    public static ChamadoAdminDetalheResponse MapDetalhe(
        Chamado chamado,
        IReadOnlyCollection<int>? statusPermitidosCodigos = null,
        IReadOnlyCollection<AcaoChamadoEnum>? acoesDisponiveis = null)
    {
        var agora = DateTime.UtcNow;
        var situacao = SlaRules.CalcularSituacao(chamado.ChamadoSla, agora);
        var aprovacao = AprovacaoChamadoHelper.ObterEstado(chamado);

        return new ChamadoAdminDetalheResponse
        {
            Id = chamado.Id,
            Codigo = chamado.Codigo,
            Titulo = chamado.Titulo,
            Descricao = chamado.Descricao,
            Solicitante = new SolicitanteAdminResponse(
                chamado.SolicitanteId,
                chamado.Solicitante.Nome,
                chamado.Solicitante.Email),
            Responsavel = chamado.Responsavel is null
                ? null
                : new ResponsavelAdminResponse(chamado.Responsavel.Id, chamado.Responsavel.Nome, chamado.Responsavel.Email),
            Status = chamado.Status.Nome,
            Prioridade = chamado.Prioridade.Nome,
            NaturezaChamado = chamado.NaturezaChamado,
            ImpactoChamado = chamado.ImpactoChamado,
            UrgenciaChamado = chamado.UrgenciaChamado,
            Categoria = chamado.Categoria.Nome,
            Subcategoria = chamado.Subcategoria?.Nome,
            TipoSolicitacao = chamado.TipoSolicitacao?.Nome,
            LocalUnidade = chamado.LocalUnidade?.Nome,
            Departamento = chamado.Departamento?.Nome,
            GrupoTecnicoId = chamado.GrupoTecnicoId,
            GrupoTecnicoNome = chamado.GrupoTecnico?.Nome,
            FilaAtendimentoId = chamado.FilaAtendimentoId,
            FilaAtendimentoNome = chamado.FilaAtendimento?.Nome,
            CategoriaId = chamado.CategoriaId,
            SubcategoriaId = chamado.SubcategoriaId,
            PrioridadeId = chamado.PrioridadeId,
            TipoSolicitacaoId = chamado.TipoSolicitacaoId,
            LocalUnidadeId = chamado.LocalUnidadeId,
            DepartamentoId = chamado.DepartamentoId,
            InventarioAtivoId = chamado.InventarioAtivoId,
            InventarioAtivoCodigo = chamado.InventarioAtivo?.Codigo,
            InventarioAtivoNome = chamado.InventarioAtivo?.Nome,
            Origem = chamado.Origem.ToString(),
            AbertoEm = chamado.AbertoEm,
            EncerradoEm = chamado.EncerradoEm,
            RequerAprovacao = aprovacao.RequerAprovacao,
            AprovacaoPendente = aprovacao.AprovacaoPendente,
            StatusAprovacao = aprovacao.StatusAprovacao,
            AprovacaoChamadoId = aprovacao.AprovacaoChamadoId,
            StatusPermitidosCodigos = statusPermitidosCodigos ?? [],
            AcoesDisponiveisCodigos = acoesDisponiveis?.Select(x => x.ToString()).ToArray() ?? [],
            RespostasFormulario = chamado.RespostasFormulario
                .Where(x => x.Ativo)
                .OrderBy(x => x.CampoFormularioServico.Ordem)
                .ThenBy(x => x.CriadoEm)
                .Select(x => new RespostaFormularioDetalheAdminResponse(
                    x.CampoFormularioServicoId,
                    x.CampoFormularioServico.Nome,
                    x.CampoFormularioServico.Rotulo,
                    x.CampoFormularioServico.Tipo,
                    x.Valor,
                    x.ObterValores(),
                    x.CampoFormularioServico.Ordem))
                .ToArray(),
            Comentarios = chamado.Comentarios
                .Where(x => x.Ativo)
                .OrderBy(x => x.CriadoEm)
                .Select(x => new ComentarioAdminResponse(x.Id, x.UsuarioId, x.Usuario.Nome, x.Mensagem, x.Interno, x.CriadoEm))
                .ToArray(),
            Anexos = chamado.Anexos
                .Where(x => x.Ativo)
                .OrderByDescending(x => x.CriadoEm)
                .Select(x => new AnexoAdminResponse(x.Id, x.NomeArquivo, x.ContentType, x.TamanhoBytes, x.CriadoEm, x.UsuarioId, x.Usuario.Nome))
                .ToArray(),
            Historico = chamado.Historicos
                .OrderByDescending(x => x.CriadoEm)
                .Select(x => new HistoricoAdminResponse(x.Id, (int)x.Tipo, x.Tipo.ToString(), x.Descricao, x.CriadoEm, x.UsuarioId, x.Usuario?.Nome))
                .ToArray(),
            HistoricoSla = chamado.EventosSla
                .OrderByDescending(x => x.DataEvento)
                .Select(x => new EventoSlaAdminResponse(
                    x.Id,
                    (int)x.TipoEvento,
                    x.TipoEvento.ToString(),
                    x.Descricao,
                    x.DataEvento,
                    x.UsuarioId,
                    x.Usuario?.Nome))
                .ToArray(),
            Sla = chamado.ChamadoSla is null
                ? null
                : new SlaAdminResponse(
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
}
