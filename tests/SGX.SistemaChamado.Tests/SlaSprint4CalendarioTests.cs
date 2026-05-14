using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Services.Sla;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Tests;

public sealed class SlaSprint4CalendarioTests
{
    [Fact]
    public void SeedDeveCriarCalendarioCorporativoPadrao()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();

        var calendario = context.CalendariosCorporativos.Single(x => x.Id == SeedData.CalendarioCorporativoPadraoId);
        var horarios = context.HorariosAtendimentoCalendario
            .Where(x => x.CalendarioCorporativoId == calendario.Id && x.Ativo)
            .ToArray();

        Assert.True(calendario.Ativo);
        Assert.True(calendario.Padrao);
        Assert.Equal("America/Sao_Paulo", calendario.TimeZone);
        Assert.Equal(5, horarios.Length);
        Assert.All(horarios, x =>
        {
            Assert.Equal(new TimeOnly(9, 0), x.HoraInicio);
            Assert.Equal(new TimeOnly(18, 0), x.HoraFim);
        });
    }

    [Fact]
    public void CalculadorDeveSomarMinutosAtravessandoFimDeSemana()
    {
        var calendario = CriarCalendarioPadrao();
        var calculador = new SlaBusinessTimeCalculator();
        var sexta1730Utc = new DateTimeOffset(2026, 5, 15, 20, 30, 0, TimeSpan.Zero);

        var prazo = calculador.AddBusinessMinutes(sexta1730Utc, 90, calendario);

        Assert.Equal(new DateTimeOffset(2026, 5, 18, 13, 0, 0, TimeSpan.Zero), prazo);
    }

    [Fact]
    public void CalculadorDeveSomarMinutosDentroDoExpediente()
    {
        var calendario = CriarCalendarioPadrao();
        var calculador = new SlaBusinessTimeCalculator();
        var segunda1000Utc = new DateTimeOffset(2026, 5, 18, 13, 0, 0, TimeSpan.Zero);

        var prazo = calculador.AddBusinessMinutes(segunda1000Utc, 60, calendario);

        Assert.Equal(new DateTimeOffset(2026, 5, 18, 14, 0, 0, TimeSpan.Zero), prazo);
        Assert.True(calculador.IsBusinessTime(segunda1000Utc, calendario));
    }

    [Fact]
    public void CalculadorDeveSomarMinutosAtravessandoIntervaloDeAlmoco()
    {
        var calendario = new CalendarioCorporativo("Calendario com almoco", "Teste", true, "America/Sao_Paulo", "teste");
        calendario.HorariosAtendimento.Add(new HorarioAtendimentoCalendario(calendario.Id, DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(12, 0), true, "teste"));
        calendario.HorariosAtendimento.Add(new HorarioAtendimentoCalendario(calendario.Id, DayOfWeek.Monday, new TimeOnly(13, 0), new TimeOnly(18, 0), true, "teste"));

        var calculador = new SlaBusinessTimeCalculator();
        var segunda1030Utc = new DateTimeOffset(2026, 5, 18, 13, 30, 0, TimeSpan.Zero);

        var prazo = calculador.AddBusinessMinutes(segunda1030Utc, 180, calendario);

        Assert.Equal(new DateTimeOffset(2026, 5, 18, 17, 30, 0, TimeSpan.Zero), prazo);
        Assert.Equal(180, calculador.CountBusinessMinutes(segunda1030Utc, prazo, calendario));
    }

    [Fact]
    public void CalculadorDeveSomarMinutosAtravessandoFimDoDia()
    {
        var calendario = CriarCalendarioPadrao();
        var calculador = new SlaBusinessTimeCalculator();
        var segunda1730Utc = new DateTimeOffset(2026, 5, 18, 20, 30, 0, TimeSpan.Zero);

        var prazo = calculador.AddBusinessMinutes(segunda1730Utc, 90, calendario);

        Assert.Equal(new DateTimeOffset(2026, 5, 19, 13, 0, 0, TimeSpan.Zero), prazo);
    }

    [Fact]
    public void CalculadorDeveIgnorarFeriado()
    {
        var calendario = CriarCalendarioPadrao();
        calendario.Excecoes.Add(new ExcecaoCalendarioCorporativo(
            calendario.Id,
            new DateOnly(2026, 5, 18),
            TipoExcecaoCalendarioCorporativo.Feriado,
            "Feriado teste",
            null,
            null,
            true,
            "teste"));

        var calculador = new SlaBusinessTimeCalculator();
        var sexta1730Utc = new DateTimeOffset(2026, 5, 15, 20, 30, 0, TimeSpan.Zero);

        var prazo = calculador.AddBusinessMinutes(sexta1730Utc, 90, calendario);

        Assert.Equal(new DateTimeOffset(2026, 5, 19, 13, 0, 0, TimeSpan.Zero), prazo);
    }

    [Fact]
    public void CalculadorDeveRespeitarExpedienteEspecial()
    {
        var calendario = CriarCalendarioPadrao();
        calendario.Excecoes.Add(new ExcecaoCalendarioCorporativo(
            calendario.Id,
            new DateOnly(2026, 5, 18),
            TipoExcecaoCalendarioCorporativo.ExpedienteEspecial,
            "Expediente reduzido",
            new TimeOnly(10, 0),
            new TimeOnly(12, 0),
            true,
            "teste"));

        var calculador = new SlaBusinessTimeCalculator();
        var segunda0900Utc = new DateTimeOffset(2026, 5, 18, 12, 0, 0, TimeSpan.Zero);

        Assert.False(calculador.IsBusinessTime(segunda0900Utc, calendario));
        Assert.Equal(new DateTimeOffset(2026, 5, 18, 13, 0, 0, TimeSpan.Zero), calculador.NextBusinessTime(segunda0900Utc, calendario));
        Assert.Equal(120, calculador.CountBusinessMinutes(segunda0900Utc, new DateTimeOffset(2026, 5, 18, 16, 0, 0, TimeSpan.Zero), calendario));
    }

    [Fact]
    public async Task PoliticaComHorarioComercialDeveUsarCalendarioNoPrazoEMinutos()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        DesativarSlaPadrao(context);
        var service = SlaTestFactory.CriarService(context);
        var (chamado, prioridade) = await CriarChamadoBaseAsync(context);

        var politica = new PoliticaSla(
            "SLA horario comercial",
            "Teste Sprint 4",
            1,
            null,
            null,
            SeedData.CalendarioCorporativoPadraoId,
            true,
            true,
            "teste");

        context.SlaPoliticas.Add(politica);
        await context.SaveChangesAsync();
        context.SlaMetas.Add(new MetaSla(politica.Id, prioridade.Id, 90, 180, null, null, "teste"));
        await context.SaveChangesAsync();

        var inicioSexta1730Utc = new DateTime(2026, 5, 15, 20, 30, 0, DateTimeKind.Utc);
        await service.InicializarNaAberturaAsync(chamado, "teste", inicioSexta1730Utc);
        await service.RegistrarPrimeiraRespostaAsync(chamado, "teste", new DateTime(2026, 5, 18, 13, 0, 0, DateTimeKind.Utc));
        await context.SaveChangesAsync();

        Assert.NotNull(chamado.ChamadoSla);
        Assert.True(chamado.ChamadoSla!.UsarHorarioComercial);
        Assert.Equal(SeedData.CalendarioCorporativoPadraoId, chamado.ChamadoSla.CalendarioCorporativoId);
        Assert.Equal(new DateTime(2026, 5, 18, 13, 0, 0, DateTimeKind.Utc), chamado.ChamadoSla.PrazoPrimeiraResposta);
        Assert.Equal(90, chamado.ChamadoSla.MinutosPrimeiraResposta);
        Assert.True(chamado.ChamadoSla.PrimeiraRespostaCumprida);
    }

    [Fact]
    public async Task PoliticaComHorarioComercialSemCalendarioPadraoDeveCairParaMinutosCorridos()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        DesativarSlaPadrao(context);
        foreach (var calendario in context.CalendariosCorporativos.Where(x => x.Ativo).ToArray())
        {
            calendario.Desativar("teste");
        }

        await context.SaveChangesAsync();
        var service = SlaTestFactory.CriarService(context);
        var (chamado, prioridade) = await CriarChamadoBaseAsync(context);

        var politica = new PoliticaSla("SLA horario sem calendario", "Teste", 1, null, null, null, true, true, "teste");
        context.SlaPoliticas.Add(politica);
        await context.SaveChangesAsync();
        context.SlaMetas.Add(new MetaSla(politica.Id, prioridade.Id, 90, 180, null, null, "teste"));
        await context.SaveChangesAsync();

        var inicioSexta1730Utc = new DateTime(2026, 5, 15, 20, 30, 0, DateTimeKind.Utc);
        await service.InicializarNaAberturaAsync(chamado, "teste", inicioSexta1730Utc);
        await context.SaveChangesAsync();

        Assert.NotNull(chamado.ChamadoSla);
        Assert.False(chamado.ChamadoSla!.UsarHorarioComercial);
        Assert.Null(chamado.ChamadoSla.CalendarioCorporativoId);
        Assert.Equal(inicioSexta1730Utc.AddMinutes(90), chamado.ChamadoSla.PrazoPrimeiraResposta);
    }

    [Fact]
    public async Task PoliticaSemHorarioComercialMantemMinutosCorridos()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        DesativarSlaPadrao(context);
        var service = SlaTestFactory.CriarService(context);
        var (chamado, prioridade) = await CriarChamadoBaseAsync(context);

        var politica = new PoliticaSla("SLA corrido", "Teste", 1, null, null, null, false, true, "teste");
        context.SlaPoliticas.Add(politica);
        await context.SaveChangesAsync();
        context.SlaMetas.Add(new MetaSla(politica.Id, prioridade.Id, 90, 180, null, null, "teste"));
        await context.SaveChangesAsync();

        var inicioSexta1730Utc = new DateTime(2026, 5, 15, 20, 30, 0, DateTimeKind.Utc);
        await service.InicializarNaAberturaAsync(chamado, "teste", inicioSexta1730Utc);
        await context.SaveChangesAsync();

        Assert.NotNull(chamado.ChamadoSla);
        Assert.False(chamado.ChamadoSla!.UsarHorarioComercial);
        Assert.Equal(inicioSexta1730Utc.AddMinutes(90), chamado.ChamadoSla.PrazoPrimeiraResposta);
    }

    [Fact]
    public async Task ServiceAdministrativoDeveImpedirSobreposicaoDeHorariosAtivos()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var service = CriarCalendarioService(context);

        var request = new HorarioAtendimentoCalendarioRequest
        {
            DiaSemana = DayOfWeek.Monday,
            HoraInicio = new TimeOnly(10, 0),
            HoraFim = new TimeOnly(11, 0),
            Ativo = true
        };

        var erro = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.AdicionarHorarioAsync(SeedData.CalendarioCorporativoPadraoId, request, "teste"));

        Assert.Contains("sobreposto", erro.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ServiceAdministrativoDeveManterApenasUmCalendarioPadraoAtivo()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var service = CriarCalendarioService(context);

        var novo = await service.CriarAsync(
            new CriarCalendarioCorporativoRequest
            {
                Nome = "Calendario futuro",
                Descricao = "Teste",
                Ativo = false,
                Padrao = true,
                TimeZone = "America/Sao_Paulo"
            },
            "teste");

        await service.AtualizarStatusAsync(novo.Id, true, "teste");

        var padroesAtivos = context.CalendariosCorporativos.Count(x => x.Ativo && x.Padrao);
        var calendarioAntigo = context.CalendariosCorporativos.Single(x => x.Id == SeedData.CalendarioCorporativoPadraoId);
        var calendarioNovo = context.CalendariosCorporativos.Single(x => x.Id == novo.Id);

        Assert.Equal(1, padroesAtivos);
        Assert.False(calendarioAntigo.Padrao);
        Assert.True(calendarioNovo.Padrao);
        Assert.True(calendarioNovo.Ativo);
    }

    private static CalendarioCorporativoService CriarCalendarioService(SGXSistemaChamadoDbContext context)
        => new(
            PortalUseCasesTestFactory.Repo<CalendarioCorporativo>(context),
            PortalUseCasesTestFactory.Repo<HorarioAtendimentoCalendario>(context),
            PortalUseCasesTestFactory.Repo<ExcecaoCalendarioCorporativo>(context),
            PortalUseCasesTestFactory.Uow(context));

    private static CalendarioCorporativo CriarCalendarioPadrao()
    {
        var calendario = new CalendarioCorporativo("Calendario teste", "Teste", true, "America/Sao_Paulo", "teste");

        foreach (var dia in new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday })
        {
            calendario.HorariosAtendimento.Add(new HorarioAtendimentoCalendario(
                calendario.Id,
                dia,
                new TimeOnly(9, 0),
                new TimeOnly(18, 0),
                true,
                "teste"));
        }

        return calendario;
    }

    private static void DesativarSlaPadrao(SGXSistemaChamadoDbContext context)
    {
        foreach (var politica in context.SlaPoliticas.Where(x => x.Ativo).ToList())
        {
            politica.Desativar("teste");
        }

        foreach (var meta in context.SlaMetas.Where(x => x.Ativo).ToList())
        {
            meta.Desativar("teste");
        }

        context.SaveChanges();
    }

    private static async Task<(Chamado Chamado, PrioridadeChamado Prioridade)> CriarChamadoBaseAsync(SGXSistemaChamadoDbContext context)
    {
        var departamento = new Departamento($"TI-{Guid.NewGuid():N}".Substring(0, 10), "TI", null, "teste");
        var categoria = new CategoriaChamado($"Infra-{Guid.NewGuid():N}".Substring(0, 10), null, departamento.Id, "teste");
        var usuario = new Usuario($"Solicitante {Guid.NewGuid():N}".Substring(0, 18), $"solicitante.{Guid.NewGuid():N}@sgx.local", $"sol_{Guid.NewGuid():N}".Substring(0, 15), "teste");
        var prioridade = context.PrioridadesChamado.First(x => x.Nivel == PrioridadeChamadoEnum.Alta);
        var statusAberto = context.StatusChamado.First(x => x.Codigo == StatusChamadoEnum.Aberto);

        context.Departamentos.Add(departamento);
        context.CategoriasChamado.Add(categoria);
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        var chamado = new Chamado(
            $"CH-SLA4-{Guid.NewGuid():N}".Substring(0, 16),
            "Chamado SLA 4",
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
