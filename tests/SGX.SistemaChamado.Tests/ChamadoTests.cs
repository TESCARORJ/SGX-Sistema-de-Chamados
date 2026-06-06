using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Tests;

public sealed class ChamadoTests
{
    private static readonly Guid SolicitanteId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly Guid CategoriaId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    [Fact]
    public void DeveCriarChamadoValido()
    {
        var chamado = CriarChamado();

        Assert.Equal("CH-0001", chamado.Codigo);
        Assert.Equal("Falha no acesso VPN", chamado.Titulo);
        Assert.Equal(SolicitanteId, chamado.SolicitanteId);
        Assert.Equal(SeedData.PrioridadeMediaId, chamado.PrioridadeId);
        Assert.Equal(SeedData.StatusAbertoId, chamado.StatusId);
    }

    [Fact]
    public void NaoDevePermitirTituloVazio()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new Chamado(
                "CH-0002",
                "   ",
                "Descricao valida",
                SolicitanteId,
                CategoriaId,
                SeedData.PrioridadeMediaId,
                SeedData.StatusAbertoId,
                OrigemChamado.Portal,
                "teste"));

        Assert.Contains("titulo", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void NaoDevePermitirDescricaoVazia()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new Chamado(
                "CH-0003",
                "Titulo valido",
                " ",
                SolicitanteId,
                CategoriaId,
                SeedData.PrioridadeMediaId,
                SeedData.StatusAbertoId,
                OrigemChamado.Portal,
                "teste"));

        Assert.Contains("descricao", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DevePermitirAlterarStatus()
    {
        var chamado = CriarChamado();

        chamado.AlterarStatus(SeedData.StatusEmAtendimentoId, "atendente@sgx.local");

        Assert.Equal(SeedData.StatusEmAtendimentoId, chamado.StatusId);
        Assert.NotNull(chamado.AtualizadoEm);
    }

    [Fact]
    public void DevePermitirAtribuirResponsavel()
    {
        var chamado = CriarChamado();
        var responsavelId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

        chamado.AtribuirResponsavel(responsavelId, "gestor@sgx.local");

        Assert.Equal(responsavelId, chamado.ResponsavelId);
    }

    [Fact]
    public void ChamadoLegadoSemGrupoFilaEResponsavelPermaneceValido()
    {
        var chamado = CriarChamado();

        chamado.DefinirGrupoTecnico(null, "teste");
        chamado.DefinirFilaAtendimento(null, "teste");
        chamado.AtribuirResponsavel(null, "teste");

        Assert.Null(chamado.GrupoTecnicoId);
        Assert.Null(chamado.FilaAtendimentoId);
        Assert.Null(chamado.ResponsavelId);
    }

    [Fact]
    public void ChamadoLegadoComResponsavelEGrupoFilaNulosPermaneceValido()
    {
        var chamado = CriarChamado();
        var responsavelId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

        chamado.AtribuirResponsavel(responsavelId, "gestor@sgx.local");

        Assert.Equal(responsavelId, chamado.ResponsavelId);
        Assert.Null(chamado.GrupoTecnicoId);
        Assert.Null(chamado.FilaAtendimentoId);
    }

    [Fact]
    public void DevePermitirEncerrarChamado()
    {
        var chamado = CriarChamado();

        chamado.Encerrar(SeedData.StatusEncerradoId, "atendente@sgx.local");

        Assert.Equal(SeedData.StatusEncerradoId, chamado.StatusId);
        Assert.NotNull(chamado.EncerradoEm);
    }

    private static Chamado CriarChamado()
    {
        return new Chamado(
            "CH-0001",
            "Falha no acesso VPN",
            "Usuario sem acesso ao ambiente corporativo.",
            SolicitanteId,
            CategoriaId,
            SeedData.PrioridadeMediaId,
            SeedData.StatusAbertoId,
            OrigemChamado.Portal,
            "solicitante@sgx.local");
    }
}
