using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class ChamadoRelacionamentoDependenciaUseCaseTests
{
    [Fact]
    public async Task BloqueadoPorDeveNormalizarOrigemComoDependenteEDestinoComoBloqueador()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var relacionamento = AdicionarRelacionamento(
            context,
            dados.ChamadoA,
            dados.ChamadoB,
            dados.Admin,
            TipoRelacionamentoChamadoEnum.BloqueadoPor);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var dependencias = await useCase.ListarDependenciasPorChamadoAsync(dados.ChamadoA.Id);

        var dependencia = Assert.Single(dependencias);
        Assert.Equal(relacionamento.Id, dependencia.RelacionamentoId);
        Assert.Equal(dados.ChamadoA.Id, dependencia.ChamadoDependenteId);
        Assert.Equal(dados.ChamadoA.Codigo, dependencia.ChamadoDependenteCodigo);
        Assert.Equal(dados.ChamadoB.Id, dependencia.ChamadoBloqueadorId);
        Assert.Equal(dados.ChamadoB.Codigo, dependencia.ChamadoBloqueadorCodigo);
        Assert.Equal(TipoRelacionamentoChamadoEnum.BloqueadoPor, dependencia.TipoRelacionamentoOriginal);
        Assert.True(dependencia.ChamadoConsultadoEhDependente);
        Assert.False(dependencia.ChamadoConsultadoEhBloqueador);
    }

    [Fact]
    public async Task BloqueiaDeveNormalizarDestinoComoDependenteEOrigemComoBloqueador()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var relacionamento = AdicionarRelacionamento(
            context,
            dados.ChamadoA,
            dados.ChamadoB,
            dados.Admin,
            TipoRelacionamentoChamadoEnum.Bloqueia);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var dependencias = await useCase.ListarDependenciasPorChamadoAsync(dados.ChamadoB.Id);

        var dependencia = Assert.Single(dependencias);
        Assert.Equal(relacionamento.Id, dependencia.RelacionamentoId);
        Assert.Equal(dados.ChamadoB.Id, dependencia.ChamadoDependenteId);
        Assert.Equal(dados.ChamadoB.Codigo, dependencia.ChamadoDependenteCodigo);
        Assert.Equal(dados.ChamadoA.Id, dependencia.ChamadoBloqueadorId);
        Assert.Equal(dados.ChamadoA.Codigo, dependencia.ChamadoBloqueadorCodigo);
        Assert.Equal(TipoRelacionamentoChamadoEnum.Bloqueia, dependencia.TipoRelacionamentoOriginal);
        Assert.True(dependencia.ChamadoConsultadoEhDependente);
        Assert.False(dependencia.ChamadoConsultadoEhBloqueador);
    }

    [Fact]
    public async Task ChamadoQueBloqueiaOutrosDeveAparecerComoBloqueador()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        AdicionarRelacionamento(
            context,
            dados.ChamadoA,
            dados.ChamadoB,
            dados.Admin,
            TipoRelacionamentoChamadoEnum.Bloqueia);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var dependencias = await useCase.ListarDependenciasPorChamadoAsync(dados.ChamadoA.Id);

        var dependencia = Assert.Single(dependencias);
        Assert.Equal(dados.ChamadoB.Id, dependencia.ChamadoDependenteId);
        Assert.Equal(dados.ChamadoA.Id, dependencia.ChamadoBloqueadorId);
        Assert.False(dependencia.ChamadoConsultadoEhDependente);
        Assert.True(dependencia.ChamadoConsultadoEhBloqueador);
    }

    [Fact]
    public async Task VinculoInativoNaoDeveGerarDependenciaAtiva()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var relacionamento = AdicionarRelacionamento(
            context,
            dados.ChamadoA,
            dados.ChamadoB,
            dados.Admin,
            TipoRelacionamentoChamadoEnum.Bloqueia);
        relacionamento.Inativar(dados.Admin.Id, dados.Admin.Login, "Dependencia removida");
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var dependencias = await useCase.ListarDependenciasPorChamadoAsync(dados.ChamadoB.Id);

        Assert.Empty(dependencias);
        Assert.False(await useCase.EstaBloqueadoPorDependenciaAsync(dados.ChamadoB.Id));
    }

    [Theory]
    [InlineData(TipoRelacionamentoChamadoEnum.Relacionado)]
    [InlineData(TipoRelacionamentoChamadoEnum.Duplicado)]
    [InlineData(TipoRelacionamentoChamadoEnum.Pai)]
    [InlineData(TipoRelacionamentoChamadoEnum.Filho)]
    [InlineData(TipoRelacionamentoChamadoEnum.DerivadoDe)]
    [InlineData(TipoRelacionamentoChamadoEnum.Origina)]
    public async Task TiposNaoBloqueantesNaoDevemGerarDependencia(TipoRelacionamentoChamadoEnum tipo)
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        AdicionarRelacionamento(context, dados.ChamadoA, dados.ChamadoB, dados.Admin, tipo);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.ContextoAdmin);

        Assert.Empty(await useCase.ListarDependenciasPorChamadoAsync(dados.ChamadoA.Id));
        Assert.Empty(await useCase.ListarDependenciasPorChamadoAsync(dados.ChamadoB.Id));
        Assert.False(await useCase.PossuiDependenciasAtivasAsync(dados.ChamadoA.Id));
        Assert.False(await useCase.PossuiDependenciasAtivasAsync(dados.ChamadoB.Id));
    }

    [Fact]
    public async Task ChamadoSemVinculosDeBloqueioNaoDeveRetornarDependencias()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);

        Assert.Empty(await useCase.ListarDependenciasPorChamadoAsync(dados.ChamadoC.Id));
        Assert.False(await useCase.PossuiDependenciasAtivasAsync(dados.ChamadoC.Id));
        Assert.False(await useCase.EstaBloqueadoPorDependenciaAsync(dados.ChamadoC.Id));
    }

    [Fact]
    public async Task BooleanosDevemIndicarQuandoChamadoEstaBloqueadoPorDependencia()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        AdicionarRelacionamento(
            context,
            dados.ChamadoA,
            dados.ChamadoB,
            dados.Admin,
            TipoRelacionamentoChamadoEnum.Bloqueia);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.ContextoAdmin);

        Assert.True(await useCase.PossuiDependenciasAtivasAsync(dados.ChamadoB.Id));
        Assert.True(await useCase.EstaBloqueadoPorDependenciaAsync(dados.ChamadoB.Id));
        Assert.False(await useCase.PossuiDependenciasAtivasAsync(dados.ChamadoA.Id));
        Assert.False(await useCase.EstaBloqueadoPorDependenciaAsync(dados.ChamadoA.Id));
    }

    [Fact]
    public async Task BloqueioDeChamadoSemDependenciasDeveRetornarResumoVazio()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var bloqueio = await useCase.ObterBloqueioPorChamadoAsync(dados.ChamadoC.Id);

        Assert.Equal(dados.ChamadoC.Id, bloqueio.ChamadoId);
        Assert.False(bloqueio.EstaBloqueado);
        Assert.False(bloqueio.BloqueiaOutrosChamados);
        Assert.Empty(bloqueio.Bloqueadores);
        Assert.Empty(bloqueio.ChamadosBloqueados);
    }

    [Fact]
    public async Task BloqueadoPorDeveGerarBloqueioNoChamadoDependente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        AdicionarRelacionamento(
            context,
            dados.ChamadoA,
            dados.ChamadoB,
            dados.Admin,
            TipoRelacionamentoChamadoEnum.BloqueadoPor);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var bloqueioDependente = await useCase.ObterBloqueioPorChamadoAsync(dados.ChamadoA.Id);
        var bloqueioBloqueador = await useCase.ObterBloqueioPorChamadoAsync(dados.ChamadoB.Id);

        Assert.True(bloqueioDependente.EstaBloqueado);
        Assert.False(bloqueioDependente.BloqueiaOutrosChamados);
        Assert.Equal(dados.ChamadoB.Id, Assert.Single(bloqueioDependente.Bloqueadores).ChamadoBloqueadorId);

        Assert.False(bloqueioBloqueador.EstaBloqueado);
        Assert.True(bloqueioBloqueador.BloqueiaOutrosChamados);
        Assert.Equal(dados.ChamadoA.Id, Assert.Single(bloqueioBloqueador.ChamadosBloqueados).ChamadoDependenteId);
    }

    [Fact]
    public async Task BloqueiaDeveGerarBloqueioNoChamadoDestinoDependente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        AdicionarRelacionamento(
            context,
            dados.ChamadoA,
            dados.ChamadoB,
            dados.Admin,
            TipoRelacionamentoChamadoEnum.Bloqueia);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var bloqueioDependente = await useCase.ObterBloqueioPorChamadoAsync(dados.ChamadoB.Id);
        var bloqueioBloqueador = await useCase.ObterBloqueioPorChamadoAsync(dados.ChamadoA.Id);

        Assert.True(bloqueioDependente.EstaBloqueado);
        Assert.False(bloqueioDependente.BloqueiaOutrosChamados);
        Assert.Equal(dados.ChamadoA.Id, Assert.Single(bloqueioDependente.Bloqueadores).ChamadoBloqueadorId);

        Assert.False(bloqueioBloqueador.EstaBloqueado);
        Assert.True(bloqueioBloqueador.BloqueiaOutrosChamados);
        Assert.Equal(dados.ChamadoB.Id, Assert.Single(bloqueioBloqueador.ChamadosBloqueados).ChamadoDependenteId);
    }

    [Fact]
    public async Task VariosBloqueadoresAtivosDevemRetornarTodosNaLista()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        AdicionarRelacionamento(
            context,
            dados.ChamadoA,
            dados.ChamadoB,
            dados.Admin,
            TipoRelacionamentoChamadoEnum.Bloqueia);
        AdicionarRelacionamento(
            context,
            dados.ChamadoB,
            dados.ChamadoC,
            dados.Admin,
            TipoRelacionamentoChamadoEnum.BloqueadoPor);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var bloqueio = await useCase.ObterBloqueioPorChamadoAsync(dados.ChamadoB.Id);

        Assert.True(bloqueio.EstaBloqueado);
        Assert.False(bloqueio.BloqueiaOutrosChamados);
        Assert.Equal(2, bloqueio.Bloqueadores.Count);
        Assert.Contains(bloqueio.Bloqueadores, x => x.ChamadoBloqueadorId == dados.ChamadoA.Id);
        Assert.Contains(bloqueio.Bloqueadores, x => x.ChamadoBloqueadorId == dados.ChamadoC.Id);
    }

    [Fact]
    public async Task VinculoInativoNaoDeveGerarBloqueio()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var relacionamento = AdicionarRelacionamento(
            context,
            dados.ChamadoA,
            dados.ChamadoB,
            dados.Admin,
            TipoRelacionamentoChamadoEnum.Bloqueia);
        relacionamento.Inativar(dados.Admin.Id, dados.Admin.Login, "Bloqueio removido");
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var bloqueio = await useCase.ObterBloqueioPorChamadoAsync(dados.ChamadoB.Id);

        Assert.False(bloqueio.EstaBloqueado);
        Assert.False(bloqueio.BloqueiaOutrosChamados);
        Assert.Empty(bloqueio.Bloqueadores);
        Assert.Empty(bloqueio.ChamadosBloqueados);
    }

    [Theory]
    [InlineData(TipoRelacionamentoChamadoEnum.Relacionado)]
    [InlineData(TipoRelacionamentoChamadoEnum.Duplicado)]
    [InlineData(TipoRelacionamentoChamadoEnum.Pai)]
    [InlineData(TipoRelacionamentoChamadoEnum.Filho)]
    [InlineData(TipoRelacionamentoChamadoEnum.DerivadoDe)]
    [InlineData(TipoRelacionamentoChamadoEnum.Origina)]
    public async Task TiposInformativosNaoDevemGerarBloqueio(TipoRelacionamentoChamadoEnum tipo)
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        AdicionarRelacionamento(context, dados.ChamadoA, dados.ChamadoB, dados.Admin, tipo);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.ContextoAdmin);
        var bloqueioA = await useCase.ObterBloqueioPorChamadoAsync(dados.ChamadoA.Id);
        var bloqueioB = await useCase.ObterBloqueioPorChamadoAsync(dados.ChamadoB.Id);

        Assert.False(bloqueioA.EstaBloqueado);
        Assert.False(bloqueioA.BloqueiaOutrosChamados);
        Assert.False(bloqueioB.EstaBloqueado);
        Assert.False(bloqueioB.BloqueiaOutrosChamados);
    }

    [Fact]
    public async Task ConsultaDeBloqueioDeveRespeitarPermissaoAdministrativa()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, AdminUseCasesTestFactory.Contexto(dados.Solicitante, "Solicitante"));

        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => useCase.ObterBloqueioPorChamadoAsync(dados.ChamadoA.Id));
        Assert.Equal("Acesso administrativo negado.", ex.Message);
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

    private static ChamadoRelacionamento AdicionarRelacionamento(
        SGXSistemaChamadoDbContext context,
        Chamado chamadoOrigem,
        Chamado chamadoDestino,
        Usuario admin,
        TipoRelacionamentoChamadoEnum tipo)
    {
        var relacionamento = new ChamadoRelacionamento(
            chamadoOrigem.Id,
            chamadoDestino.Id,
            tipo,
            admin.Id,
            admin.Login,
            "Dependencia operacional de teste");
        context.ChamadosRelacionamentos.Add(relacionamento);
        return relacionamento;
    }

    private static async Task<(
        Chamado ChamadoA,
        Chamado ChamadoB,
        Chamado ChamadoC,
        Usuario Admin,
        Usuario Solicitante,
        UsuarioContextoAplicacao ContextoAdmin)> SeedAsync(SGXSistemaChamadoDbContext context)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            context,
            "Admin Dependencia Chamado",
            $"admin.dep.rel.{Guid.NewGuid():N}@empresa.com",
            TipoPerfil.Administrador);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            context,
            "Solicitante Dependencia Chamado",
            $"sol.dep.rel.{Guid.NewGuid():N}@empresa.com",
            TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, $"Categoria Dependencia {Guid.NewGuid():N}");
        var chamadoA = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.Aberto, sufixoCodigo: "DEP-A");
        var chamadoB = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.Aberto, sufixoCodigo: "DEP-B");
        var chamadoC = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.Aberto, sufixoCodigo: "DEP-C");

        return (chamadoA, chamadoB, chamadoC, admin, solicitante, AdminUseCasesTestFactory.Contexto(admin, "Administrador"));
    }
}
