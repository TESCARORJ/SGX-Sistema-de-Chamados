using SGX.SistemaChamado.Application.DTOs.Chamados;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Chamados;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class LinhaTempoChamadoUseCasesTests
{
    [Fact]
    public async Task AdministradorVisualizaAberturaComentariosInternosPublicosAnexosEHistorico()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        await PopularEventosBaseAsync(context, dados);

        var useCase = CriarListarUseCase(context, dados.AdminContexto);
        var response = await useCase.ExecutarAsync(dados.ChamadoSolicitante.Id);

        Assert.Contains(response.Items, x => x.Tipo == "abertura");
        Assert.Contains(response.Items, x => x.Tipo == "comentario" && x.Interno);
        Assert.Contains(response.Items, x => x.Tipo == "comentario" && !x.Interno);
        Assert.Contains(response.Items, x => x.Tipo == "anexo");
        Assert.Contains(response.Items, x => x.Tipo == "status");
        Assert.Contains(response.Items, x => x.Tipo == "responsavel");
    }

    [Fact]
    public async Task AtendenteVisualizaAberturaComentariosInternosPublicosAnexosEHistorico()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        await PopularEventosBaseAsync(context, dados);

        var useCase = CriarListarUseCase(context, dados.AtendenteContexto);
        var response = await useCase.ExecutarAsync(dados.ChamadoSolicitante.Id);

        Assert.Contains(response.Items, x => x.Tipo == "abertura");
        Assert.Contains(response.Items, x => x.Tipo == "comentario" && x.Interno);
        Assert.Contains(response.Items, x => x.Tipo == "comentario" && !x.Interno);
        Assert.Contains(response.Items, x => x.Tipo == "anexo");
        Assert.Contains(response.Items, x => x.Tipo == "status");
    }

    [Fact]
    public async Task SolicitanteVisualizaAberturaComentariosPublicosEAnexosDoProprioChamado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        await PopularEventosBaseAsync(context, dados);

        var useCase = CriarListarUseCase(context, dados.SolicitanteContexto);
        var response = await useCase.ExecutarAsync(dados.ChamadoSolicitante.Id);

        Assert.Contains(response.Items, x => x.Tipo == "abertura");
        Assert.Contains(response.Items, x => x.Tipo == "comentario" && !x.Interno);
        Assert.Contains(response.Items, x => x.Tipo == "anexo");
        Assert.DoesNotContain(response.Items, x => x.Interno);
        Assert.DoesNotContain(response.Items, x => x.Tipo == "responsavel");
    }

    [Fact]
    public async Task SolicitanteNaoVisualizaComentarioInterno()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        await PopularEventosBaseAsync(context, dados);

        var useCase = CriarListarUseCase(context, dados.SolicitanteContexto);
        var response = await useCase.ExecutarAsync(dados.ChamadoSolicitante.Id);

        Assert.DoesNotContain(response.Items, x => x.Tipo == "comentario" && x.Interno);
    }

    [Fact]
    public async Task SolicitanteNaoAcessaLinhaDoTempoDeOutroSolicitante()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarListarUseCase(context, dados.SolicitanteContexto);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => useCase.ExecutarAsync(dados.ChamadoOutroSolicitante.Id));
    }

    [Fact]
    public async Task ChamadoInexistenteRetornaNaoEncontrado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var useCase = CriarListarUseCase(context, dados.AdminContexto);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => useCase.ExecutarAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task LinhaDoTempoAceitaChamadoComGrupoEFilaNulos()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        await PopularEventosBaseAsync(context, dados);

        var useCase = CriarListarUseCase(context, dados.AdminContexto);
        var response = await useCase.ExecutarAsync(dados.ChamadoSolicitante.Id);

        Assert.Null(dados.ChamadoSolicitante.GrupoTecnicoId);
        Assert.Null(dados.ChamadoSolicitante.FilaAtendimentoId);
        Assert.Contains(response.Items, x => x.Tipo == "abertura");
        Assert.Contains(response.Items, x => x.Tipo == "status");
    }

    [Fact]
    public async Task LinhaDoTempoRetornaOrdenacaoCronologicaCrescente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        await PopularEventosBaseAsync(context, dados);

        var useCase = CriarListarUseCase(context, dados.AdminContexto);
        var response = await useCase.ExecutarAsync(dados.ChamadoSolicitante.Id);

        var ordenados = response.Items
            .OrderBy(x => x.DataHora)
            .ThenBy(x => x.Tipo, StringComparer.Ordinal)
            .ThenBy(x => x.Id)
            .Select(x => x.Id)
            .ToArray();

        Assert.Equal(ordenados, response.Items.Select(x => x.Id).ToArray());
    }

    [Fact]
    public async Task AnexoNaLinhaDoTempoNaoRetornaCaminhoNemNomeFisicoArmazenado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        await PopularEventosBaseAsync(context, dados);

        var useCase = CriarListarUseCase(context, dados.AdminContexto);
        var response = await useCase.ExecutarAsync(dados.ChamadoSolicitante.Id);
        var itemAnexo = Assert.Single(response.Items, x => x.Tipo == "anexo");

        var propriedades = itemAnexo.GetType().GetProperties().Select(x => x.Name).ToArray();
        Assert.DoesNotContain("Caminho", propriedades);
        Assert.DoesNotContain("NomeArquivoArmazenado", propriedades);
    }

    [Fact]
    public async Task ComentarioInternoMantemFlagInternoTrueParaAdministradorEAtendente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        await PopularEventosBaseAsync(context, dados);

        var useCaseAdmin = CriarListarUseCase(context, dados.AdminContexto);
        var useCaseAtendente = CriarListarUseCase(context, dados.AtendenteContexto);

        var responseAdmin = await useCaseAdmin.ExecutarAsync(dados.ChamadoSolicitante.Id);
        var responseAtendente = await useCaseAtendente.ExecutarAsync(dados.ChamadoSolicitante.Id);

        Assert.Contains(responseAdmin.Items, x => x.Tipo == "comentario" && x.Interno);
        Assert.Contains(responseAtendente.Items, x => x.Tipo == "comentario" && x.Interno);
    }

    [Fact]
    public async Task ComentarioPublicoMantemFlagInternoFalse()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        await PopularEventosBaseAsync(context, dados);

        var useCase = CriarListarUseCase(context, dados.AdminContexto);
        var response = await useCase.ExecutarAsync(dados.ChamadoSolicitante.Id);

        Assert.Contains(response.Items, x => x.Tipo == "comentario" && !x.Interno);
    }

    [Fact]
    public async Task AposComentarioCriadoEleApareceNaLinhaDoTempo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var adicionarComentario = new AdicionarComentarioChamadoUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<ComentarioChamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            SlaTestFactory.CriarService(context),
            new FakeUsuarioContextoAplicacaoService(dados.SolicitanteContexto),
            PortalUseCasesTestFactory.Uow(context));

        await adicionarComentario.ExecutarAsync(
            dados.ChamadoSolicitante.Id,
            new CriarComentarioChamadoRequest { Mensagem = "Comentario novo na timeline", Interno = false });

        var listar = CriarListarUseCase(context, dados.SolicitanteContexto);
        var response = await listar.ExecutarAsync(dados.ChamadoSolicitante.Id);

        Assert.Contains(response.Items, x => x.Tipo == "comentario" && x.Descricao.Contains("Comentario novo na timeline"));
    }

    [Fact]
    public async Task AposAnexoCriadoEleApareceNaLinhaDoTempo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var storage = new FakeArquivoStorageService();

        var adicionarAnexo = new AdicionarAnexoChamadoUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<AnexoChamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            storage,
            new FakeUsuarioContextoAplicacaoService(dados.SolicitanteContexto),
            PortalUseCasesTestFactory.ArquivosOptionsPadrao,
            PortalUseCasesTestFactory.Uow(context));

        await using var stream = new MemoryStream([1, 2, 3, 4]);
        await adicionarAnexo.ExecutarAsync(dados.ChamadoSolicitante.Id, new CriarAnexoChamadoRequest
        {
            NomeArquivo = "timeline-anexo.pdf",
            ContentType = "application/pdf",
            TamanhoBytes = stream.Length,
            Conteudo = stream
        });

        var listar = CriarListarUseCase(context, dados.SolicitanteContexto);
        var response = await listar.ExecutarAsync(dados.ChamadoSolicitante.Id);

        Assert.Contains(response.Items, x => x.Tipo == "anexo" && x.Descricao == "timeline-anexo.pdf");
    }

    [Fact]
    public async Task LinhaDoTempoMapeiaHistoricosDeGrupoFilaEResponsavel()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        context.HistoricosChamado.AddRange(
            new HistoricoChamado(
                dados.ChamadoSolicitante.Id,
                TipoHistoricoChamado.GrupoTecnicoDefinido,
                "Grupo tecnico definido como Service Desk.",
                dados.Atendente.Id,
                "teste"),
            new HistoricoChamado(
                dados.ChamadoSolicitante.Id,
                TipoHistoricoChamado.GrupoTecnicoTransferido,
                "Grupo tecnico transferido de Service Desk para Infraestrutura.",
                dados.Atendente.Id,
                "teste"),
            new HistoricoChamado(
                dados.ChamadoSolicitante.Id,
                TipoHistoricoChamado.FilaAtendimentoDefinida,
                "Fila de atendimento definida como Fila Service Desk.",
                dados.Atendente.Id,
                "teste"),
            new HistoricoChamado(
                dados.ChamadoSolicitante.Id,
                TipoHistoricoChamado.FilaAtendimentoRemovida,
                "Fila de atendimento removida: Fila Service Desk.",
                dados.Atendente.Id,
                "teste"),
            new HistoricoChamado(
                dados.ChamadoSolicitante.Id,
                TipoHistoricoChamado.FilaAtendimentoTransferida,
                "Fila de atendimento transferida de Fila Service Desk para Fila Infraestrutura.",
                dados.Atendente.Id,
                "teste"),
            new HistoricoChamado(
                dados.ChamadoSolicitante.Id,
                TipoHistoricoChamado.ResponsavelRemovidoPorTransferenciaGrupo,
                "Responsavel individual removido pela transferencia de grupo tecnico.",
                dados.Atendente.Id,
                "teste"),
            new HistoricoChamado(
                dados.ChamadoSolicitante.Id,
                TipoHistoricoChamado.ChamadoAssumidoDaFila,
                "Chamado assumido da fila Fila Service Desk por Atendente.",
                dados.Atendente.Id,
                "teste"),
            new HistoricoChamado(
                dados.ChamadoSolicitante.Id,
                TipoHistoricoChamado.ResponsavelAlterado,
                "Responsavel alterado de Atendente para Outro Atendente.",
                dados.Atendente.Id,
                "teste"));
        await context.SaveChangesAsync();

        var useCase = CriarListarUseCase(context, dados.AdminContexto);
        var response = await useCase.ExecutarAsync(dados.ChamadoSolicitante.Id);

        Assert.Contains(response.Items, x => x.Tipo == "grupo-tecnico" && x.Titulo == "Grupo tecnico definido");
        Assert.Contains(response.Items, x => x.Tipo == "grupo-tecnico" && x.Titulo == "Grupo tecnico transferido");
        Assert.Contains(response.Items, x => x.Tipo == "fila-atendimento" && x.Titulo == "Fila de atendimento definida");
        Assert.Contains(response.Items, x => x.Tipo == "fila-atendimento" && x.Titulo == "Fila de atendimento removida");
        Assert.Contains(response.Items, x => x.Tipo == "fila-atendimento" && x.Titulo == "Fila de atendimento transferida");
        Assert.Contains(response.Items, x => x.Tipo == "responsavel" && x.Titulo == "Responsavel removido por transferencia de grupo");
        Assert.Contains(response.Items, x => x.Tipo == "responsavel" && x.Titulo == "Chamado assumido da fila");
        Assert.Contains(response.Items, x => x.Tipo == "responsavel" && x.Titulo == "Responsavel alterado");
        Assert.Contains(response.Items, x => x.Descricao.Contains("Service Desk", StringComparison.Ordinal));
        Assert.Contains(response.Items, x => x.Descricao.Contains("Infraestrutura", StringComparison.Ordinal));
    }

    [Fact]
    public void TipoHistoricoChamadoMantemValoresPersistidosSemReordenacao()
    {
        Assert.Equal(1, (int)TipoHistoricoChamado.Criado);
        Assert.Equal(2, (int)TipoHistoricoChamado.StatusAlterado);
        Assert.Equal(3, (int)TipoHistoricoChamado.PrioridadeAlterada);
        Assert.Equal(4, (int)TipoHistoricoChamado.CategoriaAlterada);
        Assert.Equal(5, (int)TipoHistoricoChamado.ResponsavelAlterado);
        Assert.Equal(6, (int)TipoHistoricoChamado.ComentarioAdicionado);
        Assert.Equal(7, (int)TipoHistoricoChamado.AnexoAdicionado);
        Assert.Equal(8, (int)TipoHistoricoChamado.Encerrado);
        Assert.Equal(9, (int)TipoHistoricoChamado.Reaberto);
        Assert.Equal(10, (int)TipoHistoricoChamado.IntegracaoEmail);
        Assert.Equal(11, (int)TipoHistoricoChamado.ArtigoConhecimentoVinculado);
        Assert.Equal(12, (int)TipoHistoricoChamado.ArtigoConhecimentoDesvinculado);
        Assert.Equal(13, (int)TipoHistoricoChamado.ChamadoCriadoPorCatalogoServico);
        Assert.Equal(14, (int)TipoHistoricoChamado.AtivoVinculado);
        Assert.Equal(15, (int)TipoHistoricoChamado.AtivoRemovido);
        Assert.Equal(16, (int)TipoHistoricoChamado.AprovacaoSolicitada);
        Assert.Equal(17, (int)TipoHistoricoChamado.ChamadoAprovado);
        Assert.Equal(18, (int)TipoHistoricoChamado.ChamadoReprovado);
        Assert.Equal(19, (int)TipoHistoricoChamado.AprovacaoCancelada);
        Assert.Equal(20, (int)TipoHistoricoChamado.RelacionamentoCriado);
        Assert.Equal(21, (int)TipoHistoricoChamado.RelacionamentoRecebido);
        Assert.Equal(22, (int)TipoHistoricoChamado.RelacionamentoRemovido);
        Assert.Equal(23, (int)TipoHistoricoChamado.RelacionamentoRemovidoRecebido);
        Assert.Equal(24, (int)TipoHistoricoChamado.ChamadoDerivadoCriado);
        Assert.Equal(25, (int)TipoHistoricoChamado.CriadoAPartirDeChamado);
        Assert.Equal(26, (int)TipoHistoricoChamado.TarefaCriada);
        Assert.Equal(27, (int)TipoHistoricoChamado.TarefaStatusAlterado);
        Assert.Equal(28, (int)TipoHistoricoChamado.TarefaCancelada);
        Assert.Equal(29, (int)TipoHistoricoChamado.TarefaConcluida);
        Assert.Equal(30, (int)TipoHistoricoChamado.AprovacaoCriada);
        Assert.Equal(31, (int)TipoHistoricoChamado.AprovacaoAprovada);
        Assert.Equal(32, (int)TipoHistoricoChamado.AprovacaoReprovada);
        Assert.Equal(33, (int)TipoHistoricoChamado.GrupoTecnicoTransferido);
        Assert.Equal(34, (int)TipoHistoricoChamado.GrupoTecnicoDefinido);
        Assert.Equal(35, (int)TipoHistoricoChamado.FilaAtendimentoDefinida);
        Assert.Equal(36, (int)TipoHistoricoChamado.FilaAtendimentoRemovida);
        Assert.Equal(37, (int)TipoHistoricoChamado.FilaAtendimentoTransferida);
        Assert.Equal(38, (int)TipoHistoricoChamado.ResponsavelRemovidoPorTransferenciaGrupo);
        Assert.Equal(39, (int)TipoHistoricoChamado.ChamadoAssumidoDaFila);
        Assert.Equal(45, (int)TipoHistoricoChamado.FormularioServicoPreenchidoNaAbertura);
    }

    private static ListarLinhaTempoChamadoUseCase CriarListarUseCase(SGXSistemaChamadoDbContext context, UsuarioContextoAplicacao usuario)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<ComentarioChamado>(context),
            PortalUseCasesTestFactory.Repo<AnexoChamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            new FakeUsuarioContextoAplicacaoService(usuario));

    private static async Task PopularEventosBaseAsync(
        SGXSistemaChamadoDbContext context,
        (Usuario Admin,
            Usuario Atendente,
            Usuario Solicitante,
            Chamado ChamadoSolicitante,
            Chamado ChamadoOutroSolicitante,
            UsuarioContextoAplicacao SolicitanteContexto,
            UsuarioContextoAplicacao AtendenteContexto,
            UsuarioContextoAplicacao AdminContexto) dados)
    {
        context.ComentariosChamado.AddRange(
            new ComentarioChamado(dados.ChamadoSolicitante.Id, dados.Atendente.Id, "Comentario publico", false, "teste"),
            new ComentarioChamado(dados.ChamadoSolicitante.Id, dados.Atendente.Id, "Comentario interno", true, "teste"));

        context.AnexosChamado.Add(
            new AnexoChamado(
                dados.ChamadoSolicitante.Id,
                "evidencia.pdf",
                "arquivo-fisico.pdf",
                "application/pdf",
                2048,
                "storage/anexos/arquivo-fisico.pdf",
                dados.Solicitante.Id,
                "teste"));

        context.HistoricosChamado.AddRange(
            new HistoricoChamado(
                dados.ChamadoSolicitante.Id,
                TipoHistoricoChamado.StatusAlterado,
                "Status alterado para Em atendimento",
                dados.Atendente.Id,
                "teste"),
            new HistoricoChamado(
                dados.ChamadoSolicitante.Id,
                TipoHistoricoChamado.ResponsavelAlterado,
                "Responsavel alterado para Atendente",
                dados.Atendente.Id,
                "teste"));

        await context.SaveChangesAsync();
    }

    private static async Task<(
        Usuario Admin,
        Usuario Atendente,
        Usuario Solicitante,
        Chamado ChamadoSolicitante,
        Chamado ChamadoOutroSolicitante,
        UsuarioContextoAplicacao SolicitanteContexto,
        UsuarioContextoAplicacao AtendenteContexto,
        UsuarioContextoAplicacao AdminContexto)> SeedAsync(SGXSistemaChamadoDbContext context)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "admin.timeline@empresa.com", TipoPerfil.Administrador);
        var atendente = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Atendente", "atendente.timeline@empresa.com", TipoPerfil.Atendente);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante", "solicitante.timeline@empresa.com", TipoPerfil.Solicitante);
        var outroSolicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Outro Solicitante", "outro.timeline@empresa.com", TipoPerfil.Solicitante);

        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Timeline");
        var chamadoSolicitante = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.EmAtendimento, null, "TLN-001");
        var chamadoOutro = await AdminUseCasesTestFactory.CriarChamadoAsync(context, outroSolicitante, categoria, StatusChamadoEnum.EmAtendimento, null, "TLN-002");

        return (
            admin,
            atendente,
            solicitante,
            chamadoSolicitante,
            chamadoOutro,
            AdminUseCasesTestFactory.Contexto(solicitante, "Solicitante"),
            AdminUseCasesTestFactory.Contexto(atendente, "Atendente"),
            AdminUseCasesTestFactory.Contexto(admin, "Administrador"));
    }
}
