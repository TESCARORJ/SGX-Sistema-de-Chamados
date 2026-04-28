package br.geti.sistemachamado.dominio.administracao;

import java.time.LocalDateTime;
import java.util.UUID;
import br.geti.sistemachamado.dominio.compartilhado.AgregadoRaiz;
import br.geti.sistemachamado.dominio.compartilhado.ValidadorDominio;

public record Usuario(
        UUID id,
        String nome,
        String login,
        String email,
        boolean ativo,
        PerfilAcesso perfilAcesso,
        Departamento departamento,
        LocalDateTime dataCriacao,
        LocalDateTime dataAtualizacao
) implements AgregadoRaiz {

    public Usuario {
        ValidadorDominio.obrigatorio(id, "id do usuario e obrigatorio");
        nome = ValidadorDominio.textoObrigatorio(nome, "nome do usuario e obrigatorio");
        login = ValidadorDominio.textoObrigatorio(login, "login do usuario e obrigatorio");
        email = ValidadorDominio.textoObrigatorio(email, "email do usuario e obrigatorio");
        ValidadorDominio.obrigatorio(perfilAcesso, "perfilAcesso do usuario e obrigatorio");
        validarAuditoria();
    }
}
