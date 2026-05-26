using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Tests;

public sealed class ApiHttpIntegrationTests : IClassFixture<ApiIntegrationTestFactory>
{
    private readonly ApiIntegrationTestFactory _factory;

    public ApiHttpIntegrationTests(ApiIntegrationTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task MeRetornaOkEmModoLocalDevelopment()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "solicitante.me@empresa.com", "Solicitante Teste", "Solicitante");

        var response = await client.GetAsync("/api/me");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(payload);
        Assert.Equal("LocalDevelopment", json.RootElement.GetProperty("autenticadoPor").GetString());
    }

    [Fact]
    public async Task LoginLocalSgxRetornaTokenEApiMeComAutenticadoPorLocalSgx()
    {
        const string email = "local.login@empresa.com";
        const string senha = "Senha@123456";
        await _factory.GarantirUsuarioLocalComSenhaAsync(email, "Usuario Local", senha);

        using var client = _factory.CreateClient();

        var loginResponse = await client.PostAsJsonAsync("/api/auth/local/login", new
        {
            email,
            senha
        });

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var loginPayload = await loginResponse.Content.ReadAsStringAsync();
        using var loginJson = JsonDocument.Parse(loginPayload);
        var accessToken = loginJson.RootElement.GetProperty("accessToken").GetString();
        Assert.False(string.IsNullOrWhiteSpace(accessToken));

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        var meResponse = await client.GetAsync("/api/me");

        Assert.Equal(HttpStatusCode.OK, meResponse.StatusCode);

        var mePayload = await meResponse.Content.ReadAsStringAsync();
        using var meJson = JsonDocument.Parse(mePayload);
        Assert.Equal("LocalSgx", meJson.RootElement.GetProperty("autenticadoPor").GetString());
    }

    [Fact]
    public async Task LoginLocalSgxBloqueiaUsuarioInativo()
    {
        const string email = "local.inativo@empresa.com";
        const string senha = "Senha@123456";
        await _factory.GarantirUsuarioLocalComSenhaAsync(email, "Usuario Inativo", senha, TipoPerfil.Solicitante);

        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
            var usuario = await dbContext.Usuarios.FirstAsync(x => x.Email == email);
            usuario.AlterarSituacao(SituacaoUsuario.Inativo, "integration-test");
            await dbContext.SaveChangesAsync();
        }

        using var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/auth/local/login", new
        {
            email,
            senha
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task LoginLocalSgxPermaneceDisponivelComoContingenciaQuandoMicrosoftEstaDesabilitado()
    {
        const string email = "contingencia.local@empresa.com";
        const string senha = "Senha@123456";
        await _factory.GarantirUsuarioLocalComSenhaAsync(email, "Administrador Contingencia", senha, TipoPerfil.Administrador);

        using var clientAdmin = _factory.CreateClient();
        AddDevHeaders(clientAdmin, "admin.contingencia@empresa.com", "Administrador", "Administrador");
        _ = await clientAdmin.GetAsync("/api/me");

        var atualizacao = await clientAdmin.PutAsJsonAsync("/api/admin/integracoes/microsoft-entra-id", new
        {
            habilitado = false,
            provedorPrincipal = "Local",
            loginLocalHabilitado = true,
            tenantId = "",
            clientId = "",
            audience = "",
            issuer = "",
            authority = "",
            apiScope = "",
            redirectUri = "",
            dominiosPermitidos = Array.Empty<string>(),
            criarUsuarioAutomaticamente = true,
            perfilPadraoUsuarioMicrosoft = "Solicitante"
        });

        Assert.Equal(HttpStatusCode.OK, atualizacao.StatusCode);

        using var clientLogin = _factory.CreateClient();
        var loginResponse = await clientLogin.PostAsJsonAsync("/api/auth/local/login", new
        {
            email,
            senha
        });

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
    }

    [Fact]
    public async Task AuthProvedoresRetornaConfiguracaoEsperada()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "admin.provedores@empresa.com", "Administrador", "Administrador");
        _ = await client.GetAsync("/api/me");

        _ = await client.PutAsJsonAsync("/api/admin/integracoes/microsoft-entra-id", new
        {
            habilitado = false,
            provedorPrincipal = "Local",
            loginLocalHabilitado = true,
            tenantId = "",
            clientId = "",
            audience = "",
            issuer = "",
            authority = "",
            apiScope = "",
            redirectUri = "",
            dominiosPermitidos = Array.Empty<string>(),
            criarUsuarioAutomaticamente = true,
            perfilPadraoUsuarioMicrosoft = "Solicitante"
        });

        var response = await client.GetAsync("/api/auth/provedores");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(payload);

        var provedores = json.RootElement.GetProperty("provedores").EnumerateArray().ToArray();
        Assert.NotEmpty(provedores);

        var localSgx = provedores.FirstOrDefault(x => x.GetProperty("codigo").GetString() == "LocalSgx");
        Assert.True(localSgx.ValueKind != JsonValueKind.Undefined);
        Assert.True(localSgx.GetProperty("habilitado").GetBoolean());
        Assert.True(localSgx.GetProperty("principal").GetBoolean());

        var localDevelopment = provedores.FirstOrDefault(x => x.GetProperty("codigo").GetString() == "LocalDevelopment");
        if (localDevelopment.ValueKind != JsonValueKind.Undefined)
        {
            Assert.True(localDevelopment.GetProperty("habilitado").GetBoolean());
        }
    }

    [Fact]
    public async Task LoginActiveDirectoryRetornaBadRequestQuandoProvedorDesabilitado()
    {
        using var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/ad/login", new
        {
            usuario = "thiago",
            senha = "Senha@123456",
            dominio = "EMPRESA"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AdminPodeConsultarEModificarMetodosDeLogin()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "admin.auth.methods@empresa.com", "Administrador", "Administrador");

        var getResponse = await client.GetAsync("/api/admin/autenticacao/provedores");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var payloadGet = await getResponse.Content.ReadAsStringAsync();
        using var jsonGet = JsonDocument.Parse(payloadGet);
        var provedores = jsonGet.RootElement.GetProperty("provedores").EnumerateArray().ToArray();

        var payloadPut = new
        {
            provedores = provedores.Select(p => new
            {
                codigo = p.GetProperty("codigo").GetString(),
                habilitado = p.GetProperty("codigo").GetString() == "LocalSgx",
                principal = p.GetProperty("codigo").GetString() == "LocalSgx",
                ordem = p.GetProperty("ordem").GetInt32(),
                permiteAutoProvisionamento = p.GetProperty("permiteAutoProvisionamento").GetBoolean(),
                perfilPadraoAutoProvisionamento = p.GetProperty("perfilPadraoAutoProvisionamento").GetString(),
                rotuloExibicao = p.GetProperty("rotuloExibicao").GetString()
            }).ToArray()
        };

        var putResponse = await client.PutAsJsonAsync("/api/admin/autenticacao/provedores", payloadPut);
        Assert.Equal(HttpStatusCode.OK, putResponse.StatusCode);

        var authResponse = await client.GetAsync("/api/auth/provedores");
        Assert.Equal(HttpStatusCode.OK, authResponse.StatusCode);
        var payloadAuth = await authResponse.Content.ReadAsStringAsync();
        using var authJson = JsonDocument.Parse(payloadAuth);
        var publicos = authJson.RootElement.GetProperty("provedores").EnumerateArray().ToArray();
        Assert.Single(publicos);
        Assert.Equal("LocalSgx", publicos[0].GetProperty("codigo").GetString());
    }

    [Fact]
    public async Task AtendenteSemPermissaoNaoPodeAlterarMetodosDeLogin()
    {
        var email = $"atendente.auth.methods.{Guid.NewGuid():N}@empresa.com";
        using var client = _factory.CreateClient();
        AddDevHeaders(client, email, "Atendente", "Atendente");

        var response = await client.PutAsJsonAsync("/api/admin/autenticacao/provedores", new
        {
            provedores = new[]
            {
                new
                {
                    codigo = "LocalSgx",
                    habilitado = true,
                    principal = true,
                    ordem = 10,
                    permiteAutoProvisionamento = false,
                    perfilPadraoAutoProvisionamento = "Solicitante",
                    rotuloExibicao = "Local SGX"
                },
                new
                {
                    codigo = "ActiveDirectory",
                    habilitado = false,
                    principal = false,
                    ordem = 20,
                    permiteAutoProvisionamento = false,
                    perfilPadraoAutoProvisionamento = "Solicitante",
                    rotuloExibicao = "Active Directory"
                },
                new
                {
                    codigo = "MicrosoftEntraId",
                    habilitado = false,
                    principal = false,
                    ordem = 30,
                    permiteAutoProvisionamento = false,
                    perfilPadraoAutoProvisionamento = "Solicitante",
                    rotuloExibicao = "Microsoft Entra ID"
                },
                new
                {
                    codigo = "LocalDevelopment",
                    habilitado = false,
                    principal = false,
                    ordem = 40,
                    permiteAutoProvisionamento = false,
                    perfilPadraoAutoProvisionamento = "Solicitante",
                    rotuloExibicao = "Local Development"
                }
            }
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        var evento = await dbContext.EventosAuditoria
            .OrderByDescending(x => x.DataEvento)
            .FirstOrDefaultAsync(x =>
                x.Modulo == "Autenticacao" &&
                x.Entidade == "MetodosLogin" &&
                x.Descricao.Contains("Tentativa negada", StringComparison.OrdinalIgnoreCase));

        Assert.NotNull(evento);
        Assert.False(evento!.Sucesso);
    }

    [Fact]
    public async Task AdminPodeConsultarAuditoriaAutenticacao()
    {
        const string email = "auditoria.local.sgx@empresa.com";
        const string senha = "Senha@123456";
        await _factory.GarantirUsuarioLocalComSenhaAsync(email, "Auditoria Local SGX", senha);

        using (var clientLogin = _factory.CreateClient())
        {
            var loginResponse = await clientLogin.PostAsJsonAsync("/api/auth/local/login", new
            {
                email,
                senha
            });

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        }

        using var client = _factory.CreateClient();
        AddDevHeaders(client, "admin.audit.auth@empresa.com", "Administrador", "Administrador");

        var response = await client.GetAsync("/api/admin/auditoria/autenticacao?pagina=1&tamanhoPagina=10&provedor=LocalSgx");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(payload);
        Assert.True(json.RootElement.TryGetProperty("items", out var items));
        Assert.Equal(JsonValueKind.Array, items.ValueKind);
        Assert.NotEmpty(items.EnumerateArray());

        var primeiro = items.EnumerateArray().First();
        Assert.Equal("LocalSgx", primeiro.GetProperty("provedor").GetString());
        Assert.True(primeiro.TryGetProperty("tipoEvento", out _));
        Assert.True(primeiro.TryGetProperty("resultado", out _));
        Assert.True(primeiro.TryGetProperty("mensagem", out _));
    }

    [Fact]
    public async Task AtendenteSemPermissaoNaoPodeConsultarAuditoriaAutenticacao()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "atendente.audit.auth@empresa.com", "Atendente", "Atendente");

        var response = await client.GetAsync("/api/admin/auditoria/autenticacao");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task MeRetornaPermissoesEfetivas()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "atendente.me@empresa.com", "Atendente Teste", "Atendente");

        _ = await client.GetAsync("/api/me");
        var response = await client.GetAsync("/api/me");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(payload);
        Assert.True(json.RootElement.TryGetProperty("permissoes", out var permissoes));
        Assert.Equal(JsonValueKind.Array, permissoes.ValueKind);
        Assert.Contains(permissoes.EnumerateArray().Select(x => x.GetString()), x => x == "Chamados.Assumir");
    }

    [Fact]
    public async Task PortalChamadosMantemIsolamentoPorSolicitante()
    {
        const string solicitanteA = "solicitante.a@empresa.com";
        const string solicitanteB = "solicitante.b@empresa.com";

        using (var clienteA = _factory.CreateClient())
        {
            AddDevHeaders(clienteA, solicitanteA, "Solicitante A", "Solicitante");
            _ = await clienteA.GetAsync("/api/me");
        }

        using (var clienteB = _factory.CreateClient())
        {
            AddDevHeaders(clienteB, solicitanteB, "Solicitante B", "Solicitante");
            _ = await clienteB.GetAsync("/api/me");
        }

        await _factory.SeedPortalChamadosAsync(solicitanteA, solicitanteB);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, solicitanteA, "Solicitante A", "Solicitante");
        var response = await client.GetAsync("/api/portal/chamados");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("SGX-2026-900001", content);
        Assert.DoesNotContain("SGX-2026-900002", content);
    }

    [Fact]
    public async Task PortalStatusAprovacaoDeveBloquearAcessoAoChamadoDeOutroSolicitante()
    {
        const string solicitanteA = "solicitante.aprovacao.a@empresa.com";
        const string solicitanteB = "solicitante.aprovacao.b@empresa.com";

        using (var clienteA = _factory.CreateClient())
        {
            AddDevHeaders(clienteA, solicitanteA, "Solicitante A", "Solicitante");
            _ = await clienteA.GetAsync("/api/me");
        }

        using (var clienteB = _factory.CreateClient())
        {
            AddDevHeaders(clienteB, solicitanteB, "Solicitante B", "Solicitante");
            _ = await clienteB.GetAsync("/api/me");
        }

        await _factory.SeedPortalChamadosAsync(solicitanteA, solicitanteB);
        var chamadoDoSolicitanteB = await ObterChamadoPorCodigoAsync("SGX-2026-900002");

        using var client = _factory.CreateClient();
        AddDevHeaders(client, solicitanteA, "Solicitante A", "Solicitante");

        var response = await client.GetAsync($"/api/portal/chamados/{chamadoDoSolicitanteB.Id}/aprovacao");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task PortalStatusAprovacaoRetornaDadosOrientativosSemCamposAdministrativos()
    {
        const string solicitante = "solicitante.aprovacao.status@empresa.com";
        const string solicitanteOutro = "solicitante.aprovacao.outro@empresa.com";

        using (var clientBootstrap = _factory.CreateClient())
        {
            AddDevHeaders(clientBootstrap, solicitante, "Solicitante", "Solicitante");
            _ = await clientBootstrap.GetAsync("/api/me");
        }

        using (var clientBootstrapOutro = _factory.CreateClient())
        {
            AddDevHeaders(clientBootstrapOutro, solicitanteOutro, "Solicitante Outro", "Solicitante");
            _ = await clientBootstrapOutro.GetAsync("/api/me");
        }

        await _factory.SeedPortalChamadosAsync(solicitante, solicitanteOutro);
        var chamado = await ObterChamadoPorCodigoAsync("SGX-2026-900001");
        await CriarAprovacaoNoChamadoAsync(chamado.Id, chamado.SolicitanteId, StatusAprovacaoChamado.Reprovado, "Necessario anexar evidencias.");

        using var client = _factory.CreateClient();
        AddDevHeaders(client, solicitante, "Solicitante", "Solicitante");

        var response = await client.GetAsync($"/api/portal/chamados/{chamado.Id}/aprovacao");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(payload);

        Assert.Equal(chamado.Id.ToString(), json.RootElement.GetProperty("chamadoId").GetString());
        Assert.True(json.RootElement.GetProperty("requerAprovacao").GetBoolean());
        Assert.Equal((int)StatusAprovacaoChamado.Reprovado, json.RootElement.GetProperty("statusAprovacao").GetInt32());
        Assert.Equal("Necessario anexar evidencias.", json.RootElement.GetProperty("justificativaDecisao").GetString());
        Assert.Equal("Seu chamado foi reprovado. Verifique a justificativa.", json.RootElement.GetProperty("mensagemOrientativa").GetString());
        Assert.False(json.RootElement.TryGetProperty("aprovadorNome", out _));
        Assert.False(json.RootElement.TryGetProperty("aprovadorId", out _));
    }

    [Fact]
    public async Task AdminChamadosBloqueiaSolicitante()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "solicitante.admin@empresa.com", "Solicitante", "Solicitante");

        var response = await client.GetAsync("/api/admin/chamados");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AdminDashboardBloqueiaSolicitante()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "solicitante.dashboard@empresa.com", "Solicitante", "Solicitante");

        var response = await client.GetAsync("/api/admin/dashboard");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AdminCadastrosMutacaoBloqueiaSolicitante()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "solicitante.cadastro@empresa.com", "Solicitante", "Solicitante");

        var response = await client.PostAsJsonAsync("/api/admin/cadastros/departamentos", new
        {
            nome = "Departamento Teste",
            sigla = "DTT",
            descricao = "Departamento de teste"
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AdminCadastrosAliasDeveListarDepartamentos()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "admin.cad.alias@empresa.com", "Administrador", "Administrador");

        var response = await client.GetAsync("/api/admin/departamentos");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task AdminSubcategoriasCrudComDeleteLogicoEPatchAtivar()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "admin.subcat@empresa.com", "Administrador", "Administrador");

        var categoriaResponse = await client.PostAsJsonAsync("/api/admin/categorias", new
        {
            nome = $"Categoria API {Guid.NewGuid():N}",
            descricao = "Categoria para teste de subcategoria"
        });
        Assert.Equal(HttpStatusCode.OK, categoriaResponse.StatusCode);
        var categoriaId = await ObterGuidDaRespostaAsync(categoriaResponse, "id");

        var criarSubcategoriaResponse = await client.PostAsJsonAsync("/api/admin/subcategorias", new
        {
            categoriaChamadoId = categoriaId,
            nome = "Subcategoria API",
            descricao = "Subcategoria de teste"
        });
        Assert.Equal(HttpStatusCode.OK, criarSubcategoriaResponse.StatusCode);
        var subcategoriaId = await ObterGuidDaRespostaAsync(criarSubcategoriaResponse, "id");

        var listagemCategoriaAtiva = await client.GetAsync($"/api/admin/categorias/{categoriaId}/subcategorias?ativo=true");
        Assert.Equal(HttpStatusCode.OK, listagemCategoriaAtiva.StatusCode);
        var listagemAtivaAntesDelete = await listagemCategoriaAtiva.Content.ReadAsStringAsync();
        Assert.Contains(subcategoriaId.ToString(), listagemAtivaAntesDelete, StringComparison.OrdinalIgnoreCase);

        var deleteResponse = await client.DeleteAsync($"/api/admin/subcategorias/{subcategoriaId}");
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

        var listagemCategoriaAposDelete = await client.GetAsync($"/api/admin/categorias/{categoriaId}/subcategorias?ativo=true");
        Assert.Equal(HttpStatusCode.OK, listagemCategoriaAposDelete.StatusCode);
        var listagemAtivaAposDelete = await listagemCategoriaAposDelete.Content.ReadAsStringAsync();
        Assert.DoesNotContain(subcategoriaId.ToString(), listagemAtivaAposDelete, StringComparison.OrdinalIgnoreCase);

        var ativarResponse = await client.PatchAsync($"/api/admin/subcategorias/{subcategoriaId}/ativar", null);
        Assert.Equal(HttpStatusCode.OK, ativarResponse.StatusCode);

        var listagemCategoriaAposAtivar = await client.GetAsync($"/api/admin/categorias/{categoriaId}/subcategorias?ativo=true");
        Assert.Equal(HttpStatusCode.OK, listagemCategoriaAposAtivar.StatusCode);
        var listagemAtivaAposAtivar = await listagemCategoriaAposAtivar.Content.ReadAsStringAsync();
        Assert.Contains(subcategoriaId.ToString(), listagemAtivaAposAtivar, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AdminPrioridadesCrudComAliasesEPatchAtivar()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "admin.prioridades@empresa.com", "Administrador", "Administrador");

        var nome = $"Prioridade API {Guid.NewGuid():N}";
        var criarResponse = await client.PostAsJsonAsync("/api/admin/prioridades", new
        {
            nome,
            descricao = "Prioridade para teste HTTP",
            peso = 15,
            cor = "#FF5500"
        });
        Assert.Equal(HttpStatusCode.OK, criarResponse.StatusCode);
        var prioridadeId = await ObterGuidDaRespostaAsync(criarResponse, "id");

        var listagemAlias = await client.GetAsync($"/api/admin/cadastros/prioridades?texto={Uri.EscapeDataString(nome)}");
        Assert.Equal(HttpStatusCode.OK, listagemAlias.StatusCode);
        var payloadAlias = await listagemAlias.Content.ReadAsStringAsync();
        Assert.Contains(prioridadeId.ToString(), payloadAlias, StringComparison.OrdinalIgnoreCase);

        var deleteResponse = await client.DeleteAsync($"/api/admin/prioridades/{prioridadeId}");
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

        var ativosAposDelete = await client.GetAsync($"/api/admin/prioridades?texto={Uri.EscapeDataString(nome)}&ativo=true");
        Assert.Equal(HttpStatusCode.OK, ativosAposDelete.StatusCode);
        var payloadAtivosAposDelete = await ativosAposDelete.Content.ReadAsStringAsync();
        Assert.DoesNotContain(prioridadeId.ToString(), payloadAtivosAposDelete, StringComparison.OrdinalIgnoreCase);

        var ativarResponse = await client.PatchAsync($"/api/admin/prioridades/{prioridadeId}/ativar", null);
        Assert.Equal(HttpStatusCode.OK, ativarResponse.StatusCode);

        var ativosAposAtivar = await client.GetAsync($"/api/admin/prioridades?texto={Uri.EscapeDataString(nome)}&ativo=true");
        Assert.Equal(HttpStatusCode.OK, ativosAposAtivar.StatusCode);
        var payloadAtivosAposAtivar = await ativosAposAtivar.Content.ReadAsStringAsync();
        Assert.Contains(prioridadeId.ToString(), payloadAtivosAposAtivar, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AdminTiposSolicitacaoCrudComAliasesEPatchAtivar()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "admin.tipos@empresa.com", "Administrador", "Administrador");

        var nome = $"Tipo API {Guid.NewGuid():N}";
        var criarResponse = await client.PostAsJsonAsync("/api/admin/tipos-solicitacao", new
        {
            nome,
            descricao = "Tipo para teste HTTP"
        });
        Assert.Equal(HttpStatusCode.OK, criarResponse.StatusCode);
        var tipoId = await ObterGuidDaRespostaAsync(criarResponse, "id");

        var listagemAlias = await client.GetAsync($"/api/admin/cadastros/tipos-solicitacao?texto={Uri.EscapeDataString(nome)}");
        Assert.Equal(HttpStatusCode.OK, listagemAlias.StatusCode);
        var payloadAlias = await listagemAlias.Content.ReadAsStringAsync();
        Assert.Contains(tipoId.ToString(), payloadAlias, StringComparison.OrdinalIgnoreCase);

        var deleteResponse = await client.DeleteAsync($"/api/admin/tipos-solicitacao/{tipoId}");
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

        var ativosAposDelete = await client.GetAsync($"/api/admin/tipos-solicitacao?texto={Uri.EscapeDataString(nome)}&ativo=true");
        Assert.Equal(HttpStatusCode.OK, ativosAposDelete.StatusCode);
        var payloadAtivosAposDelete = await ativosAposDelete.Content.ReadAsStringAsync();
        Assert.DoesNotContain(tipoId.ToString(), payloadAtivosAposDelete, StringComparison.OrdinalIgnoreCase);

        var ativarResponse = await client.PatchAsync($"/api/admin/tipos-solicitacao/{tipoId}/ativar", null);
        Assert.Equal(HttpStatusCode.OK, ativarResponse.StatusCode);

        var ativosAposAtivar = await client.GetAsync($"/api/admin/tipos-solicitacao?texto={Uri.EscapeDataString(nome)}&ativo=true");
        Assert.Equal(HttpStatusCode.OK, ativosAposAtivar.StatusCode);
        var payloadAtivosAposAtivar = await ativosAposAtivar.Content.ReadAsStringAsync();
        Assert.Contains(tipoId.ToString(), payloadAtivosAposAtivar, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AdminLocaisUnidadeCrudComAliasesEPatchAtivar()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "admin.locais@empresa.com", "Administrador", "Administrador");

        var nome = $"Local API {Guid.NewGuid():N}";
        var criarResponse = await client.PostAsJsonAsync("/api/admin/locais", new
        {
            nome,
            descricao = "Local para teste HTTP",
            endereco = "Rua Teste, 123"
        });
        Assert.Equal(HttpStatusCode.OK, criarResponse.StatusCode);
        var localId = await ObterGuidDaRespostaAsync(criarResponse, "id");

        var listagemAlias = await client.GetAsync($"/api/admin/cadastros/locais?texto={Uri.EscapeDataString(nome)}");
        Assert.Equal(HttpStatusCode.OK, listagemAlias.StatusCode);
        var payloadAlias = await listagemAlias.Content.ReadAsStringAsync();
        Assert.Contains(localId.ToString(), payloadAlias, StringComparison.OrdinalIgnoreCase);

        var deleteResponse = await client.DeleteAsync($"/api/admin/locais/{localId}");
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

        var ativosAposDelete = await client.GetAsync($"/api/admin/locais?texto={Uri.EscapeDataString(nome)}&ativo=true");
        Assert.Equal(HttpStatusCode.OK, ativosAposDelete.StatusCode);
        var payloadAtivosAposDelete = await ativosAposDelete.Content.ReadAsStringAsync();
        Assert.DoesNotContain(localId.ToString(), payloadAtivosAposDelete, StringComparison.OrdinalIgnoreCase);

        var ativarResponse = await client.PatchAsync($"/api/admin/locais/{localId}/ativar", null);
        Assert.Equal(HttpStatusCode.OK, ativarResponse.StatusCode);

        var ativosAposAtivar = await client.GetAsync($"/api/admin/locais?texto={Uri.EscapeDataString(nome)}&ativo=true");
        Assert.Equal(HttpStatusCode.OK, ativosAposAtivar.StatusCode);
        var payloadAtivosAposAtivar = await ativosAposAtivar.Content.ReadAsStringAsync();
        Assert.Contains(localId.ToString(), payloadAtivosAposAtivar, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AdminIntegracaoEmailLogsBloqueiaSolicitante()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "solicitante.email@empresa.com", "Solicitante", "Solicitante");

        var response = await client.GetAsync("/api/admin/integracoes/email/logs");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AdminIntegracaoEmailLogsPermiteAtendente()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "atendente.email@empresa.com", "Atendente", "Atendente");
        _ = await client.GetAsync("/api/me");

        var response = await client.GetAsync("/api/admin/integracoes/email/logs");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task AdminAuditoriaBloqueiaSolicitante()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "solicitante.auditoria@empresa.com", "Solicitante", "Solicitante");

        var response = await client.GetAsync("/api/admin/auditoria/eventos");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AdminAuditoriaAtendenteSemPermissaoNaoVisualiza()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "atendente.auditoria@empresa.com", "Atendente", "Atendente");
        _ = await client.GetAsync("/api/me");

        var response = await client.GetAsync("/api/admin/auditoria/eventos");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AdminAuditoriaAdministradorVisualizaListagemEDashboard()
    {
        await SeedEventoAuditoriaApiAsync();
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "admin.auditoria@empresa.com", "Administrador", "Administrador");
        _ = await client.GetAsync("/api/me");

        var listagem = await client.GetAsync("/api/admin/auditoria/eventos?modulo=Chamados&pagina=1&tamanhoPagina=10");
        var dashboard = await client.GetAsync("/api/admin/auditoria/dashboard?modulo=Chamados");

        Assert.Equal(HttpStatusCode.OK, listagem.StatusCode);
        Assert.Equal(HttpStatusCode.OK, dashboard.StatusCode);
    }

    [Fact]
    public async Task AdminAuditoriaDetalheInexistenteRetornaNotFound()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "admin.auditoria.notfound@empresa.com", "Administrador", "Administrador");
        _ = await client.GetAsync("/api/me");

        var response = await client.GetAsync($"/api/admin/auditoria/eventos/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AdminIntegracaoMicrosoftBloqueiaSolicitante()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "solicitante.microsoft@empresa.com", "Solicitante", "Solicitante");

        var response = await client.GetAsync("/api/admin/integracoes/microsoft-entra-id");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AdminIntegracaoMicrosoftAtendenteSemPermissaoNaoVisualiza()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "atendente.microsoft@empresa.com", "Atendente", "Atendente");
        _ = await client.GetAsync("/api/me");

        var response = await client.GetAsync("/api/admin/integracoes/microsoft-entra-id");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AdminIntegracaoMicrosoftAdministradorAtualizaConfiguracao()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "admin.microsoft@empresa.com", "Administrador", "Administrador");
        _ = await client.GetAsync("/api/me");

        var payload = new
        {
            habilitado = true,
            provedorPrincipal = "Hibrido",
            loginLocalHabilitado = true,
            tenantId = "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
            clientId = "bbbbbbbb-cccc-dddd-eeee-ffffffffffff",
            audience = "api://sgx-api",
            issuer = "https://login.microsoftonline.com/aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee/v2.0",
            authority = "https://login.microsoftonline.com/aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee/v2.0",
            apiScope = "api://sgx-api/access_as_user",
            redirectUri = "http://localhost:5173",
            dominiosPermitidos = new[] { "empresa.com" },
            criarUsuarioAutomaticamente = true,
            perfilPadraoUsuarioMicrosoft = "Solicitante"
        };

        var response = await client.PutAsJsonAsync("/api/admin/integracoes/microsoft-entra-id", payload);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(content);
        Assert.Equal("Hibrido", json.RootElement.GetProperty("provedorPrincipal").GetString());
        Assert.True(json.RootElement.GetProperty("habilitado").GetBoolean());
        Assert.True(json.RootElement.GetProperty("loginLocalHabilitado").GetBoolean());
    }

    [Fact]
    public async Task AdminIntegracaoMicrosoftRejeitaSemTenantQuandoHabilitado()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "admin.microsoft.rejeita@empresa.com", "Administrador", "Administrador");
        _ = await client.GetAsync("/api/me");

        var payload = new
        {
            habilitado = true,
            provedorPrincipal = "MicrosoftEntraId",
            loginLocalHabilitado = false,
            tenantId = "",
            clientId = "bbbbbbbb-cccc-dddd-eeee-ffffffffffff",
            audience = "api://sgx-api",
            issuer = "https://login.microsoftonline.com/aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee/v2.0",
            authority = "https://login.microsoftonline.com/aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee/v2.0",
            apiScope = "api://sgx-api/access_as_user",
            redirectUri = "http://localhost:5173",
            dominiosPermitidos = Array.Empty<string>(),
            criarUsuarioAutomaticamente = true,
            perfilPadraoUsuarioMicrosoft = "Solicitante"
        };

        var response = await client.PutAsJsonAsync("/api/admin/integracoes/microsoft-entra-id", payload);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
            "Tenant ID é obrigatório quando a integração Microsoft está habilitada.",
            await ObterMensagemErroAsync(response));
    }

    [Fact]
    public async Task AdminIntegracaoMicrosoftRejeitaSemClientIdQuandoHabilitado()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "admin.microsoft.semclient@empresa.com", "Administrador", "Administrador");
        _ = await client.GetAsync("/api/me");

        var payload = CriarPayloadMicrosoftValido(clientId: "");
        var response = await client.PutAsJsonAsync("/api/admin/integracoes/microsoft-entra-id", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
            "Client ID é obrigatório quando a integração Microsoft está habilitada.",
            await ObterMensagemErroAsync(response));
    }

    [Fact]
    public async Task AdminIntegracaoMicrosoftRejeitaSemAudienceQuandoHabilitado()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "admin.microsoft.semaudience@empresa.com", "Administrador", "Administrador");
        _ = await client.GetAsync("/api/me");

        var payload = CriarPayloadMicrosoftValido(audience: "");
        var response = await client.PutAsJsonAsync("/api/admin/integracoes/microsoft-entra-id", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
            "Audience é obrigatória quando a integração Microsoft está habilitada.",
            await ObterMensagemErroAsync(response));
    }

    [Fact]
    public async Task AdminIntegracaoMicrosoftRejeitaSemIssuerQuandoHabilitado()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "admin.microsoft.semissuer@empresa.com", "Administrador", "Administrador");
        _ = await client.GetAsync("/api/me");

        var payload = CriarPayloadMicrosoftValido(issuer: "");
        var response = await client.PutAsJsonAsync("/api/admin/integracoes/microsoft-entra-id", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
            "Issuer é obrigatório quando a integração Microsoft está habilitada.",
            await ObterMensagemErroAsync(response));
    }

    [Fact]
    public async Task AdminIntegracaoMicrosoftRejeitaSemAuthorityQuandoHabilitado()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "admin.microsoft.semauthority@empresa.com", "Administrador", "Administrador");
        _ = await client.GetAsync("/api/me");

        var payload = CriarPayloadMicrosoftValido(authority: "");
        var response = await client.PutAsJsonAsync("/api/admin/integracoes/microsoft-entra-id", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
            "Authority é obrigatória quando a integração Microsoft está habilitada.",
            await ObterMensagemErroAsync(response));
    }

    [Fact]
    public async Task AdminIntegracaoMicrosoftRejeitaSemApiScopeQuandoHabilitado()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "admin.microsoft.semscope@empresa.com", "Administrador", "Administrador");
        _ = await client.GetAsync("/api/me");

        var payload = CriarPayloadMicrosoftValido(apiScope: "");
        var response = await client.PutAsJsonAsync("/api/admin/integracoes/microsoft-entra-id", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
            "API Scope é obrigatório quando a integração Microsoft está habilitada.",
            await ObterMensagemErroAsync(response));
    }

    [Fact]
    public async Task AdminIntegracaoMicrosoftRejeitaSemRedirectUriQuandoHabilitado()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "admin.microsoft.semredirect@empresa.com", "Administrador", "Administrador");
        _ = await client.GetAsync("/api/me");

        var payload = CriarPayloadMicrosoftValido(redirectUri: "");
        var response = await client.PutAsJsonAsync("/api/admin/integracoes/microsoft-entra-id", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
            "Redirect URI é obrigatório quando a integração Microsoft está habilitada.",
            await ObterMensagemErroAsync(response));
    }

    [Fact]
    public async Task AdminIntegracaoMicrosoftAceitaModoLocalSemCamposMicrosoft()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "admin.microsoft.local@empresa.com", "Administrador", "Administrador");
        _ = await client.GetAsync("/api/me");

        var payload = CriarPayloadMicrosoftValido(
            habilitado: false,
            provedorPrincipal: "Local",
            loginLocalHabilitado: true,
            tenantId: "",
            clientId: "",
            audience: "",
            issuer: "",
            authority: "",
            apiScope: "",
            redirectUri: "");

        var response = await client.PutAsJsonAsync("/api/admin/integracoes/microsoft-entra-id", payload);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task AdminIntegracaoMicrosoftRejeitaModoLocalSemLoginLocalHabilitado()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "admin.microsoft.localsemlocal@empresa.com", "Administrador", "Administrador");
        _ = await client.GetAsync("/api/me");

        var payload = CriarPayloadMicrosoftValido(
            habilitado: false,
            provedorPrincipal: "Local",
            loginLocalHabilitado: false,
            tenantId: "",
            clientId: "",
            audience: "",
            issuer: "",
            authority: "",
            apiScope: "",
            redirectUri: "");

        var response = await client.PutAsJsonAsync("/api/admin/integracoes/microsoft-entra-id", payload);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
            "Login local SGX deve permanecer habilitado quando o modo Local estiver selecionado.",
            await ObterMensagemErroAsync(response));
    }

    [Fact]
    public async Task AdminIntegracaoMicrosoftRejeitaSemProvedorAtivo()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "admin.microsoft.semprovedor@empresa.com", "Administrador", "Administrador");
        _ = await client.GetAsync("/api/me");

        var payload = new
        {
            habilitado = false,
            provedorPrincipal = "MicrosoftEntraId",
            loginLocalHabilitado = false,
            tenantId = "",
            clientId = "",
            audience = "",
            issuer = "",
            authority = "",
            apiScope = "",
            redirectUri = "",
            dominiosPermitidos = Array.Empty<string>(),
            criarUsuarioAutomaticamente = false,
            perfilPadraoUsuarioMicrosoft = "Solicitante"
        };

        var response = await client.PutAsJsonAsync("/api/admin/integracoes/microsoft-entra-id", payload);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
            "Ao menos um provedor de autenticação deve permanecer habilitado.",
            await ObterMensagemErroAsync(response));
    }

    [Fact]
    public async Task AdminSlaPoliciesBloqueiaSolicitante()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "solicitante.sla@empresa.com", "Solicitante", "Solicitante");

        var response = await client.GetAsync("/api/admin/sla/policies");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AdminSlaPoliciesPermiteAtendenteVisualizar()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "atendente.sla@empresa.com", "Atendente", "Atendente");
        _ = await client.GetAsync("/api/me");

        var response = await client.GetAsync("/api/admin/sla/policies");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task AdminSlaPoliciesAdministradorCriaPolitica()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "admin.sla@empresa.com", "Administrador", "Administrador");
        _ = await client.GetAsync("/api/me");

        var payload = new
        {
            nome = "SLA Infraestrutura",
            descricao = "Politica de SLA para infraestrutura.",
            ativo = true,
            ordem = 20,
            categoriaId = (string?)null,
            departamentoId = (string?)null,
            usarHorarioComercial = false,
            pausarQuandoAguardandoSolicitante = true,
            metas = new[]
            {
                new
                {
                    prioridadeId = SeedData.PrioridadeBaixaId,
                    tempoPrimeiraRespostaMinutos = 480,
                    tempoResolucaoMinutos = 2880,
                    tempoAtualizacaoMinutos = (int?)null,
                    tempoRespostaSubsequenteMinutos = (int?)null,
                    ativo = true
                },
                new
                {
                    prioridadeId = SeedData.PrioridadeMediaId,
                    tempoPrimeiraRespostaMinutos = 240,
                    tempoResolucaoMinutos = 1440,
                    tempoAtualizacaoMinutos = (int?)null,
                    tempoRespostaSubsequenteMinutos = (int?)null,
                    ativo = true
                }
            }
        };

        var response = await client.PostAsJsonAsync("/api/admin/sla/policies", payload);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task AdminCadastrosPermissoesListaComSucessoParaAtendente()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "atendente.permissoes@empresa.com", "Atendente", "Atendente");

        var response = await client.GetAsync("/api/admin/cadastros/permissoes");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task AdminCadastrosPermissoesPerfilRetornaComSucesso()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "atendente.perfil@empresa.com", "Atendente", "Atendente");

        var response = await client.GetAsync($"/api/admin/cadastros/perfis/{SeedData.PerfilAtendenteId}/permissoes");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task AtendenteNaoAtualizaPermissoesDePerfil()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "atendente.put.permissao@empresa.com", "Atendente", "Atendente");

        var response = await client.PutAsJsonAsync($"/api/admin/cadastros/perfis/{SeedData.PerfilAtendenteId}/permissoes", new
        {
            codigosPermissoes = new[] { "Chamados.Visualizar", "Chamados.Assumir" }
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AdministradorAtualizaPermissoesDePerfil()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "admin.put.permissao@empresa.com", "Administrador", "Administrador");

        var response = await client.PutAsJsonAsync($"/api/admin/cadastros/perfis/{SeedData.PerfilAtendenteId}/permissoes", new
        {
            codigosPermissoes = new[] { "Chamados.Visualizar", "Chamados.Assumir", "Dashboard.Visualizar" }
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task AdministradorRedefineSenhaDeUsuario()
    {
        const string email = "usuario.redefinir@empresa.com";
        const string senhaInicial = "Senha@123456";
        const string senhaNova = "NovaSenha@123456";
        await _factory.GarantirUsuarioLocalComSenhaAsync(email, "Usuario Redefinir", senhaInicial);

        using var clientAdmin = _factory.CreateClient();
        AddDevHeaders(clientAdmin, "admin.redefinir@empresa.com", "Administrador", "Administrador");
        _ = await clientAdmin.GetAsync("/api/me");
        var usuarioId = await ObterUsuarioIdPorEmail(email);

        var redefinirResponse = await clientAdmin.PostAsJsonAsync($"/api/admin/cadastros/usuarios/{usuarioId}/redefinir-senha", new
        {
            novaSenha = senhaNova,
            confirmarNovaSenha = senhaNova,
            deveAlterarSenha = true
        });

        Assert.Equal(HttpStatusCode.OK, redefinirResponse.StatusCode);

        using var clientLogin = _factory.CreateClient();
        var loginNovo = await clientLogin.PostAsJsonAsync("/api/auth/local/login", new { email, senha = senhaNova });
        Assert.Equal(HttpStatusCode.OK, loginNovo.StatusCode);
    }

    [Fact]
    public async Task AtendenteNaoPodeRedefinirSenhaDeUsuario()
    {
        const string email = "usuario.redefinir2@empresa.com";
        const string senhaInicial = "Senha@123456";
        await _factory.GarantirUsuarioLocalComSenhaAsync(email, "Usuario Redefinir 2", senhaInicial);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, "atendente.redefinir@empresa.com", "Atendente", "Atendente");
        _ = await client.GetAsync("/api/me");
        var usuarioId = await ObterUsuarioIdPorEmail(email);

        var response = await client.PostAsJsonAsync($"/api/admin/cadastros/usuarios/{usuarioId}/redefinir-senha", new
        {
            novaSenha = "NovaSenha@123456",
            confirmarNovaSenha = "NovaSenha@123456",
            deveAlterarSenha = true
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CadastrosOperacionaisRetornamSomenteRegistrosAtivos()
    {
        var dados = await SeedCadastrosOperacionaisAsync();
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "solicitante.cad.operacional@empresa.com", "Solicitante", "Solicitante");

        var departamentos = await client.GetAsync("/api/cadastros/departamentos/ativos");
        var categorias = await client.GetAsync("/api/cadastros/categorias/ativas");
        var prioridades = await client.GetAsync("/api/cadastros/prioridades/ativas");
        var tipos = await client.GetAsync("/api/cadastros/tipos-solicitacao/ativos");
        var locais = await client.GetAsync("/api/cadastros/locais/ativos");

        Assert.Equal(HttpStatusCode.OK, departamentos.StatusCode);
        Assert.Equal(HttpStatusCode.OK, categorias.StatusCode);
        Assert.Equal(HttpStatusCode.OK, prioridades.StatusCode);
        Assert.Equal(HttpStatusCode.OK, tipos.StatusCode);
        Assert.Equal(HttpStatusCode.OK, locais.StatusCode);

        var payloadDepartamentos = await departamentos.Content.ReadAsStringAsync();
        var payloadCategorias = await categorias.Content.ReadAsStringAsync();
        var payloadPrioridades = await prioridades.Content.ReadAsStringAsync();
        var payloadTipos = await tipos.Content.ReadAsStringAsync();
        var payloadLocais = await locais.Content.ReadAsStringAsync();

        Assert.Contains(dados.DepartamentoAtivoId.ToString(), payloadDepartamentos, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(dados.DepartamentoInativoId.ToString(), payloadDepartamentos, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(dados.CategoriaAtivaId.ToString(), payloadCategorias, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(dados.CategoriaInativaId.ToString(), payloadCategorias, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(SeedData.PrioridadeBaixaId.ToString(), payloadPrioridades, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(dados.PrioridadeInativaId.ToString(), payloadPrioridades, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(dados.TipoAtivoId.ToString(), payloadTipos, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(dados.TipoInativoId.ToString(), payloadTipos, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(dados.LocalAtivoId.ToString(), payloadLocais, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(dados.LocalInativoId.ToString(), payloadLocais, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CadastrosOperacionaisSubcategoriasAtivasSaoFiltradasPorCategoria()
    {
        var dados = await SeedCadastrosOperacionaisAsync();
        using var client = _factory.CreateClient();
        AddDevHeaders(client, "solicitante.subcat.operacional@empresa.com", "Solicitante", "Solicitante");

        var response = await client.GetAsync($"/api/cadastros/categorias/{dados.CategoriaAtivaId}/subcategorias/ativas");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadAsStringAsync();
        Assert.Contains(dados.SubcategoriaAtivaId.ToString(), payload, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(dados.SubcategoriaInativaId.ToString(), payload, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(dados.SubcategoriaOutraCategoriaId.ToString(), payload, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task HealthRetornaOk()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrWhiteSpace(payload));
    }

    [Fact]
    public async Task HealthReadyRetornaOk()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/health/ready");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task HealthLiveRetornaOk()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/health/live");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
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

    private async Task<Guid> ObterUsuarioIdPorEmail(string email)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        var usuario = await dbContext.Usuarios.FirstAsync(x => x.Email == email);
        return usuario.Id;
    }

    private async Task<(Guid Id, Guid SolicitanteId)> ObterChamadoPorCodigoAsync(string codigo, CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        var chamado = await dbContext.Chamados.FirstAsync(x => x.Codigo == codigo, cancellationToken);
        return (chamado.Id, chamado.SolicitanteId);
    }

    private async Task CriarAprovacaoNoChamadoAsync(
        Guid chamadoId,
        Guid solicitanteId,
        StatusAprovacaoChamado status,
        string justificativaDecisao,
        CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        var admin = await dbContext.Usuarios.FirstAsync(cancellationToken);

        var aprovacao = new AprovacaoChamado(
            chamadoId,
            TipoOrigemAprovacaoChamado.Manual,
            admin.Id,
            admin.Login,
            solicitanteId,
            "Fluxo portal",
            "Solicitacao portal");

        if (status == StatusAprovacaoChamado.Aprovado)
        {
            aprovacao.Aprovar(admin.Id, admin.Id, admin.Login, justificativaDecisao);
        }
        else if (status == StatusAprovacaoChamado.Reprovado)
        {
            aprovacao.Reprovar(admin.Id, admin.Id, admin.Login, justificativaDecisao);
        }
        else if (status == StatusAprovacaoChamado.Cancelado)
        {
            aprovacao.Cancelar(admin.Id, admin.Login, justificativaDecisao);
        }

        dbContext.AprovacoesChamado.Add(aprovacao);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static async Task<string?> ObterMensagemErroAsync(HttpResponseMessage response)
    {
        var payload = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(payload))
        {
            return null;
        }

        using var json = JsonDocument.Parse(payload);
        return json.RootElement.TryGetProperty("mensagem", out var mensagem)
            ? mensagem.GetString()
            : null;
    }

    private static async Task<Guid> ObterGuidDaRespostaAsync(HttpResponseMessage response, string campo)
    {
        var payload = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(payload);
        var valor = json.RootElement.GetProperty(campo).GetString();
        Assert.False(string.IsNullOrWhiteSpace(valor));
        return Guid.Parse(valor!);
    }

    private static object CriarPayloadMicrosoftValido(
        bool habilitado = true,
        string provedorPrincipal = "MicrosoftEntraId",
        bool loginLocalHabilitado = false,
        string tenantId = "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
        string clientId = "bbbbbbbb-cccc-dddd-eeee-ffffffffffff",
        string audience = "api://sgx-api",
        string issuer = "https://login.microsoftonline.com/aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee/v2.0",
        string authority = "https://login.microsoftonline.com/aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee/v2.0",
        string apiScope = "api://sgx-api/access_as_user",
        string redirectUri = "http://localhost:5173")
    {
        return new
        {
            habilitado,
            provedorPrincipal,
            loginLocalHabilitado,
            tenantId,
            clientId,
            audience,
            issuer,
            authority,
            apiScope,
            redirectUri,
            dominiosPermitidos = Array.Empty<string>(),
            criarUsuarioAutomaticamente = true,
            perfilPadraoUsuarioMicrosoft = "Solicitante"
        };
    }

    private async Task<CadastrosOperacionaisSeed> SeedCadastrosOperacionaisAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();

        var departamentoAtivo = new Departamento($"Departamento Operacional Ativo {Guid.NewGuid():N}", $"DOA{Random.Shared.Next(100, 999)}", null, "integration-test");
        var departamentoInativo = new Departamento($"Departamento Operacional Inativo {Guid.NewGuid():N}", $"DOI{Random.Shared.Next(100, 999)}", null, "integration-test");
        departamentoInativo.Desativar("integration-test");

        var categoriaAtiva = new CategoriaChamado($"Categoria Operacional Ativa {Guid.NewGuid():N}", null, departamentoAtivo.Id, "integration-test");
        var categoriaInativa = new CategoriaChamado($"Categoria Operacional Inativa {Guid.NewGuid():N}", null, departamentoAtivo.Id, "integration-test");
        categoriaInativa.Desativar("integration-test");
        var categoriaSecundaria = new CategoriaChamado($"Categoria Operacional Secundaria {Guid.NewGuid():N}", null, departamentoAtivo.Id, "integration-test");

        var subcategoriaAtiva = new SubcategoriaChamado(categoriaAtiva.Id, $"Subcategoria Operacional Ativa {Guid.NewGuid():N}", null, "integration-test");
        var subcategoriaInativa = new SubcategoriaChamado(categoriaAtiva.Id, $"Subcategoria Operacional Inativa {Guid.NewGuid():N}", null, "integration-test");
        subcategoriaInativa.Desativar("integration-test");
        var subcategoriaOutraCategoria = new SubcategoriaChamado(categoriaSecundaria.Id, $"Subcategoria Operacional Outra Categoria {Guid.NewGuid():N}", null, "integration-test");

        var prioridadeInativa = new PrioridadeChamado($"Prioridade Operacional Inativa {Guid.NewGuid():N}", PrioridadeChamadoEnum.Critica, null, 1, 4, "integration-test");
        prioridadeInativa.Desativar("integration-test");

        var tipoAtivo = new TipoSolicitacao($"Tipo Operacional Ativo {Guid.NewGuid():N}", null, "integration-test");
        var tipoInativo = new TipoSolicitacao($"Tipo Operacional Inativo {Guid.NewGuid():N}", null, "integration-test");
        tipoInativo.Desativar("integration-test");

        var localAtivo = new LocalUnidade($"Local Operacional Ativo {Guid.NewGuid():N}", null, null, "integration-test");
        var localInativo = new LocalUnidade($"Local Operacional Inativo {Guid.NewGuid():N}", null, null, "integration-test");
        localInativo.Desativar("integration-test");

        await dbContext.Departamentos.AddRangeAsync([departamentoAtivo, departamentoInativo], cancellationToken);
        await dbContext.CategoriasChamado.AddRangeAsync([categoriaAtiva, categoriaInativa, categoriaSecundaria], cancellationToken);
        await dbContext.SubcategoriasChamado.AddRangeAsync([subcategoriaAtiva, subcategoriaInativa, subcategoriaOutraCategoria], cancellationToken);
        await dbContext.PrioridadesChamado.AddAsync(prioridadeInativa, cancellationToken);
        await dbContext.TiposSolicitacao.AddRangeAsync([tipoAtivo, tipoInativo], cancellationToken);
        await dbContext.LocaisUnidade.AddRangeAsync([localAtivo, localInativo], cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CadastrosOperacionaisSeed(
            departamentoAtivo.Id,
            departamentoInativo.Id,
            categoriaAtiva.Id,
            categoriaInativa.Id,
            subcategoriaAtiva.Id,
            subcategoriaInativa.Id,
            subcategoriaOutraCategoria.Id,
            prioridadeInativa.Id,
            tipoAtivo.Id,
            tipoInativo.Id,
            localAtivo.Id,
            localInativo.Id);
    }

    private async Task SeedEventoAuditoriaApiAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();

        dbContext.EventosAuditoria.Add(new EventoAuditoria(
            DateTime.UtcNow,
            Guid.NewGuid(),
            "Admin Auditoria",
            "admin.auditoria@empresa.com",
            "admin.auditoria@empresa.com",
            "127.0.0.1",
            "integration-test",
            "Chamados",
            "Chamado",
            "SGX-INT-1",
            TipoAcaoAuditoria.AlteracaoStatus,
            "Status do chamado alterado em teste de integração.",
            "{\"statusAnterior\":\"Aberto\"}",
            "{\"statusNovo\":\"EmAtendimento\"}",
            "{\"origem\":\"api\"}",
            NivelAuditoria.Informacao,
            true,
            null,
            "corr-int-1"));

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private sealed record CadastrosOperacionaisSeed(
        Guid DepartamentoAtivoId,
        Guid DepartamentoInativoId,
        Guid CategoriaAtivaId,
        Guid CategoriaInativaId,
        Guid SubcategoriaAtivaId,
        Guid SubcategoriaInativaId,
        Guid SubcategoriaOutraCategoriaId,
        Guid PrioridadeInativaId,
        Guid TipoAtivoId,
        Guid TipoInativoId,
        Guid LocalAtivoId,
        Guid LocalInativoId);
}
