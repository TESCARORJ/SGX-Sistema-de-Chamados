package br.geti.sistemachamado.aplicacao.chamado.portal;

import java.time.LocalDateTime;
import java.util.UUID;

public record AnexoChamadoPortalDto(
        UUID id,
        String nomeArquivo,
        String tipoConteudo,
        long tamanhoBytes,
        LocalDateTime dataCriacao
) {
}
