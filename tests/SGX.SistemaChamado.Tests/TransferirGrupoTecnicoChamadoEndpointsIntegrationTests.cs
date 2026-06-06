using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class TransferirGrupoTecnicoChamadoEndpointsIntegrationTests : IClassFixture<ApiIntegrationTestFactory>
{
    private readonly ApiIntegrationTestFactory _factory;

    public TransferirGrupoTecnicoChamadoEndpointsIntegrationTests(ApiIntegrationTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task AdministradorTransfereChamadoEntreGruposTecnicos()
    {
        var dados = await SeedChamadoParaTransferenciaAsync(comResponsavel: true);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.transferir.{Guid.NewGuid():N}@empresa.com", "Admin Transferir", "Administrador");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{dados.ChamadoId}/transferir-grupo-tecnico", new TransferirGrupoTecnicoChamadoRequest
        {
            GrupoTecnicoId = dados.GrupoDestinoId,
            FilaAtendimentoId = dados.FilaDestinoId
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<ChamadoAdminDetalheResponse>();
        Assert.NotNull(payload);
        Assert.Equal(dados.GrupoDestinoId, payload.GrupoTecnicoId);
        Assert.Equal(dados.FilaDestinoId, payload.FilaAtendimentoId);
        Assert.Null(payload.Responsavel);
    }

    [Fact]
    public async Task AtendenteTransfereChamadoEntreGruposTecnicos()
    {
        var dados = await SeedChamadoParaTransferenciaAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"atendente.transferir.{Guid.NewGuid():N}@empresa.com", "Atendente Transferir", "Atendente");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{dados.ChamadoId}/transferir-grupo-tecnico", new TransferirGrupoTecnicoChamadoRequest
        {
            GrupoTecnicoId = dados.GrupoDestinoId
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<ChamadoAdminDetalheResponse>();
        Assert.NotNull(payload);
        Assert.Equal(dados.GrupoDestinoId, payload.GrupoTecnicoId);
    }

    [Fact]
    public async Task SolicitanteNaoTransfereChamadoEntreGruposTecnicos()
    {
        var dados = await SeedChamadoParaTransferenciaAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"solicitante.transferir.{Guid.NewGuid():N}@empresa.com", "Solicitante Transferir", "Solicitante");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{dados.ChamadoId}/transferir-grupo-tecnico", new TransferirGrupoTecnicoChamadoRequest
        {
            GrupoTecnicoId = dados.GrupoDestinoId
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task RejeitaChamadoSemGrupoAnteriorViaUseCase()
    {
        var dados = await SeedChamadoParaTransferenciaAsync(semGrupoOrigem: true);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.transferir.semgrupo.{Guid.NewGuid():N}@empresa.com", "Admin Transferir", "Administrador");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{dados.ChamadoId}/transferir-grupo-tecnico", new TransferirGrupoTecnicoChamadoRequest
        {
            GrupoTecnicoId = dados.GrupoDestinoId
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Chamado sem grupo tecnico deve ser direcionado antes de ser transferido entre grupos.", await response.Content.ReadAsStringAsync(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task RejeitaGrupoDestinoInativoViaUseCase()
    {
        var dados = await SeedChamadoParaTransferenciaAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.transferir.grupo.inativo.{Guid.NewGuid():N}@empresa.com", "Admin Transferir", "Administrador");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{dados.ChamadoId}/transferir-grupo-tecnico", new TransferirGrupoTecnicoChamadoRequest
        {
            GrupoTecnicoId = dados.GrupoInativoId
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Grupo tecnico de destino nao encontrado ou inativo.", await response.Content.ReadAsStringAsync(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task RejeitaFilaDeOutroGrupoViaUseCase()
    {
        var dados = await SeedChamadoParaTransferenciaAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.transferir.fila.outro.{Guid.NewGuid():N}@empresa.com", "Admin Transferir", "Administrador");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{dados.ChamadoId}/transferir-grupo-tecnico", new TransferirGrupoTecnicoChamadoRequest
        {
            GrupoTecnicoId = dados.GrupoDestinoId,
            FilaAtendimentoId = dados.FilaOutroGrupoId
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Fila de atendimento de destino nao pertence ao grupo tecnico informado.", await response.Content.ReadAsStringAsync(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task RejeitaFilaDestinoInexistenteViaUseCase()
    {
        var dados = await SeedChamadoParaTransferenciaAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.transferir.fila.inexistente.{Guid.NewGuid():N}@empresa.com", "Admin Transferir", "Administrador");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{dados.ChamadoId}/transferir-grupo-tecnico", new TransferirGrupoTecnicoChamadoRequest
        {
            GrupoTecnicoId = dados.GrupoDestinoId,
            FilaAtendimentoId = Guid.NewGuid()
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Fila de atendimento de destino nao encontrada ou inativa.", await response.Content.ReadAsStringAsync(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task RejeitaFilaDestinoInativaViaUseCase()
    {
        var dados = await SeedChamadoParaTransferenciaAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.transferir.fila.inativa.{Guid.NewGuid():N}@empresa.com", "Admin Transferir", "Administrador");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{dados.ChamadoId}/transferir-grupo-tecnico", new TransferirGrupoTecnicoChamadoRequest
        {
            GrupoTecnicoId = dados.GrupoDestinoId,
            FilaAtendimentoId = dados.FilaInativaId
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Fila de atendimento de destino nao encontrada ou inativa.", await response.Content.ReadAsStringAsync(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task AlteraGrupoLimpaResponsavelELimpaFilaQuandoSemFilaDestino()
    {
        var dados = await SeedChamadoParaTransferenciaAsync(comResponsavel: true);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.transferir.limpa.{Guid.NewGuid():N}@empresa.com", "Admin Transferir", "Administrador");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{dados.ChamadoId}/transferir-grupo-tecnico", new TransferirGrupoTecnicoChamadoRequest
        {
            GrupoTecnicoId = dados.GrupoDestinoId
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        var chamado = await context.Chamados.AsNoTracking().SingleAsync(x => x.Id == dados.ChamadoId);
        Assert.Equal(dados.GrupoDestinoId, chamado.GrupoTecnicoId);
        Assert.Null(chamado.ResponsavelId);
        Assert.Null(chamado.FilaAtendimentoId);
    }

    [Fact]
    public async Task RedefineFilaQuandoFilaDestinoValida()
    {
        var dados = await SeedChamadoParaTransferenciaAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.transferir.fila.{Guid.NewGuid():N}@empresa.com", "Admin Transferir", "Administrador");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{dados.ChamadoId}/transferir-grupo-tecnico", new TransferirGrupoTecnicoChamadoRequest
        {
            GrupoTecnicoId = dados.GrupoDestinoId,
            FilaAtendimentoId = dados.FilaDestinoId
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        var chamado = await context.Chamados.AsNoTracking().SingleAsync(x => x.Id == dados.ChamadoId);
        Assert.Equal(dados.GrupoDestinoId, chamado.GrupoTecnicoId);
        Assert.Equal(dados.FilaDestinoId, chamado.FilaAtendimentoId);
    }

    [Fact]
    public async Task RegistraHistoricoDeTransferenciaERemoveResponsavel()
    {
        var dados = await SeedChamadoParaTransferenciaAsync(comResponsavel: true);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.transferir.historico.{Guid.NewGuid():N}@empresa.com", "Admin Transferir", "Administrador");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{dados.ChamadoId}/transferir-grupo-tecnico", new TransferirGrupoTecnicoChamadoRequest
        {
            GrupoTecnicoId = dados.GrupoDestinoId,
            FilaAtendimentoId = dados.FilaDestinoId
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        var historicos = await context.HistoricosChamado.AsNoTracking().Where(x => x.ChamadoId == dados.ChamadoId).ToListAsync();
        Assert.Contains(historicos, x => x.Tipo == TipoHistoricoChamado.GrupoTecnicoTransferido);
        Assert.Contains(historicos, x => x.Tipo == TipoHistoricoChamado.FilaAtendimentoTransferida);
        Assert.Contains(historicos, x => x.Tipo == TipoHistoricoChamado.ResponsavelRemovidoPorTransferenciaGrupo);
    }

    [Fact]
    public async Task NaoDirecionaChamadoInicialNaoAssumeFilaENaoAtribuiTecnico()
    {
        var dados = await SeedChamadoParaTransferenciaAsync(semGrupoOrigem: true);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.transferir.nao.atalho.{Guid.NewGuid():N}@empresa.com", "Admin Transferir", "Administrador");

        var transferir = await client.PostAsJsonAsync($"/api/admin/chamados/{dados.ChamadoId}/transferir-grupo-tecnico", new TransferirGrupoTecnicoChamadoRequest
        {
            GrupoTecnicoId = dados.GrupoDestinoId,
            FilaAtendimentoId = dados.FilaDestinoId
        });
        var atribuirTecnico = await client.PostAsJsonAsync($"/api/admin/chamados/{dados.ChamadoId}/atribuir-tecnico", new { usuarioId = dados.ResponsavelId });

        Assert.Equal(HttpStatusCode.BadRequest, transferir.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, atribuirTecnico.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        var chamado = await context.Chamados.AsNoTracking().SingleAsync(x => x.Id == dados.ChamadoId);
        Assert.Null(chamado.GrupoTecnicoId);
        Assert.Null(chamado.FilaAtendimentoId);
        Assert.Null(chamado.ResponsavelId);
    }

    private async Task<DadosTransferencia> SeedChamadoParaTransferenciaAsync(
        bool comResponsavel = false,
        bool semGrupoOrigem = false,
        CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();

        var solicitante = await CriarUsuarioAsync(dbContext, "Solicitante Transferencia", TipoPerfil.Solicitante, cancellationToken);
        var responsavel = await CriarUsuarioAsync(dbContext, "Atendente Transferencia", TipoPerfil.Atendente, cancellationToken);

        var categoria = await dbContext.CategoriasChamado.FirstOrDefaultAsync(cancellationToken)
            ?? new CategoriaChamado("Categoria Transferencia", "Categoria para transferencia", null, "integration-test");
        if (dbContext.Entry(categoria).State == EntityState.Detached)
        {
            dbContext.CategoriasChamado.Add(categoria);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var prioridade = await dbContext.PrioridadesChamado.FirstAsync(cancellationToken);
        var status = await dbContext.StatusChamado.FirstAsync(x => x.Codigo == StatusChamadoEnum.Aberto, cancellationToken);
        var grupoOrigem = new GrupoTecnico($"Grupo Origem Transferencia {Guid.NewGuid():N}", "Grupo origem", "integration-test");
        var grupoDestino = new GrupoTecnico($"Grupo Destino Transferencia {Guid.NewGuid():N}", "Grupo destino", "integration-test");
        var grupoOutro = new GrupoTecnico($"Grupo Outro Transferencia {Guid.NewGuid():N}", "Grupo outro", "integration-test");
        var grupoInativo = new GrupoTecnico($"Grupo Inativo Transferencia {Guid.NewGuid():N}", "Grupo inativo", "integration-test");
        grupoInativo.Inativar("integration-test");
        dbContext.GruposTecnicos.AddRange(grupoOrigem, grupoDestino, grupoOutro, grupoInativo);
        await dbContext.SaveChangesAsync(cancellationToken);

        var filaOrigem = new FilaAtendimento(grupoOrigem.Id, $"Fila Origem Transferencia {Guid.NewGuid():N}", "Fila origem", "integration-test");
        var filaDestino = new FilaAtendimento(grupoDestino.Id, $"Fila Destino Transferencia {Guid.NewGuid():N}", "Fila destino", "integration-test");
        var filaInativa = new FilaAtendimento(grupoDestino.Id, $"Fila Inativa Transferencia {Guid.NewGuid():N}", "Fila inativa", "integration-test");
        filaInativa.Inativar("integration-test");
        var filaOutroGrupo = new FilaAtendimento(grupoOutro.Id, $"Fila Outro Transferencia {Guid.NewGuid():N}", "Fila outro", "integration-test");
        dbContext.FilasAtendimento.AddRange(filaOrigem, filaDestino, filaInativa, filaOutroGrupo);
        await dbContext.SaveChangesAsync(cancellationToken);

        var chamado = new Chamado(
            $"SGX-TG-{Guid.NewGuid():N}".ToUpperInvariant()[..20],
            "Chamado para transferencia",
            "Descricao do chamado para transferencia",
            solicitante.Id,
            categoria.Id,
            prioridade.Id,
            status.Id,
            OrigemChamado.Portal,
            "integration-test");

        if (!semGrupoOrigem)
        {
            chamado.DirecionarGrupoTecnico(grupoOrigem.Id, filaOrigem.Id, "integration-test");
        }

        if (comResponsavel)
        {
            chamado.AtribuirResponsavel(responsavel.Id, "integration-test");
        }

        dbContext.Chamados.Add(chamado);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new DadosTransferencia(
            chamado.Id,
            grupoOrigem.Id,
            grupoDestino.Id,
            grupoInativo.Id,
            filaOrigem.Id,
            filaDestino.Id,
            filaInativa.Id,
            filaOutroGrupo.Id,
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

    private sealed record DadosTransferencia(
        Guid ChamadoId,
        Guid GrupoOrigemId,
        Guid GrupoDestinoId,
        Guid GrupoInativoId,
        Guid FilaOrigemId,
        Guid FilaDestinoId,
        Guid FilaInativaId,
        Guid FilaOutroGrupoId,
        Guid ResponsavelId);
}
