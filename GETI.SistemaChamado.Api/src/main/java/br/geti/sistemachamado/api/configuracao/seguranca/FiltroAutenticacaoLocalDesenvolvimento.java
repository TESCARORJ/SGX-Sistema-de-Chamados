package br.geti.sistemachamado.api.configuracao.seguranca;

import br.geti.sistemachamado.aplicacao.acesso.IdentidadeUsuarioAutenticado;
import br.geti.sistemachamado.aplicacao.acesso.ResolverContextoUsuarioAutenticado;
import jakarta.servlet.FilterChain;
import jakarta.servlet.ServletException;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import java.io.IOException;
import java.util.Locale;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.stereotype.Component;
import org.springframework.web.filter.OncePerRequestFilter;

@Component
public class FiltroAutenticacaoLocalDesenvolvimento extends OncePerRequestFilter {

    private final ResolverContextoUsuarioAutenticado resolverContextoUsuarioAutenticado;
    private final MapeadorPerfilAcessoParaPermissao mapeadorPerfilAcessoParaPermissao;
    private final String headerLogin;
    private final String headerNome;
    private final String headerEmail;

    public FiltroAutenticacaoLocalDesenvolvimento(
            final ResolverContextoUsuarioAutenticado resolverContextoUsuarioAutenticado,
            final MapeadorPerfilAcessoParaPermissao mapeadorPerfilAcessoParaPermissao,
            @Value("${app.seguranca.local.header-login:X-Auth-Login}") final String headerLogin,
            @Value("${app.seguranca.local.header-nome:X-Auth-Nome}") final String headerNome,
            @Value("${app.seguranca.local.header-email:X-Auth-Email}") final String headerEmail
    ) {
        this.resolverContextoUsuarioAutenticado = resolverContextoUsuarioAutenticado;
        this.mapeadorPerfilAcessoParaPermissao = mapeadorPerfilAcessoParaPermissao;
        this.headerLogin = headerLogin;
        this.headerNome = headerNome;
        this.headerEmail = headerEmail;
    }

    @Override
    protected void doFilterInternal(
            final HttpServletRequest request,
            final HttpServletResponse response,
            final FilterChain filterChain
    ) throws ServletException, IOException {
        if (SecurityContextHolder.getContext().getAuthentication() == null) {
            final var loginInformado = request.getHeader(headerLogin);
            if (loginInformado != null && !loginInformado.trim().isEmpty()) {
                final var login = loginInformado.trim().toLowerCase(Locale.ROOT);
                final var nome = normalizarTextoOpcional(request.getHeader(headerNome), login);
                final var email = normalizarEmail(request.getHeader(headerEmail), login);

                final var contexto = resolverContextoUsuarioAutenticado.resolver(
                        new IdentidadeUsuarioAutenticado(login, nome, email)
                );

                final var authorities = mapeadorPerfilAcessoParaPermissao.mapearPermissoes(contexto.perfilAcesso());
                final var principal = new UsuarioAutenticadoPrincipal(
                        contexto.usuarioId(),
                        contexto.login(),
                        contexto.nome(),
                        contexto.email(),
                        contexto.perfilAcesso(),
                        contexto.departamentoId()
                );

                final var autenticacao = new UsernamePasswordAuthenticationToken(principal, "N/A", authorities);
                SecurityContextHolder.getContext().setAuthentication(autenticacao);
            }
        }

        filterChain.doFilter(request, response);
    }

    private String normalizarTextoOpcional(final String valor, final String padrao) {
        if (valor == null || valor.trim().isEmpty()) {
            return padrao;
        }
        return valor.trim();
    }

    private String normalizarEmail(final String emailInformado, final String login) {
        if (emailInformado != null && !emailInformado.trim().isEmpty()) {
            return emailInformado.trim().toLowerCase(Locale.ROOT);
        }
        return login.contains("@") ? login : login + "@local.geti";
    }
}
