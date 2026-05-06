package br.geti.sistemachamado.dominio.administracao;

import br.geti.sistemachamado.dominio.compartilhado.AgregadoRaiz;
import br.geti.sistemachamado.dominio.compartilhado.ValidadorDominio;
import java.time.LocalDateTime;
import java.util.UUID;

public record Servico(
        UUID id,
        String nome,
        String descricao,
        boolean ativo,
        Categoria categoria,
        Departamento departamento,
        LocalDateTime dataCriacao,
        LocalDateTime dataAtualizacao
) implements AgregadoRaiz {

    public Servico {
        ValidadorDominio.obrigatorio(id, "id do servico e obrigatorio");
        nome = ValidadorDominio.textoObrigatorio(nome, "nome do servico e obrigatorio");
        ValidadorDominio.obrigatorio(categoria, "categoria do servico e obrigatoria");
        ValidadorDominio.obrigatorio(departamento, "departamento do servico e obrigatorio");
        if (descricao != null) {
            descricao = descricao.trim();
            if (descricao.isEmpty()) {
                descricao = null;
            }
        }
        validarAuditoria(dataCriacao, dataAtualizacao);
    }
}

