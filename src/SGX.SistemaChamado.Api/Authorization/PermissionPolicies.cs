namespace SGX.SistemaChamado.Api.Authorization;

public static class PermissionPolicies
{
    public const string Prefixo = "Permissao:";

    public const string DashboardVisualizar = Prefixo + PermissoesConstants.DashboardVisualizar;
    public const string CadastrosVisualizar = Prefixo + PermissoesConstants.CadastrosVisualizar;
    public const string CadastrosGerenciar = Prefixo + PermissoesConstants.CadastrosGerenciar;
    public const string UsuariosGerenciar = Prefixo + PermissoesConstants.UsuariosGerenciar;
    public const string UsuariosAlterarPerfis = Prefixo + PermissoesConstants.UsuariosAlterarPerfis;
    public const string PerfisGerenciar = Prefixo + PermissoesConstants.PerfisGerenciar;
    public const string PerfisAlterarPermissoes = Prefixo + PermissoesConstants.PerfisAlterarPermissoes;
    public const string ParametrosGerenciar = Prefixo + PermissoesConstants.ParametrosGerenciar;
    public const string IntegracoesEmailVisualizar = Prefixo + PermissoesConstants.IntegracoesEmailVisualizar;
    public const string IntegracoesMicrosoftVisualizar = Prefixo + PermissoesConstants.IntegracoesMicrosoftVisualizar;
    public const string IntegracoesMicrosoftGerenciar = Prefixo + PermissoesConstants.IntegracoesMicrosoftGerenciar;
    public const string UsuariosRedefinirSenha = Prefixo + PermissoesConstants.UsuariosRedefinirSenha;
    public const string ChamadosAssumir = Prefixo + PermissoesConstants.ChamadosAssumir;
    public const string ChamadosAtribuir = Prefixo + PermissoesConstants.ChamadosAtribuir;
    public const string ChamadosEncerrar = Prefixo + PermissoesConstants.ChamadosEncerrar;
    public const string RoadmapVisualizar = Prefixo + PermissoesConstants.RoadmapVisualizar;
    public const string RoadmapGerenciar = Prefixo + PermissoesConstants.RoadmapGerenciar;
    public const string RoadmapImplementacoesVisualizar = Prefixo + PermissoesConstants.RoadmapImplementacoesVisualizar;
    public const string RoadmapImplementacoesGerenciar = Prefixo + PermissoesConstants.RoadmapImplementacoesGerenciar;
    public const string SlaVisualizar = Prefixo + PermissoesConstants.SlaVisualizar;
    public const string SlaCriar = Prefixo + PermissoesConstants.SlaCriar;
    public const string SlaEditar = Prefixo + PermissoesConstants.SlaEditar;
    public const string SlaExcluir = Prefixo + PermissoesConstants.SlaExcluir;
    public const string SlaAtivarDesativar = Prefixo + PermissoesConstants.SlaAtivarDesativar;
    public const string AuditoriaVisualizar = Prefixo + PermissoesConstants.AuditoriaVisualizar;
    public const string AuditoriaGerenciar = Prefixo + PermissoesConstants.AuditoriaGerenciar;
}
