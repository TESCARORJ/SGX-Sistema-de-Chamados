using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Services;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class CriarChamadoDerivadoAdminUseCaseTests
{
    [Fact]
    public async Task DeveCriarChamadoDerivadoAPartirDeOrigemExistente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var response = await useCase.ExecutarAsync(dados.ChamadoOrigem.Id, CriarRequestValido(dados));

        Assert.Equal(dados.ChamadoOrigem.Id, response.ChamadoOrigemId);
        Assert.Equal(dados.ChamadoOrigem.Codigo, response.ChamadoOrigemCodigo);
        Assert.NotEqual(Guid.Empty, response.ChamadoDerivadoId);
        Assert.Equal("SGX-2026-000001", response.ChamadoDerivadoCodigo);
        Assert.Equal("Chamado derivado para investigacao", response.Titulo);
        Assert.Equal("Aberto", response.Status);
    }

    [Fact]
    public async Task DeveCriarChamadoDerivadoComoChamadoNormalComStatusAberto()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var response = await useCase.ExecutarAsync(dados.ChamadoOrigem.Id, CriarRequestValido(dados));

        var derivado = context.Chamados.Single(x => x.Id == response.ChamadoDerivadoId);
        var statusAberto = context.StatusChamado.Single(x => x.Codigo == StatusChamadoEnum.Aberto);

        Assert.Equal(statusAberto.Id, derivado.StatusId);
        Assert.Equal(OrigemChamado.Admin, derivado.Origem);
        Assert.Equal(dados.Solicitante.Id, derivado.SolicitanteId);
        Assert.Equal(dados.Categoria.Id, derivado.CategoriaId);
        Assert.Equal(NaturezaChamadoEnum.Problema, derivado.NaturezaChamado);
        Assert.NotNull(context.ChamadosSla.SingleOrDefault(x => x.ChamadoId == derivado.Id));
    }

    [Fact]
    public async Task DeveManterStatusDoChamadoOrigemInalterado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var statusOrigemAntes = dados.ChamadoOrigem.StatusId;
        var encerradoOrigemAntes = dados.ChamadoOrigem.EncerradoEm;

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        await useCase.ExecutarAsync(dados.ChamadoOrigem.Id, CriarRequestValido(dados));

        var origem = context.Chamados.Single(x => x.Id == dados.ChamadoOrigem.Id);
        Assert.Equal(statusOrigemAntes, origem.StatusId);
        Assert.Equal(encerradoOrigemAntes, origem.EncerradoEm);
    }

    [Fact]
    public async Task DeveRegistrarHistoricoNoChamadoOrigemENoChamadoDerivado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var response = await useCase.ExecutarAsync(dados.ChamadoOrigem.Id, CriarRequestValido(dados));

        var historicoOrigem = Assert.Single(context.HistoricosChamado, x =>
            x.ChamadoId == dados.ChamadoOrigem.Id &&
            x.Tipo == TipoHistoricoChamado.ChamadoDerivadoCriado);
        var historicoDerivado = Assert.Single(context.HistoricosChamado, x =>
            x.ChamadoId == response.ChamadoDerivadoId &&
            x.Tipo == TipoHistoricoChamado.CriadoAPartirDeChamado);

        Assert.Contains($"#{response.ChamadoDerivadoCodigo}", historicoOrigem.Descricao);
        Assert.Contains("Investigacao separada", historicoOrigem.Descricao);
        Assert.Contains($"#{dados.ChamadoOrigem.Codigo}", historicoDerivado.Descricao);
        Assert.Contains("Investigacao separada", historicoDerivado.Descricao);
    }

    [Fact]
    public async Task DeveBloquearQuandoChamadoOrigemNaoExiste()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            useCase.ExecutarAsync(Guid.NewGuid(), CriarRequestValido(dados)));

        Assert.Equal("Chamado origem nao encontrado.", ex.Message);
        Assert.DoesNotContain(context.HistoricosChamado, x =>
            x.Tipo is TipoHistoricoChamado.ChamadoDerivadoCriado or TipoHistoricoChamado.CriadoAPartirDeChamado);
        Assert.Empty(context.ChamadosRelacionamentos);
    }

    [Fact]
    public async Task DeveBloquearQuandoUsuarioNaoTemPermissaoAdministrativa()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, AdminUseCasesTestFactory.Contexto(dados.Solicitante, "Solicitante"));
        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            useCase.ExecutarAsync(dados.ChamadoOrigem.Id, CriarRequestValido(dados)));

        Assert.Equal("Acesso administrativo negado.", ex.Message);
        Assert.DoesNotContain(context.HistoricosChamado, x =>
            x.Tipo is TipoHistoricoChamado.ChamadoDerivadoCriado or TipoHistoricoChamado.CriadoAPartirDeChamado);
        Assert.Empty(context.ChamadosRelacionamentos);
    }

    [Fact]
    public async Task DeveBloquearDadosObrigatoriosInvalidosSemRegistrarHistorico()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            useCase.ExecutarAsync(dados.ChamadoOrigem.Id, new CriarChamadoDerivadoAdminRequest
            {
                Titulo = string.Empty,
                Descricao = "Descricao detalhada do chamado derivado para investigacao separada",
                SolicitanteId = dados.Solicitante.Id,
                CategoriaId = dados.Categoria.Id,
                PrioridadeId = dados.Prioridade.Id,
                NaturezaChamado = NaturezaChamadoEnum.Problema,
                ImpactoChamado = ImpactoChamadoEnum.Medio,
                UrgenciaChamado = UrgenciaChamadoEnum.Media
            }));

        Assert.Equal("Titulo obrigatorio.", ex.Message);
        Assert.DoesNotContain(context.HistoricosChamado, x =>
            x.Tipo is TipoHistoricoChamado.ChamadoDerivadoCriado or TipoHistoricoChamado.CriadoAPartirDeChamado);
        Assert.Empty(context.ChamadosRelacionamentos);
    }

    [Fact]
    public async Task DeveCriarVinculoAutomaticoOriginaEntreOrigemEDerivado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var response = await useCase.ExecutarAsync(dados.ChamadoOrigem.Id, CriarRequestValido(dados));

        var relacionamento = Assert.Single(context.ChamadosRelacionamentos);
        Assert.Equal(response.RelacionamentoId, relacionamento.Id);
        Assert.Equal(TipoRelacionamentoChamadoEnum.Origina, response.TipoRelacionamento);
        Assert.Equal(TipoRelacionamentoChamadoEnum.Origina, relacionamento.TipoRelacionamento);
        Assert.Equal(dados.ChamadoOrigem.Id, relacionamento.ChamadoOrigemId);
        Assert.Equal(response.ChamadoDerivadoId, relacionamento.ChamadoDestinoId);
        Assert.True(relacionamento.Ativo);
        Assert.Contains("Vinculo automatico criado a partir do fluxo de chamado derivado", relacionamento.Justificativa);
        Assert.Contains("Investigacao separada", relacionamento.Justificativa);
    }

    [Fact]
    public async Task VinculoAutomaticoDeveAparecerNaListagemDaOrigemEDoDerivado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var response = await useCase.ExecutarAsync(dados.ChamadoOrigem.Id, CriarRequestValido(dados));

        var relacionamentosUseCase = CriarRelacionamentosUseCase(context, dados.ContextoAdmin);
        var relacionamentosOrigem = await relacionamentosUseCase.ListarPorChamadoAsync(dados.ChamadoOrigem.Id);
        var relacionamentosDerivado = await relacionamentosUseCase.ListarPorChamadoAsync(response.ChamadoDerivadoId);

        var relacionamentoOrigem = Assert.Single(relacionamentosOrigem);
        var relacionamentoDerivado = Assert.Single(relacionamentosDerivado);
        Assert.Equal(response.RelacionamentoId, relacionamentoOrigem.Id);
        Assert.Equal(response.RelacionamentoId, relacionamentoDerivado.Id);
        Assert.Equal(dados.ChamadoOrigem.Id, relacionamentoOrigem.ChamadoOrigemId);
        Assert.Equal(response.ChamadoDerivadoId, relacionamentoDerivado.ChamadoDestinoId);
    }

    [Fact]
    public async Task VinculoAutomaticoOriginaNaoDeveGerarDependenciaOuBloqueio()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var response = await useCase.ExecutarAsync(dados.ChamadoOrigem.Id, CriarRequestValido(dados));

        var relacionamentosUseCase = CriarRelacionamentosUseCase(context, dados.ContextoAdmin);
        var bloqueioOrigem = await relacionamentosUseCase.ObterBloqueioPorChamadoAsync(dados.ChamadoOrigem.Id);
        var bloqueioDerivado = await relacionamentosUseCase.ObterBloqueioPorChamadoAsync(response.ChamadoDerivadoId);

        Assert.Empty(await relacionamentosUseCase.ListarDependenciasPorChamadoAsync(dados.ChamadoOrigem.Id));
        Assert.Empty(await relacionamentosUseCase.ListarDependenciasPorChamadoAsync(response.ChamadoDerivadoId));
        Assert.False(await relacionamentosUseCase.PossuiDependenciasAtivasAsync(dados.ChamadoOrigem.Id));
        Assert.False(await relacionamentosUseCase.PossuiDependenciasAtivasAsync(response.ChamadoDerivadoId));
        Assert.False(bloqueioOrigem.EstaBloqueado);
        Assert.False(bloqueioOrigem.BloqueiaOutrosChamados);
        Assert.False(bloqueioDerivado.EstaBloqueado);
        Assert.False(bloqueioDerivado.BloqueiaOutrosChamados);
    }

    [Fact]
    public async Task VinculoAutomaticoOriginaNaoDeveImpedirEncerramentoDoDerivado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var criarDerivadoUseCase = CriarUseCase(context, dados.ContextoAdmin);
        var responseDerivado = await criarDerivadoUseCase.ExecutarAsync(dados.ChamadoOrigem.Id, CriarRequestValido(dados));

        var encerrarUseCase = CriarEncerrarUseCase(context, dados.ContextoAdmin);
        var responseEncerramento = await encerrarUseCase.ExecutarAsync(
            responseDerivado.ChamadoDerivadoId,
            new EncerrarChamadoRequest { Solucao = "Investigacao derivada concluida" });

        Assert.Equal("Encerrado", responseEncerramento.Status);
    }

    [Fact]
    public async Task DeveManterHistoricosDeDerivacaoERegistrarHistoricosDoVinculoFormal()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var response = await useCase.ExecutarAsync(dados.ChamadoOrigem.Id, CriarRequestValido(dados));

        Assert.Contains(context.HistoricosChamado, x =>
            x.ChamadoId == dados.ChamadoOrigem.Id &&
            x.Tipo == TipoHistoricoChamado.ChamadoDerivadoCriado);
        Assert.Contains(context.HistoricosChamado, x =>
            x.ChamadoId == response.ChamadoDerivadoId &&
            x.Tipo == TipoHistoricoChamado.CriadoAPartirDeChamado);
        Assert.Contains(context.HistoricosChamado, x =>
            x.ChamadoId == dados.ChamadoOrigem.Id &&
            x.Tipo == TipoHistoricoChamado.RelacionamentoCriado &&
            x.Descricao.Contains(response.RelacionamentoId!.Value.ToString()));
        Assert.Contains(context.HistoricosChamado, x =>
            x.ChamadoId == response.ChamadoDerivadoId &&
            x.Tipo == TipoHistoricoChamado.RelacionamentoRecebido &&
            x.Descricao.Contains(response.RelacionamentoId!.Value.ToString()));
    }

    private static CriarChamadoDerivadoAdminRequest CriarRequestValido(DadosDerivado dados)
        => new()
        {
            Titulo = "Chamado derivado para investigacao",
            Descricao = "Descricao detalhada do chamado derivado para investigacao separada",
            SolicitanteId = dados.Solicitante.Id,
            CategoriaId = dados.Categoria.Id,
            PrioridadeId = dados.Prioridade.Id,
            NaturezaChamado = NaturezaChamadoEnum.Problema,
            ImpactoChamado = ImpactoChamadoEnum.Medio,
            UrgenciaChamado = UrgenciaChamadoEnum.Media,
            JustificativaDerivacao = "Investigacao separada"
        };

    private static CriarChamadoDerivadoAdminUseCase CriarUseCase(
        SGXSistemaChamadoDbContext context,
        UsuarioContextoAplicacao contexto)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<Usuario>(context),
            PortalUseCasesTestFactory.Repo<CategoriaChamado>(context),
            PortalUseCasesTestFactory.Repo<SubcategoriaChamado>(context),
            PortalUseCasesTestFactory.Repo<PrioridadeChamado>(context),
            PortalUseCasesTestFactory.Repo<TipoSolicitacao>(context),
            PortalUseCasesTestFactory.Repo<LocalUnidade>(context),
            PortalUseCasesTestFactory.Repo<Departamento>(context),
            PortalUseCasesTestFactory.Repo<StatusChamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            SlaTestFactory.CriarService(context),
            new FakeCodigoChamadoService(),
            new PrioridadeChamadoMatrizService(PortalUseCasesTestFactory.Repo<PrioridadeChamado>(context)),
            new CamposObrigatoriosChamadoService(),
            new FakeUsuarioContextoAplicacaoService(contexto),
            CriarRelacionamentosUseCase(context, contexto),
            PortalUseCasesTestFactory.Uow(context));

    private static RelacionamentosChamadoUseCases CriarRelacionamentosUseCase(
        SGXSistemaChamadoDbContext context,
        UsuarioContextoAplicacao contexto)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<ChamadoRelacionamento>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            new FakeUsuarioContextoAplicacaoService(contexto),
            PortalUseCasesTestFactory.Uow(context));

    private static EncerrarChamadoUseCase CriarEncerrarUseCase(
        SGXSistemaChamadoDbContext context,
        UsuarioContextoAplicacao contexto)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<StatusChamado>(context),
            PortalUseCasesTestFactory.Repo<ComentarioChamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            new FluxoStatusChamadoService(),
            new AcoesChamadoService(new FluxoStatusChamadoService()),
            CriarRelacionamentosUseCase(context, contexto),
            CriarAprovacoesUseCase(context, contexto),
            SlaTestFactory.CriarService(context),
            new FakeUsuarioContextoAplicacaoService(contexto),
            PortalUseCasesTestFactory.Uow(context));

    private static ChamadoAprovacoesUseCases CriarAprovacoesUseCase(
        SGXSistemaChamadoDbContext context,
        UsuarioContextoAplicacao contexto)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<AprovacaoChamado>(context),
            PortalUseCasesTestFactory.Repo<Usuario>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            new FakeUsuarioContextoAplicacaoService(contexto),
            PortalUseCasesTestFactory.Uow(context));

    private static async Task<DadosDerivado> SeedAsync(SGXSistemaChamadoDbContext context)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            context,
            "Admin Derivado",
            $"admin.derivado.{Guid.NewGuid():N}@empresa.com",
            TipoPerfil.Administrador);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            context,
            "Solicitante Derivado",
            $"sol.derivado.{Guid.NewGuid():N}@empresa.com",
            TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, $"Categoria Derivado {Guid.NewGuid():N}");
        var prioridade = context.PrioridadesChamado.First();
        var chamadoOrigem = await AdminUseCasesTestFactory.CriarChamadoAsync(
            context,
            solicitante,
            categoria,
            StatusChamadoEnum.EmAtendimento,
            prioridade.Id,
            "DER-ORG");

        return new DadosDerivado(
            chamadoOrigem,
            admin,
            solicitante,
            categoria,
            prioridade,
            AdminUseCasesTestFactory.Contexto(admin, "Administrador"));
    }

    private sealed record DadosDerivado(
        Chamado ChamadoOrigem,
        Usuario Admin,
        Usuario Solicitante,
        CategoriaChamado Categoria,
        PrioridadeChamado Prioridade,
        UsuarioContextoAplicacao ContextoAdmin);
}
