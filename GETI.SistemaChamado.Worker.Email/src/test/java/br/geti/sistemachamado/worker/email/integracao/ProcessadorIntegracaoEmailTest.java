package br.geti.sistemachamado.worker.email.integracao;

import static org.assertj.core.api.Assertions.assertThat;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.ArgumentMatchers.anyString;
import static org.mockito.ArgumentMatchers.eq;
import static org.mockito.Mockito.never;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;

import br.geti.sistemachamado.aplicacao.chamado.email.ChamadoAbertoPorEmailDto;
import br.geti.sistemachamado.aplicacao.chamado.email.GerenciarChamadoPorEmail;
import br.geti.sistemachamado.aplicacao.chamado.email.InteracaoChamadoPorEmailDto;
import br.geti.sistemachamado.dominio.administracao.CaixaDeEmail;
import br.geti.sistemachamado.dominio.administracao.Departamento;
import br.geti.sistemachamado.dominio.administracao.repositorio.CaixaDeEmailRepositorio;
import br.geti.sistemachamado.dominio.integracaoemail.LogDeIntegracaoEmail;
import br.geti.sistemachamado.dominio.integracaoemail.StatusProcessamentoIntegracaoEmail;
import br.geti.sistemachamado.dominio.integracaoemail.repositorio.LogDeIntegracaoEmailRepositorio;
import java.time.LocalDateTime;
import java.util.List;
import java.util.Optional;
import java.util.UUID;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.ArgumentCaptor;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;

@ExtendWith(MockitoExtension.class)
class ProcessadorIntegracaoEmailTest {

    @Mock
    private LeitorMensagensEmail leitorMensagensEmail;
    @Mock
    private CaixaDeEmailRepositorio caixaDeEmailRepositorio;
    @Mock
    private LogDeIntegracaoEmailRepositorio logDeIntegracaoEmailRepositorio;
    @Mock
    private GerenciarChamadoPorEmail gerenciarChamadoPorEmail;
    @Mock
    private CorrelacionadorRespostaEmail correlacionadorRespostaEmail;
    @Mock
    private DetectorRespostaAutomaticaEmail detectorRespostaAutomaticaEmail;

    private ProcessadorIntegracaoEmail processador;

    @BeforeEach
    void configurar() {
        processador = new ProcessadorIntegracaoEmail(
                leitorMensagensEmail,
                caixaDeEmailRepositorio,
                logDeIntegracaoEmailRepositorio,
                gerenciarChamadoPorEmail,
                correlacionadorRespostaEmail,
                detectorRespostaAutomaticaEmail
        );
        when(logDeIntegracaoEmailRepositorio.salvar(any(LogDeIntegracaoEmail.class)))
                .thenAnswer(invocacao -> invocacao.getArgument(0));
    }

    @Test
    void deveRegistrarInteracaoQuandoReplyForCorrelacionado() {
        final var caixa = criarCaixa("suporte@corp.com");
        final var chamadoId = UUID.randomUUID();
        final var mensagem = criarMensagem("<nova@corp>", "<origem@corp>");

        when(caixaDeEmailRepositorio.listarAtivas()).thenReturn(List.of(caixa));
        when(leitorMensagensEmail.listarMensagensElegiveis()).thenReturn(List.of(mensagem));
        when(logDeIntegracaoEmailRepositorio.buscarPorCaixaEChaveDeduplicacao(eq(caixa.id()), anyString()))
                .thenReturn(Optional.empty());
        when(detectorRespostaAutomaticaEmail.ehRespostaAutomatica(mensagem)).thenReturn(false);
        when(correlacionadorRespostaEmail.correlacionar(caixa.id(), mensagem))
                .thenReturn(Optional.of(new CorrelacaoRespostaEmail(chamadoId, "origem@corp")));
        when(gerenciarChamadoPorEmail.registrarRespostaEmChamado(any()))
                .thenReturn(new InteracaoChamadoPorEmailDto(chamadoId, UUID.randomUUID()));

        processador.processarCiclo();

        verify(gerenciarChamadoPorEmail).registrarRespostaEmChamado(any());
        verify(gerenciarChamadoPorEmail, never()).abrirChamadoPorEmail(any());

        final var captor = ArgumentCaptor.forClass(LogDeIntegracaoEmail.class);
        verify(logDeIntegracaoEmailRepositorio, org.mockito.Mockito.atLeast(2)).salvar(captor.capture());
        final var ultimoLog = captor.getAllValues().getLast();
        assertThat(ultimoLog.statusProcessamento()).isEqualTo(StatusProcessamentoIntegracaoEmail.RESPOSTA_CORRELACIONADA);
        assertThat(ultimoLog.chamadoId()).isEqualTo(chamadoId);
    }

    @Test
    void deveAbrirNovoChamadoQuandoNaoHouverCorrelacaoSegura() {
        final var caixa = criarCaixa("suporte@corp.com");
        final var chamadoId = UUID.randomUUID();
        final var mensagem = criarMensagem("<nova2@corp>", null);

        when(caixaDeEmailRepositorio.listarAtivas()).thenReturn(List.of(caixa));
        when(leitorMensagensEmail.listarMensagensElegiveis()).thenReturn(List.of(mensagem));
        when(logDeIntegracaoEmailRepositorio.buscarPorCaixaEChaveDeduplicacao(eq(caixa.id()), anyString()))
                .thenReturn(Optional.empty());
        when(detectorRespostaAutomaticaEmail.ehRespostaAutomatica(mensagem)).thenReturn(false);
        when(correlacionadorRespostaEmail.correlacionar(caixa.id(), mensagem)).thenReturn(Optional.empty());
        when(gerenciarChamadoPorEmail.abrirChamadoPorEmail(any()))
                .thenReturn(new ChamadoAbertoPorEmailDto(chamadoId, "CH-2026-1"));

        processador.processarCiclo();

        verify(gerenciarChamadoPorEmail).abrirChamadoPorEmail(any());
        verify(gerenciarChamadoPorEmail, never()).registrarRespostaEmChamado(any());
    }

    private CaixaDeEmail criarCaixa(final String endereco) {
        final var agora = LocalDateTime.now();
        return new CaixaDeEmail(
                UUID.randomUUID(),
                endereco,
                "Caixa de suporte",
                true,
                new Departamento(UUID.randomUUID(), "TI", true, agora, null),
                agora,
                null
        );
    }

    private MensagemEmailRecebida criarMensagem(final String messageId, final String inReplyTo) {
        return new MensagemEmailRecebida(
                "imap:inbox:1",
                messageId,
                inReplyTo,
                List.of(),
                null,
                null,
                "Solicitante",
                "solicitante@corp.com",
                List.of("suporte@corp.com"),
                "Re: Chamado 123",
                "Texto da resposta",
                LocalDateTime.now(),
                List.of(new AnexoMensagemEmailRecebida("evidencia.txt", "text/plain", "ok".getBytes()))
        );
    }
}
