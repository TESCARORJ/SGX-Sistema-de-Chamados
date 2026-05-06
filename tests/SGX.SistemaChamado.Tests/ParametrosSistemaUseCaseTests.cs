using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class ParametrosSistemaUseCaseTests
{
    [Fact]
    public async Task CriaParametro()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "param.admin@empresa.com", TipoPerfil.Administrador);

        var useCase = new CriarParametroSistemaUseCase(
            PortalUseCasesTestFactory.Repo<ParametroSistema>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        var response = await useCase.ExecutarAsync(new CriarParametroSistemaRequest
        {
            Chave = "smtp.host",
            Valor = "mail.empresa.local",
            Descricao = "Host SMTP",
            Sensivel = false
        });

        Assert.Equal("smtp.host", response.Chave);
    }

    [Fact]
    public async Task RejeitaChaveDuplicada()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "param2.admin@empresa.com", TipoPerfil.Administrador);
        context.ParametrosSistema.Add(new ParametroSistema("jwt.audience", "sgx-api", null, false, "teste"));
        await context.SaveChangesAsync();

        var useCase = new CriarParametroSistemaUseCase(
            PortalUseCasesTestFactory.Repo<ParametroSistema>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecutarAsync(new CriarParametroSistemaRequest
        {
            Chave = "jwt.audience",
            Valor = "outro",
            Sensivel = false
        }));
    }

    [Fact]
    public async Task MascaraValorSensivelEmListagem()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "param3.admin@empresa.com", TipoPerfil.Administrador);
        context.ParametrosSistema.Add(new ParametroSistema("smtp.password", "Segredo123", null, true, "teste"));
        await context.SaveChangesAsync();

        var useCase = new ListarParametrosSistemaUseCase(
            PortalUseCasesTestFactory.Repo<ParametroSistema>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")));

        var response = await useCase.ExecutarAsync(new FiltroCadastroRequest());
        var item = response.Items.Single(x => x.Chave == "smtp.password");
        Assert.Equal("********", item.Valor);
    }

    [Fact]
    public async Task AtualizaParametro()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "param4.admin@empresa.com", TipoPerfil.Administrador);
        var parametro = new ParametroSistema("feature.x.enabled", "false", "Flag", false, "teste");
        context.ParametrosSistema.Add(parametro);
        await context.SaveChangesAsync();

        var useCase = new AtualizarParametroSistemaUseCase(
            PortalUseCasesTestFactory.Repo<ParametroSistema>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        var response = await useCase.ExecutarAsync(parametro.Id, new AtualizarParametroSistemaRequest
        {
            Chave = "feature.x.enabled",
            Valor = "true",
            Descricao = "Flag atualizada",
            Sensivel = false
        });

        Assert.Equal("true", response.Valor);
    }

    [Fact]
    public async Task InativaParametro()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "param5.admin@empresa.com", TipoPerfil.Administrador);
        var parametro = new ParametroSistema("cache.ttl", "300", null, false, "teste");
        context.ParametrosSistema.Add(parametro);
        await context.SaveChangesAsync();

        var useCase = new InativarParametroSistemaUseCase(
            PortalUseCasesTestFactory.Repo<ParametroSistema>(context),
            new FakeUsuarioContextoAplicacaoService(AdminUseCasesTestFactory.Contexto(admin, "Administrador")),
            PortalUseCasesTestFactory.Uow(context));

        var response = await useCase.ExecutarAsync(parametro.Id);
        Assert.False(response.Ativo);
    }
}
