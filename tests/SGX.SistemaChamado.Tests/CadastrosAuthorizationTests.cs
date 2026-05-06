using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class CadastrosAuthorizationTests
{
    [Fact]
    public async Task SolicitanteNaoAcessaCasosAdministrativosRestritos()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante", "sol.cad@empresa.com", TipoPerfil.Solicitante);

        var useCase = new CriarDepartamentoUseCase(
            PortalUseCasesTestFactory.Repo<Departamento>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(solicitante, "Solicitante")),
            PortalUseCasesTestFactory.Uow(context));

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => useCase.ExecutarAsync(
            new CriarDepartamentoRequest { Nome = "Depto Solicitante", Sigla = "DS", Descricao = "Teste" }));
    }

    [Fact]
    public async Task AtendentePodeConsultarCadastrosPermitidos()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var atendente = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Atendente", "aten.cad@empresa.com", TipoPerfil.Atendente);
        context.Departamentos.Add(new Departamento("Financeiro", "FIN", null, "teste"));
        await context.SaveChangesAsync();

        var useCase = new ListarDepartamentosAdminUseCase(
            PortalUseCasesTestFactory.Repo<Departamento>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(atendente, "Atendente")));

        var response = await useCase.ExecutarAsync(new FiltroCadastroRequest());
        Assert.NotEmpty(response.Items);
    }

    [Fact]
    public async Task AtendenteNaoPodeCriarEditarInativarCadastros()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var atendente = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Atendente", "aten.block@empresa.com", TipoPerfil.Atendente);
        var departamento = new Departamento("Operacoes", "OPE", null, "teste");
        context.Departamentos.Add(departamento);
        await context.SaveChangesAsync();

        var useCase = new InativarDepartamentoUseCase(
            PortalUseCasesTestFactory.Repo<Departamento>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(atendente, "Atendente")),
            PortalUseCasesTestFactory.Uow(context));

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => useCase.ExecutarAsync(departamento.Id));
    }

    [Fact]
    public async Task AdministradorPodeCriarEditarInativarCadastros()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "admin.cad@empresa.com", TipoPerfil.Administrador);

        var criarUseCase = new CriarDepartamentoUseCase(
            PortalUseCasesTestFactory.Repo<Departamento>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        var criado = await criarUseCase.ExecutarAsync(new CriarDepartamentoRequest { Nome = "Planejamento", Sigla = "PLN", Descricao = "Teste" });

        var inativarUseCase = new InativarDepartamentoUseCase(
            PortalUseCasesTestFactory.Repo<Departamento>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        var resultado = await inativarUseCase.ExecutarAsync(criado.Id);
        Assert.False(resultado.Ativo);
    }
}
