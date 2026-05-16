using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class TiposSolicitacaoAdminUseCaseTests
{
    [Fact]
    public async Task CriarTipoSolicitacaoValido()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "tipo.criar@empresa.com", TipoPerfil.Administrador);

        var useCase = new CriarTipoSolicitacaoUseCase(
            PortalUseCasesTestFactory.Repo<TipoSolicitacao>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        var response = await useCase.ExecutarAsync(new CriarTipoSolicitacaoRequest
        {
            Nome = "Incidente",
            Descricao = "Incidentes operacionais"
        });

        Assert.Equal("Incidente", response.Nome);
    }

    [Fact]
    public async Task BloqueiaTipoSolicitacaoDuplicado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "tipo.dup@empresa.com", TipoPerfil.Administrador);
        context.TiposSolicitacao.Add(new TipoSolicitacao("Mudanca", null, "teste"));
        await context.SaveChangesAsync();

        var useCase = new CriarTipoSolicitacaoUseCase(
            PortalUseCasesTestFactory.Repo<TipoSolicitacao>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarTipoSolicitacaoRequest
        {
            Nome = "Mudanca"
        }));
    }

    [Fact]
    public async Task EditarInativarReativarTipoSolicitacao()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "tipo.status@empresa.com", TipoPerfil.Administrador);
        var tipo = new TipoSolicitacao("Requisicao", null, "teste");
        context.TiposSolicitacao.Add(tipo);
        await context.SaveChangesAsync();

        var atualizarUseCase = new AtualizarTipoSolicitacaoUseCase(
            PortalUseCasesTestFactory.Repo<TipoSolicitacao>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        var atualizado = await atualizarUseCase.ExecutarAsync(tipo.Id, new AtualizarTipoSolicitacaoRequest
        {
            Nome = "Requisicao de Servico",
            Descricao = "Atualizada"
        });
        Assert.Equal("Requisicao de Servico", atualizado.Nome);

        var inativarUseCase = new InativarTipoSolicitacaoUseCase(
            PortalUseCasesTestFactory.Repo<TipoSolicitacao>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));
        var inativado = await inativarUseCase.ExecutarAsync(tipo.Id);
        Assert.False(inativado.Ativo);

        var reativarUseCase = new ReativarTipoSolicitacaoUseCase(
            PortalUseCasesTestFactory.Repo<TipoSolicitacao>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));
        var reativado = await reativarUseCase.ExecutarAsync(tipo.Id);
        Assert.True(reativado.Ativo);
    }

    [Fact]
    public async Task ListarBuscarEFiltrarTiposSolicitacaoPorStatus()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "tipo.list@empresa.com", TipoPerfil.Administrador);
        var ativo = new TipoSolicitacao("Incidente de Rede", null, "teste");
        var inativo = new TipoSolicitacao("Incidente Legado", null, "teste");
        inativo.Desativar("teste");
        context.TiposSolicitacao.AddRange(ativo, inativo);
        await context.SaveChangesAsync();

        var useCase = new ListarTiposSolicitacaoAdminUseCase(
            PortalUseCasesTestFactory.Repo<TipoSolicitacao>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")));

        var response = await useCase.ExecutarAsync(new FiltroCadastroRequest
        {
            Texto = "Incidente",
            Ativo = true,
            OrdenarPor = "nome",
            DirecaoOrdenacao = "asc"
        });

        Assert.Single(response.Items);
        Assert.Equal("Incidente de Rede", response.Items.Single().Nome);
    }

    [Fact]
    public async Task ListarTiposSolicitacaoComFiltroInativo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "tipo.list.inativo@empresa.com", TipoPerfil.Administrador);
        var ativa = new TipoSolicitacao("Mudanca Ativa", null, "teste");
        var inativa = new TipoSolicitacao("Mudanca Inativa", null, "teste");
        inativa.Desativar("teste");
        context.TiposSolicitacao.AddRange(ativa, inativa);
        await context.SaveChangesAsync();

        var useCase = new ListarTiposSolicitacaoAdminUseCase(
            PortalUseCasesTestFactory.Repo<TipoSolicitacao>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")));

        var response = await useCase.ExecutarAsync(new FiltroCadastroRequest { Ativo = false, Texto = "Mudanca" });

        Assert.Single(response.Items);
        Assert.Equal("Mudanca Inativa", response.Items.Single().Nome);
        Assert.False(response.Items.Single().Ativo);
    }

    [Fact]
    public async Task ListarTiposSolicitacaoSemFiltroRetornaAtivosEInativos()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "tipo.list.todos@empresa.com", TipoPerfil.Administrador);
        var ativa = new TipoSolicitacao("Servico Ativo", null, "teste");
        var inativa = new TipoSolicitacao("Servico Inativo", null, "teste");
        inativa.Desativar("teste");
        context.TiposSolicitacao.AddRange(ativa, inativa);
        await context.SaveChangesAsync();

        var useCase = new ListarTiposSolicitacaoAdminUseCase(
            PortalUseCasesTestFactory.Repo<TipoSolicitacao>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")));

        var response = await useCase.ExecutarAsync(new FiltroCadastroRequest { Texto = "Servico" });

        Assert.Contains(response.Items, x => x.Nome == "Servico Ativo");
        Assert.Contains(response.Items, x => x.Nome == "Servico Inativo");
    }
}
