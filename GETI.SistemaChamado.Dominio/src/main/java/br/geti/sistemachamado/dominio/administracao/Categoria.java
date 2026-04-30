package br.geti.sistemachamado.dominio.administracao;

import br.geti.sistemachamado.dominio.compartilhado.AgregadoRaiz;
import br.geti.sistemachamado.dominio.compartilhado.ValidadorDominio;
import java.time.LocalDateTime;
import java.util.UUID;

public record Categoria(
        UUID id,
        String nome,
        String descricao,
        boolean ativo,
        LocalDateTime dataCriacao,
        LocalDateTime dataAtualizacao
) implements AgregadoRaiz {

    public Categoria {
        ValidadorDominio.obrigatorio(id, "id da categoria e obrigatorio");
        nome = ValidadorDominio.textoObrigatorio(nome, "nome da categoria e obrigatorio");
        if (descricao != null) {
            descricao = descricao.trim();
            if (descricao.isEmpty()) {
                descricao = null;
            }
        }
        validarAuditoria(dataCriacao, dataAtualizacao);
    }
}

