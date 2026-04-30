package br.geti.sistemachamado.aplicacao.chamado.email;

import java.util.List;
import java.util.UUID;

public record RespostaChamadoEmailComando(
        UUID chamadoId,
        String remetenteNome,
        String remetenteEmail,
        String assunto,
        String corpoMensagem,
        String messageId,
        String inReplyTo,
        List<AnexoChamadoEmailComando> anexos
) {
}
