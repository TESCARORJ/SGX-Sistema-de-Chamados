package br.geti.sistemachamado.aplicacao.chamado.portal;

import java.util.UUID;

public record AnexoArmazenadoChamado(
        UUID anexoId,
        String nomeArmazenado,
        String caminhoArmazenamento
) {
}
