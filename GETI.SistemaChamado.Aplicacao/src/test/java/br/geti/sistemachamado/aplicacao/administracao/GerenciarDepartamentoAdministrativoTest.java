package br.geti.sistemachamado.aplicacao.administracao;

import static org.assertj.core.api.Assertions.assertThat;
import static org.assertj.core.api.Assertions.assertThatThrownBy;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.when;

import br.geti.sistemachamado.dominio.administracao.Departamento;
import br.geti.sistemachamado.dominio.administracao.repositorio.DepartamentoRepositorio;
import br.geti.sistemachamado.dominio.compartilhado.ErroDeDominio;
import java.time.LocalDateTime;
import java.util.Optional;
import java.util.UUID;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;

@ExtendWith(MockitoExtension.class)
class GerenciarDepartamentoAdministrativoTest {

    @Mock
    private DepartamentoRepositorio departamentoRepositorio;

    @Test
    void deveCriarDepartamentoAtivoQuandoNomeDisponivel() {
        final var agora = LocalDateTime.now();
        when(departamentoRepositorio.buscarPorNome("Suporte N1")).thenReturn(Optional.empty());
        when(departamentoRepositorio.salvar(any(Departamento.class)))
                .thenAnswer(invocacao -> invocacao.getArgument(0));

        final var servico = new GerenciarDepartamentoAdministrativo(departamentoRepositorio);
        final var criado = servico.criar("Suporte N1");

        assertThat(criado.nome()).isEqualTo("Suporte N1");
        assertThat(criado.ativo()).isTrue();
        assertThat(criado.id()).isNotNull();
        assertThat(criado.dataCriacao()).isAfterOrEqualTo(agora.minusSeconds(2));
    }

    @Test
    void deveImpedirCriacaoQuandoNomeJaExiste() {
        when(departamentoRepositorio.buscarPorNome("Infraestrutura"))
                .thenReturn(Optional.of(new Departamento(
                        UUID.randomUUID(),
                        "Infraestrutura",
                        true,
                        LocalDateTime.now().minusDays(1),
                        null
                )));

        final var servico = new GerenciarDepartamentoAdministrativo(departamentoRepositorio);

        assertThatThrownBy(() -> servico.criar("Infraestrutura"))
                .isInstanceOf(ErroDeDominio.class)
                .hasMessageContaining("Ja existe departamento");
    }
}
