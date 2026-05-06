package br.geti.sistemachamado.worker.email.integracao;

public record AnexoMensagemEmailRecebida(
        String nomeArquivo,
        String tipoConteudo,
        byte[] conteudo
) {
}

