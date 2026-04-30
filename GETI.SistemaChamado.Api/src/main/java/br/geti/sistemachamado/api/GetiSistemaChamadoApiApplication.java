package br.geti.sistemachamado.api;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.data.jpa.repository.config.EnableJpaRepositories;
import org.springframework.scheduling.annotation.EnableAsync;
import br.geti.sistemachamado.infraestrutura.configuracao.PacotesJpa;

@SpringBootApplication(scanBasePackages = "br.geti.sistemachamado")
@EnableAsync
@EnableJpaRepositories(basePackages = PacotesJpa.REPOSITORIOS)
public class GetiSistemaChamadoApiApplication {

    public static void main(final String[] args) {
        SpringApplication.run(GetiSistemaChamadoApiApplication.class, args);
    }
}

