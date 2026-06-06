using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class GruposTecnicosAdminUseCaseTests
{
    [Fact]
    public async Task ListaGruposTecnicosComBuscaFiltroAtivoEPaginacao()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await CriarAdminAsync(context, "grupo.lista@empresa.com");
        var termo = $"Grupo Busca {Guid.NewGuid():N}";
        var ativo = new GrupoTecnico($"{termo} Ativo", "Atendimento inicial", "teste");
        var inativo = new GrupoTecnico($"{termo} Inativo", "Fila antiga", "teste");
        inativo.Inativar("teste");
        context.GruposTecnicos.AddRange(ativo, inativo);
        await context.SaveChangesAsync();

        var useCase = new ListarGruposTecnicosAdminUseCase(
            PortalUseCasesTestFactory.Repo<GrupoTecnico>(context),
            Contexto(admin));

        var response = await useCase.ExecutarAsync(new ListarGruposTecnicosRequest
        {
            Texto = termo,
            Ativo = true,
            Pagina = 1,
            TamanhoPagina = 10,
            OrdenarPor = "nome",
            DirecaoOrdenacao = "asc"
        });

        Assert.Single(response.Items);
        Assert.Equal($"{termo} Ativo", response.Items.Single().Nome);
        Assert.True(response.Items.Single().Ativo);
        Assert.Equal(1, response.Pagina);
        Assert.Equal(10, response.TamanhoPagina);
    }

    [Fact]
    public async Task ListaGruposTecnicosInativos()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await CriarAdminAsync(context, "grupo.lista.inativo@empresa.com");
        var termo = $"Grupo Inativo {Guid.NewGuid():N}";
        var ativo = new GrupoTecnico($"{termo} Ativo", null, "teste");
        var inativo = new GrupoTecnico($"{termo} Inativo", null, "teste");
        inativo.Inativar("teste");
        context.GruposTecnicos.AddRange(ativo, inativo);
        await context.SaveChangesAsync();

        var useCase = new ListarGruposTecnicosAdminUseCase(
            PortalUseCasesTestFactory.Repo<GrupoTecnico>(context),
            Contexto(admin));

        var response = await useCase.ExecutarAsync(new ListarGruposTecnicosRequest
        {
            Texto = termo,
            Ativo = false,
            Pagina = 1,
            TamanhoPagina = 10
        });

        Assert.Single(response.Items);
        Assert.Equal($"{termo} Inativo", response.Items.Single().Nome);
        Assert.False(response.Items.Single().Ativo);
    }

    [Fact]
    public async Task ObtemGrupoTecnicoPorId()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await CriarAdminAsync(context, "grupo.obter@empresa.com");
        var grupo = new GrupoTecnico("Infraestrutura", "Operacao de infraestrutura", "teste");
        context.GruposTecnicos.Add(grupo);
        await context.SaveChangesAsync();

        var useCase = new ObterGrupoTecnicoAdminUseCase(
            PortalUseCasesTestFactory.Repo<GrupoTecnico>(context),
            Contexto(admin));

        var response = await useCase.ExecutarAsync(grupo.Id);

        Assert.Equal(grupo.Id, response.Id);
        Assert.Equal("Infraestrutura", response.Nome);
        Assert.Equal("Operacao de infraestrutura", response.Descricao);
        Assert.True(response.Ativo);
    }

    [Fact]
    public async Task ObterGrupoTecnicoInexistenteRejeita()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await CriarAdminAsync(context, "grupo.obter.inexistente@empresa.com");

        var useCase = new ObterGrupoTecnicoAdminUseCase(
            PortalUseCasesTestFactory.Repo<GrupoTecnico>(context),
            Contexto(admin));

        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => useCase.ExecutarAsync(Guid.NewGuid()));

        Assert.Equal("Grupo tecnico nao encontrado.", ex.Message);
    }

    [Fact]
    public async Task CriaGrupoTecnicoAtivoPorPadrao()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await CriarAdminAsync(context, "grupo.criar@empresa.com");
        var useCase = CriarUseCase(context, admin);

        var response = await useCase.ExecutarAsync(new CriarGrupoTecnicoRequest
        {
            Nome = "  Sistemas Corporativos Teste  ",
            Descricao = "  Sustentacao de sistemas  "
        });

        Assert.Equal("Sistemas Corporativos Teste", response.Nome);
        Assert.Equal("Sustentacao de sistemas", response.Descricao);
        Assert.True(response.Ativo);
        Assert.Contains(context.GruposTecnicos, x => x.Id == response.Id && x.Ativo);
    }

    [Fact]
    public async Task CriaGrupoTecnicoComDescricaoNula()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await CriarAdminAsync(context, "grupo.criar.descricao.nula@empresa.com");
        var useCase = CriarUseCase(context, admin);

        var response = await useCase.ExecutarAsync(new CriarGrupoTecnicoRequest
        {
            Nome = "Grupo Sem Descricao",
            Descricao = null
        });

        Assert.Equal("Grupo Sem Descricao", response.Nome);
        Assert.Null(response.Descricao);
        Assert.True(response.Ativo);
    }

    [Fact]
    public async Task RejeitaCriacaoComNomeVazio()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await CriarAdminAsync(context, "grupo.nomevazio@empresa.com");
        var useCase = CriarUseCase(context, admin);

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecutarAsync(new CriarGrupoTecnicoRequest
        {
            Nome = "   ",
            Descricao = "Sem nome"
        }));
    }

    [Fact]
    public async Task RejeitaCriacaoComNomeDuplicado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await CriarAdminAsync(context, "grupo.duplicado@empresa.com");
        context.GruposTecnicos.Add(new GrupoTecnico("Redes", null, "teste"));
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context, admin);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarGrupoTecnicoRequest
        {
            Nome = "Redes",
            Descricao = "Duplicado"
        }));
    }

    [Fact]
    public async Task AtualizaGrupoTecnico()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await CriarAdminAsync(context, "grupo.atualizar@empresa.com");
        var grupo = new GrupoTecnico("Suporte N1", null, "teste");
        context.GruposTecnicos.Add(grupo);
        await context.SaveChangesAsync();

        var useCase = new AtualizarGrupoTecnicoAdminUseCase(
            PortalUseCasesTestFactory.Repo<GrupoTecnico>(context),
            Contexto(admin),
            PortalUseCasesTestFactory.Uow(context));

        var response = await useCase.ExecutarAsync(grupo.Id, new AtualizarGrupoTecnicoRequest
        {
            Nome = "Suporte N2",
            Descricao = "Atendimento avancado"
        });

        Assert.Equal("Suporte N2", response.Nome);
        Assert.Equal("Atendimento avancado", response.Descricao);
        Assert.NotNull(context.GruposTecnicos.Single(x => x.Id == grupo.Id).AtualizadoEm);
    }

    [Fact]
    public async Task RejeitaAtualizacaoComNomeVazio()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await CriarAdminAsync(context, "grupo.atualizar.nomevazio@empresa.com");
        var grupo = new GrupoTecnico("Suporte Atualizacao", null, "teste");
        context.GruposTecnicos.Add(grupo);
        await context.SaveChangesAsync();

        var useCase = new AtualizarGrupoTecnicoAdminUseCase(
            PortalUseCasesTestFactory.Repo<GrupoTecnico>(context),
            Contexto(admin),
            PortalUseCasesTestFactory.Uow(context));

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecutarAsync(grupo.Id, new AtualizarGrupoTecnicoRequest
        {
            Nome = "   ",
            Descricao = "Nome vazio"
        }));
    }

    [Fact]
    public async Task RejeitaAtualizacaoComNomeDuplicadoEmOutroGrupo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await CriarAdminAsync(context, "grupo.atualizar.duplicado@empresa.com");
        var grupo = new GrupoTecnico("Seguranca", null, "teste");
        var outro = new GrupoTecnico("Observabilidade", null, "teste");
        context.GruposTecnicos.AddRange(grupo, outro);
        await context.SaveChangesAsync();

        var useCase = new AtualizarGrupoTecnicoAdminUseCase(
            PortalUseCasesTestFactory.Repo<GrupoTecnico>(context),
            Contexto(admin),
            PortalUseCasesTestFactory.Uow(context));

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(outro.Id, new AtualizarGrupoTecnicoRequest
        {
            Nome = "Seguranca",
            Descricao = null
        }));
    }

    [Fact]
    public async Task AtualizaStatusGrupoTecnicoParaInativoEAtivo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await CriarAdminAsync(context, "grupo.status@empresa.com");
        var grupo = new GrupoTecnico("Operacoes", null, "teste");
        context.GruposTecnicos.Add(grupo);
        await context.SaveChangesAsync();

        var useCase = new AtualizarStatusGrupoTecnicoAdminUseCase(
            PortalUseCasesTestFactory.Repo<GrupoTecnico>(context),
            Contexto(admin),
            PortalUseCasesTestFactory.Uow(context));

        var inativado = await useCase.ExecutarAsync(grupo.Id, new AlterarStatusGrupoTecnicoRequest { Ativo = false });
        var reativado = await useCase.ExecutarAsync(grupo.Id, new AlterarStatusGrupoTecnicoRequest { Ativo = true });

        Assert.False(inativado.Ativo);
        Assert.True(reativado.Ativo);
        Assert.True(context.GruposTecnicos.Single(x => x.Id == grupo.Id).Ativo);
    }

    [Fact]
    public async Task CadastroEdicaoEStatusDeGrupoTecnicoNaoAlteramResponsavelDoChamado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await CriarAdminAsync(context, "grupo.nao.altera.responsavel@empresa.com");
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante Grupo Responsavel", "sol.grupo.responsavel@empresa.com", TipoPerfil.Solicitante);
        var atendente = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Atendente Grupo Responsavel", "aten.grupo.responsavel@empresa.com", TipoPerfil.Atendente);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Categoria Grupo Responsavel");
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.Aberto, null, "GR-RESP");
        chamado.AtribuirResponsavel(atendente.Id, "teste");
        await context.SaveChangesAsync();

        var criarUseCase = CriarUseCase(context, admin);
        var criado = await criarUseCase.ExecutarAsync(new CriarGrupoTecnicoRequest
        {
            Nome = $"Grupo Preserva Responsavel {Guid.NewGuid():N}",
            Descricao = "Nao deve alterar chamados"
        });

        var atualizarUseCase = new AtualizarGrupoTecnicoAdminUseCase(
            PortalUseCasesTestFactory.Repo<GrupoTecnico>(context),
            Contexto(admin),
            PortalUseCasesTestFactory.Uow(context));
        await atualizarUseCase.ExecutarAsync(criado.Id, new AtualizarGrupoTecnicoRequest
        {
            Nome = criado.Nome + " Atualizado",
            Descricao = "Ainda nao deve alterar chamados"
        });

        var statusUseCase = new AtualizarStatusGrupoTecnicoAdminUseCase(
            PortalUseCasesTestFactory.Repo<GrupoTecnico>(context),
            Contexto(admin),
            PortalUseCasesTestFactory.Uow(context));
        await statusUseCase.ExecutarAsync(criado.Id, new AlterarStatusGrupoTecnicoRequest { Ativo = false });
        await statusUseCase.ExecutarAsync(criado.Id, new AlterarStatusGrupoTecnicoRequest { Ativo = true });

        Assert.Equal(atendente.Id, context.Chamados.Single(x => x.Id == chamado.Id).ResponsavelId);
    }

    [Fact]
    public async Task ListaFilasAtendimentoDoGrupoTecnicoComFiltroAtivoEBusca()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await CriarAdminAsync(context, "grupo.filas@empresa.com");
        var grupo = new GrupoTecnico("Service Desk Filas", null, "teste");
        var outroGrupo = new GrupoTecnico("Outro Grupo Filas", null, "teste");
        context.GruposTecnicos.AddRange(grupo, outroGrupo);
        await context.SaveChangesAsync();

        var filaAtiva = new FilaAtendimento(grupo.Id, "Fila Incidentes", "Atendimento de incidentes", "teste");
        var filaInativa = new FilaAtendimento(grupo.Id, "Fila Requisicoes", "Requisicoes antigas", "teste");
        filaInativa.Inativar("teste");
        var filaOutroGrupo = new FilaAtendimento(outroGrupo.Id, "Fila Incidentes Outro", "Nao deve retornar", "teste");
        context.FilasAtendimento.AddRange(filaAtiva, filaInativa, filaOutroGrupo);
        await context.SaveChangesAsync();

        var useCase = new ListarFilasAtendimentoGrupoTecnicoAdminUseCase(
            PortalUseCasesTestFactory.Repo<GrupoTecnico>(context),
            PortalUseCasesTestFactory.Repo<FilaAtendimento>(context),
            Contexto(admin));

        var response = await useCase.ExecutarAsync(grupo.Id, new ListarFilasAtendimentoGrupoTecnicoRequest
        {
            Ativo = true,
            Busca = "Incidentes"
        });

        Assert.Single(response);
        Assert.Equal(filaAtiva.Id, response.Single().Id);
        Assert.Equal(grupo.Id, response.Single().GrupoTecnicoId);
        Assert.True(response.Single().Ativo);
    }

    [Fact]
    public async Task ListaFilasAtendimentoRejeitaGrupoInexistente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await CriarAdminAsync(context, "grupo.filas.inexistente@empresa.com");
        var useCase = new ListarFilasAtendimentoGrupoTecnicoAdminUseCase(
            PortalUseCasesTestFactory.Repo<GrupoTecnico>(context),
            PortalUseCasesTestFactory.Repo<FilaAtendimento>(context),
            Contexto(admin));

        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => useCase.ExecutarAsync(
            Guid.NewGuid(),
            new ListarFilasAtendimentoGrupoTecnicoRequest()));

        Assert.Equal("Grupo tecnico nao encontrado.", ex.Message);
    }

    private static CriarGrupoTecnicoAdminUseCase CriarUseCase(SGXSistemaChamadoDbContext context, Usuario admin)
        => new(
            PortalUseCasesTestFactory.Repo<GrupoTecnico>(context),
            Contexto(admin),
            PortalUseCasesTestFactory.Uow(context));

    private static async Task<Usuario> CriarAdminAsync(SGXSistemaChamadoDbContext context, string email)
        => await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin Grupo Tecnico", email, TipoPerfil.Administrador);

    private static FakeUsuarioContextoAplicacaoService Contexto(Usuario admin)
        => new(AdminUseCasesTestFactory.Contexto(admin, "Administrador"));
}
