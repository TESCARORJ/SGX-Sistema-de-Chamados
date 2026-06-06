using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class TransferirGrupoTecnicoChamadoUseCaseTests
{
    [Fact]
    public async Task RejeitaTransferenciaDeChamadoSemGrupoTecnico()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "TG1");
        var grupoDestino = await CriarGrupoAsync(context, "Service Desk TG1");
        var useCase = CriarUseCase(context, dados.AdminContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(
            dados.Chamado.Id,
            new TransferirGrupoTecnicoChamadoRequest { GrupoTecnicoId = grupoDestino.Id }));

        Assert.Equal("Chamado sem grupo tecnico deve ser direcionado antes de ser transferido entre grupos.", ex.Message);
        Assert.Null(context.Chamados.Single().GrupoTecnicoId);
    }

    [Fact]
    public async Task TransfereChamadoDeUmGrupoParaOutro()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "TG2");
        var grupoOrigem = await CriarGrupoAsync(context, "Origem TG2");
        var grupoDestino = await CriarGrupoAsync(context, "Destino TG2");
        dados.Chamado.DefinirGrupoTecnico(grupoOrigem.Id, "teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.AdminContexto);

        await useCase.ExecutarAsync(dados.Chamado.Id, new TransferirGrupoTecnicoChamadoRequest { GrupoTecnicoId = grupoDestino.Id });

        Assert.Equal(grupoDestino.Id, context.Chamados.Single().GrupoTecnicoId);
    }

    [Fact]
    public async Task RejeitaTransferenciaPorSolicitante()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "TG16");
        var grupoOrigem = await CriarGrupoAsync(context, "Origem TG16");
        var grupoDestino = await CriarGrupoAsync(context, "Destino TG16");
        dados.Chamado.DefinirGrupoTecnico(grupoOrigem.Id, "teste");
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            context,
            "Solicitante TG16",
            "solicitante-tg16@empresa.com",
            TipoPerfil.Solicitante);
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, AdminUseCasesTestFactory.Contexto(solicitante, "Solicitante"));

        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => useCase.ExecutarAsync(
            dados.Chamado.Id,
            new TransferirGrupoTecnicoChamadoRequest { GrupoTecnicoId = grupoDestino.Id }));

        Assert.Equal("Acesso administrativo negado.", ex.Message);
        Assert.Equal(grupoOrigem.Id, context.Chamados.Single(x => x.Id == dados.Chamado.Id).GrupoTecnicoId);
    }

    [Fact]
    public async Task TransferenciaLimpaResponsavelIndividual()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "TG3");
        var grupoOrigem = await CriarGrupoAsync(context, "Origem TG3");
        var grupoDestino = await CriarGrupoAsync(context, "Destino TG3");
        dados.Chamado.DefinirGrupoTecnico(grupoOrigem.Id, "teste");
        dados.Chamado.AtribuirResponsavel(dados.Atendente.Id, "teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.AdminContexto);

        await useCase.ExecutarAsync(dados.Chamado.Id, new TransferirGrupoTecnicoChamadoRequest { GrupoTecnicoId = grupoDestino.Id });

        Assert.Null(context.Chamados.Single().ResponsavelId);
    }

    [Fact]
    public async Task TransferenciaSemFilaDestinoLimpaFilaAtual()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "TG4");
        var grupoOrigem = await CriarGrupoAsync(context, "Origem TG4");
        var filaOrigem = await CriarFilaAsync(context, grupoOrigem, "Fila Origem TG4");
        var grupoDestino = await CriarGrupoAsync(context, "Destino TG4");
        dados.Chamado.DefinirGrupoTecnico(grupoOrigem.Id, "teste");
        dados.Chamado.DefinirFilaAtendimento(filaOrigem.Id, "teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.AdminContexto);

        await useCase.ExecutarAsync(dados.Chamado.Id, new TransferirGrupoTecnicoChamadoRequest { GrupoTecnicoId = grupoDestino.Id });

        var chamado = context.Chamados.Single();
        Assert.Equal(grupoDestino.Id, chamado.GrupoTecnicoId);
        Assert.Null(chamado.FilaAtendimentoId);
    }

    [Fact]
    public async Task TransferenciaDefineFilaValidaDoGrupoDestino()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "TG5");
        var grupoOrigem = await CriarGrupoAsync(context, "Origem TG5");
        var grupoDestino = await CriarGrupoAsync(context, "Destino TG5");
        var filaDestino = await CriarFilaAsync(context, grupoDestino, "Fila Destino TG5");
        dados.Chamado.DefinirGrupoTecnico(grupoOrigem.Id, "teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.AdminContexto);

        await useCase.ExecutarAsync(
            dados.Chamado.Id,
            new TransferirGrupoTecnicoChamadoRequest { GrupoTecnicoId = grupoDestino.Id, FilaAtendimentoId = filaDestino.Id });

        var chamado = context.Chamados.Single();
        Assert.Equal(grupoDestino.Id, chamado.GrupoTecnicoId);
        Assert.Equal(filaDestino.Id, chamado.FilaAtendimentoId);
    }

    [Fact]
    public async Task TransferenciaRejeitaFilaDeOutroGrupo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "TG6");
        var grupoDestino = await CriarGrupoAsync(context, "Destino TG6");
        var outroGrupo = await CriarGrupoAsync(context, "Outro TG6");
        var filaOutroGrupo = await CriarFilaAsync(context, outroGrupo, "Fila Outro TG6");
        var useCase = CriarUseCase(context, dados.AdminContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(
            dados.Chamado.Id,
            new TransferirGrupoTecnicoChamadoRequest { GrupoTecnicoId = grupoDestino.Id, FilaAtendimentoId = filaOutroGrupo.Id }));

        Assert.Equal("Fila de atendimento de destino nao pertence ao grupo tecnico informado.", ex.Message);
    }

    [Fact]
    public async Task TransferenciaRejeitaGrupoInativo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "TG7");
        var grupoDestino = await CriarGrupoAsync(context, "Destino TG7");
        grupoDestino.Inativar("teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.AdminContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(
            dados.Chamado.Id,
            new TransferirGrupoTecnicoChamadoRequest { GrupoTecnicoId = grupoDestino.Id }));

        Assert.Equal("Grupo tecnico de destino nao encontrado ou inativo.", ex.Message);
    }

    [Fact]
    public async Task TransferenciaRejeitaGrupoInexistente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "TG8");
        var grupoOrigem = await CriarGrupoAsync(context, "Origem TG8");
        dados.Chamado.DefinirGrupoTecnico(grupoOrigem.Id, "teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.AdminContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(
            dados.Chamado.Id,
            new TransferirGrupoTecnicoChamadoRequest { GrupoTecnicoId = Guid.NewGuid() }));

        Assert.Equal("Grupo tecnico de destino nao encontrado ou inativo.", ex.Message);
    }

    [Fact]
    public async Task TransferenciaRejeitaFilaInativa()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "TG14");
        var grupoOrigem = await CriarGrupoAsync(context, "Origem TG14");
        var grupoDestino = await CriarGrupoAsync(context, "Destino TG14");
        var filaDestino = await CriarFilaAsync(context, grupoDestino, "Fila Destino TG14");
        filaDestino.Inativar("teste");
        dados.Chamado.DefinirGrupoTecnico(grupoOrigem.Id, "teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.AdminContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(
            dados.Chamado.Id,
            new TransferirGrupoTecnicoChamadoRequest { GrupoTecnicoId = grupoDestino.Id, FilaAtendimentoId = filaDestino.Id }));

        Assert.Equal("Fila de atendimento de destino nao encontrada ou inativa.", ex.Message);
    }

    [Fact]
    public async Task TransferenciaRejeitaFilaInexistente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "TG17");
        var grupoOrigem = await CriarGrupoAsync(context, "Origem TG17");
        var grupoDestino = await CriarGrupoAsync(context, "Destino TG17");
        dados.Chamado.DefinirGrupoTecnico(grupoOrigem.Id, "teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.AdminContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(
            dados.Chamado.Id,
            new TransferirGrupoTecnicoChamadoRequest { GrupoTecnicoId = grupoDestino.Id, FilaAtendimentoId = Guid.NewGuid() }));

        Assert.Equal("Fila de atendimento de destino nao encontrada ou inativa.", ex.Message);
    }

    [Fact]
    public async Task TransferenciaDeGrupoRegistraHistoricoComOrigemEDestino()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "TG9");
        var grupoOrigem = await CriarGrupoAsync(context, "Origem TG9");
        var grupoDestino = await CriarGrupoAsync(context, "Destino TG9");
        dados.Chamado.DefinirGrupoTecnico(grupoOrigem.Id, "teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.AdminContexto);

        await useCase.ExecutarAsync(dados.Chamado.Id, new TransferirGrupoTecnicoChamadoRequest { GrupoTecnicoId = grupoDestino.Id });

        Assert.Contains(context.HistoricosChamado, x =>
            x.ChamadoId == dados.Chamado.Id &&
            x.Tipo == TipoHistoricoChamado.GrupoTecnicoTransferido &&
            x.Descricao.Contains("Origem TG9", StringComparison.Ordinal) &&
            x.Descricao.Contains("Destino TG9", StringComparison.Ordinal));
    }

    [Fact]
    public async Task TransferenciaComFilaDestinoRegistraEntradaNaFila()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "TG10");
        var grupoOrigem = await CriarGrupoAsync(context, "Origem TG10");
        var grupoDestino = await CriarGrupoAsync(context, "Destino TG10");
        var filaDestino = await CriarFilaAsync(context, grupoDestino, "Fila Destino TG10");
        dados.Chamado.DefinirGrupoTecnico(grupoOrigem.Id, "teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.AdminContexto);

        await useCase.ExecutarAsync(
            dados.Chamado.Id,
            new TransferirGrupoTecnicoChamadoRequest { GrupoTecnicoId = grupoDestino.Id, FilaAtendimentoId = filaDestino.Id });

        Assert.Contains(context.HistoricosChamado, x =>
            x.ChamadoId == dados.Chamado.Id &&
            x.Tipo == TipoHistoricoChamado.FilaAtendimentoDefinida &&
            x.Descricao.Contains("Fila Destino TG10", StringComparison.Ordinal));
    }

    [Fact]
    public async Task TransferenciaSemFilaDestinoRegistraSaidaDaFilaAnterior()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "TG11");
        var grupoOrigem = await CriarGrupoAsync(context, "Origem TG11");
        var filaOrigem = await CriarFilaAsync(context, grupoOrigem, "Fila Origem TG11");
        var grupoDestino = await CriarGrupoAsync(context, "Destino TG11");
        dados.Chamado.DefinirGrupoTecnico(grupoOrigem.Id, "teste");
        dados.Chamado.DefinirFilaAtendimento(filaOrigem.Id, "teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.AdminContexto);

        await useCase.ExecutarAsync(dados.Chamado.Id, new TransferirGrupoTecnicoChamadoRequest { GrupoTecnicoId = grupoDestino.Id });

        Assert.Contains(context.HistoricosChamado, x =>
            x.ChamadoId == dados.Chamado.Id &&
            x.Tipo == TipoHistoricoChamado.FilaAtendimentoRemovida &&
            x.Descricao.Contains("Fila Origem TG11", StringComparison.Ordinal));
    }

    [Fact]
    public async Task TransferenciaEntreFilasRegistraFilaAnteriorENovaFila()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "TG12");
        var grupoOrigem = await CriarGrupoAsync(context, "Origem TG12");
        var filaOrigem = await CriarFilaAsync(context, grupoOrigem, "Fila Origem TG12");
        var grupoDestino = await CriarGrupoAsync(context, "Destino TG12");
        var filaDestino = await CriarFilaAsync(context, grupoDestino, "Fila Destino TG12");
        dados.Chamado.DefinirGrupoTecnico(grupoOrigem.Id, "teste");
        dados.Chamado.DefinirFilaAtendimento(filaOrigem.Id, "teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.AdminContexto);

        await useCase.ExecutarAsync(
            dados.Chamado.Id,
            new TransferirGrupoTecnicoChamadoRequest { GrupoTecnicoId = grupoDestino.Id, FilaAtendimentoId = filaDestino.Id });

        Assert.Contains(context.HistoricosChamado, x =>
            x.ChamadoId == dados.Chamado.Id &&
            x.Tipo == TipoHistoricoChamado.FilaAtendimentoTransferida &&
            x.Descricao.Contains("Fila Origem TG12", StringComparison.Ordinal) &&
            x.Descricao.Contains("Fila Destino TG12", StringComparison.Ordinal));
    }

    [Fact]
    public async Task TransferenciaQueLimpaResponsavelRegistraHistoricoResponsavel()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "TG13");
        var grupoOrigem = await CriarGrupoAsync(context, "Origem TG13");
        var grupoDestino = await CriarGrupoAsync(context, "Destino TG13");
        dados.Chamado.DefinirGrupoTecnico(grupoOrigem.Id, "teste");
        dados.Chamado.AtribuirResponsavel(dados.Atendente.Id, "teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.AdminContexto);

        await useCase.ExecutarAsync(dados.Chamado.Id, new TransferirGrupoTecnicoChamadoRequest { GrupoTecnicoId = grupoDestino.Id });

        Assert.Contains(context.HistoricosChamado, x =>
            x.ChamadoId == dados.Chamado.Id &&
            x.Tipo == TipoHistoricoChamado.ResponsavelRemovidoPorTransferenciaGrupo &&
            x.Descricao.Contains("Atendente TG13", StringComparison.Ordinal));
    }

    [Fact]
    public async Task TransferenciaNaoAlteraSlaDoChamado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "TG15");
        var grupoOrigem = await CriarGrupoAsync(context, "Origem TG15");
        var grupoDestino = await CriarGrupoAsync(context, "Destino TG15");
        var dataInicio = new DateTime(2026, 6, 5, 10, 0, 0, DateTimeKind.Utc);
        var prazoResposta = dataInicio.AddHours(4);
        var prazoResolucao = dataInicio.AddHours(8);
        var sla = new ChamadoSla(dados.Chamado.Id, null, dados.Chamado.PrioridadeId, dataInicio, prazoResposta, prazoResolucao, true, false, null, "teste");
        dados.Chamado.DefinirGrupoTecnico(grupoOrigem.Id, "teste");
        context.ChamadosSla.Add(sla);
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.AdminContexto);

        await useCase.ExecutarAsync(dados.Chamado.Id, new TransferirGrupoTecnicoChamadoRequest { GrupoTecnicoId = grupoDestino.Id });

        var slaAtual = context.ChamadosSla.Single(x => x.ChamadoId == dados.Chamado.Id);
        Assert.Equal(dataInicio, slaAtual.DataInicio);
        Assert.Equal(prazoResposta, slaAtual.PrazoPrimeiraResposta);
        Assert.Equal(prazoResolucao, slaAtual.PrazoResolucao);
    }

    private static TransferirGrupoTecnicoChamadoUseCase CriarUseCase(SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context, UsuarioContextoAplicacao usuario)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<GrupoTecnico>(context),
            PortalUseCasesTestFactory.Repo<FilaAtendimento>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            new FakeUsuarioContextoAplicacaoService(usuario),
            PortalUseCasesTestFactory.Uow(context));

    private static async Task<(Chamado Chamado, Usuario Atendente, UsuarioContextoAplicacao AdminContexto)> SeedAsync(
        SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context,
        string sufixo)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, $"Admin {sufixo}", $"admin-{sufixo}@empresa.com", TipoPerfil.Administrador);
        var atendente = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, $"Atendente {sufixo}", $"atendente-{sufixo}@empresa.com", TipoPerfil.Atendente);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, $"Solicitante {sufixo}", $"sol-{sufixo}@empresa.com", TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, $"Categoria {sufixo}");
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.Aberto, null, sufixo);

        return (chamado, atendente, AdminUseCasesTestFactory.Contexto(admin, "Administrador"));
    }

    private static async Task<GrupoTecnico> CriarGrupoAsync(SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context, string nome)
    {
        var grupo = new GrupoTecnico(nome, "Grupo tecnico de teste", "teste");
        context.GruposTecnicos.Add(grupo);
        await context.SaveChangesAsync();
        return grupo;
    }

    private static async Task<FilaAtendimento> CriarFilaAsync(SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context, GrupoTecnico grupo, string nome)
    {
        var fila = new FilaAtendimento(grupo.Id, nome, "Fila de teste", "teste");
        context.FilasAtendimento.Add(fila);
        await context.SaveChangesAsync();
        return fila;
    }
}
