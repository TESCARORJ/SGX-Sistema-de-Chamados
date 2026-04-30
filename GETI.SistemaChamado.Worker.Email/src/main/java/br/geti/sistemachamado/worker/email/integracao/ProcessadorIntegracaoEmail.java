package br.geti.sistemachamado.worker.email.integracao;

import br.geti.sistemachamado.aplicacao.chamado.email.AberturaChamadoEmailComando;
import br.geti.sistemachamado.aplicacao.chamado.email.AnexoChamadoEmailComando;
import br.geti.sistemachamado.aplicacao.chamado.email.GerenciarChamadoPorEmail;
import br.geti.sistemachamado.aplicacao.chamado.email.RespostaChamadoEmailComando;
import br.geti.sistemachamado.dominio.administracao.CaixaDeEmail;
import br.geti.sistemachamado.dominio.administracao.repositorio.CaixaDeEmailRepositorio;
import br.geti.sistemachamado.dominio.integracaoemail.LogDeIntegracaoEmail;
import br.geti.sistemachamado.dominio.integracaoemail.StatusProcessamentoIntegracaoEmail;
import br.geti.sistemachamado.dominio.integracaoemail.repositorio.LogDeIntegracaoEmailRepositorio;
import java.nio.charset.StandardCharsets;
import java.security.MessageDigest;
import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.HexFormat;
import java.util.LinkedHashSet;
import java.util.List;
import java.util.Locale;
import java.util.Map;
import java.util.Objects;
import java.util.UUID;
import java.util.stream.Collectors;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Service;

@Service
public class ProcessadorIntegracaoEmail {

    private static final Logger LOGGER = LoggerFactory.getLogger(ProcessadorIntegracaoEmail.class);

    private final LeitorMensagensEmail leitorMensagensEmail;
    private final CaixaDeEmailRepositorio caixaDeEmailRepositorio;
    private final LogDeIntegracaoEmailRepositorio logDeIntegracaoEmailRepositorio;
    private final GerenciarChamadoPorEmail gerenciarChamadoPorEmail;
    private final CorrelacionadorRespostaEmail correlacionadorRespostaEmail;
    private final DetectorRespostaAutomaticaEmail detectorRespostaAutomaticaEmail;

    public ProcessadorIntegracaoEmail(
            final LeitorMensagensEmail leitorMensagensEmail,
            final CaixaDeEmailRepositorio caixaDeEmailRepositorio,
            final LogDeIntegracaoEmailRepositorio logDeIntegracaoEmailRepositorio,
            final GerenciarChamadoPorEmail gerenciarChamadoPorEmail,
            final CorrelacionadorRespostaEmail correlacionadorRespostaEmail,
            final DetectorRespostaAutomaticaEmail detectorRespostaAutomaticaEmail
    ) {
        this.leitorMensagensEmail = leitorMensagensEmail;
        this.caixaDeEmailRepositorio = caixaDeEmailRepositorio;
        this.logDeIntegracaoEmailRepositorio = logDeIntegracaoEmailRepositorio;
        this.gerenciarChamadoPorEmail = gerenciarChamadoPorEmail;
        this.correlacionadorRespostaEmail = correlacionadorRespostaEmail;
        this.detectorRespostaAutomaticaEmail = detectorRespostaAutomaticaEmail;
    }

    public void processarCiclo() {
        final var caixasAtivas = caixaDeEmailRepositorio.listarAtivas();
        if (caixasAtivas.isEmpty()) {
            LOGGER.info("Nenhuma caixa de e-mail ativa cadastrada para processamento IMAP.");
            return;
        }

        final Map<String, CaixaDeEmail> caixasPorEndereco = caixasAtivas.stream()
                .collect(Collectors.toMap(
                        caixa -> caixa.enderecoEmail().toLowerCase(Locale.ROOT),
                        caixa -> caixa
                ));

        final var mensagens = leitorMensagensEmail.listarMensagensElegiveis();
        if (mensagens.isEmpty()) {
            LOGGER.debug("Nenhuma mensagem elegivel encontrada no ciclo de processamento.");
            return;
        }

        for (final var mensagem : mensagens) {
            final var caixasRelacionadas = encontrarCaixasRelacionadas(mensagem, caixasPorEndereco);
            if (caixasRelacionadas.isEmpty()) {
                LOGGER.info(
                        "Mensagem sem caixa de destino cadastrada. origem={} messageId={}",
                        mensagem.identificadorOrigem(),
                        mensagem.messageId()
                );
                continue;
            }
            for (final var caixaDeEmail : caixasRelacionadas) {
                processarMensagemDaCaixa(mensagem, caixaDeEmail);
            }
        }
    }

    private List<CaixaDeEmail> encontrarCaixasRelacionadas(
            final MensagemEmailRecebida mensagem,
            final Map<String, CaixaDeEmail> caixasPorEndereco
    ) {
        final var caixas = new LinkedHashSet<CaixaDeEmail>();
        if (mensagem.destinatarios() == null) {
            return List.of();
        }

        for (final var destinatario : mensagem.destinatarios()) {
            if (destinatario == null || destinatario.isBlank()) {
                continue;
            }
            final var caixa = caixasPorEndereco.get(destinatario.trim().toLowerCase(Locale.ROOT));
            if (caixa != null) {
                caixas.add(caixa);
            }
        }
        return List.copyOf(caixas);
    }

    private void processarMensagemDaCaixa(final MensagemEmailRecebida mensagem, final CaixaDeEmail caixaDeEmail) {
        final var chaveDeduplicacaoOriginal = gerarChaveDeduplicacao(mensagem);
        final var duplicado = logDeIntegracaoEmailRepositorio.buscarPorCaixaEChaveDeduplicacao(
                caixaDeEmail.id(),
                chaveDeduplicacaoOriginal
        ).isPresent();

        if (duplicado) {
            final var chaveDuplicado = truncar(chaveDeduplicacaoOriginal + "#duplicado", 600);
            if (logDeIntegracaoEmailRepositorio.buscarPorCaixaEChaveDeduplicacao(caixaDeEmail.id(), chaveDuplicado)
                    .isEmpty()) {
                salvarNovoLog(
                        caixaDeEmail,
                        mensagem,
                        chaveDuplicado,
                        StatusProcessamentoIntegracaoEmail.DUPLICADO,
                        "Mensagem ignorada por duplicidade.",
                        null
                );
            }
            LOGGER.info(
                    "Mensagem duplicada ignorada. caixa={} chave={}",
                    caixaDeEmail.enderecoEmail(),
                    chaveDeduplicacaoOriginal
            );
            return;
        }

        final var logInicial = salvarNovoLog(
                caixaDeEmail,
                mensagem,
                chaveDeduplicacaoOriginal,
                StatusProcessamentoIntegracaoEmail.RECEBIDO,
                "Mensagem recebida e em processamento.",
                null
        );

        try {
            if (mensagem.remetenteEmail() == null || mensagem.remetenteEmail().isBlank()) {
                atualizarLog(
                        logInicial,
                        StatusProcessamentoIntegracaoEmail.FALHA,
                        "Mensagem sem remetente valido. Processamento abortado.",
                        null
                );
                LOGGER.warn(
                        "Mensagem sem remetente valido. caixa={} messageId={} origem={}",
                        caixaDeEmail.enderecoEmail(),
                        mensagem.messageId(),
                        mensagem.identificadorOrigem()
                );
                return;
            }

            if (detectorRespostaAutomaticaEmail.ehRespostaAutomatica(mensagem)) {
                atualizarLog(
                        logInicial,
                        StatusProcessamentoIntegracaoEmail.SUCESSO,
                        "Mensagem automatica identificada e ignorada.",
                        null
                );
                LOGGER.info(
                        "Mensagem automatica ignorada. caixa={} messageId={}",
                        caixaDeEmail.enderecoEmail(),
                        mensagem.messageId()
                );
                return;
            }

            final var correlacao = correlacionadorRespostaEmail.correlacionar(caixaDeEmail.id(), mensagem);
            if (correlacao.isPresent()) {
                final var interacao = gerenciarChamadoPorEmail.registrarRespostaEmChamado(new RespostaChamadoEmailComando(
                        correlacao.get().chamadoId(),
                        mensagem.remetenteNome(),
                        mensagem.remetenteEmail(),
                        mensagem.assunto(),
                        mensagem.corpoMensagem(),
                        mensagem.messageId(),
                        mensagem.inReplyTo(),
                        converterAnexos(mensagem.anexos())
                ));

                atualizarLog(
                        logInicial,
                        StatusProcessamentoIntegracaoEmail.RESPOSTA_CORRELACIONADA,
                        "Resposta correlacionada ao chamado existente.",
                        interacao.chamadoId()
                );
                LOGGER.info(
                        "Resposta de e-mail correlacionada. caixa={} messageId={} chamadoId={} messageIdBase={}",
                        caixaDeEmail.enderecoEmail(),
                        mensagem.messageId(),
                        interacao.chamadoId(),
                        correlacao.get().messageIdCorrelacionado()
                );
                return;
            }

            final var chamado = gerenciarChamadoPorEmail.abrirChamadoPorEmail(new AberturaChamadoEmailComando(
                    caixaDeEmail.id(),
                    mensagem.remetenteNome(),
                    mensagem.remetenteEmail(),
                    caixaDeEmail.enderecoEmail(),
                    mensagem.assunto(),
                    mensagem.corpoMensagem(),
                    mensagem.messageId(),
                    null,
                    converterAnexos(mensagem.anexos())
            ));

            atualizarLog(
                    logInicial,
                    StatusProcessamentoIntegracaoEmail.SUCESSO,
                    "Chamado aberto automaticamente com numero " + chamado.numeroChamado() + ".",
                    chamado.chamadoId()
            );
        } catch (final Exception exception) {
            atualizarLog(
                    logInicial,
                    StatusProcessamentoIntegracaoEmail.FALHA,
                    "Falha ao abrir chamado automaticamente: " + truncar(exception.getMessage(), 1800),
                    null
            );
            LOGGER.error(
                    "Falha ao processar mensagem de e-mail. caixa={} messageId={} erro={}",
                    caixaDeEmail.enderecoEmail(),
                    mensagem.messageId(),
                    exception.getMessage(),
                    exception
            );
        }
    }

    private LogDeIntegracaoEmail salvarNovoLog(
            final CaixaDeEmail caixaDeEmail,
            final MensagemEmailRecebida mensagem,
            final String chaveDeduplicacao,
            final StatusProcessamentoIntegracaoEmail status,
            final String detalhe,
            final UUID chamadoId
    ) {
        final var agora = LocalDateTime.now();
        return logDeIntegracaoEmailRepositorio.salvar(new LogDeIntegracaoEmail(
                UUID.randomUUID(),
                caixaDeEmail.id(),
                truncar(normalizarMessageId(mensagem.messageId()), 500),
                truncar(valorOuPadrao(mensagem.remetenteEmail(), "remetente-nao-informado@local"), 255),
                truncar(caixaDeEmail.enderecoEmail(), 255),
                truncar(valorOuPadrao(mensagem.assunto(), "Sem assunto"), 500),
                status,
                detalhe,
                truncar(chaveDeduplicacao, 600),
                chamadoId,
                agora,
                agora,
                null
        ));
    }

    private void atualizarLog(
            final LogDeIntegracaoEmail logAtual,
            final StatusProcessamentoIntegracaoEmail novoStatus,
            final String detalhe,
            final UUID chamadoId
    ) {
        final var atualizado = new LogDeIntegracaoEmail(
                logAtual.id(),
                logAtual.caixaDeEmailId(),
                logAtual.messageId(),
                logAtual.remetente(),
                logAtual.destinatario(),
                logAtual.assunto(),
                novoStatus,
                detalhe,
                logAtual.chaveDeduplicacao(),
                chamadoId,
                LocalDateTime.now(),
                logAtual.dataCriacao(),
                LocalDateTime.now()
        );
        logDeIntegracaoEmailRepositorio.salvar(atualizado);
    }

    private List<AnexoChamadoEmailComando> converterAnexos(final List<AnexoMensagemEmailRecebida> anexos) {
        if (anexos == null || anexos.isEmpty()) {
            return List.of();
        }

        final var convertidos = new ArrayList<AnexoChamadoEmailComando>();
        for (final var anexo : anexos) {
            if (anexo == null || anexo.conteudo() == null) {
                continue;
            }
            convertidos.add(new AnexoChamadoEmailComando(
                    anexo.nomeArquivo(),
                    anexo.tipoConteudo(),
                    anexo.conteudo()
            ));
        }
        return List.copyOf(convertidos);
    }

    private String gerarChaveDeduplicacao(final MensagemEmailRecebida mensagem) {
        final var messageIdNormalizado = normalizarMessageId(mensagem.messageId());
        if (messageIdNormalizado != null && !messageIdNormalizado.isBlank()) {
            return "MSGID:" + messageIdNormalizado;
        }

        final var base = String.join("|",
                valorOuPadrao(mensagem.remetenteEmail(), ""),
                String.join(",", normalizarDestinatarios(mensagem.destinatarios())),
                valorOuPadrao(mensagem.assunto(), ""),
                valorOuPadrao(mensagem.corpoMensagem(), ""),
                mensagem.dataRecebimento() != null ? mensagem.dataRecebimento().toString() : "",
                valorOuPadrao(mensagem.identificadorOrigem(), "")
        );
        return "SEMMSGID:" + sha256(base);
    }

    private List<String> normalizarDestinatarios(final List<String> destinatarios) {
        if (destinatarios == null || destinatarios.isEmpty()) {
            return List.of();
        }
        return destinatarios.stream()
                .filter(Objects::nonNull)
                .map(valor -> valor.trim().toLowerCase(Locale.ROOT))
                .filter(valor -> !valor.isBlank())
                .sorted()
                .toList();
    }

    private String sha256(final String texto) {
        try {
            final var digest = MessageDigest.getInstance("SHA-256");
            final var bytes = digest.digest(texto.getBytes(StandardCharsets.UTF_8));
            return HexFormat.of().formatHex(bytes);
        } catch (final Exception exception) {
            throw new IllegalStateException("Falha ao calcular hash de deduplicacao.", exception);
        }
    }

    private String normalizarMessageId(final String messageId) {
        if (messageId == null || messageId.isBlank()) {
            return null;
        }
        return messageId.trim().replace("<", "").replace(">", "");
    }

    private String valorOuPadrao(final String valor, final String padrao) {
        if (valor == null || valor.isBlank()) {
            return padrao;
        }
        return valor.trim();
    }

    private String truncar(final String valor, final int tamanhoMaximo) {
        if (valor == null) {
            return null;
        }
        return valor.length() <= tamanhoMaximo ? valor : valor.substring(0, tamanhoMaximo);
    }
}
