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

        Assert.Equal(42, permissoes.Count);
        Assert.Equal(66, vinculos.Count);

        Assert.Equal(permissoes.Count, permissoes.Select(x => x.Codigo).Distinct(StringComparer.Ordinal).Count());
        Assert.Equal(vinculos.Count, vinculos.Select(x => new { x.PerfilAcessoId, x.PermissaoSistemaId }).Distinct().Count());

        var totalAdmin = vinculos.Count(x => x.PerfilAcessoId == SeedData.PerfilAdministradorId);
        var totalAtendente = vinculos.Count(x => x.PerfilAcessoId == SeedData.PerfilAtendenteId);
        var totalSolicitante = vinculos.Count(x => x.PerfilAcessoId == SeedData.PerfilSolicitanteId);

        Assert.Equal(42, totalAdmin);
        Assert.Equal(19, totalAtendente);
        Assert.Equal(5, totalSolicitante);
    }
}
