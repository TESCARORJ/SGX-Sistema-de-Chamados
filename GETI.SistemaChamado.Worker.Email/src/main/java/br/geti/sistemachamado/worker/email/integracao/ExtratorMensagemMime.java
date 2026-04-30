package br.geti.sistemachamado.worker.email.integracao;

import jakarta.mail.Address;
import jakarta.mail.BodyPart;
import jakarta.mail.Message;
import jakarta.mail.MessagingException;
import jakarta.mail.Part;
import jakarta.mail.internet.InternetAddress;
import jakarta.mail.internet.MimeMessage;
import jakarta.mail.internet.MimeUtility;
import jakarta.mail.Multipart;
import java.io.IOException;
import java.io.InputStream;
import java.time.LocalDateTime;
import java.time.ZoneId;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.LinkedHashSet;
import java.util.List;
import java.util.Locale;
import java.util.Set;
import java.util.regex.Pattern;

final class ExtratorMensagemMime {

    private static final Pattern PADRAO_MESSAGE_ID = Pattern.compile("<([^>]+)>");

    private ExtratorMensagemMime() {
    }

    static MensagemEmailRecebida extrair(
            final MimeMessage mensagem,
            final String identificadorOrigem
    ) throws MessagingException, IOException {
        final var remetente = extrairRemetente(mensagem);
        final var destinatarios = extrairDestinatarios(mensagem);
        final var assunto = mensagem.getSubject();
        final var corpo = extrairCorpoTexto(mensagem);
        final var anexos = new ArrayList<AnexoMensagemEmailRecebida>();
        extrairAnexos(mensagem, anexos);

        final var dataRecebimento = mensagem.getReceivedDate() != null
                ? LocalDateTime.ofInstant(mensagem.getReceivedDate().toInstant(), ZoneId.systemDefault())
                : LocalDateTime.now();

        return new MensagemEmailRecebida(
                identificadorOrigem,
                normalizarMessageId(mensagem.getHeader("Message-Id", null)),
                normalizarMessageId(mensagem.getHeader("In-Reply-To", null)),
                extrairReferences(mensagem),
                normalizarTexto(mensagem.getHeader("Auto-Submitted", null)),
                normalizarTexto(mensagem.getHeader("Precedence", null)),
                remetente.nome(),
                remetente.email(),
                List.copyOf(destinatarios),
                assunto,
                corpo,
                dataRecebimento,
                List.copyOf(anexos)
        );
    }

    private static RemetenteEmail extrairRemetente(final MimeMessage mensagem) throws MessagingException {
        final Address[] remetentes = mensagem.getFrom();
        if (remetentes == null || remetentes.length == 0) {
            final Address[] replyTo = mensagem.getReplyTo();
            if (replyTo != null && replyTo.length > 0) {
                final var remetenteReplyTo = replyTo[0];
                if (remetenteReplyTo instanceof InternetAddress internetAddress) {
                    return new RemetenteEmail(
                            normalizarTexto(internetAddress.getPersonal()),
                            normalizarEmail(internetAddress.getAddress())
                    );
                }
                return new RemetenteEmail(null, normalizarEmail(remetenteReplyTo.toString()));
            }

            final var senderCabecalho = mensagem.getHeader("Sender", null);
            return new RemetenteEmail(null, normalizarEmail(senderCabecalho));
        }

        final var remetente = remetentes[0];
        if (remetente instanceof InternetAddress internetAddress) {
            return new RemetenteEmail(
                    normalizarTexto(internetAddress.getPersonal()),
                    normalizarEmail(internetAddress.getAddress())
            );
        }
        return new RemetenteEmail(null, normalizarEmail(remetente.toString()));
    }

    private static List<String> extrairDestinatarios(final MimeMessage mensagem) throws MessagingException {
        final Set<String> destinatarios = new LinkedHashSet<>();
        adicionarDestinatarios(destinatarios, mensagem.getRecipients(Message.RecipientType.TO));
        adicionarDestinatarios(destinatarios, mensagem.getRecipients(Message.RecipientType.CC));
        adicionarDestinatarios(destinatarios, mensagem.getRecipients(Message.RecipientType.BCC));
        adicionarCabecalhoEndereco(destinatarios, mensagem, "Delivered-To");
        adicionarCabecalhoEndereco(destinatarios, mensagem, "X-Original-To");
        return new ArrayList<>(destinatarios);
    }

    private static List<String> extrairReferences(final MimeMessage mensagem) throws MessagingException {
        final var references = mensagem.getHeader("References", null);
        if (references == null || references.isBlank()) {
            return List.of();
        }

        final var coletados = new LinkedHashSet<String>();
        final var matcher = PADRAO_MESSAGE_ID.matcher(references);
        while (matcher.find()) {
            final var normalizado = normalizarMessageId(matcher.group(1));
            if (normalizado != null) {
                coletados.add(normalizado);
            }
        }

        if (coletados.isEmpty()) {
            Arrays.stream(references.split("\\s+"))
                    .map(ExtratorMensagemMime::normalizarMessageId)
                    .filter(valor -> valor != null && !valor.isBlank())
                    .forEach(coletados::add);
        }

        return List.copyOf(coletados);
    }

    private static void adicionarCabecalhoEndereco(
            final Set<String> destinatarios,
            final MimeMessage mensagem,
            final String cabecalho
    ) throws MessagingException {
        final var valorCabecalho = mensagem.getHeader(cabecalho, null);
        if (valorCabecalho == null || valorCabecalho.isBlank()) {
            return;
        }

        final var normalizado = normalizarEmail(valorCabecalho);
        if (normalizado != null) {
            destinatarios.add(normalizado);
        }
    }

    private static void adicionarDestinatarios(final Set<String> destino, final Address[] origem) {
        if (origem == null || origem.length == 0) {
            return;
        }

        for (final var endereco : origem) {
            if (endereco instanceof InternetAddress internetAddress) {
                final var normalizado = normalizarEmail(internetAddress.getAddress());
                if (normalizado != null) {
                    destino.add(normalizado);
                }
            } else {
                final var normalizado = normalizarEmail(endereco.toString());
                if (normalizado != null) {
                    destino.add(normalizado);
                }
            }
        }
    }

    private static String extrairCorpoTexto(final Part parte) throws MessagingException, IOException {
        if (parte.isMimeType("text/plain")) {
            final var conteudo = parte.getContent();
            return conteudo != null ? conteudo.toString() : null;
        }
        if (parte.isMimeType("text/html")) {
            final var conteudo = parte.getContent();
            if (conteudo == null) {
                return null;
            }
            return removerTagsHtml(conteudo.toString());
        }
        if (parte.isMimeType("multipart/*")) {
            final var multipart = (Multipart) parte.getContent();
            String corpoHtml = null;
            for (int indice = 0; indice < multipart.getCount(); indice++) {
                final BodyPart bodyPart = multipart.getBodyPart(indice);
                if (bodyPart.isMimeType("text/plain")) {
                    final var conteudo = bodyPart.getContent();
                    if (conteudo != null && !conteudo.toString().isBlank()) {
                        return conteudo.toString();
                    }
                } else if (bodyPart.isMimeType("text/html")) {
                    final var conteudo = bodyPart.getContent();
                    if (conteudo != null && !conteudo.toString().isBlank()) {
                        corpoHtml = removerTagsHtml(conteudo.toString());
                    }
                } else if (bodyPart.isMimeType("multipart/*")) {
                    final var corpoMultipart = extrairCorpoTexto(bodyPart);
                    if (corpoMultipart != null && !corpoMultipart.isBlank()) {
                        return corpoMultipart;
                    }
                }
            }
            return corpoHtml;
        }
        return null;
    }

    private static void extrairAnexos(
            final Part parte,
            final List<AnexoMensagemEmailRecebida> anexos
    ) throws MessagingException, IOException {
        if (parte.isMimeType("multipart/*")) {
            final var multipart = (Multipart) parte.getContent();
            for (int indice = 0; indice < multipart.getCount(); indice++) {
                final BodyPart bodyPart = multipart.getBodyPart(indice);
                if (isAnexo(bodyPart)) {
                    anexos.add(new AnexoMensagemEmailRecebida(
                            resolverNomeArquivo(bodyPart.getFileName()),
                            bodyPart.getContentType(),
                            lerConteudo(bodyPart.getInputStream())
                    ));
                } else {
                    extrairAnexos(bodyPart, anexos);
                }
            }
        }
    }

    private static boolean isAnexo(final BodyPart bodyPart) throws MessagingException {
        final var disposition = bodyPart.getDisposition();
        return Part.ATTACHMENT.equalsIgnoreCase(disposition)
                || (Part.INLINE.equalsIgnoreCase(disposition) && bodyPart.getFileName() != null)
                || bodyPart.getFileName() != null;
    }

    private static String resolverNomeArquivo(final String nomeArquivoOriginal) throws IOException {
        if (nomeArquivoOriginal == null || nomeArquivoOriginal.isBlank()) {
            return "anexo-email.bin";
        }
        return MimeUtility.decodeText(nomeArquivoOriginal).trim();
    }

    private static byte[] lerConteudo(final InputStream inputStream) throws IOException {
        return inputStream.readAllBytes();
    }

    private static String removerTagsHtml(final String html) {
        return html
                .replaceAll("(?is)<style[^>]*>.*?</style>", " ")
                .replaceAll("(?is)<script[^>]*>.*?</script>", " ")
                .replaceAll("(?is)<[^>]+>", " ")
                .replace("&nbsp;", " ")
                .replaceAll("\\s+", " ")
                .trim();
    }

    private static String normalizarMessageId(final String messageId) {
        final var normalizado = normalizarTexto(messageId);
        if (normalizado == null) {
            return null;
        }
        return normalizado
                .replace("<", "")
                .replace(">", "")
                .trim();
    }

    private static String normalizarTexto(final String valor) {
        if (valor == null) {
            return null;
        }
        final var normalizado = valor.trim();
        return normalizado.isEmpty() ? null : normalizado;
    }

    private static String normalizarEmail(final String email) {
        final var normalizado = normalizarTexto(email);
        if (normalizado == null) {
            return null;
        }
        return normalizado.toLowerCase(Locale.ROOT);
    }

    private record RemetenteEmail(String nome, String email) {
    }
}
