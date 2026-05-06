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

        Assert.Equal(dados.Atendente.Id, context.Chamados.Single().ResponsavelId);
        Assert.Equal(dados.Chamado.Id, response.Id);
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

        Assert.Contains(context.HistoricosChamado, x => x.Tipo == TipoHistoricoChamado.ResponsavelAlterado);
    }

    private static AtribuirChamadoUseCase CriarUseCase(SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context, UsuarioContextoAplicacao contexto)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<Usuario>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            SlaTestFactory.CriarService(context),
            new FakeUsuarioContextoAplicacaoService(contexto),
            PortalUseCasesTestFactory.Uow(context));

    private static async Task<(Chamado Chamado, Usuario Atendente, Usuario SomenteSolicitante, UsuarioContextoAplicacao AdminContexto)> SeedAsync(SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Admin", "admin@empresa.com", TipoPerfil.Administrador);
        var atendente = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Atendente", "aten@empresa.com", TipoPerfil.Atendente);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Solicitante", "sol@empresa.com", TipoPerfil.Solicitante);
        var somenteSolicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Sem Atendimento", "sem@empresa.com", TipoPerfil.Solicitante);

        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, "Infra");
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(context, solicitante, categoria, StatusChamadoEnum.Aberto, null, "ATR1");

        return (chamado, atendente, somenteSolicitante, AdminUseCasesTestFactory.Contexto(admin, "Administrador"));
    }
}

