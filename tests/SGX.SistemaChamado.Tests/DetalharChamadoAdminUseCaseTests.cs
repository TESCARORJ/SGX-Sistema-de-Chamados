using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class DetalharChamadoAdminUseCaseTests
{
    [Fact]
    public async Task AdminConsegueDetalharChamadoAbertoNoPortalComHistoricoEAnexo()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var dados = await SeedAsync(context);

        var useCase = new DetalharChamadoAdminUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            new FakeUsuarioContextoAplicacaoService(dados.ContextoAdmin));

        var response = await useCase.ExecutarAsync(dados.Chamado.Id);

        Assert.Equal("Portal", response.Origem);
        Assert.Equal("Aberto", response.Status);
        Assert.Equal(dados.Solicitante.Nome, response.Solicitante.Nome);
        Assert.Contains(response.Historico, item => item.Descricao == "Chamado criado pelo portal");
        Assert.Contains(response.Anexos, item => item.NomeArquivo == "evidencia.pdf");
    }

    private static async Task<(Chamado Chamado, Usuario Solicitante, UsuarioContextoAplicacao ContextoAdmin)> SeedAsync(SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "admin.det@empresa.com", TipoPerfil.Administrador);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante", "sol.det@empresa.com", TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Infra");
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.Aberto, null, "DET");

        var historicoCriacao = new HistoricoChamado(
            chamado.Id,
            TipoHistoricoChamado.Criado,
            "Chamado criado pelo portal",
            solicitante.Id,
            "teste");

        var anexo = new AnexoChamado(
            chamado.Id,
            "evidencia.pdf",
            "evidencia_armazenada.pdf",
            "application/pdf",
            1024,
            "storage/anexos/evidencia_armazenada.pdf",
            solicitante.Id,
            "teste");

        context.HistoricosChamado.Add(historicoCriacao);
        context.AnexosChamado.Add(anexo);
        await context.SaveChangesAsync();

        return (chamado, solicitante, AdminUseCasesTestFactory.Contexto(admin, "Administrador"));
    }
}
