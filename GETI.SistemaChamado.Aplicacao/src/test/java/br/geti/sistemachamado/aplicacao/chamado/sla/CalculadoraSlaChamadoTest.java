package br.geti.sistemachamado.aplicacao.chamado.sla;

import static org.assertj.core.api.Assertions.assertThat;

import br.geti.sistemachamado.dominio.administracao.Categoria;
import br.geti.sistemachamado.dominio.administracao.Departamento;
import br.geti.sistemachamado.dominio.administracao.PerfilAcesso;
import br.geti.sistemachamado.dominio.administracao.Servico;
import br.geti.sistemachamado.dominio.administracao.Usuario;
import br.geti.sistemachamado.dominio.chamado.Chamado;
import br.geti.sistemachamado.dominio.chamado.OrigemChamado;
import br.geti.sistemachamado.dominio.chamado.PrioridadeChamado;
import br.geti.sistemachamado.dominio.chamado.SituacaoChamado;
import br.geti.sistemachamado.dominio.chamado.StatusSlaChamado;
import java.time.LocalDateTime;
import java.util.UUID;
import org.junit.jupiter.api.Test;

class CalculadoraSlaChamadoTest {

    private final CalculadoraSlaChamado calculadora = new CalculadoraSlaChamado();

    @Test
    void deveClassificarComoDentroDoPrazo() {
        final var chamado = criarChamado(LocalDateTime.now().minusHours(1), LocalDateTime.now().plusHours(3), SituacaoChamado.ABERTO);

        final var calculado = calculadora.calcular(chamado, LocalDateTime.now());

        assertThat(calculado.statusSla()).isEqualTo(StatusSlaChamado.DENTRO_DO_PRAZO);
        assertThat(calculado.minutosAtraso()).isZero();
    }

    @Test
    void deveClassificarComoProximoDoVencimento() {
        final var agora = LocalDateTime.now();
        final var chamado = criarChamado(agora.minusHours(7), agora.plusMinutes(90), SituacaoChamado.EM_ATENDIMENTO);

        final var calculado = calculadora.calcular(chamado, agora);

        assertThat(calculado.statusSla()).isEqualTo(StatusSlaChamado.PROXIMO_DO_VENCIMENTO);
        assertThat(calculado.minutosRestantes()).isLessThanOrEqualTo(120);
    }

    @Test
    void deveClassificarComoVencidoComAtraso() {
        final var agora = LocalDateTime.now();
        final var chamado = criarChamado(agora.minusHours(12), agora.minusMinutes(30), SituacaoChamado.ABERTO);

        final var calculado = calculadora.calcular(chamado, agora);

        assertThat(calculado.statusSla()).isEqualTo(StatusSlaChamado.VENCIDO);
        assertThat(calculado.minutosAtraso()).isGreaterThanOrEqualTo(30);
    }

    private Chamado criarChamado(
            final LocalDateTime dataCriacao,
            final LocalDateTime dataLimiteSla,
            final SituacaoChamado situacao
    ) {
        final var departamento = new Departamento(UUID.randomUUID(), "TI", true, dataCriacao.minusDays(1), null);
        final var categoria = new Categoria(UUID.randomUUID(), "Suporte", null, true, dataCriacao.minusDays(1), null);
        final var servico = new Servico(UUID.randomUUID(), "Atendimento", null, true, categoria, departamento, dataCriacao.minusDays(1), null);
        final var perfil = new PerfilAcesso(UUID.randomUUID(), "SOLICITANTE", null, true, dataCriacao.minusDays(1), null);
        final var solicitante = new Usuario(
                UUID.randomUUID(),
                "Usuario Solicitante",
                "usuario.solicitante",
                "solicitante@corp.com",
                true,
                perfil,
                departamento,
                dataCriacao.minusDays(1),
                null
        );

        return new Chamado(
                UUID.randomUUID(),
                "CH-TESTE",
                "Chamado teste SLA",
                "Descricao do chamado teste",
                situacao,
                PrioridadeChamado.ALTA,
                OrigemChamado.PORTAL,
                solicitante,
                null,
                departamento,
                categoria,
                servico,
                480,
                dataLimiteSla,
                dataCriacao,
                dataCriacao.plusMinutes(10)
        );
    }
}
