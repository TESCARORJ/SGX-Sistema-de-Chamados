package br.geti.sistemachamado.aplicacao.acesso;

import java.util.UUID;

public record ContextoUsuarioAutenticado(
        UUID usuarioId,
        String login,
        String nome,
        String email,
        String perfilAcesso,
        UUID departamentoId
) {
}
