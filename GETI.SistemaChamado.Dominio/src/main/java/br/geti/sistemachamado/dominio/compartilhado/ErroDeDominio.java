package br.geti.sistemachamado.dominio.compartilhado;

public class ErroDeDominio extends RuntimeException {

    public ErroDeDominio(final String mensagem) {
        super(mensagem);
    }
}

