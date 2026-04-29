package br.geti.sistemachamado.aplicacao.chamado.portal;

import java.time.LocalDateTime;
import java.util.UUID;

public record InteracaoChamadoPortalDto(
        UUID id,
        String tipoInteracao,
        String mensagem,
        String autor,
        LocalDateTime dataCriacao
) {
}
