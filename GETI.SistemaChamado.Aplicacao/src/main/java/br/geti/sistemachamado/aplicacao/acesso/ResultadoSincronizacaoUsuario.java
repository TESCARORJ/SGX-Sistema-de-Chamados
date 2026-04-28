package br.geti.sistemachamado.aplicacao.acesso;

import java.util.UUID;

public record ResultadoSincronizacaoUsuario(
        UUID usuarioId,
        String nome,
        String login,
        String email,
        String perfilAcesso,
        UUID departamentoId,
        boolean criado
) {
}
