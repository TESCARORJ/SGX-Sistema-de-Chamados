package br.geti.sistemachamado.api.configuracao.seguranca;

import br.geti.sistemachamado.aplicacao.acesso.IdentidadeUsuarioAutenticado;
import br.geti.sistemachamado.dominio.compartilhado.ErroDeDominio;
import java.util.Locale;
import org.springframework.security.oauth2.jwt.Jwt;
import org.springframework.stereotype.Component;

@Component
public class ExtratorIdentidadeJwt {

    public IdentidadeUsuarioAutenticado extrair(final Jwt jwt) {
        final var login = normalizarIdentificador(
                primeiroPreenchido(
                        jwt.getClaimAsString("preferred_username"),
                        jwt.getClaimAsString("upn"),
                        jwt.getClaimAsString("unique_name"),
                        jwt.getClaimAsString("email"),
                        jwt.getSubject()
                ),
                "login nao identificado no token corporativo"
        );

        final var email = normalizarIdentificador(
                primeiroPreenchido(
                        jwt.getClaimAsString("email"),
                        jwt.getClaimAsString("upn"),
                        jwt.getClaimAsString("preferred_username"),
                        login
                ),
                "email nao identificado no token corporativo"
        );

        final var nome = normalizarTexto(
                primeiroPreenchido(
                        jwt.getClaimAsString("name"),
                        jwt.getClaimAsString("given_name"),
                        login
                ),
                "nome nao identificado no token corporativo"
        );

        return new IdentidadeUsuarioAutenticado(login, nome, email);
    }

    private String primeiroPreenchido(final String... candidatos) {
        for (final var candidato : candidatos) {
            if (candidato != null && !candidato.trim().isEmpty()) {
                return candidato;
            }
        }
        return null;
    }

    private String normalizarTexto(final String valor, final String mensagemErro) {
        if (valor == null || valor.trim().isEmpty()) {
            throw new ErroDeDominio(mensagemErro);
        }
        return valor.trim();
    }

    private String normalizarIdentificador(final String valor, final String mensagemErro) {
        return normalizarTexto(valor, mensagemErro).toLowerCase(Locale.ROOT);
    }
}
