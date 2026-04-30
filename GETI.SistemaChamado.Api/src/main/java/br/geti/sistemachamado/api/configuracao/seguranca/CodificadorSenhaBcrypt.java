package br.geti.sistemachamado.api.configuracao.seguranca;

import br.geti.sistemachamado.aplicacao.acesso.CodificadorSenha;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Component;

@Component
public class CodificadorSenhaBcrypt implements CodificadorSenha {

    private final PasswordEncoder passwordEncoder;

    public CodificadorSenhaBcrypt(final PasswordEncoder passwordEncoder) {
        this.passwordEncoder = passwordEncoder;
    }

    @Override
    public String codificar(final String senhaAberta) {
        return passwordEncoder.encode(senhaAberta);
    }

    @Override
    public boolean corresponde(final String senhaAberta, final String senhaHash) {
        return passwordEncoder.matches(senhaAberta, senhaHash);
    }
}
