package br.geti.sistemachamado.aplicacao.chamado.admin;

import java.time.LocalDateTime;
import java.util.UUID;

public record ChamadoAdminResumoDashboardDto(
        UUID id,
        String numero,
        String titulo,
        String situacao,
        String prioridade,
        String departamento,
        String responsavel,
        LocalDateTime dataCriacao
) {
}
