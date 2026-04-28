package br.geti.sistemachamado.dominio.compartilhado;

public final class ValidadorDominio {

    private ValidadorDominio() {
    }

    public static <T> T obrigatorio(final T valor, final String mensagem) {
        if (valor == null) {
            throw new ErroDeDominio(mensagem);
        }
        return valor;
    }

    public static String textoObrigatorio(final String valor, final String mensagem) {
        obrigatorio(valor, mensagem);
        final var normalizado = valor.trim();
        if (normalizado.isEmpty()) {
            throw new ErroDeDominio(mensagem);
        }
        return normalizado;
    }
}
