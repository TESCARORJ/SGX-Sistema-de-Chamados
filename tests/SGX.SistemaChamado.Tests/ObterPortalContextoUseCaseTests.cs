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

        var prioridadeInativa = context.PrioridadesChamado.First();
        prioridadeInativa.Desativar("teste");

        context.Departamentos.Add(departamento);
        context.Departamentos.Add(departamentoInativo);
        context.Usuarios.Add(solicitante);
        context.CategoriasChamado.Add(categoria);
        context.CategoriasChamado.Add(categoriaInativa);
        await context.SaveChangesAsync();

        var arquivosOptions = Options.Create(new ArquivosOptions
        {
            TamanhoMaximoBytes = 2 * 1024 * 1024,
            ContentTypesPermitidos = ["application/pdf", "image/png"]
        });

        var useCase = new ObterPortalContextoUseCase(
            PortalUseCasesTestFactory.Repo<Departamento>(context),
            PortalUseCasesTestFactory.Repo<CategoriaChamado>(context),
            PortalUseCasesTestFactory.Repo<PrioridadeChamado>(context),
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
        Assert.DoesNotContain(response.Prioridades, x => x.Id == prioridadeInativa.Id);
    }
}
