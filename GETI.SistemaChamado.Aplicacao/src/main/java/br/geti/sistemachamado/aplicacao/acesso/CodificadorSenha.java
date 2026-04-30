package br.geti.sistemachamado.aplicacao.acesso;

public interface CodificadorSenha {

    String codificar(String senhaAberta);

    boolean corresponde(String senhaAberta, String senhaHash);
}
