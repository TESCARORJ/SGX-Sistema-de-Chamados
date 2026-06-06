using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class MembrosGruposTecnicosAdminUseCaseTests
{
    [Fact]
    public async Task ListaMembrosDoGrupoComFiltroAtivo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await CriarAdminAsync(context, "membros.lista@empresa.com");
        var grupo = await CriarGrupoAsync(context, "Grupo Lista");
        var usuarioAtivo = await CriarUsuarioAsync(context, "Usuario Ativo", "membro.ativo@empresa.com");
        var usuarioInativo = await CriarUsuarioAsync(context, "Usuario Inativo", "membro.inativo@empresa.com");
        var membroAtivo = new MembroGrupoTecnico(grupo.Id, usuarioAtivo.Id, "teste");
        var membroInativo = new MembroGrupoTecnico(grupo.Id, usuarioInativo.Id, "teste");
        membroInativo.Inativar("teste");
        context.MembrosGruposTecnicos.AddRange(membroAtivo, membroInativo);
        await context.SaveChangesAsync();

        var useCase = new ListarMembrosGrupoTecnicoAdminUseCase(
            PortalUseCasesTestFactory.Repo<GrupoTecnico>(context),
            PortalUseCasesTestFactory.Repo<MembroGrupoTecnico>(context),
            Contexto(admin));

        var response = await useCase.ExecutarAsync(grupo.Id, new ListarMembrosGrupoTecnicoRequest { Ativo = true });

        Assert.Single(response);
        Assert.Equal(usuarioAtivo.Id, response.Single().UsuarioId);
        Assert.Equal("Usuario Ativo", response.Single().UsuarioNome);
        Assert.True(response.Single().Ativo);
    }

    [Fact]
    public async Task ListaMembrosDoGrupoComFiltroInativo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await CriarAdminAsync(context, "membros.lista.inativos@empresa.com");
        var grupo = await CriarGrupoAsync(context, "Grupo Lista Inativos");
        var usuarioAtivo = await CriarUsuarioAsync(context, "Usuario Ativo Lista", "membro.lista.ativo@empresa.com");
        var usuarioInativo = await CriarUsuarioAsync(context, "Usuario Inativo Lista", "membro.lista.inativo@empresa.com");
        var membroAtivo = new MembroGrupoTecnico(grupo.Id, usuarioAtivo.Id, "teste");
        var membroInativo = new MembroGrupoTecnico(grupo.Id, usuarioInativo.Id, "teste");
        membroInativo.Inativar("teste");
        context.MembrosGruposTecnicos.AddRange(membroAtivo, membroInativo);
        await context.SaveChangesAsync();

        var useCase = new ListarMembrosGrupoTecnicoAdminUseCase(
            PortalUseCasesTestFactory.Repo<GrupoTecnico>(context),
            PortalUseCasesTestFactory.Repo<MembroGrupoTecnico>(context),
            Contexto(admin));

        var response = await useCase.ExecutarAsync(grupo.Id, new ListarMembrosGrupoTecnicoRequest { Ativo = false });

        Assert.Single(response);
        Assert.Equal(usuarioInativo.Id, response.Single().UsuarioId);
        Assert.False(response.Single().Ativo);
    }

    [Fact]
    public async Task RejeitaListarMembrosDeGrupoInexistente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await CriarAdminAsync(context, "membros.lista.inexistente@empresa.com");
        var useCase = new ListarMembrosGrupoTecnicoAdminUseCase(
            PortalUseCasesTestFactory.Repo<GrupoTecnico>(context),
            PortalUseCasesTestFactory.Repo<MembroGrupoTecnico>(context),
            Contexto(admin));

        await Assert.ThrowsAsync<KeyNotFoundException>(() => useCase.ExecutarAsync(
            Guid.NewGuid(),
            new ListarMembrosGrupoTecnicoRequest()));
    }

    [Fact]
    public async Task AdicionaUsuarioComoMembroDeGrupoTecnico()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await CriarAdminAsync(context, "membros.adicionar@empresa.com");
        var grupo = await CriarGrupoAsync(context, "Grupo Adicionar");
        var usuario = await CriarUsuarioAsync(context, "Usuario Membro", "membro.add@empresa.com");
        var useCase = CriarAdicionarUseCase(context, admin);

        var response = await useCase.ExecutarAsync(grupo.Id, new AdicionarMembroGrupoTecnicoRequest { UsuarioId = usuario.Id });

        Assert.Equal(grupo.Id, response.GrupoTecnicoId);
        Assert.Equal(usuario.Id, response.UsuarioId);
        Assert.Equal("Usuario Membro", response.UsuarioNome);
        Assert.True(response.Ativo);
        Assert.Single(context.MembrosGruposTecnicos.Where(x => x.GrupoTecnicoId == grupo.Id && x.UsuarioId == usuario.Id));
    }

    [Fact]
    public async Task RejeitaAdicionarMembroEmGrupoInativo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await CriarAdminAsync(context, "membros.grupo.inativo@empresa.com");
        var grupo = await CriarGrupoAsync(context, "Grupo Inativo");
        grupo.Inativar("teste");
        await context.SaveChangesAsync();
        var usuario = await CriarUsuarioAsync(context, "Usuario Grupo Inativo", "membro.grupo.inativo@empresa.com");
        var useCase = CriarAdicionarUseCase(context, admin);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(
            grupo.Id,
            new AdicionarMembroGrupoTecnicoRequest { UsuarioId = usuario.Id }));
    }

    [Fact]
    public async Task RejeitaAdicionarMembroEmGrupoInexistente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await CriarAdminAsync(context, "membros.grupo.inexistente@empresa.com");
        var usuario = await CriarUsuarioAsync(context, "Usuario Grupo Inexistente", "membro.grupo.inexistente@empresa.com");
        var useCase = CriarAdicionarUseCase(context, admin);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => useCase.ExecutarAsync(
            Guid.NewGuid(),
            new AdicionarMembroGrupoTecnicoRequest { UsuarioId = usuario.Id }));
    }

    [Fact]
    public async Task RejeitaAdicionarMembroComIdVazio()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await CriarAdminAsync(context, "membros.id.vazio@empresa.com");
        var grupo = await CriarGrupoAsync(context, "Grupo Id Vazio");
        var useCase = CriarAdicionarUseCase(context, admin);

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecutarAsync(
            grupo.Id,
            new AdicionarMembroGrupoTecnicoRequest { UsuarioId = Guid.Empty }));
    }

    [Fact]
    public async Task RejeitaUsuarioInexistenteAoAdicionarMembro()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await CriarAdminAsync(context, "membros.usuario.inexistente@empresa.com");
        var grupo = await CriarGrupoAsync(context, "Grupo Usuario Inexistente");
        var useCase = CriarAdicionarUseCase(context, admin);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => useCase.ExecutarAsync(
            grupo.Id,
            new AdicionarMembroGrupoTecnicoRequest { UsuarioId = Guid.NewGuid() }));
    }

    [Fact]
    public async Task RejeitaDuplicidadeAtivaNoMesmoGrupo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await CriarAdminAsync(context, "membros.duplicado@empresa.com");
        var grupo = await CriarGrupoAsync(context, "Grupo Duplicado");
        var usuario = await CriarUsuarioAsync(context, "Usuario Duplicado", "membro.duplicado.usuario@empresa.com");
        context.MembrosGruposTecnicos.Add(new MembroGrupoTecnico(grupo.Id, usuario.Id, "teste"));
        await context.SaveChangesAsync();
        var useCase = CriarAdicionarUseCase(context, admin);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(
            grupo.Id,
            new AdicionarMembroGrupoTecnicoRequest { UsuarioId = usuario.Id }));
    }

    [Fact]
    public async Task ReativaVinculoInativoAoAdicionarNovamente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await CriarAdminAsync(context, "membros.reativar.inativo@empresa.com");
        var grupo = await CriarGrupoAsync(context, "Grupo Reativar Vinculo");
        var usuario = await CriarUsuarioAsync(context, "Usuario Reativado", "membro.reativado@empresa.com");
        var membro = new MembroGrupoTecnico(grupo.Id, usuario.Id, "teste");
        membro.Inativar("teste");
        context.MembrosGruposTecnicos.Add(membro);
        await context.SaveChangesAsync();
        var useCase = CriarAdicionarUseCase(context, admin);

        var response = await useCase.ExecutarAsync(grupo.Id, new AdicionarMembroGrupoTecnicoRequest { UsuarioId = usuario.Id });

        Assert.Equal(membro.Id, response.Id);
        Assert.True(response.Ativo);
        Assert.Single(context.MembrosGruposTecnicos.Where(x => x.GrupoTecnicoId == grupo.Id && x.UsuarioId == usuario.Id));
    }

    [Fact]
    public async Task AtualizaStatusDoMembroParaInativoEAtivo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await CriarAdminAsync(context, "membros.status@empresa.com");
        var grupo = await CriarGrupoAsync(context, "Grupo Status Membro");
        var usuario = await CriarUsuarioAsync(context, "Usuario Status", "membro.status.usuario@empresa.com");
        var membro = new MembroGrupoTecnico(grupo.Id, usuario.Id, "teste");
        context.MembrosGruposTecnicos.Add(membro);
        await context.SaveChangesAsync();

        var useCase = new AtualizarStatusMembroGrupoTecnicoAdminUseCase(
            PortalUseCasesTestFactory.Repo<MembroGrupoTecnico>(context),
            Contexto(admin),
            PortalUseCasesTestFactory.Uow(context));

        var inativado = await useCase.ExecutarAsync(membro.Id, new AlterarStatusMembroGrupoTecnicoRequest { Ativo = false });
        var reativado = await useCase.ExecutarAsync(membro.Id, new AlterarStatusMembroGrupoTecnicoRequest { Ativo = true });

        Assert.False(inativado.Ativo);
        Assert.True(reativado.Ativo);
        Assert.True(context.MembrosGruposTecnicos.Single(x => x.Id == membro.Id).Ativo);
    }

    [Fact]
    public async Task GestaoDeMembrosNaoAlteraResponsavelDoChamado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await CriarAdminAsync(context, "membros.nao.altera.responsavel@empresa.com");
        var grupo = await CriarGrupoAsync(context, "Grupo Nao Altera Responsavel");
        var usuarioMembro = await CriarUsuarioAsync(context, "Usuario Novo Membro", "novo.membro.responsavel@empresa.com");
        var usuarioReativado = await CriarUsuarioAsync(context, "Usuario Reativado Responsavel", "reativado.membro.responsavel@empresa.com");
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante Membro Responsavel", "sol.membro.responsavel@empresa.com", TipoPerfil.Solicitante);
        var responsavel = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Responsavel Membro", "responsavel.membro@empresa.com", TipoPerfil.Atendente);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Categoria Membro Responsavel");
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.Aberto, null, "MEM-RESP");
        chamado.AtribuirResponsavel(responsavel.Id, "teste");
        var membroInativo = new MembroGrupoTecnico(grupo.Id, usuarioReativado.Id, "teste");
        membroInativo.Inativar("teste");
        context.MembrosGruposTecnicos.Add(membroInativo);
        await context.SaveChangesAsync();

        var adicionarUseCase = CriarAdicionarUseCase(context, admin);
        var novoMembro = await adicionarUseCase.ExecutarAsync(grupo.Id, new AdicionarMembroGrupoTecnicoRequest { UsuarioId = usuarioMembro.Id });
        await adicionarUseCase.ExecutarAsync(grupo.Id, new AdicionarMembroGrupoTecnicoRequest { UsuarioId = usuarioReativado.Id });

        var statusUseCase = new AtualizarStatusMembroGrupoTecnicoAdminUseCase(
            PortalUseCasesTestFactory.Repo<MembroGrupoTecnico>(context),
            Contexto(admin),
            PortalUseCasesTestFactory.Uow(context));
        await statusUseCase.ExecutarAsync(novoMembro.Id, new AlterarStatusMembroGrupoTecnicoRequest { Ativo = false });
        await statusUseCase.ExecutarAsync(novoMembro.Id, new AlterarStatusMembroGrupoTecnicoRequest { Ativo = true });

        Assert.Equal(responsavel.Id, context.Chamados.Single(x => x.Id == chamado.Id).ResponsavelId);
    }

    [Fact]
    public async Task ListaGruposTecnicosDoUsuario()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await CriarAdminAsync(context, "membros.grupos.usuario@empresa.com");
        var grupo = await CriarGrupoAsync(context, "Grupo Do Usuario");
        var usuario = await CriarUsuarioAsync(context, "Usuario Com Grupo", "membro.grupo.usuario@empresa.com");
        context.MembrosGruposTecnicos.Add(new MembroGrupoTecnico(grupo.Id, usuario.Id, "teste"));
        await context.SaveChangesAsync();

        var useCase = new ListarGruposTecnicosDoUsuarioAdminUseCase(
            PortalUseCasesTestFactory.Repo<Usuario>(context),
            PortalUseCasesTestFactory.Repo<MembroGrupoTecnico>(context),
            Contexto(admin));

        var response = await useCase.ExecutarAsync(usuario.Id);

        Assert.Single(response);
        Assert.Equal(grupo.Id, response.Single().GrupoTecnicoId);
        Assert.StartsWith("Grupo Do Usuario", response.Single().Nome, StringComparison.Ordinal);
    }

    private static AdicionarMembroGrupoTecnicoAdminUseCase CriarAdicionarUseCase(SGXSistemaChamadoDbContext context, Usuario admin)
        => new(
            PortalUseCasesTestFactory.Repo<GrupoTecnico>(context),
            PortalUseCasesTestFactory.Repo<MembroGrupoTecnico>(context),
            PortalUseCasesTestFactory.Repo<Usuario>(context),
            Contexto(admin),
            PortalUseCasesTestFactory.Uow(context));

    private static async Task<Usuario> CriarAdminAsync(SGXSistemaChamadoDbContext context, string email)
        => await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin Membros Grupo", email, TipoPerfil.Administrador);

    private static async Task<Usuario> CriarUsuarioAsync(SGXSistemaChamadoDbContext context, string nome, string email)
        => await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, nome, email, TipoPerfil.Atendente);

    private static async Task<GrupoTecnico> CriarGrupoAsync(SGXSistemaChamadoDbContext context, string nome)
    {
        var grupo = new GrupoTecnico($"{nome} {Guid.NewGuid():N}", "Grupo tecnico de teste", "teste");
        context.GruposTecnicos.Add(grupo);
        await context.SaveChangesAsync();
        return grupo;
    }

    private static FakeUsuarioContextoAplicacaoService Contexto(Usuario admin)
        => new(AdminUseCasesTestFactory.Contexto(admin, "Administrador"));
}
