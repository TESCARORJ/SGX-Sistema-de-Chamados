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
        TipoAutenticacaoUsuario tipoAutenticacao,
        String senhaHash,
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
        tipoAutenticacao = ValidadorDominio.obrigatorio(
                tipoAutenticacao,
                "tipoAutenticacao do usuario e obrigatorio"
        );
        senhaHash = normalizarSenhaHash(senhaHash, tipoAutenticacao);
        ValidadorDominio.obrigatorio(perfilAcesso, "perfilAcesso do usuario e obrigatorio");
        validarAuditoria(dataCriacao, dataAtualizacao);
    }

    public Usuario(
            final UUID id,
            final String nome,
            final String login,
            final String email,
            final boolean ativo,
            final PerfilAcesso perfilAcesso,
            final Departamento departamento,
            final LocalDateTime dataCriacao,
            final LocalDateTime dataAtualizacao
    ) {
        this(
                id,
                nome,
                login,
                email,
                TipoAutenticacaoUsuario.CORPORATIVA,
                null,
                ativo,
                perfilAcesso,
                departamento,
                dataCriacao,
                dataAtualizacao
        );
    }

    private static String normalizarSenhaHash(final String senhaHash, final TipoAutenticacaoUsuario tipoAutenticacao) {
        if (senhaHash == null || senhaHash.trim().isEmpty()) {
            if (tipoAutenticacao == TipoAutenticacaoUsuario.LOCAL) {
                throw new br.geti.sistemachamado.dominio.compartilhado.ErroDeDominio(
                        "senhaHash do usuario local e obrigatoria"
                );
            }
            return null;
        }
        return senhaHash.trim();
    }
}

