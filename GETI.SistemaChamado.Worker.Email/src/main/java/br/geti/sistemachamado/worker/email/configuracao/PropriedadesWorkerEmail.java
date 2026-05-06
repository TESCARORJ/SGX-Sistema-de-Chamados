package br.geti.sistemachamado.worker.email.configuracao;

import org.springframework.boot.context.properties.ConfigurationProperties;
import org.springframework.stereotype.Component;

@Component
@ConfigurationProperties(prefix = "app.worker.email")
public class PropriedadesWorkerEmail {

    private long intervaloProcessamentoMs = 60000;
    private boolean modoLocalHabilitado = false;
    private String diretorioArquivosEml = "data/worker-email/eml";

    public long getIntervaloProcessamentoMs() {
        return intervaloProcessamentoMs;
    }

    public void setIntervaloProcessamentoMs(final long intervaloProcessamentoMs) {
        this.intervaloProcessamentoMs = intervaloProcessamentoMs;
    }

    public boolean isModoLocalHabilitado() {
        return modoLocalHabilitado;
    }

    public void setModoLocalHabilitado(final boolean modoLocalHabilitado) {
        this.modoLocalHabilitado = modoLocalHabilitado;
    }

    public String getDiretorioArquivosEml() {
        return diretorioArquivosEml;
    }

    public void setDiretorioArquivosEml(final String diretorioArquivosEml) {
        this.diretorioArquivosEml = diretorioArquivosEml;
    }
}

