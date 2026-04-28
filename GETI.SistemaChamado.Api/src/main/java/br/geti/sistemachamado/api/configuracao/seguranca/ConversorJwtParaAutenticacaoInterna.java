package br.geti.sistemachamado.api.configuracao.seguranca;

import br.geti.sistemachamado.aplicacao.acesso.ResolverContextoUsuarioAutenticado;
import org.springframework.core.convert.converter.Converter;
import org.springframework.security.authentication.AbstractAuthenticationToken;
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.security.oauth2.jwt.Jwt;
import org.springframework.stereotype.Component;

@Component
public class ConversorJwtParaAutenticacaoInterna implements Converter<Jwt, AbstractAuthenticationToken> {

    private final ExtratorIdentidadeJwt extratorIdentidadeJwt;
    private final ResolverContextoUsuarioAutenticado resolverContextoUsuarioAutenticado;
    private final MapeadorPerfilAcessoParaPermissao mapeadorPerfilAcessoParaPermissao;

    public ConversorJwtParaAutenticacaoInterna(
            final ExtratorIdentidadeJwt extratorIdentidadeJwt,
            final ResolverContextoUsuarioAutenticado resolverContextoUsuarioAutenticado,
            final MapeadorPerfilAcessoParaPermissao mapeadorPerfilAcessoParaPermissao
    ) {
        this.extratorIdentidadeJwt = extratorIdentidadeJwt;
        this.resolverContextoUsuarioAutenticado = resolverContextoUsuarioAutenticado;
        this.mapeadorPerfilAcessoParaPermissao = mapeadorPerfilAcessoParaPermissao;
    }

    @Override
    public AbstractAuthenticationToken convert(final Jwt jwt) {
        final var identidade = extratorIdentidadeJwt.extrair(jwt);
        final var contexto = resolverContextoUsuarioAutenticado.resolver(identidade);
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
        autenticacao.setDetails(jwt);
        return autenticacao;
    }
}
