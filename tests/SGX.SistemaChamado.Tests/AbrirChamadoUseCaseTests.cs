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
            PortalUseCasesTestFactory.Repo<SubcategoriaChamado>(context),
            PortalUseCasesTestFactory.Repo<PrioridadeChamado>(context),
            PortalUseCasesTestFactory.Repo<TipoSolicitacao>(context),
            PortalUseCasesTestFactory.Repo<LocalUnidade>(context),
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
    public async Task DeveCriarChamadoComSubcategoriaTipoELocalAtivos()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        var response = await useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Erro no sistema de notas",
            Descricao = "Usuario sem acesso ao modulo.",
            CategoriaId = dados.Categoria.Id,
            SubcategoriaId = dados.Subcategoria.Id,
            PrioridadeId = dados.Prioridade.Id,
            TipoSolicitacaoId = dados.TipoSolicitacao.Id,
            LocalUnidadeId = dados.LocalUnidade.Id
        });

        var chamadoCriado = context.Chamados.Single();
        Assert.Equal(dados.Subcategoria.Id, chamadoCriado.SubcategoriaId);
        Assert.Equal(dados.TipoSolicitacao.Id, chamadoCriado.TipoSolicitacaoId);
        Assert.Equal(dados.LocalUnidade.Id, chamadoCriado.LocalUnidadeId);
        Assert.Equal(dados.Subcategoria.Nome, response.Subcategoria);
        Assert.Equal(dados.TipoSolicitacao.Nome, response.TipoSolicitacao);
        Assert.Equal(dados.LocalUnidade.Nome, response.LocalUnidade);
    }

    [Fact]
    public async Task DeveRejeitarSubcategoriaDeOutraCategoria()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        var categoriaSecundaria = new CategoriaChamado("Rede", null, dados.Departamento.Id, "teste");
        var subcategoriaOutra = new SubcategoriaChamado(categoriaSecundaria.Id, "Wi-Fi", null, "teste");
        context.CategoriasChamado.Add(categoriaSecundaria);
        context.SubcategoriasChamado.Add(subcategoriaOutra);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Erro",
            Descricao = "Descricao valida",
            CategoriaId = dados.Categoria.Id,
            SubcategoriaId = subcategoriaOutra.Id,
            PrioridadeId = dados.Prioridade.Id
        }));
    }

    [Fact]
    public async Task DeveRejeitarSubcategoriaInativa()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        dados.Subcategoria.Desativar("teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Erro",
            Descricao = "Descricao valida",
            CategoriaId = dados.Categoria.Id,
            SubcategoriaId = dados.Subcategoria.Id,
            PrioridadeId = dados.Prioridade.Id
        }));
    }

    [Fact]
    public async Task DeveRejeitarTipoSolicitacaoInativo()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        dados.TipoSolicitacao.Desativar("teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Erro",
            Descricao = "Descricao valida",
            CategoriaId = dados.Categoria.Id,
            PrioridadeId = dados.Prioridade.Id,
            TipoSolicitacaoId = dados.TipoSolicitacao.Id
        }));
    }

    [Fact]
    public async Task DeveRejeitarLocalUnidadeInativo()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        dados.LocalUnidade.Desativar("teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Erro",
            Descricao = "Descricao valida",
            CategoriaId = dados.Categoria.Id,
            PrioridadeId = dados.Prioridade.Id,
            LocalUnidadeId = dados.LocalUnidade.Id
        }));
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
        Assert.Equal(OrigemChamado.Portal, context.Chamados.Single().Origem);
        Assert.Contains(
            context.HistoricosChamado,
            x => x.Tipo == TipoHistoricoChamado.Criado && x.Descricao == "Chamado criado pelo portal");
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
    public async Task DeveRejeitarCategoriaInativa()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        dados.Categoria.Desativar("teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Erro",
            Descricao = "Descricao valida",
            CategoriaId = dados.Categoria.Id,
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
    public async Task DeveRejeitarPrioridadeInativa()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedBasico(context);
        dados.Prioridade.Desativar("teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.UsuarioContexto);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarChamadoRequest
        {
            Titulo = "Erro",
            Descricao = "Descricao valida",
            CategoriaId = dados.Categoria.Id,
            PrioridadeId = dados.Prioridade.Id
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
            PortalUseCasesTestFactory.Repo<SubcategoriaChamado>(context),
            PortalUseCasesTestFactory.Repo<PrioridadeChamado>(context),
            PortalUseCasesTestFactory.Repo<TipoSolicitacao>(context),
            PortalUseCasesTestFactory.Repo<LocalUnidade>(context),
            PortalUseCasesTestFactory.Repo<Departamento>(context),
            PortalUseCasesTestFactory.Repo<StatusChamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            SlaTestFactory.CriarService(context),
            new FakeCodigoChamadoService(),
            new FakeUsuarioContextoAplicacaoService(usuario),
            PortalUseCasesTestFactory.Uow(context));

    private static async Task<(Usuario Usuario, UsuarioContextoAplicacao UsuarioContexto, Departamento Departamento, CategoriaChamado Categoria, SubcategoriaChamado Subcategoria, PrioridadeChamado Prioridade, TipoSolicitacao TipoSolicitacao, LocalUnidade LocalUnidade)> SeedBasico(SGXSistemaChamadoDbContext context)
    {
        var departamento = new Departamento("Tecnologia da Informacao", "TI", null, "teste");
        var categoria = new CategoriaChamado("Suporte Tecnico", null, departamento.Id, "teste");
        var subcategoria = new SubcategoriaChamado(categoria.Id, "Acesso", null, "teste");
        var tipoSolicitacao = new TipoSolicitacao("Incidente", null, "teste");
        var localUnidade = new LocalUnidade("Matriz", null, "Endereco", "teste");
        var usuario = new Usuario("Solicitante Teste", "solicitante@empresa.com", "solicitante", "teste", departamento.Id);

        context.Departamentos.Add(departamento);
        context.CategoriasChamado.Add(categoria);
        context.SubcategoriasChamado.Add(subcategoria);
        context.TiposSolicitacao.Add(tipoSolicitacao);
        context.LocaisUnidade.Add(localUnidade);
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        var prioridade = await context.PrioridadesChamado.FirstAsync(x => x.Ativo);

        return (
            usuario,
            new UsuarioContextoAplicacao(usuario.Id, usuario.Nome, usuario.Email, usuario.Login, ["Solicitante"]),
            departamento,
            categoria,
            subcategoria,
            prioridade,
            tipoSolicitacao,
            localUnidade);
    }
}
