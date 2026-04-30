package br.geti.sistemachamado.aplicacao.chamado.email;

public record AnexoChamadoEmailComando(
        String nomeArquivo,
        String tipoConteudo,
        byte[] conteudo
) {
}

