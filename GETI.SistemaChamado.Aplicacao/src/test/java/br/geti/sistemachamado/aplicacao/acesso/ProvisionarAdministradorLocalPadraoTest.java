package br.geti.sistemachamado.aplicacao.acesso;

import static org.assertj.core.api.Assertions.assertThat;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.never;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;

import br.geti.sistemachamado.dominio.administracao.PerfilAcesso;
import br.geti.sistemachamado.dominio.administracao.TipoAutenticacaoUsuario;
import br.geti.sistemachamado.dominio.administracao.Usuario;
import br.geti.sistemachamado.dominio.administracao.repositorio.PerfilAcessoRepositorio;
import br.geti.sistemachamado.dominio.administracao.repositorio.UsuarioRepositorio;
import java.time.LocalDateTime;
import java.util.Optional;
import java.util.UUID;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.ArgumentCaptor;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;

@ExtendWith(MockitoExtension.class)
class ProvisionarAdministradorLocalPadraoTest {

    @Mock
    private UsuarioRepositorio usuarioRepositorio;
    @Mock
    private PerfilAcessoRepositorio perfilAcessoRepositorio;
    @Mock
    private CodificadorSenha codificadorSenha;

    @Test
    void deveCriarAdministradorLocalQuandoNaoExistirUsuarioComEmail() {
        final var perfilAdministrador = new PerfilAcesso(
                UUID.randomUUID(),
                "Administrador",
                "Perfil administrador",
                true,
                LocalDateTime.now().minusDays(1),
                null
        );

        when(usuarioRepositorio.buscarPorEmail("admin.local@crea-rj.org.br")).thenReturn(Optional.empty());
        when(perfilAcessoRepositorio.buscarPorNome("Administrador")).thenReturn(Optional.of(perfilAdministrador));
        when(codificadorSenha.codificar("Alterar@123")).thenReturn("hash-seguro");
        when(usuarioRepositorio.salvar(any(Usuario.class))).thenAnswer(invocacao -> invocacao.getArgument(0));

        final var servico = new ProvisionarAdministradorLocalPadrao(
                usuarioRepositorio,
                perfilAcessoRepositorio,
                codificadorSenha
        );

        final var resultado = servico.provisionar(new ComandoProvisionamentoAdministradorLocal(
                "Administrador Local",
                "admin.local@crea-rj.org.br",
                "Alterar@123"
        ));

        final var captor = ArgumentCaptor.forClass(Usuario.class);
        verify(usuarioRepositorio).salvar(captor.capture());
        final var salvo = captor.getValue();

        assertThat(resultado.criado()).isTrue();
        assertThat(salvo.tipoAutenticacao()).isEqualTo(TipoAutenticacaoUsuario.LOCAL);
        assertThat(salvo.login()).isEqualTo("admin.local@crea-rj.org.br");
        assertThat(salvo.senhaHash()).isEqualTo("hash-seguro");
        assertThat(salvo.perfilAcesso().nome()).isEqualTo("Administrador");
    }

    @Test
    void naoDeveCriarQuandoAdministradorLocalJaExiste() {
        final var perfilAdministrador = new PerfilAcesso(
                UUID.randomUUID(),
                "Administrador",
                "Perfil administrador",
                true,
                LocalDateTime.now().minusDays(1),
                null
        );
        final var existente = new Usuario(
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

        when(usuarioRepositorio.buscarPorEmail("admin.local@crea-rj.org.br")).thenReturn(Optional.of(existente));

        final var servico = new ProvisionarAdministradorLocalPadrao(
                usuarioRepositorio,
                perfilAcessoRepositorio,
                codificadorSenha
        );

        final var resultado = servico.provisionar(new ComandoProvisionamentoAdministradorLocal(
                "Administrador Local",
                "admin.local@crea-rj.org.br",
                "Alterar@123"
        ));

        assertThat(resultado.criado()).isFalse();
        assertThat(resultado.motivo()).isEqualTo("administrador local ja existente");
        verify(usuarioRepositorio, never()).salvar(any(Usuario.class));
    }
}
