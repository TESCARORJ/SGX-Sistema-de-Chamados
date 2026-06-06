using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class ListarChamadosAdminEndpointsIntegrationTests : IClassFixture<ApiIntegrationTestFactory>
{
    private readonly ApiIntegrationTestFactory _factory;

    public ListarChamadosAdminEndpointsIntegrationTests(ApiIntegrationTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task EndpointListagemAceitaFiltrosGrupoFilaRetornaCamposENaoAlteraDados()
    {
        var dados = await SeedChamadosAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.listagem.grupo.fila.{Guid.NewGuid():N}@empresa.com", "Admin Listagem", "Administrador");

        var response = await client.GetAsync($"/api/admin/chamados?grupoTecnicoId={dados.GrupoTecnicoId}&filaAtendimentoId={dados.FilaAtendimentoId}&pagina=1&tamanhoPagina=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<ListaChamadosAdminResponse>();
        Assert.NotNull(payload);
        var item = Assert.Single(payload.Items);
        Assert.Equal(dados.ChamadoComGrupoId, item.Id);
        Assert.Equal(dados.GrupoTecnicoId, item.GrupoTecnicoId);
        Assert.Equal("Grupo Listagem API", item.GrupoTecnicoNome);
        Assert.Equal(dados.FilaAtendimentoId, item.FilaAtendimentoId);
        Assert.Equal("Fila Listagem API", item.FilaAtendimentoNome);

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        var chamadoDepois = await context.Chamados.AsNoTracking().SingleAsync(x => x.Id == dados.ChamadoComGrupoId);
        Assert.Equal(dados.GrupoTecnicoId, chamadoDepois.GrupoTecnicoId);
        Assert.Equal(dados.FilaAtendimentoId, chamadoDepois.FilaAtendimentoId);
        Assert.Null(chamadoDepois.ResponsavelId);
        Assert.Empty(await context.HistoricosChamado.AsNoTracking().Where(x => x.ChamadoId == dados.ChamadoComGrupoId).ToListAsync());
    }

    [Fact]
    public async Task EndpointListagemSemFiltroPreservaChamadosComESemGrupoFila()
    {
        var dados = await SeedChamadosAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"atendente.listagem.grupo.fila.{Guid.NewGuid():N}@empresa.com", "Atendente Listagem", "Atendente");

        var response = await client.GetAsync($"/api/admin/chamados?texto={dados.PrefixoCodigo}&pagina=1&tamanhoPagina=10&ordenarPor=codigo&direcaoOrdenacao=asc");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<ListaChamadosAdminResponse>();
        Assert.NotNull(payload);
        Assert.Equal(2, payload.Total);
        Assert.Contains(payload.Items, x => x.Id == dados.ChamadoSemGrupoId && x.GrupoTecnicoId == null && x.FilaAtendimentoId == null);
        Assert.Contains(payload.Items, x => x.Id == dados.ChamadoComGrupoId && x.GrupoTecnicoId == dados.GrupoTecnicoId && x.FilaAtendimentoId == dados.FilaAtendimentoId);
    }

    private async Task<DadosListagem> SeedChamadosAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();

        var prefixo = $"SGX-LST-{Guid.NewGuid():N}"[..16].ToUpperInvariant();
        var solicitante = await CriarUsuarioAsync(dbContext, $"Solicitante {prefixo}", TipoPerfil.Solicitante, cancellationToken);
        var categoria = await CriarCategoriaAsync(dbContext, prefixo, cancellationToken);
        var prioridade = await dbContext.PrioridadesChamado.FirstAsync(cancellationToken);
        var status = await dbContext.StatusChamado.FirstAsync(x => x.Codigo == StatusChamadoEnum.Aberto, cancellationToken);

        var chamadoSemGrupo = new Chamado(
            $"{prefixo}-A",
            "Chamado legado sem grupo",
            "Chamado sem grupo tecnico e sem fila para validar listagem.",
            solicitante.Id,
            categoria.Id,
            prioridade.Id,
            status.Id,
            OrigemChamado.Portal,
            "integration-test");

        var chamadoComGrupo = new Chamado(
            $"{prefixo}-B",
            "Chamado com grupo e fila",
            "Chamado com grupo tecnico e fila para validar filtros.",
            solicitante.Id,
            categoria.Id,
            prioridade.Id,
            status.Id,
            OrigemChamado.Portal,
            "integration-test");

        var grupo = new GrupoTecnico("Grupo Listagem API", "Grupo tecnico para teste de listagem", "integration-test");
        dbContext.GruposTecnicos.Add(grupo);
        await dbContext.SaveChangesAsync(cancellationToken);

        var fila = new FilaAtendimento(grupo.Id, "Fila Listagem API", "Fila para teste de listagem", "integration-test");
        dbContext.FilasAtendimento.Add(fila);
        await dbContext.SaveChangesAsync(cancellationToken);

        chamadoComGrupo.DefinirGrupoTecnico(grupo.Id, "integration-test");
        chamadoComGrupo.DefinirFilaAtendimento(fila.Id, "integration-test");
        dbContext.Chamados.AddRange(chamadoSemGrupo, chamadoComGrupo);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new DadosListagem(prefixo, chamadoSemGrupo.Id, chamadoComGrupo.Id, grupo.Id, fila.Id);
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

    private static async Task<CategoriaChamado> CriarCategoriaAsync(
        SGXSistemaChamadoDbContext dbContext,
        string prefixo,
        CancellationToken cancellationToken)
    {
        var categoria = new CategoriaChamado($"Categoria {prefixo}", "Categoria para teste de listagem", null, "integration-test");
        dbContext.CategoriasChamado.Add(categoria);
        await dbContext.SaveChangesAsync(cancellationToken);
        return categoria;
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

    private sealed record DadosListagem(
        string PrefixoCodigo,
        Guid ChamadoSemGrupoId,
        Guid ChamadoComGrupoId,
        Guid GrupoTecnicoId,
        Guid FilaAtendimentoId);
}
