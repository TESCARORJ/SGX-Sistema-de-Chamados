using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class AssumirChamadoUseCaseTests
{
    [Fact]
    public async Task AtendenteAssumeChamadoSemResponsavel()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = new AssumirChamadoUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            SlaTestFactory.CriarService(context),
            new FakeUsuarioContextoAplicacaoService(dados.AtendenteContexto),
            PortalUseCasesTestFactory.Uow(context));

        var response = await useCase.ExecutarAsync(dados.Chamado.Id);

        var chamado = context.Chamados.Single();
        Assert.Equal(dados.Atendente.Id, chamado.ResponsavelId);
        Assert.Null(chamado.GrupoTecnicoId);
        Assert.Null(chamado.FilaAtendimentoId);
        Assert.Equal(dados.Chamado.Id, response.Id);
    }

    [Fact]
    public async Task CriaHistoricoAoAssumir()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = new AssumirChamadoUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            SlaTestFactory.CriarService(context),
            new FakeUsuarioContextoAplicacaoService(dados.AtendenteContexto),
            PortalUseCasesTestFactory.Uow(context));

        await useCase.ExecutarAsync(dados.Chamado.Id);

        Assert.Contains(context.HistoricosChamado, x => x.Tipo == TipoHistoricoChamado.ResponsavelAlterado);
    }

    [Fact]
    public async Task PreservaGrupoEFilaAoAssumirChamado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var (grupo, fila) = await CriarGrupoEFilaAsync(context);
        dados.Chamado.DefinirGrupoTecnico(grupo.Id, "teste");
        dados.Chamado.DefinirFilaAtendimento(fila.Id, "teste");
        await context.SaveChangesAsync();

        var useCase = new AssumirChamadoUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            SlaTestFactory.CriarService(context),
            new FakeUsuarioContextoAplicacaoService(dados.AtendenteContexto),
            PortalUseCasesTestFactory.Uow(context));

        await useCase.ExecutarAsync(dados.Chamado.Id);

        var chamado = context.Chamados.Single();
        Assert.Equal(dados.Atendente.Id, chamado.ResponsavelId);
        Assert.Equal(grupo.Id, chamado.GrupoTecnicoId);
        Assert.Equal(fila.Id, chamado.FilaAtendimentoId);
    }

    [Fact]
    public async Task BloqueiaSolicitante()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = new AssumirChamadoUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            SlaTestFactory.CriarService(context),
            new FakeUsuarioContextoAplicacaoService(dados.SolicitanteContexto),
            PortalUseCasesTestFactory.Uow(context));

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => useCase.ExecutarAsync(dados.Chamado.Id));
    }

    [Fact]
    public async Task BloqueiaAssumirQuandoChamadoAguardaAprovacao()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var aprovacao = new AprovacaoChamado(
            dados.Chamado.Id,
            TipoOrigemAprovacaoChamado.CatalogoServico,
            dados.Atendente.Id,
            dados.Atendente.Login,
            dados.Chamado.SolicitanteId,
            "Servico teste",
            "Aprovacao pendente");
        context.AprovacoesChamado.Add(aprovacao);
        await context.SaveChangesAsync();

        var useCase = new AssumirChamadoUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            SlaTestFactory.CriarService(context),
            new FakeUsuarioContextoAplicacaoService(dados.AtendenteContexto),
            PortalUseCasesTestFactory.Uow(context));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(dados.Chamado.Id));
        Assert.Equal("Este chamado possui aprovacao pendente e nao pode avancar enquanto a aprovacao bloqueante nao for decidida.", ex.Message);
    }

    private static async Task<(Chamado Chamado, Usuario Atendente, UsuarioContextoAplicacao AtendenteContexto, UsuarioContextoAplicacao SolicitanteContexto)> SeedAsync(SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context)
    {
        var atendente = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Atendente", "atendente@empresa.com", TipoPerfil.Atendente);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante", "sol@empresa.com", TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Aplicacoes");
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.Aberto, null, "AS1");

        return (
            chamado,
            atendente,
            AdminUseCasesTestFactory.Contexto(atendente, "Atendente"),
            AdminUseCasesTestFactory.Contexto(solicitante, "Solicitante"));
    }

    private static async Task<(GrupoTecnico Grupo, FilaAtendimento Fila)> CriarGrupoEFilaAsync(SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context)
    {
        var grupo = new GrupoTecnico("Service Desk Teste", "Grupo tecnico de teste", "teste");
        context.GruposTecnicos.Add(grupo);
        await context.SaveChangesAsync();

        var fila = new FilaAtendimento(grupo.Id, "Fila Service Desk Teste", "Fila de teste", "teste");
        context.FilasAtendimento.Add(fila);
        await context.SaveChangesAsync();

        return (grupo, fila);
    }
}

