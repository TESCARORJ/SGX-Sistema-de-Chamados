using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class ChamadoTarefaUseCaseTests
{
    [Fact]
    public async Task DeveCriarTarefaVinculadaAChamadoExistente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var response = await useCase.CriarAsync(dados.Chamado.Id, CriarRequest(dados.Atendente.Id));

        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal(dados.Chamado.Id, response.ChamadoId);
        Assert.Equal("Validar acesso do usuario no AD", response.Titulo);
        Assert.Equal(StatusTarefaChamadoEnum.Pendente, response.Status);
        Assert.Equal("Pendente", response.StatusDescricao);
        Assert.Equal(dados.Atendente.Id, response.ResponsavelUsuarioId);
        Assert.Equal(dados.Atendente.Nome, response.ResponsavelNome);
        Assert.True(response.Ativo);
        Assert.Single(context.ChamadosTarefas);
    }

    [Fact]
    public async Task DeveBloquearCriacaoParaChamadoInexistente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);

        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            useCase.CriarAsync(Guid.NewGuid(), CriarRequest()));

        Assert.Equal("Chamado nao encontrado.", ex.Message);
        Assert.Empty(context.ChamadosTarefas);
    }

    [Fact]
    public async Task DeveBloquearCriacaoSemTitulo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);

        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            useCase.CriarAsync(dados.Chamado.Id, new CriarChamadoTarefaAdminRequest { Titulo = " " }));

        Assert.Contains("titulo da tarefa e obrigatorio", ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Empty(context.ChamadosTarefas);
    }

    [Fact]
    public async Task DeveListarApenasTarefasDoChamadoInformado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var outroChamado = await AdminUseCasesTestFactory.CriarChamadoAsync(
            context,
            dados.Solicitante,
            dados.Categoria,
            StatusChamadoEnum.Aberto,
            sufixoCodigo: "TAR-OUTRO");

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var tarefaDoChamado = await useCase.CriarAsync(dados.Chamado.Id, CriarRequest());
        await useCase.CriarAsync(outroChamado.Id, new CriarChamadoTarefaAdminRequest { Titulo = "Tarefa de outro chamado" });

        var tarefas = await useCase.ListarPorChamadoAsync(dados.Chamado.Id);

        var tarefa = Assert.Single(tarefas);
        Assert.Equal(tarefaDoChamado.Id, tarefa.Id);
        Assert.Equal(dados.Chamado.Id, tarefa.ChamadoId);
    }

    [Fact]
    public async Task DeveAlterarStatusParaEmAndamento()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var tarefa = await useCase.CriarAsync(dados.Chamado.Id, CriarRequest());

        var response = await useCase.AtualizarStatusAsync(
            dados.Chamado.Id,
            tarefa.Id,
            new AtualizarStatusChamadoTarefaAdminRequest { Status = StatusTarefaChamadoEnum.EmAndamento });

        Assert.Equal(StatusTarefaChamadoEnum.EmAndamento, response.Status);
        Assert.Null(response.ConcluidoEm);
    }

    [Fact]
    public async Task DeveConcluirTarefaERegistrarConcluidoEm()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var tarefa = await useCase.CriarAsync(dados.Chamado.Id, CriarRequest());

        var response = await useCase.AtualizarStatusAsync(
            dados.Chamado.Id,
            tarefa.Id,
            new AtualizarStatusChamadoTarefaAdminRequest { Status = StatusTarefaChamadoEnum.Concluida });

        Assert.Equal(StatusTarefaChamadoEnum.Concluida, response.Status);
        Assert.NotNull(response.ConcluidoEm);
        Assert.True(response.Ativo);
    }

    [Fact]
    public async Task DeveCancelarTarefaERegistrarMotivo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var tarefa = await useCase.CriarAsync(dados.Chamado.Id, CriarRequest());

        await useCase.CancelarAsync(
            dados.Chamado.Id,
            tarefa.Id,
            new CancelarChamadoTarefaAdminRequest { MotivoCancelamento = "Atividade deixou de ser necessaria" });

        var entidade = context.ChamadosTarefas.Single(x => x.Id == tarefa.Id);
        Assert.Equal(StatusTarefaChamadoEnum.Cancelada, entidade.Status);
        Assert.NotNull(entidade.CanceladoEm);
        Assert.Equal("Atividade deixou de ser necessaria", entidade.MotivoCancelamento);
        Assert.False(entidade.Ativo);
    }

    [Fact]
    public async Task TarefaCanceladaNaoDeveAparecerComoPendenteNaListagemPadrao()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var tarefa = await useCase.CriarAsync(dados.Chamado.Id, CriarRequest());
        await useCase.CancelarAsync(dados.Chamado.Id, tarefa.Id, new CancelarChamadoTarefaAdminRequest());

        var tarefasAtivas = await useCase.ListarPorChamadoAsync(dados.Chamado.Id);
        var tarefasComInativas = await useCase.ListarPorChamadoAsync(dados.Chamado.Id, incluirInativas: true);

        Assert.DoesNotContain(tarefasAtivas, x => x.Status == StatusTarefaChamadoEnum.Pendente);
        var cancelada = Assert.Single(tarefasComInativas);
        Assert.Equal(StatusTarefaChamadoEnum.Cancelada, cancelada.Status);
        Assert.False(cancelada.Ativo);
    }

    [Fact]
    public async Task DeveRegistrarHistoricosDaTarefa()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var tarefa = await useCase.CriarAsync(dados.Chamado.Id, CriarRequest());
        await useCase.AtualizarStatusAsync(
            dados.Chamado.Id,
            tarefa.Id,
            new AtualizarStatusChamadoTarefaAdminRequest { Status = StatusTarefaChamadoEnum.EmAndamento });
        await useCase.AtualizarStatusAsync(
            dados.Chamado.Id,
            tarefa.Id,
            new AtualizarStatusChamadoTarefaAdminRequest { Status = StatusTarefaChamadoEnum.Concluida });

        var tarefaCancelada = await useCase.CriarAsync(dados.Chamado.Id, new CriarChamadoTarefaAdminRequest { Titulo = "Coletar evidencia" });
        await useCase.CancelarAsync(
            dados.Chamado.Id,
            tarefaCancelada.Id,
            new CancelarChamadoTarefaAdminRequest { MotivoCancelamento = "Fornecedor respondeu por outro canal" });

        Assert.Contains(context.HistoricosChamado, x => x.ChamadoId == dados.Chamado.Id && x.Tipo == TipoHistoricoChamado.TarefaCriada);
        Assert.Contains(context.HistoricosChamado, x => x.ChamadoId == dados.Chamado.Id && x.Tipo == TipoHistoricoChamado.TarefaStatusAlterado);
        Assert.Contains(context.HistoricosChamado, x => x.ChamadoId == dados.Chamado.Id && x.Tipo == TipoHistoricoChamado.TarefaConcluida);
        Assert.Contains(context.HistoricosChamado, x =>
            x.ChamadoId == dados.Chamado.Id &&
            x.Tipo == TipoHistoricoChamado.TarefaCancelada &&
            x.Descricao.Contains("Fornecedor respondeu por outro canal"));
    }

    [Fact]
    public async Task DevePermitirOperacaoParaAtendente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, AdminUseCasesTestFactory.Contexto(dados.Atendente, "Atendente"));
        var response = await useCase.CriarAsync(dados.Chamado.Id, CriarRequest());

        Assert.NotEqual(Guid.Empty, response.Id);
    }

    [Fact]
    public async Task DeveBloquearOperacaoParaSolicitante()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, AdminUseCasesTestFactory.Contexto(dados.Solicitante, "Solicitante"));

        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            useCase.CriarAsync(dados.Chamado.Id, CriarRequest()));

        Assert.Equal("Acesso administrativo negado.", ex.Message);
    }

    private static CriarChamadoTarefaAdminRequest CriarRequest(Guid? responsavelId = null)
        => new()
        {
            Titulo = "Validar acesso do usuario no AD",
            Descricao = "Conferir grupos e permissoes antes da correcao.",
            ResponsavelUsuarioId = responsavelId,
            Prazo = DateTime.UtcNow.AddDays(2)
        };

    private static ChamadoTarefasUseCases CriarUseCase(
        SGXSistemaChamadoDbContext context,
        UsuarioContextoAplicacao usuario)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<ChamadoTarefa>(context),
            PortalUseCasesTestFactory.Repo<Usuario>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            new FakeUsuarioContextoAplicacaoService(usuario),
            PortalUseCasesTestFactory.Uow(context));

    private static async Task<DadosTarefa> SeedAsync(SGXSistemaChamadoDbContext context)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            context,
            "Admin Tarefa",
            $"admin.tarefa.{Guid.NewGuid():N}@empresa.com",
            TipoPerfil.Administrador);
        var atendente = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            context,
            "Atendente Tarefa",
            $"atendente.tarefa.{Guid.NewGuid():N}@empresa.com",
            TipoPerfil.Atendente);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            context,
            "Solicitante Tarefa",
            $"sol.tarefa.{Guid.NewGuid():N}@empresa.com",
            TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, $"Categoria Tarefa {Guid.NewGuid():N}");
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(
            context,
            solicitante,
            categoria,
            StatusChamadoEnum.Aberto,
            sufixoCodigo: "TAR-001");

        return new DadosTarefa(
            chamado,
            categoria,
            admin,
            atendente,
            solicitante,
            AdminUseCasesTestFactory.Contexto(admin, "Administrador"));
    }

    private sealed record DadosTarefa(
        Chamado Chamado,
        CategoriaChamado Categoria,
        Usuario Admin,
        Usuario Atendente,
        Usuario Solicitante,
        UsuarioContextoAplicacao ContextoAdmin);
}
