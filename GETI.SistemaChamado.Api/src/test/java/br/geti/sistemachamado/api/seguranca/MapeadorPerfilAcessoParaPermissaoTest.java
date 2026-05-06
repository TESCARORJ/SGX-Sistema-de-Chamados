package br.geti.sistemachamado.api.seguranca;

import static org.assertj.core.api.Assertions.assertThat;

import br.geti.sistemachamado.api.configuracao.seguranca.MapeadorPerfilAcessoParaPermissao;
import org.junit.jupiter.api.Test;

class MapeadorPerfilAcessoParaPermissaoTest {

    private final MapeadorPerfilAcessoParaPermissao mapeador = new MapeadorPerfilAcessoParaPermissao();

    @Test
    void deveMapearPerfilAdministrador() {
        final var authorities = mapeador.mapearPermissoes("administrador");

        assertThat(authorities)
                .extracting(authority -> authority.getAuthority())
                .containsExactly("ROLE_ADMINISTRADOR");
    }

    @Test
    void deveRetornarVazioParaPerfilDesconhecido() {
        final var authorities = mapeador.mapearPermissoes("perfil-invalido");

        assertThat(authorities).isEmpty();
    }
}
