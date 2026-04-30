package br.geti.sistemachamado.api.configuracao.seguranca;

import br.geti.sistemachamado.aplicacao.acesso.ComandoProvisionamentoAdministradorLocal;
import br.geti.sistemachamado.aplicacao.acesso.ProvisionarAdministradorLocal;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.boot.ApplicationArguments;
import org.springframework.boot.ApplicationRunner;
import org.springframework.stereotype.Component;

@Component
public class InicializadorAdministradorLocal implements ApplicationRunner {

    private static final Logger LOGGER = LoggerFactory.getLogger(InicializadorAdministradorLocal.class);

    private final ProvisionarAdministradorLocal provisionarAdministradorLocal;
    private final boolean habilitado;
    private final String nome;
    private final String email;
    private final String senhaInicial;

    public InicializadorAdministradorLocal(
            final ProvisionarAdministradorLocal provisionarAdministradorLocal,
            @Value("${app.admin-local.habilitado:true}") final boolean habilitado,
            @Value("${AdminLocal.Nome:${app.admin-local.nome:Administrador Local}}") final String nome,
            @Value("${AdminLocal.Email:${app.admin-local.email:admin.local@crea-rj.org.br}}") final String email,
            @Value("${AdminLocal.SenhaInicial:${app.admin-local.senha-inicial:Alterar@123}}") final String senhaInicial
    ) {
        this.provisionarAdministradorLocal = provisionarAdministradorLocal;
        this.habilitado = habilitado;
        this.nome = nome;
        this.email = email;
        this.senhaInicial = senhaInicial;
    }

    @Override
    public void run(final ApplicationArguments args) {
        if (!habilitado) {
            LOGGER.info("Provisionamento do administrador local desabilitado por configuracao.");
            return;
        }

        final var resultado = provisionarAdministradorLocal.provisionar(
                new ComandoProvisionamentoAdministradorLocal(nome, email, senhaInicial)
        );

        if (resultado.criado()) {
            LOGGER.info(
                    "Administrador local criado com sucesso. usuarioId={} email={}",
                    resultado.usuarioId(),
                    resultado.email()
            );
            return;
        }

        LOGGER.info(
                "Provisionamento do administrador local nao criou novo usuario. motivo={} email={}",
                resultado.motivo(),
                resultado.email()
        );
    }
}
