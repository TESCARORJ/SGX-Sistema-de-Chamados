package br.geti.sistemachamado.infraestrutura.configuracao;

import br.geti.sistemachamado.aplicacao.saude.ConsultaSaudeSistema;
import br.geti.sistemachamado.aplicacao.saude.ConsultarSaudeSistemaPadrao;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

@Configuration
public class ConfiguracaoAplicacao {

    @Bean
    public ConsultaSaudeSistema consultaSaudeSistema(
            @Value("${spring.application.name}") final String nomeServico,
            @Value("${app.ambiente}") final String ambiente
    ) {
        return new ConsultarSaudeSistemaPadrao(nomeServico, ambiente);
    }
}

