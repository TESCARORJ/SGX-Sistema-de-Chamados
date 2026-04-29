package br.geti.sistemachamado.infraestrutura.chamado;

import br.geti.sistemachamado.dominio.chamado.servico.GeradorNumeroChamado;
import jakarta.persistence.EntityManager;
import org.springframework.stereotype.Component;

import java.time.LocalDate;

@Component
public class GeradorNumeroChamadoSequencial implements GeradorNumeroChamado {

    private final EntityManager entityManager;

    public GeradorNumeroChamadoSequencial(final EntityManager entityManager) {
        this.entityManager = entityManager;
    }

    @Override
    public String gerarNumero() {
        final var valor = ((Number) entityManager
                .createNativeQuery("SELECT nextval('seq_numero_chamado')")
                .getSingleResult())
                .longValue();

        return "CH-" + LocalDate.now().getYear() + "-" + String.format("%08d", valor);
    }
}
