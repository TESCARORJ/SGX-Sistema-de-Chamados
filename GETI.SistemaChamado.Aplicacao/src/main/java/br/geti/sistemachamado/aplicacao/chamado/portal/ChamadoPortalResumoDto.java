package br.geti.sistemachamado.aplicacao.chamado.portal;

import java.time.LocalDateTime;
import java.util.UUID;

public record ChamadoPortalResumoDto(
        UUID id,
        String numero,
        String titulo,
        String situacao,
        String prioridade,
        String categoria,
        String servico,
        LocalDateTime dataCriacao,
        LocalDateTime dataAtualizacao
) {
}
