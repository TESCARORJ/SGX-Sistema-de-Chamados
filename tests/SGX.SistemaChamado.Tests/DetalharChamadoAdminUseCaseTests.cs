using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Services;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class DetalharChamadoAdminUseCaseTests
{
    [Fact]
    public async Task AdminConsegueDetalharChamadoAbertoNoPortalComHistoricoEAnexo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = new DetalharChamadoAdminUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FluxoStatusChamadoService(),
            new AcoesChamadoService(new FluxoStatusChamadoService()),
            new FakeUsuarioContextoAplicacaoService(dados.ContextoAdmin));

        var response = await useCase.ExecutarAsync(dados.Chamado.Id);

        Assert.Equal("Portal", response.Origem);
        Assert.Equal("Aberto", response.Status);
        Assert.Equal(dados.Solicitante.Nome, response.Solicitante.Nome);
        Assert.Equal("Acesso", response.Subcategoria);
        Assert.Equal("Incidente", response.TipoSolicitacao);
        Assert.Equal("Matriz", response.LocalUnidade);
        Assert.True(Enum.IsDefined(response.NaturezaChamado));
        Assert.True(Enum.IsDefined(response.ImpactoChamado));
        Assert.True(Enum.IsDefined(response.UrgenciaChamado));
        Assert.NotEmpty(response.StatusPermitidosCodigos);
        Assert.NotEmpty(response.AcoesDisponiveisCodigos);
        Assert.Contains("AlterarStatus", response.AcoesDisponiveisCodigos);
        Assert.Contains((int)StatusChamadoEnum.Aberto, response.StatusPermitidosCodigos);
        Assert.Contains(response.Historico, item => item.Descricao == "Chamado criado pelo portal");
        Assert.Contains(response.Anexos, item => item.NomeArquivo == "evidencia.pdf");
    }

    [Fact]
    public async Task DetalheAdminDeveIndicarAprovacaoPendente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var aprovacao = new AprovacaoChamado(
            dados.Chamado.Id,
            TipoOrigemAprovacaoChamado.CatalogoServico,
            dados.Solicitante.Id,
            dados.Solicitante.Login,
            dados.Solicitante.Id,
            "Servico teste",
            "Aprovacao automatica por catalogo");
        context.AprovacoesChamado.Add(aprovacao);
        await context.SaveChangesAsync();

        var useCase = new DetalharChamadoAdminUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FluxoStatusChamadoService(),
            new AcoesChamadoService(new FluxoStatusChamadoService()),
            new FakeUsuarioContextoAplicacaoService(dados.ContextoAdmin));

        var response = await useCase.ExecutarAsync(dados.Chamado.Id);

        Assert.True(response.RequerAprovacao);
        Assert.True(response.AprovacaoPendente);
        Assert.Equal(StatusAprovacaoChamado.Pendente, response.StatusAprovacao);
        Assert.Equal(aprovacao.Id, response.AprovacaoChamadoId);
    }

    [Fact]
    public async Task DetalheAdminDeveRefletirAprovacaoAprovada()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var aprovacao = new AprovacaoChamado(
            dados.Chamado.Id,
            TipoOrigemAprovacaoChamado.CatalogoServico,
            dados.Solicitante.Id,
            dados.Solicitante.Login,
            dados.Solicitante.Id,
            "Servico teste",
            "Aprovacao automatica por catalogo");
        aprovacao.Aprovar(dados.Solicitante.Id, dados.Solicitante.Id, dados.Solicitante.Login, "Aprovado");
        context.AprovacoesChamado.Add(aprovacao);
        await context.SaveChangesAsync();

        var useCase = new DetalharChamadoAdminUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FluxoStatusChamadoService(),
            new AcoesChamadoService(new FluxoStatusChamadoService()),
            new FakeUsuarioContextoAplicacaoService(dados.ContextoAdmin));

        var response = await useCase.ExecutarAsync(dados.Chamado.Id);

        Assert.True(response.RequerAprovacao);
        Assert.False(response.AprovacaoPendente);
        Assert.Equal(StatusAprovacaoChamado.Aprovado, response.StatusAprovacao);
        Assert.Equal(aprovacao.Id, response.AprovacaoChamadoId);
    }

    [Fact]
    public async Task DetalheAdminDeveRetornarStatusPermitidosConformeNatureza()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, NaturezaChamadoEnum.EventoAlerta);

        var useCase = new DetalharChamadoAdminUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FluxoStatusChamadoService(),
            new AcoesChamadoService(new FluxoStatusChamadoService()),
            new FakeUsuarioContextoAplicacaoService(dados.ContextoAdmin));

        var response = await useCase.ExecutarAsync(dados.Chamado.Id);

        Assert.Equal(NaturezaChamadoEnum.EventoAlerta, response.NaturezaChamado);
        Assert.DoesNotContain((int)StatusChamadoEnum.AguardandoSolicitante, response.StatusPermitidosCodigos);
        Assert.DoesNotContain((int)StatusChamadoEnum.EmAtendimento, response.StatusPermitidosCodigos);
        Assert.Contains((int)StatusChamadoEnum.EmAnalise, response.StatusPermitidosCodigos);
        Assert.Contains((int)StatusChamadoEnum.Correlacionado, response.StatusPermitidosCodigos);
        Assert.Contains((int)StatusChamadoEnum.Tratado, response.StatusPermitidosCodigos);
        Assert.Contains((int)StatusChamadoEnum.Encerrado, response.StatusPermitidosCodigos);
    }

    [Fact]
    public async Task DetalheAdminAceitaChamadoComGrupoEFilaNulos()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = new DetalharChamadoAdminUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FluxoStatusChamadoService(),
            new AcoesChamadoService(new FluxoStatusChamadoService()),
            new FakeUsuarioContextoAplicacaoService(dados.ContextoAdmin));

        var response = await useCase.ExecutarAsync(dados.Chamado.Id);

        Assert.Null(dados.Chamado.GrupoTecnicoId);
        Assert.Null(dados.Chamado.FilaAtendimentoId);
        Assert.Null(dados.Chamado.ResponsavelId);
        Assert.Equal(dados.Chamado.Id, response.Id);
        Assert.Null(response.GrupoTecnicoId);
        Assert.Null(response.GrupoTecnicoNome);
        Assert.Null(response.FilaAtendimentoId);
        Assert.Null(response.FilaAtendimentoNome);
        Assert.Null(response.Responsavel);
    }

    [Fact]
    public async Task DetalheAdminRetornaGrupoEFilaQuandoPreenchidos()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var (grupo, fila) = await CriarGrupoEFilaAsync(context);
        dados.Chamado.DefinirGrupoTecnico(grupo.Id, "teste");
        dados.Chamado.DefinirFilaAtendimento(fila.Id, "teste");
        await context.SaveChangesAsync();

        var useCase = new DetalharChamadoAdminUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FluxoStatusChamadoService(),
            new AcoesChamadoService(new FluxoStatusChamadoService()),
            new FakeUsuarioContextoAplicacaoService(dados.ContextoAdmin));

        var response = await useCase.ExecutarAsync(dados.Chamado.Id);

        Assert.Equal(grupo.Id, response.GrupoTecnicoId);
        Assert.Equal("Grupo Detalhe", response.GrupoTecnicoNome);
        Assert.Equal(fila.Id, response.FilaAtendimentoId);
        Assert.Equal("Fila Detalhe", response.FilaAtendimentoNome);
    }

    [Fact]
    public async Task DetalheAdminExibeResponsavelSemAlterarResponsavelDoChamado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var responsavel = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Responsavel Detalhe", "responsavel.det@empresa.com", TipoPerfil.Atendente);
        dados.Chamado.AtribuirResponsavel(responsavel.Id, "teste");
        await context.SaveChangesAsync();

        var useCase = new DetalharChamadoAdminUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FluxoStatusChamadoService(),
            new AcoesChamadoService(new FluxoStatusChamadoService()),
            new FakeUsuarioContextoAplicacaoService(dados.ContextoAdmin));

        var response = await useCase.ExecutarAsync(dados.Chamado.Id);

        Assert.NotNull(response.Responsavel);
        Assert.Equal(responsavel.Id, response.Responsavel.Id);
        Assert.Equal(responsavel.Id, context.Chamados.Single(x => x.Id == dados.Chamado.Id).ResponsavelId);
    }

    private static async Task<(Chamado Chamado, Usuario Solicitante, UsuarioContextoAplicacao ContextoAdmin)> SeedAsync(
        SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context,
        NaturezaChamadoEnum naturezaChamado = NaturezaChamadoEnum.Requisicao)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "admin.det@empresa.com", TipoPerfil.Administrador);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante", "sol.det@empresa.com", TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Infra");
        var subcategoria = new SubcategoriaChamado(categoria.Id, "Acesso", null, "teste");
        var tipoSolicitacao = new TipoSolicitacao("Incidente", null, "teste");
        var localUnidade = new LocalUnidade("Matriz", null, null, "teste");
        context.SubcategoriasChamado.Add(subcategoria);
        context.TiposSolicitacao.Add(tipoSolicitacao);
        context.LocaisUnidade.Add(localUnidade);
        await context.SaveChangesAsync();

        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(
            context,
            solicitante,
            categoria,
            StatusChamadoEnum.Aberto,
            null,
            "DET",
            subcategoriaId: subcategoria.Id,
            tipoSolicitacaoId: tipoSolicitacao.Id,
            localUnidadeId: localUnidade.Id,
            naturezaChamado: naturezaChamado);

        subcategoria.Desativar("teste");
        tipoSolicitacao.Desativar("teste");
        localUnidade.Desativar("teste");
        await context.SaveChangesAsync();

        var historicoCriacao = new HistoricoChamado(
            chamado.Id,
            TipoHistoricoChamado.Criado,
            "Chamado criado pelo portal",
            solicitante.Id,
            "teste");

        var anexo = new AnexoChamado(
            chamado.Id,
            "evidencia.pdf",
            "evidencia_armazenada.pdf",
            "application/pdf",
            1024,
            "storage/anexos/evidencia_armazenada.pdf",
            solicitante.Id,
            "teste");

        context.HistoricosChamado.Add(historicoCriacao);
        context.AnexosChamado.Add(anexo);
        await context.SaveChangesAsync();

        return (chamado, solicitante, AdminUseCasesTestFactory.Contexto(admin, "Administrador"));
    }

    private static async Task<(GrupoTecnico Grupo, FilaAtendimento Fila)> CriarGrupoEFilaAsync(
        SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context)
    {
        var grupo = new GrupoTecnico("Grupo Detalhe", "Grupo para teste de detalhe", "teste");
        context.GruposTecnicos.Add(grupo);
        await context.SaveChangesAsync();

        var fila = new FilaAtendimento(grupo.Id, "Fila Detalhe", "Fila para teste de detalhe", "teste");
        context.FilasAtendimento.Add(fila);
        await context.SaveChangesAsync();

        return (grupo, fila);
    }
}
