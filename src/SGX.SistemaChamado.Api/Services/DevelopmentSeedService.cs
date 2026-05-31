using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text;
using SGX.SistemaChamado.Api.Options;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Api.Services;

public sealed class DevelopmentSeedService(
    SGXSistemaChamadoDbContext dbContext,
    IHostEnvironment environment,
    IOptions<AuthOptions> authOptions,
    IPasswordHasher<Usuario> passwordHasher,
    ILogger<DevelopmentSeedService> logger)
{
    private const string UsuarioTecnico = "seed.development";
    private const string DominioDemoOficial = "@sgxdigital.com";
    private const string DominioDemoLegado = "@sgx.local";

    private static readonly UsuarioDemoDef[] UsuariosDemo =
    [
        new("Administrador Demo 1", "admin@sgxdigital.com", "Administrador"),
        new("Administrador Demo 2", "admin2@sgxdigital.com", "Administrador"),
        new("Atendente Demo 1", "atendente.demo@sgxdigital.com", "Atendente"),
        new("Atendente Demo 2", "atendente2.demo@sgxdigital.com", "Atendente"),
        new("Solicitante Demo 1", "solicitante.demo@sgxdigital.com", "Solicitante"),
        new("Solicitante Demo 2", "solicitante2.demo@sgxdigital.com", "Solicitante"),

        new("Solicitante Homologacao", "solicitante.hml@sgx.local", "Solicitante"),
        new("Atendente N1 Homologacao", "atendente.n1.hml@sgx.local", "Atendente N1"),
        new("Tecnico N2 Homologacao", "tecnico.n2.hml@sgx.local", "Técnico N2"),
        new("Coordenador Service Desk Homologacao", "coordenador.service.desk.hml@sgx.local", "Coordenador Service Desk"),
        new("Gestor TI Homologacao", "gestor.ti.hml@sgx.local", "Gestor TI"),
        new("Administrador Homologacao", "administrador.hml@sgx.local", "Administrador"),
        new("Auditor Governanca Homologacao", "auditor.governanca.hml@sgx.local", "Auditor Governança")
    ];

    private static readonly HashSet<string> EmailsDemoLegados = new(StringComparer.OrdinalIgnoreCase)
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
        "solicitante.sla.local@sgx.local"
    };

    private static readonly string[] PrefixosDemoLegados =
    [
        "administrador.",
        "admin.",
        "atendente.",
        "solicitante.",
        "usuario.homol."
    ];

    private static readonly string[] MarcadoresParteLocalDemoLegados =
    [
        ".local",
        ".homol",
        ".demo",
        ".sla",
        ".portal",
        ".teste",
        ".seed"
    ];

    private static readonly string[] MarcadoresDemoLegados =
    [
        ".local@",
        ".homol.",
        ".demo@",
        ".sla.",
        ".portal@"
    ];

    private static readonly CadastroDepartamentoDef[] DepartamentosIniciais =
    [
        new("Tecnologia da Informacao", "TI", "Departamento de tecnologia da informacao."),
        new("Recursos Humanos", "RH", "Departamento de recursos humanos."),
        new("Financeiro", "FIN", "Departamento financeiro."),
        new("Juridico", "JUR", "Departamento juridico."),
        new("Atendimento", "ATD", "Departamento de atendimento."),
        new("Infraestrutura", "INF", "Departamento de infraestrutura.")
    ];

    private static readonly string[] CategoriasIniciais =
    [
        "Hardware",
        "Software",
        "Rede",
        "Sistema",
        "Acesso",
        "E-mail",
        "Impressora",
        "Telefonia",
        "Solicitacao Administrativa"
    ];

    private static readonly string[] TiposSolicitacaoIniciais =
    [
        "Incidente",
        "Solicitacao de Servico",
        "Duvida",
        "Melhoria",
        "Problema Recorrente"
    ];

    private static readonly string[] LocaisUnidadeIniciais =
    [
        "Sede",
        "Filial",
        "Inspetoria",
        "Datacenter",
        "Almoxarifado",
        "Atendimento Externo"
    ];

    private static readonly CadastroPrioridadeDef[] PrioridadesIniciais =
    [
        new("Baixa", PrioridadeChamadoEnum.Baixa, 1, "#22C55E", 8, 48),
        new("Media", PrioridadeChamadoEnum.Media, 2, "#EAB308", 4, 24),
        new("Alta", PrioridadeChamadoEnum.Alta, 3, "#F97316", 2, 8),
        new("Critica", PrioridadeChamadoEnum.Critica, 4, "#EF4444", 1, 4)
    ];

    private static readonly IReadOnlyDictionary<string, string[]> SubcategoriasIniciaisPorCategoria =
        new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
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

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (!environment.IsDevelopment())
        {
            return;
        }

        var departamentos = await GarantirDepartamentosIniciaisAsync(cancellationToken);
        var departamentoTecnologia = departamentos.First(x => x.Sigla == "TI");
        var categorias = await GarantirCategoriasIniciaisAsync(departamentoTecnologia.Id, cancellationToken);
        await GarantirSubcategoriasIniciaisAsync(categorias, cancellationToken);
        await GarantirPrioridadesIniciaisAsync(cancellationToken);
        await GarantirTiposSolicitacaoIniciaisAsync(cancellationToken);
        await GarantirLocaisUnidadeIniciaisAsync(cancellationToken);

        var perfisLista = await dbContext.PerfisAcesso
            .Where(x => x.Ativo)
            .ToListAsync(cancellationToken);

        var perfisPorNome = perfisLista.ToDictionary(x => x.Nome, x => x, StringComparer.OrdinalIgnoreCase);

        var emailsPermitidos = new HashSet<string>(
            UsuariosDemo.Select(x => x.Email),
            StringComparer.OrdinalIgnoreCase);

        var usuariosRelacionados = await dbContext.Usuarios
            .Include(x => x.UsuarioPerfis)
            .ThenInclude(x => x.PerfilAcesso)
            .ToListAsync(cancellationToken);

        var emailAdminInicialProtegido = ObterEmailAdminInicialProtegido();

        foreach (var usuarioExistente in usuariosRelacionados)
        {
            var email = NormalizarEmail(usuarioExistente.Email);
            if (emailsPermitidos.Contains(email))
            {
                continue;
            }

            if (EhAdministradorInicialProtegido(usuarioExistente, email, emailAdminInicialProtegido))
            {
                continue;
            }

            if (!EhUsuarioDemonstrativoAntigo(usuarioExistente, email))
            {
                continue;
            }

            if (usuarioExistente.Ativo)
            {
                usuarioExistente.Desativar(UsuarioTecnico);
            }

            if (usuarioExistente.Situacao != SituacaoUsuario.Inativo)
            {
                usuarioExistente.AlterarSituacao(SituacaoUsuario.Inativo, UsuarioTecnico);
            }
        }

        foreach (var usuarioDemo in UsuariosDemo)
        {
            var usuario = usuariosRelacionados.FirstOrDefault(x =>
                string.Equals(x.Email, usuarioDemo.Email, StringComparison.OrdinalIgnoreCase)
                || string.Equals(x.Login, usuarioDemo.Email, StringComparison.OrdinalIgnoreCase));

            if (usuario is null)
            {
                usuario = new Usuario(
                    usuarioDemo.Nome,
                    usuarioDemo.Email,
                    usuarioDemo.Email,
                    UsuarioTecnico,
                    departamentoTecnologia.Id);

                await dbContext.Usuarios.AddAsync(usuario, cancellationToken);
                usuariosRelacionados.Add(usuario);
            }
            else
            {
                usuario.DefinirNome(usuarioDemo.Nome);
                usuario.DefinirEmail(usuarioDemo.Email);
                usuario.DefinirLogin(usuarioDemo.Email);
                usuario.DefinirDepartamento(departamentoTecnologia.Id, UsuarioTecnico);
                if (!usuario.Ativo)
                {
                    usuario.Ativar(UsuarioTecnico);
                }

                if (usuario.Situacao != SituacaoUsuario.Ativo)
                {
                    usuario.AlterarSituacao(SituacaoUsuario.Ativo, UsuarioTecnico);
                }
            }

            if (!perfisPorNome.TryGetValue(usuarioDemo.PerfilNome, out var perfilEsperado))
            {
                logger.LogWarning("Perfil '{PerfilNome}' nao encontrado para o usuario demo '{Email}'. Pulo.", usuarioDemo.PerfilNome, usuarioDemo.Email);
                continue;
            }

            var jaPossuiPerfil = usuario.UsuarioPerfis.Any(x => x.PerfilAcessoId == perfilEsperado.Id);
            if (!jaPossuiPerfil)
            {
                var vinculo = new UsuarioPerfilAcesso(usuario.Id, perfilEsperado.Id, UsuarioTecnico);
                usuario.UsuarioPerfis.Add(vinculo);
                await dbContext.UsuariosPerfisAcesso.AddAsync(vinculo, cancellationToken);
            }

            var vinculosParaRemover = usuario.UsuarioPerfis
                .Where(x => x.PerfilAcessoId != perfilEsperado.Id)
                .ToList();
            foreach (var vinculo in vinculosParaRemover)
            {
                usuario.UsuarioPerfis.Remove(vinculo);
                dbContext.UsuariosPerfisAcesso.Remove(vinculo);
            }

            AplicarSenhaLocalSeConfigurada(usuario);
            usuario.DefinirDeveAlterarSenha(false, UsuarioTecnico);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Seed Development de usuarios de homologacao aplicado com sucesso.");

        // Tarefa 2 — Massa de chamados para homologação
        var categoriasDb = await dbContext.CategoriasChamado.ToListAsync(cancellationToken);
        var categoriaSistema = categoriasDb.FirstOrDefault(x => x.Nome == "Sistema") ?? categoriasDb.First();
        var categoriaAcesso = categoriasDb.FirstOrDefault(x => x.Nome == "Acesso") ?? categoriasDb.First();
        var categoriaRede = categoriasDb.FirstOrDefault(x => x.Nome == "Rede") ?? categoriasDb.First();
        var categoriaSuporte = categoriasDb.FirstOrDefault(x => x.Nome == "Suporte Tecnico") ?? categoriasDb.First();

        var statusDb = await dbContext.StatusChamado.ToListAsync(cancellationToken);
        var statusAberto = statusDb.FirstOrDefault(x => x.Codigo == StatusChamadoEnum.Aberto) ?? statusDb.First();
        var statusEmAtendimento = statusDb.FirstOrDefault(x => x.Codigo == StatusChamadoEnum.EmAtendimento) ?? statusDb.First();
        var statusEncerrado = statusDb.FirstOrDefault(x => x.Codigo == StatusChamadoEnum.Encerrado) ?? statusDb.First();

        var prioridadesDb = await dbContext.PrioridadesChamado.ToListAsync(cancellationToken);
        var prioridadeBaixa = prioridadesDb.FirstOrDefault(x => x.Nivel == PrioridadeChamadoEnum.Baixa) ?? prioridadesDb.First();
        var prioridadeMedia = prioridadesDb.FirstOrDefault(x => x.Nivel == PrioridadeChamadoEnum.Media) ?? prioridadesDb.First();
        var prioridadeAlta = prioridadesDb.FirstOrDefault(x => x.Nivel == PrioridadeChamadoEnum.Alta) ?? prioridadesDb.First();
        var prioridadeCritica = prioridadesDb.FirstOrDefault(x => x.Nivel == PrioridadeChamadoEnum.Critica) ?? prioridadesDb.First();

        var solicitanteHml = usuariosRelacionados.First(x => x.Email == "solicitante.hml@sgx.local");
        var atendenteN1Hml = usuariosRelacionados.First(x => x.Email == "atendente.n1.hml@sgx.local");
        var tecnicoN2Hml = usuariosRelacionados.First(x => x.Email == "tecnico.n2.hml@sgx.local");

        if (!await dbContext.Chamados.AnyAsync(cancellationToken))
        {
            var chamado1 = new Chamado(
                "HML-INC-001",
                "Sistema indisponível para usuário final",
                "O portal corporativo está apresentando erro HTTP 500 para todos os usuários da rede interna.",
                solicitanteHml.Id,
                categoriaSistema.Id,
                prioridadeCritica.Id,
                statusEmAtendimento.Id,
                OrigemChamado.Portal,
                UsuarioTecnico,
                naturezaChamado: NaturezaChamadoEnum.Incidente,
                impactoChamado: ImpactoChamadoEnum.Alto,
                urgenciaChamado: UrgenciaChamadoEnum.Alta);
            chamado1.AtribuirResponsavel(tecnicoN2Hml.Id, UsuarioTecnico);

            var chamado2 = new Chamado(
                "HML-REQ-002",
                "Solicitação de acesso a sistema",
                "Favor liberar acesso ao módulo financeiro do ERP para a nova analista de contas a pagar.",
                solicitanteHml.Id,
                categoriaAcesso.Id,
                prioridadeBaixa.Id,
                statusAberto.Id,
                OrigemChamado.Portal,
                UsuarioTecnico,
                naturezaChamado: NaturezaChamadoEnum.Requisicao,
                impactoChamado: ImpactoChamadoEnum.Baixo,
                urgenciaChamado: UrgenciaChamadoEnum.Media);

            var chamado3 = new Chamado(
                "HML-MUD-003",
                "Alteração planejada em configuração de servidor",
                "Upgrade de memória RAM e processadores do servidor de banco de dados em homologação.",
                solicitanteHml.Id,
                categoriaSuporte.Id,
                prioridadeAlta.Id,
                statusAberto.Id,
                OrigemChamado.Portal,
                UsuarioTecnico,
                naturezaChamado: NaturezaChamadoEnum.Mudanca,
                impactoChamado: ImpactoChamadoEnum.Alto,
                urgenciaChamado: UrgenciaChamadoEnum.Media);

            var chamado4 = new Chamado(
                "HML-PROB-004",
                "Falha recorrente em autenticação",
                "Investigação da causa raiz de quedas de login que ocorrem diariamente às 14h.",
                solicitanteHml.Id,
                categoriaSistema.Id,
                prioridadeMedia.Id,
                statusEmAtendimento.Id,
                OrigemChamado.Portal,
                UsuarioTecnico,
                naturezaChamado: NaturezaChamadoEnum.Problema,
                impactoChamado: ImpactoChamadoEnum.Medio,
                urgenciaChamado: UrgenciaChamadoEnum.Media);
            chamado4.AtribuirResponsavel(tecnicoN2Hml.Id, UsuarioTecnico);

            var chamado5 = new Chamado(
                "HML-ALR-005",
                "Alerta de monitoramento de serviço crítico",
                "Uso de CPU acima de 95% detectado no Datacenter Principal.",
                solicitanteHml.Id,
                categoriaRede.Id,
                prioridadeAlta.Id,
                statusAberto.Id,
                OrigemChamado.Portal,
                UsuarioTecnico,
                naturezaChamado: NaturezaChamadoEnum.EventoAlerta,
                impactoChamado: ImpactoChamadoEnum.Medio,
                urgenciaChamado: UrgenciaChamadoEnum.Alta);

            var chamado6 = new Chamado(
                "HML-TAR-006",
                "Rotina de verificação de backup",
                "Execução da rotina semanal de restore e validação da integridade dos backups do ERP.",
                solicitanteHml.Id,
                categoriaSuporte.Id,
                prioridadeBaixa.Id,
                statusEncerrado.Id,
                OrigemChamado.Portal,
                UsuarioTecnico,
                naturezaChamado: NaturezaChamadoEnum.TarefaOperacional,
                impactoChamado: ImpactoChamadoEnum.Baixo,
                urgenciaChamado: UrgenciaChamadoEnum.Baixa);
            chamado6.AtribuirResponsavel(atendenteN1Hml.Id, UsuarioTecnico);
            chamado6.Encerrar(statusEncerrado.Id, UsuarioTecnico);

            await dbContext.Chamados.AddRangeAsync(
                [chamado1, chamado2, chamado3, chamado4, chamado5, chamado6],
                cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Massa de 6 chamados de homologacao criada com sucesso.");
        }
    }

    private static bool EhAdministradorInicialProtegido(Usuario usuario, string email, string? emailAdminInicialProtegido)
    {
        if (string.IsNullOrWhiteSpace(emailAdminInicialProtegido))
        {
            return false;
        }

        return string.Equals(email, emailAdminInicialProtegido, StringComparison.OrdinalIgnoreCase);
    }

    private static bool EhUsuarioDemonstrativoAntigo(Usuario usuario, string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        if (EmailsDemoLegados.Contains(email))
        {
            return true;
        }

        if ((usuario.Nome ?? string.Empty).Contains("homol", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (email.EndsWith(DominioDemoLegado, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (!email.EndsWith(DominioDemoOficial, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var parteLocal = ObterParteLocalEmail(email);
        if (string.IsNullOrWhiteSpace(parteLocal))
        {
            return false;
        }

        if (parteLocal.StartsWith("usuario.homol.", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (PrefixosDemoLegados.Any(prefixo => parteLocal.StartsWith(prefixo, StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        if (MarcadoresParteLocalDemoLegados.Any(marcador => parteLocal.Contains(marcador, StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        return MarcadoresDemoLegados.Any(marcador => email.Contains(marcador, StringComparison.OrdinalIgnoreCase));
    }

    private static string NormalizarEmail(string? email)
        => (email ?? string.Empty).Trim().ToLowerInvariant();

    private static string ObterParteLocalEmail(string email)
    {
        var arroba = email.IndexOf('@');
        if (arroba <= 0)
        {
            return string.Empty;
        }

        return email[..arroba];
    }

    private static string? ObterEmailAdminInicialProtegido()
    {
        var email = Environment.GetEnvironmentVariable("SGX_ADMIN_INICIAL_EMAIL");
        if (string.IsNullOrWhiteSpace(email))
        {
            return null;
        }

        return NormalizarEmail(email);
    }

    private async Task<List<Departamento>> GarantirDepartamentosIniciaisAsync(CancellationToken cancellationToken)
    {
        var departamentosExistentes = await dbContext.Departamentos
            .ToListAsync(cancellationToken);

        var departamentosCriados = new List<Departamento>();
        foreach (var definicao in DepartamentosIniciais)
        {
            var chaveNome = NormalizarChaveTexto(definicao.Nome);
            var existente = departamentosExistentes.FirstOrDefault(x =>
                NormalizarChaveTexto(x.Nome) == chaveNome
                || string.Equals(x.Sigla, definicao.Sigla, StringComparison.OrdinalIgnoreCase));

            if (existente is not null)
            {
                continue;
            }

            var departamento = new Departamento(
                definicao.Nome,
                definicao.Sigla,
                definicao.Descricao,
                UsuarioTecnico);

            await dbContext.Departamentos.AddAsync(departamento, cancellationToken);
            departamentosCriados.Add(departamento);
            departamentosExistentes.Add(departamento);
        }

        if (departamentosCriados.Count > 0)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return departamentosExistentes;
    }

    private async Task<List<CategoriaChamado>> GarantirCategoriasIniciaisAsync(Guid departamentoId, CancellationToken cancellationToken)
    {
        var categoriasExistentes = await dbContext.CategoriasChamado
            .ToListAsync(cancellationToken);

        var categoriasCriadas = new List<CategoriaChamado>();
        foreach (var nomeCategoria in CategoriasIniciais)
        {
            var chaveNome = NormalizarChaveTexto(nomeCategoria);
            var existente = categoriasExistentes.FirstOrDefault(x => NormalizarChaveTexto(x.Nome) == chaveNome);
            if (existente is not null)
            {
                continue;
            }

            var categoria = new CategoriaChamado(
                nomeCategoria,
                null,
                departamentoId,
                UsuarioTecnico);

            await dbContext.CategoriasChamado.AddAsync(categoria, cancellationToken);
            categoriasCriadas.Add(categoria);
            categoriasExistentes.Add(categoria);
        }

        var suporteTecnicoExistente = categoriasExistentes.FirstOrDefault(x => NormalizarChaveTexto(x.Nome) == "suporte tecnico");
        if (suporteTecnicoExistente is null)
        {
            var suporteTecnico = new CategoriaChamado(
                "Suporte Tecnico",
                "Categoria inicial para atendimento tecnico interno.",
                departamentoId,
                UsuarioTecnico);

            await dbContext.CategoriasChamado.AddAsync(suporteTecnico, cancellationToken);
            categoriasCriadas.Add(suporteTecnico);
        }

        if (categoriasCriadas.Count > 0)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return categoriasExistentes;
    }

    private async Task GarantirSubcategoriasIniciaisAsync(IReadOnlyCollection<CategoriaChamado> categorias, CancellationToken cancellationToken)
    {
        var subcategoriasExistentes = await dbContext.SubcategoriasChamado
            .ToListAsync(cancellationToken);

        var subcategoriasCriadas = new List<SubcategoriaChamado>();
        foreach (var item in SubcategoriasIniciaisPorCategoria)
        {
            var chaveCategoria = NormalizarChaveTexto(item.Key);
            var categoria = categorias.FirstOrDefault(x => NormalizarChaveTexto(x.Nome) == chaveCategoria);
            if (categoria is null)
            {
                continue;
            }

            foreach (var nomeSubcategoria in item.Value)
            {
                var chaveSubcategoria = NormalizarChaveTexto(nomeSubcategoria);
                var existente = subcategoriasExistentes.FirstOrDefault(x =>
                    x.CategoriaChamadoId == categoria.Id
                    && NormalizarChaveTexto(x.Nome) == chaveSubcategoria);

                if (existente is not null)
                {
                    continue;
                }

                var subcategoria = new SubcategoriaChamado(categoria.Id, nomeSubcategoria, null, UsuarioTecnico);
                await dbContext.SubcategoriasChamado.AddAsync(subcategoria, cancellationToken);
                subcategoriasCriadas.Add(subcategoria);
                subcategoriasExistentes.Add(subcategoria);
            }
        }

        if (subcategoriasCriadas.Count > 0)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task GarantirPrioridadesIniciaisAsync(CancellationToken cancellationToken)
    {
        var prioridadesExistentes = await dbContext.PrioridadesChamado
            .ToListAsync(cancellationToken);

        var alterouPrioridade = false;
        foreach (var definicao in PrioridadesIniciais)
        {
            var prioridade = prioridadesExistentes.FirstOrDefault(x =>
                x.Nivel == definicao.Nivel
                || NormalizarChaveTexto(x.Nome) == NormalizarChaveTexto(definicao.Nome));

            if (prioridade is null)
            {
                prioridade = new PrioridadeChamado(
                    definicao.Nome,
                    definicao.Nivel,
                    null,
                    definicao.PrazoPrimeiraRespostaHoras,
                    definicao.PrazoResolucaoHoras,
                    UsuarioTecnico);

                prioridade.DefinirPesoECor(definicao.Peso, definicao.Cor);
                await dbContext.PrioridadesChamado.AddAsync(prioridade, cancellationToken);
                prioridadesExistentes.Add(prioridade);
                alterouPrioridade = true;
                continue;
            }

            var pesoAtual = prioridade.Peso;
            var corAtual = prioridade.Cor?.Trim().ToUpperInvariant();
            var corEsperada = definicao.Cor.Trim().ToUpperInvariant();
            var deveAtualizar = pesoAtual != definicao.Peso || !string.Equals(corAtual, corEsperada, StringComparison.Ordinal);

            if (!deveAtualizar)
            {
                continue;
            }

            prioridade.DefinirPesoECor(definicao.Peso, definicao.Cor);
            prioridade.AtualizarAuditoria(UsuarioTecnico);
            alterouPrioridade = true;
        }

        if (alterouPrioridade)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task GarantirTiposSolicitacaoIniciaisAsync(CancellationToken cancellationToken)
    {
        var tiposExistentes = await dbContext.TiposSolicitacao
            .ToListAsync(cancellationToken);

        var tiposCriados = new List<TipoSolicitacao>();
        foreach (var nomeTipo in TiposSolicitacaoIniciais)
        {
            var chaveNome = NormalizarChaveTexto(nomeTipo);
            var existente = tiposExistentes.FirstOrDefault(x => NormalizarChaveTexto(x.Nome) == chaveNome);
            if (existente is not null)
            {
                continue;
            }

            var tipo = new TipoSolicitacao(nomeTipo, null, UsuarioTecnico);
            await dbContext.TiposSolicitacao.AddAsync(tipo, cancellationToken);
            tiposCriados.Add(tipo);
            tiposExistentes.Add(tipo);
        }

        if (tiposCriados.Count > 0)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task GarantirLocaisUnidadeIniciaisAsync(CancellationToken cancellationToken)
    {
        var locaisExistentes = await dbContext.LocaisUnidade
            .ToListAsync(cancellationToken);

        var locaisCriados = new List<LocalUnidade>();
        foreach (var nomeLocal in LocaisUnidadeIniciais)
        {
            var chaveNome = NormalizarChaveTexto(nomeLocal);
            var existente = locaisExistentes.FirstOrDefault(x => NormalizarChaveTexto(x.Nome) == chaveNome);
            if (existente is not null)
            {
                continue;
            }

            var local = new LocalUnidade(nomeLocal, null, null, UsuarioTecnico);
            await dbContext.LocaisUnidade.AddAsync(local, cancellationToken);
            locaisCriados.Add(local);
            locaisExistentes.Add(local);
        }

        if (locaisCriados.Count > 0)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private void AplicarSenhaLocalSeConfigurada(Usuario usuario)
    {
        if (!authOptions.Value.LoginLocalHabilitado)
        {
            return;
        }

        var senhaAdminLocal = authOptions.Value.AdminLocalSenha?.Trim();
        if (string.IsNullOrWhiteSpace(senhaAdminLocal))
        {
            return;
        }

        usuario.DefinirSenhaHashLocal(passwordHasher.HashPassword(usuario, senhaAdminLocal), UsuarioTecnico);
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

    private sealed record CadastroDepartamentoDef(string Nome, string Sigla, string Descricao);
    private sealed record CadastroPrioridadeDef(
        string Nome,
        PrioridadeChamadoEnum Nivel,
        int Peso,
        string Cor,
        int PrazoPrimeiraRespostaHoras,
        int PrazoResolucaoHoras);
    private sealed record UsuarioDemoDef(string Nome, string Email, string PerfilNome);
}
