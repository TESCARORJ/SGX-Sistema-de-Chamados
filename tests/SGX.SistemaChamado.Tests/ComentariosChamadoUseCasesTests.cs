using SGX.SistemaChamado.Application.DTOs.Chamados;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Chamados;
using SGX.SistemaChamado.Application.Validators;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class ComentariosChamadoUseCasesTests
{
    [Fact]
    public async Task SolicitanteCriaComentarioPublicoNoProprioChamado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarAdicionarUseCase(context, dados.SolicitanteContexto);
        var response = await useCase.ExecutarAsync(
            dados.ChamadoProprio.Id,
            new CriarComentarioChamadoRequest { Mensagem = "Comentario publico do solicitante", Interno = false });

        Assert.False(response.Interno);
        Assert.Equal("Comentario publico do solicitante", response.Mensagem);
        Assert.Contains(context.ComentariosChamado, x => x.ChamadoId == dados.ChamadoProprio.Id && !x.Interno);
    }

    [Fact]
    public async Task SolicitanteNaoCriaComentarioInterno()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarAdicionarUseCase(context, dados.SolicitanteContexto);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => useCase.ExecutarAsync(
            dados.ChamadoProprio.Id,
            new CriarComentarioChamadoRequest { Mensagem = "Interno", Interno = true }));
    }

    [Fact]
    public async Task SolicitanteNaoVisualizaComentarioInterno()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        context.ComentariosChamado.AddRange(
            new ComentarioChamado(dados.ChamadoProprio.Id, dados.Atendente.Id, "Publico", false, "teste"),
            new ComentarioChamado(dados.ChamadoProprio.Id, dados.Atendente.Id, "Interno", true, "teste"));
        await context.SaveChangesAsync();

        var useCase = CriarListarUseCase(context, dados.SolicitanteContexto);
        var response = await useCase.ExecutarAsync(dados.ChamadoProprio.Id);

        Assert.Single(response);
        Assert.DoesNotContain(response, x => x.Interno);
    }

    [Fact]
    public async Task AtendenteCriaComentarioInterno()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarAdicionarUseCase(context, dados.AtendenteContexto);
        var response = await useCase.ExecutarAsync(
            dados.ChamadoProprio.Id,
            new CriarComentarioChamadoRequest { Mensagem = "Interno do atendimento", Interno = true });

        Assert.True(response.Interno);
        Assert.Contains(context.ComentariosChamado, x => x.ChamadoId == dados.ChamadoProprio.Id && x.Interno);
    }

    [Fact]
    public async Task AdministradorVisualizaComentariosPublicosEInternos()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        context.ComentariosChamado.AddRange(
            new ComentarioChamado(dados.ChamadoProprio.Id, dados.Atendente.Id, "Publico", false, "teste"),
            new ComentarioChamado(dados.ChamadoProprio.Id, dados.Atendente.Id, "Interno", true, "teste"));
        await context.SaveChangesAsync();

        var useCase = CriarListarUseCase(context, dados.AdminContexto);
        var response = await useCase.ExecutarAsync(dados.ChamadoProprio.Id);

        Assert.Equal(2, response.Count);
        Assert.Contains(response, x => x.Interno);
        Assert.Contains(response, x => !x.Interno);
    }

    [Fact]
    public async Task SolicitanteNaoAcessaComentariosDeOutroSolicitante()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarListarUseCase(context, dados.SolicitanteContexto);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => useCase.ExecutarAsync(dados.ChamadoOutroSolicitante.Id));
    }

    [Fact]
    public void MensagemVaziaRejeitada()
    {
        var validator = new CriarComentarioChamadoRequestValidator();
        var result = validator.Validate(new CriarComentarioChamadoRequest { Mensagem = "   ", Interno = false });

        Assert.False(result.IsValid);
    }

    [Fact]
    public void MensagemAcimaDoLimiteRejeitada()
    {
        var validator = new CriarComentarioChamadoRequestValidator();
        var mensagem = new string('x', 4001);
        var result = validator.Validate(new CriarComentarioChamadoRequest { Mensagem = mensagem, Interno = false });

        Assert.False(result.IsValid);
    }

    private static AdicionarComentarioChamadoUseCase CriarAdicionarUseCase(SGXSistemaChamadoDbContext context, UsuarioContextoAplicacao usuario)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<ComentarioChamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            SlaTestFactory.CriarService(context),
            new FakeUsuarioContextoAplicacaoService(usuario),
            PortalUseCasesTestFactory.Uow(context));

    private static ListarComentariosChamadoUseCase CriarListarUseCase(SGXSistemaChamadoDbContext context, UsuarioContextoAplicacao usuario)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<ComentarioChamado>(context),
            new FakeUsuarioContextoAplicacaoService(usuario));

    private static async Task<(
        Usuario Atendente,
        Chamado ChamadoProprio,
        Chamado ChamadoOutroSolicitante,
        UsuarioContextoAplicacao SolicitanteContexto,
        UsuarioContextoAplicacao AtendenteContexto,
        UsuarioContextoAplicacao AdminContexto)> SeedAsync(SGXSistemaChamadoDbContext context)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "admin.coment@empresa.com", TipoPerfil.Administrador);
        var atendente = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Atendente", "atendente.coment@empresa.com", TipoPerfil.Atendente);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante", "solicitante.coment@empresa.com", TipoPerfil.Solicitante);
        var outroSolicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Outro Solicitante", "outro.coment@empresa.com", TipoPerfil.Solicitante);

        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Atendimento");
        var chamadoProprio = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.EmAtendimento, null, "COMN-001");
        var chamadoOutro = await AdminUseCasesTestFactory.CriarChamadoAsync(context, outroSolicitante, categoria, StatusChamadoEnum.EmAtendimento, null, "COMN-002");

        return (
            atendente,
            chamadoProprio,
            chamadoOutro,
            AdminUseCasesTestFactory.Contexto(solicitante, "Solicitante"),
            AdminUseCasesTestFactory.Contexto(atendente, "Atendente"),
            AdminUseCasesTestFactory.Contexto(admin, "Administrador"));
    }
}
