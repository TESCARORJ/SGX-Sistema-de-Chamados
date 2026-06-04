using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class ChamadoRelacionamentoRemoverUseCaseTests
{
    [Fact]
    public async Task DeveInativarVinculoAtivoExistente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var relacionamento = new ChamadoRelacionamento(
            dados.ChamadoOrigem.Id,
            dados.ChamadoDestino.Id,
            TipoRelacionamentoChamadoEnum.Bloqueia,
            dados.AdminUsuario.Id,
            dados.AdminUsuario.Login);
        context.ChamadosRelacionamentos.Add(relacionamento);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var request = new RemoverChamadoRelacionamentoRequest
        {
            RelacionamentoId = relacionamento.Id,
            Motivo = "Remocao solicitada pelo gestor."
        };

        await useCase.RemoverAsync(request);

        var relNoBanco = await context.ChamadosRelacionamentos.FindAsync(relacionamento.Id);
        Assert.NotNull(relNoBanco);
        Assert.False(relNoBanco.Ativo);
    }

    [Fact]
    public async Task DeveBloquearCriacaoSeUsuarioNaoForAdminOuAtendenteNaAplicacao()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var solicitante = await context.Usuarios.FindAsync(dados.ChamadoOrigem.SolicitanteId);
        Assert.NotNull(solicitante);

        var useCase = CriarUseCase(context, AdminUseCasesTestFactory.Contexto(solicitante, "Solicitante"));
        var request = new CriarChamadoRelacionamentoRequest
        {
            ChamadoOrigemId = dados.ChamadoOrigem.Id,
            ChamadoDestinoId = dados.ChamadoDestino.Id,
            TipoRelacionamento = TipoRelacionamentoChamadoEnum.Relacionado
        };

        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => useCase.CriarAsync(request));
        Assert.Equal("Acesso administrativo negado.", ex.Message);

        Assert.Empty(context.ChamadosRelacionamentos);
    }

    [Fact]
    public async Task DeveBloquearRemocaoSeUsuarioNaoForAdminOuAtendenteNaAplicacao()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var solicitante = await context.Usuarios.FindAsync(dados.ChamadoOrigem.SolicitanteId);
        Assert.NotNull(solicitante);

        var relacionamento = new ChamadoRelacionamento(
            dados.ChamadoOrigem.Id,
            dados.ChamadoDestino.Id,
            TipoRelacionamentoChamadoEnum.Relacionado,
            dados.AdminUsuario.Id,
            dados.AdminUsuario.Login);
        context.ChamadosRelacionamentos.Add(relacionamento);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, AdminUseCasesTestFactory.Contexto(solicitante, "Solicitante"));
        var request = new RemoverChamadoRelacionamentoRequest
        {
            ChamadoId = dados.ChamadoOrigem.Id,
            RelacionamentoId = relacionamento.Id
        };

        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => useCase.RemoverAsync(request));
        Assert.Equal("Acesso administrativo negado.", ex.Message);

        var relNoBanco = await context.ChamadosRelacionamentos.FindAsync(relacionamento.Id);
        Assert.NotNull(relNoBanco);
        Assert.True(relNoBanco.Ativo);
    }

    [Fact]
    public async Task DeveBloquearListagemSeUsuarioNaoForAdminOuAtendenteNaAplicacao()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var solicitante = await context.Usuarios.FindAsync(dados.ChamadoOrigem.SolicitanteId);
        Assert.NotNull(solicitante);

        var relacionamento = new ChamadoRelacionamento(
            dados.ChamadoOrigem.Id,
            dados.ChamadoDestino.Id,
            TipoRelacionamentoChamadoEnum.Relacionado,
            dados.AdminUsuario.Id,
            dados.AdminUsuario.Login);
        context.ChamadosRelacionamentos.Add(relacionamento);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, AdminUseCasesTestFactory.Contexto(solicitante, "Solicitante"));

        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => useCase.ListarPorChamadoAsync(dados.ChamadoOrigem.Id));
        Assert.Equal("Acesso administrativo negado.", ex.Message);
    }

    [Fact]
    public async Task DeveBloquearObtencaoSeUsuarioNaoForAdminOuAtendenteNaAplicacao()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var solicitante = await context.Usuarios.FindAsync(dados.ChamadoOrigem.SolicitanteId);
        Assert.NotNull(solicitante);

        var relacionamento = new ChamadoRelacionamento(
            dados.ChamadoOrigem.Id,
            dados.ChamadoDestino.Id,
            TipoRelacionamentoChamadoEnum.Relacionado,
            dados.AdminUsuario.Id,
            dados.AdminUsuario.Login);
        context.ChamadosRelacionamentos.Add(relacionamento);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, AdminUseCasesTestFactory.Contexto(solicitante, "Solicitante"));

        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => useCase.ObterPorIdAsync(relacionamento.Id));
        Assert.Equal("Acesso administrativo negado.", ex.Message);
    }

    [Fact]
    public async Task DevePreencherRemovidoEm()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var relacionamento = new ChamadoRelacionamento(
            dados.ChamadoOrigem.Id,
            dados.ChamadoDestino.Id,
            TipoRelacionamentoChamadoEnum.Bloqueia,
            dados.AdminUsuario.Id,
            dados.AdminUsuario.Login);
        context.ChamadosRelacionamentos.Add(relacionamento);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var request = new RemoverChamadoRelacionamentoRequest
        {
            RelacionamentoId = relacionamento.Id
        };

        await useCase.RemoverAsync(request);

        var relNoBanco = await context.ChamadosRelacionamentos.FindAsync(relacionamento.Id);
        Assert.NotNull(relNoBanco);
        Assert.NotNull(relNoBanco.RemovidoEm);
        Assert.True(relNoBanco.RemovidoEm > DateTime.UtcNow.AddMinutes(-1));
    }

    [Fact]
    public async Task DevePreencherRemovidoPorUsuarioIdQuandoDisponivel()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var relacionamento = new ChamadoRelacionamento(
            dados.ChamadoOrigem.Id,
            dados.ChamadoDestino.Id,
            TipoRelacionamentoChamadoEnum.Bloqueia,
            dados.AdminUsuario.Id,
            dados.AdminUsuario.Login);
        context.ChamadosRelacionamentos.Add(relacionamento);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var request = new RemoverChamadoRelacionamentoRequest
        {
            RelacionamentoId = relacionamento.Id
        };

        await useCase.RemoverAsync(request);

        var relNoBanco = await context.ChamadosRelacionamentos.FindAsync(relacionamento.Id);
        Assert.NotNull(relNoBanco);
        Assert.Equal(dados.AdminUsuario.Id, relNoBanco.RemovidoPorUsuarioId);
    }

    [Fact]
    public async Task DevePreencherMotivoRemocaoQuandoInformado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var relacionamento = new ChamadoRelacionamento(
            dados.ChamadoOrigem.Id,
            dados.ChamadoDestino.Id,
            TipoRelacionamentoChamadoEnum.Bloqueia,
            dados.AdminUsuario.Id,
            dados.AdminUsuario.Login);
        context.ChamadosRelacionamentos.Add(relacionamento);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var request = new RemoverChamadoRelacionamentoRequest
        {
            RelacionamentoId = relacionamento.Id,
            Motivo = "Dependencia resolvida"
        };

        await useCase.RemoverAsync(request);

        var relNoBanco = await context.ChamadosRelacionamentos.FindAsync(relacionamento.Id);
        Assert.NotNull(relNoBanco);
        Assert.Equal("Dependencia resolvida", relNoBanco.MotivoRemocao);
    }

    [Fact]
    public async Task DeveRegistrarHistoricoNoChamadoOrigemEDestino()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var relacionamento = new ChamadoRelacionamento(
            dados.ChamadoOrigem.Id,
            dados.ChamadoDestino.Id,
            TipoRelacionamentoChamadoEnum.Bloqueia,
            dados.AdminUsuario.Id,
            dados.AdminUsuario.Login);
        context.ChamadosRelacionamentos.Add(relacionamento);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var request = new RemoverChamadoRelacionamentoRequest
        {
            RelacionamentoId = relacionamento.Id,
            Motivo = "Mudanca de prioridades"
        };

        await useCase.RemoverAsync(request);

        var historicosOrigem = await context.HistoricosChamado
            .Where(x => x.ChamadoId == dados.ChamadoOrigem.Id && x.Tipo == TipoHistoricoChamado.RelacionamentoRemovido)
            .ToListAsync();
        Assert.Single(historicosOrigem);

        var historicosDestino = await context.HistoricosChamado
            .Where(x => x.ChamadoId == dados.ChamadoDestino.Id && x.Tipo == TipoHistoricoChamado.RelacionamentoRemovidoRecebido)
            .ToListAsync();
        Assert.Single(historicosDestino);
    }

    [Fact]
    public async Task HistoricoDeveConterTipoDeVinculoERelacionamentoIdEMotivo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var relacionamento = new ChamadoRelacionamento(
            dados.ChamadoOrigem.Id,
            dados.ChamadoDestino.Id,
            TipoRelacionamentoChamadoEnum.Bloqueia,
            dados.AdminUsuario.Id,
            dados.AdminUsuario.Login);
        context.ChamadosRelacionamentos.Add(relacionamento);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var request = new RemoverChamadoRelacionamentoRequest
        {
            RelacionamentoId = relacionamento.Id,
            Motivo = "Mudanca de prioridades"
        };

        await useCase.RemoverAsync(request);

        var historicoOrigem = await context.HistoricosChamado
            .SingleAsync(x => x.ChamadoId == dados.ChamadoOrigem.Id && x.Tipo == TipoHistoricoChamado.RelacionamentoRemovido);

        Assert.Contains(relacionamento.Id.ToString(), historicoOrigem.Descricao, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(TipoRelacionamentoChamadoEnum.Bloqueia.ToString(), historicoOrigem.Descricao, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Motivo: Mudanca de prioridades", historicoOrigem.Descricao, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task NaoDeveRegistrarHistoricoSeORelacionamentoNaoExiste()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var request = new RemoverChamadoRelacionamentoRequest
        {
            RelacionamentoId = Guid.NewGuid(),
            Motivo = "Teste"
        };

        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => useCase.RemoverAsync(request));
        Assert.Equal("Relacionamento nao encontrado.", ex.Message);

        var countHistoricos = await context.HistoricosChamado.CountAsync();
        Assert.Equal(0, countHistoricos);
    }

    [Fact]
    public async Task NaoDeveRegistrarHistoricoSeORelacionamentoJaEstiverInativo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var relacionamento = new ChamadoRelacionamento(
            dados.ChamadoOrigem.Id,
            dados.ChamadoDestino.Id,
            TipoRelacionamentoChamadoEnum.Bloqueia,
            dados.AdminUsuario.Id,
            dados.AdminUsuario.Login);
        relacionamento.Inativar(dados.AdminUsuario.Id, dados.AdminUsuario.Login, "Inativacao inicial");
        context.ChamadosRelacionamentos.Add(relacionamento);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var request = new RemoverChamadoRelacionamentoRequest
        {
            RelacionamentoId = relacionamento.Id,
            Motivo = "Segunda inativacao"
        };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.RemoverAsync(request));
        Assert.Equal("Relacionamento ja esta inativo.", ex.Message);

        var countHistoricos = await context.HistoricosChamado.CountAsync();
        Assert.Equal(0, countHistoricos);
    }

    [Fact]
    public async Task NaoDeveExcluirFisicamenteORelacionamento()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var relacionamento = new ChamadoRelacionamento(
            dados.ChamadoOrigem.Id,
            dados.ChamadoDestino.Id,
            TipoRelacionamentoChamadoEnum.Bloqueia,
            dados.AdminUsuario.Id,
            dados.AdminUsuario.Login);
        context.ChamadosRelacionamentos.Add(relacionamento);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var request = new RemoverChamadoRelacionamentoRequest
        {
            RelacionamentoId = relacionamento.Id
        };

        await useCase.RemoverAsync(request);

        var relNoBanco = await context.ChamadosRelacionamentos.FindAsync(relacionamento.Id);
        Assert.NotNull(relNoBanco);
    }

    [Fact]
    public async Task NaoDeveRemoverQuandoRelacionamentoNaoPertenceAoChamadoInformado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var outroChamado = await AdminUseCasesTestFactory.CriarChamadoAsync(
            context,
            dados.ChamadoOrigem.Solicitante,
            dados.ChamadoOrigem.Categoria,
            StatusChamadoEnum.Aberto,
            sufixoCodigo: "OUTR-REM");

        var relacionamento = new ChamadoRelacionamento(
            dados.ChamadoOrigem.Id,
            dados.ChamadoDestino.Id,
            TipoRelacionamentoChamadoEnum.Bloqueia,
            dados.AdminUsuario.Id,
            dados.AdminUsuario.Login);
        context.ChamadosRelacionamentos.Add(relacionamento);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var request = new RemoverChamadoRelacionamentoRequest
        {
            ChamadoId = outroChamado.Id,
            RelacionamentoId = relacionamento.Id,
            Motivo = "Rota de chamado divergente"
        };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.RemoverAsync(request));
        Assert.Equal("Relacionamento nao pertence ao chamado informado.", ex.Message);

        var relNoBanco = await context.ChamadosRelacionamentos.FindAsync(relacionamento.Id);
        Assert.NotNull(relNoBanco);
        Assert.True(relNoBanco.Ativo);
    }

    [Fact]
    public async Task DeveListarRelacionamentosAtivosOndeChamadoEOrigem()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var relacionamento = new ChamadoRelacionamento(
            dados.ChamadoOrigem.Id,
            dados.ChamadoDestino.Id,
            TipoRelacionamentoChamadoEnum.Relacionado,
            dados.AdminUsuario.Id,
            dados.AdminUsuario.Login);
        context.ChamadosRelacionamentos.Add(relacionamento);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var lista = await useCase.ListarPorChamadoAsync(dados.ChamadoOrigem.Id);

        Assert.Single(lista);
        Assert.Equal(relacionamento.Id, lista[0].Id);
        Assert.Equal(dados.ChamadoOrigem.Id, lista[0].ChamadoOrigemId);
        Assert.Equal(dados.ChamadoOrigem.Codigo, lista[0].ChamadoOrigemCodigo);
        Assert.Equal(dados.ChamadoDestino.Codigo, lista[0].ChamadoDestinoCodigo);
        Assert.Equal("Relacionado", lista[0].TipoRelacionamentoDescricao);
    }

    [Fact]
    public async Task DeveListarRelacionamentosAtivosOndeChamadoEDestino()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var relacionamento = new ChamadoRelacionamento(
            dados.ChamadoOrigem.Id,
            dados.ChamadoDestino.Id,
            TipoRelacionamentoChamadoEnum.Bloqueia,
            dados.AdminUsuario.Id,
            dados.AdminUsuario.Login);
        context.ChamadosRelacionamentos.Add(relacionamento);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var lista = await useCase.ListarPorChamadoAsync(dados.ChamadoDestino.Id);

        Assert.Single(lista);
        Assert.Equal(relacionamento.Id, lista[0].Id);
        Assert.Equal(dados.ChamadoDestino.Id, lista[0].ChamadoDestinoId);
    }

    [Fact]
    public async Task DeveIgnorarRelacionamentosInativosPorPadraoNaListagem()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var relacionamento = new ChamadoRelacionamento(
            dados.ChamadoOrigem.Id,
            dados.ChamadoDestino.Id,
            TipoRelacionamentoChamadoEnum.Relacionado,
            dados.AdminUsuario.Id,
            dados.AdminUsuario.Login);
        relacionamento.Inativar(dados.AdminUsuario.Id, dados.AdminUsuario.Login, "Removido");
        context.ChamadosRelacionamentos.Add(relacionamento);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var lista = await useCase.ListarPorChamadoAsync(dados.ChamadoOrigem.Id);

        Assert.Empty(lista);
    }

    [Fact]
    public async Task DeveIncluirRelacionamentosInativosNaListagemQuandoSolicitado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var relacionamento = new ChamadoRelacionamento(
            dados.ChamadoOrigem.Id,
            dados.ChamadoDestino.Id,
            TipoRelacionamentoChamadoEnum.Relacionado,
            dados.AdminUsuario.Id,
            dados.AdminUsuario.Login);
        relacionamento.Inativar(dados.AdminUsuario.Id, dados.AdminUsuario.Login, "Removido");
        context.ChamadosRelacionamentos.Add(relacionamento);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var lista = await useCase.ListarPorChamadoAsync(dados.ChamadoOrigem.Id, incluirInativos: true);

        Assert.Single(lista);
        Assert.False(lista[0].Ativo);
        Assert.Equal("Removido", lista[0].MotivoRemocao);
    }

    [Fact]
    public async Task DeveIgnorarRelacionamentosDeOutrosChamadosNaListagem()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var outroChamado = await AdminUseCasesTestFactory.CriarChamadoAsync(
            context,
            dados.ChamadoOrigem.Solicitante,
            dados.ChamadoOrigem.Categoria,
            StatusChamadoEnum.Aberto,
            sufixoCodigo: "OUTRO");

        var relacionamento = new ChamadoRelacionamento(
            dados.ChamadoOrigem.Id,
            dados.ChamadoDestino.Id,
            TipoRelacionamentoChamadoEnum.Relacionado,
            dados.AdminUsuario.Id,
            dados.AdminUsuario.Login);
        context.ChamadosRelacionamentos.Add(relacionamento);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var lista = await useCase.ListarPorChamadoAsync(outroChamado.Id);

        Assert.Empty(lista);
    }

    [Fact]
    public async Task DeveLancarErroAoListarSeChamadoNaoExistir()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);

        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => useCase.ListarPorChamadoAsync(Guid.NewGuid()));
        Assert.Equal("Chamado nao encontrado.", ex.Message);
    }

    [Fact]
    public async Task DeveObterRelacionamentoPorIdComSucesso()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var relacionamento = new ChamadoRelacionamento(
            dados.ChamadoOrigem.Id,
            dados.ChamadoDestino.Id,
            TipoRelacionamentoChamadoEnum.Relacionado,
            dados.AdminUsuario.Id,
            dados.AdminUsuario.Login,
            "Justificativa de teste");
        context.ChamadosRelacionamentos.Add(relacionamento);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var response = await useCase.ObterPorIdAsync(relacionamento.Id);

        Assert.NotNull(response);
        Assert.Equal(relacionamento.Id, response.Id);
        Assert.Equal(dados.ChamadoOrigem.Codigo, response.ChamadoOrigemCodigo);
        Assert.Equal(dados.ChamadoDestino.Codigo, response.ChamadoDestinoCodigo);
        Assert.Equal("Justificativa de teste", response.Justificativa);
    }

    [Fact]
    public async Task DeveLancarErroAoObterPorIdSeNaoExistir()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);

        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => useCase.ObterPorIdAsync(Guid.NewGuid()));
        Assert.Equal("Relacionamento nao encontrado.", ex.Message);
    }

    private static RelacionamentosChamadoUseCases CriarUseCase(
        SGXSistemaChamadoDbContext context,
        UsuarioContextoAplicacao contexto)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<ChamadoRelacionamento>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            new FakeUsuarioContextoAplicacaoService(contexto),
            PortalUseCasesTestFactory.Uow(context));

    private static async Task<(Chamado ChamadoOrigem, Chamado ChamadoDestino, Usuario AdminUsuario, UsuarioContextoAplicacao ContextoAdmin)> SeedAsync(SGXSistemaChamadoDbContext context)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            context,
            "Admin Relacionamento Test Consol",
            $"admin.rel.test.consol.{Guid.NewGuid():N}@empresa.com",
            TipoPerfil.Administrador);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            context,
            "Solicitante Relacionamento Test Consol",
            $"sol.rel.test.consol.{Guid.NewGuid():N}@empresa.com",
            TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, $"Categoria Rel Test Consol {Guid.NewGuid():N}");

        var chamadoOrigem = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.Aberto, sufixoCodigo: "RELA-TC");
        var chamadoDestino = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.Aberto, sufixoCodigo: "RELB-TC");

        return (chamadoOrigem, chamadoDestino, admin, AdminUseCasesTestFactory.Contexto(admin, "Administrador"));
    }
}
