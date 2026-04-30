package br.geti.sistemachamado.worker.email.configuracao;

import org.springframework.boot.context.properties.ConfigurationProperties;
import org.springframework.stereotype.Component;

@Component
@ConfigurationProperties(prefix = "app.worker.email.imap")
public class PropriedadesIntegracaoImap {

    private String host;
    private int porta = 993;
    private String usuario;
    private String senha;
    private String pasta = "INBOX";
    private boolean sslHabilitado = true;
    private boolean tlsHabilitado = true;
    private int timeoutMillis = 30000;
    private int connectTimeoutMillis = 30000;
    private int maxMensagensPorCiclo = 50;

    public String getHost() {
        return host;
    }

    public void setHost(final String host) {
        this.host = host;
    }

    public int getPorta() {
        return porta;
    }

    public void setPorta(final int porta) {
        this.porta = porta;
    }

    public String getUsuario() {
        return usuario;
    }

    public void setUsuario(final String usuario) {
        this.usuario = usuario;
    }

    public String getSenha() {
        return senha;
    }

    public void setSenha(final String senha) {
        this.senha = senha;
    }

    public String getPasta() {
        return pasta;
    }

    public void setPasta(final String pasta) {
        this.pasta = pasta;
    }

    public boolean isSslHabilitado() {
        return sslHabilitado;
    }

    public void setSslHabilitado(final boolean sslHabilitado) {
        this.sslHabilitado = sslHabilitado;
    }

    public boolean isTlsHabilitado() {
        return tlsHabilitado;
    }

    public void setTlsHabilitado(final boolean tlsHabilitado) {
        this.tlsHabilitado = tlsHabilitado;
    }

    public int getTimeoutMillis() {
        return timeoutMillis;
    }

    public void setTimeoutMillis(final int timeoutMillis) {
        this.timeoutMillis = timeoutMillis;
    }

    public int getConnectTimeoutMillis() {
        return connectTimeoutMillis;
    }

    public void setConnectTimeoutMillis(final int connectTimeoutMillis) {
        this.connectTimeoutMillis = connectTimeoutMillis;
    }

    public int getMaxMensagensPorCiclo() {
        return maxMensagensPorCiclo;
    }

    public void setMaxMensagensPorCiclo(final int maxMensagensPorCiclo) {
        this.maxMensagensPorCiclo = maxMensagensPorCiclo;
    }
}

