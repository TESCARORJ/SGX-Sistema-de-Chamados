package br.geti.sistemachamado.aplicacao.chamado.automacao;

import br.geti.sistemachamado.dominio.administracao.Usuario;

public record ResultadoAtribuicaoAutomaticaChamado(
        Usuario responsavel,
        String motivo
) {
}
