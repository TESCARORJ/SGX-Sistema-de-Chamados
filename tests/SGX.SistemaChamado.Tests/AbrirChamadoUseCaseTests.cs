using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Portal;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Portal;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class AbrirChamadoUseCaseTests
{
    [Fact]
    public async Task DeveAbrirChamadoValidoParaUsuarioAutenticado()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);

        var useCase = new AbrirChamadoUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<CategoriaChamado>(context),
            PortalUseCasesTestFactory.Repo<PrioridadeChamado>(context),
            PortalUseCasesTestFactory.Repo<Departamento>(context),
            PortalUseCasesTestFactory.Repo<StatusChamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            SlaTestFactory.CriarService(context),
            new FakeCodigoChamadoService(),
            new FakeUsuarioContextoAplicacaoService(dados.UsuarioContexto),
            PortalUseCasesTestFactory.Uow(context));

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Portal nao autentica",
            Descricao = "Erro ao autenticar no SSO",
            CategoriaId = dados.Categoria.Id,
            PrioridadeId = dados.Prioridade.Id,
            DepartamentoId = dados.Departamento.Id
        });

        Assert.Equal(dados.Usuario.Id, context.Chamados.Single().SolicitanteId);
        Assert.Equal("Portal nao autentica", response.Titulo);
    }

    [Fact]
    public async Task DeveCriarHistoricoEStatusAberto()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);

        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Erro VPN",
            Descricao = "Sem acesso VPN",
            CategoriaId = dados.Categoria.Id,
            PrioridadeId = dados.Prioridade.Id
        });

        Assert.Equal("Aberto", response.Status);
        Assert.Contains(context.HistoricosChamado, x => x.Tipo == TipoHistoricoChamado.Criado);
    }

    [Fact]
    public async Task DeveRejeitarCategoriaInexistenteOuInativa()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Erro",
            Descricao = "Descricao valida",
            CategoriaId = Guid.NewGuid(),
            PrioridadeId = dados.Prioridade.Id
        }));
    }

    [Fact]
    public async Task DeveRejeitarPrioridadeInexistenteOuInativa()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Erro",
            Descricao = "Descricao valida",
            CategoriaId = dados.Categoria.Id,
            PrioridadeId = Guid.NewGuid()
        }));
    }

    [Fact]
    public async Task NaoDeveAceitarTituloVazio()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = " ",
            Descricao = "Descricao valida",
            CategoriaId = dados.Categoria.Id,
            PrioridadeId = dados.Prioridade.Id
        }));
    }

    [Fact]
    public async Task NaoDeveAceitarDescricaoVazia()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Titulo valido",
            Descricao = "  ",
            CategoriaId = dados.Categoria.Id,
            PrioridadeId = dados.Prioridade.Id
        }));
    }

    private static AbrirChamadoUseCase CriarUseCase(SGXSistemaChamadoDbContext context, UsuarioContextoAplicacao usuario)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<CategoriaChamado>(context),
            PortalUseCasesTestFactory.Repo<PrioridadeChamado>(context),
            PortalUseCasesTestFactory.Repo<Departamento>(context),
            PortalUseCasesTestFactory.Repo<StatusChamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            SlaTestFactory.CriarService(context),
            new FakeCodigoChamadoService(),
            new FakeUsuarioContextoAplicacaoService(usuario),
            PortalUseCasesTestFactory.Uow(context));

    private static async Task<(Usuario Usuario, UsuarioContextoAplicacao UsuarioContexto, Departamento Departamento, CategoriaChamado Categoria, PrioridadeChamado Prioridade)> SeedBasico(SGXSistemaChamadoDbContext context)
    {
        var departamento = new Departamento("Tecnologia da Informacao", "TI", null, "teste");
        var categoria = new CategoriaChamado("Suporte Tecnico", null, departamento.Id, "teste");
        var usuario = new Usuario("Solicitante Teste", "solicitante@empresa.com", "solicitante", "teste", departamento.Id);

        context.Departamentos.Add(departamento);
        context.CategoriasChamado.Add(categoria);
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        var prioridade = await context.PrioridadesChamado.FirstAsync(x => x.Ativo);

        return (
            usuario,
            new UsuarioContextoAplicacao(usuario.Id, usuario.Nome, usuario.Email, usuario.Login, ["Solicitante"]),
            departamento,
            categoria,
            prioridade);
    }
}
