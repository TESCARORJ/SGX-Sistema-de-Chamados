package br.geti.sistemachamado.infraestrutura.persistencia.entidade;

import br.geti.sistemachamado.dominio.integracaoemail.StatusProcessamentoIntegracaoEmail;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.EnumType;
import jakarta.persistence.Enumerated;
import jakarta.persistence.FetchType;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.ManyToOne;
import jakarta.persistence.Table;
import java.time.LocalDateTime;
import java.util.UUID;

@Entity
@Table(name = "logs_integracao_email")
public class LogDeIntegracaoEmailEntidadeJpa extends EntidadeBaseJpa {

    @ManyToOne(fetch = FetchType.LAZY, optional = false)
    @JoinColumn(name = "caixa_email_id", nullable = false)
    private CaixaDeEmailEntidadeJpa caixaEmail;

    @Column(name = "message_id", length = 500)
    private String messageId;

    @Column(name = "remetente", nullable = false, length = 255)
    private String remetente;

    @Column(name = "destinatario", nullable = false, length = 255)
    private String destinatario;

    @Column(name = "assunto", nullable = false, length = 500)
    private String assunto;

    @Enumerated(EnumType.STRING)
    @Column(name = "status_processamento", nullable = false, length = 30)
    private StatusProcessamentoIntegracaoEmail statusProcessamento;

    @Column(name = "detalhe_processamento", columnDefinition = "TEXT")
    private String detalheProcessamento;

    @Column(name = "chave_deduplicacao", nullable = false, length = 600)
    private String chaveDeduplicacao;

    @Column(name = "chamado_id")
    private UUID chamadoId;

    @Column(name = "data_processamento", nullable = false)
    private LocalDateTime dataProcessamento;

    public CaixaDeEmailEntidadeJpa getCaixaEmail() {
        return caixaEmail;
    }

    public void setCaixaEmail(final CaixaDeEmailEntidadeJpa caixaEmail) {
        this.caixaEmail = caixaEmail;
    }

    public String getMessageId() {
        return messageId;
    }

    public void setMessageId(final String messageId) {
        this.messageId = messageId;
    }

    public String getRemetente() {
        return remetente;
    }

    public void setRemetente(final String remetente) {
        this.remetente = remetente;
    }

    public String getDestinatario() {
        return destinatario;
    }

    public void setDestinatario(final String destinatario) {
        this.destinatario = destinatario;
    }

    public String getAssunto() {
        return assunto;
    }

    public void setAssunto(final String assunto) {
        this.assunto = assunto;
    }

    public StatusProcessamentoIntegracaoEmail getStatusProcessamento() {
        return statusProcessamento;
    }

    public void setStatusProcessamento(final StatusProcessamentoIntegracaoEmail statusProcessamento) {
        this.statusProcessamento = statusProcessamento;
    }

    public String getDetalheProcessamento() {
        return detalheProcessamento;
    }

    public void setDetalheProcessamento(final String detalheProcessamento) {
        this.detalheProcessamento = detalheProcessamento;
    }

    public String getChaveDeduplicacao() {
        return chaveDeduplicacao;
    }

    public void setChaveDeduplicacao(final String chaveDeduplicacao) {
        this.chaveDeduplicacao = chaveDeduplicacao;
    }

    public UUID getChamadoId() {
        return chamadoId;
    }

    public void setChamadoId(final UUID chamadoId) {
        this.chamadoId = chamadoId;
    }

    public LocalDateTime getDataProcessamento() {
        return dataProcessamento;
    }

    public void setDataProcessamento(final LocalDateTime dataProcessamento) {
        this.dataProcessamento = dataProcessamento;
    }
}

