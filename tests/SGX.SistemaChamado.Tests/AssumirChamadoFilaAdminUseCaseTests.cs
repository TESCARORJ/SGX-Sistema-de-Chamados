using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class AssumirChamadoFilaAdminUseCaseTests
{
    [Fact]
    public async Task AssumeChamadoDeFilaComUsuarioMembroAtivoDoGrupo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "AF1");
        var (grupo, fila) = await CriarGrupoFilaEMembroAsync(context, dados.Atendente, "AF1");
        dados.Chamado.DirecionarGrupoTecnico(grupo.Id, fila.Id, "teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.AtendenteContexto);

        await useCase.ExecutarAsync(dados.Chamado.Id, new AssumirChamadoFilaRequest { UsuarioId = dados.Atendente.Id });

        var chamado = context.Chamados.Single();
        Assert.Equal(dados.Atendente.Id, chamado.ResponsavelId);
        Assert.Equal(grupo.Id, chamado.GrupoTecnicoId);
        Assert.Equal(fila.Id, chamado.FilaAtendimentoId);
    }

    [Fact]
    public async Task PreservaGrupoTecnicoEFilaAtendimentoAoAssumir()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "AF2");
        var (grupo, fila) = await CriarGrupoFilaEMembroAsync(context, dados.Atendente, "AF2");
        dados.Chamado.DirecionarGrupoTecnico(grupo.Id, fila.Id, "teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.AtendenteContexto);

        await useCase.ExecutarAsync(dados.Chamado.Id, new AssumirChamadoFilaRequest { UsuarioId = dados.Atendente.Id });

        var chamado = context.Chamados.Single();
        Assert.Equal(grupo.Id, chamado.GrupoTecnicoId);
        Assert.Equal(fila.Id, chamado.FilaAtendimentoId);
    }

    [Fact]
    public async Task DefineResponsavelIdComUsuarioQueAssume()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "AF3");
        var (grupo, fila) = await CriarGrupoFilaEMembroAsync(context, dados.Atendente, "AF3");
        dados.Chamado.DirecionarGrupoTecnico(grupo.Id, fila.Id, "teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.AtendenteContexto);

        await useCase.ExecutarAsync(dados.Chamado.Id, new AssumirChamadoFilaRequest { UsuarioId = dados.Atendente.Id });

        Assert.Equal(dados.Atendente.Id, context.Chamados.Single().ResponsavelId);
    }

    [Fact]
    public async Task RejeitaAssuncaoDaFilaPorUsuarioSemPerfilAdministrativo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "AF16");
        var (grupo, fila) = await CriarGrupoFilaEMembroAsync(context, dados.Atendente, "AF16");
        dados.Chamado.DirecionarGrupoTecnico(grupo.Id, fila.Id, "teste");
        await context.SaveChangesAsync();
        var contextoSemPerfilAdmin = AdminUseCasesTestFactory.Contexto(dados.Atendente, "Solicitante");
        var useCase = CriarUseCase(context, contextoSemPerfilAdmin);

        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => useCase.ExecutarAsync(
            dados.Chamado.Id,
            new AssumirChamadoFilaRequest { UsuarioId = dados.Atendente.Id }));

        Assert.Equal("Acesso administrativo negado.", ex.Message);
        Assert.Null(context.Chamados.Single(x => x.Id == dados.Chamado.Id).ResponsavelId);
    }

    [Fact]
    public async Task RejeitaChamadoInexistente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "AF4");
        var useCase = CriarUseCase(context, dados.AtendenteContexto);

        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => useCase.ExecutarAsync(
            Guid.NewGuid(),
            new AssumirChamadoFilaRequest { UsuarioId = dados.Atendente.Id }));

        Assert.Equal("Chamado nao encontrado.", ex.Message);
    }

    [Fact]
    public async Task RejeitaChamadoSemGrupoTecnico()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "AF5");
        var useCase = CriarUseCase(context, dados.AtendenteContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(
            dados.Chamado.Id,
            new AssumirChamadoFilaRequest { UsuarioId = dados.Atendente.Id }));

        Assert.Equal("Chamado precisa estar vinculado a um grupo tecnico para ser assumido da fila.", ex.Message);
    }

    [Fact]
    public async Task RejeitaChamadoSemFilaAtendimento()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "AF6");
        var grupo = await CriarGrupoAsync(context, "Grupo AF6");
        await CriarMembroAsync(context, grupo, dados.Atendente);
        dados.Chamado.DefinirGrupoTecnico(grupo.Id, "teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.AtendenteContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(
            dados.Chamado.Id,
            new AssumirChamadoFilaRequest { UsuarioId = dados.Atendente.Id }));

        Assert.Equal("Chamado precisa estar vinculado a uma fila de atendimento para ser assumido da fila.", ex.Message);
    }

    [Fact]
    public async Task RejeitaChamadoJaComResponsavel()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "AF7");
        var (grupo, fila) = await CriarGrupoFilaEMembroAsync(context, dados.Atendente, "AF7");
        dados.Chamado.DirecionarGrupoTecnico(grupo.Id, fila.Id, "teste");
        dados.Chamado.AtribuirResponsavel(dados.OutroAtendente.Id, "teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.AtendenteContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(
            dados.Chamado.Id,
            new AssumirChamadoFilaRequest { UsuarioId = dados.Atendente.Id }));

        Assert.Equal("Chamado da fila ja possui responsavel individual.", ex.Message);
        Assert.Equal(dados.OutroAtendente.Id, context.Chamados.Single().ResponsavelId);
    }

    [Fact]
    public async Task RejeitaUsuarioQueNaoEMembroAtivoDoGrupo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "AF8");
        var grupo = await CriarGrupoAsync(context, "Grupo AF8");
        var fila = await CriarFilaAsync(context, grupo, "Fila AF8");
        dados.Chamado.DirecionarGrupoTecnico(grupo.Id, fila.Id, "teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.AtendenteContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(
            dados.Chamado.Id,
            new AssumirChamadoFilaRequest { UsuarioId = dados.Atendente.Id }));

        Assert.Equal("Usuario nao e membro ativo do grupo tecnico do chamado.", ex.Message);
    }

    [Fact]
    public async Task RejeitaMembroInativoDoGrupo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "AF9");
        var (grupo, fila) = await CriarGrupoFilaEMembroAsync(context, dados.Atendente, "AF9");
        var membro = context.MembrosGruposTecnicos.Single(x => x.GrupoTecnicoId == grupo.Id && x.UsuarioId == dados.Atendente.Id);
        membro.Inativar("teste");
        dados.Chamado.DirecionarGrupoTecnico(grupo.Id, fila.Id, "teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.AtendenteContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(
            dados.Chamado.Id,
            new AssumirChamadoFilaRequest { UsuarioId = dados.Atendente.Id }));

        Assert.Equal("Usuario nao e membro ativo do grupo tecnico do chamado.", ex.Message);
    }

    [Fact]
    public async Task RejeitaGrupoInativo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "AF10");
        var (grupo, fila) = await CriarGrupoFilaEMembroAsync(context, dados.Atendente, "AF10");
        grupo.Inativar("teste");
        dados.Chamado.DirecionarGrupoTecnico(grupo.Id, fila.Id, "teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.AtendenteContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(
            dados.Chamado.Id,
            new AssumirChamadoFilaRequest { UsuarioId = dados.Atendente.Id }));

        Assert.Equal("Grupo tecnico do chamado nao encontrado ou inativo.", ex.Message);
    }

    [Fact]
    public async Task RejeitaFilaInativa()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "AF11");
        var (grupo, fila) = await CriarGrupoFilaEMembroAsync(context, dados.Atendente, "AF11");
        fila.Inativar("teste");
        dados.Chamado.DirecionarGrupoTecnico(grupo.Id, fila.Id, "teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.AtendenteContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(
            dados.Chamado.Id,
            new AssumirChamadoFilaRequest { UsuarioId = dados.Atendente.Id }));

        Assert.Equal("Fila de atendimento do chamado nao encontrada ou inativa.", ex.Message);
    }

    [Fact]
    public async Task RejeitaFilaQueNaoPertenceAoGrupoDoChamado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "AF12");
        var grupo = await CriarGrupoAsync(context, "Grupo AF12");
        var outroGrupo = await CriarGrupoAsync(context, "Outro Grupo AF12");
        var filaOutroGrupo = await CriarFilaAsync(context, outroGrupo, "Fila Outro AF12");
        await CriarMembroAsync(context, grupo, dados.Atendente);
        dados.Chamado.DirecionarGrupoTecnico(grupo.Id, filaOutroGrupo.Id, "teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.AtendenteContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(
            dados.Chamado.Id,
            new AssumirChamadoFilaRequest { UsuarioId = dados.Atendente.Id }));

        Assert.Equal("Fila de atendimento do chamado nao pertence ao grupo tecnico do chamado.", ex.Message);
    }

    [Fact]
    public async Task RejeitaUsuarioDiferenteDoAutenticadoParaEvitarAtribuicaoManual()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "AF13");
        var (grupo, fila) = await CriarGrupoFilaEMembroAsync(context, dados.Atendente, "AF13");
        dados.Chamado.DirecionarGrupoTecnico(grupo.Id, fila.Id, "teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.AtendenteContexto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(
            dados.Chamado.Id,
            new AssumirChamadoFilaRequest { UsuarioId = dados.OutroAtendente.Id }));

        Assert.Equal("Chamado da fila so pode ser assumido pelo proprio usuario autenticado.", ex.Message);
    }

    [Fact]
    public async Task RegistraHistoricoTextual()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "AF14");
        var (grupo, fila) = await CriarGrupoFilaEMembroAsync(context, dados.Atendente, "AF14");
        dados.Chamado.DirecionarGrupoTecnico(grupo.Id, fila.Id, "teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, dados.AtendenteContexto);

        await useCase.ExecutarAsync(
            dados.Chamado.Id,
            new AssumirChamadoFilaRequest { UsuarioId = dados.Atendente.Id, Observacao = "Atendimento iniciado" });

        Assert.Contains(context.HistoricosChamado, x =>
            x.ChamadoId == dados.Chamado.Id &&
            x.Tipo == TipoHistoricoChamado.ChamadoAssumidoDaFila &&
            x.Descricao.Contains("Fila AF14", StringComparison.Ordinal) &&
            x.Descricao.Contains("Atendente AF14", StringComparison.Ordinal) &&
            x.Descricao.Contains("Atendimento iniciado", StringComparison.Ordinal));
    }

    [Fact]
    public async Task FluxoLegadoDeAssumirChamadoContinuaFuncionando()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context, "AF15");
        var useCase = new AssumirChamadoUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            SlaTestFactory.CriarService(context),
            new FakeUsuarioContextoAplicacaoService(dados.AtendenteContexto),
            PortalUseCasesTestFactory.Uow(context));

        await useCase.ExecutarAsync(dados.Chamado.Id);

        Assert.Equal(dados.Atendente.Id, context.Chamados.Single().ResponsavelId);
        Assert.Contains(context.HistoricosChamado, x => x.Tipo == TipoHistoricoChamado.ResponsavelAlterado);
    }

    private static AssumirChamadoFilaAdminUseCase CriarUseCase(SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context, UsuarioContextoAplicacao usuario)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<MembroGrupoTecnico>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            new FakeUsuarioContextoAplicacaoService(usuario),
            PortalUseCasesTestFactory.Uow(context));

    private static async Task<(Chamado Chamado, Usuario Atendente, Usuario OutroAtendente, UsuarioContextoAplicacao AtendenteContexto)> SeedAsync(
        SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context,
        string sufixo)
    {
        var atendente = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, $"Atendente {sufixo}", $"atendente-{sufixo}@empresa.com", TipoPerfil.Atendente);
        var outroAtendente = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, $"Outro Atendente {sufixo}", $"outro-{sufixo}@empresa.com", TipoPerfil.Atendente);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, $"Solicitante {sufixo}", $"sol-{sufixo}@empresa.com", TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, $"Categoria {sufixo}");
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.Aberto, null, sufixo);

        return (chamado, atendente, outroAtendente, AdminUseCasesTestFactory.Contexto(atendente, "Atendente"));
    }

    private static async Task<(GrupoTecnico Grupo, FilaAtendimento Fila)> CriarGrupoFilaEMembroAsync(
        SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context,
        Usuario usuario,
        string sufixo)
    {
        var grupo = await CriarGrupoAsync(context, $"Grupo {sufixo}");
        var fila = await CriarFilaAsync(context, grupo, $"Fila {sufixo}");
        await CriarMembroAsync(context, grupo, usuario);
        return (grupo, fila);
    }

    private static async Task<GrupoTecnico> CriarGrupoAsync(SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context, string nome)
    {
        var grupo = new GrupoTecnico(nome, "Grupo tecnico de teste", "teste");
        context.GruposTecnicos.Add(grupo);
        await context.SaveChangesAsync();
        return grupo;
    }

    private static async Task<FilaAtendimento> CriarFilaAsync(SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context, GrupoTecnico grupo, string nome)
    {
        var fila = new FilaAtendimento(grupo.Id, nome, "Fila de teste", "teste");
        context.FilasAtendimento.Add(fila);
        await context.SaveChangesAsync();
        return fila;
    }

    private static async Task CriarMembroAsync(SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context, GrupoTecnico grupo, Usuario usuario)
    {
        context.MembrosGruposTecnicos.Add(new MembroGrupoTecnico(grupo.Id, usuario.Id, "teste"));
        await context.SaveChangesAsync();
    }
}
