package br.geti.sistemachamado.aplicacao.chamado.admin;

import java.util.UUID;

public record EncaminhamentoChamadoAdminComando(
        UUID chamadoId,
        UUID departamentoId,
        UUID categoriaId,
        UUID servicoId,
        UUID agenteId
) {
}
