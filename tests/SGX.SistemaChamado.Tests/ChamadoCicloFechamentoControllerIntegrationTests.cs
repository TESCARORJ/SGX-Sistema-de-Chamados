using System.Net;
using System.Net.Http.Json;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.DTOs.Chamados;
using SGX.SistemaChamado.Application.DTOs.Portal;
using SGX.SistemaChamado.Application.UseCases.Chamados;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class ChamadoCicloFechamentoControllerIntegrationTests : IClassFixture<ApiIntegrationTestFactory>
{
    private const string MensagemBloqueioAprovacao =
        "Este chamado possui aprovacao pendente e nao pode avancar enquanto a aprovacao bloqueante nao for decidida.";

    private readonly ApiIntegrationTestFactory _factory;

    public ChamadoCicloFechamentoControllerIntegrationTests(ApiIntegrationTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ResolverEndpoint_DeveResolverChamadoComContratoValido()
    {
        var chamadoId = await SeedChamadoAbertoAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.resolver.{Guid.NewGuid():N}@empresa.com", "Admin Resolver", "Administrador");
        _ = await client.GetAsync("/api/me");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{chamadoId}/resolver", new ResolverChamadoRequest
        {
            Solucao = "Aplicacao de correcao definitiva.",
            ComentarioInterno = false
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<ChamadoAdminDetalheResponse>();
        Assert.NotNull(payload);
        Assert.Equal("Resolvido", payload.Status);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        var chamadoDb = await dbContext.Chamados.Include(x => x.Status).SingleAsync(x => x.Id == chamadoId);
        Assert.Equal(StatusChamadoEnum.Resolvido, chamadoDb.Status.Codigo);
        Assert.NotNull(chamadoDb.ResolvidoEm);
    }

    [Fact]
    public async Task ResolverEndpoint_DeveRetornarBadRequestQuandoSolucaoTecnicaForInvalida()
    {
        var chamadoId = await SeedChamadoAbertoAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.resolver.invalido.{Guid.NewGuid():N}@empresa.com", "Admin Resolver Invalido", "Administrador");
        _ = await client.GetAsync("/api/me");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{chamadoId}/resolver", new ResolverChamadoRequest
        {
            Solucao = "",
            ComentarioInterno = false
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var payload = await response.Content.ReadAsStringAsync();
        Assert.Contains("Solucao tecnica obrigatoria para resolucao.", payload, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ResolverEndpoint_DeveExigirAutorizacaoAdequada()
    {
        var chamadoId = await SeedChamadoAbertoAsync();

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"sol.resolver.{Guid.NewGuid():N}@empresa.com", "Solicitante Resolver", "Solicitante");
        _ = await client.GetAsync("/api/me");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{chamadoId}/resolver", new ResolverChamadoRequest
        {
            Solucao = "Tentativa sem permissao.",
            ComentarioInterno = false
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AceiteEndpoint_DeveAceitarSolucaoQuandoValido()
    {
        var email = $"sol.aceite.{Guid.NewGuid():N}@empresa.com";
        var chamadoId = await SeedChamadoResolvidoPortalAsync(email);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, email, "Solicitante Aceite", "Solicitante");
        _ = await client.GetAsync("/api/me");

        var response = await client.PostAsJsonAsync($"/api/portal/chamados/{chamadoId}/aceitar-solucao", new AceitarSolucaoChamadoRequest
        {
            ObservacaoAceite = "Pode encerrar."
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<ChamadoDetalheResponse>();
        Assert.NotNull(payload);
        Assert.Equal("Encerrado", payload.Status);
        Assert.NotNull(payload.EncerradoEm);
    }

    [Fact]
    public async Task AceiteEndpoint_DeveRetornarErroQuandoHaAprovacaoPendenteBloqueante()
    {
        var email = $"sol.aceite.bloq.{Guid.NewGuid():N}@empresa.com";
        var chamadoId = await SeedChamadoResolvidoPortalAsync(email);
        await CriarAprovacaoAsync(chamadoId, bloqueiaAvancoAtendimento: true);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, email, "Solicitante Aceite Bloqueado", "Solicitante");
        _ = await client.GetAsync("/api/me");

        var response = await client.PostAsJsonAsync($"/api/portal/chamados/{chamadoId}/aceitar-solucao", new AceitarSolucaoChamadoRequest
        {
            ObservacaoAceite = "Tentativa bloqueada."
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var payload = await response.Content.ReadAsStringAsync();
        Assert.Contains(MensagemBloqueioAprovacao, payload, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Portal_NaoDeveExporRotaAdministrativaDeEncerramento()
    {
        var email = $"sol.portal.semadmin.{Guid.NewGuid():N}@empresa.com";
        var chamadoId = await SeedChamadoResolvidoPortalAsync(email);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, email, "Solicitante Sem Rota Admin", "Solicitante");
        _ = await client.GetAsync("/api/me");

        var response = await client.PostAsJsonAsync($"/api/portal/chamados/{chamadoId}/encerrar", new
        {
            solucao = "Nao deveria existir no portal."
        });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RejeicaoEndpoint_DeveRetornarBadRequestQuandoMotivoForInvalido()
    {
        var email = $"sol.rejeicao.invalida.{Guid.NewGuid():N}@empresa.com";
        var chamadoId = await SeedChamadoResolvidoPortalAsync(email);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, email, "Solicitante Rejeicao Invalida", "Solicitante");
        _ = await client.GetAsync("/api/me");

        var response = await client.PostAsJsonAsync($"/api/portal/chamados/{chamadoId}/rejeitar-solucao", new RejeitarSolucaoChamadoRequest
        {
            MotivoRejeicao = ""
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task RejeicaoEndpoint_DevePreservarSeparacaoComReabertura()
    {
        var email = $"sol.rejeicao.{Guid.NewGuid():N}@empresa.com";
        var chamadoId = await SeedChamadoResolvidoPortalAsync(email);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, email, "Solicitante Rejeicao", "Solicitante");
        _ = await client.GetAsync("/api/me");

        var response = await client.PostAsJsonAsync($"/api/portal/chamados/{chamadoId}/rejeitar-solucao", new RejeitarSolucaoChamadoRequest
        {
            MotivoRejeicao = "A solucao nao resolveu o problema."
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<ChamadoDetalheResponse>();
        Assert.NotNull(payload);
        Assert.Equal("Em Atendimento", payload.Status);

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        var chamadoDb = await dbContext.Chamados.Include(x => x.Status).SingleAsync(x => x.Id == chamadoId);
        Assert.Equal(StatusChamadoEnum.EmAtendimento, chamadoDb.Status.Codigo);
        Assert.Null(chamadoDb.EncerradoEm);
    }

    [Fact]
    public async Task ReabrirEndpoint_DeveReabrirChamadoQuandoPermitido()
    {
        var chamadoId = await SeedChamadoEncerradoAsync(DateTime.UtcNow.AddHours(-2));

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.reabrir.{Guid.NewGuid():N}@empresa.com", "Admin Reabrir", "Administrador");
        _ = await client.GetAsync("/api/me");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{chamadoId}/reabrir", new ReabrirChamadoRequest
        {
            Mensagem = "Reabertura para ajuste adicional."
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<ChamadoAdminDetalheResponse>();
        Assert.NotNull(payload);
        Assert.Equal("Em Atendimento", payload.Status);
    }

    [Fact]
    public async Task ReabrirEndpoint_DeveRetornarBadRequestQuandoMotivoForInvalido()
    {
        var chamadoId = await SeedChamadoEncerradoAsync(DateTime.UtcNow.AddHours(-2));

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.reabrir.invalido.{Guid.NewGuid():N}@empresa.com", "Admin Reabrir Invalido", "Administrador");
        _ = await client.GetAsync("/api/me");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{chamadoId}/reabrir", new ReabrirChamadoRequest
        {
            Mensagem = ""
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var payload = await response.Content.ReadAsStringAsync();
        Assert.Contains("Mensagem obrigatoria para reabertura.", payload, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ReabrirEndpoint_DeveExigirAutorizacaoAdequada()
    {
        var chamadoId = await SeedChamadoEncerradoAsync(DateTime.UtcNow.AddHours(-2));

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"sol.reabrir.{Guid.NewGuid():N}@empresa.com", "Solicitante Reabrir", "Solicitante");
        _ = await client.GetAsync("/api/me");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{chamadoId}/reabrir", new ReabrirChamadoRequest
        {
            Mensagem = "Sem permissao."
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task ReabrirEndpoint_DevePreservarRegraDePrazoPolitica()
    {
        var chamadoId = await SeedChamadoEncerradoAsync(DateTime.UtcNow.AddDays(-10));

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.reabrir.prazo.{Guid.NewGuid():N}@empresa.com", "Admin Reabrir Prazo", "Administrador");
        _ = await client.GetAsync("/api/me");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{chamadoId}/reabrir", new ReabrirChamadoRequest
        {
            Mensagem = "Tentativa fora da politica."
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var payload = await response.Content.ReadAsStringAsync();
        Assert.Contains("prazo", payload, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task FechamentoAutomaticoManualEndpoint_DeveExecutarUseCaseQuandoAutorizado()
    {
        await SeedChamadoResolvidoParaFechamentoAutomaticoAsync(DateTime.UtcNow.AddHours(-48));

        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"admin.autoclose.{Guid.NewGuid():N}@empresa.com", "Admin Auto Close", "Administrador");
        _ = await client.GetAsync("/api/me");

        var response = await client.PostAsJsonAsync(
            "/api/admin/chamados/fechamento-automatico/prazo-aceite/executar",
            new FecharChamadosAutomaticamentePorPrazoAceiteRequest
            {
                DataReferencia = DateTime.UtcNow,
                PrazoAceiteHoras = 24,
                LimiteProcessamento = 10
            });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<FecharChamadosAutomaticamentePorPrazoAceiteResponse>();
        Assert.NotNull(payload);
        Assert.True(payload.TotalAnalisados >= 1);
        Assert.True(payload.TotalFechados >= 1);
    }

    [Fact]
    public async Task FechamentoAutomaticoManualEndpoint_DeveExigirAutorizacaoAdministrativa()
    {
        using var client = _factory.CreateClient();
        AddDevHeaders(client, $"sol.autoclose.{Guid.NewGuid():N}@empresa.com", "Solicitante Auto Close", "Solicitante");
        _ = await client.GetAsync("/api/me");

        var response = await client.PostAsJsonAsync(
            "/api/admin/chamados/fechamento-automatico/prazo-aceite/executar",
            new FecharChamadosAutomaticamentePorPrazoAceiteRequest
            {
                DataReferencia = DateTime.UtcNow,
                PrazoAceiteHoras = 24
            });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    private async Task<Guid> SeedChamadoAbertoAsync(CancellationToken cancellationToken = default)
    {
        return await SeedChamadoAsync(
            $"sol.aberto.{Guid.NewGuid():N}@empresa.com",
            StatusChamadoEnum.Aberto,
            cancellationToken: cancellationToken);
    }

    private async Task<Guid> SeedChamadoResolvidoPortalAsync(string emailSolicitante, CancellationToken cancellationToken = default)
    {
        return await SeedChamadoAsync(
            emailSolicitante,
            StatusChamadoEnum.Resolvido,
            resolvidoEm: DateTime.UtcNow.AddHours(-2),
            cancellationToken: cancellationToken);
    }

    private async Task<Guid> SeedChamadoEncerradoAsync(DateTime encerradoEm, CancellationToken cancellationToken = default)
    {
        return await SeedChamadoAsync(
            $"sol.encerrado.{Guid.NewGuid():N}@empresa.com",
            StatusChamadoEnum.Encerrado,
            resolvidoEm: encerradoEm.AddHours(-1),
            encerradoEm: encerradoEm,
            cancellationToken: cancellationToken);
    }

    private async Task<Guid> SeedChamadoResolvidoParaFechamentoAutomaticoAsync(DateTime resolvidoEm, CancellationToken cancellationToken = default)
    {
        return await SeedChamadoAsync(
            $"sol.auto.seed.{Guid.NewGuid():N}@empresa.com",
            StatusChamadoEnum.Resolvido,
            resolvidoEm: resolvidoEm,
            cancellationToken: cancellationToken);
    }

    private async Task<Guid> SeedChamadoAsync(
        string emailSolicitante,
        StatusChamadoEnum statusDestino,
        DateTime? resolvidoEm = null,
        DateTime? encerradoEm = null,
        CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();

        var solicitante = await GarantirUsuarioAsync(dbContext, emailSolicitante, "Solicitante Teste", TipoPerfil.Solicitante, cancellationToken);
        var departamento = await GarantirDepartamentoAsync(dbContext, cancellationToken);
        var categoria = await GarantirCategoriaAsync(dbContext, departamento.Id, cancellationToken);
        var prioridade = await dbContext.PrioridadesChamado.FirstAsync(cancellationToken);
        var statusAberto = await dbContext.StatusChamado.FirstAsync(x => x.Codigo == StatusChamadoEnum.Aberto, cancellationToken);
        var statusResolvido = await dbContext.StatusChamado.FirstAsync(x => x.Codigo == StatusChamadoEnum.Resolvido, cancellationToken);
        var statusEncerrado = await dbContext.StatusChamado.FirstAsync(x => x.Codigo == StatusChamadoEnum.Encerrado, cancellationToken);

        var chamado = new Chamado(
            $"SGX-CTL-{Guid.NewGuid():N}".ToUpperInvariant()[..20],
            "Chamado ciclo fechamento",
            "Descricao para testes de endpoint do ciclo de fechamento.",
            solicitante.Id,
            categoria.Id,
            prioridade.Id,
            statusAberto.Id,
            OrigemChamado.Portal,
            "integration-test",
            naturezaChamado: NaturezaChamadoEnum.Requisicao);

        if (statusDestino == StatusChamadoEnum.Resolvido || statusDestino == StatusChamadoEnum.Encerrado)
        {
            chamado.Resolver(statusResolvido.Id, "Solucao tecnica aplicada", "integration-test");
            if (resolvidoEm.HasValue)
            {
                DefinirPropriedadePrivada(chamado, nameof(Chamado.ResolvidoEm), resolvidoEm.Value);
            }
        }

        if (statusDestino == StatusChamadoEnum.Encerrado)
        {
            chamado.AceitarSolucao(statusEncerrado.Id, solicitante.Id, "Aceite anterior", emailSolicitante);
            if (encerradoEm.HasValue)
            {
                DefinirPropriedadePrivada(chamado, nameof(Chamado.EncerradoEm), encerradoEm.Value);
            }
        }

        dbContext.Chamados.Add(chamado);
        await dbContext.SaveChangesAsync(cancellationToken);
        return chamado.Id;
    }

    private static async Task<Usuario> GarantirUsuarioAsync(
        SGXSistemaChamadoDbContext dbContext,
        string email,
        string nome,
        TipoPerfil tipoPerfil,
        CancellationToken cancellationToken)
    {
        var usuario = await dbContext.Usuarios
            .FirstOrDefaultAsync(x => x.Email == email || x.Login == email, cancellationToken);

        if (usuario is null)
        {
            usuario = new Usuario(nome, email, email, "integration-test");
            dbContext.Usuarios.Add(usuario);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var perfil = await dbContext.PerfisAcesso.FirstAsync(x => x.TipoPerfil == tipoPerfil, cancellationToken);
        var possuiPerfil = await dbContext.UsuariosPerfisAcesso
            .AnyAsync(x => x.UsuarioId == usuario.Id && x.PerfilAcessoId == perfil.Id, cancellationToken);

        if (!possuiPerfil)
        {
            dbContext.UsuariosPerfisAcesso.Add(new UsuarioPerfilAcesso(usuario.Id, perfil.Id, "integration-test"));
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return usuario;
    }

    private static async Task<Departamento> GarantirDepartamentoAsync(
        SGXSistemaChamadoDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var departamento = await dbContext.Departamentos.FirstOrDefaultAsync(cancellationToken);
        if (departamento is not null)
        {
            return departamento;
        }

        departamento = new Departamento("Tecnologia", "TI", "Departamento para testes de endpoint.", "integration-test");
        dbContext.Departamentos.Add(departamento);
        await dbContext.SaveChangesAsync(cancellationToken);
        return departamento;
    }

    private static async Task<CategoriaChamado> GarantirCategoriaAsync(
        SGXSistemaChamadoDbContext dbContext,
        Guid departamentoId,
        CancellationToken cancellationToken)
    {
        var categoria = await dbContext.CategoriasChamado.FirstOrDefaultAsync(cancellationToken);
        if (categoria is not null)
        {
            return categoria;
        }

        categoria = new CategoriaChamado("Suporte", "Categoria para testes de endpoint.", departamentoId, "integration-test");
        dbContext.CategoriasChamado.Add(categoria);
        await dbContext.SaveChangesAsync(cancellationToken);
        return categoria;
    }

    private async Task CriarAprovacaoAsync(Guid chamadoId, bool bloqueiaAvancoAtendimento, CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        var admin = await dbContext.Usuarios.FirstAsync(x => x.Login.Contains("admin"), cancellationToken);
        var chamado = await dbContext.Chamados.AsNoTracking().FirstAsync(x => x.Id == chamadoId, cancellationToken);

        dbContext.AprovacoesChamado.Add(new AprovacaoChamado(
            chamadoId,
            TipoOrigemAprovacaoChamado.Manual,
            admin.Id,
            admin.Login,
            chamado.SolicitanteId,
            "Aprovacao endpoint ciclo fechamento",
            "Bloqueio de fechamento definitivo para testes.",
            "Aprovacao pendente",
            bloqueiaAvancoAtendimento: bloqueiaAvancoAtendimento));

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static void DefinirPropriedadePrivada<T>(Chamado chamado, string nomePropriedade, T valor)
    {
        var propriedade = typeof(Chamado).GetProperty(nomePropriedade, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException($"Propriedade '{nomePropriedade}' nao encontrada em Chamado.");

        propriedade.SetValue(chamado, valor);
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
