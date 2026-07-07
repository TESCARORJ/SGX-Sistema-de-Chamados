using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class FormularioServicosEndpointsIntegrationTests : IClassFixture<ApiIntegrationTestFactory>
{
    private readonly ApiIntegrationTestFactory _factory;

    public FormularioServicosEndpointsIntegrationTests(ApiIntegrationTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetListaFormularios()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var email = $"admin.form.endpoint.{prefixo}@empresa.com";

        using var client = _factory.CreateClient();
        AddDevHeaders(client, email, "Admin Formulario", "Administrador");
        _ = await client.GetAsync("/api/me");

        var dados = await SeedCatalogoAsync(prefixo, email);
        await SeedFormularioCompletoAsync(prefixo, dados.CatalogoId);

        var response = await client.GetAsync($"/api/admin/formulario-servicos?catalogoServicoId={dados.CatalogoId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<List<FormularioServicoAdminDto>>();
        var item = Assert.Single(payload!);
        Assert.Equal(dados.CatalogoId, item.CatalogoServicoId);
    }

    [Fact]
    public async Task GetDetalheFormulario()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var email = $"admin.form.endpoint.{prefixo}@empresa.com";

        using var client = _factory.CreateClient();
        AddDevHeaders(client, email, "Admin Formulario", "Administrador");
        _ = await client.GetAsync("/api/me");

        var dados = await SeedCatalogoAsync(prefixo, email);
        var formularioId = await SeedFormularioCompletoAsync(prefixo, dados.CatalogoId);

        var response = await client.GetAsync($"/api/admin/formulario-servicos/{formularioId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<FormularioServicoDetalheAdminDto>();
        Assert.NotNull(payload);
        Assert.Equal(formularioId, payload!.Id);
        Assert.Single(payload.Versoes);
        Assert.Single(payload.Versoes.Single().Campos);
        Assert.Equal(2, payload.Versoes.Single().Campos.Single().Opcoes.Count);
    }

    [Fact]
    public async Task PostCriaFormulario()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var email = $"admin.form.endpoint.{prefixo}@empresa.com";

        using var client = _factory.CreateClient();
        AddDevHeaders(client, email, "Admin Formulario", "Administrador");
        _ = await client.GetAsync("/api/me");

        var dados = await SeedCatalogoAsync(prefixo, email);

        var response = await client.PostAsJsonAsync("/api/admin/formulario-servicos", new
        {
            catalogoServicoId = dados.CatalogoId,
            nome = $"Formulario {prefixo}",
            descricao = "Descricao do formulario",
            ativo = true
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<FormularioServicoDetalheAdminDto>();
        Assert.NotNull(payload);
        Assert.Equal(dados.CatalogoId, payload!.CatalogoServicoId);
    }

    [Fact]
    public async Task PutAtualizaFormulario()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var email = $"admin.form.endpoint.{prefixo}@empresa.com";

        using var client = _factory.CreateClient();
        AddDevHeaders(client, email, "Admin Formulario", "Administrador");
        _ = await client.GetAsync("/api/me");

        var dados = await SeedCatalogoAsync(prefixo, email);
        var formularioId = await SeedFormularioAsync(prefixo, dados.CatalogoId);

        var response = await client.PutAsJsonAsync($"/api/admin/formulario-servicos/{formularioId}", new
        {
            nome = $"Formulario atualizado {prefixo}",
            descricao = "Descricao atualizada",
            ativo = true
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<FormularioServicoDetalheAdminDto>();
        Assert.Equal($"Formulario atualizado {prefixo}", payload!.Nome);
    }

    [Fact]
    public async Task PostInativaEReativaFormulario()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var email = $"admin.form.endpoint.{prefixo}@empresa.com";

        using var client = _factory.CreateClient();
        AddDevHeaders(client, email, "Admin Formulario", "Administrador");
        _ = await client.GetAsync("/api/me");

        var dados = await SeedCatalogoAsync(prefixo, email);
        var formularioId = await SeedFormularioAsync(prefixo, dados.CatalogoId);

        var inativar = await client.PostAsync($"/api/admin/formulario-servicos/{formularioId}/inativar", null);
        var reativar = await client.PostAsync($"/api/admin/formulario-servicos/{formularioId}/reativar", null);

        Assert.Equal(HttpStatusCode.OK, inativar.StatusCode);
        Assert.Equal(HttpStatusCode.OK, reativar.StatusCode);
    }

    [Fact]
    public async Task GetListaVersoesEPostCriaVersaoEPutAtualizaVersao()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var email = $"admin.form.endpoint.{prefixo}@empresa.com";

        using var client = _factory.CreateClient();
        AddDevHeaders(client, email, "Admin Formulario", "Administrador");
        _ = await client.GetAsync("/api/me");

        var dados = await SeedCatalogoAsync(prefixo, email);
        var formularioId = await SeedFormularioAsync(prefixo, dados.CatalogoId);

        var criar = await client.PostAsJsonAsync($"/api/admin/formulario-servicos/{formularioId}/versoes", new
        {
            numero = 1,
            publicada = false,
            ativo = true
        });

        Assert.Equal(HttpStatusCode.OK, criar.StatusCode);
        var versao = await criar.Content.ReadFromJsonAsync<FormularioServicoVersaoAdminDto>();
        Assert.NotNull(versao);

        var listar = await client.GetAsync($"/api/admin/formulario-servicos/{formularioId}/versoes");
        Assert.Equal(HttpStatusCode.OK, listar.StatusCode);

        var atualizar = await client.PutAsJsonAsync($"/api/admin/formulario-servicos/versoes/{versao!.Id}", new
        {
            numero = 2,
            publicada = false,
            ativo = true
        });

        Assert.Equal(HttpStatusCode.OK, atualizar.StatusCode);
        var atualizado = await atualizar.Content.ReadFromJsonAsync<FormularioServicoVersaoAdminDto>();
        Assert.Equal(2, atualizado!.Numero);
    }

    [Fact]
    public async Task GetListaCamposEPostCriaCampoEPutAtualizaCampo()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var email = $"admin.form.endpoint.{prefixo}@empresa.com";

        using var client = _factory.CreateClient();
        AddDevHeaders(client, email, "Admin Formulario", "Administrador");
        _ = await client.GetAsync("/api/me");

        var dados = await SeedCatalogoAsync(prefixo, email);
        var formularioId = await SeedFormularioAsync(prefixo, dados.CatalogoId);
        var versaoId = await SeedVersaoAsync(formularioId);

        var criar = await client.PostAsJsonAsync($"/api/admin/formulario-servicos/versoes/{versaoId}/campos", new
        {
            nome = "justificativa",
            rotulo = "Justificativa",
            tipo = (int)TipoCampoFormularioServico.TextoLongo,
            obrigatorio = true,
            ordem = 1,
            textoAjuda = "Explique a necessidade",
            visivel = true,
            ativo = true
        });

        Assert.Equal(HttpStatusCode.OK, criar.StatusCode);
        var campo = await criar.Content.ReadFromJsonAsync<CampoFormularioServicoAdminDto>();
        Assert.NotNull(campo);

        var listar = await client.GetAsync($"/api/admin/formulario-servicos/versoes/{versaoId}/campos");
        Assert.Equal(HttpStatusCode.OK, listar.StatusCode);

        var atualizar = await client.PutAsJsonAsync($"/api/admin/formulario-servicos/campos/{campo!.Id}", new
        {
            nome = "justificativa",
            rotulo = "Justificativa atualizada",
            tipo = (int)TipoCampoFormularioServico.TextoLongo,
            obrigatorio = false,
            ordem = 2,
            textoAjuda = "Ajuda atualizada",
            visivel = true,
            ativo = true
        });

        Assert.Equal(HttpStatusCode.OK, atualizar.StatusCode);
        var atualizado = await atualizar.Content.ReadFromJsonAsync<CampoFormularioServicoAdminDto>();
        Assert.Equal(2, atualizado!.Ordem);
    }

    [Fact]
    public async Task GetListaOpcoesEPostCriaOpcaoEPutAtualizaOpcao()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var email = $"admin.form.endpoint.{prefixo}@empresa.com";

        using var client = _factory.CreateClient();
        AddDevHeaders(client, email, "Admin Formulario", "Administrador");
        _ = await client.GetAsync("/api/me");

        var dados = await SeedCatalogoAsync(prefixo, email);
        var formularioId = await SeedFormularioAsync(prefixo, dados.CatalogoId);
        var versaoId = await SeedVersaoAsync(formularioId);
        var campoId = await SeedCampoAsync(versaoId, TipoCampoFormularioServico.SelecaoUnica);

        var criar = await client.PostAsJsonAsync($"/api/admin/formulario-servicos/campos/{campoId}/opcoes", new
        {
            valor = "vpn",
            rotulo = "VPN",
            ordem = 1,
            ativo = true
        });

        Assert.Equal(HttpStatusCode.OK, criar.StatusCode);
        var opcao = await criar.Content.ReadFromJsonAsync<OpcaoCampoFormularioServicoAdminDto>();
        Assert.NotNull(opcao);

        var listar = await client.GetAsync($"/api/admin/formulario-servicos/campos/{campoId}/opcoes");
        Assert.Equal(HttpStatusCode.OK, listar.StatusCode);

        var atualizar = await client.PutAsJsonAsync($"/api/admin/formulario-servicos/opcoes/{opcao!.Id}", new
        {
            valor = "rdp",
            rotulo = "RDP",
            ordem = 2,
            ativo = true
        });

        Assert.Equal(HttpStatusCode.OK, atualizar.StatusCode);
        var atualizada = await atualizar.Content.ReadFromJsonAsync<OpcaoCampoFormularioServicoAdminDto>();
        Assert.Equal("rdp", atualizada!.Valor);
    }

    [Fact]
    public async Task EndpointRejeitaRequestInvalido()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var email = $"admin.form.endpoint.{prefixo}@empresa.com";

        using var client = _factory.CreateClient();
        AddDevHeaders(client, email, "Admin Formulario", "Administrador");
        _ = await client.GetAsync("/api/me");

        var dados = await SeedCatalogoAsync(prefixo, email);

        var response = await client.PostAsJsonAsync("/api/admin/formulario-servicos", new
        {
            catalogoServicoId = dados.CatalogoId,
            nome = "",
            descricao = new string('a', 4100)
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task EndpointPreservaAutorizacaoAdministrativa()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var email = $"solicitante.form.endpoint.{prefixo}@empresa.com";

        using var client = _factory.CreateClient();
        AddDevHeaders(client, email, "Solicitante Formulario", "Solicitante");
        _ = await client.GetAsync("/api/me");

        var response = await client.GetAsync("/api/admin/formulario-servicos");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AtendentePodeConsultarMasNaoManterFormulario()
    {
        var prefixo = Guid.NewGuid().ToString("N");
        var email = $"atendente.form.endpoint.{prefixo}@empresa.com";

        using var client = _factory.CreateClient();
        AddDevHeaders(client, email, "Atendente Formulario", "Atendente");
        _ = await client.GetAsync("/api/me");

        var dados = await SeedCatalogoAsync(prefixo, email);
        var formularioId = await SeedFormularioAsync(prefixo, dados.CatalogoId);

        var consulta = await client.GetAsync($"/api/admin/formulario-servicos/{formularioId}");
        var criacao = await client.PostAsJsonAsync("/api/admin/formulario-servicos", new
        {
            catalogoServicoId = dados.CatalogoId,
            nome = $"Formulario bloqueado {prefixo}",
            descricao = "Descricao",
            ativo = true
        });
        var inativacao = await client.PostAsync($"/api/admin/formulario-servicos/{formularioId}/inativar", null);

        Assert.Equal(HttpStatusCode.OK, consulta.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, criacao.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, inativacao.StatusCode);
    }

    private async Task<(Guid CatalogoId, Guid AdminId)> SeedCatalogoAsync(string prefixo, string emailAdmin, CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();

        var admin = await dbContext.Usuarios
            .AsNoTracking()
            .FirstAsync(x => x.Email == emailAdmin, cancellationToken);

        var departamento = new Departamento($"Departamento {prefixo}", "DPT", null, "integration-test");
        dbContext.Departamentos.Add(departamento);
        await dbContext.SaveChangesAsync(cancellationToken);

        var catalogo = new CatalogoServico(
            $"Servico {prefixo}",
            $"servico-{prefixo}",
            "Descricao",
            null,
            departamento.Id,
            null,
            null,
            null,
            null,
            null,
            VisibilidadeCatalogoServico.Interno,
            true,
            false,
            1,
            admin.Id,
            "integration-test");

        dbContext.CatalogosServico.Add(catalogo);
        await dbContext.SaveChangesAsync(cancellationToken);
        return (catalogo.Id, admin.Id);
    }

    private async Task<Guid> SeedFormularioAsync(string prefixo, Guid catalogoId, CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();

        var formulario = new FormularioServico(catalogoId, $"Formulario {prefixo}", "Descricao", "integration-test");
        dbContext.FormulariosServico.Add(formulario);
        await dbContext.SaveChangesAsync(cancellationToken);
        return formulario.Id;
    }

    private async Task<Guid> SeedVersaoAsync(Guid formularioId, CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();

        var versao = new FormularioServicoVersao(formularioId, 1, false, null, "integration-test");
        dbContext.FormulariosServicoVersoes.Add(versao);
        await dbContext.SaveChangesAsync(cancellationToken);
        return versao.Id;
    }

    private async Task<Guid> SeedCampoAsync(Guid versaoId, TipoCampoFormularioServico tipo, CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();

        var campo = new CampoFormularioServico(
            versaoId,
            "tipo_acesso",
            "Tipo de acesso",
            tipo,
            false,
            1,
            null,
            true,
            "integration-test");

        dbContext.CamposFormularioServico.Add(campo);
        await dbContext.SaveChangesAsync(cancellationToken);
        return campo.Id;
    }

    private async Task<Guid> SeedFormularioCompletoAsync(string prefixo, Guid catalogoId, CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();

        var formulario = new FormularioServico(catalogoId, $"Formulario completo {prefixo}", "Descricao", "integration-test");
        dbContext.FormulariosServico.Add(formulario);
        await dbContext.SaveChangesAsync(cancellationToken);

        var versao = new FormularioServicoVersao(formulario.Id, 1, false, null, "integration-test");
        dbContext.FormulariosServicoVersoes.Add(versao);
        await dbContext.SaveChangesAsync(cancellationToken);

        var campo = new CampoFormularioServico(
            versao.Id,
            "tipo_acesso",
            "Tipo de acesso",
            TipoCampoFormularioServico.SelecaoMultipla,
            true,
            1,
            "Selecione as opcoes",
            true,
            "integration-test");

        dbContext.CamposFormularioServico.Add(campo);
        await dbContext.SaveChangesAsync(cancellationToken);

        dbContext.OpcoesCamposFormularioServico.Add(new OpcaoCampoFormularioServico(campo.Id, "vpn", "VPN", 1, "integration-test"));
        dbContext.OpcoesCamposFormularioServico.Add(new OpcaoCampoFormularioServico(campo.Id, "rdp", "RDP", 2, "integration-test"));
        await dbContext.SaveChangesAsync(cancellationToken);

        return formulario.Id;
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
