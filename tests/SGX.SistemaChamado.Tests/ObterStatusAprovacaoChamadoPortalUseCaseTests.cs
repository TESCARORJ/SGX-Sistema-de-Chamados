using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Portal;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class ObterStatusAprovacaoChamadoPortalUseCaseTests
{
    [Fact]
    public async Task DeveRetornarStatusPendenteParaSolicitanteDoChamado()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedChamados(context);

        var aprovacao = new AprovacaoChamado(
            dados.ChamadoSolicitante.Id,
            TipoOrigemAprovacaoChamado.Manual,
            dados.SolicitanteContexto.Id,
            dados.SolicitanteContexto.Login,
            dados.SolicitanteContexto.Id,
            "Fluxo",
            "Justificativa");
        context.AprovacoesChamado.Add(aprovacao);
        await context.SaveChangesAsync();

        var useCase = new ObterStatusAprovacaoChamadoPortalUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FakeUsuarioContextoAplicacaoService(dados.SolicitanteContexto));

        var response = await useCase.ExecutarAsync(dados.ChamadoSolicitante.Id);

        Assert.Equal(dados.ChamadoSolicitante.Id, response.ChamadoId);
        Assert.True(response.RequerAprovacao);
        Assert.True(response.AprovacaoPendente);
        Assert.Equal(StatusAprovacaoChamado.Pendente, response.StatusAprovacao);
        Assert.Equal("Seu chamado esta aguardando aprovacao antes de seguir para atendimento.", response.MensagemOrientativa);
    }

    [Fact]
    public async Task NaoDevePermitirSolicitanteConsultarChamadoDeOutroUsuario()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedChamados(context);

        var useCase = new ObterStatusAprovacaoChamadoPortalUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FakeUsuarioContextoAplicacaoService(dados.SolicitanteContexto));

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => useCase.ExecutarAsync(dados.ChamadoOutro.Id));
    }

    [Fact]
    public async Task DeveRetornarJustificativaQuandoReprovado()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedChamados(context);

        var aprovacao = new AprovacaoChamado(
            dados.ChamadoSolicitante.Id,
            TipoOrigemAprovacaoChamado.Manual,
            dados.SolicitanteContexto.Id,
            dados.SolicitanteContexto.Login,
            dados.SolicitanteContexto.Id,
            "Fluxo",
            "Justificativa");
        aprovacao.Reprovar(dados.AdminContexto.Id, dados.AdminContexto.Id, dados.AdminContexto.Login, "Faltou informacao obrigatoria.");
        context.AprovacoesChamado.Add(aprovacao);
        await context.SaveChangesAsync();

        var useCase = new ObterStatusAprovacaoChamadoPortalUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FakeUsuarioContextoAplicacaoService(dados.SolicitanteContexto));

        var response = await useCase.ExecutarAsync(dados.ChamadoSolicitante.Id);

        Assert.Equal(StatusAprovacaoChamado.Reprovado, response.StatusAprovacao);
        Assert.Equal("Faltou informacao obrigatoria.", response.JustificativaDecisao);
        Assert.Equal("Seu chamado foi reprovado. Verifique a justificativa.", response.MensagemOrientativa);
    }

    [Fact]
    public async Task DeveRetornarNaoRequerAprovacaoQuandoNaoHaAprovacoes()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await SeedChamados(context);

        var useCase = new ObterStatusAprovacaoChamadoPortalUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FakeUsuarioContextoAplicacaoService(dados.SolicitanteContexto));

        var response = await useCase.ExecutarAsync(dados.ChamadoSolicitante.Id);

        Assert.False(response.RequerAprovacao);
        Assert.False(response.AprovacaoPendente);
        Assert.Null(response.StatusAprovacao);
        Assert.Equal("Este chamado nao requer aprovacao.", response.MensagemOrientativa);
    }

    private static async Task<(Chamado ChamadoSolicitante, Chamado ChamadoOutro, UsuarioContextoAplicacao SolicitanteContexto, UsuarioContextoAplicacao AdminContexto)> SeedChamados(SGXSistemaChamadoDbContext context)
    {
        var prioridade = context.PrioridadesChamado.First();
        var status = context.StatusChamado.First(x => x.Codigo == StatusChamadoEnum.Aberto);
        var categoria = new CategoriaChamado("Categoria", null, null, "teste");

        var solicitante = new Usuario("Usuario Solicitante", "solicitante.status@empresa.com", "solicitante.status", "teste");
        var outro = new Usuario("Outro Usuario", "outro.status@empresa.com", "outro.status", "teste");

        context.CategoriasChamado.Add(categoria);
        context.Usuarios.AddRange(solicitante, outro);
        await context.SaveChangesAsync();

        var chamadoSolicitante = new Chamado("CH-STATUS-PROP", "Chamado Proprio", "Descricao", solicitante.Id, categoria.Id, prioridade.Id, status.Id, OrigemChamado.Portal, "teste");
        var chamadoOutro = new Chamado("CH-STATUS-OUTRO", "Chamado Outro", "Descricao", outro.Id, categoria.Id, prioridade.Id, status.Id, OrigemChamado.Portal, "teste");
        context.Chamados.AddRange(chamadoSolicitante, chamadoOutro);
        await context.SaveChangesAsync();

        return (
            chamadoSolicitante,
            chamadoOutro,
            new UsuarioContextoAplicacao(solicitante.Id, solicitante.Nome, solicitante.Email, solicitante.Login, ["Solicitante"]),
            new UsuarioContextoAplicacao(Guid.NewGuid(), "Admin", "admin.status@empresa.com", "admin.status", ["Administrador"]));
    }
}
