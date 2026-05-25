using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SGX.SistemaChamado.Api.Authorization;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class AprovacaoChamadosAuthorizationIntegrationTests : IClassFixture<ApiIntegrationTestFactory>
{
    private readonly ApiIntegrationTestFactory _factory;

    public AprovacaoChamadosAuthorizationIntegrationTests(ApiIntegrationTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task VisualizarProtegeListagemEDetalhe()
    {
        await DefinirPermissaoAtivaAsync(PermissoesConstants.AprovacaoChamadosVisualizar, false);
        try
        {
            var (chamadoId, aprovacaoId) = await CriarChamadoEAprovacaoPendenteAsync();

            using var client = _factory.CreateClient();
            AddDevHeaders(client, $"aten.aprov.listagem.{Guid.NewGuid():N}@empresa.com", "Atendente Aprovacao", "Atendente");
            _ = await client.GetAsync("/api/me");

            var listagemResponse = await client.GetAsync("/api/admin/aprovacao-chamados");
            var detalheResponse = await client.GetAsync($"/api/admin/aprovacao-chamados/{aprovacaoId}");

            Assert.Equal(HttpStatusCode.Forbidden, listagemResponse.StatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, detalheResponse.StatusCode);
            _ = chamadoId;
        }
        finally
        {
            await DefinirPermissaoAtivaAsync(PermissoesConstants.AprovacaoChamadosVisualizar, true);
        }
    }

    [Fact]
    public async Task GerenciarProtegeSolicitar()
    {
        await DefinirPermissaoAtivaAsync(PermissoesConstants.AprovacaoChamadosGerenciar, false);
        try
        {
            var (chamadoId, _) = await CriarChamadoEAprovacaoPendenteAsync(criarAprovacao: false);

            using var client = _factory.CreateClient();
            AddDevHeaders(client, $"aten.aprov.solicitar.{Guid.NewGuid():N}@empresa.com", "Atendente Aprovacao", "Atendente");
            _ = await client.GetAsync("/api/me");

            var response = await client.PostAsJsonAsync($"/api/admin/chamados/{chamadoId}/aprovacao/solicitar", new
            {
                tipoOrigem = (int)TipoOrigemAprovacaoChamado.Manual,
                origemDescricao = "Solicitacao manual",
                justificativaSolicitacao = "Necessario validar"
            });

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
        finally
        {
            await DefinirPermissaoAtivaAsync(PermissoesConstants.AprovacaoChamadosGerenciar, true);
        }
    }

    [Fact]
    public async Task AprovarProtegeAprovar()
    {
        await DefinirPermissaoAtivaAsync(PermissoesConstants.AprovacaoChamadosAprovar, false);
        try
        {
            var (_, aprovacaoId) = await CriarChamadoEAprovacaoPendenteAsync();

            using var client = _factory.CreateClient();
            AddDevHeaders(client, $"aten.aprov.aprovar.{Guid.NewGuid():N}@empresa.com", "Atendente Aprovacao", "Atendente");
            _ = await client.GetAsync("/api/me");

            var response = await client.PostAsJsonAsync($"/api/admin/aprovacao-chamados/{aprovacaoId}/aprovar", new
            {
                justificativaDecisao = "Aprovado"
            });

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
        finally
        {
            await DefinirPermissaoAtivaAsync(PermissoesConstants.AprovacaoChamadosAprovar, true);
        }
    }

    [Fact]
    public async Task ReprovarProtegeReprovar()
    {
        await DefinirPermissaoAtivaAsync(PermissoesConstants.AprovacaoChamadosReprovar, false);
        try
        {
            var (_, aprovacaoId) = await CriarChamadoEAprovacaoPendenteAsync();

            using var client = _factory.CreateClient();
            AddDevHeaders(client, $"aten.aprov.reprovar.{Guid.NewGuid():N}@empresa.com", "Atendente Aprovacao", "Atendente");
            _ = await client.GetAsync("/api/me");

            var response = await client.PostAsJsonAsync($"/api/admin/aprovacao-chamados/{aprovacaoId}/reprovar", new
            {
                justificativaDecisao = "Reprovado"
            });

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
        finally
        {
            await DefinirPermissaoAtivaAsync(PermissoesConstants.AprovacaoChamadosReprovar, true);
        }
    }

    [Fact]
    public async Task CancelarProtegeCancelar()
    {
        await DefinirPermissaoAtivaAsync(PermissoesConstants.AprovacaoChamadosCancelar, false);
        try
        {
            var (_, aprovacaoId) = await CriarChamadoEAprovacaoPendenteAsync();

            using var client = _factory.CreateClient();
            AddDevHeaders(client, $"aten.aprov.cancelar.{Guid.NewGuid():N}@empresa.com", "Atendente Aprovacao", "Atendente");
            _ = await client.GetAsync("/api/me");

            var response = await client.PostAsJsonAsync($"/api/admin/aprovacao-chamados/{aprovacaoId}/cancelar", new
            {
                justificativaDecisao = "Cancelado"
            });

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
        finally
        {
            await DefinirPermissaoAtivaAsync(PermissoesConstants.AprovacaoChamadosCancelar, true);
        }
    }

    private async Task<(Guid chamadoId, Guid? aprovacaoId)> CriarChamadoEAprovacaoPendenteAsync(
        bool criarAprovacao = true,
        CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();

        var solicitante = await CriarUsuarioComPerfilAsync(
            dbContext,
            $"sol.aprov.{Guid.NewGuid():N}@empresa.com",
            "Solicitante Aprovacao",
            TipoPerfil.Solicitante,
            cancellationToken);

        var criadorAprovacao = await CriarUsuarioComPerfilAsync(
            dbContext,
            $"admin.aprov.{Guid.NewGuid():N}@empresa.com",
            "Administrador Aprovacao",
            TipoPerfil.Administrador,
            cancellationToken);

        var departamento = await dbContext.Departamentos.FirstOrDefaultAsync(cancellationToken);
        if (departamento is null)
        {
            departamento = new Departamento("Operacoes", "OPS", "Departamento de operacoes", "integration-test");
            dbContext.Departamentos.Add(departamento);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var categoria = await dbContext.CategoriasChamado.FirstOrDefaultAsync(cancellationToken);
        if (categoria is null)
        {
            categoria = new CategoriaChamado("Solicitacoes Gerais", "Categoria para aprovacao", departamento.Id, "integration-test");
            dbContext.CategoriasChamado.Add(categoria);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var prioridade = await dbContext.PrioridadesChamado.FirstAsync(cancellationToken);
        var status = await dbContext.StatusChamado.FirstAsync(x => x.Codigo == StatusChamadoEnum.Aberto, cancellationToken);

        var chamado = new Chamado(
            $"SGX-APR-{Guid.NewGuid():N}".ToUpperInvariant()[..20],
            "Chamado para autorizacao de aprovacao",
            "Descricao do chamado de aprovacao",
            solicitante.Id,
            categoria.Id,
            prioridade.Id,
            status.Id,
            OrigemChamado.Portal,
            "integration-test");

        dbContext.Chamados.Add(chamado);
        await dbContext.SaveChangesAsync(cancellationToken);

        if (!criarAprovacao)
        {
            return (chamado.Id, null);
        }

        var aprovacao = new AprovacaoChamado(
            chamado.Id,
            TipoOrigemAprovacaoChamado.Manual,
            criadorAprovacao.Id,
            criadorAprovacao.Login,
            chamado.SolicitanteId,
            "Origem manual",
            "Solicitacao inicial");

        dbContext.AprovacoesChamado.Add(aprovacao);
        await dbContext.SaveChangesAsync(cancellationToken);

        return (chamado.Id, aprovacao.Id);
    }

    private static async Task<Usuario> CriarUsuarioComPerfilAsync(
        SGXSistemaChamadoDbContext dbContext,
        string email,
        string nome,
        TipoPerfil tipoPerfil,
        CancellationToken cancellationToken)
    {
        var usuario = new Usuario(nome, email, email, "integration-test");
        dbContext.Usuarios.Add(usuario);
        await dbContext.SaveChangesAsync(cancellationToken);

        var perfil = await dbContext.PerfisAcesso.FirstAsync(x => x.TipoPerfil == tipoPerfil, cancellationToken);
        dbContext.UsuariosPerfisAcesso.Add(new UsuarioPerfilAcesso(usuario.Id, perfil.Id, "integration-test"));
        await dbContext.SaveChangesAsync(cancellationToken);

        return usuario;
    }

    private async Task DefinirPermissaoAtivaAsync(string codigoPermissao, bool ativa, CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();

        var permissao = await dbContext.PermissoesSistema
            .FirstAsync(x => x.Codigo == codigoPermissao, cancellationToken);

        if (permissao.Ativo == ativa)
        {
            return;
        }

        if (ativa)
        {
            permissao.Ativar("integration-test");
        }
        else
        {
            permissao.Desativar("integration-test");
        }

        await dbContext.SaveChangesAsync(cancellationToken);
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
}
