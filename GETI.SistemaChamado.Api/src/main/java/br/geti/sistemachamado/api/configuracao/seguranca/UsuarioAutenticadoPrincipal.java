package br.geti.sistemachamado.api.configuracao.seguranca;

import java.util.UUID;

public record UsuarioAutenticadoPrincipal(
        UUID usuarioId,
        String login,
        String nome,
        String email,
        String perfilAcesso,
        UUID departamentoId
) {
}
