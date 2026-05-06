package br.geti.sistemachamado.aplicacao.acesso;

public interface ResolverContextoUsuarioAutenticado {

    ContextoUsuarioAutenticado resolver(IdentidadeUsuarioAutenticado identidade);
}
