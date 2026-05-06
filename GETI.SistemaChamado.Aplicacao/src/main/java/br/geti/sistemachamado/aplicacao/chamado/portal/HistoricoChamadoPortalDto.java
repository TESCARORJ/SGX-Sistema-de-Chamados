package br.geti.sistemachamado.aplicacao.chamado.portal;

import java.time.LocalDateTime;
import java.util.UUID;

public record HistoricoChamadoPortalDto(
        UUID id,
        String descricao,
        String situacaoAnterior,
        String situacaoNova,
        LocalDateTime dataCriacao
) {
}
