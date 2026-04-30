package br.geti.sistemachamado.aplicacao.chamado.admin;

import java.time.LocalDateTime;
import java.util.List;
import java.util.UUID;

public record ChamadoAdminDetalheDto(
        UUID id,
        String numero,
        String titulo,
        String descricao,
        String situacao,
        String prioridade,
        String origem,
        UUID solicitanteId,
        String solicitanteNome,
        String solicitanteLogin,
        String solicitanteEmail,
        UUID responsavelId,
        String responsavelNome,
        UUID departamentoId,
        String departamentoNome,
        UUID categoriaId,
        String categoriaNome,
        UUID servicoId,
        String servicoNome,
        String statusSla,
        Integer prazoSlaMinutos,
        LocalDateTime dataLimiteSla,
        Long minutosRestantesSla,
        Long minutosAtrasoSla,
        LocalDateTime dataCriacao,
        LocalDateTime dataAtualizacao,
        List<InteracaoChamadoAdminDto> interacoes,
        List<HistoricoChamadoAdminDto> historicos,
        List<AnexoChamadoAdminDto> anexos
) {
}
