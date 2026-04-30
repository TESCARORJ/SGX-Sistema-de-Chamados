package br.geti.sistemachamado.aplicacao.chamado.sla;

import br.geti.sistemachamado.dominio.chamado.Chamado;
import br.geti.sistemachamado.dominio.chamado.PrioridadeChamado;
import br.geti.sistemachamado.dominio.chamado.SituacaoChamado;
import br.geti.sistemachamado.dominio.chamado.StatusSlaChamado;
import br.geti.sistemachamado.dominio.compartilhado.ValidadorDominio;
import java.time.LocalDateTime;
import java.time.temporal.ChronoUnit;
import org.springframework.stereotype.Service;

@Service
public class CalculadoraSlaChamado {

    public int calcularPrazoInicialMinutos(final PrioridadeChamado prioridade) {
        final var prioridadeAplicada = ValidadorDominio.obrigatorio(prioridade, "prioridade do chamado e obrigatoria");
        return switch (prioridadeAplicada) {
            case CRITICA -> 240;
            case ALTA -> 480;
            case MEDIA -> 1440;
            case BAIXA -> 4320;
        };
    }

    public LocalDateTime calcularDataLimite(
            final LocalDateTime dataCriacao,
            final int prazoSlaMinutos
    ) {
        final var dataCriacaoAplicada = ValidadorDominio.obrigatorio(dataCriacao, "data de criacao e obrigatoria");
        if (prazoSlaMinutos <= 0) {
            throw new br.geti.sistemachamado.dominio.compartilhado.ErroDeDominio(
                    "prazo inicial de sla deve ser maior que zero"
            );
        }
        return dataCriacaoAplicada.plusMinutes(prazoSlaMinutos);
    }

    public SlaChamadoCalculado calcular(
            final Chamado chamado,
            final LocalDateTime referencia
    ) {
        final var chamadoAplicado = ValidadorDominio.obrigatorio(chamado, "chamado e obrigatorio");
        final var referenciaAplicada = ValidadorDominio.obrigatorio(referencia, "referencia de calculo e obrigatoria");
        final var momentoAvaliacao = resolverMomentoAvaliacao(chamadoAplicado, referenciaAplicada);

        final var minutosRestantes = ChronoUnit.MINUTES.between(momentoAvaliacao, chamadoAplicado.dataLimiteSla());
        final var minutosAtraso = Math.max(0, ChronoUnit.MINUTES.between(chamadoAplicado.dataLimiteSla(), momentoAvaliacao));
        final var statusSla = resolverStatus(chamadoAplicado, minutosRestantes, minutosAtraso);

        return new SlaChamadoCalculado(
                chamadoAplicado.prazoSlaMinutos(),
                chamadoAplicado.dataLimiteSla(),
                minutosRestantes,
                minutosAtraso,
                statusSla
        );
    }

    private StatusSlaChamado resolverStatus(
            final Chamado chamado,
            final long minutosRestantes,
            final long minutosAtraso
    ) {
        if (minutosAtraso > 0) {
            return StatusSlaChamado.VENCIDO;
        }

        final var limiarProximo = resolverLimiarProximoVencimentoMinutos(chamado.prioridade());
        if (minutosRestantes <= limiarProximo) {
            return StatusSlaChamado.PROXIMO_DO_VENCIMENTO;
        }

        return StatusSlaChamado.DENTRO_DO_PRAZO;
    }

    private long resolverLimiarProximoVencimentoMinutos(final PrioridadeChamado prioridade) {
        return switch (prioridade) {
            case CRITICA -> 60;
            case ALTA -> 120;
            case MEDIA -> 360;
            case BAIXA -> 720;
        };
    }

    private LocalDateTime resolverMomentoAvaliacao(
            final Chamado chamado,
            final LocalDateTime referencia
    ) {
        final boolean chamadoEncerrado = chamado.situacao() == SituacaoChamado.RESOLVIDO
                || chamado.situacao() == SituacaoChamado.CANCELADO;
        if (!chamadoEncerrado || chamado.dataAtualizacao() == null) {
            return referencia;
        }
        return chamado.dataAtualizacao().isBefore(referencia) ? chamado.dataAtualizacao() : referencia;
    }
}
