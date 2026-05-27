using SGX.SistemaChamado.Application.Interfaces.Sla;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class SlaServiceTests
{
    [Fact]
    public async Task ChamadoComPrioridadeBaixaRecebeSlaCorreto()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var service = SlaTestFactory.CriarService(context);
        var (chamado, prioridade) = await CriarChamadoBaseAsync(context, PrioridadeChamadoEnum.Baixa);

        await CriarPoliticaAsync(context, "SLA Teste Baixa", prioridade.Id, 120, 240, true);
        var referencia = DateTime.UtcNow;

        await service.InicializarNaAberturaAsync(chamado, "teste", referencia);
        await context.SaveChangesAsync();

        Assert.NotNull(chamado.ChamadoSla);
        Assert.Equal(referencia.AddMinutes(120), chamado.ChamadoSla!.PrazoPrimeiraResposta, TimeSpan.FromSeconds(1));
        Assert.Equal(referencia.AddMinutes(240), chamado.ChamadoSla.PrazoResolucao, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task ChamadoComPrioridadeCriticaRecebeSlaCorreto()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var service = SlaTestFactory.CriarService(context);
        var (chamado, prioridade) = await CriarChamadoBaseAsync(context, PrioridadeChamadoEnum.Critica);

        await CriarPoliticaAsync(context, "SLA Teste Critica", prioridade.Id, 15, 45, true);
        var referencia = DateTime.UtcNow;

        await service.InicializarNaAberturaAsync(chamado, "teste", referencia);
        await context.SaveChangesAsync();

        Assert.NotNull(chamado.ChamadoSla);
        Assert.Equal(referencia.AddMinutes(15), chamado.ChamadoSla!.PrazoPrimeiraResposta, TimeSpan.FromSeconds(1));
        Assert.Equal(referencia.AddMinutes(45), chamado.ChamadoSla.PrazoResolucao, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task PrimeiraRespostaDentroDoPrazo()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var service = SlaTestFactory.CriarService(context);
        var (chamado, prioridade) = await CriarChamadoBaseAsync(context, PrioridadeChamadoEnum.Alta);

        await CriarPoliticaAsync(context, "SLA Primeira Resposta", prioridade.Id, 60, 600, true);
        var inicio = DateTime.UtcNow;

        await service.InicializarNaAberturaAsync(chamado, "teste", inicio);
        await service.RegistrarPrimeiraRespostaAsync(chamado, "teste", inicio.AddMinutes(30));
        await context.SaveChangesAsync();

        Assert.NotNull(chamado.ChamadoSla);
        Assert.True(chamado.ChamadoSla!.PrimeiraRespostaCumprida);
        Assert.False(chamado.ChamadoSla.PrimeiraRespostaViolada);
    }

    [Fact]
    public async Task PrimeiraRespostaForaDoPrazo()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var service = SlaTestFactory.CriarService(context);
        var (chamado, prioridade) = await CriarChamadoBaseAsync(context, PrioridadeChamadoEnum.Alta);

        await CriarPoliticaAsync(context, "SLA Primeira Resposta Fora", prioridade.Id, 10, 600, true);
        var inicio = DateTime.UtcNow;

        await service.InicializarNaAberturaAsync(chamado, "teste", inicio);
        await service.RegistrarPrimeiraRespostaAsync(chamado, "teste", inicio.AddMinutes(20));
        await context.SaveChangesAsync();

        Assert.NotNull(chamado.ChamadoSla);
        Assert.False(chamado.ChamadoSla!.PrimeiraRespostaCumprida);
        Assert.True(chamado.ChamadoSla.PrimeiraRespostaViolada);
    }

    [Fact]
    public async Task ResolucaoDentroDoPrazo()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var service = SlaTestFactory.CriarService(context);
        var (chamado, prioridade) = await CriarChamadoBaseAsync(context, PrioridadeChamadoEnum.Media);

        await CriarPoliticaAsync(context, "SLA Resolucao", prioridade.Id, 30, 120, true);
        var inicio = DateTime.UtcNow;

        await service.InicializarNaAberturaAsync(chamado, "teste", inicio);
        await service.RegistrarEncerramentoAsync(chamado, "teste", inicio.AddMinutes(90));
        await context.SaveChangesAsync();

        Assert.NotNull(chamado.ChamadoSla);
        Assert.True(chamado.ChamadoSla!.ResolucaoCumprida);
        Assert.False(chamado.ChamadoSla.ResolucaoViolada);
    }

    [Fact]
    public async Task ResolucaoForaDoPrazo()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var service = SlaTestFactory.CriarService(context);
        var (chamado, prioridade) = await CriarChamadoBaseAsync(context, PrioridadeChamadoEnum.Media);

        await CriarPoliticaAsync(context, "SLA Resolucao Fora", prioridade.Id, 30, 60, true);
        var inicio = DateTime.UtcNow;

        await service.InicializarNaAberturaAsync(chamado, "teste", inicio);
        await service.RegistrarEncerramentoAsync(chamado, "teste", inicio.AddMinutes(90));
        await context.SaveChangesAsync();

        Assert.NotNull(chamado.ChamadoSla);
        Assert.False(chamado.ChamadoSla!.ResolucaoCumprida);
        Assert.True(chamado.ChamadoSla.ResolucaoViolada);
    }

    [Fact]
    public async Task NaoSobrescreveDataPrimeiraResposta()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var service = SlaTestFactory.CriarService(context);
        var (chamado, prioridade) = await CriarChamadoBaseAsync(context, PrioridadeChamadoEnum.Alta);

        await CriarPoliticaAsync(context, "SLA Nao Sobrescrever PR", prioridade.Id, 60, 600, true);
        var inicio = DateTime.UtcNow;
        var primeira = inicio.AddMinutes(10);

        await service.InicializarNaAberturaAsync(chamado, "teste", inicio);
        await service.RegistrarPrimeiraRespostaAsync(chamado, "teste", primeira);
        await service.RegistrarPrimeiraRespostaAsync(chamado, "teste", inicio.AddMinutes(20));
        await context.SaveChangesAsync();

        Assert.NotNull(chamado.ChamadoSla!.DataPrimeiraResposta);
        Assert.Equal(primeira, chamado.ChamadoSla.DataPrimeiraResposta!.Value, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task NaoSobrescreveDataResolucao()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var service = SlaTestFactory.CriarService(context);
        var (chamado, prioridade) = await CriarChamadoBaseAsync(context, PrioridadeChamadoEnum.Alta);

        await CriarPoliticaAsync(context, "SLA Nao Sobrescrever Resolucao", prioridade.Id, 60, 600, true);
        var inicio = DateTime.UtcNow;
        var resolucao = inicio.AddMinutes(20);

        await service.InicializarNaAberturaAsync(chamado, "teste", inicio);
        await service.RegistrarEncerramentoAsync(chamado, "teste", resolucao);
        await service.RegistrarEncerramentoAsync(chamado, "teste", inicio.AddMinutes(30));
        await context.SaveChangesAsync();

        Assert.NotNull(chamado.ChamadoSla!.DataResolucao);
        Assert.Equal(resolucao, chamado.ChamadoSla.DataResolucao!.Value, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task PausaSlaQuandoAguardandoSolicitante()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var service = SlaTestFactory.CriarService(context);
        var (chamado, prioridade) = await CriarChamadoBaseAsync(context, PrioridadeChamadoEnum.Alta);

        await CriarPoliticaAsync(context, "SLA Pausa", prioridade.Id, 60, 240, true, pausarQuandoAguardandoSolicitante: true);
        var baseTime = DateTime.UtcNow;

        await service.InicializarNaAberturaAsync(chamado, "teste", baseTime);

        var statusEmAtendimento = context.StatusChamado.First(x => x.Codigo == StatusChamadoEnum.EmAtendimento);
        var statusAguardando = context.StatusChamado.First(x => x.Codigo == StatusChamadoEnum.AguardandoSolicitante);

        await service.AplicarMudancaStatusAsync(chamado, statusEmAtendimento, statusAguardando, "teste", baseTime.AddMinutes(10));
        await service.AplicarMudancaStatusAsync(chamado, statusAguardando, statusEmAtendimento, "teste", baseTime.AddMinutes(40));
        await context.SaveChangesAsync();

        Assert.NotNull(chamado.ChamadoSla);
        Assert.False(chamado.ChamadoSla!.Pausado);
        Assert.True(chamado.ChamadoSla.MinutosPausados >= 29);
    }

    [Fact]
    public async Task StatusFinalEspecificoDeveEncerrarControleSla()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var service = SlaTestFactory.CriarService(context);
        var (chamado, prioridade) = await CriarChamadoBaseAsync(context, PrioridadeChamadoEnum.Alta);

        await CriarPoliticaAsync(context, "SLA Final Especifico", prioridade.Id, 60, 240, true);
        var baseTime = DateTime.UtcNow;

        await service.InicializarNaAberturaAsync(chamado, "teste", baseTime);

        var statusEmAnalise = context.StatusChamado.First(x => x.Codigo == StatusChamadoEnum.EmAnalise);
        var statusTratado = context.StatusChamado.First(x => x.Codigo == StatusChamadoEnum.Tratado);

        await service.AplicarMudancaStatusAsync(chamado, statusEmAnalise, statusTratado, "teste", baseTime.AddMinutes(20));
        await context.SaveChangesAsync();

        Assert.NotNull(chamado.ChamadoSla?.DataResolucao);
    }

    [Fact]
    public async Task PoliticaEspecificaPrevaleceSobreGeral()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var service = SlaTestFactory.CriarService(context);
        var (chamado, prioridade) = await CriarChamadoBaseAsync(context, PrioridadeChamadoEnum.Alta);

        await CriarPoliticaAsync(context, "SLA Geral", prioridade.Id, 180, 600, true);
        await CriarPoliticaAsync(context, "SLA Categoria", prioridade.Id, 30, 120, true, categoriaId: chamado.CategoriaId);

        var inicio = DateTime.UtcNow;
        await service.InicializarNaAberturaAsync(chamado, "teste", inicio);
        await context.SaveChangesAsync();

        Assert.NotNull(chamado.ChamadoSla);
        Assert.Equal(inicio.AddMinutes(30), chamado.ChamadoSla!.PrazoPrimeiraResposta, TimeSpan.FromSeconds(1));
        Assert.Equal(inicio.AddMinutes(120), chamado.ChamadoSla.PrazoResolucao, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task ChamadoSemPoliticaAtivaNaoQuebraSistema()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var service = SlaTestFactory.CriarService(context);
        var (chamado, _) = await CriarChamadoBaseAsync(context, PrioridadeChamadoEnum.Alta);

        await service.InicializarNaAberturaAsync(chamado, "teste", DateTime.UtcNow);
        await context.SaveChangesAsync();

        Assert.Null(chamado.ChamadoSla);
    }

    [Fact]
    public async Task ChamadoSemMetaParaPrioridadeNaoQuebraSistema()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var service = SlaTestFactory.CriarService(context);
        var (chamado, prioridade) = await CriarChamadoBaseAsync(context, PrioridadeChamadoEnum.Alta);

        var politica = new PoliticaSla(
            "SLA Sem Meta",
            "Sem metas para a prioridade do chamado.",
            1,
            null,
            null,
            null,
            false,
            true,
            "teste");

        context.SlaPoliticas.Add(politica);
        await context.SaveChangesAsync();

        await service.InicializarNaAberturaAsync(chamado, "teste", DateTime.UtcNow);
        await context.SaveChangesAsync();

        Assert.Null(chamado.ChamadoSla);
    }

    private static async Task<(Chamado Chamado, PrioridadeChamado Prioridade)> CriarChamadoBaseAsync(
        SGXSistemaChamadoDbContext context,
        PrioridadeChamadoEnum nivelPrioridade)
    {
        foreach (var politica in context.SlaPoliticas.Where(x => x.Ativo).ToList())
        {
            politica.Desativar("teste");
        }

        foreach (var meta in context.SlaMetas.Where(x => x.Ativo).ToList())
        {
            meta.Desativar("teste");
        }

        await context.SaveChangesAsync();

        var departamento = new Departamento($"TI-{Guid.NewGuid():N}".Substring(0, 10), "TI", null, "teste");
        var categoria = new CategoriaChamado($"Infra-{Guid.NewGuid():N}".Substring(0, 10), null, departamento.Id, "teste");
        var usuario = new Usuario($"Solicitante {Guid.NewGuid():N}".Substring(0, 18), $"solicitante.{Guid.NewGuid():N}@sgx.local", $"sol_{Guid.NewGuid():N}".Substring(0, 15), "teste");
        var prioridade = context.PrioridadesChamado.First(x => x.Nivel == nivelPrioridade);
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

    private static async Task CriarPoliticaAsync(
        SGXSistemaChamadoDbContext context,
        string nome,
        Guid prioridadeId,
        int primeiraRespostaMinutos,
        int resolucaoMinutos,
        bool ativo,
        bool pausarQuandoAguardandoSolicitante = true,
        Guid? categoriaId = null,
        Guid? departamentoId = null)
    {
        var politica = new PoliticaSla(
            nome,
            "Politica de teste",
            1,
            categoriaId,
            departamentoId,
            null,
            false,
            pausarQuandoAguardandoSolicitante,
            "teste");

        if (!ativo)
        {
            politica.Desativar("teste");
        }

        context.SlaPoliticas.Add(politica);
        await context.SaveChangesAsync();

        var meta = new MetaSla(
            politica.Id,
            prioridadeId,
            primeiraRespostaMinutos,
            resolucaoMinutos,
            null,
            null,
            "teste");

        if (!ativo)
        {
            meta.Desativar("teste");
        }

        context.SlaMetas.Add(meta);
        await context.SaveChangesAsync();
    }
}
