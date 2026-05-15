using SGX.SistemaChamado.Application.Helpers;

namespace SGX.SistemaChamado.Tests;

public sealed class AuditoriaDiffHelperTests
{
    [Fact]
    public void DeveMascararCamposSensiveis()
    {
        var json = AuditoriaDiffHelper.SerializarSeguro(new
        {
            senha = "abc",
            password = "def",
            accessToken = "ghi",
            refresh_token = "jkl",
            clientSecret = "mno",
            authorization = "Bearer xyz",
            apiKey = "123"
        });

        Assert.NotNull(json);
        Assert.DoesNotContain("abc", json, StringComparison.Ordinal);
        Assert.DoesNotContain("def", json, StringComparison.Ordinal);
        Assert.DoesNotContain("ghi", json, StringComparison.Ordinal);
        Assert.DoesNotContain("jkl", json, StringComparison.Ordinal);
        Assert.DoesNotContain("mno", json, StringComparison.Ordinal);
        Assert.DoesNotContain("xyz", json, StringComparison.Ordinal);
        Assert.DoesNotContain("123", json, StringComparison.Ordinal);
        Assert.Contains("***", json, StringComparison.Ordinal);
    }

    [Fact]
    public void DeveGerarDiffSomenteComCamposAlterados()
    {
        var (antes, depois) = AuditoriaDiffHelper.CriarDiff(
            new { prioridade = "Media", status = "Aberto" },
            new { prioridade = "Alta", status = "Aberto" });

        Assert.NotNull(antes);
        Assert.NotNull(depois);
        Assert.Contains("prioridade", antes, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("prioridade", depois, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("status", antes, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("status", depois, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DeveMascararDiffComCamposSensiveis()
    {
        var (antes, depois) = AuditoriaDiffHelper.CriarDiff(
            new { token = "abc" },
            new { token = "xyz" },
            somenteAlterados: false);

        Assert.NotNull(antes);
        Assert.NotNull(depois);
        Assert.DoesNotContain("abc", antes, StringComparison.Ordinal);
        Assert.DoesNotContain("xyz", depois, StringComparison.Ordinal);
        Assert.Contains("***", antes, StringComparison.Ordinal);
        Assert.Contains("***", depois, StringComparison.Ordinal);
    }
}
