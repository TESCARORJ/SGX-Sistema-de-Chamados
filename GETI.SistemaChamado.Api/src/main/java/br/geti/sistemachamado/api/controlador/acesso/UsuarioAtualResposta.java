package br.geti.sistemachamado.api.controlador.acesso;

import java.util.UUID;

public record UsuarioAtualResposta(
        UUID usuarioId,
        String login,
        String nome,
        String email,
        String perfilAcesso,
        UUID departamentoId
) {
}
