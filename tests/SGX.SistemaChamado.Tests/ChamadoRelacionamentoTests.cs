using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class ChamadoRelacionamentoTests
{
    private static readonly Guid ChamadoOrigemId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid ChamadoDestinoId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid CriadoPorUsuarioId = Guid.Parse("33333333-3333-3333-3333-333333333333");
    private static readonly Guid RemovidoPorUsuarioId = Guid.Parse("44444444-4444-4444-4444-444444444444");

    [Fact]
    public void DeveCriarRelacionamentoValidoEntreChamadosDiferentes()
    {
        var relacionamento = CriarRelacionamento();

        Assert.Equal(ChamadoOrigemId, relacionamento.ChamadoOrigemId);
        Assert.Equal(ChamadoDestinoId, relacionamento.ChamadoDestinoId);
        Assert.Equal(TipoRelacionamentoChamadoEnum.Bloqueia, relacionamento.TipoRelacionamento);
        Assert.Equal(CriadoPorUsuarioId, relacionamento.CriadoPorUsuarioId);
        Assert.True(relacionamento.Ativo);
        Assert.NotEqual(default, relacionamento.CriadoEm);
    }

    [Fact]
    public void NaoDevePermitirRelacionamentoComOrigemIgualAoDestino()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new ChamadoRelacionamento(
                ChamadoOrigemId,
                ChamadoOrigemId,
                TipoRelacionamentoChamadoEnum.Relacionado,
                CriadoPorUsuarioId,
                "usuario@sgx.local"));

        Assert.Contains("origem", ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("destino", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DeveExigirChamadoOrigem()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new ChamadoRelacionamento(
                Guid.Empty,
                ChamadoDestinoId,
                TipoRelacionamentoChamadoEnum.Relacionado,
                CriadoPorUsuarioId,
                "usuario@sgx.local"));

        Assert.Contains("origem", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DeveExigirChamadoDestino()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new ChamadoRelacionamento(
                ChamadoOrigemId,
                Guid.Empty,
                TipoRelacionamentoChamadoEnum.Relacionado,
                CriadoPorUsuarioId,
                "usuario@sgx.local"));

        Assert.Contains("destino", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DeveExigirTipoRelacionamentoValido()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new ChamadoRelacionamento(
                ChamadoOrigemId,
                ChamadoDestinoId,
                (TipoRelacionamentoChamadoEnum)0,
                CriadoPorUsuarioId,
                "usuario@sgx.local"));

        Assert.Contains("tipo", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DeveExigirUsuarioCriador()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new ChamadoRelacionamento(
                ChamadoOrigemId,
                ChamadoDestinoId,
                TipoRelacionamentoChamadoEnum.Relacionado,
                Guid.Empty,
                "usuario@sgx.local"));

        Assert.Contains("usuario", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DeveIniciarComoAtivo()
    {
        var relacionamento = CriarRelacionamento();

        Assert.True(relacionamento.Ativo);
        Assert.Null(relacionamento.RemovidoEm);
        Assert.Null(relacionamento.RemovidoPorUsuarioId);
    }

    [Fact]
    public void DevePermitirRegistrarJustificativa()
    {
        var relacionamento = new ChamadoRelacionamento(
            ChamadoOrigemId,
            ChamadoDestinoId,
            TipoRelacionamentoChamadoEnum.Duplicado,
            CriadoPorUsuarioId,
            "usuario@sgx.local",
            "  Vinculo identificado na triagem inicial.  ");

        Assert.Equal("Vinculo identificado na triagem inicial.", relacionamento.Justificativa);
    }

    [Fact]
    public void DeveNormalizarJustificativaEmBrancoParaNulo()
    {
        var relacionamento = new ChamadoRelacionamento(
            ChamadoOrigemId,
            ChamadoDestinoId,
            TipoRelacionamentoChamadoEnum.Duplicado,
            CriadoPorUsuarioId,
            "usuario@sgx.local",
            "   ");

        Assert.Null(relacionamento.Justificativa);
    }

    [Fact]
    public void NaoDevePermitirJustificativaMaiorQueLimite()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new ChamadoRelacionamento(
                ChamadoOrigemId,
                ChamadoDestinoId,
                TipoRelacionamentoChamadoEnum.Duplicado,
                CriadoPorUsuarioId,
                "usuario@sgx.local",
                new string('a', 2001)));

        Assert.Contains("maximo 2000", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DevePermitirInativacaoLogica()
    {
        var relacionamento = CriarRelacionamento();

        relacionamento.Inativar(RemovidoPorUsuarioId, "analista@sgx.local", "Vinculo deixou de ser aplicavel.");

        Assert.False(relacionamento.Ativo);
        Assert.NotNull(relacionamento.RemovidoEm);
        Assert.Equal(RemovidoPorUsuarioId, relacionamento.RemovidoPorUsuarioId);
        Assert.Equal("Vinculo deixou de ser aplicavel.", relacionamento.MotivoRemocao);
        Assert.NotNull(relacionamento.AtualizadoEm);
        Assert.Equal("analista@sgx.local", relacionamento.AtualizadoPor);
    }

    [Fact]
    public void DeveExigirUsuarioDeInativacao()
    {
        var relacionamento = CriarRelacionamento();

        var ex = Assert.Throws<ArgumentException>(() =>
            relacionamento.Inativar(Guid.Empty, "analista@sgx.local", "Remocao invalida."));

        Assert.Contains("usuario", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void NaoDevePermitirInativarRelacionamentoJaInativo()
    {
        var relacionamento = CriarRelacionamento();
        relacionamento.Inativar(RemovidoPorUsuarioId, "analista@sgx.local", "Primeira remocao.");

        var ex = Assert.Throws<InvalidOperationException>(() =>
            relacionamento.Inativar(RemovidoPorUsuarioId, "analista@sgx.local", "Segunda remocao."));

        Assert.Contains("ja esta inativo", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void NaoDevePermitirMotivoRemocaoMaiorQueLimite()
    {
        var relacionamento = CriarRelacionamento();

        var ex = Assert.Throws<ArgumentException>(() =>
            relacionamento.Inativar(RemovidoPorUsuarioId, "analista@sgx.local", new string('a', 1001)));

        Assert.Contains("maximo 1000", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DevePreservarDadosDeCriacaoAoInativar()
    {
        var relacionamento = CriarRelacionamento();
        var criadoEm = relacionamento.CriadoEm;
        var criadoPor = relacionamento.CriadoPor;

        relacionamento.Inativar(RemovidoPorUsuarioId, "analista@sgx.local", "Limpeza operacional.");

        Assert.Equal(criadoEm, relacionamento.CriadoEm);
        Assert.Equal(criadoPor, relacionamento.CriadoPor);
        Assert.Equal(CriadoPorUsuarioId, relacionamento.CriadoPorUsuarioId);
    }

    [Fact]
    public void DeveManterValoresDoEnumEstaveisParaContratosDaSprint2()
    {
        Assert.Equal(1, (int)TipoRelacionamentoChamadoEnum.Relacionado);
        Assert.Equal(2, (int)TipoRelacionamentoChamadoEnum.Pai);
        Assert.Equal(3, (int)TipoRelacionamentoChamadoEnum.Filho);
        Assert.Equal(4, (int)TipoRelacionamentoChamadoEnum.Duplicado);
        Assert.Equal(5, (int)TipoRelacionamentoChamadoEnum.Bloqueia);
        Assert.Equal(6, (int)TipoRelacionamentoChamadoEnum.BloqueadoPor);
        Assert.Equal(7, (int)TipoRelacionamentoChamadoEnum.DerivadoDe);
        Assert.Equal(8, (int)TipoRelacionamentoChamadoEnum.Origina);
    }

    [Theory]
    [InlineData(TipoRelacionamentoChamadoEnum.Bloqueia)]
    [InlineData(TipoRelacionamentoChamadoEnum.BloqueadoPor)]
    public void DeveReconhecerTiposDeBloqueioComoTiposOperacionais(TipoRelacionamentoChamadoEnum tipo)
    {
        Assert.True(tipo is TipoRelacionamentoChamadoEnum.Bloqueia or TipoRelacionamentoChamadoEnum.BloqueadoPor);
    }

    [Theory]
    [InlineData(TipoRelacionamentoChamadoEnum.Relacionado)]
    [InlineData(TipoRelacionamentoChamadoEnum.Duplicado)]
    [InlineData(TipoRelacionamentoChamadoEnum.Pai)]
    [InlineData(TipoRelacionamentoChamadoEnum.Filho)]
    [InlineData(TipoRelacionamentoChamadoEnum.DerivadoDe)]
    [InlineData(TipoRelacionamentoChamadoEnum.Origina)]
    public void DeveReconhecerTiposNaoBloqueantesComoInformativosParaDependencia(TipoRelacionamentoChamadoEnum tipo)
    {
        Assert.False(tipo is TipoRelacionamentoChamadoEnum.Bloqueia or TipoRelacionamentoChamadoEnum.BloqueadoPor);
    }

    private static ChamadoRelacionamento CriarRelacionamento()
    {
        return new ChamadoRelacionamento(
            ChamadoOrigemId,
            ChamadoDestinoId,
            TipoRelacionamentoChamadoEnum.Bloqueia,
            CriadoPorUsuarioId,
            "sistema@sgx.local");
    }
}
