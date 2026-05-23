using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class BaseConhecimentoChamadoUseCasesTests
{
    [Fact]
    public async Task ListaArtigosVinculadosDoChamado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var artigo = await CriarArtigoAsync(context, dados.Admin, dados.Categoria.Id, "Guia VPN", StatusArtigoConhecimento.Publicado);
        await CriarVinculoAsync(context, dados.Chamado.Id, artigo.Id, dados.Admin.Id, "Referencia inicial");

        var useCase = new ListarArtigosConhecimentoDoChamadoUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<ChamadoArtigoConhecimento>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(dados.Admin, "Administrador")));

        var itens = await useCase.ExecutarAsync(dados.Chamado.Id);

        var vinculado = Assert.Single(itens);
        Assert.Equal(artigo.Id, vinculado.ArtigoId);
        Assert.Equal("Guia VPN", vinculado.Titulo);
        Assert.Equal("Administrador BC", vinculado.VinculadoPorUsuario);
    }

    [Fact]
    public async Task VinculaArtigoPublicadoAoChamado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var artigo = await CriarArtigoAsync(context, dados.Admin, dados.Categoria.Id, "Checklist VPN", StatusArtigoConhecimento.Publicado);

        var useCase = CriarUseCaseVinculo(context, dados.Admin);
        var response = await useCase.ExecutarAsync(dados.Chamado.Id, artigo.Id, new VincularArtigoChamadoRequest
        {
            Observacao = "Aplicar no atendimento N1"
        });

        Assert.Equal(artigo.Id, response.ArtigoId);
        Assert.Equal("Checklist VPN", response.Titulo);
        Assert.Equal("Aplicar no atendimento N1", response.Observacao);
        Assert.True(await context.ChamadosArtigosConhecimento.AnyAsync(x => x.ChamadoId == dados.Chamado.Id && x.ArtigoId == artigo.Id));
    }

    [Fact]
    public async Task ImpedeVinculoDuplicado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var artigo = await CriarArtigoAsync(context, dados.Admin, dados.Categoria.Id, "FAQ Acesso", StatusArtigoConhecimento.Publicado);

        var useCase = CriarUseCaseVinculo(context, dados.Admin);
        await useCase.ExecutarAsync(dados.Chamado.Id, artigo.Id);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(dados.Chamado.Id, artigo.Id));
    }

    [Fact]
    public async Task ImpedeVinculoDeArtigoArquivado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var artigo = await CriarArtigoAsync(context, dados.Admin, dados.Categoria.Id, "Artigo legado", StatusArtigoConhecimento.Publicado);
        artigo.Arquivar(dados.Admin.Id, dados.Admin.Login);
        await context.SaveChangesAsync();

        var useCase = CriarUseCaseVinculo(context, dados.Admin);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(dados.Chamado.Id, artigo.Id));
    }

    [Fact]
    public async Task ImpedeVinculoDeArtigoInativo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var artigo = await CriarArtigoAsync(context, dados.Admin, dados.Categoria.Id, "Artigo inativo", StatusArtigoConhecimento.Publicado);
        artigo.Desativar(dados.Admin.Login);
        await context.SaveChangesAsync();

        var useCase = CriarUseCaseVinculo(context, dados.Admin);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(dados.Chamado.Id, artigo.Id));
    }

    [Fact]
    public async Task ImpedeVinculoDeArtigoNaoPublicado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var artigo = await CriarArtigoAsync(context, dados.Admin, dados.Categoria.Id, "Rascunho interno", StatusArtigoConhecimento.Rascunho);

        var useCase = CriarUseCaseVinculo(context, dados.Admin);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(dados.Chamado.Id, artigo.Id));
    }

    [Fact]
    public async Task RemoveVinculoDoChamado()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var artigo = await CriarArtigoAsync(context, dados.Admin, dados.Categoria.Id, "Remover vinculo", StatusArtigoConhecimento.Publicado);
        await CriarVinculoAsync(context, dados.Chamado.Id, artigo.Id, dados.Admin.Id);

        var useCase = new RemoverArtigoConhecimentoDoChamadoUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<ChamadoArtigoConhecimento>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(dados.Admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        var response = await useCase.ExecutarAsync(dados.Chamado.Id, artigo.Id);

        Assert.Equal(artigo.Id, response.Id);
        Assert.False(await context.ChamadosArtigosConhecimento.AnyAsync(x => x.ChamadoId == dados.Chamado.Id && x.ArtigoId == artigo.Id));
    }

    [Fact]
    public async Task RegistraHistoricoEAuditoriaNoVinculoERemocao()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var artigo = await CriarArtigoAsync(context, dados.Admin, dados.Categoria.Id, "Documento auditoria", StatusArtigoConhecimento.Publicado);
        var auditoria = new FakeAuditoriaService();

        var vincular = new VincularArtigoConhecimentoAoChamadoUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<BaseConhecimentoArtigo>(context),
            PortalUseCasesTestFactory.Repo<ChamadoArtigoConhecimento>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(dados.Admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context),
            auditoria);

        await vincular.ExecutarAsync(dados.Chamado.Id, artigo.Id);

        var remover = new RemoverArtigoConhecimentoDoChamadoUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<ChamadoArtigoConhecimento>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(dados.Admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context),
            auditoria);

        await remover.ExecutarAsync(dados.Chamado.Id, artigo.Id);

        Assert.Contains(context.HistoricosChamado, x => x.Tipo == TipoHistoricoChamado.ArtigoConhecimentoVinculado);
        Assert.Contains(context.HistoricosChamado, x => x.Tipo == TipoHistoricoChamado.ArtigoConhecimentoDesvinculado);
        Assert.True(auditoria.Eventos.Count >= 2);
    }

    [Fact]
    public async Task SolicitanteNaoPodeVincularArtigoNoFluxoAdministrativo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);
        var artigo = await CriarArtigoAsync(context, dados.Admin, dados.Categoria.Id, "Documento restrito", StatusArtigoConhecimento.Publicado);

        var useCase = new VincularArtigoConhecimentoAoChamadoUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<BaseConhecimentoArtigo>(context),
            PortalUseCasesTestFactory.Repo<ChamadoArtigoConhecimento>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(dados.Solicitante, "Solicitante")),
            PortalUseCasesTestFactory.Uow(context));

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => useCase.ExecutarAsync(dados.Chamado.Id, artigo.Id));
    }

    private static VincularArtigoConhecimentoAoChamadoUseCase CriarUseCaseVinculo(SGXSistemaChamadoDbContext context, Usuario usuario)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<BaseConhecimentoArtigo>(context),
            PortalUseCasesTestFactory.Repo<ChamadoArtigoConhecimento>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(usuario, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

    private static async Task<(Usuario Admin, Usuario Solicitante, CategoriaChamado Categoria, Chamado Chamado)> SeedAsync(SGXSistemaChamadoDbContext context)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            context,
            "Administrador BC",
            $"admin.bc.vinculo.{Guid.NewGuid():N}@empresa.com",
            TipoPerfil.Administrador);

        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            context,
            "Solicitante BC",
            $"sol.bc.vinculo.{Guid.NewGuid():N}@empresa.com",
            TipoPerfil.Solicitante);

        var departamento = new Departamento("Tecnologia BC", "TBC", null, "teste");
        context.Departamentos.Add(departamento);
        await context.SaveChangesAsync();

        var categoria = new CategoriaChamado("Base de conhecimento", null, departamento.Id, "teste");
        context.CategoriasChamado.Add(categoria);
        await context.SaveChangesAsync();

        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(
            context,
            solicitante,
            categoria,
            StatusChamadoEnum.Aberto,
            sufixoCodigo: "BC-001");

        return (admin, solicitante, categoria, chamado);
    }

    private static async Task<BaseConhecimentoArtigo> CriarArtigoAsync(
        SGXSistemaChamadoDbContext context,
        Usuario usuario,
        Guid categoriaId,
        string titulo,
        StatusArtigoConhecimento status)
    {
        var artigo = new BaseConhecimentoArtigo(
            titulo,
            $"{titulo.ToLowerInvariant().Replace(' ', '-')}-{Guid.NewGuid():N}",
            "Resumo de apoio",
            "Conteudo validado para atendimento.",
            categoriaId,
            status,
            VisibilidadeArtigoConhecimento.Atendente,
            "tag,base",
            usuario.Id,
            usuario.Login);

        context.BaseConhecimentoArtigos.Add(artigo);
        await context.SaveChangesAsync();
        return artigo;
    }

    private static async Task CriarVinculoAsync(
        SGXSistemaChamadoDbContext context,
        Guid chamadoId,
        Guid artigoId,
        Guid usuarioId,
        string? observacao = null)
    {
        context.ChamadosArtigosConhecimento.Add(new ChamadoArtigoConhecimento(
            chamadoId,
            artigoId,
            usuarioId,
            observacao,
            "teste"));
        await context.SaveChangesAsync();
    }
}
