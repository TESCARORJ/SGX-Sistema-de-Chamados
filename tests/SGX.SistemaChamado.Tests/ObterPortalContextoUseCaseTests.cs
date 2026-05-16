using Microsoft.Extensions.Options;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Options;
using SGX.SistemaChamado.Application.UseCases.Portal;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Tests;

public sealed class ObterPortalContextoUseCaseTests
{
    [Fact]
    public async Task DeveRetornarConfiguracaoDeAnexosQuandoDisponivel()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var departamento = new Departamento("Tecnologia", "TI", null, "teste");
        var departamentoInativo = new Departamento("Inativo", "INV", null, "teste");
        departamentoInativo.Desativar("teste");
        var solicitante = new Usuario("Solicitante", "solicitante.contexto@empresa.com", "sol.contexto", "teste", departamento.Id);
        var categoria = new CategoriaChamado("Sistemas", null, departamento.Id, "teste");
        var categoriaInativa = new CategoriaChamado("Inativa", null, departamento.Id, "teste");
        categoriaInativa.Desativar("teste");
        var subcategoria = new SubcategoriaChamado(categoria.Id, "Acesso", null, "teste");
        var subcategoriaInativa = new SubcategoriaChamado(categoria.Id, "Inativa", null, "teste");
        subcategoriaInativa.Desativar("teste");
        var tipoSolicitacao = new TipoSolicitacao("Incidente", null, "teste");
        var tipoSolicitacaoInativo = new TipoSolicitacao("Inativo", null, "teste");
        tipoSolicitacaoInativo.Desativar("teste");
        var localUnidade = new LocalUnidade("Matriz", null, null, "teste");
        var localUnidadeInativo = new LocalUnidade("Inativo", null, null, "teste");
        localUnidadeInativo.Desativar("teste");

        var prioridadeInativa = context.PrioridadesChamado.First();
        prioridadeInativa.Desativar("teste");

        context.Departamentos.Add(departamento);
        context.Departamentos.Add(departamentoInativo);
        context.Usuarios.Add(solicitante);
        context.CategoriasChamado.Add(categoria);
        context.CategoriasChamado.Add(categoriaInativa);
        context.SubcategoriasChamado.Add(subcategoria);
        context.SubcategoriasChamado.Add(subcategoriaInativa);
        context.TiposSolicitacao.Add(tipoSolicitacao);
        context.TiposSolicitacao.Add(tipoSolicitacaoInativo);
        context.LocaisUnidade.Add(localUnidade);
        context.LocaisUnidade.Add(localUnidadeInativo);
        await context.SaveChangesAsync();

        var arquivosOptions = Options.Create(new ArquivosOptions
        {
            TamanhoMaximoBytes = 2 * 1024 * 1024,
            ContentTypesPermitidos = ["application/pdf", "image/png"]
        });

        var useCase = new ObterPortalContextoUseCase(
            PortalUseCasesTestFactory.Repo<Departamento>(context),
            PortalUseCasesTestFactory.Repo<CategoriaChamado>(context),
            PortalUseCasesTestFactory.Repo<SubcategoriaChamado>(context),
            PortalUseCasesTestFactory.Repo<PrioridadeChamado>(context),
            PortalUseCasesTestFactory.Repo<TipoSolicitacao>(context),
            PortalUseCasesTestFactory.Repo<LocalUnidade>(context),
            PortalUseCasesTestFactory.Repo<StatusChamado>(context),
            new FakeUsuarioContextoAplicacaoService(new UsuarioContextoAplicacao(
                solicitante.Id,
                solicitante.Nome,
                solicitante.Email,
                solicitante.Login,
                ["Solicitante"])),
            arquivosOptions);

        var response = await useCase.ExecutarAsync();

        Assert.NotNull(response.ConfiguracaoAnexos);
        Assert.Equal(2 * 1024 * 1024, response.ConfiguracaoAnexos!.TamanhoMaximoBytes);
        Assert.Equal(2, response.ConfiguracaoAnexos.TiposPermitidos.Count);
        Assert.Contains(response.Departamentos, x => x.Id == departamento.Id);
        Assert.DoesNotContain(response.Departamentos, x => x.Id == departamentoInativo.Id);
        Assert.Contains(response.Categorias, x => x.Id == categoria.Id);
        Assert.DoesNotContain(response.Categorias, x => x.Id == categoriaInativa.Id);
        Assert.Contains(response.Subcategorias, x => x.Id == subcategoria.Id);
        Assert.DoesNotContain(response.Subcategorias, x => x.Id == subcategoriaInativa.Id);
        Assert.DoesNotContain(response.Prioridades, x => x.Id == prioridadeInativa.Id);
        Assert.Contains(response.TiposSolicitacao, x => x.Id == tipoSolicitacao.Id);
        Assert.DoesNotContain(response.TiposSolicitacao, x => x.Id == tipoSolicitacaoInativo.Id);
        Assert.Contains(response.LocaisUnidade, x => x.Id == localUnidade.Id);
        Assert.DoesNotContain(response.LocaisUnidade, x => x.Id == localUnidadeInativo.Id);
    }
}
