package br.geti.sistemachamado.aplicacao.acesso;

import br.geti.sistemachamado.dominio.administracao.Departamento;
import br.geti.sistemachamado.dominio.administracao.PerfilAcesso;
import br.geti.sistemachamado.dominio.administracao.Usuario;
import br.geti.sistemachamado.dominio.administracao.repositorio.DepartamentoRepositorio;
import br.geti.sistemachamado.dominio.administracao.repositorio.PerfilAcessoRepositorio;
import br.geti.sistemachamado.dominio.administracao.repositorio.UsuarioRepositorio;
import br.geti.sistemachamado.dominio.compartilhado.ErroDeDominio;
import java.time.LocalDateTime;
import java.util.Locale;
import java.util.Optional;
import java.util.UUID;
import org.springframework.stereotype.Service;

@Service
public class SincronizarUsuarioAutenticadoPadrao implements SincronizarUsuarioAutenticado {

    private static final String PERFIL_PADRAO_SOLICITANTE = "Solicitante";

    private final UsuarioRepositorio usuarioRepositorio;
    private final PerfilAcessoRepositorio perfilAcessoRepositorio;
    private final DepartamentoRepositorio departamentoRepositorio;

    public SincronizarUsuarioAutenticadoPadrao(
            final UsuarioRepositorio usuarioRepositorio,
            final PerfilAcessoRepositorio perfilAcessoRepositorio,
            final DepartamentoRepositorio departamentoRepositorio
    ) {
        this.usuarioRepositorio = usuarioRepositorio;
        this.perfilAcessoRepositorio = perfilAcessoRepositorio;
        this.departamentoRepositorio = departamentoRepositorio;
    }

    @Override
    public ResultadoSincronizacaoUsuario sincronizar(final ComandoSincronizacaoUsuarioAutenticado comando) {
        final var nomeNormalizado = normalizarTextoObrigatorio(comando.nome(), "nome do usuario autenticado e obrigatorio");
        final var loginNormalizado = normalizarIdentificadorObrigatorio(
                comando.login(),
                "login do usuario autenticado e obrigatorio"
        );
        final var emailNormalizado = normalizarIdentificadorObrigatorio(
                comando.email(),
                "email do usuario autenticado e obrigatorio"
        );

        final var usuarioPorLogin = usuarioRepositorio.buscarPorLogin(loginNormalizado);
        final var usuarioPorEmail = usuarioRepositorio.buscarPorEmail(emailNormalizado);
        validarConflitoIdentidade(usuarioPorLogin, usuarioPorEmail);

        final var departamento = buscarDepartamentoOpcional(comando.departamentoId());

        if (usuarioPorLogin.isPresent() || usuarioPorEmail.isPresent()) {
            final var existente = usuarioPorLogin.orElseGet(usuarioPorEmail::get);
            return atualizarUsuarioExistente(existente, nomeNormalizado, loginNormalizado, emailNormalizado, departamento);
        }

        return criarNovoUsuario(nomeNormalizado, loginNormalizado, emailNormalizado, departamento);
    }

    private ResultadoSincronizacaoUsuario criarNovoUsuario(
            final String nome,
            final String login,
            final String email,
            final Departamento departamento
    ) {
        final var perfilPadrao = perfilAcessoRepositorio.buscarPorNome(PERFIL_PADRAO_SOLICITANTE)
                .orElseThrow(() -> new ErroDeDominio(
                        "Perfil padrao Solicitante nao encontrado para sincronizacao inicial do usuario."
                ));

        final var agora = LocalDateTime.now();
        final var novoUsuario = new Usuario(
                UUID.randomUUID(),
                nome,
                login,
                email,
                true,
                perfilPadrao,
                departamento,
                agora,
                null
        );

        final var salvo = usuarioRepositorio.salvar(novoUsuario);
        return paraResultado(salvo, true);
    }

    private ResultadoSincronizacaoUsuario atualizarUsuarioExistente(
            final Usuario existente,
            final String nome,
            final String login,
            final String email,
            final Departamento departamentoInformado
    ) {
        final Departamento departamentoFinal = departamentoInformado != null
                ? departamentoInformado
                : existente.departamento();

        final PerfilAcesso perfilFinal = existente.perfilAcesso();
        final var atualizado = new Usuario(
                existente.id(),
                nome,
                login,
                email,
                existente.ativo(),
                perfilFinal,
                departamentoFinal,
                existente.dataCriacao(),
                LocalDateTime.now()
        );

        final var salvo = usuarioRepositorio.salvar(atualizado);
        return paraResultado(salvo, false);
    }

    private Departamento buscarDepartamentoOpcional(final UUID departamentoId) {
        if (departamentoId == null) {
            return null;
        }

        return departamentoRepositorio.buscarPorId(departamentoId)
                .orElseThrow(() -> new ErroDeDominio(
                        "Departamento informado nao encontrado para sincronizacao do usuario."
                ));
    }

    private void validarConflitoIdentidade(
            final Optional<Usuario> usuarioPorLogin,
            final Optional<Usuario> usuarioPorEmail
    ) {
        if (usuarioPorLogin.isPresent()
                && usuarioPorEmail.isPresent()
                && !usuarioPorLogin.get().id().equals(usuarioPorEmail.get().id())) {
            throw new ErroDeDominio(
                    "Conflito de identidade: login e email pertencem a usuarios diferentes no sistema."
            );
        }
    }

    private ResultadoSincronizacaoUsuario paraResultado(final Usuario usuario, final boolean criado) {
        return new ResultadoSincronizacaoUsuario(
                usuario.id(),
                usuario.nome(),
                usuario.login(),
                usuario.email(),
                usuario.perfilAcesso().nome(),
                usuario.departamento() != null ? usuario.departamento().id() : null,
                criado
        );
    }

    private String normalizarTextoObrigatorio(final String valor, final String mensagem) {
        if (valor == null || valor.trim().isEmpty()) {
            throw new ErroDeDominio(mensagem);
        }
        return valor.trim();
    }

    private String normalizarIdentificadorObrigatorio(final String valor, final String mensagem) {
        return normalizarTextoObrigatorio(valor, mensagem).toLowerCase(Locale.ROOT);
    }
}
