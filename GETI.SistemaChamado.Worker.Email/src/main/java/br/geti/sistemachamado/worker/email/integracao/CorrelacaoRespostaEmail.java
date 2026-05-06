package br.geti.sistemachamado.worker.email.integracao;

import java.util.UUID;

public record CorrelacaoRespostaEmail(
        UUID chamadoId,
        String messageIdCorrelacionado
) {
}
