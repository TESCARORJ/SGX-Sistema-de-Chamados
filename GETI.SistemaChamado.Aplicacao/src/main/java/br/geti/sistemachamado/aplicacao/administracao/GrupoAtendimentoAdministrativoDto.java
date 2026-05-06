package br.geti.sistemachamado.aplicacao.administracao;

import java.time.LocalDateTime;
import java.util.UUID;

public record GrupoAtendimentoAdministrativoDto(
        UUID id,
        String nome,
        String descricao,
        boolean ativo,
        UUID departamentoId,
        String departamentoNome,
        LocalDateTime dataCriacao,
        LocalDateTime dataAtualizacao
) {
}
