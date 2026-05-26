namespace SGX.SistemaChamado.Domain.Enums;

public enum TipoEventoAutenticacao
{
    LoginLocalSgxSucesso = 1,
    LoginLocalSgxNegado = 2,
    LoginActiveDirectorySucesso = 3,
    LoginActiveDirectoryNegado = 4,
    LoginMicrosoftEntraIdSucesso = 5,
    UsuarioInativoBloqueado = 6,
    ProvedorDesabilitadoTentativaLogin = 7,
    FalhaConfiguracaoProvedor = 8,
    FalhaCredencialInvalida = 9,
    AutoProvisionamentoUsuario = 10,
    TrocaObrigatoriaSenhaConcluida = 11,
    RecuperacaoSenhaSolicitada = 12,
    RedefinicaoSenhaConcluida = 13,
    AlteracaoProvedorHabilitado = 14,
    AlteracaoProvedorPrincipal = 15,
    AlteracaoOrdemExibicao = 16,
    AlteracaoAutoProvisionamento = 17,
    AlteracaoPerfilPadraoProvisionamento = 18,
    TentativaNegadaAlteracaoMetodosLogin = 19,
    BloqueioConfiguracaoInsegura = 20,
    AlteracaoRotuloExibicao = 21
}
