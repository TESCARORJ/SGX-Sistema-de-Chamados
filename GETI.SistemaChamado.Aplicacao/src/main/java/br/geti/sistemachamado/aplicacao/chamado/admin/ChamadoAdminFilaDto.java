package br.geti.sistemachamado.aplicacao.chamado.admin;

import java.time.LocalDateTime;
import java.util.UUID;

public record ChamadoAdminFilaDto(
        UUID id,
        String numero,
        String titulo,
        String situacao,
        String prioridade,
        String origem,
        String solicitanteNome,
        String departamentoNome,
        String categoriaNome,
        String servicoNome,
        UUID responsavelId,
        String responsavelNome,
        String statusSla,
        Integer prazoSlaMinutos,
        LocalDateTime dataLimiteSla,
        Long minutosRestantesSla,
        Long minutosAtrasoSla,
        LocalDateTime dataCriacao,
        LocalDateTime dataAtualizacao
) {
}
