using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class AtribuirChamadoEndpointsIntegrationTests : IClassFixture<ApiIntegrationTestFactory>
{
    private readonly ApiIntegrationTestFactory _factory;

    public AtribuirChamadoEndpointsIntegrationTests(ApiIntegrationTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task EndpointAtribuicaoAlteraResponsavelERetornaDetalheAtualizado()
    {
        var dados = await SeedChamadoParaAtribuicaoAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.atribuir.{Guid.NewGuid():N}@empresa.com", "Admin Atribuir", "Administrador");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{dados.ChamadoId}/atribuir", new AtribuirChamadoRequest
        {
            ResponsavelId = dados.ResponsavelId
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

        var historicos = await context.HistoricosChamado.AsNoTracking().Where(x => x.ChamadoId == dados.ChamadoId).ToListAsync();
        Assert.Contains(historicos, x =>
            x.Tipo == TipoHistoricoChamado.ResponsavelAlterado &&
            x.Descricao.Contains("Atendente Atribuicao", StringComparison.Ordinal));
    }

    private async Task<DadosAtribuicao> SeedChamadoParaAtribuicaoAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();

        var solicitante = await CriarUsuarioAsync(dbContext, "Solicitante Atribuicao", TipoPerfil.Solicitante, cancellationToken);
        var responsavel = await CriarUsuarioAsync(dbContext, "Atendente Atribuicao", TipoPerfil.Atendente, cancellationToken);
        var categoria = await dbContext.CategoriasChamado.FirstOrDefaultAsync(cancellationToken)
            ?? new CategoriaChamado("Categoria Atribuicao", "Categoria para atribuicao", null, "integration-test");
        if (dbContext.Entry(categoria).State == EntityState.Detached)
        {
            dbContext.CategoriasChamado.Add(categoria);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var prioridade = await dbContext.PrioridadesChamado.FirstAsync(cancellationToken);
        var status = await dbContext.StatusChamado.FirstAsync(x => x.Codigo == StatusChamadoEnum.Aberto, cancellationToken);
        var chamado = new Chamado(
            $"SGX-AT-{Guid.NewGuid():N}".ToUpperInvariant()[..20],
            "Chamado para atribuicao",
            "Descricao do chamado para atribuicao",
            solicitante.Id,
            categoria.Id,
            prioridade.Id,
            status.Id,
            OrigemChamado.Portal,
            "integration-test");

        dbContext.Chamados.Add(chamado);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new DadosAtribuicao(chamado.Id, responsavel.Id);
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

    private sealed record DadosAtribuicao(Guid ChamadoId, Guid ResponsavelId);
}
