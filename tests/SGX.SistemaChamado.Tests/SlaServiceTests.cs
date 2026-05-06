using SGX.SistemaChamado.Application.Interfaces.Sla;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class SlaServiceTests
{
    [Fact]
    public async Task CalculaSlaDentroDoPrazo()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var (chamado, _) = await CriarChamadoBaseAsync(context);
        var service = SlaTestFactory.CriarService(context);

        await service.InicializarNaAberturaAsync(chamado, "teste", DateTime.UtcNow, default);
        await context.SaveChangesAsync();

        var controle = context.SlaControles.Single(x => x.ChamadoId == chamado.Id);
        Assert.False(controle.EstaVencido);
    }

    [Fact]
    public async Task IdentificaSlaVencido()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var (chamado, prioridade) = await CriarChamadoBaseAsync(context);
        var service = SlaTestFactory.CriarService(context);

        var config = new SlaConfiguracao(prioridade.Id, 0, 0, "teste");
        context.SlaConfiguracoes.Add(config);
        await context.SaveChangesAsync();

        var referencia = DateTime.UtcNow.AddHours(-5);
        await service.InicializarNaAberturaAsync(chamado, "teste", referencia, default);
        await context.SaveChangesAsync();

        await service.RegistrarEncerramentoAsync(chamado, "teste", DateTime.UtcNow);
        await context.SaveChangesAsync();

        var controle = context.SlaControles.Single(x => x.ChamadoId == chamado.Id);
        Assert.True(controle.EstaVencido);
    }

    [Fact]
    public async Task IdentificaProximoDoVencimento()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var (chamado, prioridade) = await CriarChamadoBaseAsync(context);
        var service = SlaTestFactory.CriarService(context);

        context.SlaConfiguracoes.Add(new SlaConfiguracao(prioridade.Id, 1, 1, "teste"));
        await context.SaveChangesAsync();

        var referencia = DateTime.UtcNow.AddMinutes(-30);
        await service.InicializarNaAberturaAsync(chamado, "teste", referencia, default);
        await context.SaveChangesAsync();

        Assert.True(service.EstaProximoDoVencimento(chamado.SlaControle, DateTime.UtcNow));
    }

    [Fact]
    public async Task UsaConfiguracaoMaisEspecifica()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var (chamado, prioridade) = await CriarChamadoBaseAsync(context);
        var service = SlaTestFactory.CriarService(context);

        context.SlaConfiguracoes.Add(new SlaConfiguracao(prioridade.Id, 4, 24, "teste"));
        context.SlaConfiguracoes.Add(new SlaConfiguracao(prioridade.Id, 2, 8, "teste", chamado.DepartamentoId, chamado.CategoriaId));
        await context.SaveChangesAsync();

        var referencia = DateTime.UtcNow;
        await service.InicializarNaAberturaAsync(chamado, "teste", referencia, default);
        await context.SaveChangesAsync();

        Assert.NotNull(chamado.SlaControle);
        Assert.Equal(referencia.AddHours(2), chamado.SlaControle!.PrazoPrimeiraRespostaEm, TimeSpan.FromSeconds(1));
        Assert.Equal(referencia.AddHours(8), chamado.SlaControle.PrazoResolucaoEm, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task UsaFallbackDaPrioridadeQuandoNaoHaConfiguracao()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var (chamado, prioridade) = await CriarChamadoBaseAsync(context);
        var service = SlaTestFactory.CriarService(context);
        var referencia = DateTime.UtcNow;

        await service.InicializarNaAberturaAsync(chamado, "teste", referencia, default);
        await context.SaveChangesAsync();

        Assert.NotNull(chamado.SlaControle);
        Assert.Equal(referencia.AddHours(prioridade.PrazoResolucaoHoras), chamado.SlaControle!.PrazoResolucaoEm, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task PausaERetomaAcumulandoTempoPausado()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var (chamado, prioridade) = await CriarChamadoBaseAsync(context);
        var service = SlaTestFactory.CriarService(context);

        context.SlaConfiguracoes.Add(new SlaConfiguracao(prioridade.Id, 2, 8, "teste"));
        await context.SaveChangesAsync();

        var baseTime = DateTime.UtcNow.AddHours(-1);
        await service.InicializarNaAberturaAsync(chamado, "teste", baseTime, default);
        await context.SaveChangesAsync();

        var statusAnterior = context.StatusChamado.First(x => x.Codigo == StatusChamadoEnum.EmAtendimento);
        var statusPausa = context.StatusChamado.First(x => x.Codigo == StatusChamadoEnum.AguardandoSolicitante);

        await service.AplicarMudancaStatusAsync(chamado, statusAnterior, statusPausa, "teste", baseTime.AddMinutes(10));
        await service.AplicarMudancaStatusAsync(chamado, statusPausa, statusAnterior, "teste", baseTime.AddMinutes(40));
        await context.SaveChangesAsync();

        Assert.NotNull(chamado.SlaControle);
        Assert.False(chamado.SlaControle!.EstaPausado);
        Assert.True(chamado.SlaControle.TotalMinutosPausado >= 29);
    }

    [Fact]
    public async Task PreenchePrimeiraRespostaNaAtribuicaoOuAssumir()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var (chamado, _) = await CriarChamadoBaseAsync(context);
        var service = SlaTestFactory.CriarService(context);

        await service.InicializarNaAberturaAsync(chamado, "teste", DateTime.UtcNow, default);
        await context.SaveChangesAsync();

        var agora = DateTime.UtcNow.AddMinutes(15);
        await service.RegistrarPrimeiraRespostaAsync(chamado, "teste", agora, default);
        await context.SaveChangesAsync();

        Assert.NotNull(chamado.SlaControle!.PrimeiraRespostaEm);
        Assert.True((chamado.SlaControle.PrimeiraRespostaEm!.Value - agora).Duration() <= TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task PreencheResolucaoNoEncerramento()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var (chamado, _) = await CriarChamadoBaseAsync(context);
        var service = SlaTestFactory.CriarService(context);

        await service.InicializarNaAberturaAsync(chamado, "teste", DateTime.UtcNow, default);
        await context.SaveChangesAsync();

        var agora = DateTime.UtcNow.AddHours(1);
        await service.RegistrarEncerramentoAsync(chamado, "teste", agora);
        await context.SaveChangesAsync();

        Assert.NotNull(chamado.SlaControle!.ResolvidoEm);
        Assert.True((chamado.SlaControle.ResolvidoEm!.Value - agora).Duration() <= TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task RecalculaSlaAoMudarPrioridade()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var (chamado, prioridadeAtual) = await CriarChamadoBaseAsync(context);
        var service = SlaTestFactory.CriarService(context);

        var prioridadeCritica = context.PrioridadesChamado.First(x => x.Nivel == PrioridadeChamadoEnum.Critica);
        context.SlaConfiguracoes.Add(new SlaConfiguracao(prioridadeAtual.Id, 4, 24, "teste"));
        context.SlaConfiguracoes.Add(new SlaConfiguracao(prioridadeCritica.Id, 1, 2, "teste"));
        await context.SaveChangesAsync();

        var abertura = DateTime.UtcNow.AddHours(-3);
        await service.InicializarNaAberturaAsync(chamado, "teste", abertura, default);
        await context.SaveChangesAsync();

        chamado.AlterarPrioridade(prioridadeCritica.Id, "teste");
        var alteracao = DateTime.UtcNow;
        await service.AplicarMudancaPrioridadeAsync(chamado, "teste", alteracao, default);
        await context.SaveChangesAsync();

        Assert.Equal(alteracao.AddHours(2), chamado.SlaControle!.PrazoResolucaoEm, TimeSpan.FromSeconds(1));
    }

    private static async Task<(Chamado Chamado, PrioridadeChamado Prioridade)> CriarChamadoBaseAsync(SGXSistemaChamadoDbContext context)
    {
        var departamento = new Departamento("TI", "TI", null, "teste");
        var categoria = new CategoriaChamado("Infra", null, departamento.Id, "teste");
        var usuario = new Usuario("Solicitante", "solicitante@sgx.local", "solicitante", "teste");
        var prioridade = context.PrioridadesChamado.First(x => x.Nivel == PrioridadeChamadoEnum.Alta);
        var statusAberto = context.StatusChamado.First(x => x.Codigo == StatusChamadoEnum.Aberto);

        context.Departamentos.Add(departamento);
        context.CategoriasChamado.Add(categoria);
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        var chamado = new Chamado(
            $"CH-SLA-{Guid.NewGuid():N}".Substring(0, 16),
            "Chamado SLA",
            "Descricao",
            usuario.Id,
            categoria.Id,
            prioridade.Id,
            statusAberto.Id,
            OrigemChamado.Portal,
            "teste",
            departamento.Id);

        context.Chamados.Add(chamado);
        await context.SaveChangesAsync();
        return (chamado, prioridade);
    }
}
