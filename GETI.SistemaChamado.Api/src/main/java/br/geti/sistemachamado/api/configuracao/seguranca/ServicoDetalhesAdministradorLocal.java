package br.geti.sistemachamado.api.configuracao.seguranca;

import br.geti.sistemachamado.dominio.administracao.PerfilUsuario;
import br.geti.sistemachamado.dominio.administracao.TipoAutenticacaoUsuario;
import br.geti.sistemachamado.dominio.administracao.repositorio.UsuarioRepositorio;
import java.util.Locale;
import org.springframework.security.core.userdetails.User;
import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.security.core.userdetails.UserDetailsService;
import org.springframework.security.core.userdetails.UsernameNotFoundException;
import org.springframework.stereotype.Service;

@Service
public class ServicoDetalhesAdministradorLocal implements UserDetailsService {

    private final UsuarioRepositorio usuarioRepositorio;
    private final MapeadorPerfilAcessoParaPermissao mapeadorPerfilAcessoParaPermissao;

    public ServicoDetalhesAdministradorLocal(
            final UsuarioRepositorio usuarioRepositorio,
            final MapeadorPerfilAcessoParaPermissao mapeadorPerfilAcessoParaPermissao
    ) {
        this.usuarioRepositorio = usuarioRepositorio;
        this.mapeadorPerfilAcessoParaPermissao = mapeadorPerfilAcessoParaPermissao;
    }

    @Override
    public UserDetails loadUserByUsername(final String email) throws UsernameNotFoundException {
        final var emailNormalizado = normalizarEmail(email);
        final var usuario = usuarioRepositorio.buscarPorEmail(emailNormalizado)
                .filter(u -> u.tipoAutenticacao() == TipoAutenticacaoUsuario.LOCAL)
                .filter(u -> u.ativo())
                .filter(u -> PerfilUsuario.ADMINISTRADOR.nomePerfilAcesso().equalsIgnoreCase(u.perfilAcesso().nome()))
                .orElseThrow(() -> new UsernameNotFoundException("Administrador local nao encontrado."));

        if (usuario.senhaHash() == null || usuario.senhaHash().isBlank()) {
            throw new UsernameNotFoundException("Administrador local sem senha configurada.");
        }

        return User.withUsername(usuario.email())
                .password(usuario.senhaHash())
                .authorities(mapeadorPerfilAcessoParaPermissao.mapearPermissoes(usuario.perfilAcesso().nome()))
                .accountExpired(false)
                .accountLocked(false)
                .credentialsExpired(false)
                .disabled(false)
                .build();
    }

    private String normalizarEmail(final String email) {
        if (email == null || email.isBlank()) {
            throw new UsernameNotFoundException("Email do administrador local nao informado.");
        }
        return email.trim().toLowerCase(Locale.ROOT);
    }
}
