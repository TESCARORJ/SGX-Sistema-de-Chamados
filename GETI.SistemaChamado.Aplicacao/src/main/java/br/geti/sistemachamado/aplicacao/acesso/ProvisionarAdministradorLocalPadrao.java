package br.geti.sistemachamado.aplicacao.acesso;

import br.geti.sistemachamado.dominio.administracao.PerfilUsuario;
import br.geti.sistemachamado.dominio.administracao.TipoAutenticacaoUsuario;
import br.geti.sistemachamado.dominio.administracao.Usuario;
import br.geti.sistemachamado.dominio.administracao.repositorio.PerfilAcessoRepositorio;
import br.geti.sistemachamado.dominio.administracao.repositorio.UsuarioRepositorio;
import br.geti.sistemachamado.dominio.compartilhado.ErroDeDominio;
import br.geti.sistemachamado.dominio.compartilhado.ValidadorDominio;
import java.time.LocalDateTime;
import java.util.Locale;
import java.util.UUID;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

@Service
@Transactional
public class ProvisionarAdministradorLocalPadrao implements ProvisionarAdministradorLocal {

    private final UsuarioRepositorio usuarioRepositorio;
    private final PerfilAcessoRepositorio perfilAcessoRepositorio;
    private final CodificadorSenha codificadorSenha;

    public ProvisionarAdministradorLocalPadrao(
            final UsuarioRepositorio usuarioRepositorio,
            final PerfilAcessoRepositorio perfilAcessoRepositorio,
            final CodificadorSenha codificadorSenha
    ) {
        this.usuarioRepositorio = usuarioRepositorio;
        this.perfilAcessoRepositorio = perfilAcessoRepositorio;
        this.codificadorSenha = codificadorSenha;
    }

    @Override
    public ResultadoProvisionamentoAdministradorLocal provisionar(final ComandoProvisionamentoAdministradorLocal comando) {
        ValidadorDominio.obrigatorio(comando, "comando do provisionamento do administrador local e obrigatorio");
        final var nome = ValidadorDominio.textoObrigatorio(
                comando.nome(),
                "nome do administrador local e obrigatorio"
        );
        final var email = normalizarIdentificadorObrigatorio(
                comando.email(),
                "email do administrador local e obrigatorio"
        );
        final var senhaInicial = ValidadorDominio.textoObrigatorio(
                comando.senhaInicial(),
                "senha inicial do administrador local e obrigatoria"
        );

        final var usuarioExistente = usuarioRepositorio.buscarPorEmail(email);
        if (usuarioExistente.isPresent()) {
            final var existente = usuarioExistente.get();
            final var ehAdminLocal = existente.tipoAutenticacao() == TipoAutenticacaoUsuario.LOCAL
                    && PerfilUsuario.ADMINISTRADOR.nomePerfilAcesso().equalsIgnoreCase(existente.perfilAcesso().nome());

            if (ehAdminLocal) {
                return new ResultadoProvisionamentoAdministradorLocal(
                        false,
                        existente.id(),
                        existente.email(),
                        "administrador local ja existente"
                );
            }

            return new ResultadoProvisionamentoAdministradorLocal(
                    false,
                    existente.id(),
                    existente.email(),
                    "email ja utilizado por outro usuario"
            );
        }

        final var perfilAdministrador = perfilAcessoRepositorio
                .buscarPorNome(PerfilUsuario.ADMINISTRADOR.nomePerfilAcesso())
                .orElseThrow(() -> new ErroDeDominio("Perfil Administrador nao encontrado para seed do admin local."));

        final var agora = LocalDateTime.now();
        final var usuario = new Usuario(
                UUID.randomUUID(),
                nome,
                email,
                email,
                TipoAutenticacaoUsuario.LOCAL,
                codificadorSenha.codificar(senhaInicial),
                true,
                perfilAdministrador,
                null,
                agora,
                null
        );

        final var salvo = usuarioRepositorio.salvar(usuario);
        return new ResultadoProvisionamentoAdministradorLocal(
                true,
                salvo.id(),
                salvo.email(),
                "administrador local criado"
        );
    }

    private String normalizarIdentificadorObrigatorio(final String valor, final String mensagem) {
        final var normalizado = ValidadorDominio.textoObrigatorio(valor, mensagem);
        return normalizado.toLowerCase(Locale.ROOT);
    }
}
