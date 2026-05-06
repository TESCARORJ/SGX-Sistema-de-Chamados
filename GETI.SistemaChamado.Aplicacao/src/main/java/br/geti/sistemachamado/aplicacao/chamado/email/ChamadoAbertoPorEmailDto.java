package br.geti.sistemachamado.aplicacao.chamado.email;

import java.util.UUID;

public record ChamadoAbertoPorEmailDto(
        UUID chamadoId,
        String numeroChamado
) {
}

