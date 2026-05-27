using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Services;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class ReabrirChamadoUseCaseTests
{
    [Fact]
    public async Task ReabreChamadoEncerrado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedEncerradoAsync(context);

        var useCase = CriarUseCase(context, dados.AdminContexto);
        var response = await useCase.ExecutarAsync(dados.Chamado.Id, new ReabrirChamadoRequest { Mensagem = "Reabrindo" });

        Assert.Equal("Em Atendimento", response.Status);
        Assert.Null(context.Chamados.Single().EncerradoEm);
    }

    [Fact]
    public async Task CriaHistorico()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedEncerradoAsync(context);

        var useCase = CriarUseCase(context, dados.AdminContexto);
        await useCase.ExecutarAsync(dados.Chamado.Id, new ReabrirChamadoRequest { Mensagem = "Reabrindo" });

        Assert.Contains(context.HistoricosChamado, x => x.Tipo == TipoHistoricoChamado.Reaberto);
    }

    [Fact]
    public async Task ReabreChamadoComStatusFinalEspecificoDaNatureza()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedFinalAsync(context, NaturezaChamadoEnum.EventoAlerta, StatusChamadoEnum.Tratado, "REA3");
        var useCase = CriarUseCase(context, dados.AdminContexto);

        var response = await useCase.ExecutarAsync(dados.Chamado.Id, new ReabrirChamadoRequest { Mensagem = "Reabrindo evento" });

        Assert.Equal("Em Analise", response.Status);
    }

    [Fact]
    public async Task BloqueiaReabrirChamadoAberto()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAbertoAsync(context);

        var useCase = CriarUseCase(context, dados.AdminContexto);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(dados.Chamado.Id, new ReabrirChamadoRequest { Mensagem = "Teste" }));
    }

    [Fact]
    public async Task BloqueiaReabrirQuandoAprovacaoPendente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedEncerradoAsync(context);
        await AdicionarAprovacaoAsync(context, dados.Chamado, StatusAprovacaoChamado.Pendente);

        var useCase = CriarUseCase(context, dados.AdminContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            useCase.ExecutarAsync(dados.Chamado.Id, new ReabrirChamadoRequest { Mensagem = "Reabrir" }));

        Assert.Equal("Este chamado aguarda aprovacao antes de seguir para atendimento.", ex.Message);
    }

    [Fact]
    public async Task BloqueiaReabrirQuandoAprovacaoReprovada()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedEncerradoAsync(context);
        await AdicionarAprovacaoAsync(context, dados.Chamado, StatusAprovacaoChamado.Reprovado);

        var useCase = CriarUseCase(context, dados.AdminContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            useCase.ExecutarAsync(dados.Chamado.Id, new ReabrirChamadoRequest { Mensagem = "Reabrir" }));

        Assert.Equal("Este chamado foi reprovado e nao pode seguir para atendimento.", ex.Message);
    }

    private static ReabrirChamadoUseCase CriarUseCase(SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context, UsuarioContextoAplicacao contexto)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<StatusChamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            PortalUseCasesTestFactory.Repo<ComentarioChamado>(context),
            new FluxoStatusChamadoService(),
            new AcoesChamadoService(new FluxoStatusChamadoService()),
            SlaTestFactory.CriarService(context),
            new FakeUsuarioContextoAplicacaoService(contexto),
            PortalUseCasesTestFactory.Uow(context));

    private static async Task<(Chamado Chamado, UsuarioContextoAplicacao AdminContexto)> SeedEncerradoAsync(SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context)
    {
        return await SeedFinalAsync(context, NaturezaChamadoEnum.Requisicao, StatusChamadoEnum.Encerrado, "REA1");
    }

    private static async Task<(Chamado Chamado, UsuarioContextoAplicacao AdminContexto)> SeedFinalAsync(
        SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context,
        NaturezaChamadoEnum naturezaChamado,
        StatusChamadoEnum statusFinal,
        string sufixoCodigo)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "admin@empresa.com", TipoPerfil.Administrador);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante", "sol@empresa.com", TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Infra");
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, statusFinal, null, sufixoCodigo, naturezaChamado: naturezaChamado);

        if (statusFinal == StatusChamadoEnum.Encerrado)
        {
            chamado.Encerrar(context.StatusChamado.First(x => x.Codigo == StatusChamadoEnum.Encerrado).Id, "teste");
        }

        context.Chamados.Update(chamado);
        await context.SaveChangesAsync();

        return (chamado, AdminUseCasesTestFactory.Contexto(admin, "Administrador"));
    }

    private static async Task<(Chamado Chamado, UsuarioContextoAplicacao AdminContexto)> SeedAbertoAsync(SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin2", "admin2@empresa.com", TipoPerfil.Administrador);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante2", "sol2@empresa.com", TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Sistemas");
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.Aberto, null, "REA2");

        return (chamado, AdminUseCasesTestFactory.Contexto(admin, "Administrador"));
    }

    private static async Task AdicionarAprovacaoAsync(
        SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context,
        Chamado chamado,
        StatusAprovacaoChamado status)
    {
        var admin = context.Usuarios.First(x => x.Email == "admin@empresa.com");
        var aprovacao = new AprovacaoChamado(
            chamado.Id,
            TipoOrigemAprovacaoChamado.Manual,
            admin.Id,
            admin.Login,
            chamado.SolicitanteId,
            "Origem",
            "Justificativa");

        if (status == StatusAprovacaoChamado.Aprovado)
        {
            aprovacao.Aprovar(admin.Id, admin.Id, admin.Login, "Aprovado");
        }
        else if (status == StatusAprovacaoChamado.Reprovado)
        {
            aprovacao.Reprovar(admin.Id, admin.Id, admin.Login, "Reprovado");
        }
        else if (status == StatusAprovacaoChamado.Cancelado)
        {
            aprovacao.Cancelar(admin.Id, admin.Login, "Cancelado");
        }

        await context.AprovacoesChamado.AddAsync(aprovacao);
        await context.SaveChangesAsync();
    }
}

