package br.geti.sistemachamado.aplicacao.chamado.email;

import java.util.UUID;

public record InteracaoChamadoPorEmailDto(
        UUID chamadoId,
        UUID interacaoId
) {
}
