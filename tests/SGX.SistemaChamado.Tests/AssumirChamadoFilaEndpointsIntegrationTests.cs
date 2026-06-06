using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class AssumirChamadoFilaEndpointsIntegrationTests : IClassFixture<ApiIntegrationTestFactory>
{
    private readonly ApiIntegrationTestFactory _factory;

    public AssumirChamadoFilaEndpointsIntegrationTests(ApiIntegrationTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task AdministradorAssumeChamadoDaFilaQuandoMembroAtivoDoGrupo()
    {
        var email = $"admin.assumir.fila.{Guid.NewGuid():N}@empresa.com";
        var dados = await SeedChamadoNaFilaAsync(email, "Administrador", criarMembro: true);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, email, "Admin Assumir Fila", "Administrador");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{dados.ChamadoId}/assumir-fila", new AssumirChamadoFilaRequest
        {
            UsuarioId = dados.UsuarioId,
            Observacao = "Inicio do atendimento"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<ChamadoAdminDetalheResponse>();
        Assert.NotNull(payload);
        Assert.NotNull(payload.Responsavel);
        Assert.Equal(dados.UsuarioId, payload.Responsavel.Id);
        Assert.Equal(dados.GrupoTecnicoId, payload.GrupoTecnicoId);
        Assert.Equal(dados.FilaAtendimentoId, payload.FilaAtendimentoId);
    }

    [Fact]
    public async Task AtendenteAssumeChamadoDaFilaQuandoMembroAtivoDoGrupo()
    {
        var email = $"atendente.assumir.fila.{Guid.NewGuid():N}@empresa.com";
        var dados = await SeedChamadoNaFilaAsync(email, "Atendente", criarMembro: true);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, email, "Atendente Assumir Fila", "Atendente");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{dados.ChamadoId}/assumir-fila", new AssumirChamadoFilaRequest
        {
            UsuarioId = dados.UsuarioId
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<ChamadoAdminDetalheResponse>();
        Assert.NotNull(payload);
        Assert.NotNull(payload.Responsavel);
        Assert.Equal(dados.UsuarioId, payload.Responsavel.Id);
    }

    [Fact]
    public async Task SolicitanteNaoAssumeChamadoDaFila()
    {
        var email = $"solicitante.assumir.fila.{Guid.NewGuid():N}@empresa.com";
        var dados = await SeedChamadoNaFilaAsync(email, "Solicitante", criarMembro: true);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, email, "Solicitante Assumir Fila", "Solicitante");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{dados.ChamadoId}/assumir-fila", new AssumirChamadoFilaRequest
        {
            UsuarioId = dados.UsuarioId
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task RejeitaUsuarioForaDoGrupoViaUseCase()
    {
        var email = $"atendente.fora.grupo.{Guid.NewGuid():N}@empresa.com";
        var dados = await SeedChamadoNaFilaAsync(email, "Atendente", criarMembro: false);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, email, "Atendente Fora Grupo", "Atendente");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{dados.ChamadoId}/assumir-fila", new AssumirChamadoFilaRequest
        {
            UsuarioId = dados.UsuarioId
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Usuario nao e membro ativo do grupo tecnico do chamado.", await response.Content.ReadAsStringAsync(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task RejeitaVinculoInativoViaUseCase()
    {
        var email = $"atendente.vinculo.inativo.{Guid.NewGuid():N}@empresa.com";
        var dados = await SeedChamadoNaFilaAsync(email, "Atendente", criarMembro: true, membroInativo: true);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, email, "Atendente Vinculo Inativo", "Atendente");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{dados.ChamadoId}/assumir-fila", new AssumirChamadoFilaRequest
        {
            UsuarioId = dados.UsuarioId
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Usuario nao e membro ativo do grupo tecnico do chamado.", await response.Content.ReadAsStringAsync(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task RejeitaChamadoSemGrupoOuSemFila()
    {
        var emailSemGrupo = $"atendente.sem.grupo.{Guid.NewGuid():N}@empresa.com";
        var semGrupo = await SeedChamadoNaFilaAsync(emailSemGrupo, "Atendente", criarMembro: true, semGrupo: true);

        using var clientSemGrupo = _factory.CreateClient();
        AddDevHeaders(clientSemGrupo, emailSemGrupo, "Atendente Sem Grupo", "Atendente");

        var responseSemGrupo = await clientSemGrupo.PostAsJsonAsync($"/api/admin/chamados/{semGrupo.ChamadoId}/assumir-fila", new AssumirChamadoFilaRequest
        {
            UsuarioId = semGrupo.UsuarioId
        });

        var emailSemFila = $"atendente.sem.fila.{Guid.NewGuid():N}@empresa.com";
        var semFila = await SeedChamadoNaFilaAsync(emailSemFila, "Atendente", criarMembro: true, semFila: true);

        using var clientSemFila = _factory.CreateClient();
        AddDevHeaders(clientSemFila, emailSemFila, "Atendente Sem Fila", "Atendente");

        var responseSemFila = await clientSemFila.PostAsJsonAsync($"/api/admin/chamados/{semFila.ChamadoId}/assumir-fila", new AssumirChamadoFilaRequest
        {
            UsuarioId = semFila.UsuarioId
        });

        Assert.Equal(HttpStatusCode.BadRequest, responseSemGrupo.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, responseSemFila.StatusCode);
        Assert.Contains("Chamado precisa estar vinculado a um grupo tecnico", await responseSemGrupo.Content.ReadAsStringAsync(), StringComparison.Ordinal);
        Assert.Contains("Chamado precisa estar vinculado a uma fila de atendimento", await responseSemFila.Content.ReadAsStringAsync(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task RejeitaGrupoFilaInativosOuFilaDeOutroGrupo()
    {
        var emailGrupoInativo = $"atendente.grupo.inativo.{Guid.NewGuid():N}@empresa.com";
        var grupoInativo = await SeedChamadoNaFilaAsync(emailGrupoInativo, "Atendente", criarMembro: true, grupoInativo: true);

        using var clientGrupoInativo = _factory.CreateClient();
        AddDevHeaders(clientGrupoInativo, emailGrupoInativo, "Atendente Grupo Inativo", "Atendente");

        var responseGrupoInativo = await clientGrupoInativo.PostAsJsonAsync($"/api/admin/chamados/{grupoInativo.ChamadoId}/assumir-fila", new AssumirChamadoFilaRequest
        {
            UsuarioId = grupoInativo.UsuarioId
        });

        var emailFilaInativa = $"atendente.fila.inativa.{Guid.NewGuid():N}@empresa.com";
        var filaInativa = await SeedChamadoNaFilaAsync(emailFilaInativa, "Atendente", criarMembro: true, filaInativa: true);

        using var clientFilaInativa = _factory.CreateClient();
        AddDevHeaders(clientFilaInativa, emailFilaInativa, "Atendente Fila Inativa", "Atendente");

        var responseFilaInativa = await clientFilaInativa.PostAsJsonAsync($"/api/admin/chamados/{filaInativa.ChamadoId}/assumir-fila", new AssumirChamadoFilaRequest
        {
            UsuarioId = filaInativa.UsuarioId
        });

        var emailFilaOutroGrupo = $"atendente.fila.outro.grupo.{Guid.NewGuid():N}@empresa.com";
        var filaOutroGrupo = await SeedChamadoNaFilaAsync(emailFilaOutroGrupo, "Atendente", criarMembro: true, filaOutroGrupo: true);

        using var clientFilaOutroGrupo = _factory.CreateClient();
        AddDevHeaders(clientFilaOutroGrupo, emailFilaOutroGrupo, "Atendente Fila Outro Grupo", "Atendente");

        var responseFilaOutroGrupo = await clientFilaOutroGrupo.PostAsJsonAsync($"/api/admin/chamados/{filaOutroGrupo.ChamadoId}/assumir-fila", new AssumirChamadoFilaRequest
        {
            UsuarioId = filaOutroGrupo.UsuarioId
        });

        Assert.Equal(HttpStatusCode.BadRequest, responseGrupoInativo.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, responseFilaInativa.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, responseFilaOutroGrupo.StatusCode);
        Assert.Contains("Grupo tecnico do chamado nao encontrado ou inativo.", await responseGrupoInativo.Content.ReadAsStringAsync(), StringComparison.Ordinal);
        Assert.Contains("Fila de atendimento do chamado nao encontrada ou inativa.", await responseFilaInativa.Content.ReadAsStringAsync(), StringComparison.Ordinal);
        Assert.Contains("Fila de atendimento do chamado nao pertence ao grupo tecnico do chamado.", await responseFilaOutroGrupo.Content.ReadAsStringAsync(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task RejeitaChamadoJaComResponsavelViaUseCase()
    {
        var email = $"atendente.chamado.responsavel.{Guid.NewGuid():N}@empresa.com";
        var dados = await SeedChamadoNaFilaAsync(email, "Atendente", criarMembro: true, comResponsavel: true);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, email, "Atendente Com Responsavel", "Atendente");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{dados.ChamadoId}/assumir-fila", new AssumirChamadoFilaRequest
        {
            UsuarioId = dados.UsuarioId
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Chamado da fila ja possui responsavel individual.", await response.Content.ReadAsStringAsync(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task PreencheResponsavelEPreservaGrupoEFila()
    {
        var email = $"atendente.preservar.fila.{Guid.NewGuid():N}@empresa.com";
        var dados = await SeedChamadoNaFilaAsync(email, "Atendente", criarMembro: true);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, email, "Atendente Preservar Fila", "Atendente");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{dados.ChamadoId}/assumir-fila", new AssumirChamadoFilaRequest
        {
            UsuarioId = dados.UsuarioId
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        var chamado = await context.Chamados.AsNoTracking().SingleAsync(x => x.Id == dados.ChamadoId);
        Assert.Equal(dados.UsuarioId, chamado.ResponsavelId);
        Assert.Equal(dados.GrupoTecnicoId, chamado.GrupoTecnicoId);
        Assert.Equal(dados.FilaAtendimentoId, chamado.FilaAtendimentoId);
    }

    [Fact]
    public async Task RegistraHistoricoChamadoAssumidoDaFila()
    {
        var email = $"atendente.historico.fila.{Guid.NewGuid():N}@empresa.com";
        var dados = await SeedChamadoNaFilaAsync(email, "Atendente", criarMembro: true);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, email, "Atendente Historico Fila", "Atendente");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{dados.ChamadoId}/assumir-fila", new AssumirChamadoFilaRequest
        {
            UsuarioId = dados.UsuarioId,
            Observacao = "Peguei da fila"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        Assert.Contains(await context.HistoricosChamado.AsNoTracking().Where(x => x.ChamadoId == dados.ChamadoId).ToListAsync(), x =>
            x.Tipo == TipoHistoricoChamado.ChamadoAssumidoDaFila &&
            x.Descricao.Contains("Peguei da fila", StringComparison.Ordinal));
    }

    [Fact]
    public async Task RejeitaAssumirEmNomeDeOutroUsuarioViaUseCase()
    {
        var email = $"atendente.outro.usuario.{Guid.NewGuid():N}@empresa.com";
        var dados = await SeedChamadoNaFilaAsync(email, "Atendente", criarMembro: true);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, email, "Atendente Outro Usuario", "Atendente");

        var response = await client.PostAsJsonAsync($"/api/admin/chamados/{dados.ChamadoId}/assumir-fila", new AssumirChamadoFilaRequest
        {
            UsuarioId = dados.OutroUsuarioId
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Chamado da fila so pode ser assumido pelo proprio usuario autenticado.", await response.Content.ReadAsStringAsync(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task NaoExpoeEndpointDeAtribuicaoTecnicoNestaEtapa()
    {
        var email = $"admin.sem.transferencia.{Guid.NewGuid():N}@empresa.com";
        var dados = await SeedChamadoNaFilaAsync(email, "Administrador", criarMembro: true);

        using var client = _factory.CreateClient();
        AddDevHeaders(client, email, "Admin Sem Transferencia", "Administrador");

        var atribuirTecnico = await client.PostAsJsonAsync($"/api/admin/chamados/{dados.ChamadoId}/atribuir-tecnico", new { usuarioId = dados.OutroUsuarioId });

        Assert.Equal(HttpStatusCode.NotFound, atribuirTecnico.StatusCode);
    }

    private async Task<DadosAssumirFila> SeedChamadoNaFilaAsync(
        string emailUsuario,
        string perfilUsuario,
        bool criarMembro,
        bool comResponsavel = false,
        bool membroInativo = false,
        bool semGrupo = false,
        bool semFila = false,
        bool grupoInativo = false,
        bool filaInativa = false,
        bool filaOutroGrupo = false,
        CancellationToken cancellationToken = default)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();

        var usuario = await CriarUsuarioAsync(dbContext, $"Usuario Assumir {perfilUsuario}", emailUsuario, ObterTipoPerfil(perfilUsuario), cancellationToken);
        var outroUsuario = await CriarUsuarioAsync(dbContext, $"Outro Usuario {perfilUsuario}", $"outro.assumir.{Guid.NewGuid():N}@empresa.com", TipoPerfil.Atendente, cancellationToken);
        var solicitante = await CriarUsuarioAsync(dbContext, $"Solicitante Assumir {Guid.NewGuid():N}", $"sol.assumir.{Guid.NewGuid():N}@empresa.com", TipoPerfil.Solicitante, cancellationToken);

        var categoria = await dbContext.CategoriasChamado.FirstOrDefaultAsync(cancellationToken)
            ?? new CategoriaChamado("Categoria Assumir Fila", "Categoria para assumir fila", null, "integration-test");
        if (dbContext.Entry(categoria).State == EntityState.Detached)
        {
            dbContext.CategoriasChamado.Add(categoria);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var prioridade = await dbContext.PrioridadesChamado.FirstAsync(cancellationToken);
        var status = await dbContext.StatusChamado.FirstAsync(x => x.Codigo == StatusChamadoEnum.Aberto, cancellationToken);
        var grupo = new GrupoTecnico($"Grupo Assumir Fila {Guid.NewGuid():N}", "Grupo para assumir fila", "integration-test");
        dbContext.GruposTecnicos.Add(grupo);
        await dbContext.SaveChangesAsync(cancellationToken);

        var fila = new FilaAtendimento(grupo.Id, $"Fila Assumir {Guid.NewGuid():N}", "Fila para assumir", "integration-test");
        dbContext.FilasAtendimento.Add(fila);
        await dbContext.SaveChangesAsync(cancellationToken);

        if (criarMembro)
        {
            var membro = new MembroGrupoTecnico(grupo.Id, usuario.Id, "integration-test");
            if (membroInativo)
            {
                membro.Inativar("integration-test");
            }

            dbContext.MembrosGruposTecnicos.Add(membro);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        if (grupoInativo)
        {
            grupo.Inativar("integration-test");
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        if (filaInativa)
        {
            fila.Inativar("integration-test");
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        FilaAtendimento? filaOutroGrupoEntidade = null;
        if (filaOutroGrupo)
        {
            var outroGrupo = new GrupoTecnico($"Grupo Outra Fila {Guid.NewGuid():N}", "Grupo de outra fila", "integration-test");
            dbContext.GruposTecnicos.Add(outroGrupo);
            await dbContext.SaveChangesAsync(cancellationToken);

            filaOutroGrupoEntidade = new FilaAtendimento(outroGrupo.Id, $"Fila Outro Grupo {Guid.NewGuid():N}", "Fila de outro grupo", "integration-test");
            dbContext.FilasAtendimento.Add(filaOutroGrupoEntidade);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var chamado = new Chamado(
            $"SGX-AF-{Guid.NewGuid():N}".ToUpperInvariant()[..20],
            "Chamado para assumir fila",
            "Descricao do chamado para assumir fila",
            solicitante.Id,
            categoria.Id,
            prioridade.Id,
            status.Id,
            OrigemChamado.Portal,
            "integration-test");

        if (!semGrupo)
        {
            chamado.DefinirGrupoTecnico(grupo.Id, "integration-test");
        }

        if (!semFila)
        {
            chamado.DefinirFilaAtendimento((filaOutroGrupoEntidade ?? fila).Id, "integration-test");
        }

        if (comResponsavel)
        {
            chamado.AtribuirResponsavel(outroUsuario.Id, "integration-test");
        }

        dbContext.Chamados.Add(chamado);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new DadosAssumirFila(chamado.Id, usuario.Id, outroUsuario.Id, grupo.Id, fila.Id);
    }

    private static async Task<Usuario> CriarUsuarioAsync(
        SGXSistemaChamadoDbContext dbContext,
        string nome,
        string email,
        TipoPerfil tipoPerfil,
        CancellationToken cancellationToken)
    {
        var emailNormalizado = email.Trim().ToLowerInvariant();
        var usuario = new Usuario(nome, emailNormalizado, emailNormalizado, "integration-test");
        dbContext.Usuarios.Add(usuario);
        await dbContext.SaveChangesAsync(cancellationToken);

        var perfil = await dbContext.PerfisAcesso.FirstAsync(x => x.TipoPerfil == tipoPerfil, cancellationToken);
        dbContext.UsuariosPerfisAcesso.Add(new UsuarioPerfilAcesso(usuario.Id, perfil.Id, "integration-test"));
        await dbContext.SaveChangesAsync(cancellationToken);
        return usuario;
    }

    private static TipoPerfil ObterTipoPerfil(string perfil)
        => string.Equals(perfil, "Administrador", StringComparison.OrdinalIgnoreCase)
            ? TipoPerfil.Administrador
            : string.Equals(perfil, "Atendente", StringComparison.OrdinalIgnoreCase)
                ? TipoPerfil.Atendente
                : TipoPerfil.Solicitante;

    private static void AddDevHeaders(HttpClient client, string email, string nome, string role)
    {
        client.DefaultRequestHeaders.Remove("X-Dev-User-Email");
        client.DefaultRequestHeaders.Remove("X-Dev-User-Name");
        client.DefaultRequestHeaders.Remove("X-Dev-User-Role");
        client.DefaultRequestHeaders.Add("X-Dev-User-Email", email);
        client.DefaultRequestHeaders.Add("X-Dev-User-Name", nome);
        client.DefaultRequestHeaders.Add("X-Dev-User-Role", role);
    }

    private sealed record DadosAssumirFila(
        Guid ChamadoId,
        Guid UsuarioId,
        Guid OutroUsuarioId,
        Guid GrupoTecnicoId,
        Guid FilaAtendimentoId);
}
