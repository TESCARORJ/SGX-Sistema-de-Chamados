package br.geti.sistemachamado.aplicacao.acesso;

public record IdentidadeUsuarioAutenticado(
        String login,
        String nome,
        String email
) {
}
