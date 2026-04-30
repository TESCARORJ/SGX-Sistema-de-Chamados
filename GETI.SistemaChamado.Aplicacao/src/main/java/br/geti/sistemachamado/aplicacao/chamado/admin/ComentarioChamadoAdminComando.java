package br.geti.sistemachamado.aplicacao.chamado.admin;

import java.util.UUID;

public record ComentarioChamadoAdminComando(
        UUID chamadoId,
        UUID autorId,
        String mensagem
) {
}
