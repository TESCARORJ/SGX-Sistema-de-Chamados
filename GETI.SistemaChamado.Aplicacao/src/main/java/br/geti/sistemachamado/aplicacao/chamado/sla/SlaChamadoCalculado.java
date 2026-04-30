package br.geti.sistemachamado.aplicacao.chamado.sla;

import br.geti.sistemachamado.dominio.chamado.StatusSlaChamado;
import java.time.LocalDateTime;

public record SlaChamadoCalculado(
        int prazoSlaMinutos,
        LocalDateTime dataLimiteSla,
        long minutosRestantes,
        long minutosAtraso,
        StatusSlaChamado statusSla
) {
}
