using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class DirecionarChamadoGrupoTecnicoEndpointsIntegrationTests : IClassFixture<ApiIntegrationTestFactory>
{
    private readonly ApiIntegrationTestFactory _factory;

    public DirecionarChamadoGrupoTecnicoEndpointsIntegrationTests(ApiIntegrationTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task AdministradorDirecionaChamadoParaGrupoTecnico()
    {
        var dados = await SeedChamadoComGruposAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.direcionar.{Guid.NewGuid():N}@empresa.com", "Admin Direcionar", "Administrador");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{dados.ChamadoId}/direcionar-grupo-tecnico", new DirecionarChamadoGrupoTecnicoRequest
        {
            GrupoTecnicoId = dados.GrupoDestinoId,
            Observacao = "Triagem administrativa"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<ChamadoAdminDetalheResponse>();
        Assert.NotNull(payload);
        Assert.Equal(dados.GrupoDestinoId, payload.GrupoTecnicoId);
        Assert.StartsWith("Grupo Direcionamento", payload.GrupoTecnicoNome, StringComparison.Ordinal);
    }

    [Fact]
    public async Task AtendenteDirecionaChamadoParaGrupoTecnico()
    {
        var dados = await SeedChamadoComGruposAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"atendente.direcionar.{Guid.NewGuid():N}@empresa.com", "Atendente Direcionar", "Atendente");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{dados.ChamadoId}/direcionar-grupo-tecnico", new DirecionarChamadoGrupoTecnicoRequest
        {
            GrupoTecnicoId = dados.GrupoDestinoId
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<ChamadoAdminDetalheResponse>();
        Assert.NotNull(payload);
        Assert.Equal(dados.GrupoDestinoId, payload.GrupoTecnicoId);
    }

    [Fact]
    public async Task AdministradorDirecionaChamadoParaGrupoTecnicoComFilaValida()
    {
        var dados = await SeedChamadoComGruposAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.direcionar.fila.{Guid.NewGuid():N}@empresa.com", "Admin Direcionar", "Administrador");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{dados.ChamadoId}/direcionar-grupo-tecnico", new DirecionarChamadoGrupoTecnicoRequest
        {
            GrupoTecnicoId = dados.GrupoDestinoId,
            FilaAtendimentoId = dados.FilaDestinoId
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<ChamadoAdminDetalheResponse>();
        Assert.NotNull(payload);
        Assert.Equal(dados.GrupoDestinoId, payload.GrupoTecnicoId);
        Assert.Equal(dados.FilaDestinoId, payload.FilaAtendimentoId);
    }

    [Fact]
    public async Task SolicitanteNaoDirecionaChamadoParaGrupoTecnico()
    {
        var dados = await SeedChamadoComGruposAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"solicitante.direcionar.{Guid.NewGuid():N}@empresa.com", "Solicitante Direcionar", "Solicitante");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{dados.ChamadoId}/direcionar-grupo-tecnico", new DirecionarChamadoGrupoTecnicoRequest
        {
            GrupoTecnicoId = dados.GrupoDestinoId
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task RejeitaGrupoInexistenteOuInativo()
    {
        var dados = await SeedChamadoComGruposAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.direcionar.grupo.invalido.{Guid.NewGuid():N}@empresa.com", "Admin Direcionar", "Administrador");

        var inexistente = await client.PostAsJsonAsync($"/api/admin/chamados/{dados.ChamadoId}/direcionar-grupo-tecnico", new DirecionarChamadoGrupoTecnicoRequest
        {
            GrupoTecnicoId = Guid.NewGuid()
        });

        var inativo = await client.PostAsJsonAsync($"/api/admin/chamados/{dados.ChamadoId}/direcionar-grupo-tecnico", new DirecionarChamadoGrupoTecnicoRequest
        {
            GrupoTecnicoId = dados.GrupoInativoId
        });

        Assert.Equal(HttpStatusCode.BadRequest, inexistente.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, inativo.StatusCode);
        Assert.Contains("Grupo tecnico nao encontrado ou inativo.", await inexistente.Content.ReadAsStringAsync(), StringComparison.Ordinal);
        Assert.Contains("Grupo tecnico nao encontrado ou inativo.", await inativo.Content.ReadAsStringAsync(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task RejeitaFilaInexistenteOuInativa()
    {
        var dados = await SeedChamadoComGruposAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.direcionar.fila.invalida.{Guid.NewGuid():N}@empresa.com", "Admin Direcionar", "Administrador");

        var inexistente = await client.PostAsJsonAsync($"/api/admin/chamados/{dados.ChamadoId}/direcionar-grupo-tecnico", new DirecionarChamadoGrupoTecnicoRequest
        {
            GrupoTecnicoId = dados.GrupoDestinoId,
            FilaAtendimentoId = Guid.NewGuid()
        });

        var inativa = await client.PostAsJsonAsync($"/api/admin/chamados/{dados.ChamadoId}/direcionar-grupo-tecnico", new DirecionarChamadoGrupoTecnicoRequest
        {
            GrupoTecnicoId = dados.GrupoDestinoId,
            FilaAtendimentoId = dados.FilaInativaId
        });

        Assert.Equal(HttpStatusCode.BadRequest, inexistente.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, inativa.StatusCode);
        Assert.Contains("Fila de atendimento nao encontrada ou inativa.", await inexistente.Content.ReadAsStringAsync(), StringComparison.Ordinal);
        Assert.Contains("Fila de atendimento nao encontrada ou inativa.", await inativa.Content.ReadAsStringAsync(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task RejeitaFilaDeOutroGrupo()
    {
        var dados = await SeedChamadoComGruposAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.direcionar.fila.outro.{Guid.NewGuid():N}@empresa.com", "Admin Direcionar", "Administrador");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{dados.ChamadoId}/direcionar-grupo-tecnico", new DirecionarChamadoGrupoTecnicoRequest
        {
            GrupoTecnicoId = dados.GrupoDestinoId,
            FilaAtendimentoId = dados.FilaOutroGrupoId
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Fila de atendimento nao pertence ao grupo tecnico informado.", await response.Content.ReadAsStringAsync(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task PreservaResponsavelAoDirecionarChamado()
    {
        var dados = await SeedChamadoComGruposAsync(comResponsavel: true);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.direcionar.responsavel.{Guid.NewGuid():N}@empresa.com", "Admin Direcionar", "Administrador");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{dados.ChamadoId}/direcionar-grupo-tecnico", new DirecionarChamadoGrupoTecnicoRequest
        {
            GrupoTecnicoId = dados.GrupoDestinoId
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<ChamadoAdminDetalheResponse>();
        Assert.NotNull(payload);
        Assert.NotNull(payload.Responsavel);
        Assert.Equal(dados.ResponsavelId, payload.Responsavel.Id);

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        var chamado = await context.Chamados.AsNoTracking().SingleAsync(x => x.Id == dados.ChamadoId);
        Assert.Equal(dados.ResponsavelId, chamado.ResponsavelId);
    }

    [Fact]
    public async Task DirecionamentoNaoAssumeChamadoDaFila()
    {
        var dados = await SeedChamadoComGruposAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.direcionar.sem.assumir.{Guid.NewGuid():N}@empresa.com", "Admin Direcionar", "Administrador");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{dados.ChamadoId}/direcionar-grupo-tecnico", new DirecionarChamadoGrupoTecnicoRequest
        {
            GrupoTecnicoId = dados.GrupoDestinoId,
            FilaAtendimentoId = dados.FilaDestinoId
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<ChamadoAdminDetalheResponse>();
        Assert.NotNull(payload);
        Assert.Equal(dados.FilaDestinoId, payload.FilaAtendimentoId);
        Assert.Null(payload.Responsavel);

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        var chamado = await context.Chamados.AsNoTracking().SingleAsync(x => x.Id == dados.ChamadoId);
        Assert.Null(chamado.ResponsavelId);
    }

    [Fact]
    public async Task EndpointRegistraHistoricoDeGrupoEFila()
    {
        var dados = await SeedChamadoComGruposAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.direcionar.historico.{Guid.NewGuid():N}@empresa.com", "Admin Direcionar", "Administrador");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{dados.ChamadoId}/direcionar-grupo-tecnico", new DirecionarChamadoGrupoTecnicoRequest
        {
            GrupoTecnicoId = dados.GrupoDestinoId,
            FilaAtendimentoId = dados.FilaDestinoId,
            Observacao = "Roteamento inicial"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        Assert.Contains(context.HistoricosChamado, x =>
            x.ChamadoId == dados.ChamadoId &&
            x.Tipo == TipoHistoricoChamado.GrupoTecnicoDefinido &&
            x.Descricao.Contains("Roteamento inicial", StringComparison.Ordinal));
        Assert.Contains(context.HistoricosChamado, x =>
            x.ChamadoId == dados.ChamadoId &&
            x.Tipo == TipoHistoricoChamado.FilaAtendimentoDefinida);
    }

    [Fact]
    public async Task NaoTransfereChamadoQuandoJaPossuiOutroGrupo()
    {
        var dados = await SeedChamadoComGruposAsync(comGrupoOrigem: true);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.direcionar.nao.transfere.{Guid.NewGuid():N}@empresa.com", "Admin Direcionar", "Administrador");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{dados.ChamadoId}/direcionar-grupo-tecnico", new DirecionarChamadoGrupoTecnicoRequest
        {
            GrupoTecnicoId = dados.GrupoDestinoId
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Use a transferencia entre grupos tecnicos", await response.Content.ReadAsStringAsync(), StringComparison.Ordinal);

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        var chamado = await context.Chamados.AsNoTracking().SingleAsync(x => x.Id == dados.ChamadoId);
        Assert.Equal(dados.GrupoOrigemId, chamado.GrupoTecnicoId);
    }

    [Fact]
    public async Task NaoExpoeEndpointDeAtribuicaoTecnicoNestaEtapa()
    {
        var dados = await SeedChamadoComGruposAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.direcionar.sem.outros.{Guid.NewGuid():N}@empresa.com", "Admin Direcionar", "Administrador");

        var atribuirTecnico = await client.PostAsJsonAsync($"/api/admin/chamados/{dados.ChamadoId}/atribuir-tecnico", new { usuarioId = dados.ResponsavelId });

        Assert.Equal(HttpStatusCode.NotFound, atribuirTecnico.StatusCode);
    }

    private async Task<DadosDirecionamento> SeedChamadoComGruposAsync(
        bool comResponsavel = false,
        bool comGrupoOrigem = false,
        CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();

        var solicitante = await CriarUsuarioAsync(dbContext, "Solicitante Direcionamento", TipoPerfil.Solicitante, cancellationToken);
        var responsavel = await CriarUsuarioAsync(dbContext, "Atendente Direcionamento", TipoPerfil.Atendente, cancellationToken);

        var categoria = await dbContext.CategoriasChamado.FirstOrDefaultAsync(cancellationToken)
            ?? new CategoriaChamado("Categoria Direcionamento", "Categoria para direcionamento", null, "integration-test");
        if (dbContext.Entry(categoria).State == EntityState.Detached)
        {
            dbContext.CategoriasChamado.Add(categoria);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var prioridade = await dbContext.PrioridadesChamado.FirstAsync(cancellationToken);
        var status = await dbContext.StatusChamado.FirstAsync(x => x.Codigo == StatusChamadoEnum.Aberto, cancellationToken);
        var grupoOrigem = new GrupoTecnico($"Grupo Origem {Guid.NewGuid():N}", "Grupo origem de teste", "integration-test");
        var grupoDestino = new GrupoTecnico($"Grupo Direcionamento {Guid.NewGuid():N}", "Grupo destino de teste", "integration-test");
        var grupoOutro = new GrupoTecnico($"Grupo Outro {Guid.NewGuid():N}", "Grupo de outra fila", "integration-test");
        var grupoInativo = new GrupoTecnico($"Grupo Inativo {Guid.NewGuid():N}", "Grupo inativo de teste", "integration-test");
        grupoInativo.Inativar("integration-test");

        dbContext.GruposTecnicos.AddRange(grupoOrigem, grupoDestino, grupoOutro, grupoInativo);
        await dbContext.SaveChangesAsync(cancellationToken);

        var filaDestino = new FilaAtendimento(grupoDestino.Id, $"Fila Direcionamento {Guid.NewGuid():N}", "Fila destino de teste", "integration-test");
        var filaOutroGrupo = new FilaAtendimento(grupoOutro.Id, $"Fila Outro Grupo {Guid.NewGuid():N}", "Fila de outro grupo", "integration-test");
        var filaInativa = new FilaAtendimento(grupoDestino.Id, $"Fila Inativa {Guid.NewGuid():N}", "Fila inativa de teste", "integration-test");
        filaInativa.Inativar("integration-test");
        dbContext.FilasAtendimento.AddRange(filaDestino, filaOutroGrupo, filaInativa);
        await dbContext.SaveChangesAsync(cancellationToken);

        var chamado = new Chamado(
            $"SGX-DG-{Guid.NewGuid():N}".ToUpperInvariant()[..20],
            "Chamado para direcionamento",
            "Descricao do chamado para direcionamento",
            solicitante.Id,
            categoria.Id,
            prioridade.Id,
            status.Id,
            OrigemChamado.Portal,
            "integration-test");

        if (comResponsavel)
        {
            chamado.AtribuirResponsavel(responsavel.Id, "integration-test");
        }

        if (comGrupoOrigem)
        {
            chamado.DefinirGrupoTecnico(grupoOrigem.Id, "integration-test");
        }

        dbContext.Chamados.Add(chamado);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new DadosDirecionamento(
            chamado.Id,
            grupoOrigem.Id,
            grupoDestino.Id,
            grupoInativo.Id,
            filaDestino.Id,
            filaOutroGrupo.Id,
            filaInativa.Id,
            responsavel.Id);
    }

    private static async Task<Usuario> CriarUsuarioAsync(
        SGXSistemaChamadoDbContext dbContext,
        string nome,
        TipoPerfil tipoPerfil,
        CancellationToken cancellationToken)
    {
        var email = $"{nome.ToLowerInvariant().Replace(' ', '.')}.{Guid.NewGuid():N}@empresa.com";
        var usuario = new Usuario(nome, email, email, "integration-test");
        dbContext.Usuarios.Add(usuario);
        await dbContext.SaveChangesAsync(cancellationToken);

        var perfil = await dbContext.PerfisAcesso.FirstAsync(x => x.TipoPerfil == tipoPerfil, cancellationToken);
        dbContext.UsuariosPerfisAcesso.Add(new UsuarioPerfilAcesso(usuario.Id, perfil.Id, "integration-test"));
        await dbContext.SaveChangesAsync(cancellationToken);
        return usuario;
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

    private sealed record DadosDirecionamento(
        Guid ChamadoId,
        Guid GrupoOrigemId,
        Guid GrupoDestinoId,
        Guid GrupoInativoId,
        Guid FilaDestinoId,
        Guid FilaOutroGrupoId,
        Guid FilaInativaId,
        Guid ResponsavelId);
}
