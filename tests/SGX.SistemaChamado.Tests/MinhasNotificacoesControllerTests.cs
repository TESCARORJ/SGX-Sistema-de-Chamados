using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGX.SistemaChamado.Api.Controllers;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces.Notificacoes;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class MinhasNotificacoesControllerTests
{
    [Fact]
    public void DeveExigirAuthorizeENaoReceberUsuarioId()
    {
        var authorize = typeof(MinhasNotificacoesController).GetCustomAttribute<AuthorizeAttribute>();
        Assert.NotNull(authorize);

        var parametros = typeof(MinhasNotificacoesController)
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .SelectMany(x => x.GetParameters())
            .Select(x => x.Name)
            .Where(x => x is not null)
            .ToArray();

        Assert.DoesNotContain(parametros, x => string.Equals(x, "usuarioId", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ControllerNaoDeveDependerDeDbContext()
    {
        var tiposConstrutor = typeof(MinhasNotificacoesController)
            .GetConstructors()
            .Single()
            .GetParameters()
            .Select(x => x.ParameterType)
            .ToArray();

        Assert.DoesNotContain(typeof(SGXSistemaChamadoDbContext), tiposConstrutor);
    }

    [Fact]
    public async Task DeveOrquestrarListagemDetalheMarcacaoEContagem()
    {
        var notificacaoId = Guid.NewGuid();
        var listar = new FakeListarMinhasNotificacoesUseCase();
        var obter = new FakeObterMinhaNotificacaoUseCase(notificacaoId);
        var marcarLida = new FakeMarcarMinhaNotificacaoComoLidaUseCase(notificacaoId);
        var marcarNaoLida = new FakeMarcarMinhaNotificacaoComoNaoLidaUseCase(notificacaoId);
        var contar = new FakeContarMinhasNotificacoesNaoLidasUseCase();

        var controller = new MinhasNotificacoesController(listar, obter, marcarLida, marcarNaoLida, contar);

        var listarResult = Assert.IsType<OkObjectResult>(await controller.Listar());
        var obterResult = Assert.IsType<OkObjectResult>(await controller.Obter(notificacaoId, default));
        var lidaResult = Assert.IsType<OkObjectResult>(await controller.MarcarComoLida(notificacaoId, default));
        var naoLidaResult = Assert.IsType<OkObjectResult>(await controller.MarcarComoNaoLida(notificacaoId, default));
        var contarResult = Assert.IsType<OkObjectResult>(await controller.ContarNaoLidas(default));

        Assert.IsType<ListarMinhasNotificacoesResponse>(listarResult.Value);
        Assert.IsType<MinhaNotificacaoDetalheResponse>(obterResult.Value);
        Assert.IsType<AlterarLeituraNotificacaoResponse>(lidaResult.Value);
        Assert.IsType<AlterarLeituraNotificacaoResponse>(naoLidaResult.Value);
        Assert.IsType<ContagemMinhasNotificacoesNaoLidasResponse>(contarResult.Value);
    }

    [Fact]
    public async Task DeveRetornar404QuandoUseCaseInformarNaoEncontrada()
    {
        var controller = new MinhasNotificacoesController(
            new FakeListarMinhasNotificacoesUseCase(),
            new FakeObterMinhaNotificacaoUseCase(Guid.Empty, lancarNaoEncontrado: true),
            new FakeMarcarMinhaNotificacaoComoLidaUseCase(Guid.Empty, lancarNaoEncontrado: true),
            new FakeMarcarMinhaNotificacaoComoNaoLidaUseCase(Guid.Empty, lancarNaoEncontrado: true),
            new FakeContarMinhasNotificacoesNaoLidasUseCase());

        Assert.IsType<NotFoundObjectResult>(await controller.Obter(Guid.NewGuid(), default));
        Assert.IsType<NotFoundObjectResult>(await controller.MarcarComoLida(Guid.NewGuid(), default));
        Assert.IsType<NotFoundObjectResult>(await controller.MarcarComoNaoLida(Guid.NewGuid(), default));
    }

    private sealed class FakeListarMinhasNotificacoesUseCase : IListarMinhasNotificacoesUseCase
    {
        public Task<ListarMinhasNotificacoesResponse> ExecutarAsync(ListarMinhasNotificacoesRequest request, CancellationToken cancellationToken = default)
            => Task.FromResult(new ListarMinhasNotificacoesResponse(
                [new MinhaNotificacaoResumoResponse(Guid.NewGuid(), TipoEventoNotificacao.EventoChamado, "Assunto", "Resumo", DateTime.UtcNow, false, null, null)],
                1,
                20,
                1,
                1,
                1));
    }

    private sealed class FakeObterMinhaNotificacaoUseCase(Guid notificacaoId, bool lancarNaoEncontrado = false) : IObterMinhaNotificacaoUseCase
    {
        public Task<MinhaNotificacaoDetalheResponse> ExecutarAsync(Guid notificacaoIdRequest, CancellationToken cancellationToken = default)
        {
            if (lancarNaoEncontrado)
            {
                throw new KeyNotFoundException("Notificacao nao encontrada.");
            }

            return Task.FromResult(new MinhaNotificacaoDetalheResponse(
                notificacaoId == Guid.Empty ? notificacaoIdRequest : notificacaoId,
                TipoEventoNotificacao.EventoChamado,
                "Assunto",
                "Conteudo",
                DateTime.UtcNow,
                false,
                null,
                null,
                "corr"));
        }
    }

    private sealed class FakeMarcarMinhaNotificacaoComoLidaUseCase(Guid notificacaoId, bool lancarNaoEncontrado = false) : IMarcarMinhaNotificacaoComoLidaUseCase
    {
        public Task<AlterarLeituraNotificacaoResponse> ExecutarAsync(Guid notificacaoIdRequest, CancellationToken cancellationToken = default)
        {
            if (lancarNaoEncontrado)
            {
                throw new KeyNotFoundException("Notificacao nao encontrada.");
            }

            return Task.FromResult(new AlterarLeituraNotificacaoResponse(
                notificacaoId == Guid.Empty ? notificacaoIdRequest : notificacaoId,
                true,
                DateTime.UtcNow,
                true));
        }
    }

    private sealed class FakeMarcarMinhaNotificacaoComoNaoLidaUseCase(Guid notificacaoId, bool lancarNaoEncontrado = false) : IMarcarMinhaNotificacaoComoNaoLidaUseCase
    {
        public Task<AlterarLeituraNotificacaoResponse> ExecutarAsync(Guid notificacaoIdRequest, CancellationToken cancellationToken = default)
        {
            if (lancarNaoEncontrado)
            {
                throw new KeyNotFoundException("Notificacao nao encontrada.");
            }

            return Task.FromResult(new AlterarLeituraNotificacaoResponse(
                notificacaoId == Guid.Empty ? notificacaoIdRequest : notificacaoId,
                false,
                null,
                true));
        }
    }

    private sealed class FakeContarMinhasNotificacoesNaoLidasUseCase : IContarMinhasNotificacoesNaoLidasUseCase
    {
        public Task<ContagemMinhasNotificacoesNaoLidasResponse> ExecutarAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(new ContagemMinhasNotificacoesNaoLidasResponse(3));
    }
}
