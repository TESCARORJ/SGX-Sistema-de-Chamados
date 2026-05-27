using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Services;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class AcoesChamadoServiceTests
{
    [Fact]
    public async Task DeveRetornarAcoesParaIncidenteAberto()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var (chamado, usuario) = await CriarCenarioAsync(context, NaturezaChamadoEnum.Incidente, StatusChamadoEnum.Aberto);
        var service = CriarService();

        var acoes = service.ObterAcoesDisponiveis(chamado, usuario);

        Assert.Contains(AcaoChamadoEnum.Assumir, acoes);
        Assert.Contains(AcaoChamadoEnum.Atribuir, acoes);
        Assert.Contains(AcaoChamadoEnum.AlterarStatus, acoes);
        Assert.Contains(AcaoChamadoEnum.AlterarPrioridade, acoes);
        Assert.Contains(AcaoChamadoEnum.AlterarCategoria, acoes);
        Assert.Contains(AcaoChamadoEnum.Encerrar, acoes);
    }

    [Fact]
    public async Task DeveRetornarAcoesParaRequisicaoAberta()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var (chamado, usuario) = await CriarCenarioAsync(context, NaturezaChamadoEnum.Requisicao, StatusChamadoEnum.Aberto);
        var service = CriarService();

        var acoes = service.ObterAcoesDisponiveis(chamado, usuario);

        Assert.Contains(AcaoChamadoEnum.Assumir, acoes);
        Assert.Contains(AcaoChamadoEnum.Atribuir, acoes);
        Assert.Contains(AcaoChamadoEnum.AlterarStatus, acoes);
        Assert.Contains(AcaoChamadoEnum.AlterarPrioridade, acoes);
        Assert.Contains(AcaoChamadoEnum.AlterarCategoria, acoes);
        Assert.Contains(AcaoChamadoEnum.Encerrar, acoes);
    }

    [Fact]
    public async Task DeveRetornarAcoesParaMudancaAbertaSemFluxoDeAprovacaoCompleto()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var (chamado, usuario) = await CriarCenarioAsync(context, NaturezaChamadoEnum.Mudanca, StatusChamadoEnum.Aberto);
        var service = CriarService();

        var acoes = service.ObterAcoesDisponiveis(chamado, usuario);

        Assert.Contains(AcaoChamadoEnum.Assumir, acoes);
        Assert.Contains(AcaoChamadoEnum.Atribuir, acoes);
        Assert.Contains(AcaoChamadoEnum.AlterarStatus, acoes);
        Assert.Contains(AcaoChamadoEnum.AlterarPrioridade, acoes);
        Assert.Contains(AcaoChamadoEnum.AlterarCategoria, acoes);
        Assert.Contains(AcaoChamadoEnum.Encerrar, acoes);
        Assert.DoesNotContain(AcaoChamadoEnum.AprovarMudanca, acoes);
    }

    [Fact]
    public async Task DeveRetornarAcoesParaProblemaAbertoSemFluxoDeCausaRaizCompleto()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var (chamado, usuario) = await CriarCenarioAsync(context, NaturezaChamadoEnum.Problema, StatusChamadoEnum.Aberto);
        var service = CriarService();

        var acoes = service.ObterAcoesDisponiveis(chamado, usuario);

        Assert.Contains(AcaoChamadoEnum.Assumir, acoes);
        Assert.Contains(AcaoChamadoEnum.Atribuir, acoes);
        Assert.Contains(AcaoChamadoEnum.AlterarStatus, acoes);
        Assert.Contains(AcaoChamadoEnum.AlterarPrioridade, acoes);
        Assert.Contains(AcaoChamadoEnum.AlterarCategoria, acoes);
        Assert.Contains(AcaoChamadoEnum.Encerrar, acoes);
        Assert.DoesNotContain(AcaoChamadoEnum.RegistrarCausaRaiz, acoes);
    }

    [Fact]
    public async Task DeveRetornarAcoesParaEventoAlertaAberto()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var (chamado, usuario) = await CriarCenarioAsync(context, NaturezaChamadoEnum.EventoAlerta, StatusChamadoEnum.Aberto);
        var service = CriarService();

        var acoes = service.ObterAcoesDisponiveis(chamado, usuario);

        Assert.Contains(AcaoChamadoEnum.Assumir, acoes);
        Assert.Contains(AcaoChamadoEnum.Atribuir, acoes);
        Assert.Contains(AcaoChamadoEnum.AlterarStatus, acoes);
        Assert.Contains(AcaoChamadoEnum.Encerrar, acoes);
        Assert.DoesNotContain(AcaoChamadoEnum.AlterarPrioridade, acoes);
        Assert.DoesNotContain(AcaoChamadoEnum.AlterarCategoria, acoes);
        Assert.DoesNotContain(AcaoChamadoEnum.CorrelacionarEvento, acoes);
    }

    [Fact]
    public async Task DeveRetornarAcoesParaTarefaOperacionalAberta()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var (chamado, usuario) = await CriarCenarioAsync(context, NaturezaChamadoEnum.TarefaOperacional, StatusChamadoEnum.Aberto);
        var service = CriarService();

        var acoes = service.ObterAcoesDisponiveis(chamado, usuario);

        Assert.Contains(AcaoChamadoEnum.Assumir, acoes);
        Assert.Contains(AcaoChamadoEnum.Atribuir, acoes);
        Assert.Contains(AcaoChamadoEnum.AlterarStatus, acoes);
        Assert.Contains(AcaoChamadoEnum.Encerrar, acoes);
        Assert.DoesNotContain(AcaoChamadoEnum.AlterarPrioridade, acoes);
        Assert.DoesNotContain(AcaoChamadoEnum.AlterarCategoria, acoes);
        Assert.DoesNotContain(AcaoChamadoEnum.ConcluirTarefa, acoes);
    }

    [Fact]
    public async Task ChamadoEncerradoNaoDevePermitirAcoesOperacionais()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var (chamado, usuario) = await CriarCenarioAsync(context, NaturezaChamadoEnum.Incidente, StatusChamadoEnum.Encerrado);
        var service = CriarService();

        var acoes = service.ObterAcoesDisponiveis(chamado, usuario);

        Assert.DoesNotContain(AcaoChamadoEnum.AlterarStatus, acoes);
        Assert.DoesNotContain(AcaoChamadoEnum.AlterarPrioridade, acoes);
        Assert.DoesNotContain(AcaoChamadoEnum.AlterarCategoria, acoes);
        Assert.DoesNotContain(AcaoChamadoEnum.Encerrar, acoes);
    }

    [Fact]
    public async Task DeveExibirReabrirQuandoStatusFinalEPermissaoPermitir()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var atendente = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Atendente Reabertura", "atendente.reabrir@sgx.local", TipoPerfil.Atendente);
        var (chamado, _) = await CriarCenarioAsync(context, NaturezaChamadoEnum.Requisicao, StatusChamadoEnum.Encerrado);
        var service = CriarService();
        var usuario = AdminUseCasesTestFactory.Contexto(atendente, ["Atendente"], ["Chamados.Reabrir"]);

        var acoes = service.ObterAcoesDisponiveis(chamado, usuario);

        Assert.Contains(AcaoChamadoEnum.Reabrir, acoes);
    }

    [Fact]
    public async Task DeveExibirReabrirQuandoStatusFinalEspecificoEPermissaoPermitir()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var atendente = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Atendente Reabertura 2", "atendente.reabrir2@sgx.local", TipoPerfil.Atendente);
        var (chamado, _) = await CriarCenarioAsync(context, NaturezaChamadoEnum.EventoAlerta, StatusChamadoEnum.Tratado);
        var service = CriarService();
        var usuario = AdminUseCasesTestFactory.Contexto(atendente, ["Atendente"], ["Chamados.Reabrir"]);

        var acoes = service.ObterAcoesDisponiveis(chamado, usuario);

        Assert.Contains(AcaoChamadoEnum.Reabrir, acoes);
        Assert.DoesNotContain(AcaoChamadoEnum.AlterarStatus, acoes);
    }

    [Fact]
    public async Task AlterarStatusDependeDaRegraDeStatusPorNatureza()
    {
        using var context = AdminUseCasesTestFactory.CriarContexto();
        var (chamado, usuario) = await CriarCenarioAsync(
            context,
            NaturezaChamadoEnum.EventoAlerta,
            StatusChamadoEnum.AguardandoSolicitante);
        var service = CriarService();

        var acoes = service.ObterAcoesDisponiveis(chamado, usuario);

        Assert.DoesNotContain(AcaoChamadoEnum.AlterarStatus, acoes);
    }

    private static IAcoesChamadoService CriarService()
        => new AcoesChamadoService(new FluxoStatusChamadoService());

    private static async Task<(Chamado Chamado, UsuarioContextoAplicacao Usuario)> CriarCenarioAsync(
        SGXSistemaChamadoDbContext context,
        NaturezaChamadoEnum natureza,
        StatusChamadoEnum status)
    {
        var admin = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            context,
            $"Admin {natureza}",
            $"{natureza.ToString().ToLowerInvariant()}@sgx.local",
            TipoPerfil.Administrador);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            context,
            $"Solicitante {natureza}",
            $"sol.{natureza.ToString().ToLowerInvariant()}@sgx.local",
            TipoPerfil.Solicitante);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, $"Categoria {natureza}");
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(
            context,
            solicitante,
            categoria,
            status,
            null,
            sufixoCodigo: $"{(int)natureza}{(int)status}",
            naturezaChamado: natureza);

        var chamadoCompleto = await context.Chamados
            .Include(x => x.Status)
            .Include(x => x.Aprovacoes)
            .FirstAsync(x => x.Id == chamado.Id);

        var usuario = AdminUseCasesTestFactory.Contexto(admin, ["Administrador"]);
        return (chamadoCompleto, usuario);
    }
}
