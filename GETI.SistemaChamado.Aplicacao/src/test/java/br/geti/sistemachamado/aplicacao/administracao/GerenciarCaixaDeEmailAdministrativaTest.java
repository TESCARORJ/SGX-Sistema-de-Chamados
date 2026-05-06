package br.geti.sistemachamado.aplicacao.administracao;

import static org.assertj.core.api.Assertions.assertThat;
import static org.assertj.core.api.Assertions.assertThatThrownBy;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.when;

import br.geti.sistemachamado.dominio.administracao.CaixaDeEmail;
import br.geti.sistemachamado.dominio.administracao.Departamento;
import br.geti.sistemachamado.dominio.administracao.repositorio.CaixaDeEmailRepositorio;
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
class GerenciarCaixaDeEmailAdministrativaTest {

    @Mock
    private CaixaDeEmailRepositorio caixaDeEmailRepositorio;
    @Mock
    private DepartamentoRepositorio departamentoRepositorio;

    @Test
    void deveCriarCaixaComEmailNormalizado() {
        final var departamento = new Departamento(UUID.randomUUID(), "TI", true, LocalDateTime.now().minusDays(1), null);
        when(departamentoRepositorio.buscarPorId(departamento.id())).thenReturn(Optional.of(departamento));
        when(caixaDeEmailRepositorio.buscarPorEnderecoEmail("suporte@corp.com")).thenReturn(Optional.empty());
        when(caixaDeEmailRepositorio.salvar(any(CaixaDeEmail.class)))
                .thenAnswer(invocacao -> invocacao.getArgument(0));

        final var servico = new GerenciarCaixaDeEmailAdministrativa(caixaDeEmailRepositorio, departamentoRepositorio);
        final var criado = servico.criar("SUPORTE@CORP.COM", "Suporte TI", departamento.id());

        assertThat(criado.enderecoEmail()).isEqualTo("suporte@corp.com");
        assertThat(criado.ativa()).isTrue();
        assertThat(criado.departamentoId()).isEqualTo(departamento.id());
    }

    @Test
    void deveImpedirCriacaoQuandoEnderecoJaExiste() {
        final var departamento = new Departamento(UUID.randomUUID(), "TI", true, LocalDateTime.now().minusDays(1), null);
        when(departamentoRepositorio.buscarPorId(departamento.id())).thenReturn(Optional.of(departamento));
        when(caixaDeEmailRepositorio.buscarPorEnderecoEmail("suporte@corp.com"))
                .thenReturn(Optional.of(new CaixaDeEmail(
                        UUID.randomUUID(),
                        "suporte@corp.com",
                        "Suporte",
                        true,
                        departamento,
                        LocalDateTime.now().minusDays(1),
                        null
                )));

        final var servico = new GerenciarCaixaDeEmailAdministrativa(caixaDeEmailRepositorio, departamentoRepositorio);

        assertThatThrownBy(() -> servico.criar("suporte@corp.com", "Suporte TI", departamento.id()))
                .isInstanceOf(ErroDeDominio.class)
                .hasMessageContaining("Ja existe caixa de email");
    }
}
