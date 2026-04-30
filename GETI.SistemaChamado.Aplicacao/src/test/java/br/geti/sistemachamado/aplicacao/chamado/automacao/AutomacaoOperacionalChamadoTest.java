package br.geti.sistemachamado.aplicacao.chamado.automacao;

import static org.assertj.core.api.Assertions.assertThat;
import static org.mockito.Mockito.when;

import br.geti.sistemachamado.dominio.administracao.Departamento;
import br.geti.sistemachamado.dominio.administracao.PerfilAcesso;
import br.geti.sistemachamado.dominio.administracao.Usuario;
import br.geti.sistemachamado.dominio.administracao.repositorio.UsuarioRepositorio;
import java.time.LocalDateTime;
import java.util.List;
import java.util.Optional;
import java.util.UUID;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;

@ExtendWith(MockitoExtension.class)
class AutomacaoOperacionalChamadoTest {

    @Mock
    private UsuarioRepositorio usuarioRepositorio;

    @Test
    void deveAtribuirAutomaticamenteQuandoExisteUnicoAtendenteNoDepartamento() {
        final var departamento = new Departamento(UUID.randomUUID(), "Infra", true, LocalDateTime.now().minusDays(1), null);
        final var atendente = criarUsuario("ATENDENTE", departamento);
        final var solicitanteOutroDepartamento = criarUsuario(
                "SOLICITANTE",
                new Departamento(UUID.randomUUID(), "Financeiro", true, LocalDateTime.now().minusDays(1), null)
        );
        when(usuarioRepositorio.listarAtivos()).thenReturn(List.of(atendente, solicitanteOutroDepartamento));

        final var automacao = new AutomacaoOperacionalChamado(usuarioRepositorio);
        final Optional<ResultadoAtribuicaoAutomaticaChamado> resultado = automacao.resolverAtribuicaoAutomatica(departamento, null);

        assertThat(resultado).isPresent();
        assertThat(resultado.get().responsavel().id()).isEqualTo(atendente.id());
    }

    @Test
    void naoDeveAtribuirAutomaticamenteQuandoExistemMultiplosAtendentesNoMesmoDepartamento() {
        final var departamento = new Departamento(UUID.randomUUID(), "Infra", true, LocalDateTime.now().minusDays(1), null);
        final var atendente1 = criarUsuario("ATENDENTE", departamento);
        final var atendente2 = criarUsuario("OPERADOR", departamento);
        when(usuarioRepositorio.listarAtivos()).thenReturn(List.of(atendente1, atendente2));

        final var automacao = new AutomacaoOperacionalChamado(usuarioRepositorio);
        final var resultado = automacao.resolverAtribuicaoAutomatica(departamento, null);

        assertThat(resultado).isEmpty();
    }

    private Usuario criarUsuario(final String perfilNome, final Departamento departamento) {
        final var agora = LocalDateTime.now().minusDays(2);
        return new Usuario(
                UUID.randomUUID(),
                "Usuario " + perfilNome,
                "usuario." + perfilNome.toLowerCase(),
                perfilNome.toLowerCase() + "@corp.com",
                true,
                new PerfilAcesso(UUID.randomUUID(), perfilNome, null, true, agora, null),
                departamento,
                agora,
                null
        );
    }
}
