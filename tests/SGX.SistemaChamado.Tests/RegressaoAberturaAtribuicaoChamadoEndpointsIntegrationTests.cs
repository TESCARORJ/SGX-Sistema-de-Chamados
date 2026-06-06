using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class RegressaoAberturaAtribuicaoChamadoEndpointsIntegrationTests : IClassFixture<ApiIntegrationTestFactory>
{
    private readonly ApiIntegrationTestFactory _factory;

    public RegressaoAberturaAtribuicaoChamadoEndpointsIntegrationTests(ApiIntegrationTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task FluxoLegadoAberturaListagemDetalheAssumirEAtribuirPreservaGrupoFilaNulos()
    {
        var dados = await SeedContextoAberturaAsync();
        var solicitanteEmail = $"sol.regressao.{Guid.NewGuid():N}@empresa.com";
        var atendenteEmail = $"aten.regressao.{Guid.NewGuid():N}@empresa.com";
        var adminEmail = $"admin.regressao.{Guid.NewGuid():N}@empresa.com";
        var tecnicoDestinoId = await _factory.GarantirUsuarioLocalComSenhaAsync(
            $"tec.destino.regressao.{Guid.NewGuid():N}@empresa.com",
            "Tecnico Destino Regressao",
            "Senha@123456",
            TipoPerfil.Atendente);

        using var portalClient = _factory.CreateClient();
        AddDevHeaders(portalClient, solicitanteEmail, "Solicitante Regressao", "Solicitante");
        _ = await portalClient.GetAsync("/api/me");

        var abertura = await portalClient.PostAsJsonAsync("/api/portal/chamados", new
        {
            titulo = $"Regressao abertura {Guid.NewGuid():N}",
            descricao = "Chamado de regressao aberto sem grupo tecnico e sem fila de atendimento.",
            categoriaId = dados.CategoriaId,
            subcategoriaId = dados.SubcategoriaId,
            prioridadeId = dados.PrioridadeId,
            tipoSolicitacaoId = dados.TipoSolicitacaoId,
            localUnidadeId = dados.LocalUnidadeId,
            naturezaChamado = NaturezaChamadoEnum.Incidente,
            impactoChamado = ImpactoChamadoEnum.Baixo,
            urgenciaChamado = UrgenciaChamadoEnum.Baixa
        });

        Assert.Equal(HttpStatusCode.Created, abertura.StatusCode);
        var chamadoCriado = await abertura.Content.ReadFromJsonAsync<ChamadoCriadoApiResponse>();
        Assert.NotNull(chamadoCriado);

        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
            var chamado = await dbContext.Chamados
                .Include(x => x.Historicos)
                .Include(x => x.ChamadoSla)
                .SingleAsync(x => x.Id == chamadoCriado.Id);

            Assert.Null(chamado.GrupoTecnicoId);
            Assert.Null(chamado.FilaAtendimentoId);
            Assert.Null(chamado.ResponsavelId);
            Assert.Equal(NaturezaChamadoEnum.Incidente, chamado.NaturezaChamado);
            Assert.Equal(ImpactoChamadoEnum.Baixo, chamado.ImpactoChamado);
            Assert.Equal(UrgenciaChamadoEnum.Baixa, chamado.UrgenciaChamado);
            Assert.Contains(chamado.Historicos, x => x.Tipo == TipoHistoricoChamado.Criado);
        }

        using var adminClient = _factory.CreateClient();
        AddDevHeaders(adminClient, adminEmail, "Admin Regressao", "Administrador");
        _ = await adminClient.GetAsync("/api/me");

        var listagem = await adminClient.GetAsync($"/api/admin/chamados?texto={chamadoCriado.Codigo}&pagina=1&tamanhoPagina=10");
        Assert.Equal(HttpStatusCode.OK, listagem.StatusCode);
        var listagemPayload = await listagem.Content.ReadFromJsonAsync<ListaChamadosApiResponse>();
        Assert.NotNull(listagemPayload);
        var item = Assert.Single(listagemPayload.Items);
        Assert.Equal(chamadoCriado.Id, item.Id);
        Assert.Null(item.GrupoTecnicoId);
        Assert.Null(item.FilaAtendimentoId);

        var detalheAdmin = await adminClient.GetAsync($"/api/admin/chamados/{chamadoCriado.Id}");
        Assert.Equal(HttpStatusCode.OK, detalheAdmin.StatusCode);
        var detalhePayload = await detalheAdmin.Content.ReadFromJsonAsync<ChamadoDetalheApiResponse>();
        Assert.NotNull(detalhePayload);
        Assert.Null(detalhePayload.GrupoTecnicoId);
        Assert.Null(detalhePayload.FilaAtendimentoId);
        Assert.Null(detalhePayload.Responsavel);

        var detalhePortal = await portalClient.GetAsync($"/api/portal/chamados/{chamadoCriado.Id}");
        Assert.Equal(HttpStatusCode.OK, detalhePortal.StatusCode);

        using var atendenteClient = _factory.CreateClient();
        AddDevHeaders(atendenteClient, atendenteEmail, "Atendente Regressao", "Atendente");
        _ = await atendenteClient.GetAsync("/api/me");

        var assumir = await atendenteClient.PostAsync($"/api/admin/chamados/{chamadoCriado.Id}/assumir", null);
        Assert.Equal(HttpStatusCode.OK, assumir.StatusCode);
        var chamadoAssumido = await assumir.Content.ReadFromJsonAsync<ChamadoDetalheApiResponse>();
        Assert.NotNull(chamadoAssumido);
        Assert.NotNull(chamadoAssumido.Responsavel);
        Assert.Null(chamadoAssumido.GrupoTecnicoId);
        Assert.Null(chamadoAssumido.FilaAtendimentoId);

        var atribuir = await adminClient.PostAsJsonAsync($"/api/admin/chamados/{chamadoCriado.Id}/atribuir", new
        {
            responsavelId = tecnicoDestinoId
        });

        Assert.Equal(HttpStatusCode.OK, atribuir.StatusCode);
        var chamadoAtribuido = await atribuir.Content.ReadFromJsonAsync<ChamadoDetalheApiResponse>();
        Assert.NotNull(chamadoAtribuido);
        Assert.Equal(tecnicoDestinoId, chamadoAtribuido.Responsavel?.Id);
        Assert.Null(chamadoAtribuido.GrupoTecnicoId);
        Assert.Null(chamadoAtribuido.FilaAtendimentoId);

        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
            var chamado = await dbContext.Chamados.AsNoTracking().SingleAsync(x => x.Id == chamadoCriado.Id);
            Assert.Equal(tecnicoDestinoId, chamado.ResponsavelId);
            Assert.Null(chamado.GrupoTecnicoId);
            Assert.Null(chamado.FilaAtendimentoId);

            var historicos = await dbContext.HistoricosChamado
                .AsNoTracking()
                .Where(x => x.ChamadoId == chamadoCriado.Id)
                .ToListAsync();

            Assert.Contains(historicos, x => x.Tipo == TipoHistoricoChamado.Criado);
            Assert.Contains(historicos, x => x.Tipo == TipoHistoricoChamado.ResponsavelAlterado);
            Assert.DoesNotContain(historicos, x =>
                x.Tipo == TipoHistoricoChamado.GrupoTecnicoDefinido ||
                x.Tipo == TipoHistoricoChamado.GrupoTecnicoTransferido ||
                x.Tipo == TipoHistoricoChamado.ChamadoAssumidoDaFila);
        }
    }

    private async Task<DadosAbertura> SeedContextoAberturaAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        var prefixo = $"Regressao {Guid.NewGuid():N}"[..22];

        var categoria = new CategoriaChamado($"Categoria {prefixo}", "Categoria de regressao", null, "integration-test");
        dbContext.CategoriasChamado.Add(categoria);
        await dbContext.SaveChangesAsync(cancellationToken);

        var subcategoria = new SubcategoriaChamado(categoria.Id, $"Subcategoria {prefixo}", null, "integration-test");
        var tipoSolicitacao = new TipoSolicitacao($"Tipo {prefixo}", null, "integration-test");
        var localUnidade = new LocalUnidade($"Local {prefixo}", null, null, "integration-test");
        dbContext.SubcategoriasChamado.Add(subcategoria);
        dbContext.TiposSolicitacao.Add(tipoSolicitacao);
        dbContext.LocaisUnidade.Add(localUnidade);
        await dbContext.SaveChangesAsync(cancellationToken);

        var prioridade = await dbContext.PrioridadesChamado.FirstAsync(x => x.Ativo, cancellationToken);
        return new DadosAbertura(categoria.Id, subcategoria.Id, prioridade.Id, tipoSolicitacao.Id, localUnidade.Id);
    }

    private static void AddDevHeaders(HttpClient client, string email, string nome, string role)
    {
        client.DefaultRequestHeaders.Remove("X-Dev-User-Email");
        client.DefaultRequestHeaders.Remove("X-Dev-User-Name");
        client.DefaultRequestHeaders.Remove("X-Dev-User-Role");
        client.DefaultRequestHeaders.Add("X-Dev-User-Email", email);
        client.DefaultRequestHeaders.Add("X-Dev-User-Name", nome);
        client.DefaultRequestHeaders.Add("X-Dev-User-Role", role);
    }

    private sealed record DadosAbertura(
        Guid CategoriaId,
        Guid SubcategoriaId,
        Guid PrioridadeId,
        Guid TipoSolicitacaoId,
        Guid LocalUnidadeId);

    private sealed record ChamadoCriadoApiResponse(Guid Id, string Codigo);

    private sealed class ListaChamadosApiResponse
    {
        public IReadOnlyCollection<ChamadoResumoApiResponse> Items { get; init; } = [];
    }

    private sealed record ChamadoResumoApiResponse(
        Guid Id,
        Guid? GrupoTecnicoId,
        Guid? FilaAtendimentoId);

    private sealed record ResponsavelApiResponse(Guid Id, string Nome, string Email);

    private sealed record ChamadoDetalheApiResponse(
        Guid Id,
        Guid? GrupoTecnicoId,
        Guid? FilaAtendimentoId,
        ResponsavelApiResponse? Responsavel);
}
