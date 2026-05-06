package br.geti.sistemachamado.api.erro;

import java.time.OffsetDateTime;

public record ErroApiResposta(
        String codigo,
        String mensagem,
        String caminho,
        OffsetDateTime timestamp
) {
}

