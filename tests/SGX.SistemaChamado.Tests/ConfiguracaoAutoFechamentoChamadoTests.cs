using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Api.Authorization;
using SGX.SistemaChamado.Api.Controllers;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.DTOs.Chamados;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Chamados;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Application.UseCases.Chamados;
using SGX.SistemaChamado.Application.Validators;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;
using System.Reflection;

namespace SGX.SistemaChamado.Tests;

public sealed class ConfiguracaoAutoFechamentoChamadoUseCasesTests
{
    [Fact]
    public async Task Obtem_Configuracao_Atual_De_Prazo_De_Auto_Fechamento()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var useCase = CriarObterUseCase(context);

        var response = await useCase.ExecutarAsync();

        Assert.Equal(48, response.PrazoAutoFechamentoHoras);
        Assert.Equal(2m, response.PrazoAutoFechamentoDias);
        Assert.True(response.Ativo);
    }

    [Fact]
    public async Task Atualiza_Prazo_Com_Valor_Valido()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var auditoria = new FakeAuditoriaService();
        var useCase = CriarAtualizarUseCase(context, auditoria);

        var response = await useCase.ExecutarAsync(new AtualizarConfiguracaoAutoFechamentoChamadoRequest(
            96,
            "Ajuste administrativo para homologacao."
        ));

        var parametro = await context.ParametrosSistema.SingleAsync(
            x => x.Id == SeedData.ParametroPrazoAutoFechamentoChamadoId);

        Assert.Equal(96, response.PrazoAutoFechamentoHoras);
        Assert.Equal("96", parametro.Valor);
        Assert.NotNull(parametro.AtualizadoEm);
        Assert.Equal("admin", parametro.AtualizadoPor);
        Assert.Single(auditoria.Eventos);
        Assert.Equal("Configuracao administrativa do prazo de auto-fechamento de chamados atualizada.", auditoria.Eventos.Single().Descricao);
    }

    [Fact]
    public async Task Nao_Altera_Chamados_Ao_Atualizar_Configuracao()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var chamado = await CriarChamadoAsync(context, "CFG-001", StatusChamadoEnum.Resolvido);
        var resolvidoEm = new DateTime(2026, 6, 12, 8, 0, 0, DateTimeKind.Utc);
        DefinirPropriedade(chamado, nameof(Chamado.ResolvidoEm), resolvidoEm);
        await context.SaveChangesAsync();

        var useCase = CriarAtualizarUseCase(context);
        await useCase.ExecutarAsync(new AtualizarConfiguracaoAutoFechamentoChamadoRequest(
            120,
            null
        ));

        var atualizado = await context.Chamados.Include(x => x.Historicos).SingleAsync(x => x.Id == chamado.Id);
        Assert.Equal(SeedData.StatusResolvidoId, atualizado.StatusId);
        Assert.Equal(resolvidoEm, atualizado.ResolvidoEm);
        Assert.Null(atualizado.EncerradoEm);
        Assert.Empty(atualizado.Historicos);
    }

    [Fact]
    public async Task Politica_Usa_Prazo_Configurado_Quando_Request_Nao_E_Informado()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var chamado = await CriarChamadoAsync(context, "CFG-002", StatusChamadoEnum.Resolvido);
        var dataReferencia = new DateTime(2026, 6, 12, 12, 0, 0, DateTimeKind.Utc);
        DefinirPropriedade(chamado, nameof(Chamado.ResolvidoEm), dataReferencia.AddHours(-80));
        await context.SaveChangesAsync();

        var useCase = CriarFechamentoUseCase(context);

        var response = await useCase.ExecutarAsync(new FecharChamadosAutomaticamentePorPrazoAceiteRequest
        {
            DataReferencia = dataReferencia
        });

        var atualizado = await context.Chamados.SingleAsync(x => x.Id == chamado.Id);
        Assert.Equal(1, response.TotalFechados);
        Assert.Equal(SeedData.StatusEncerradoId, atualizado.StatusId);
    }

    [Fact]
    public async Task Politica_Respeita_Prazo_Explicito_Do_Request()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var chamado = await CriarChamadoAsync(context, "CFG-003", StatusChamadoEnum.Resolvido);
        var dataReferencia = new DateTime(2026, 6, 12, 12, 0, 0, DateTimeKind.Utc);
        DefinirPropriedade(chamado, nameof(Chamado.ResolvidoEm), dataReferencia.AddHours(-80));
        await context.SaveChangesAsync();

        var useCase = CriarFechamentoUseCase(context);

        var response = await useCase.ExecutarAsync(new FecharChamadosAutomaticamentePorPrazoAceiteRequest
        {
            DataReferencia = dataReferencia,
            PrazoAceiteHoras = 120
        });

        var atualizado = await context.Chamados.SingleAsync(x => x.Id == chamado.Id);
        Assert.Equal(0, response.TotalFechados);
        Assert.Equal(SeedData.StatusResolvidoId, atualizado.StatusId);
    }

    [Fact]
    public async Task Politica_Nao_Fecha_Chamado_Dentro_Do_Prazo_Configurado()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var chamado = await CriarChamadoAsync(context, "CFG-004", StatusChamadoEnum.Resolvido);
        var dataReferencia = new DateTime(2026, 6, 12, 12, 0, 0, DateTimeKind.Utc);
        DefinirPropriedade(chamado, nameof(Chamado.ResolvidoEm), dataReferencia.AddHours(-24));
        await context.SaveChangesAsync();

        var useCase = CriarFechamentoUseCase(context);

        var response = await useCase.ExecutarAsync(new FecharChamadosAutomaticamentePorPrazoAceiteRequest
        {
            DataReferencia = dataReferencia
        });

        var atualizado = await context.Chamados.SingleAsync(x => x.Id == chamado.Id);
        Assert.Equal(0, response.TotalFechados);
        Assert.Equal(SeedData.StatusResolvidoId, atualizado.StatusId);
    }

    [Fact]
    public void Rejeita_Prazo_Zero()
    {
        var validator = new AtualizarConfiguracaoAutoFechamentoChamadoRequestValidator();
        var validation = validator.Validate(new AtualizarConfiguracaoAutoFechamentoChamadoRequest(
            0,
            null
        ));

        Assert.False(validation.IsValid);
    }

    [Fact]
    public void Rejeita_Prazo_Negativo()
    {
        var validator = new AtualizarConfiguracaoAutoFechamentoChamadoRequestValidator();
        var validation = validator.Validate(new AtualizarConfiguracaoAutoFechamentoChamadoRequest(
            -1,
            null
        ));

        Assert.False(validation.IsValid);
    }

    [Fact]
    public void Rejeita_Prazo_Acima_Do_Maximo_Permitido()
    {
        var validator = new AtualizarConfiguracaoAutoFechamentoChamadoRequestValidator();
        var validation = validator.Validate(new AtualizarConfiguracaoAutoFechamentoChamadoRequest(
            721,
            null
        ));

        Assert.False(validation.IsValid);
    }

    private static IObterConfiguracaoAutoFechamentoChamadoUseCase CriarObterUseCase(
        Infrastructure.Persistence.SGXSistemaChamadoDbContext context)
        => new ObterConfiguracaoAutoFechamentoChamadoUseCase(
            PortalUseCasesTestFactory.Repo<ParametroSistema>(context),
            CriarUsuarioAdmin());

    private static IAtualizarConfiguracaoAutoFechamentoChamadoUseCase CriarAtualizarUseCase(
        Infrastructure.Persistence.SGXSistemaChamadoDbContext context,
        FakeAuditoriaService? auditoria = null)
        => new AtualizarConfiguracaoAutoFechamentoChamadoUseCase(
            PortalUseCasesTestFactory.Repo<ParametroSistema>(context),
            CriarUsuarioAdmin(),
            PortalUseCasesTestFactory.Uow(context),
            auditoria ?? new FakeAuditoriaService());

    private static IFecharChamadosAutomaticamentePorPrazoAceiteUseCase CriarFechamentoUseCase(
        Infrastructure.Persistence.SGXSistemaChamadoDbContext context)
        => new FecharChamadosAutomaticamentePorPrazoAceiteUseCase(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<StatusChamado>(context),
            PortalUseCasesTestFactory.Repo<HistoricoChamado>(context),
            PortalUseCasesTestFactory.Repo<ParametroSistema>(context),
            new FakeValidarBloqueioMovimentacaoAprovacaoPendenteUseCase(),
            PortalUseCasesTestFactory.Uow(context),
            new FakeAuditoriaService());

    private static IUsuarioContextoAplicacaoService CriarUsuarioAdmin()
        => new FakeUsuarioContextoAplicacaoService(new UsuarioContextoAplicacao(
            Guid.NewGuid(),
            "Administrador",
            "admin@sgx.local",
            "admin",
            ["Administrador"]));

    private static async Task<Chamado> CriarChamadoAsync(
        Infrastructure.Persistence.SGXSistemaChamadoDbContext context,
        string codigo,
        StatusChamadoEnum status)
    {
        var chamado = new Chamado(
            codigo,
            "Titulo teste",
            "Descricao teste",
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            ObterStatusId(status),
            OrigemChamado.Admin,
            "teste");

        await context.Chamados.AddAsync(chamado);
        await context.SaveChangesAsync();
        return chamado;
    }

    private static Guid ObterStatusId(StatusChamadoEnum status)
        => status switch
        {
            StatusChamadoEnum.Resolvido => SeedData.StatusResolvidoId,
            StatusChamadoEnum.Encerrado => SeedData.StatusEncerradoId,
            StatusChamadoEnum.Cancelado => SeedData.StatusCanceladoId,
            StatusChamadoEnum.EmAtendimento => SeedData.StatusEmAtendimentoId,
            _ => throw new InvalidOperationException($"Status nao mapeado para teste: {status}.")
        };

    private static void DefinirPropriedade<T>(Chamado chamado, string propriedade, T valor)
    {
        var propertyInfo = typeof(Chamado).GetProperty(
            propriedade,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        propertyInfo!.SetValue(chamado, valor);
    }

    private sealed class FakeValidarBloqueioMovimentacaoAprovacaoPendenteUseCase : IValidarBloqueioMovimentacaoAprovacaoPendenteUseCase
    {
        public Task<ValidarBloqueioMovimentacaoAprovacaoPendenteResponse> ExecutarAsync(
            ValidarBloqueioMovimentacaoAprovacaoPendenteRequest request,
            CancellationToken cancellationToken = default)
            => Task.FromResult(new ValidarBloqueioMovimentacaoAprovacaoPendenteResponse { Bloqueado = false });
    }
}

public sealed class ConfiguracaoAutoFechamentoChamadoApiTests
{
    [Fact]
    public async Task Get_Retorna_Configuracao_Atual()
    {
        var controller = new AdminChamadosConfiguracoesController(
            new FakeObterConfiguracaoAutoFechamentoChamadoUseCase(),
            new FakeAtualizarConfiguracaoAutoFechamentoChamadoUseCase(),
            new AtualizarConfiguracaoAutoFechamentoChamadoRequestValidator());

        var result = await controller.ObterAutoFechamento(CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        var payload = Assert.IsType<ObterConfiguracaoAutoFechamentoChamadoResponse>(ok.Value);
        Assert.Equal(48, payload.PrazoAutoFechamentoHoras);
    }

    [Fact]
    public async Task Put_Rejeita_Payload_Invalido()
    {
        var controller = new AdminChamadosConfiguracoesController(
            new FakeObterConfiguracaoAutoFechamentoChamadoUseCase(),
            new FakeAtualizarConfiguracaoAutoFechamentoChamadoUseCase(),
            new AtualizarConfiguracaoAutoFechamentoChamadoRequestValidator());

        var result = await controller.AtualizarAutoFechamento(
            new AtualizarConfiguracaoAutoFechamentoChamadoRequest(
                0,
                null
            ),
            CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void Endpoint_Put_Exige_Permissao_Administrativa()
    {
        var method = typeof(AdminChamadosConfiguracoesController)
            .GetMethod(nameof(AdminChamadosConfiguracoesController.AtualizarAutoFechamento));

        var authorize = method!.GetCustomAttribute<AuthorizeAttribute>();

        Assert.NotNull(authorize);
        Assert.Equal(PermissionPolicies.ParametrosGerenciar, authorize!.Policy);
    }

    private sealed class FakeObterConfiguracaoAutoFechamentoChamadoUseCase : IObterConfiguracaoAutoFechamentoChamadoUseCase
    {
        public Task<ObterConfiguracaoAutoFechamentoChamadoResponse> ExecutarAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(new ObterConfiguracaoAutoFechamentoChamadoResponse(
                PrazoAutoFechamentoHoras: 48,
                PrazoAutoFechamentoDias: 2m,
                Ativo: true,
                CriadoEm: SeedData.DataBase,
                CriadoPor: "seed.sistema",
                AtualizadoEm: null,
                AtualizadoPor: null));
    }

    private sealed class FakeAtualizarConfiguracaoAutoFechamentoChamadoUseCase : IAtualizarConfiguracaoAutoFechamentoChamadoUseCase
    {
        public Task<AtualizarConfiguracaoAutoFechamentoChamadoResponse> ExecutarAsync(
            AtualizarConfiguracaoAutoFechamentoChamadoRequest request,
            CancellationToken cancellationToken = default)
            => Task.FromResult(new AtualizarConfiguracaoAutoFechamentoChamadoResponse(
                PrazoAutoFechamentoHoras: request.PrazoAutoFechamentoHoras,
                PrazoAutoFechamentoDias: Math.Round(request.PrazoAutoFechamentoHoras / 24m, 2),
                Ativo: true,
                CriadoEm: SeedData.DataBase,
                CriadoPor: "seed.sistema",
                AtualizadoEm: SeedData.DataBase,
                AtualizadoPor: "admin",
                ObservacaoAlteracao: request.ObservacaoAlteracao));
    }
}
