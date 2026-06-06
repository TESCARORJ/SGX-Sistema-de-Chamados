using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class DirecionarChamadoGrupoTecnicoAdminUseCaseTests
{
    [Fact]
    public async Task DirecionaChamadoSemGrupoParaGrupoAtivo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "DG1");
        var grupo = await CriarGrupoAsync(context, "Service Desk DG1");
        var useCase = CriarUseCase(context, dados.AdminContexto);

        await useCase.ExecutarAsync(dados.Chamado.Id, new DirecionarChamadoGrupoTecnicoRequest { GrupoTecnicoId = grupo.Id });

        var chamado = context.Chamados.Single();
        Assert.Equal(grupo.Id, chamado.GrupoTecnicoId);
        Assert.Null(chamado.FilaAtendimentoId);
    }

    [Fact]
    public async Task RejeitaDirecionamentoPorSolicitante()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "DG15");
        var grupo = await CriarGrupoAsync(context, "Service Desk DG15");
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            context,
            "Solicitante DG15",
            "solicitante-dg15@empresa.com",
            TipoPerfil.Solicitante);
        var useCase = CriarUseCase(context, AdminUseCasesTestFactory.Contexto(solicitante, "Solicitante"));

        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => useCase.ExecutarAsync(
            dados.Chamado.Id,
            new DirecionarChamadoGrupoTecnicoRequest { GrupoTecnicoId = grupo.Id }));

        Assert.Equal("Acesso administrativo negado.", ex.Message);
        Assert.Null(context.Chamados.Single(x => x.Id == dados.Chamado.Id).GrupoTecnicoId);
    }

    [Fact]
    public async Task RejeitaChamadoInexistente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "DG16");
        var grupo = await CriarGrupoAsync(context, "Service Desk DG16");
        var useCase = CriarUseCase(context, dados.AdminContexto);

        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => useCase.ExecutarAsync(
            Guid.NewGuid(),
            new DirecionarChamadoGrupoTecnicoRequest { GrupoTecnicoId = grupo.Id }));

        Assert.Equal("Chamado nao encontrado.", ex.Message);
    }

    [Fact]
    public async Task DirecionaChamadoSemGrupoParaGrupoAtivoEFilaValida()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "DG2");
        var grupo = await CriarGrupoAsync(context, "Service Desk DG2");
        var fila = await CriarFilaAsync(context, grupo, "Fila Service Desk DG2");
        var useCase = CriarUseCase(context, dados.AdminContexto);

        await useCase.ExecutarAsync(
            dados.Chamado.Id,
            new DirecionarChamadoGrupoTecnicoRequest { GrupoTecnicoId = grupo.Id, FilaAtendimentoId = fila.Id });

        var chamado = context.Chamados.Single();
        Assert.Equal(grupo.Id, chamado.GrupoTecnicoId);
        Assert.Equal(fila.Id, chamado.FilaAtendimentoId);
    }

    [Fact]
    public async Task RejeitaGrupoInexistente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "DG3");
        var useCase = CriarUseCase(context, dados.AdminContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(
            dados.Chamado.Id,
            new DirecionarChamadoGrupoTecnicoRequest { GrupoTecnicoId = Guid.NewGuid() }));

        Assert.Equal("Grupo tecnico nao encontrado ou inativo.", ex.Message);
    }

    [Fact]
    public async Task RejeitaGrupoInativo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "DG4");
        var grupo = await CriarGrupoAsync(context, "Service Desk DG4");
        grupo.Inativar("teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.AdminContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(
            dados.Chamado.Id,
            new DirecionarChamadoGrupoTecnicoRequest { GrupoTecnicoId = grupo.Id }));

        Assert.Equal("Grupo tecnico nao encontrado ou inativo.", ex.Message);
    }

    [Fact]
    public async Task RejeitaFilaInexistente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "DG5");
        var grupo = await CriarGrupoAsync(context, "Service Desk DG5");
        var useCase = CriarUseCase(context, dados.AdminContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(
            dados.Chamado.Id,
            new DirecionarChamadoGrupoTecnicoRequest { GrupoTecnicoId = grupo.Id, FilaAtendimentoId = Guid.NewGuid() }));

        Assert.Equal("Fila de atendimento nao encontrada ou inativa.", ex.Message);
    }

    [Fact]
    public async Task RejeitaFilaInativa()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "DG6");
        var grupo = await CriarGrupoAsync(context, "Service Desk DG6");
        var fila = await CriarFilaAsync(context, grupo, "Fila Service Desk DG6");
        fila.Inativar("teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.AdminContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(
            dados.Chamado.Id,
            new DirecionarChamadoGrupoTecnicoRequest { GrupoTecnicoId = grupo.Id, FilaAtendimentoId = fila.Id }));

        Assert.Equal("Fila de atendimento nao encontrada ou inativa.", ex.Message);
    }

    [Fact]
    public async Task RejeitaFilaDeOutroGrupo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "DG7");
        var grupo = await CriarGrupoAsync(context, "Service Desk DG7");
        var outroGrupo = await CriarGrupoAsync(context, "Infra DG7");
        var filaOutroGrupo = await CriarFilaAsync(context, outroGrupo, "Fila Infra DG7");
        var useCase = CriarUseCase(context, dados.AdminContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(
            dados.Chamado.Id,
            new DirecionarChamadoGrupoTecnicoRequest { GrupoTecnicoId = grupo.Id, FilaAtendimentoId = filaOutroGrupo.Id }));

        Assert.Equal("Fila de atendimento nao pertence ao grupo tecnico informado.", ex.Message);
    }

    [Fact]
    public async Task PreservaResponsavelNoDirecionamentoInicial()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "DG8");
        var grupo = await CriarGrupoAsync(context, "Service Desk DG8");
        dados.Chamado.AtribuirResponsavel(dados.Atendente.Id, "teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.AdminContexto);

        await useCase.ExecutarAsync(dados.Chamado.Id, new DirecionarChamadoGrupoTecnicoRequest { GrupoTecnicoId = grupo.Id });

        Assert.Equal(dados.Atendente.Id, context.Chamados.Single().ResponsavelId);
    }

    [Fact]
    public async Task PreservaResponsavelNoDirecionamentoInicialComFila()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "DG18");
        var grupo = await CriarGrupoAsync(context, "Service Desk DG18");
        var fila = await CriarFilaAsync(context, grupo, "Fila Service Desk DG18");
        dados.Chamado.AtribuirResponsavel(dados.Atendente.Id, "teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.AdminContexto);

        await useCase.ExecutarAsync(
            dados.Chamado.Id,
            new DirecionarChamadoGrupoTecnicoRequest { GrupoTecnicoId = grupo.Id, FilaAtendimentoId = fila.Id });

        var chamado = context.Chamados.Single();
        Assert.Equal(dados.Atendente.Id, chamado.ResponsavelId);
        Assert.Equal(grupo.Id, chamado.GrupoTecnicoId);
        Assert.Equal(fila.Id, chamado.FilaAtendimentoId);
    }

    [Fact]
    public async Task RegistraHistoricoDeEntradaEmGrupo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "DG9");
        var grupo = await CriarGrupoAsync(context, "Service Desk DG9");
        var useCase = CriarUseCase(context, dados.AdminContexto);

        await useCase.ExecutarAsync(dados.Chamado.Id, new DirecionarChamadoGrupoTecnicoRequest { GrupoTecnicoId = grupo.Id, Observacao = "Triagem manual" });

        Assert.Contains(context.HistoricosChamado, x =>
            x.ChamadoId == dados.Chamado.Id &&
            x.Tipo == TipoHistoricoChamado.GrupoTecnicoDefinido &&
            x.Descricao.Contains("Service Desk DG9", StringComparison.Ordinal) &&
            x.Descricao.Contains("Triagem manual", StringComparison.Ordinal));
    }

    [Fact]
    public async Task RegistraHistoricoDeEntradaEmFilaQuandoInformada()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "DG10");
        var grupo = await CriarGrupoAsync(context, "Service Desk DG10");
        var fila = await CriarFilaAsync(context, grupo, "Fila Service Desk DG10");
        var useCase = CriarUseCase(context, dados.AdminContexto);

        await useCase.ExecutarAsync(
            dados.Chamado.Id,
            new DirecionarChamadoGrupoTecnicoRequest { GrupoTecnicoId = grupo.Id, FilaAtendimentoId = fila.Id });

        Assert.Contains(context.HistoricosChamado, x =>
            x.ChamadoId == dados.Chamado.Id &&
            x.Tipo == TipoHistoricoChamado.FilaAtendimentoDefinida &&
            x.Descricao.Contains("Fila Service Desk DG10", StringComparison.Ordinal));
    }

    [Fact]
    public async Task NaoAlteraSlaDoChamado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "DG11");
        var grupo = await CriarGrupoAsync(context, "Service Desk DG11");
        var dataInicio = new DateTime(2026, 6, 5, 10, 0, 0, DateTimeKind.Utc);
        var prazoResposta = dataInicio.AddHours(4);
        var prazoResolucao = dataInicio.AddHours(8);
        var sla = new ChamadoSla(dados.Chamado.Id, null, dados.Chamado.PrioridadeId, dataInicio, prazoResposta, prazoResolucao, true, false, null, "teste");
        context.ChamadosSla.Add(sla);
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.AdminContexto);

        await useCase.ExecutarAsync(dados.Chamado.Id, new DirecionarChamadoGrupoTecnicoRequest { GrupoTecnicoId = grupo.Id });

        var slaAtual = await context.ChamadosSla.SingleAsync(x => x.ChamadoId == dados.Chamado.Id);
        Assert.Equal(dataInicio, slaAtual.DataInicio);
        Assert.Equal(prazoResposta, slaAtual.PrazoPrimeiraResposta);
        Assert.Equal(prazoResolucao, slaAtual.PrazoResolucao);
    }

    [Fact]
    public async Task PreservaFilaAtualQuandoJaPertenceAoGrupoInformado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "DG12");
        var grupo = await CriarGrupoAsync(context, "Service Desk DG12");
        var fila = await CriarFilaAsync(context, grupo, "Fila Service Desk DG12");
        dados.Chamado.DirecionarGrupoTecnico(grupo.Id, fila.Id, "teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.AdminContexto);

        await useCase.ExecutarAsync(dados.Chamado.Id, new DirecionarChamadoGrupoTecnicoRequest { GrupoTecnicoId = grupo.Id });

        Assert.Equal(grupo.Id, context.Chamados.Single().GrupoTecnicoId);
        Assert.Equal(fila.Id, context.Chamados.Single().FilaAtendimentoId);
    }

    [Fact]
    public async Task LimpaFilaAtualQuandoPertenceAOutroGrupo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "DG13");
        var grupoDestino = await CriarGrupoAsync(context, "Service Desk DG13");
        var outroGrupo = await CriarGrupoAsync(context, "Infra DG13");
        var filaOutroGrupo = await CriarFilaAsync(context, outroGrupo, "Fila Infra DG13");
        dados.Chamado.DefinirFilaAtendimento(filaOutroGrupo.Id, "teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.AdminContexto);

        await useCase.ExecutarAsync(dados.Chamado.Id, new DirecionarChamadoGrupoTecnicoRequest { GrupoTecnicoId = grupoDestino.Id });

        var chamado = context.Chamados.Single();
        Assert.Equal(grupoDestino.Id, chamado.GrupoTecnicoId);
        Assert.Null(chamado.FilaAtendimentoId);
        Assert.Contains(context.HistoricosChamado, x => x.Tipo == TipoHistoricoChamado.FilaAtendimentoRemovida);
    }

    [Fact]
    public async Task AjustaFilaQuandoChamadoJaEstaNoMesmoGrupoERegistraTransferenciaDeFila()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "DG17");
        var grupo = await CriarGrupoAsync(context, "Service Desk DG17");
        var filaOrigem = await CriarFilaAsync(context, grupo, "Fila Origem DG17");
        var filaDestino = await CriarFilaAsync(context, grupo, "Fila Destino DG17");
        dados.Chamado.DirecionarGrupoTecnico(grupo.Id, filaOrigem.Id, "teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.AdminContexto);

        await useCase.ExecutarAsync(
            dados.Chamado.Id,
            new DirecionarChamadoGrupoTecnicoRequest { GrupoTecnicoId = grupo.Id, FilaAtendimentoId = filaDestino.Id });

        var chamado = context.Chamados.Single();
        Assert.Equal(grupo.Id, chamado.GrupoTecnicoId);
        Assert.Equal(filaDestino.Id, chamado.FilaAtendimentoId);
        Assert.Contains(context.HistoricosChamado, x =>
            x.ChamadoId == dados.Chamado.Id &&
            x.Tipo == TipoHistoricoChamado.FilaAtendimentoTransferida &&
            x.Descricao.Contains("Fila Origem DG17", StringComparison.Ordinal) &&
            x.Descricao.Contains("Fila Destino DG17", StringComparison.Ordinal));
    }

    [Fact]
    public async Task AjustaFilaNoMesmoGrupoPreservandoResponsavel()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "DG19");
        var grupo = await CriarGrupoAsync(context, "Service Desk DG19");
        var filaOrigem = await CriarFilaAsync(context, grupo, "Fila Origem DG19");
        var filaDestino = await CriarFilaAsync(context, grupo, "Fila Destino DG19");
        dados.Chamado.DirecionarGrupoTecnico(grupo.Id, filaOrigem.Id, "teste");
        dados.Chamado.AtribuirResponsavel(dados.Atendente.Id, "teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.AdminContexto);

        await useCase.ExecutarAsync(
            dados.Chamado.Id,
            new DirecionarChamadoGrupoTecnicoRequest { GrupoTecnicoId = grupo.Id, FilaAtendimentoId = filaDestino.Id });

        var chamado = context.Chamados.Single();
        Assert.Equal(dados.Atendente.Id, chamado.ResponsavelId);
        Assert.Equal(grupo.Id, chamado.GrupoTecnicoId);
        Assert.Equal(filaDestino.Id, chamado.FilaAtendimentoId);
    }

    [Fact]
    public async Task RejeitaDirecionamentoQuandoChamadoJaPossuiOutroGrupo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "DG14");
        var grupoOrigem = await CriarGrupoAsync(context, "Origem DG14");
        var grupoDestino = await CriarGrupoAsync(context, "Destino DG14");
        dados.Chamado.DefinirGrupoTecnico(grupoOrigem.Id, "teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.AdminContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(
            dados.Chamado.Id,
            new DirecionarChamadoGrupoTecnicoRequest { GrupoTecnicoId = grupoDestino.Id }));

        Assert.Equal("Chamado ja possui outro grupo tecnico. Use a transferencia entre grupos tecnicos para mudar o grupo responsavel.", ex.Message);
        Assert.Equal(grupoOrigem.Id, context.Chamados.Single().GrupoTecnicoId);
    }

    private static DirecionarChamadoGrupoTecnicoAdminUseCase CriarUseCase(SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context, UsuarioContextoAplicacao usuario)
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
