using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Tests;

public sealed class GruposTecnicosEndpointsIntegrationTests : IClassFixture<ApiIntegrationTestFactory>
{
    private readonly ApiIntegrationTestFactory _factory;

    public GruposTecnicosEndpointsIntegrationTests(ApiIntegrationTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task AtendenteListaGruposTecnicos()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"atendente.grupos.lista.{Guid.NewGuid():N}@empresa.com", "Atendente Grupos", "Atendente");

        var response = await client.GetAsync("/api/admin/grupos-tecnicos?texto=Service%20Desk");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<PagedResultResponse<GrupoTecnicoResumoResponse>>();
        Assert.NotNull(payload);
        Assert.Contains(payload.Items, x => x.Nome == "Service Desk");
    }

    [Fact]
    public async Task AdministradorListaGruposTecnicos()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.grupos.lista.{Guid.NewGuid():N}@empresa.com", "Admin Grupos", "Administrador");

        var response = await client.GetAsync("/api/admin/grupos-tecnicos?ativo=true");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<PagedResultResponse<GrupoTecnicoResumoResponse>>();
        Assert.NotNull(payload);
        Assert.Contains(payload.Items, x => x.Nome == "Service Desk" && x.Ativo);
    }

    [Fact]
    public async Task AtendenteObtemGrupoTecnicoPorId()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"atendente.grupos.obter.{Guid.NewGuid():N}@empresa.com", "Atendente Grupos", "Atendente");

        var response = await client.GetAsync($"/api/admin/grupos-tecnicos/{SeedData.GrupoTecnicoServiceDeskId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<GrupoTecnicoResponse>();
        Assert.NotNull(payload);
        Assert.Equal(SeedData.GrupoTecnicoServiceDeskId, payload.Id);
        Assert.Equal("Service Desk", payload.Nome);
    }

    [Fact]
    public async Task AdministradorCriaGrupoTecnico()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.grupos.criar.{Guid.NewGuid():N}@empresa.com", "Admin Grupos", "Administrador");
        var nome = $"Grupo Endpoint {Guid.NewGuid():N}";

        var response = await client.PostAsJsonAsync("/api/admin/grupos-tecnicos", new CriarGrupoTecnicoRequest
        {
            Nome = nome,
            Descricao = "Criado por teste de endpoint"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<GrupoTecnicoResponse>();
        Assert.NotNull(payload);
        Assert.Equal(nome, payload.Nome);
        Assert.True(payload.Ativo);
    }

    [Fact]
    public async Task CriarGrupoTecnicoComNomeVazioRetornaErroAmigavel()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.grupos.nomevazio.{Guid.NewGuid():N}@empresa.com", "Admin Grupos", "Administrador");

        var response = await client.PostAsJsonAsync("/api/admin/grupos-tecnicos", new CriarGrupoTecnicoRequest
        {
            Nome = "   ",
            Descricao = "Sem nome"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("O nome do grupo tecnico e obrigatorio.", await response.Content.ReadAsStringAsync(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task CriarGrupoTecnicoDuplicadoRetornaErroAmigavel()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.grupos.duplicado.{Guid.NewGuid():N}@empresa.com", "Admin Grupos", "Administrador");
        var nome = $"Grupo Duplicado {Guid.NewGuid():N}";
        await CriarGrupoAsync(client, nome);

        var response = await client.PostAsJsonAsync("/api/admin/grupos-tecnicos", new CriarGrupoTecnicoRequest
        {
            Nome = nome
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Ja existe grupo tecnico com este nome.", await response.Content.ReadAsStringAsync(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task AtendenteNaoCriaGrupoTecnico()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"atendente.grupos.criar.{Guid.NewGuid():N}@empresa.com", "Atendente Grupos", "Atendente");

        var response = await client.PostAsJsonAsync("/api/admin/grupos-tecnicos", new CriarGrupoTecnicoRequest
        {
            Nome = $"Grupo Bloqueado {Guid.NewGuid():N}"
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AdministradorAtualizaGrupoTecnico()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.grupos.atualizar.{Guid.NewGuid():N}@empresa.com", "Admin Grupos", "Administrador");
        var criado = await CriarGrupoAsync(client);
        var novoNome = $"Grupo Atualizado {Guid.NewGuid():N}";

        var response = await client.PutAsJsonAsync($"/api/admin/grupos-tecnicos/{criado.Id}", new AtualizarGrupoTecnicoRequest
        {
            Nome = novoNome,
            Descricao = "Atualizado por teste de endpoint"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<GrupoTecnicoResponse>();
        Assert.NotNull(payload);
        Assert.Equal(novoNome, payload.Nome);
    }

    [Fact]
    public async Task AtendenteNaoAtualizaGrupoTecnico()
    {
        using var adminClient = _factory.CreateClient();
        AddDevHeaders(adminClient, $"admin.grupos.seed.{Guid.NewGuid():N}@empresa.com", "Admin Grupos", "Administrador");
        var criado = await CriarGrupoAsync(adminClient);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"atendente.grupos.atualizar.{Guid.NewGuid():N}@empresa.com", "Atendente Grupos", "Atendente");

        var response = await client.PutAsJsonAsync($"/api/admin/grupos-tecnicos/{criado.Id}", new AtualizarGrupoTecnicoRequest
        {
            Nome = $"Tentativa Atendente {Guid.NewGuid():N}"
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AdministradorAtualizaStatusGrupoTecnico()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.grupos.status.{Guid.NewGuid():N}@empresa.com", "Admin Grupos", "Administrador");
        var criado = await CriarGrupoAsync(client);

        var response = await client.PatchAsJsonAsync($"/api/admin/grupos-tecnicos/{criado.Id}/status", new AlterarStatusGrupoTecnicoRequest
        {
            Ativo = false
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<AlterarSituacaoCadastroResponse>();
        Assert.NotNull(payload);
        Assert.False(payload.Ativo);
    }

    [Fact]
    public async Task AtendenteNaoAtualizaStatusGrupoTecnico()
    {
        using var adminClient = _factory.CreateClient();
        AddDevHeaders(adminClient, $"admin.grupos.seed.status.{Guid.NewGuid():N}@empresa.com", "Admin Grupos", "Administrador");
        var criado = await CriarGrupoAsync(adminClient);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"atendente.grupos.status.{Guid.NewGuid():N}@empresa.com", "Atendente Grupos", "Atendente");

        var response = await client.PatchAsJsonAsync($"/api/admin/grupos-tecnicos/{criado.Id}/status", new AlterarStatusGrupoTecnicoRequest
        {
            Ativo = false
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AdministradorListaFilasDoGrupoTecnico()
    {
        var dados = await SeedFilasGrupoAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.filas.lista.{Guid.NewGuid():N}@empresa.com", "Admin Filas", "Administrador");

        var response = await client.GetAsync($"/api/admin/grupos-tecnicos/{dados.GrupoId}/filas");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<List<FilaAtendimentoResumoResponse>>();
        Assert.NotNull(payload);
        Assert.Contains(payload, x => x.Id == dados.FilaAtivaId && x.GrupoTecnicoId == dados.GrupoId);
        Assert.Contains(payload, x => x.Id == dados.FilaInativaId && x.GrupoTecnicoId == dados.GrupoId);
        Assert.DoesNotContain(payload, x => x.GrupoTecnicoId == dados.OutroGrupoId);
    }

    [Fact]
    public async Task AtendenteListaFilasDoGrupoTecnicoComFiltroAtivoEBusca()
    {
        var dados = await SeedFilasGrupoAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"atendente.filas.lista.{Guid.NewGuid():N}@empresa.com", "Atendente Filas", "Atendente");

        var response = await client.GetAsync($"/api/admin/grupos-tecnicos/{dados.GrupoId}/filas?ativo=true&busca=Incidentes");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<List<FilaAtendimentoResumoResponse>>();
        Assert.NotNull(payload);
        Assert.Single(payload);
        Assert.Equal(dados.FilaAtivaId, payload.Single().Id);
        Assert.True(payload.Single().Ativo);
    }

    [Fact]
    public async Task SolicitanteNaoListaFilasDoGrupoTecnico()
    {
        var dados = await SeedFilasGrupoAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"solicitante.filas.lista.{Guid.NewGuid():N}@empresa.com", "Solicitante Filas", "Solicitante");

        var response = await client.GetAsync($"/api/admin/grupos-tecnicos/{dados.GrupoId}/filas");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task ListarFilasGrupoInexistenteRetornaNotFound()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.filas.inexistente.{Guid.NewGuid():N}@empresa.com", "Admin Filas", "Administrador");

        var response = await client.GetAsync($"/api/admin/grupos-tecnicos/{Guid.NewGuid()}/filas");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains("Grupo tecnico nao encontrado.", await response.Content.ReadAsStringAsync(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task ListarFilasNaoAlteraChamadosENaoExpoeCadastroFila()
    {
        var dados = await SeedFilasGrupoAsync();
        var chamadosAntes = await ContarChamadosAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.filas.sem.cadastro.{Guid.NewGuid():N}@empresa.com", "Admin Filas", "Administrador");

        var listar = await client.GetAsync($"/api/admin/grupos-tecnicos/{dados.GrupoId}/filas");
        var criar = await client.PostAsJsonAsync($"/api/admin/grupos-tecnicos/{dados.GrupoId}/filas", new { nome = "Fila Nova" });
        var editar = await client.PutAsJsonAsync($"/api/admin/grupos-tecnicos/{dados.GrupoId}/filas/{dados.FilaAtivaId}", new { nome = "Fila Editada" });
        var chamadosDepois = await ContarChamadosAsync();

        Assert.Equal(HttpStatusCode.OK, listar.StatusCode);
        Assert.Equal(HttpStatusCode.MethodNotAllowed, criar.StatusCode);
        Assert.Contains(editar.StatusCode, new[] { HttpStatusCode.NotFound, HttpStatusCode.MethodNotAllowed });
        Assert.Equal(chamadosAntes, chamadosDepois);
    }

    [Fact]
    public async Task AdministradorListaMembrosDoGrupo()
    {
        using var adminClient = _factory.CreateClient();
        AddDevHeaders(adminClient, $"admin.membros.lista.{Guid.NewGuid():N}@empresa.com", "Admin Membros", "Administrador");
        var grupo = await CriarGrupoAsync(adminClient);
        var usuarioId = await CriarUsuarioAtendenteAsync("Membro Lista Admin");
        var membro = await AdicionarMembroAsync(adminClient, grupo.Id, usuarioId);

        var response = await adminClient.GetAsync($"/api/admin/grupos-tecnicos/{grupo.Id}/membros?ativo=true");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<List<MembroGrupoTecnicoResponse>>();
        Assert.NotNull(payload);
        Assert.Contains(payload, x => x.Id == membro.Id && x.UsuarioId == usuarioId && x.Ativo);
    }

    [Fact]
    public async Task AtendenteListaMembrosDoGrupo()
    {
        using var adminClient = _factory.CreateClient();
        AddDevHeaders(adminClient, $"admin.membros.seed.{Guid.NewGuid():N}@empresa.com", "Admin Membros", "Administrador");
        var grupo = await CriarGrupoAsync(adminClient);
        var usuarioId = await CriarUsuarioAtendenteAsync("Membro Lista Atendente");
        await AdicionarMembroAsync(adminClient, grupo.Id, usuarioId);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"atendente.membros.lista.{Guid.NewGuid():N}@empresa.com", "Atendente Membros", "Atendente");

        var response = await client.GetAsync($"/api/admin/grupos-tecnicos/{grupo.Id}/membros");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<List<MembroGrupoTecnicoResponse>>();
        Assert.NotNull(payload);
        Assert.Contains(payload, x => x.UsuarioId == usuarioId);
    }

    [Fact]
    public async Task AdministradorAdicionaMembroAoGrupo()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.membros.adiciona.{Guid.NewGuid():N}@empresa.com", "Admin Membros", "Administrador");
        var grupo = await CriarGrupoAsync(client);
        var usuarioId = await CriarUsuarioAtendenteAsync("Membro Adicionar");

        var response = await client.PostAsJsonAsync($"/api/admin/grupos-tecnicos/{grupo.Id}/membros", new AdicionarMembroGrupoTecnicoRequest
        {
            UsuarioId = usuarioId
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<MembroGrupoTecnicoResponse>();
        Assert.NotNull(payload);
        Assert.Equal(grupo.Id, payload.GrupoTecnicoId);
        Assert.Equal(usuarioId, payload.UsuarioId);
        Assert.True(payload.Ativo);
    }

    [Fact]
    public async Task AtendenteNaoAdicionaMembroAoGrupo()
    {
        using var adminClient = _factory.CreateClient();
        AddDevHeaders(adminClient, $"admin.membros.seed.add.{Guid.NewGuid():N}@empresa.com", "Admin Membros", "Administrador");
        var grupo = await CriarGrupoAsync(adminClient);
        var usuarioId = await CriarUsuarioAtendenteAsync("Membro Bloqueado");

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"atendente.membros.adiciona.{Guid.NewGuid():N}@empresa.com", "Atendente Membros", "Atendente");

        var response = await client.PostAsJsonAsync($"/api/admin/grupos-tecnicos/{grupo.Id}/membros", new AdicionarMembroGrupoTecnicoRequest
        {
            UsuarioId = usuarioId
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AdministradorAlteraStatusDoMembro()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.membros.status.{Guid.NewGuid():N}@empresa.com", "Admin Membros", "Administrador");
        var grupo = await CriarGrupoAsync(client);
        var usuarioId = await CriarUsuarioAtendenteAsync("Membro Status");
        var membro = await AdicionarMembroAsync(client, grupo.Id, usuarioId);

        var response = await client.PatchAsJsonAsync($"/api/admin/grupos-tecnicos/{grupo.Id}/membros/{membro.Id}/status", new AlterarStatusMembroGrupoTecnicoRequest
        {
            Ativo = false
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<MembroGrupoTecnicoResponse>();
        Assert.NotNull(payload);
        Assert.Equal(membro.Id, payload.Id);
        Assert.False(payload.Ativo);
    }

    [Fact]
    public async Task AtendenteNaoAlteraStatusDoMembro()
    {
        using var adminClient = _factory.CreateClient();
        AddDevHeaders(adminClient, $"admin.membros.seed.status.{Guid.NewGuid():N}@empresa.com", "Admin Membros", "Administrador");
        var grupo = await CriarGrupoAsync(adminClient);
        var usuarioId = await CriarUsuarioAtendenteAsync("Membro Status Bloqueado");
        var membro = await AdicionarMembroAsync(adminClient, grupo.Id, usuarioId);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"atendente.membros.status.{Guid.NewGuid():N}@empresa.com", "Atendente Membros", "Atendente");

        var response = await client.PatchAsJsonAsync($"/api/admin/grupos-tecnicos/{grupo.Id}/membros/{membro.Id}/status", new AlterarStatusMembroGrupoTecnicoRequest
        {
            Ativo = false
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task EndpointNaoPermiteDuplicidadeDeMembroAtivo()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.membros.duplicidade.{Guid.NewGuid():N}@empresa.com", "Admin Membros", "Administrador");
        var grupo = await CriarGrupoAsync(client);
        var usuarioId = await CriarUsuarioAtendenteAsync("Membro Duplicado");
        await AdicionarMembroAsync(client, grupo.Id, usuarioId);

        var response = await client.PostAsJsonAsync($"/api/admin/grupos-tecnicos/{grupo.Id}/membros", new AdicionarMembroGrupoTecnicoRequest
        {
            UsuarioId = usuarioId
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var mensagem = await response.Content.ReadAsStringAsync();
        Assert.Contains("Usuario ja e membro ativo deste grupo tecnico.", mensagem, StringComparison.Ordinal);
    }

    [Fact]
    public async Task EndpointReativaMembroInativoSemDuplicarVinculo()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.membros.reativar.{Guid.NewGuid():N}@empresa.com", "Admin Membros", "Administrador");
        var grupo = await CriarGrupoAsync(client);
        var usuarioId = await CriarUsuarioAtendenteAsync("Membro Reativar");
        var membro = await AdicionarMembroAsync(client, grupo.Id, usuarioId);
        var inativar = await client.PatchAsJsonAsync($"/api/admin/grupos-tecnicos/{grupo.Id}/membros/{membro.Id}/status", new AlterarStatusMembroGrupoTecnicoRequest
        {
            Ativo = false
        });
        inativar.EnsureSuccessStatusCode();

        var response = await client.PostAsJsonAsync($"/api/admin/grupos-tecnicos/{grupo.Id}/membros", new AdicionarMembroGrupoTecnicoRequest
        {
            UsuarioId = usuarioId
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<MembroGrupoTecnicoResponse>();
        Assert.NotNull(payload);
        Assert.Equal(membro.Id, payload.Id);
        Assert.True(payload.Ativo);

        var listar = await client.GetAsync($"/api/admin/grupos-tecnicos/{grupo.Id}/membros");
        listar.EnsureSuccessStatusCode();
        var membros = await listar.Content.ReadFromJsonAsync<List<MembroGrupoTecnicoResponse>>();
        Assert.NotNull(membros);
        Assert.Single(membros, x => x.UsuarioId == usuarioId);
    }

    [Fact]
    public async Task AdicionarMembroEmGrupoInexistenteRetornaErroAmigavel()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.membros.grupo.inexistente.{Guid.NewGuid():N}@empresa.com", "Admin Membros", "Administrador");
        var usuarioId = await CriarUsuarioAtendenteAsync("Membro Grupo Inexistente");

        var response = await client.PostAsJsonAsync($"/api/admin/grupos-tecnicos/{Guid.NewGuid()}/membros", new AdicionarMembroGrupoTecnicoRequest
        {
            UsuarioId = usuarioId
        });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains("Grupo tecnico nao encontrado.", await response.Content.ReadAsStringAsync(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task AdicionarMembroEmGrupoInativoRetornaErroAmigavel()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.membros.grupo.inativo.{Guid.NewGuid():N}@empresa.com", "Admin Membros", "Administrador");
        var grupo = await CriarGrupoAsync(client);
        var usuarioId = await CriarUsuarioAtendenteAsync("Membro Grupo Inativo");
        var inativarGrupo = await client.PatchAsJsonAsync($"/api/admin/grupos-tecnicos/{grupo.Id}/status", new AlterarStatusGrupoTecnicoRequest
        {
            Ativo = false
        });
        inativarGrupo.EnsureSuccessStatusCode();

        var response = await client.PostAsJsonAsync($"/api/admin/grupos-tecnicos/{grupo.Id}/membros", new AdicionarMembroGrupoTecnicoRequest
        {
            UsuarioId = usuarioId
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Grupo tecnico inativo nao pode receber membros.", await response.Content.ReadAsStringAsync(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task AdicionarMembroComUsuarioInexistenteRetornaErroAmigavel()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.membros.usuario.inexistente.{Guid.NewGuid():N}@empresa.com", "Admin Membros", "Administrador");
        var grupo = await CriarGrupoAsync(client);

        var response = await client.PostAsJsonAsync($"/api/admin/grupos-tecnicos/{grupo.Id}/membros", new AdicionarMembroGrupoTecnicoRequest
        {
            UsuarioId = Guid.NewGuid()
        });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains("Usuario nao encontrado.", await response.Content.ReadAsStringAsync(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task GerenciarMembrosNaoAlteraChamados()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.membros.sem.chamado.{Guid.NewGuid():N}@empresa.com", "Admin Membros", "Administrador");
        var grupo = await CriarGrupoAsync(client);
        var usuarioId = await CriarUsuarioAtendenteAsync("Membro Sem Chamado");
        var chamadosAntes = await ContarChamadosAsync();

        var membro = await AdicionarMembroAsync(client, grupo.Id, usuarioId);
        var statusResponse = await client.PatchAsJsonAsync($"/api/admin/grupos-tecnicos/{grupo.Id}/membros/{membro.Id}/status", new AlterarStatusMembroGrupoTecnicoRequest
        {
            Ativo = false
        });

        statusResponse.EnsureSuccessStatusCode();
        var chamadosDepois = await ContarChamadosAsync();
        Assert.Equal(chamadosAntes, chamadosDepois);
    }

    [Fact]
    public async Task AtendenteListaGruposTecnicosDoUsuario()
    {
        using var adminClient = _factory.CreateClient();
        AddDevHeaders(adminClient, $"admin.membros.seed.usuario.{Guid.NewGuid():N}@empresa.com", "Admin Membros", "Administrador");
        var grupo = await CriarGrupoAsync(adminClient);
        var usuarioId = await CriarUsuarioAtendenteAsync("Usuario Grupos Membros");
        await AdicionarMembroAsync(adminClient, grupo.Id, usuarioId);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"atendente.membros.usuario.{Guid.NewGuid():N}@empresa.com", "Atendente Membros", "Atendente");

        var response = await client.GetAsync($"/api/admin/usuarios/{usuarioId}/grupos-tecnicos");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<List<GrupoTecnicoDoUsuarioResponse>>();
        Assert.NotNull(payload);
        Assert.Contains(payload, x => x.GrupoTecnicoId == grupo.Id && x.Ativo);
    }

    [Fact]
    public async Task NaoExpoeEndpointLegadoDeDirecionamentoNestaEtapa()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.grupos.sem.chamado.{Guid.NewGuid():N}@empresa.com", "Admin Grupos", "Administrador");

        var direcionar = await client.PostAsJsonAsync($"/api/admin/chamados/{Guid.NewGuid()}/grupo-tecnico", new { grupoTecnicoId = SeedData.GrupoTecnicoServiceDeskId });

        Assert.Equal(HttpStatusCode.NotFound, direcionar.StatusCode);
    }

    private static async Task<GrupoTecnicoResponse> CriarGrupoAsync(HttpClient client)
        => await CriarGrupoAsync(client, $"Grupo Teste {Guid.NewGuid():N}");

    private static async Task<GrupoTecnicoResponse> CriarGrupoAsync(HttpClient client, string nome)
    {
        var response = await client.PostAsJsonAsync("/api/admin/grupos-tecnicos", new CriarGrupoTecnicoRequest
        {
            Nome = nome,
            Descricao = "Grupo criado por teste"
        });

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<GrupoTecnicoResponse>();
        Assert.NotNull(payload);
        return payload;
    }

    private async Task<Guid> CriarUsuarioAtendenteAsync(string nome)
        => await _factory.GarantirUsuarioLocalComSenhaAsync(
            $"{nome.ToLowerInvariant().Replace(' ', '.')}.{Guid.NewGuid():N}@empresa.com",
            nome,
            "Senha!123456",
            TipoPerfil.Atendente);

    private static async Task<MembroGrupoTecnicoResponse> AdicionarMembroAsync(HttpClient client, Guid grupoTecnicoId, Guid usuarioId)
    {
        var response = await client.PostAsJsonAsync($"/api/admin/grupos-tecnicos/{grupoTecnicoId}/membros", new AdicionarMembroGrupoTecnicoRequest
        {
            UsuarioId = usuarioId
        });

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<MembroGrupoTecnicoResponse>();
        Assert.NotNull(payload);
        return payload;
    }

    private async Task<int> ContarChamadosAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        return await context.Chamados.CountAsync();
    }

    private async Task<DadosFilasGrupo> SeedFilasGrupoAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();

        var grupo = new GrupoTecnico($"Grupo Filas {Guid.NewGuid():N}", "Grupo com filas", "integration-test");
        var outroGrupo = new GrupoTecnico($"Outro Grupo Filas {Guid.NewGuid():N}", "Outro grupo", "integration-test");
        context.GruposTecnicos.AddRange(grupo, outroGrupo);
        await context.SaveChangesAsync();

        var filaAtiva = new FilaAtendimento(grupo.Id, $"Fila Incidentes {Guid.NewGuid():N}", "Atendimento de incidentes", "integration-test");
        var filaInativa = new FilaAtendimento(grupo.Id, $"Fila Requisicoes {Guid.NewGuid():N}", "Requisicoes antigas", "integration-test");
        filaInativa.Inativar("integration-test");
        var filaOutroGrupo = new FilaAtendimento(outroGrupo.Id, $"Fila Incidentes Outro {Guid.NewGuid():N}", "Nao deve retornar", "integration-test");
        context.FilasAtendimento.AddRange(filaAtiva, filaInativa, filaOutroGrupo);
        await context.SaveChangesAsync();

        return new DadosFilasGrupo(grupo.Id, outroGrupo.Id, filaAtiva.Id, filaInativa.Id, filaOutroGrupo.Id);
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

    private sealed record DadosFilasGrupo(
        Guid GrupoId,
        Guid OutroGrupoId,
        Guid FilaAtivaId,
        Guid FilaInativaId,
        Guid FilaOutroGrupoId);
}
