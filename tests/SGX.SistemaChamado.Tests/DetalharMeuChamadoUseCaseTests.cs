using SGX.SistemaChamado.Application.UseCases.Portal;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class DetalharMeuChamadoUseCaseTests
{
    [Fact]
    public async Task SolicitanteVeProprioChamado()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedChamados(context);

        var useCase = new DetalharMeuChamadoUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FakeUsuarioContextoAplicacaoService(dados.SolicitanteContexto));

        var response = await useCase.ExecutarAsync(dados.ChamadoSolicitante.Id);

        Assert.Equal(dados.ChamadoSolicitante.Id, response.Id);
    }

    [Fact]
    public async Task SolicitanteNaoVeChamadoDeOutroUsuario()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedChamados(context);

        var useCase = new DetalharMeuChamadoUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FakeUsuarioContextoAplicacaoService(dados.SolicitanteContexto));

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => useCase.ExecutarAsync(dados.ChamadoOutro.Id));
    }

    [Fact]
    public async Task AdministradorPodeVisualizarConformeRegra()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedChamados(context);

        var useCase = new DetalharMeuChamadoUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FakeUsuarioContextoAplicacaoService(dados.AdminContexto));

        var response = await useCase.ExecutarAsync(dados.ChamadoOutro.Id);

        Assert.Equal(dados.ChamadoOutro.Id, response.Id);
    }

    [Fact]
    public async Task DeveRetornarDadosCompletosSemComentariosInternos()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedChamados(context);

        var comentarioPublico = new ComentarioChamado(
            dados.ChamadoSolicitante.Id,
            dados.SolicitanteContexto.Id,
            "Comentario publico",
            false,
            "teste");

        var comentarioInterno = new ComentarioChamado(
            dados.ChamadoSolicitante.Id,
            dados.SolicitanteContexto.Id,
            "Comentario interno",
            true,
            "teste");

        var historicoCriacao = new HistoricoChamado(
            dados.ChamadoSolicitante.Id,
            TipoHistoricoChamado.Criado,
            "Chamado criado pelo portal",
            dados.SolicitanteContexto.Id,
            "teste");

        var anexo = new AnexoChamado(
            dados.ChamadoSolicitante.Id,
            "evidencia.pdf",
            "arquivo.pdf",
            "application/pdf",
            1024,
            "storage/anexos/arquivo.pdf",
            dados.SolicitanteContexto.Id,
            "teste");

        context.ComentariosChamado.AddRange(comentarioPublico, comentarioInterno);
        context.HistoricosChamado.Add(historicoCriacao);
        context.AnexosChamado.Add(anexo);
        await context.SaveChangesAsync();

        var useCase = new DetalharMeuChamadoUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FakeUsuarioContextoAplicacaoService(dados.SolicitanteContexto));

        var response = await useCase.ExecutarAsync(dados.ChamadoSolicitante.Id);

        Assert.Equal("Chamado Proprio", response.Titulo);
        Assert.Equal("Acesso", response.Subcategoria);
        Assert.Equal("Incidente", response.TipoSolicitacao);
        Assert.Equal("Matriz", response.LocalUnidade);
        Assert.NotEmpty(response.Historico);
        Assert.Single(response.Anexos);
        Assert.Single(response.Comentarios);
        Assert.DoesNotContain(response.Comentarios, item => item.Mensagem.Contains("interno", StringComparison.OrdinalIgnoreCase));
    }

    private static async Task<(Chamado ChamadoSolicitante, Chamado ChamadoOutro, UsuarioContextoAplicacao SolicitanteContexto, UsuarioContextoAplicacao AdminContexto)> SeedChamados(SGXSistemaChamadoDbContext context)
    {
        var prioridade = context.PrioridadesChamado.First();
        var status = context.StatusChamado.First(x => x.Codigo == StatusChamadoEnum.Aberto);
        var categoria = new CategoriaChamado("Categoria", null, null, "teste");
        var subcategoria = new SubcategoriaChamado(categoria.Id, "Acesso", null, "teste");
        var tipoSolicitacao = new TipoSolicitacao("Incidente", null, "teste");
        var localUnidade = new LocalUnidade("Matriz", null, null, "teste");

        var solicitante = new Usuario("Usuario Solicitante", "solicitante@empresa.com", "solicitante", "teste");
        var outro = new Usuario("Outro Usuario", "outro@empresa.com", "outro", "teste");

        context.CategoriasChamado.Add(categoria);
        context.SubcategoriasChamado.Add(subcategoria);
        context.TiposSolicitacao.Add(tipoSolicitacao);
        context.LocaisUnidade.Add(localUnidade);
        context.Usuarios.AddRange(solicitante, outro);
        await context.SaveChangesAsync();

        var chamadoSolicitante = new Chamado("CH-PROP", "Chamado Proprio", "Descricao", solicitante.Id, categoria.Id, prioridade.Id, status.Id, OrigemChamado.Portal, "teste", null, subcategoria.Id, tipoSolicitacao.Id, localUnidade.Id);
        var chamadoOutro = new Chamado("CH-OUTRO", "Chamado Outro", "Descricao", outro.Id, categoria.Id, prioridade.Id, status.Id, OrigemChamado.Portal, "teste", null, subcategoria.Id, tipoSolicitacao.Id, localUnidade.Id);
        context.Chamados.AddRange(chamadoSolicitante, chamadoOutro);
        await context.SaveChangesAsync();

        return (
            chamadoSolicitante,
            chamadoOutro,
            new UsuarioContextoAplicacao(solicitante.Id, solicitante.Nome, solicitante.Email, solicitante.Login, ["Solicitante"]),
            new UsuarioContextoAplicacao(Guid.NewGuid(), "Admin", "admin@empresa.com", "admin", ["Administrador"]));
    }
}
