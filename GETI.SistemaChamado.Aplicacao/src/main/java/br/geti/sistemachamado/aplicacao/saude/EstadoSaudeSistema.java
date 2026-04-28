package br.geti.sistemachamado.aplicacao.saude;

import java.time.OffsetDateTime;

public record EstadoSaudeSistema(String servico, String ambiente, OffsetDateTime timestamp) {
}

