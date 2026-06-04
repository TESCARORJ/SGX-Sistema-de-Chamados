using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class ChamadoRelacionamentoEndpointsIntegrationTests : IClassFixture<ApiIntegrationTestFactory>
{
    private readonly ApiIntegrationTestFactory _factory;

    public ChamadoRelacionamentoEndpointsIntegrationTests(ApiIntegrationTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task DeveRetornarOkEListarRelacionamentosOndeChamadoEOrigem()
    {
        var (chamadoOrigemId, chamadoDestinoId, _) = await SeedCadeiaDeChamadosAsync();
        var relacionamentoId = await CriarRelacionamentoAsync(chamadoOrigemId, chamadoDestinoId, TipoRelacionamentoChamadoEnum.Bloqueia);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.{Guid.NewGuid():N}@empresa.com", "Admin Relacionamento", "Administrador");

        var response = await client.GetAsync($"/api/admin/chamados/{chamadoOrigemId}/relacionamentos");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<List<ChamadoRelacionamentoAdminResponse>>();
        Assert.NotNull(payload);
        Assert.Single(payload);
        Assert.Equal(relacionamentoId, payload[0].Id);
        Assert.Equal(chamadoOrigemId, payload[0].ChamadoOrigemId);
        Assert.Equal(chamadoDestinoId, payload[0].ChamadoDestinoId);
        Assert.Equal("Bloqueia", payload[0].TipoRelacionamentoDescricao);
        Assert.True(payload[0].Ativo);
        Assert.NotNull(payload[0].ChamadoOrigemCodigo);
        Assert.NotNull(payload[0].ChamadoDestinoCodigo);
    }

    [Fact]
    public async Task DeveRetornarOkEListarRelacionamentosOndeChamadoEDestino()
    {
        var (chamadoOrigemId, chamadoDestinoId, _) = await SeedCadeiaDeChamadosAsync();
        var relacionamentoId = await CriarRelacionamentoAsync(chamadoOrigemId, chamadoDestinoId, TipoRelacionamentoChamadoEnum.Bloqueia);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.{Guid.NewGuid():N}@empresa.com", "Admin Relacionamento", "Administrador");

        var response = await client.GetAsync($"/api/admin/chamados/{chamadoDestinoId}/relacionamentos");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<List<ChamadoRelacionamentoAdminResponse>>();
        Assert.NotNull(payload);
        Assert.Single(payload);
        Assert.Equal(relacionamentoId, payload[0].Id);
        Assert.Equal(chamadoOrigemId, payload[0].ChamadoOrigemId);
        Assert.Equal(chamadoDestinoId, payload[0].ChamadoDestinoId);
    }

    [Fact]
    public async Task DeveRetornarApenasRelacionamentosAtivosPorPadrao()
    {
        var (chamadoOrigemId, chamadoDestinoId, _) = await SeedCadeiaDeChamadosAsync();
        var relacionamentoId = await CriarRelacionamentoAsync(chamadoOrigemId, chamadoDestinoId, TipoRelacionamentoChamadoEnum.Bloqueia, ativo: false);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.{Guid.NewGuid():N}@empresa.com", "Admin Relacionamento", "Administrador");

        var response = await client.GetAsync($"/api/admin/chamados/{chamadoOrigemId}/relacionamentos");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<List<ChamadoRelacionamentoAdminResponse>>();
        Assert.NotNull(payload);
        Assert.Empty(payload);
    }

    [Fact]
    public async Task DeveRetornarRelacionamentosInativosQuandoIncluirInativosForTrue()
    {
        var (chamadoOrigemId, chamadoDestinoId, _) = await SeedCadeiaDeChamadosAsync();
        var relacionamentoId = await CriarRelacionamentoAsync(chamadoOrigemId, chamadoDestinoId, TipoRelacionamentoChamadoEnum.Bloqueia, ativo: false, motivo: "Motivo remocao teste");

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.{Guid.NewGuid():N}@empresa.com", "Admin Relacionamento", "Administrador");

        var response = await client.GetAsync($"/api/admin/chamados/{chamadoOrigemId}/relacionamentos?incluirInativos=true");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<List<ChamadoRelacionamentoAdminResponse>>();
        Assert.NotNull(payload);
        Assert.Single(payload);
        Assert.Equal(relacionamentoId, payload[0].Id);
        Assert.False(payload[0].Ativo);
        Assert.Equal("Motivo remocao teste", payload[0].MotivoRemocao);
    }

    [Fact]
    public async Task DeveIgnorarRelacionamentosDeOutrosChamados()
    {
        var (chamadoOrigemId, chamadoDestinoId, chamadoOutroId) = await SeedCadeiaDeChamadosAsync();
        var relacionamentoId = await CriarRelacionamentoAsync(chamadoOrigemId, chamadoDestinoId, TipoRelacionamentoChamadoEnum.Bloqueia);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.{Guid.NewGuid():N}@empresa.com", "Admin Relacionamento", "Administrador");

        var response = await client.GetAsync($"/api/admin/chamados/{chamadoOutroId}/relacionamentos");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<List<ChamadoRelacionamentoAdminResponse>>();
        Assert.NotNull(payload);
        Assert.Empty(payload);
    }

    [Fact]
    public async Task DeveRetornarListaVaziaSeChamadoNaoPossuirRelacionamentos()
    {
        var (_, _, chamadoOutroId) = await SeedCadeiaDeChamadosAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.{Guid.NewGuid():N}@empresa.com", "Admin Relacionamento", "Administrador");

        var response = await client.GetAsync($"/api/admin/chamados/{chamadoOutroId}/relacionamentos");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<List<ChamadoRelacionamentoAdminResponse>>();
        Assert.NotNull(payload);
        Assert.Empty(payload);
    }

    [Fact]
    public async Task DeveRetornarErro404SeChamadoNaoExistir()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.{Guid.NewGuid():N}@empresa.com", "Admin Relacionamento", "Administrador");

        var response = await client.GetAsync($"/api/admin/chamados/{Guid.NewGuid()}/relacionamentos");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var payload = await response.Content.ReadAsStringAsync();
        Assert.Contains("Chamado nao encontrado.", payload, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeveBloquearAcessoSeUsuarioNaoForAtendenteOuAdmin()
    {
        var (chamadoOrigemId, _, _) = await SeedCadeiaDeChamadosAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"solic.{Guid.NewGuid():N}@empresa.com", "Solicitante Teste", "Solicitante");

        var response = await client.GetAsync($"/api/admin/chamados/{chamadoOrigemId}/relacionamentos");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeveBloquearListagemSeUsuarioNaoEstiverAutenticado()
    {
        var (chamadoOrigemId, _, _) = await SeedCadeiaDeChamadosAsync();

        using var client = _factory.CreateClient();
        AddInvalidBearer(client);

        var response = await client.GetAsync($"/api/admin/chamados/{chamadoOrigemId}/relacionamentos");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeveCriarRelacionamentoValidoERetornarDto()
    {
        var (chamadoOrigemId, chamadoDestinoId, _) = await SeedCadeiaDeChamadosAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.criar.rel.{Guid.NewGuid():N}@empresa.com", "Admin Criar Rel", "Administrador");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{chamadoOrigemId}/relacionamentos", new
        {
            chamadoDestinoId,
            tipoRelacionamento = TipoRelacionamentoChamadoEnum.Relacionado,
            justificativa = "Chamado relacionado por causa comum identificada na triagem."
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<ChamadoRelacionamentoAdminResponse>();
        Assert.NotNull(payload);
        Assert.NotEqual(Guid.Empty, payload.Id);
        Assert.Equal(chamadoOrigemId, payload.ChamadoOrigemId);
        Assert.Equal(chamadoDestinoId, payload.ChamadoDestinoId);
        Assert.Equal(TipoRelacionamentoChamadoEnum.Relacionado, payload.TipoRelacionamento);
        Assert.Equal("Relacionado", payload.TipoRelacionamentoDescricao);
        Assert.Equal("Chamado relacionado por causa comum identificada na triagem.", payload.Justificativa);
        Assert.True(payload.Ativo);
        Assert.False(string.IsNullOrWhiteSpace(payload.ChamadoOrigemCodigo));
        Assert.False(string.IsNullOrWhiteSpace(payload.ChamadoDestinoCodigo));
        Assert.False(string.IsNullOrWhiteSpace(payload.CriadoPor));
    }

    [Fact]
    public async Task DeveUsarChamadoIdDaRotaComoOrigem()
    {
        var (chamadoOrigemId, chamadoDestinoId, chamadoOutroId) = await SeedCadeiaDeChamadosAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.origem.rota.{Guid.NewGuid():N}@empresa.com", "Admin Origem Rota", "Administrador");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{chamadoOrigemId}/relacionamentos", new
        {
            chamadoOrigemId = chamadoOutroId,
            chamadoDestinoId,
            tipoRelacionamento = TipoRelacionamentoChamadoEnum.Duplicado,
            justificativa = "Origem divergente no body deve ser ignorada."
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<ChamadoRelacionamentoAdminResponse>();
        Assert.NotNull(payload);
        Assert.Equal(chamadoOrigemId, payload.ChamadoOrigemId);
        Assert.NotEqual(chamadoOutroId, payload.ChamadoOrigemId);
        Assert.Equal(chamadoDestinoId, payload.ChamadoDestinoId);
    }

    [Fact]
    public async Task DeveBloquearCriacaoComOrigemIgualAoDestino()
    {
        var (chamadoOrigemId, _, _) = await SeedCadeiaDeChamadosAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.origem.destino.{Guid.NewGuid():N}@empresa.com", "Admin Origem Destino", "Administrador");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{chamadoOrigemId}/relacionamentos", new
        {
            chamadoDestinoId = chamadoOrigemId,
            tipoRelacionamento = TipoRelacionamentoChamadoEnum.Relacionado,
            justificativa = "Mesmo chamado."
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var payload = await response.Content.ReadAsStringAsync();
        Assert.Contains("origem nao pode ser igual", payload, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeveBloquearCriacaoDeRelacionamentoDuplicado()
    {
        var (chamadoOrigemId, chamadoDestinoId, _) = await SeedCadeiaDeChamadosAsync();
        await CriarRelacionamentoAsync(chamadoOrigemId, chamadoDestinoId, TipoRelacionamentoChamadoEnum.Bloqueia);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.dup.rel.{Guid.NewGuid():N}@empresa.com", "Admin Duplicado Rel", "Administrador");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{chamadoOrigemId}/relacionamentos", new
        {
            chamadoDestinoId,
            tipoRelacionamento = TipoRelacionamentoChamadoEnum.Bloqueia,
            justificativa = "Duplicado."
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var payload = await response.Content.ReadAsStringAsync();
        Assert.Contains("Ja existe um relacionamento ativo", payload, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeveBloquearCriacaoDeRelacionamentoCircularIndevido()
    {
        var (chamadoOrigemId, chamadoDestinoId, _) = await SeedCadeiaDeChamadosAsync();
        await CriarRelacionamentoAsync(chamadoOrigemId, chamadoDestinoId, TipoRelacionamentoChamadoEnum.Bloqueia);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.ciclo.rel.{Guid.NewGuid():N}@empresa.com", "Admin Ciclo Rel", "Administrador");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{chamadoDestinoId}/relacionamentos", new
        {
            chamadoDestinoId = chamadoOrigemId,
            tipoRelacionamento = TipoRelacionamentoChamadoEnum.Bloqueia,
            justificativa = "Ciclo indevido."
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var payload = await response.Content.ReadAsStringAsync();
        Assert.Contains("ciclo indevido", payload, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeveRetornar404AoCriarSeChamadoOrigemNaoExistir()
    {
        var (_, chamadoDestinoId, _) = await SeedCadeiaDeChamadosAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.origem.inexistente.{Guid.NewGuid():N}@empresa.com", "Admin Origem Inexistente", "Administrador");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{Guid.NewGuid()}/relacionamentos", new
        {
            chamadoDestinoId,
            tipoRelacionamento = TipoRelacionamentoChamadoEnum.Relacionado,
            justificativa = "Origem inexistente."
        });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var payload = await response.Content.ReadAsStringAsync();
        Assert.Contains("Chamado de origem nao encontrado.", payload, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeveRetornar404AoCriarSeChamadoDestinoNaoExistir()
    {
        var (chamadoOrigemId, _, _) = await SeedCadeiaDeChamadosAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.destino.inexistente.{Guid.NewGuid():N}@empresa.com", "Admin Destino Inexistente", "Administrador");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{chamadoOrigemId}/relacionamentos", new
        {
            chamadoDestinoId = Guid.NewGuid(),
            tipoRelacionamento = TipoRelacionamentoChamadoEnum.Relacionado,
            justificativa = "Destino inexistente."
        });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var payload = await response.Content.ReadAsStringAsync();
        Assert.Contains("Chamado de destino nao encontrado.", payload, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeveRegistrarHistoricoAoCriarRelacionamentoViaEndpoint()
    {
        var (chamadoOrigemId, chamadoDestinoId, _) = await SeedCadeiaDeChamadosAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.hist.rel.{Guid.NewGuid():N}@empresa.com", "Admin Historico Rel", "Administrador");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{chamadoOrigemId}/relacionamentos", new
        {
            chamadoDestinoId,
            tipoRelacionamento = TipoRelacionamentoChamadoEnum.Origina,
            justificativa = "Historico via endpoint."
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();

        Assert.Contains(dbContext.HistoricosChamado, x =>
            x.ChamadoId == chamadoOrigemId &&
            x.Tipo == TipoHistoricoChamado.RelacionamentoCriado &&
            x.Descricao.Contains("Historico via endpoint."));

        Assert.Contains(dbContext.HistoricosChamado, x =>
            x.ChamadoId == chamadoDestinoId &&
            x.Tipo == TipoHistoricoChamado.RelacionamentoRecebido &&
            x.Descricao.Contains("Historico via endpoint."));
    }

    [Fact]
    public async Task DevePermitirCriacaoParaAtendente()
    {
        var (chamadoOrigemId, chamadoDestinoId, _) = await SeedCadeiaDeChamadosAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"atendente.criar.rel.{Guid.NewGuid():N}@empresa.com", "Atendente Criar Rel", "Atendente");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{chamadoOrigemId}/relacionamentos", new
        {
            chamadoDestinoId,
            tipoRelacionamento = TipoRelacionamentoChamadoEnum.Relacionado,
            justificativa = "Criacao por atendente."
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeveBloquearCriacaoSeUsuarioNaoForAtendenteOuAdmin()
    {
        var (chamadoOrigemId, chamadoDestinoId, _) = await SeedCadeiaDeChamadosAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"solic.criar.rel.{Guid.NewGuid():N}@empresa.com", "Solicitante Criar Rel", "Solicitante");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{chamadoOrigemId}/relacionamentos", new
        {
            chamadoDestinoId,
            tipoRelacionamento = TipoRelacionamentoChamadoEnum.Relacionado,
            justificativa = "Solicitante nao autorizado."
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeveBloquearCriacaoSeUsuarioNaoEstiverAutenticado()
    {
        var (chamadoOrigemId, chamadoDestinoId, _) = await SeedCadeiaDeChamadosAsync();

        using var client = _factory.CreateClient();
        AddInvalidBearer(client);

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{chamadoOrigemId}/relacionamentos", new
        {
            chamadoDestinoId,
            tipoRelacionamento = TipoRelacionamentoChamadoEnum.Relacionado,
            justificativa = "Usuario sem autenticacao."
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeveExibirRelacionamentoCriadoNaListagem()
    {
        var (chamadoOrigemId, chamadoDestinoId, _) = await SeedCadeiaDeChamadosAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.listaposcreate.{Guid.NewGuid():N}@empresa.com", "Admin Lista Pos Create", "Administrador");

        var criarResponse = await client.PostAsJsonAsync($"/api/admin/chamados/{chamadoOrigemId}/relacionamentos", new
        {
            chamadoDestinoId,
            tipoRelacionamento = TipoRelacionamentoChamadoEnum.DerivadoDe,
            justificativa = "Listagem apos criacao."
        });

        Assert.Equal(HttpStatusCode.OK, criarResponse.StatusCode);
        var criado = await criarResponse.Content.ReadFromJsonAsync<ChamadoRelacionamentoAdminResponse>();
        Assert.NotNull(criado);

        var listarResponse = await client.GetAsync($"/api/admin/chamados/{chamadoOrigemId}/relacionamentos");

        Assert.Equal(HttpStatusCode.OK, listarResponse.StatusCode);
        var lista = await listarResponse.Content.ReadFromJsonAsync<List<ChamadoRelacionamentoAdminResponse>>();
        Assert.NotNull(lista);
        Assert.Contains(lista, x =>
            x.Id == criado.Id &&
            x.ChamadoOrigemId == chamadoOrigemId &&
            x.ChamadoDestinoId == chamadoDestinoId);
    }

    [Fact]
    public async Task DeveRemoverRelacionamentoAtivoViaEndpointSemExcluirFisicamente()
    {
        var (chamadoOrigemId, chamadoDestinoId, _) = await SeedCadeiaDeChamadosAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.remover.rel.{Guid.NewGuid():N}@empresa.com", "Admin Remover Rel", "Administrador");

        var criarResponse = await client.PostAsJsonAsync($"/api/admin/chamados/{chamadoOrigemId}/relacionamentos", new
        {
            chamadoDestinoId,
            tipoRelacionamento = TipoRelacionamentoChamadoEnum.Bloqueia,
            justificativa = "Criacao preservada antes da remocao."
        });

        Assert.Equal(HttpStatusCode.OK, criarResponse.StatusCode);
        var criado = await criarResponse.Content.ReadFromJsonAsync<ChamadoRelacionamentoAdminResponse>();
        Assert.NotNull(criado);

        var deleteRequest = new HttpRequestMessage(
            HttpMethod.Delete,
            $"/api/admin/chamados/{chamadoOrigemId}/relacionamentos/{criado.Id}")
        {
            Content = JsonContent.Create(new
            {
                motivoRemocao = "Vinculo criado incorretamente durante a triagem."
            })
        };

        var removerResponse = await client.SendAsync(deleteRequest);

        Assert.Equal(HttpStatusCode.NoContent, removerResponse.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        var relacionamento = await dbContext.ChamadosRelacionamentos.SingleAsync(x => x.Id == criado.Id);

        Assert.False(relacionamento.Ativo);
        Assert.NotNull(relacionamento.RemovidoEm);
        Assert.NotNull(relacionamento.RemovidoPorUsuarioId);
        Assert.Equal("Vinculo criado incorretamente durante a triagem.", relacionamento.MotivoRemocao);

        Assert.Contains(dbContext.HistoricosChamado, x =>
            x.ChamadoId == chamadoOrigemId &&
            x.Tipo == TipoHistoricoChamado.RelacionamentoCriado &&
            x.Descricao.Contains("Criacao preservada antes da remocao."));

        Assert.Contains(dbContext.HistoricosChamado, x =>
            x.ChamadoId == chamadoOrigemId &&
            x.Tipo == TipoHistoricoChamado.RelacionamentoRemovido &&
            x.Descricao.Contains("Vinculo criado incorretamente durante a triagem."));

        Assert.Contains(dbContext.HistoricosChamado, x =>
            x.ChamadoId == chamadoDestinoId &&
            x.Tipo == TipoHistoricoChamado.RelacionamentoRemovidoRecebido &&
            x.Descricao.Contains("Vinculo criado incorretamente durante a triagem."));

        var listarAtivosResponse = await client.GetAsync($"/api/admin/chamados/{chamadoOrigemId}/relacionamentos");
        Assert.Equal(HttpStatusCode.OK, listarAtivosResponse.StatusCode);
        var ativos = await listarAtivosResponse.Content.ReadFromJsonAsync<List<ChamadoRelacionamentoAdminResponse>>();
        Assert.NotNull(ativos);
        Assert.DoesNotContain(ativos, x => x.Id == criado.Id);

        var listarInativosResponse = await client.GetAsync($"/api/admin/chamados/{chamadoOrigemId}/relacionamentos?incluirInativos=true");
        Assert.Equal(HttpStatusCode.OK, listarInativosResponse.StatusCode);
        var inativos = await listarInativosResponse.Content.ReadFromJsonAsync<List<ChamadoRelacionamentoAdminResponse>>();
        Assert.NotNull(inativos);
        var relacionamentoInativo = Assert.Single(inativos, x => x.Id == criado.Id);
        Assert.False(relacionamentoInativo.Ativo);
        Assert.Equal("Vinculo criado incorretamente durante a triagem.", relacionamentoInativo.MotivoRemocao);
    }

    [Fact]
    public async Task DeveRetornar404AoRemoverRelacionamentoInexistente()
    {
        var (chamadoOrigemId, _, _) = await SeedCadeiaDeChamadosAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.remover.inexistente.{Guid.NewGuid():N}@empresa.com", "Admin Remover Inexistente", "Administrador");

        var response = await client.DeleteAsync($"/api/admin/chamados/{chamadoOrigemId}/relacionamentos/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var payload = await response.Content.ReadAsStringAsync();
        Assert.Contains("Relacionamento nao encontrado.", payload, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeveRetornar400AoRemoverRelacionamentoJaInativo()
    {
        var (chamadoOrigemId, chamadoDestinoId, _) = await SeedCadeiaDeChamadosAsync();
        var relacionamentoId = await CriarRelacionamentoAsync(
            chamadoOrigemId,
            chamadoDestinoId,
            TipoRelacionamentoChamadoEnum.Relacionado,
            ativo: false,
            motivo: "Inativado antes do endpoint.");

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.remover.inativo.{Guid.NewGuid():N}@empresa.com", "Admin Remover Inativo", "Administrador");

        var response = await client.DeleteAsync($"/api/admin/chamados/{chamadoOrigemId}/relacionamentos/{relacionamentoId}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var payload = await response.Content.ReadAsStringAsync();
        Assert.Contains("Relacionamento ja esta inativo.", payload, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeveRetornar400AoRemoverRelacionamentoQueNaoPertenceAoChamadoDaRota()
    {
        var (chamadoOrigemId, chamadoDestinoId, chamadoOutroId) = await SeedCadeiaDeChamadosAsync();
        var relacionamentoId = await CriarRelacionamentoAsync(
            chamadoOrigemId,
            chamadoDestinoId,
            TipoRelacionamentoChamadoEnum.Relacionado);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.remover.outro.{Guid.NewGuid():N}@empresa.com", "Admin Remover Outro", "Administrador");

        var response = await client.DeleteAsync($"/api/admin/chamados/{chamadoOutroId}/relacionamentos/{relacionamentoId}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var payload = await response.Content.ReadAsStringAsync();
        Assert.Contains("Relacionamento nao pertence ao chamado informado.", payload, StringComparison.OrdinalIgnoreCase);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        var relacionamento = await dbContext.ChamadosRelacionamentos.SingleAsync(x => x.Id == relacionamentoId);
        Assert.True(relacionamento.Ativo);
    }

    [Fact]
    public async Task DevePermitirRemocaoParaAtendente()
    {
        var (chamadoOrigemId, chamadoDestinoId, _) = await SeedCadeiaDeChamadosAsync();
        var relacionamentoId = await CriarRelacionamentoAsync(
            chamadoOrigemId,
            chamadoDestinoId,
            TipoRelacionamentoChamadoEnum.Relacionado);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"atendente.remover.rel.{Guid.NewGuid():N}@empresa.com", "Atendente Remover Rel", "Atendente");

        var response = await client.DeleteAsync($"/api/admin/chamados/{chamadoOrigemId}/relacionamentos/{relacionamentoId}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeveBloquearRemocaoSeUsuarioNaoForAtendenteOuAdmin()
    {
        var (chamadoOrigemId, chamadoDestinoId, _) = await SeedCadeiaDeChamadosAsync();
        var relacionamentoId = await CriarRelacionamentoAsync(
            chamadoOrigemId,
            chamadoDestinoId,
            TipoRelacionamentoChamadoEnum.Relacionado);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"solic.remover.rel.{Guid.NewGuid():N}@empresa.com", "Solicitante Remover Rel", "Solicitante");

        var response = await client.DeleteAsync($"/api/admin/chamados/{chamadoOrigemId}/relacionamentos/{relacionamentoId}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeveBloquearRemocaoSeUsuarioNaoEstiverAutenticado()
    {
        var (chamadoOrigemId, chamadoDestinoId, _) = await SeedCadeiaDeChamadosAsync();
        var relacionamentoId = await CriarRelacionamentoAsync(
            chamadoOrigemId,
            chamadoDestinoId,
            TipoRelacionamentoChamadoEnum.Relacionado);

        using var client = _factory.CreateClient();
        AddInvalidBearer(client);

        var response = await client.DeleteAsync($"/api/admin/chamados/{chamadoOrigemId}/relacionamentos/{relacionamentoId}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private async Task<(Guid ChamadoOrigemId, Guid ChamadoDestinoId, Guid ChamadoOutroId)> SeedCadeiaDeChamadosAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();

        var solicitante = new Usuario(
            "Solicitante Rel Endpoints",
            $"sol.rel.endpoints.{Guid.NewGuid():N}@empresa.com",
            $"sol.rel.endpoints.{Guid.NewGuid():N}@empresa.com",
            "integration-test");
        dbContext.Usuarios.Add(solicitante);
        await dbContext.SaveChangesAsync(cancellationToken);

        var perfilSolicitante = await dbContext.PerfisAcesso.FirstAsync(x => x.TipoPerfil == TipoPerfil.Solicitante, cancellationToken);
        dbContext.UsuariosPerfisAcesso.Add(new UsuarioPerfilAcesso(solicitante.Id, perfilSolicitante.Id, "integration-test"));
        await dbContext.SaveChangesAsync(cancellationToken);

        var departamento = await dbContext.Departamentos.FirstOrDefaultAsync(cancellationToken);
        if (departamento is null)
        {
            departamento = new Departamento("Operacoes Rel", "OPR", "Departamento de operacoes rel", "integration-test");
            dbContext.Departamentos.Add(departamento);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var categoria = await dbContext.CategoriasChamado.FirstOrDefaultAsync(cancellationToken);
        if (categoria is null)
        {
            categoria = new CategoriaChamado("Solicitacoes Rel Gerais", "Categoria de apoio rel", departamento.Id, "integration-test");
            dbContext.CategoriasChamado.Add(categoria);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var prioridade = await dbContext.PrioridadesChamado.FirstAsync(cancellationToken);
        var status = await dbContext.StatusChamado.FirstAsync(x => x.Codigo == StatusChamadoEnum.Aberto, cancellationToken);

        var chamadoOrigem = new Chamado(
            $"SGX-RO-{Guid.NewGuid():N}".ToUpperInvariant()[..20],
            "Chamado Origem Relacao",
            "Descricao",
            solicitante.Id,
            categoria.Id,
            prioridade.Id,
            status.Id,
            OrigemChamado.Portal,
            "integration-test");

        var chamadoDestino = new Chamado(
            $"SGX-RD-{Guid.NewGuid():N}".ToUpperInvariant()[..20],
            "Chamado Destino Relacao",
            "Descricao",
            solicitante.Id,
            categoria.Id,
            prioridade.Id,
            status.Id,
            OrigemChamado.Portal,
            "integration-test");

        var chamadoOutro = new Chamado(
            $"SGX-RC-{Guid.NewGuid():N}".ToUpperInvariant()[..20],
            "Chamado Outro Relacao",
            "Descricao",
            solicitante.Id,
            categoria.Id,
            prioridade.Id,
            status.Id,
            OrigemChamado.Portal,
            "integration-test");

        dbContext.Chamados.AddRange(chamadoOrigem, chamadoDestino, chamadoOutro);
        await dbContext.SaveChangesAsync(cancellationToken);

        return (chamadoOrigem.Id, chamadoDestino.Id, chamadoOutro.Id);
    }

    private async Task<Guid> CriarRelacionamentoAsync(Guid chamadoOrigemId, Guid chamadoDestinoId, TipoRelacionamentoChamadoEnum tipo, bool ativo = true, string? motivo = null)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        var admin = await dbContext.Usuarios.FirstAsync(x => x.Login.Contains("admin"));

        var relacionamento = new ChamadoRelacionamento(
            chamadoOrigemId,
            chamadoDestinoId,
            tipo,
            admin.Id,
            admin.Login,
            "Justificativa de integracao");

        if (!ativo)
        {
            relacionamento.Inativar(admin.Id, admin.Login, motivo);
        }

        dbContext.ChamadosRelacionamentos.Add(relacionamento);
        await dbContext.SaveChangesAsync();

        return relacionamento.Id;
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

    private static void AddInvalidBearer(HttpClient client)
    {
        client.DefaultRequestHeaders.Remove("X-Dev-User-Email");
        client.DefaultRequestHeaders.Remove("X-Dev-User-Name");
        client.DefaultRequestHeaders.Remove("X-Dev-User-Role");
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "token-invalido");
    }
}
