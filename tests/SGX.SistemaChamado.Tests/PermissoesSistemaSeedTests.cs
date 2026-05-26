using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Tests;

public sealed class PermissoesSistemaSeedTests
{
    [Fact]
    public void DeveSeedarPermissoesPorPerfilSemDuplicidades()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();

        var permissoes = context.PermissoesSistema.ToList();
        var vinculos = context.PerfisAcessoPermissoes.ToList();

        Assert.Equal(71, permissoes.Count);
        Assert.Equal(103, vinculos.Count);

        Assert.Equal(permissoes.Count, permissoes.Select(x => x.Codigo).Distinct(StringComparer.Ordinal).Count());
        Assert.Equal(vinculos.Count, vinculos.Select(x => new { x.PerfilAcessoId, x.PermissaoSistemaId }).Distinct().Count());
        Assert.Contains(permissoes, x => x.Codigo == "RelatoriosAvancados.Visualizar");
        Assert.Contains(permissoes, x => x.Codigo == "RelatoriosAvancados.Exportar");
        Assert.Contains(permissoes, x => x.Codigo == "RelatoriosAvancados.Gerencial");
        Assert.Contains(permissoes, x => x.Codigo == "RelatoriosAvancados.Operacional");
        Assert.Contains(permissoes, x => x.Codigo == "RelatoriosAvancados.Auditoria");
        Assert.Contains(permissoes, x => x.Codigo == "AutenticacaoProvedores.Visualizar");
        Assert.Contains(permissoes, x => x.Codigo == "AutenticacaoProvedores.Gerenciar");
        Assert.Contains(permissoes, x => x.Codigo == "AuditoriaAutenticacao.Visualizar");
        Assert.Contains(permissoes, x => x.Codigo == "IntegracoesActiveDirectory.Visualizar");
        Assert.Contains(permissoes, x => x.Codigo == "IntegracoesActiveDirectory.Gerenciar");

        var totalAdmin = vinculos.Count(x => x.PerfilAcessoId == SeedData.PerfilAdministradorId);
        var totalAtendente = vinculos.Count(x => x.PerfilAcessoId == SeedData.PerfilAtendenteId);
        var totalSolicitante = vinculos.Count(x => x.PerfilAcessoId == SeedData.PerfilSolicitanteId);

        Assert.Equal(71, totalAdmin);
        Assert.Equal(27, totalAtendente);
        Assert.Equal(5, totalSolicitante);
    }
}
