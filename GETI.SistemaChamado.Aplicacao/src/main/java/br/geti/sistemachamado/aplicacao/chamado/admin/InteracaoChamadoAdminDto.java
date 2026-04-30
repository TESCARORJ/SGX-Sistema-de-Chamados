package br.geti.sistemachamado.aplicacao.chamado.admin;

import java.time.LocalDateTime;
import java.util.UUID;

public record InteracaoChamadoAdminDto(
        UUID id,
        String tipoInteracao,
        String mensagem,
        boolean visivelSolicitante,
        UUID autorId,
        String autorNome,
        LocalDateTime dataCriacao
) {
}
