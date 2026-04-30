package br.geti.sistemachamado.aplicacao.chamado.admin;

import java.time.LocalDateTime;
import java.util.UUID;

public record AnexoChamadoAdminDto(
        UUID id,
        String nomeArquivo,
        String tipoConteudo,
        long tamanhoBytes,
        UUID autorId,
        String autorNome,
        LocalDateTime dataCriacao
) {
}
