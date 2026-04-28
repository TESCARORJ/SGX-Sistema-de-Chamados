package br.geti.sistemachamado.aplicacao.acesso;

import br.geti.sistemachamado.dominio.compartilhado.ErroDeDominio;
import org.springframework.stereotype.Service;

@Service
public class ResolverContextoUsuarioAutenticadoPadrao implements ResolverContextoUsuarioAutenticado {

    private final SincronizarUsuarioAutenticado sincronizarUsuarioAutenticado;

    public ResolverContextoUsuarioAutenticadoPadrao(
            final SincronizarUsuarioAutenticado sincronizarUsuarioAutenticado
    ) {
        this.sincronizarUsuarioAutenticado = sincronizarUsuarioAutenticado;
    }

    @Override
    public ContextoUsuarioAutenticado resolver(final IdentidadeUsuarioAutenticado identidade) {
        if (identidade == null) {
            throw new ErroDeDominio("Identidade autenticada nao informada.");
        }

        final var sincronizado = sincronizarUsuarioAutenticado.sincronizar(
                new ComandoSincronizacaoUsuarioAutenticado(
                        identidade.nome(),
                        identidade.login(),
                        identidade.email(),
                        null
                )
        );

        return new ContextoUsuarioAutenticado(
                sincronizado.usuarioId(),
                sincronizado.login(),
                sincronizado.nome(),
                sincronizado.email(),
                sincronizado.perfilAcesso(),
                sincronizado.departamentoId()
        );
    }
}
