using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Tests;

public sealed class PermissoesSistemaSeedTests
{
    [Fact]
    public void DeveSeedarPermissoesPorPerfilSemDuplicidades()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();

        var permissoes = context.PermissoesSistema.ToList();
        var vinculos = context.PerfisAcessoPermissoes.ToList();
        var perfis = context.PerfisAcesso.ToList();

        // 1. Verificar contagem de permissões
        Assert.True(permissoes.Count >= 77);
        
        // 2. Verificar que perfis padrão existem no seed e não possuem duplicidade
        Assert.Contains(perfis, x => x.Nome == "Administrador");
        Assert.Contains(perfis, x => x.Nome == "Atendente");
        Assert.Contains(perfis, x => x.Nome == "Solicitante");
        Assert.Contains(perfis, x => x.Nome == "Atendente N1");
        Assert.Contains(perfis, x => x.Nome == "Técnico N2");
        Assert.Contains(perfis, x => x.Nome == "Coordenador Service Desk");
        Assert.Contains(perfis, x => x.Nome == "Gestor TI");
        Assert.Contains(perfis, x => x.Nome == "Auditor Governança");
        Assert.Equal(perfis.Count, perfis.Select(x => x.Nome.ToLowerInvariant()).Distinct().Count());

        Assert.Equal(permissoes.Count, permissoes.Select(x => x.Codigo).Distinct(StringComparer.Ordinal).Count());
        Assert.Equal(vinculos.Count, vinculos.Select(x => new { x.PerfilAcessoId, x.PermissaoSistemaId }).Distinct().Count());
        
        // 3. Administrador mantém acesso total
        var totalAdmin = vinculos.Count(x => x.PerfilAcessoId == SeedData.PerfilAdministradorId);
        Assert.Equal(permissoes.Count, totalAdmin);

        // 4. Solicitante não possui permissões administrativas
        var solicitanteVinculos = vinculos
            .Where(x => x.PerfilAcessoId == SeedData.PerfilSolicitanteId)
            .Select(x => x.PermissaoSistema.Codigo)
            .ToList();
        Assert.True(solicitanteVinculos.Count >= 5);
        Assert.Contains("Chamados.Abrir", solicitanteVinculos);
        Assert.DoesNotContain("Usuarios.Gerenciar", solicitanteVinculos);
        Assert.DoesNotContain("Perfis.Gerenciar", solicitanteVinculos);

        // 5. Gestor TI não recebe ações operacionais por padrão
        var gestorVinculos = vinculos
            .Where(x => x.PerfilAcessoId == SeedData.PerfilGestorTiId)
            .Select(x => x.PermissaoSistema.Codigo)
            .ToList();
        Assert.Contains("Dashboard.Visualizar", gestorVinculos);
        Assert.Contains("RelatoriosAvancados.Visualizar", gestorVinculos);
        Assert.DoesNotContain("Chamados.Abrir", gestorVinculos);
        Assert.DoesNotContain("Chamados.Assumir", gestorVinculos);
        Assert.DoesNotContain("Problemas.Gerenciar", gestorVinculos);
        Assert.DoesNotContain("Mudancas.Gerenciar", gestorVinculos);

        // 6. Auditor Governança não recebe ações operacionais por padrão
        var auditorVinculos = vinculos
            .Where(x => x.PerfilAcessoId == SeedData.PerfilAuditorGovernancaId)
            .Select(x => x.PermissaoSistema.Codigo)
            .ToList();
        Assert.Contains("Auditoria.Visualizar", auditorVinculos);
        Assert.Contains("AuditoriaAutenticacao.Visualizar", auditorVinculos);
        Assert.DoesNotContain("Chamados.Abrir", auditorVinculos);
        Assert.DoesNotContain("Chamados.Assumir", auditorVinculos);
        Assert.DoesNotContain("Problemas.Gerenciar", auditorVinculos);
        Assert.DoesNotContain("Mudancas.Gerenciar", auditorVinculos);

        // 7. Técnico N2 possui permissões de ITSM
        var tecnicoVinculos = vinculos
            .Where(x => x.PerfilAcessoId == SeedData.PerfilTecnicoN2Id)
            .Select(x => x.PermissaoSistema.Codigo)
            .ToList();
        Assert.Contains("Problemas.Gerenciar", tecnicoVinculos);
        Assert.Contains("Mudancas.Gerenciar", tecnicoVinculos);
        Assert.Contains("Tarefas.Gerenciar", tecnicoVinculos);

        // 8. Atendente N1 possui permissões operacionais de atendimento, mas não gerenciais avançadas de ITSM
        var atendenteN1Vinculos = vinculos
            .Where(x => x.PerfilAcessoId == SeedData.PerfilAtendenteN1Id)
            .Select(x => x.PermissaoSistema.Codigo)
            .ToList();
        Assert.Contains("Chamados.Visualizar", atendenteN1Vinculos);
        Assert.Contains("Chamados.Assumir", atendenteN1Vinculos);
        Assert.DoesNotContain("Problemas.Gerenciar", atendenteN1Vinculos);
        Assert.DoesNotContain("Mudancas.Gerenciar", atendenteN1Vinculos);
        Assert.DoesNotContain("Tarefas.Gerenciar", atendenteN1Vinculos);

        // 9. Coordenador possui permissões de gestão operacional, SLA e atribuição
        var coordenadorVinculos = vinculos
            .Where(x => x.PerfilAcessoId == SeedData.PerfilCoordenadorServiceDeskId)
            .Select(x => x.PermissaoSistema.Codigo)
            .ToList();
        Assert.Contains("Sla.Criar", coordenadorVinculos);
        Assert.Contains("AprovacaoChamados.Aprovar", coordenadorVinculos);
        Assert.Contains("Chamados.Atribuir", coordenadorVinculos);
    }

    [Fact]
    public void Seed_NaoPossuiCodigoDuplicado_EmPermissaoSistema()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var codigos = context.PermissoesSistema.Select(x => x.Codigo).ToList();
        
        Assert.Equal(codigos.Count, codigos.Distinct(StringComparer.Ordinal).Count());
    }

    [Fact]
    public void PermissoesEssenciais_Chamados_Existem()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var codigos = context.PermissoesSistema.Select(x => x.Codigo).ToList();

        Assert.Contains("Chamados.Visualizar", codigos);
        Assert.Contains("Chamados.VisualizarTodos", codigos);
        Assert.Contains("Chamados.Abrir", codigos);
        Assert.Contains("Chamados.Assumir", codigos);
        Assert.Contains("Chamados.Atribuir", codigos);
        Assert.Contains("Chamados.AlterarStatus", codigos);
        Assert.Contains("Chamados.Encerrar", codigos);
    }

    [Fact]
    public void PermissoesEssenciais_Relatorios_Existem()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var codigos = context.PermissoesSistema.Select(x => x.Codigo).ToList();

        Assert.Contains("RelatoriosAvancados.Visualizar", codigos);
        Assert.Contains("RelatoriosAvancados.Exportar", codigos);
        Assert.Contains("RelatoriosAvancados.Gerencial", codigos);
        Assert.Contains("RelatoriosAvancados.Operacional", codigos);
    }

    [Fact]
    public void PermissoesEssenciais_Administracao_Existem()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var codigos = context.PermissoesSistema.Select(x => x.Codigo).ToList();

        Assert.Contains("Usuarios.Gerenciar", codigos);
        Assert.Contains("Perfis.Gerenciar", codigos);
        Assert.Contains("Cadastros.Gerenciar", codigos);
        Assert.Contains("Parametros.Gerenciar", codigos);
    }

    [Fact]
    public void PermissoesEssenciais_Auditoria_Existem()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var codigos = context.PermissoesSistema.Select(x => x.Codigo).ToList();

        Assert.Contains("Auditoria.Visualizar", codigos);
        Assert.Contains("AuditoriaAutenticacao.Visualizar", codigos);
    }

    [Fact]
    public void Perfil_Administrador_MantemPermissoesAdministrativas()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        
        var vinculos = context.PerfisAcessoPermissoes
            .Where(x => x.PerfilAcessoId == SeedData.PerfilAdministradorId)
            .Select(x => x.PermissaoSistema.Codigo)
            .ToList();

        Assert.Contains("Usuarios.Gerenciar", vinculos);
        Assert.Contains("Perfis.Gerenciar", vinculos);
        Assert.Contains("Perfis.AlterarPermissoes", vinculos);
        Assert.Contains("Parametros.Gerenciar", vinculos);
    }

    [Fact]
    public void Perfil_Solicitante_MantemPermissoesDePortal()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        
        var vinculos = context.PerfisAcessoPermissoes
            .Where(x => x.PerfilAcessoId == SeedData.PerfilSolicitanteId)
            .Select(x => x.PermissaoSistema.Codigo)
            .ToList();

        Assert.True(vinculos.Count >= 5);
        Assert.Contains("Chamados.Visualizar", vinculos);
        Assert.Contains("Chamados.Abrir", vinculos);
        Assert.Contains("Chamados.Comentar", vinculos);
        Assert.Contains("Chamados.Anexar", vinculos);
        Assert.Contains("Notificacoes.Visualizar", vinculos);
    }

    [Fact]
    public void Perfil_Atendente_MantemPermissoesOperacionais()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        
        var vinculos = context.PerfisAcessoPermissoes
            .Where(x => x.PerfilAcessoId == SeedData.PerfilAtendenteId)
            .Select(x => x.PermissaoSistema.Codigo)
            .ToList();

        Assert.Contains("Chamados.Visualizar", vinculos);
        Assert.Contains("Chamados.VisualizarTodos", vinculos);
        Assert.Contains("Chamados.Assumir", vinculos);
        Assert.Contains("Chamados.AlterarStatus", vinculos);
        Assert.Contains("Problemas.Visualizar", vinculos);
        Assert.Contains("Mudancas.Visualizar", vinculos);
        Assert.Contains("Tarefas.Visualizar", vinculos);
    }
}
