package br.geti.sistemachamado.dominio.administracao;

import java.time.LocalDateTime;
import java.util.UUID;
import br.geti.sistemachamado.dominio.compartilhado.AgregadoRaiz;
import br.geti.sistemachamado.dominio.compartilhado.ValidadorDominio;

public record PerfilAcesso(
        UUID id,
        String nome,
        String descricao,
        boolean ativo,
        LocalDateTime dataCriacao,
        LocalDateTime dataAtualizacao
) implements AgregadoRaiz {

    public PerfilAcesso {
        ValidadorDominio.obrigatorio(id, "id do perfil de acesso e obrigatorio");
        nome = ValidadorDominio.textoObrigatorio(nome, "nome do perfil de acesso e obrigatorio");
        validarAuditoria(dataCriacao, dataAtualizacao);
    }
}

