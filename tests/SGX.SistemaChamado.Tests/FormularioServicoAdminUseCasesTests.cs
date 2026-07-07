using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class FormularioServicoAdminUseCasesTests
{
    [Fact]
    public async Task CriarFormularioParaCatalogoValido()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);

        var response = await useCase.CriarAsync(new CriarFormularioServicoRequest
        {
            CatalogoServicoId = seed.Catalogo.Id,
            Nome = "Formulario de acesso",
            Descricao = "Coleta dados basicos"
        });

        Assert.Equal(seed.Catalogo.Id, response.CatalogoServicoId);
        Assert.Equal("Formulario de acesso", response.Nome);
        Assert.True(response.Ativo);
        Assert.Empty(response.Versoes);
    }

    [Fact]
    public async Task RejeitarFormularioParaCatalogoInexistente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.CriarAsync(new CriarFormularioServicoRequest
        {
            CatalogoServicoId = Guid.NewGuid(),
            Nome = "Formulario invalido"
        }));

        Assert.Equal("Catalogo de servico informado nao encontrado ou inativo.", ex.Message);
    }

    [Fact]
    public async Task RejeitarDuplicidadeDeFormularioPorCatalogo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);

        await useCase.CriarAsync(new CriarFormularioServicoRequest
        {
            CatalogoServicoId = seed.Catalogo.Id,
            Nome = "Formulario original"
        });

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.CriarAsync(new CriarFormularioServicoRequest
        {
            CatalogoServicoId = seed.Catalogo.Id,
            Nome = "Formulario duplicado"
        }));

        Assert.Equal("Catalogo de servico ja possui formulario configurado.", ex.Message);
    }

    [Fact]
    public async Task AtualizarFormularioExistente()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);
        var formulario = await CriarFormularioAsync(context, seed.Catalogo.Id);

        var response = await useCase.AtualizarAsync(formulario.Id, new AtualizarFormularioServicoRequest
        {
            Nome = "Formulario atualizado",
            Descricao = "Descricao revisada",
            Ativo = true
        });

        Assert.Equal(formulario.Id, response.Id);
        Assert.Equal("Formulario atualizado", response.Nome);
        Assert.Equal("Descricao revisada", response.Descricao);
    }

    [Fact]
    public async Task CriarVersaoParaFormularioValido()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);
        var formulario = await CriarFormularioAsync(context, seed.Catalogo.Id);

        var response = await useCase.CriarVersaoAsync(new CriarFormularioServicoVersaoRequest
        {
            FormularioServicoId = formulario.Id,
            Numero = 1,
            Publicada = false
        });

        Assert.Equal(formulario.Id, response.FormularioServicoId);
        Assert.Equal(1, response.Numero);
        Assert.Empty(response.Campos);
    }

    [Fact]
    public async Task RejeitarVersaoDuplicadaNoMesmoFormulario()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);
        var formulario = await CriarFormularioAsync(context, seed.Catalogo.Id);
        await CriarVersaoAsync(context, formulario.Id, 1);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.CriarVersaoAsync(new CriarFormularioServicoVersaoRequest
        {
            FormularioServicoId = formulario.Id,
            Numero = 1
        }));

        Assert.Equal("Ja existe versao com o mesmo numero para este formulario.", ex.Message);
    }

    [Fact]
    public async Task CriarCampoParaVersaoValida()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);
        var formulario = await CriarFormularioAsync(context, seed.Catalogo.Id);
        var versao = await CriarVersaoAsync(context, formulario.Id, 1);

        var response = await useCase.CriarCampoAsync(new CriarCampoFormularioServicoRequest
        {
            FormularioServicoVersaoId = versao.Id,
            Nome = "justificativa",
            Rotulo = "Justificativa",
            Tipo = TipoCampoFormularioServico.TextoLongo,
            Obrigatorio = true,
            Ordem = 1,
            TextoAjuda = "Explique a necessidade",
            Visivel = true
        });

        Assert.Equal(versao.Id, response.FormularioServicoVersaoId);
        Assert.Equal("justificativa", response.Nome);
        Assert.Equal(TipoCampoFormularioServico.TextoLongo, response.Tipo);
    }

    [Fact]
    public async Task RejeitarNomeDuplicadoNaMesmaVersao()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);
        var formulario = await CriarFormularioAsync(context, seed.Catalogo.Id);
        var versao = await CriarVersaoAsync(context, formulario.Id, 1);
        await CriarCampoAsync(context, versao.Id, "justificativa", 1, TipoCampoFormularioServico.TextoCurto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.CriarCampoAsync(new CriarCampoFormularioServicoRequest
        {
            FormularioServicoVersaoId = versao.Id,
            Nome = "justificativa",
            Rotulo = "Outra justificativa",
            Tipo = TipoCampoFormularioServico.TextoLongo,
            Ordem = 2,
            Visivel = true
        }));

        Assert.Equal("Ja existe campo com o mesmo nome nesta versao.", ex.Message);
    }

    [Fact]
    public async Task RejeitarOrdemDuplicadaNaMesmaVersao()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);
        var formulario = await CriarFormularioAsync(context, seed.Catalogo.Id);
        var versao = await CriarVersaoAsync(context, formulario.Id, 1);
        await CriarCampoAsync(context, versao.Id, "justificativa", 1, TipoCampoFormularioServico.TextoCurto);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.CriarCampoAsync(new CriarCampoFormularioServicoRequest
        {
            FormularioServicoVersaoId = versao.Id,
            Nome = "centro_custo",
            Rotulo = "Centro de custo",
            Tipo = TipoCampoFormularioServico.TextoCurto,
            Ordem = 1,
            Visivel = true
        }));

        Assert.Equal("Ja existe campo com a mesma ordem nesta versao.", ex.Message);
    }

    [Fact]
    public async Task CriarOpcaoParaCampoEnumerado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);
        var formulario = await CriarFormularioAsync(context, seed.Catalogo.Id);
        var versao = await CriarVersaoAsync(context, formulario.Id, 1);
        var campo = await CriarCampoAsync(context, versao.Id, "tipo_acesso", 1, TipoCampoFormularioServico.SelecaoUnica);

        var response = await useCase.CriarOpcaoAsync(new CriarOpcaoCampoFormularioServicoRequest
        {
            CampoFormularioServicoId = campo.Id,
            Valor = "vpn",
            Rotulo = "VPN",
            Ordem = 1
        });

        Assert.Equal(campo.Id, response.CampoFormularioServicoId);
        Assert.Equal("vpn", response.Valor);
    }

    [Fact]
    public async Task RejeitarOpcaoParaCampoNaoEnumerado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);
        var formulario = await CriarFormularioAsync(context, seed.Catalogo.Id);
        var versao = await CriarVersaoAsync(context, formulario.Id, 1);
        var campo = await CriarCampoAsync(context, versao.Id, "justificativa", 1, TipoCampoFormularioServico.TextoLongo);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.CriarOpcaoAsync(new CriarOpcaoCampoFormularioServicoRequest
        {
            CampoFormularioServicoId = campo.Id,
            Valor = "opcao",
            Rotulo = "Opcao",
            Ordem = 1
        }));

        Assert.Equal("Opcoes so podem ser configuradas para campos dos tipos SelecaoUnica ou SelecaoMultipla.", ex.Message);
    }

    [Fact]
    public async Task RejeitarValorDuplicadoNoMesmoCampo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);
        var formulario = await CriarFormularioAsync(context, seed.Catalogo.Id);
        var versao = await CriarVersaoAsync(context, formulario.Id, 1);
        var campo = await CriarCampoAsync(context, versao.Id, "tipo_acesso", 1, TipoCampoFormularioServico.SelecaoUnica);
        await CriarOpcaoAsync(context, campo.Id, "vpn", 1);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.CriarOpcaoAsync(new CriarOpcaoCampoFormularioServicoRequest
        {
            CampoFormularioServicoId = campo.Id,
            Valor = "vpn",
            Rotulo = "VPN duplicada",
            Ordem = 2
        }));

        Assert.Equal("Ja existe opcao com o mesmo valor para este campo.", ex.Message);
    }

    [Fact]
    public async Task RejeitarOrdemDuplicadaNoMesmoCampo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);
        var formulario = await CriarFormularioAsync(context, seed.Catalogo.Id);
        var versao = await CriarVersaoAsync(context, formulario.Id, 1);
        var campo = await CriarCampoAsync(context, versao.Id, "tipo_acesso", 1, TipoCampoFormularioServico.SelecaoUnica);
        await CriarOpcaoAsync(context, campo.Id, "vpn", 1);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.CriarOpcaoAsync(new CriarOpcaoCampoFormularioServicoRequest
        {
            CampoFormularioServicoId = campo.Id,
            Valor = "rdp",
            Rotulo = "RDP",
            Ordem = 1
        }));

        Assert.Equal("Ja existe opcao com a mesma ordem para este campo.", ex.Message);
    }

    [Fact]
    public async Task ListarEObterDetalheComVersoesCamposEOpcoes()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var useCase = CriarUseCase(context, seed.Admin);
        var formulario = await CriarFormularioAsync(context, seed.Catalogo.Id, "Formulario principal");
        var versao = await CriarVersaoAsync(context, formulario.Id, 2);
        var campo = await CriarCampoAsync(context, versao.Id, "tipo_acesso", 1, TipoCampoFormularioServico.SelecaoMultipla);
        await CriarOpcaoAsync(context, campo.Id, "vpn", 1);
        await CriarOpcaoAsync(context, campo.Id, "rdp", 2);

        var listagem = await useCase.ListarAsync(seed.Catalogo.Id);
        var detalhe = await useCase.ObterPorIdAsync(formulario.Id);
        var versoes = await useCase.ListarVersoesAsync(formulario.Id);
        var campos = await useCase.ListarCamposAsync(versao.Id);
        var opcoes = await useCase.ListarOpcoesAsync(campo.Id);

        Assert.Single(listagem);
        Assert.Equal(formulario.Id, listagem.Single().Id);

        Assert.Single(detalhe.Versoes);
        Assert.Single(detalhe.Versoes.Single().Campos);
        Assert.Equal(2, detalhe.Versoes.Single().Campos.Single().Opcoes.Count);

        Assert.Single(versoes);
        Assert.Single(campos);
        Assert.Equal(2, opcoes.Count);
    }

    [Fact]
    public async Task AtendentePodeConsultarFormularioMasNaoCriar()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var formulario = await CriarFormularioAsync(context, seed.Catalogo.Id, "Formulario visivel");
        var useCaseAtendente = CriarUseCase(context, seed.Atendente, "Atendente");

        var listagem = await useCaseAtendente.ListarAsync(seed.Catalogo.Id);
        var detalhe = await useCaseAtendente.ObterPorIdAsync(formulario.Id);

        Assert.Single(listagem);
        Assert.Equal(formulario.Id, detalhe.Id);

        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => useCaseAtendente.CriarAsync(new CriarFormularioServicoRequest
        {
            CatalogoServicoId = seed.Catalogo.Id,
            Nome = "Formulario bloqueado"
        }));

        Assert.Equal("Acao permitida somente para Administrador.", ex.Message);
    }

    [Fact]
    public async Task AtendenteNaoPodeInativarVersao()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var seed = await SeedAsync(context);
        var formulario = await CriarFormularioAsync(context, seed.Catalogo.Id);
        var versao = await CriarVersaoAsync(context, formulario.Id, 1);
        var useCaseAtendente = CriarUseCase(context, seed.Atendente, "Atendente");

        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => useCaseAtendente.InativarVersaoAsync(versao.Id));

        Assert.Equal("Acao permitida somente para Administrador.", ex.Message);
    }

    private static IAdminFormularioServicosUseCases CriarUseCase(SGXSistemaChamadoDbContext context, Usuario usuario, string perfil = "Administrador")
        => new FormularioServicoAdminUseCases(
            PortalUseCasesTestFactory.Repo<CatalogoServico>(context),
            PortalUseCasesTestFactory.Repo<FormularioServico>(context),
            PortalUseCasesTestFactory.Repo<FormularioServicoVersao>(context),
            PortalUseCasesTestFactory.Repo<CampoFormularioServico>(context),
            PortalUseCasesTestFactory.Repo<OpcaoCampoFormularioServico>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(usuario, perfil)),
            PortalUseCasesTestFactory.Uow(context));

    private static async Task<SeedFormularioAdmin> SeedAsync(SGXSistemaChamadoDbContext context)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            context,
            "Administrador Formularios",
            "admin.formularios@sgx.local",
            TipoPerfil.Administrador);

        var atendente = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            context,
            "Atendente Formularios",
            "atendente.formularios@sgx.local",
            TipoPerfil.Atendente);

        var departamento = new Departamento("Tecnologia", "TI", "Departamento de tecnologia", "teste");
        context.Departamentos.Add(departamento);
        await context.SaveChangesAsync();

        var catalogo = new CatalogoServico(
            "Acesso remoto",
            "acesso-remoto",
            "Servico de acesso remoto",
            null,
            departamento.Id,
            null,
            null,
            null,
            null,
            null,
            VisibilidadeCatalogoServico.Solicitante,
            true,
            false,
            1,
            admin.Id,
            "teste");

        context.CatalogosServico.Add(catalogo);
        await context.SaveChangesAsync();

        return new SeedFormularioAdmin(admin, atendente, catalogo);
    }

    private static async Task<FormularioServico> CriarFormularioAsync(
        SGXSistemaChamadoDbContext context,
        Guid catalogoServicoId,
        string nome = "Formulario")
    {
        var formulario = new FormularioServico(catalogoServicoId, nome, "Descricao", "teste");
        context.FormulariosServico.Add(formulario);
        await context.SaveChangesAsync();
        return formulario;
    }

    private static async Task<FormularioServicoVersao> CriarVersaoAsync(
        SGXSistemaChamadoDbContext context,
        Guid formularioServicoId,
        int numero)
    {
        var versao = new FormularioServicoVersao(formularioServicoId, numero, false, null, "teste");
        context.FormulariosServicoVersoes.Add(versao);
        await context.SaveChangesAsync();
        return versao;
    }

    private static async Task<CampoFormularioServico> CriarCampoAsync(
        SGXSistemaChamadoDbContext context,
        Guid formularioServicoVersaoId,
        string nome,
        int ordem,
        TipoCampoFormularioServico tipo)
    {
        var campo = new CampoFormularioServico(
            formularioServicoVersaoId,
            nome,
            $"Rotulo {nome}",
            tipo,
            false,
            ordem,
            null,
            true,
            "teste");

        context.CamposFormularioServico.Add(campo);
        await context.SaveChangesAsync();
        return campo;
    }

    private static async Task<OpcaoCampoFormularioServico> CriarOpcaoAsync(
        SGXSistemaChamadoDbContext context,
        Guid campoFormularioServicoId,
        string valor,
        int ordem)
    {
        var opcao = new OpcaoCampoFormularioServico(campoFormularioServicoId, valor, valor.ToUpperInvariant(), ordem, "teste");
        context.OpcoesCamposFormularioServico.Add(opcao);
        await context.SaveChangesAsync();
        return opcao;
    }

    private sealed record SeedFormularioAdmin(Usuario Admin, Usuario Atendente, CatalogoServico Catalogo);
}
