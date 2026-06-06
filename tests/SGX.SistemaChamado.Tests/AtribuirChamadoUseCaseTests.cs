using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class AtribuirChamadoUseCaseTests
{
    [Fact]
    public async Task AdministradorAtribuiChamadoParaAtendente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.AdminContexto);
        var response = await useCase.ExecutarAsync(dados.Chamado.Id, new AtribuirChamadoRequest { ResponsavelId = dados.Atendente.Id });

        var chamado = context.Chamados.Single();
        Assert.Equal(dados.Atendente.Id, chamado.ResponsavelId);
        Assert.Null(chamado.GrupoTecnicoId);
        Assert.Null(chamado.FilaAtendimentoId);
        Assert.Equal(dados.Chamado.Id, response.Id);
    }

    [Fact]
    public async Task AtendenteNaoPodeAtribuirChamadoATecnicoEspecifico()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, AdminUseCasesTestFactory.Contexto(dados.Atendente, "Atendente"));

        var erro = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            useCase.ExecutarAsync(dados.Chamado.Id, new AtribuirChamadoRequest { ResponsavelId = dados.OutroAtendente.Id }));

        Assert.Equal("Atribuicao de responsavel permitida apenas para Administrador nesta sprint.", erro.Message);
        Assert.Null(context.Chamados.Single().ResponsavelId);
    }

    [Fact]
    public async Task BloqueiaAtribuicaoParaUsuarioSemPerfilAtendimento()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.AdminContexto);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(dados.Chamado.Id, new AtribuirChamadoRequest { ResponsavelId = dados.SomenteSolicitante.Id }));
    }

    [Fact]
    public async Task CriaHistoricoAoAtribuir()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = CriarUseCase(context, dados.AdminContexto);
        await useCase.ExecutarAsync(dados.Chamado.Id, new AtribuirChamadoRequest { ResponsavelId = dados.Atendente.Id });

        Assert.Contains(context.HistoricosChamado, x =>
            x.Tipo == TipoHistoricoChamado.ResponsavelAlterado &&
            x.Descricao == "Responsavel alterado para Atendente");
    }

    [Fact]
    public async Task PreservaGrupoEFilaAoAtribuirResponsavel()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var (grupo, fila) = await CriarGrupoEFilaAsync(context);
        await CriarMembroAsync(context, grupo.Id, dados.Atendente.Id);
        dados.Chamado.DefinirGrupoTecnico(grupo.Id, "teste");
        dados.Chamado.DefinirFilaAtendimento(fila.Id, "teste");
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.AdminContexto);
        await useCase.ExecutarAsync(dados.Chamado.Id, new AtribuirChamadoRequest { ResponsavelId = dados.Atendente.Id });

        var chamado = context.Chamados.Single();
        Assert.Equal(dados.Atendente.Id, chamado.ResponsavelId);
        Assert.Equal(grupo.Id, chamado.GrupoTecnicoId);
        Assert.Equal(fila.Id, chamado.FilaAtendimentoId);
        Assert.Contains(context.HistoricosChamado, x =>
            x.Tipo == TipoHistoricoChamado.ResponsavelAlterado &&
            x.Descricao == "Responsavel alterado para Atendente");
    }

    [Fact]
    public async Task RejeitaTecnicoQueNaoEMembroDoGrupo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var (grupo, fila) = await CriarGrupoEFilaAsync(context);
        dados.Chamado.DefinirGrupoTecnico(grupo.Id, "teste");
        dados.Chamado.DefinirFilaAtendimento(fila.Id, "teste");
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.AdminContexto);

        var erro = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            useCase.ExecutarAsync(dados.Chamado.Id, new AtribuirChamadoRequest { ResponsavelId = dados.Atendente.Id }));

        Assert.Equal("Responsavel informado nao e membro ativo do grupo tecnico do chamado.", erro.Message);
        Assert.Null(context.Chamados.Single().ResponsavelId);
    }

    [Fact]
    public async Task RejeitaTecnicoComVinculoInativoNoGrupo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var (grupo, fila) = await CriarGrupoEFilaAsync(context);
        var membro = await CriarMembroAsync(context, grupo.Id, dados.Atendente.Id);
        membro.Inativar("teste");
        dados.Chamado.DefinirGrupoTecnico(grupo.Id, "teste");
        dados.Chamado.DefinirFilaAtendimento(fila.Id, "teste");
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.AdminContexto);

        var erro = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            useCase.ExecutarAsync(dados.Chamado.Id, new AtribuirChamadoRequest { ResponsavelId = dados.Atendente.Id }));

        Assert.Equal("Responsavel informado nao e membro ativo do grupo tecnico do chamado.", erro.Message);
    }

    [Fact]
    public async Task RejeitaGrupoTecnicoInativo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var (grupo, fila) = await CriarGrupoEFilaAsync(context);
        await CriarMembroAsync(context, grupo.Id, dados.Atendente.Id);
        grupo.Inativar("teste");
        dados.Chamado.DefinirGrupoTecnico(grupo.Id, "teste");
        dados.Chamado.DefinirFilaAtendimento(fila.Id, "teste");
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.AdminContexto);

        var erro = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            useCase.ExecutarAsync(dados.Chamado.Id, new AtribuirChamadoRequest { ResponsavelId = dados.Atendente.Id }));

        Assert.Equal("Grupo tecnico do chamado nao encontrado ou inativo.", erro.Message);
    }

    [Fact]
    public async Task RejeitaFilaInativa()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var (grupo, fila) = await CriarGrupoEFilaAsync(context);
        await CriarMembroAsync(context, grupo.Id, dados.Atendente.Id);
        fila.Inativar("teste");
        dados.Chamado.DefinirGrupoTecnico(grupo.Id, "teste");
        dados.Chamado.DefinirFilaAtendimento(fila.Id, "teste");
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.AdminContexto);

        var erro = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            useCase.ExecutarAsync(dados.Chamado.Id, new AtribuirChamadoRequest { ResponsavelId = dados.Atendente.Id }));

        Assert.Equal("Fila de atendimento do chamado nao encontrada ou inativa.", erro.Message);
    }

    [Fact]
    public async Task RejeitaFilaDeOutroGrupo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var (grupo, _) = await CriarGrupoEFilaAsync(context);
        var (_, filaOutroGrupo) = await CriarGrupoEFilaAsync(context, "Aplicacoes Teste", "Fila Aplicacoes Teste");
        await CriarMembroAsync(context, grupo.Id, dados.Atendente.Id);
        dados.Chamado.DefinirGrupoTecnico(grupo.Id, "teste");
        dados.Chamado.DefinirFilaAtendimento(filaOutroGrupo.Id, "teste");
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.AdminContexto);

        var erro = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            useCase.ExecutarAsync(dados.Chamado.Id, new AtribuirChamadoRequest { ResponsavelId = dados.Atendente.Id }));

        Assert.Equal("Fila de atendimento do chamado nao pertence ao grupo tecnico do chamado.", erro.Message);
    }

    [Fact]
    public async Task PermiteReatribuirChamadoComGrupoParaOutroMembroAtivo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var (grupo, fila) = await CriarGrupoEFilaAsync(context);
        await CriarMembroAsync(context, grupo.Id, dados.Atendente.Id);
        await CriarMembroAsync(context, grupo.Id, dados.OutroAtendente.Id);
        dados.Chamado.DefinirGrupoTecnico(grupo.Id, "teste");
        dados.Chamado.DefinirFilaAtendimento(fila.Id, "teste");
        dados.Chamado.AtribuirResponsavel(dados.OutroAtendente.Id, "teste");
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context, dados.AdminContexto);
        await useCase.ExecutarAsync(dados.Chamado.Id, new AtribuirChamadoRequest { ResponsavelId = dados.Atendente.Id });

        var chamado = context.Chamados.Single();
        Assert.Equal(dados.Atendente.Id, chamado.ResponsavelId);
        Assert.Equal(grupo.Id, chamado.GrupoTecnicoId);
        Assert.Equal(fila.Id, chamado.FilaAtendimentoId);
        Assert.Contains(context.HistoricosChamado, x =>
            x.Tipo == TipoHistoricoChamado.ResponsavelAlterado &&
            x.Descricao == "Responsavel alterado de Outro Atendente para Atendente");
    }

    private static AtribuirChamadoUseCase CriarUseCase(SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context, UsuarioContextoAplicacao contexto)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<Usuario>(context),
            PortalUseCasesTestFactory.Repo<MembroGrupoTecnico>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            SlaTestFactory.CriarService(context),
            new FakeUsuarioContextoAplicacaoService(contexto),
            PortalUseCasesTestFactory.Uow(context));

    private static async Task<(Chamado Chamado, Usuario Atendente, Usuario OutroAtendente, Usuario SomenteSolicitante, UsuarioContextoAplicacao AdminContexto)> SeedAsync(SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "admin@empresa.com", TipoPerfil.Administrador);
        var atendente = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Atendente", "aten@empresa.com", TipoPerfil.Atendente);
        var outroAtendente = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Outro Atendente", "outro.aten@empresa.com", TipoPerfil.Atendente);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante", "sol@empresa.com", TipoPerfil.Solicitante);
        var somenteSolicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Sem Atendimento", "sem@empresa.com", TipoPerfil.Solicitante);

        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Infra");
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.Aberto, null, "ATR1");

        return (chamado, atendente, outroAtendente, somenteSolicitante, AdminUseCasesTestFactory.Contexto(admin, "Administrador"));
    }

    private static async Task<(GrupoTecnico Grupo, FilaAtendimento Fila)> CriarGrupoEFilaAsync(
        SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context,
        string nomeGrupo = "Infra Teste",
        string nomeFila = "Fila Infra Teste")
    {
        var grupo = new GrupoTecnico(nomeGrupo, "Grupo tecnico de teste", "teste");
        context.GruposTecnicos.Add(grupo);
        await context.SaveChangesAsync();

        var fila = new FilaAtendimento(grupo.Id, nomeFila, "Fila de teste", "teste");
        context.FilasAtendimento.Add(fila);
        await context.SaveChangesAsync();

        return (grupo, fila);
    }

    private static async Task<MembroGrupoTecnico> CriarMembroAsync(
        SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context,
        Guid grupoId,
        Guid usuarioId)
    {
        var membro = new MembroGrupoTecnico(grupoId, usuarioId, "teste");
        context.MembrosGruposTecnicos.Add(membro);
        await context.SaveChangesAsync();
        return membro;
    }
}

