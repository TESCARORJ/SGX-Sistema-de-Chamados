package br.geti.sistemachamado.aplicacao.chamado.email;

import br.geti.sistemachamado.dominio.chamado.PrioridadeChamado;
import java.util.List;
import java.util.UUID;

public record AberturaChamadoEmailComando(
        UUID caixaDeEmailId,
        String remetenteNome,
        String remetenteEmail,
        String destinatario,
        String assunto,
        String corpoMensagem,
        String messageId,
        PrioridadeChamado prioridade,
        List<AnexoChamadoEmailComando> anexos
) {
}

