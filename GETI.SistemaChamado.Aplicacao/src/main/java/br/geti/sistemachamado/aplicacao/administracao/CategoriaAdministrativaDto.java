package br.geti.sistemachamado.aplicacao.administracao;

import java.time.LocalDateTime;
import java.util.UUID;

public record CategoriaAdministrativaDto(
        UUID id,
        String nome,
        String descricao,
        boolean ativo,
        LocalDateTime dataCriacao,
        LocalDateTime dataAtualizacao
) {
}
