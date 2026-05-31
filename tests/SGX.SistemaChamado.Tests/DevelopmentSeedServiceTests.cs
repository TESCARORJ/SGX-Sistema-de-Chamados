using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text;
using SGX.SistemaChamado.Api.Options;
using SGX.SistemaChamado.Api.Services;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

[Collection("EnvironmentVariables")]
public sealed class DevelopmentSeedServiceTests
{
    private const string UsuarioTecnicoTeste = "teste.seed";

    private static readonly Dictionary<string, TipoPerfil> UsuariosOficiaisPorEmail = new(StringComparer.OrdinalIgnoreCase)
    {
        ["admin@sgxdigital.com"] = TipoPerfil.Administrador,
        ["admin2@sgxdigital.com"] = TipoPerfil.Administrador,
        ["atendente.demo@sgxdigital.com"] = TipoPerfil.Atendente,
        ["atendente2.demo@sgxdigital.com"] = TipoPerfil.Atendente,
        ["solicitante.demo@sgxdigital.com"] = TipoPerfil.Solicitante,
        ["solicitante2.demo@sgxdigital.com"] = TipoPerfil.Solicitante,

        ["solicitante.hml@sgx.local"] = TipoPerfil.Solicitante,
        ["atendente.n1.hml@sgx.local"] = TipoPerfil.Atendente,
        ["tecnico.n2.hml@sgx.local"] = TipoPerfil.Atendente,
        ["coordenador.service.desk.hml@sgx.local"] = TipoPerfil.Atendente,
        ["gestor.ti.hml@sgx.local"] = TipoPerfil.Atendente,
        ["administrador.hml@sgx.local"] = TipoPerfil.Administrador,
        ["auditor.governanca.hml@sgx.local"] = TipoPerfil.Atendente
    };

    [Fact]
    public async Task SeedCriaApenasDoisUsuariosAtivosPorPerfilEmBaseNova()
    {
        await using var dbContext = CriarContexto();
        var service = CriarService(dbContext);

        await service.SeedAsync();

        var usuariosAtivos = await dbContext.Usuarios
            .AsNoTracking()
            .Include(x => x.UsuarioPerfis)
            .ThenInclude(x => x.PerfilAcesso)
            .Where(x => x.Ativo && x.Situacao == SituacaoUsuario.Ativo)
            .ToListAsync();

        Assert.Equal(13, usuariosAtivos.Count);
        Assert.All(usuariosAtivos, usuario => Assert.Contains(usuario.Email, UsuariosOficiaisPorEmail.Keys, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(3, usuariosAtivos.Count(x => x.UsuarioPerfis.Any(p => p.PerfilAcesso.TipoPerfil == TipoPerfil.Administrador)));
        Assert.Equal(7, usuariosAtivos.Count(x => x.UsuarioPerfis.Any(p => p.PerfilAcesso.TipoPerfil == TipoPerfil.Atendente)));
        Assert.Equal(3, usuariosAtivos.Count(x => x.UsuarioPerfis.Any(p => p.PerfilAcesso.TipoPerfil == TipoPerfil.Solicitante)));

        foreach (var usuario in usuariosAtivos)
        {
            Assert.True(UsuariosOficiaisPorEmail.TryGetValue(usuario.Email, out var perfilEsperado));
            Assert.Single(usuario.UsuarioPerfis);
            Assert.Equal(perfilEsperado, usuario.UsuarioPerfis.Single().PerfilAcesso.TipoPerfil);
        }
    }

    [Fact]
    public async Task SeedNaoDuplicaUsuariosNemRecriaDemoInativado()
    {
        await using var dbContext = CriarContexto();
        var perfis = await CarregarPerfisAsync(dbContext);
        await CriarUsuarioAsync(
            dbContext,
            "Solicitante Local",
            "solicitante.local@sgx.local",
            TipoPerfil.Solicitante,
            perfis);

        var service = CriarService(dbContext);
        await service.SeedAsync();
        await service.SeedAsync();

        var oficiais = await dbContext.Usuarios
            .AsNoTracking()
            .Where(x => UsuariosOficiaisPorEmail.Keys.Contains(x.Email))
            .ToListAsync();
        Assert.Equal(13, oficiais.Count);
        Assert.Equal(13, oficiais.Select(x => x.Email).Distinct(StringComparer.OrdinalIgnoreCase).Count());

        var demoAntigo = await dbContext.Usuarios
            .AsNoTracking()
            .SingleAsync(x => x.Email == "solicitante.local@sgx.local");
        Assert.False(demoAntigo.Ativo);
        Assert.Equal(SituacaoUsuario.Inativo, demoAntigo.Situacao);
    }

    [Fact]
    public async Task SeedInativaUsuariosDemonstrativosAntigos()
    {
        await using var dbContext = CriarContexto();
        var perfis = await CarregarPerfisAsync(dbContext);

        var emailsAntigos = new[]
        {
            "administrador.admin@sgx.local",
            "admin.local@sgx.local",
            "atendente.admin@sgx.local",
            "atendente.local@sgx.local",
            "atendente.sla.local@sgx.local",
            "solicitante.a.local@sgx.local",
            "solicitante.a@sgx.local",
            "solicitante.admin@sgx.local",
            "solicitante.b@sgx.local",
            "solicitante.b.local@sgx.local",
            "solicitante.local@sgx.local",
            "solicitante.portal@sgx.local",
            "solicitante.sla.local@sgx.local",
            "usuario.homol.demo@sgx.local"
        };

        foreach (var email in emailsAntigos)
        {
            var tipoPerfil = email.Contains("atendente", StringComparison.OrdinalIgnoreCase)
                ? TipoPerfil.Atendente
                : email.Contains("solicitante", StringComparison.OrdinalIgnoreCase)
                    ? TipoPerfil.Solicitante
                    : TipoPerfil.Administrador;

            await CriarUsuarioAsync(dbContext, $"Usuário demo {email}", email, tipoPerfil, perfis);
        }

        await CriarUsuarioAsync(
            dbContext,
            "Usuário Homol Especial",
            "qualquer.homologacao@empresa.com",
            TipoPerfil.Solicitante,
            perfis);

        var service = CriarService(dbContext);
        await service.SeedAsync();

        var antigos = await dbContext.Usuarios
            .AsNoTracking()
            .Where(x => (emailsAntigos.Contains(x.Email) || x.Nome.Contains("Homol")) && !UsuariosOficiaisPorEmail.Keys.Contains(x.Email))
            .ToListAsync();

        Assert.NotEmpty(antigos);
        Assert.All(antigos, usuario =>
        {
            Assert.False(usuario.Ativo);
            Assert.Equal(SituacaoUsuario.Inativo, usuario.Situacao);
        });
    }

    [Fact]
    public async Task SeedInativaUsuarioDemonstrativoGenericoDoDominioLegado()
    {
        await using var dbContext = CriarContexto();
        var perfis = await CarregarPerfisAsync(dbContext);

        _ = await CriarUsuarioAsync(
            dbContext,
            "Atendente Legado Generico",
            "at1@sgx.local",
            TipoPerfil.Atendente,
            perfis);

        var service = CriarService(dbContext);
        await service.SeedAsync();

        var usuario = await dbContext.Usuarios
            .AsNoTracking()
            .SingleAsync(x => x.Email == "at1@sgx.local");

        Assert.False(usuario.Ativo);
        Assert.Equal(SituacaoUsuario.Inativo, usuario.Situacao);
    }

    [Fact]
    public async Task SeedNaoInativaUsuarioRealForaDosPadroesDemonstrativos()
    {
        await using var dbContext = CriarContexto();
        var perfis = await CarregarPerfisAsync(dbContext);
        var usuarioReal = await CriarUsuarioAsync(
            dbContext,
            "Maria da Silva",
            "maria.silva@empresa.com",
            TipoPerfil.Solicitante,
            perfis);

        var service = CriarService(dbContext);
        await service.SeedAsync();

        var usuarioAtualizado = await dbContext.Usuarios.AsNoTracking().SingleAsync(x => x.Id == usuarioReal.Id);
        Assert.True(usuarioAtualizado.Ativo);
        Assert.Equal(SituacaoUsuario.Ativo, usuarioAtualizado.Situacao);
    }

    [Fact]
    public async Task SeedNaoInativaAdministradorInicialReal()
    {
        const string emailAdminInicial = "admin.real@sgxdigital.com";
        var emailOriginal = Environment.GetEnvironmentVariable("SGX_ADMIN_INICIAL_EMAIL");

        try
        {
            Environment.SetEnvironmentVariable("SGX_ADMIN_INICIAL_EMAIL", emailAdminInicial);

            await using var dbContext = CriarContexto();
            var perfis = await CarregarPerfisAsync(dbContext);
            var adminInicial = await CriarUsuarioAsync(
                dbContext,
                "Administrador Inicial",
                emailAdminInicial,
                TipoPerfil.Administrador,
                perfis);

            var service = CriarService(dbContext);
            await service.SeedAsync();

            var usuarioAtualizado = await dbContext.Usuarios.AsNoTracking().SingleAsync(x => x.Id == adminInicial.Id);
            Assert.True(usuarioAtualizado.Ativo);
            Assert.Equal(SituacaoUsuario.Ativo, usuarioAtualizado.Situacao);
        }
        finally
        {
            Environment.SetEnvironmentVariable("SGX_ADMIN_INICIAL_EMAIL", emailOriginal);
        }
    }

    [Fact]
    public async Task SeedGaranteCadastrosIniciaisSemDuplicidade()
    {
        await using var dbContext = CriarContexto();
        var service = CriarService(dbContext);

        await service.SeedAsync();
        await service.SeedAsync();

        var departamentos = await dbContext.Departamentos.AsNoTracking().ToListAsync();
        var categorias = await dbContext.CategoriasChamado.AsNoTracking().ToListAsync();
        var subcategorias = await dbContext.SubcategoriasChamado
            .AsNoTracking()
            .Include(x => x.CategoriaChamado)
            .ToListAsync();
        var prioridades = await dbContext.PrioridadesChamado.AsNoTracking().ToListAsync();
        var tipos = await dbContext.TiposSolicitacao.AsNoTracking().ToListAsync();
        var locais = await dbContext.LocaisUnidade.AsNoTracking().ToListAsync();

        var departamentosEsperados = new[]
        {
            "Tecnologia da Informacao",
            "Recursos Humanos",
            "Financeiro",
            "Juridico",
            "Atendimento",
            "Infraestrutura"
        };

        var categoriasEsperadas = new[]
        {
            "Hardware",
            "Software",
            "Rede",
            "Sistema",
            "Acesso",
            "E-mail",
            "Impressora",
            "Telefonia",
            "Solicitacao Administrativa",
            "Suporte Tecnico"
        };

        var subcategoriasEsperadas = new Dictionary<string, string[]>
        {
            ["Hardware"] = ["Computador", "Notebook", "Monitor", "Teclado / Mouse"],
            ["Software"] = ["Instalacao", "Erro no sistema", "Atualizacao"],
            ["Rede"] = ["Internet", "Wi-Fi", "Cabeamento"],
            ["Sistema"] = ["Erro de acesso", "Erro de operacao", "Lentidao"],
            ["Acesso"] = ["Criacao de usuario", "Reset de senha", "Permissao de acesso"],
            ["E-mail"] = ["Criacao de conta", "Problema de envio/recebimento", "Configuracao"],
            ["Impressora"] = ["Instalacao", "Falha de impressao", "Toner"],
            ["Telefonia"] = ["Ramal", "Aparelho", "Ligacao externa"],
            ["Solicitacao Administrativa"] = ["Apoio operacional", "Solicitacao interna"]
        };

        var tiposEsperados = new[]
        {
            "Incidente",
            "Solicitacao de Servico",
            "Duvida",
            "Melhoria",
            "Problema Recorrente"
        };

        var locaisEsperados = new[]
        {
            "Sede",
            "Filial",
            "Inspetoria",
            "Datacenter",
            "Almoxarifado",
            "Atendimento Externo"
        };

        AssertNomesSemDuplicidadeComEsperados(departamentos.Select(x => x.Nome), departamentosEsperados);
        AssertNomesSemDuplicidadeComEsperados(categorias.Select(x => x.Nome), categoriasEsperadas);
        AssertSubcategoriasEsperadas(subcategorias, subcategoriasEsperadas);
        AssertPrioridadesEsperadas(prioridades);
        AssertNomesSemDuplicidadeComEsperados(tipos.Select(x => x.Nome), tiposEsperados);
        AssertNomesSemDuplicidadeComEsperados(locais.Select(x => x.Nome), locaisEsperados);
    }

    [Fact]
    public async Task SeedNaoDuplicaQuandoJaExisteCadastroComVariacaoDeAcentuacao()
    {
        await using var dbContext = CriarContexto();
        await dbContext.Departamentos.AddAsync(new Departamento("Jurídico", "JUR", null, UsuarioTecnicoTeste));
        await dbContext.CategoriasChamado.AddAsync(new CategoriaChamado("Solicitação Administrativa", null, null, UsuarioTecnicoTeste));
        await dbContext.TiposSolicitacao.AddAsync(new TipoSolicitacao("Dúvida", null, UsuarioTecnicoTeste));
        await dbContext.LocaisUnidade.AddAsync(new LocalUnidade("Datacenter", null, null, UsuarioTecnicoTeste));
        await dbContext.SaveChangesAsync();

        var service = CriarService(dbContext);
        await service.SeedAsync();

        var departamentos = await dbContext.Departamentos.AsNoTracking().ToListAsync();
        var categorias = await dbContext.CategoriasChamado.AsNoTracking().ToListAsync();
        var tipos = await dbContext.TiposSolicitacao.AsNoTracking().ToListAsync();
        var locais = await dbContext.LocaisUnidade.AsNoTracking().ToListAsync();

        _ = Assert.Single(departamentos, x => NormalizarChaveTexto(x.Nome) == NormalizarChaveTexto("Juridico"));
        _ = Assert.Single(categorias, x => NormalizarChaveTexto(x.Nome) == NormalizarChaveTexto("Solicitacao Administrativa"));
        _ = Assert.Single(tipos, x => NormalizarChaveTexto(x.Nome) == NormalizarChaveTexto("Duvida"));
        _ = Assert.Single(locais, x => NormalizarChaveTexto(x.Nome) == NormalizarChaveTexto("Datacenter"));
    }

    [Fact]
    public async Task SeedGaranteUsuariosDeHomologacaoEMassaDeChamadosSemDuplicidade()
    {
        await using var dbContext = CriarContexto();
        var service = CriarService(dbContext);

        await service.SeedAsync();

        // 1. Validar usuários de homologação
        var solicitante = await dbContext.Usuarios.Include(x => x.UsuarioPerfis).ThenInclude(x => x.PerfilAcesso)
            .SingleOrDefaultAsync(x => x.Email == "solicitante.hml@sgx.local");
        Assert.NotNull(solicitante);
        Assert.True(solicitante.Ativo);
        Assert.Equal("Solicitante", solicitante.UsuarioPerfis.Single().PerfilAcesso.Nome);

        var atendenteN1 = await dbContext.Usuarios.Include(x => x.UsuarioPerfis).ThenInclude(x => x.PerfilAcesso)
            .SingleOrDefaultAsync(x => x.Email == "atendente.n1.hml@sgx.local");
        Assert.NotNull(atendenteN1);
        Assert.True(atendenteN1.Ativo);
        Assert.Equal("Atendente N1", atendenteN1.UsuarioPerfis.Single().PerfilAcesso.Nome);

        var tecnicoN2 = await dbContext.Usuarios.Include(x => x.UsuarioPerfis).ThenInclude(x => x.PerfilAcesso)
            .SingleOrDefaultAsync(x => x.Email == "tecnico.n2.hml@sgx.local");
        Assert.NotNull(tecnicoN2);
        Assert.True(tecnicoN2.Ativo);
        Assert.Equal("Técnico N2", tecnicoN2.UsuarioPerfis.Single().PerfilAcesso.Nome);

        var coordenador = await dbContext.Usuarios.Include(x => x.UsuarioPerfis).ThenInclude(x => x.PerfilAcesso)
            .SingleOrDefaultAsync(x => x.Email == "coordenador.service.desk.hml@sgx.local");
        Assert.NotNull(coordenador);
        Assert.True(coordenador.Ativo);
        Assert.Equal("Coordenador Service Desk", coordenador.UsuarioPerfis.Single().PerfilAcesso.Nome);

        var gestor = await dbContext.Usuarios.Include(x => x.UsuarioPerfis).ThenInclude(x => x.PerfilAcesso)
            .SingleOrDefaultAsync(x => x.Email == "gestor.ti.hml@sgx.local");
        Assert.NotNull(gestor);
        Assert.True(gestor.Ativo);
        Assert.Equal("Gestor TI", gestor.UsuarioPerfis.Single().PerfilAcesso.Nome);

        var admin = await dbContext.Usuarios.Include(x => x.UsuarioPerfis).ThenInclude(x => x.PerfilAcesso)
            .SingleOrDefaultAsync(x => x.Email == "administrador.hml@sgx.local");
        Assert.NotNull(admin);
        Assert.True(admin.Ativo);
        Assert.Equal("Administrador", admin.UsuarioPerfis.Single().PerfilAcesso.Nome);

        var auditor = await dbContext.Usuarios.Include(x => x.UsuarioPerfis).ThenInclude(x => x.PerfilAcesso)
            .SingleOrDefaultAsync(x => x.Email == "auditor.governanca.hml@sgx.local");
        Assert.NotNull(auditor);
        Assert.True(auditor.Ativo);
        Assert.Equal("Auditor Governança", auditor.UsuarioPerfis.Single().PerfilAcesso.Nome);

        // 2. Validar chamados de homologação
        var chamados = await dbContext.Chamados.ToListAsync();
        Assert.Equal(6, chamados.Count);

        var inc = Assert.Single(chamados, x => x.NaturezaChamado == NaturezaChamadoEnum.Incidente);
        Assert.Equal("HML-INC-001", inc.Codigo);
        Assert.Equal(ImpactoChamadoEnum.Alto, inc.ImpactoChamado);
        Assert.Equal(UrgenciaChamadoEnum.Alta, inc.UrgenciaChamado);
        Assert.Equal(tecnicoN2.Id, inc.ResponsavelId);

        var req = Assert.Single(chamados, x => x.NaturezaChamado == NaturezaChamadoEnum.Requisicao);
        Assert.Equal("HML-REQ-002", req.Codigo);
        Assert.Equal(ImpactoChamadoEnum.Baixo, req.ImpactoChamado);
        Assert.Equal(UrgenciaChamadoEnum.Media, req.UrgenciaChamado);

        var mud = Assert.Single(chamados, x => x.NaturezaChamado == NaturezaChamadoEnum.Mudanca);
        Assert.Equal("HML-MUD-003", mud.Codigo);
        Assert.Equal(ImpactoChamadoEnum.Alto, mud.ImpactoChamado);
        Assert.Equal(UrgenciaChamadoEnum.Media, mud.UrgenciaChamado);

        var prob = Assert.Single(chamados, x => x.NaturezaChamado == NaturezaChamadoEnum.Problema);
        Assert.Equal("HML-PROB-004", prob.Codigo);
        Assert.Equal(ImpactoChamadoEnum.Medio, prob.ImpactoChamado);
        Assert.Equal(UrgenciaChamadoEnum.Media, prob.UrgenciaChamado);

        var alr = Assert.Single(chamados, x => x.NaturezaChamado == NaturezaChamadoEnum.EventoAlerta);
        Assert.Equal("HML-ALR-005", alr.Codigo);
        Assert.Equal(ImpactoChamadoEnum.Medio, alr.ImpactoChamado);
        Assert.Equal(UrgenciaChamadoEnum.Alta, alr.UrgenciaChamado);

        var tar = Assert.Single(chamados, x => x.NaturezaChamado == NaturezaChamadoEnum.TarefaOperacional);
        Assert.Equal("HML-TAR-006", tar.Codigo);
        Assert.Equal(ImpactoChamadoEnum.Baixo, tar.ImpactoChamado);
        Assert.Equal(UrgenciaChamadoEnum.Baixa, tar.UrgenciaChamado);

        // 3. Validar idempotência rodando de novo
        await service.SeedAsync();
        var chamadosNovos = await dbContext.Chamados.ToListAsync();
        Assert.Equal(6, chamadosNovos.Count);
    }

    private static async Task<Dictionary<TipoPerfil, PerfilAcesso>> CarregarPerfisAsync(SGXSistemaChamadoDbContext dbContext)
    {
        var perfis = await dbContext.PerfisAcesso
            .AsNoTracking()
            .Where(x => x.Ativo && (x.Nome == "Administrador" || x.Nome == "Atendente" || x.Nome == "Solicitante"))
            .ToListAsync();

        return perfis.ToDictionary(x => x.TipoPerfil, x => x);
    }

    private static async Task<Usuario> CriarUsuarioAsync(
        SGXSistemaChamadoDbContext dbContext,
        string nome,
        string email,
        TipoPerfil tipoPerfil,
        IReadOnlyDictionary<TipoPerfil, PerfilAcesso> perfis)
    {
        var usuario = new Usuario(nome, email, email, UsuarioTecnicoTeste);
        await dbContext.Usuarios.AddAsync(usuario);
        await dbContext.SaveChangesAsync();

        var vinculoPerfil = new UsuarioPerfilAcesso(usuario.Id, perfis[tipoPerfil].Id, UsuarioTecnicoTeste);
        await dbContext.UsuariosPerfisAcesso.AddAsync(vinculoPerfil);
        await dbContext.SaveChangesAsync();

        return usuario;
    }

    private static SGXSistemaChamadoDbContext CriarContexto()
    {
        var options = new DbContextOptionsBuilder<SGXSistemaChamadoDbContext>()
            .UseInMemoryDatabase($"seed-tests-{Guid.NewGuid():N}")
            .Options;

        var context = new SGXSistemaChamadoDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    private static DevelopmentSeedService CriarService(SGXSistemaChamadoDbContext dbContext)
    {
        var authOptions = Options.Create(new AuthOptions
        {
            LoginLocalHabilitado = false
        });

        return new DevelopmentSeedService(
            dbContext,
            new FakeEnvironment(),
            authOptions,
            new PasswordHasher<Usuario>(),
            NullLogger<DevelopmentSeedService>.Instance);
    }

    private static void AssertNomesSemDuplicidadeComEsperados(IEnumerable<string> nomesAtuais, IEnumerable<string> nomesEsperados)
    {
        var nomesNormalizadosAtuais = nomesAtuais
            .Select(NormalizarChaveTexto)
            .ToArray();

        var duplicados = nomesNormalizadosAtuais
            .GroupBy(x => x)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToArray();

        Assert.Empty(duplicados);

        var conjuntoAtual = nomesNormalizadosAtuais.ToHashSet(StringComparer.Ordinal);
        foreach (var esperado in nomesEsperados)
        {
            Assert.Contains(NormalizarChaveTexto(esperado), conjuntoAtual);
        }
    }

    private static void AssertSubcategoriasEsperadas(
        IEnumerable<SubcategoriaChamado> subcategorias,
        IReadOnlyDictionary<string, string[]> esperadoPorCategoria)
    {
        var agrupado = subcategorias
            .Where(x => x.CategoriaChamado is not null)
            .GroupBy(x => NormalizarChaveTexto(x.CategoriaChamado.Nome))
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => NormalizarChaveTexto(x.Nome)).ToArray());

        foreach (var item in esperadoPorCategoria)
        {
            var chaveCategoria = NormalizarChaveTexto(item.Key);
            Assert.True(agrupado.TryGetValue(chaveCategoria, out var subcategoriasCategoria));
            Assert.NotNull(subcategoriasCategoria);

            var subcategoriasDuplicadas = subcategoriasCategoria!
                .GroupBy(x => x)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToArray();

            Assert.Empty(subcategoriasDuplicadas);

            var conjuntoSubcategorias = subcategoriasCategoria.ToHashSet(StringComparer.Ordinal);
            foreach (var nomeEsperado in item.Value)
            {
                Assert.Contains(NormalizarChaveTexto(nomeEsperado), conjuntoSubcategorias);
            }
        }
    }

    private static void AssertPrioridadesEsperadas(IEnumerable<PrioridadeChamado> prioridades)
    {
        static string Cor(string valor) => valor.Trim().ToUpperInvariant();

        var baixa = Assert.Single(prioridades, x => x.Nivel == PrioridadeChamadoEnum.Baixa);
        Assert.Equal(1, baixa.Peso);
        Assert.Equal(Cor("#22C55E"), Cor(baixa.Cor ?? string.Empty));

        var media = Assert.Single(prioridades, x => x.Nivel == PrioridadeChamadoEnum.Media);
        Assert.Equal(2, media.Peso);
        Assert.Equal(Cor("#EAB308"), Cor(media.Cor ?? string.Empty));

        var alta = Assert.Single(prioridades, x => x.Nivel == PrioridadeChamadoEnum.Alta);
        Assert.Equal(3, alta.Peso);
        Assert.Equal(Cor("#F97316"), Cor(alta.Cor ?? string.Empty));

        var critica = Assert.Single(prioridades, x => x.Nivel == PrioridadeChamadoEnum.Critica);
        Assert.Equal(4, critica.Peso);
        Assert.Equal(Cor("#EF4444"), Cor(critica.Cor ?? string.Empty));
    }

    private static string NormalizarChaveTexto(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return string.Empty;
        }

        var texto = valor.Trim().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(texto.Length);

        foreach (var caractere in texto)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(caractere) == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            if (char.IsLetterOrDigit(caractere))
            {
                builder.Append(char.ToLowerInvariant(caractere));
                continue;
            }

            if (char.IsWhiteSpace(caractere))
            {
                builder.Append(' ');
            }
        }

        return string.Join(' ', builder.ToString().Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }

    private sealed class FakeEnvironment : Microsoft.Extensions.Hosting.IHostEnvironment
    {
        public string EnvironmentName { get; set; } = "Development";
        public string ApplicationName { get; set; } = "SGX.SistemaChamado.Tests";
        public string ContentRootPath { get; set; } = Directory.GetCurrentDirectory();
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
