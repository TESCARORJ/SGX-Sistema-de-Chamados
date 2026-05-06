package br.geti.sistemachamado.aplicacao.acesso;

import java.util.UUID;

public record ComandoSincronizacaoUsuarioAutenticado(
        String nome,
        String login,
        String email,
        UUID departamentoId
) {
}
