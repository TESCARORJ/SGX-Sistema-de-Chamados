package br.geti.sistemachamado.aplicacao.chamado.admin;

import java.time.LocalDateTime;
import java.util.UUID;

public record HistoricoChamadoAdminDto(
        UUID id,
        String descricao,
        String situacaoAnterior,
        String situacaoNova,
        boolean visivelSolicitante,
        LocalDateTime dataCriacao
) {
}
