package br.geti.sistemachamado.aplicacao.chamado.portal;

public record AnexoChamadoPortalComando(
        String nomeArquivo,
        String tipoConteudo,
        byte[] conteudo
) {
}
