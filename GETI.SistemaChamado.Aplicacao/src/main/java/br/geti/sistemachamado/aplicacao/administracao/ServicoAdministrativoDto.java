package br.geti.sistemachamado.aplicacao.administracao;

import java.time.LocalDateTime;
import java.util.UUID;

public record ServicoAdministrativoDto(
        UUID id,
        String nome,
        String descricao,
        boolean ativo,
        UUID categoriaId,
        String categoriaNome,
        UUID departamentoId,
        String departamentoNome,
        LocalDateTime dataCriacao,
        LocalDateTime dataAtualizacao
) {
}
