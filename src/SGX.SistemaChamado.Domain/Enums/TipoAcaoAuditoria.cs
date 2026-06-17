namespace SGX.SistemaChamado.Domain.Enums;

public enum TipoAcaoAuditoria
{
    Login = 1,
    Logout = 2,
    Criacao = 3,
    Edicao = 4,
    ExclusaoLogica = 5,
    Ativacao = 6,
    Inativacao = 7,
    AlteracaoStatus = 8,
    AlteracaoPermissao = 9,
    Visualizacao = 10,
    Exportacao = 11,
    Importacao = 12,
    Erro = 13,
    ExecucaoJob = 14,
    Configuracao = 15,
    Homologacao = 16,
    Outro = 17,
    ResolverChamado = 18,
    FecharChamadoAutomaticamentePorPrazoAceite = 19,
    RejeitarSolucaoChamado = 20,
    AceitarSolucaoChamado = 21,
    ReabrirChamado = 22
}
