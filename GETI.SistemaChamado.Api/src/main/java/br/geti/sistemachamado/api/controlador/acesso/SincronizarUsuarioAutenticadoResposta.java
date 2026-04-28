package br.geti.sistemachamado.api.controlador.acesso;

import java.util.UUID;

public record SincronizarUsuarioAutenticadoResposta(
        UUID usuarioId,
        String nome,
        String login,
        String email,
        String perfilAcesso,
        UUID departamentoId,
        boolean criado
) {
}
