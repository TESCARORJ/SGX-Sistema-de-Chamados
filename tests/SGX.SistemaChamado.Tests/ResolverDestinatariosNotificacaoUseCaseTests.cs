using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.UseCases.Notificacoes;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class ResolverDestinatariosNotificacaoUseCaseTests
{
    [Fact]
    public async Task DeveResolverSolicitanteDoChamado()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioChamadoAsync(context, "solicitante");
        var useCase = CriarUseCase(context);

        var response = await useCase.ExecutarAsync(CriarRequest(
            dados.Chamado.Id,
            [TipoParticipacaoDestinatarioNotificacao.Solicitante]));

        var destinatario = Assert.Single(response.Destinatarios);
        Assert.Equal(dados.Solicitante.Id, destinatario.UsuarioId);
        Assert.Equal([TipoParticipacaoDestinatarioNotificacao.Solicitante], destinatario.Origens);
        Assert.Empty(response.Avisos);
        Assert.Empty(context.Notificacoes);
    }

    [Fact]
    public async Task DeveResolverResponsavelAtualQuandoExistir()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioChamadoAsync(context, "responsavel");
        dados.Chamado.AtribuirResponsavel(dados.Responsavel!.Id, "teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context);

        var response = await useCase.ExecutarAsync(CriarRequest(
            dados.Chamado.Id,
            [TipoParticipacaoDestinatarioNotificacao.ResponsavelAtual]));

        var destinatario = Assert.Single(response.Destinatarios);
        Assert.Equal(dados.Responsavel.Id, destinatario.UsuarioId);
        Assert.Equal([TipoParticipacaoDestinatarioNotificacao.ResponsavelAtual], destinatario.Origens);
    }

    [Fact]
    public async Task DeveResolverUsuarioOriginadorQuandoSolicitado()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioChamadoAsync(context, "originador");
        var useCase = CriarUseCase(context);

        var response = await useCase.ExecutarAsync(CriarRequest(
            dados.Chamado.Id,
            [TipoParticipacaoDestinatarioNotificacao.UsuarioOriginador],
            usuarioOriginadorId: dados.Responsavel!.Id));

        var destinatario = Assert.Single(response.Destinatarios);
        Assert.Equal(dados.Responsavel.Id, destinatario.UsuarioId);
    }

    [Fact]
    public async Task DeveExcluirUsuarioOriginadorQuandoConfigurado()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioChamadoAsync(context, "excluir-originador");
        dados.Chamado.AtribuirResponsavel(dados.Solicitante.Id, "teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context);

        var response = await useCase.ExecutarAsync(CriarRequest(
            dados.Chamado.Id,
            [
                TipoParticipacaoDestinatarioNotificacao.Solicitante,
                TipoParticipacaoDestinatarioNotificacao.UsuarioOriginador
            ],
            usuarioOriginadorId: dados.Solicitante.Id,
            excluirUsuarioOriginador: true));

        Assert.Empty(response.Destinatarios);
    }

    [Fact]
    public async Task DeveResolverAprovadorLegadoReal()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioChamadoAsync(context, "aprov-legado");
        var aprovacao = new AprovacaoChamado(
            dados.Chamado.Id,
            TipoOrigemAprovacaoChamado.Categoria,
            dados.Administrador.Id,
            "teste",
            solicitanteId: dados.Solicitante.Id,
            aprovadorId: dados.Responsavel!.Id);
        context.AprovacoesChamado.Add(aprovacao);
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context);

        var response = await useCase.ExecutarAsync(CriarRequest(
            dados.Chamado.Id,
            [TipoParticipacaoDestinatarioNotificacao.AprovadorLegado],
            aprovacaoChamadoId: aprovacao.Id));

        var destinatario = Assert.Single(response.Destinatarios);
        Assert.Equal(dados.Responsavel.Id, destinatario.UsuarioId);
        Assert.Equal([TipoParticipacaoDestinatarioNotificacao.AprovadorLegado], destinatario.Origens);
    }

    [Fact]
    public async Task DeveResolverAprovadorDaInstanciaQuandoHouverUsuarioResolvido()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioChamadoAsync(context, "aprov-instancia");
        var instancia = new InstanciaAprovacaoChamado(
            chamadoId: dados.Chamado.Id,
            solicitanteId: dados.Solicitante.Id,
            origem: OrigemInstanciaAprovacaoChamado.RegraMotor,
            tipoFluxoAprovacao: TipoFluxoAprovacao.Simples,
            efeitoOperacional: EfeitoOperacionalRegraAprovacao.ExigirAprovacaoEBloquearAvanco,
            escopoRegra: EscopoRegraAprovacao.AtendimentoChamado,
            tipoRegra: TipoRegraAprovacao.Combinada,
            exigeAprovacao: true,
            bloqueante: true,
            tipoResolucaoAprovador: TipoResolucaoAprovadorRegraAprovacao.AprovadorEspecifico,
            criadoPorUsuarioId: dados.Administrador.Id,
            criadoPor: "teste",
            categoriaId: dados.Categoria.Id,
            aprovadorEspecificoUsuarioId: dados.Responsavel!.Id,
            aprovadorResolvidoUsuarioId: dados.Responsavel.Id,
            regraNomeSnapshot: "Regra de teste",
            regraVersaoSnapshot: 1,
            regraCriterioSnapshot: "Critero");
        context.InstanciasAprovacaoChamado.Add(instancia);
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context);

        var response = await useCase.ExecutarAsync(CriarRequest(
            dados.Chamado.Id,
            [TipoParticipacaoDestinatarioNotificacao.AprovadorInstancia],
            instanciaAprovacaoChamadoId: instancia.Id));

        var destinatario = Assert.Single(response.Destinatarios);
        Assert.Equal(dados.Responsavel.Id, destinatario.UsuarioId);
    }

    [Fact]
    public async Task DeveResolverGrupoTecnicoComDeduplicacaoEMultiplasOrigens()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioChamadoAsync(context, "grupo");
        var grupo = new GrupoTecnico("Service Desk Teste", null, "teste");
        context.GruposTecnicos.Add(grupo);
        await context.SaveChangesAsync();

        dados.Chamado.DefinirGrupoTecnico(grupo.Id, "teste");
        dados.Chamado.AtribuirResponsavel(dados.Responsavel!.Id, "teste");
        await context.SaveChangesAsync();

        context.MembrosGruposTecnicos.Add(new MembroGrupoTecnico(grupo.Id, dados.Responsavel.Id, "teste"));
        context.MembrosGruposTecnicos.Add(new MembroGrupoTecnico(grupo.Id, dados.Administrador.Id, "teste"));
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context);
        var response = await useCase.ExecutarAsync(CriarRequest(
            dados.Chamado.Id,
            [
                TipoParticipacaoDestinatarioNotificacao.ResponsavelAtual,
                TipoParticipacaoDestinatarioNotificacao.MembroGrupoTecnico
            ]));

        Assert.Equal(2, response.Destinatarios.Count);
        var responsavel = response.Destinatarios.Single(x => x.UsuarioId == dados.Responsavel!.Id);
        Assert.Equal(
            [
                TipoParticipacaoDestinatarioNotificacao.ResponsavelAtual,
                TipoParticipacaoDestinatarioNotificacao.MembroGrupoTecnico
            ],
            responsavel.Origens);
        Assert.Equal(
            response.Destinatarios.OrderBy(x => x.Nome, StringComparer.OrdinalIgnoreCase).Select(x => x.UsuarioId),
            response.Destinatarios.Select(x => x.UsuarioId));
    }

    [Fact]
    public async Task DeveResolverUsuariosPorPerfilQuandoHouverVinculoSeguro()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioChamadoAsync(context, "perfil");
        var perfil = new PerfilAcesso("Perfil Notificacao Interna", TipoPerfil.Atendente, null, "teste");
        context.PerfisAcesso.Add(perfil);
        await context.SaveChangesAsync();
        context.UsuariosPerfisAcesso.Add(new UsuarioPerfilAcesso(dados.Responsavel!.Id, perfil.Id, "teste"));
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context);

        var response = await useCase.ExecutarAsync(CriarRequest(
            dados.Chamado.Id,
            [TipoParticipacaoDestinatarioNotificacao.PerfilAcesso],
            perfilAcessoId: perfil.Id));

        var destinatarios = response.Destinatarios.Select(x => x.UsuarioId).ToArray();
        Assert.Contains(dados.Responsavel.Id, destinatarios);
    }

    [Fact]
    public async Task DeveIgnorarUsuarioInativoEVinculoInativo()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioChamadoAsync(context, "elegibilidade");
        var grupo = new GrupoTecnico("Grupo Elegibilidade", null, "teste");
        context.GruposTecnicos.Add(grupo);
        await context.SaveChangesAsync();

        var ativo = dados.Responsavel!;
        var inativo = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(context, "Usuario Inativo", "inativo@sgx.local", TipoPerfil.Atendente);
        inativo.AlterarSituacao(SituacaoUsuario.Inativo, "teste");
        var membroInativo = new MembroGrupoTecnico(grupo.Id, inativo.Id, "teste");
        membroInativo.Inativar("teste");

        context.MembrosGruposTecnicos.Add(new MembroGrupoTecnico(grupo.Id, ativo.Id, "teste"));
        context.MembrosGruposTecnicos.Add(membroInativo);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context);
        var response = await useCase.ExecutarAsync(CriarRequest(
            dados.Chamado.Id,
            [TipoParticipacaoDestinatarioNotificacao.MembroGrupoTecnico],
            grupoTecnicoId: grupo.Id));

        var destinatario = Assert.Single(response.Destinatarios);
        Assert.Equal(ativo.Id, destinatario.UsuarioId);
    }

    [Fact]
    public async Task DeveRetornarVazioComAvisoQuandoNaoHouverElegivel()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioChamadoAsync(context, "vazio");
        dados.Chamado.AtribuirResponsavel(null, "teste");
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context);

        var response = await useCase.ExecutarAsync(CriarRequest(
            dados.Chamado.Id,
            [TipoParticipacaoDestinatarioNotificacao.ResponsavelAtual]));

        Assert.Empty(response.Destinatarios);
        Assert.NotEmpty(response.Avisos);
    }

    [Fact]
    public async Task DeveRespeitarCancellationToken()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var dados = await CriarCenarioChamadoAsync(context, "cancelamento");
        var useCase = CriarUseCase(context);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            useCase.ExecutarAsync(CriarRequest(
                dados.Chamado.Id,
                [TipoParticipacaoDestinatarioNotificacao.Solicitante]), cts.Token));
    }

    [Fact]
    public async Task DeveLancarValidationExceptionQuandoRequestInvalido()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var useCase = CriarUseCase(context);

        await Assert.ThrowsAsync<ValidationException>(() =>
            useCase.ExecutarAsync(new ResolverDestinatariosNotificacaoRequest(
                CriarEvento(null),
                [],
                null,
                null,
                null,
                null,
                false)));
    }

    private static ResolverDestinatariosNotificacaoUseCase CriarUseCase(SGXSistemaChamadoDbContext context)
        => new(
            PortalUseCasesTestFactory.Repo<Chamado>(context),
            PortalUseCasesTestFactory.Repo<Usuario>(context),
            PortalUseCasesTestFactory.Repo<AprovacaoChamado>(context),
            PortalUseCasesTestFactory.Repo<InstanciaAprovacaoChamado>(context),
            PortalUseCasesTestFactory.Repo<MembroGrupoTecnico>(context),
            PortalUseCasesTestFactory.Repo<PerfilAcesso>(context),
            PortalUseCasesTestFactory.Repo<UsuarioPerfilAcesso>(context));

    private static ResolverDestinatariosNotificacaoRequest CriarRequest(
        Guid? chamadoId,
        IReadOnlyCollection<TipoParticipacaoDestinatarioNotificacao> participacoes,
        Guid? usuarioOriginadorId = null,
        Guid? aprovacaoChamadoId = null,
        Guid? instanciaAprovacaoChamadoId = null,
        Guid? grupoTecnicoId = null,
        Guid? perfilAcessoId = null,
        bool excluirUsuarioOriginador = false)
        => new(
            CriarEvento(chamadoId, usuarioOriginadorId),
            participacoes,
            aprovacaoChamadoId,
            instanciaAprovacaoChamadoId,
            grupoTecnicoId,
            perfilAcessoId,
            excluirUsuarioOriginador);

    private static EventoCandidatoNotificacao CriarEvento(Guid? chamadoId, Guid? usuarioOriginadorId = null)
        => new(
            TipoEventoNotificacao.EventoChamado,
            chamadoId,
            usuarioOriginadorId,
            new DateTime(2026, 6, 21, 12, 30, 0, DateTimeKind.Utc),
            "corr-resolver-001",
            $"idem-resolver-{Guid.NewGuid():N}",
            new Dictionary<string, string>());

    private static async Task<(Usuario Administrador, Usuario Solicitante, Usuario? Responsavel, CategoriaChamado Categoria, Chamado Chamado)> CriarCenarioChamadoAsync(
        SGXSistemaChamadoDbContext context,
        string sufixo)
    {
        var administrador = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            context,
            $"Administrador {sufixo}",
            $"admin.dest.{sufixo}@sgx.local",
            TipoPerfil.Administrador);
        var solicitante = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            context,
            $"Solicitante {sufixo}",
            $"sol.dest.{sufixo}@sgx.local",
            TipoPerfil.Solicitante);
        var responsavel = await AdminUseCasesTestFactory.CriarUsuarioComPerfilAsync(
            context,
            $"Responsavel {sufixo}",
            $"resp.dest.{sufixo}@sgx.local",
            TipoPerfil.Atendente);
        var categoria = await AdminUseCasesTestFactory.CriarCategoriaAsync(context, $"Categoria destinatarios {sufixo}");
        var chamado = await AdminUseCasesTestFactory.CriarChamadoAsync(
            context,
            solicitante,
            categoria,
            StatusChamadoEnum.EmAtendimento,
            sufixoCodigo: sufixo);

        return (administrador, solicitante, responsavel, categoria, chamado);
    }
}
