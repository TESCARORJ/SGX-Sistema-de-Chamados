package br.geti.sistemachamado.api.seguranca;

import static org.assertj.core.api.Assertions.assertThat;
import static org.assertj.core.api.Assertions.assertThatThrownBy;
import static org.mockito.Mockito.when;

import br.geti.sistemachamado.api.configuracao.seguranca.MapeadorPerfilAcessoParaPermissao;
import br.geti.sistemachamado.api.configuracao.seguranca.ServicoDetalhesAdministradorLocal;
import br.geti.sistemachamado.dominio.administracao.PerfilAcesso;
import br.geti.sistemachamado.dominio.administracao.TipoAutenticacaoUsuario;
import br.geti.sistemachamado.dominio.administracao.Usuario;
import br.geti.sistemachamado.dominio.administracao.repositorio.UsuarioRepositorio;
import java.time.LocalDateTime;
import java.util.Optional;
import java.util.UUID;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;
import org.springframework.security.core.userdetails.UsernameNotFoundException;

@ExtendWith(MockitoExtension.class)
class ServicoDetalhesAdministradorLocalTest {

    @Mock
    private UsuarioRepositorio usuarioRepositorio;

    @Test
    void deveCarregarAdministradorLocalAtivo() {
        final var perfilAdministrador = new PerfilAcesso(
                UUID.randomUUID(),
                "Administrador",
                "Perfil administrador",
                true,
                LocalDateTime.now().minusDays(1),
                null
        );
        final var usuario = new Usuario(
                UUID.randomUUID(),
                "Administrador Local",
                "admin.local@crea-rj.org.br",
                "admin.local@crea-rj.org.br",
                TipoAutenticacaoUsuario.LOCAL,
                "hash-seguro",
                true,
                perfilAdministrador,
                null,
                LocalDateTime.now().minusDays(1),
                null
        );

        when(usuarioRepositorio.buscarPorEmail("admin.local@crea-rj.org.br")).thenReturn(Optional.of(usuario));

        final var servico = new ServicoDetalhesAdministradorLocal(
                usuarioRepositorio,
                new MapeadorPerfilAcessoParaPermissao()
        );

        final var userDetails = servico.loadUserByUsername("admin.local@crea-rj.org.br");
        assertThat(userDetails.getUsername()).isEqualTo("admin.local@crea-rj.org.br");
        assertThat(userDetails.getPassword()).isEqualTo("hash-seguro");
        assertThat(userDetails.getAuthorities()).extracting("authority").contains("ROLE_ADMINISTRADOR");
    }

    @Test
    void deveFalharQuandoUsuarioNaoForAdminLocal() {
        final var perfilAtendente = new PerfilAcesso(
                UUID.randomUUID(),
                "Atendente",
                "Perfil atendente",
                true,
                LocalDateTime.now().minusDays(1),
                null
        );
        final var usuario = new Usuario(
                UUID.randomUUID(),
                "Atendente",
                "atendente@crea-rj.org.br",
                "atendente@crea-rj.org.br",
                TipoAutenticacaoUsuario.CORPORATIVA,
                null,
                true,
                perfilAtendente,
                null,
                LocalDateTime.now().minusDays(1),
                null
        );

        when(usuarioRepositorio.buscarPorEmail("atendente@crea-rj.org.br")).thenReturn(Optional.of(usuario));

        final var servico = new ServicoDetalhesAdministradorLocal(
                usuarioRepositorio,
                new MapeadorPerfilAcessoParaPermissao()
        );

        assertThatThrownBy(() -> servico.loadUserByUsername("atendente@crea-rj.org.br"))
                .isInstanceOf(UsernameNotFoundException.class);
    }
}
