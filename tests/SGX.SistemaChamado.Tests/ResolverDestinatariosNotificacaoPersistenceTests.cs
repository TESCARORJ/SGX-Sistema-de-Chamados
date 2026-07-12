using Microsoft.EntityFrameworkCore;
using Npgsql;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.UseCases.Notificacoes;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

[Collection("NotificacoesPersistenceRelational")]
public sealed class ResolverDestinatariosNotificacaoPersistenceTests : IClassFixture<NotificacaoPersistenceDatabaseFixture>
{
    private const string CriadoPorTeste = "test.destinatarios.persistence";
    private readonly NotificacaoPersistenceDatabaseFixture _fixture;

    public ResolverDestinatariosNotificacaoPersistenceTests(NotificacaoPersistenceDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task DeveResolverSolicitanteEResponsavelPersistidosSemCriarNotificacao()
    {
        await ResetAsync();
        await using var context = _fixture.CreateContext();
        var cenario = await CriarCenarioChamadoAsync(context, "sol-resp");
        cenario.Chamado.AtribuirResponsavel(cenario.Responsavel.Id, CriadoPorTeste);
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context);

        var response = await useCase.ExecutarAsync(new ResolverDestinatariosNotificacaoRequest(
            CriarEvento(cenario.Chamado.Id),
            [
                TipoParticipacaoDestinatarioNotificacao.Solicitante,
                TipoParticipacaoDestinatarioNotificacao.ResponsavelAtual
            ]));

        Assert.Equal(2, response.Destinatarios.Count);
        Assert.Equal(0, await context.Notificacoes.CountAsync());
    }

    [Fact]
    public async Task DeveResolverGrupoTecnicoComUsuariosElegiveisEDeduplicar()
    {
        await ResetAsync();
        await using var context = _fixture.CreateContext();
        var cenario = await CriarCenarioChamadoAsync(context, "grupo");
        var grupo = new GrupoTecnico("Grupo Relacional", null, CriadoPorTeste);
        context.GruposTecnicos.Add(grupo);
        await context.SaveChangesAsync();

        cenario.Chamado.DefinirGrupoTecnico(grupo.Id, CriadoPorTeste);
        cenario.Chamado.AtribuirResponsavel(cenario.Responsavel.Id, CriadoPorTeste);
        var inativo = await CriarUsuarioAsync(context, "grupo.inativo");
        inativo.AlterarSituacao(SituacaoUsuario.Inativo, CriadoPorTeste);

        context.MembrosGruposTecnicos.Add(new MembroGrupoTecnico(grupo.Id, cenario.Responsavel.Id, CriadoPorTeste));
        context.MembrosGruposTecnicos.Add(new MembroGrupoTecnico(grupo.Id, cenario.Administrador.Id, CriadoPorTeste));
        context.MembrosGruposTecnicos.Add(new MembroGrupoTecnico(grupo.Id, inativo.Id, CriadoPorTeste));
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context);
        var response = await useCase.ExecutarAsync(new ResolverDestinatariosNotificacaoRequest(
            CriarEvento(cenario.Chamado.Id),
            [
                TipoParticipacaoDestinatarioNotificacao.ResponsavelAtual,
                TipoParticipacaoDestinatarioNotificacao.MembroGrupoTecnico
            ]));

        Assert.Equal(2, response.Destinatarios.Count);
        Assert.DoesNotContain(response.Destinatarios, x => x.UsuarioId == inativo.Id);
        Assert.Equal(0, await context.Notificacoes.CountAsync());
    }

    [Fact]
    public async Task DeveResolverAprovadoresELerPerfilSemPersistirAlteracoes()
    {
        await ResetAsync();
        await using var context = _fixture.CreateContext();
        var cenario = await CriarCenarioChamadoAsync(context, "aprov-perfil");
        var perfilNotificacao = new PerfilAcesso("Perfil Destinatario Relacional", TipoPerfil.Atendente, null, CriadoPorTeste);
        context.PerfisAcesso.Add(perfilNotificacao);
        await context.SaveChangesAsync();
        context.UsuariosPerfisAcesso.Add(new UsuarioPerfilAcesso(cenario.Responsavel.Id, perfilNotificacao.Id, CriadoPorTeste));

        var aprovacao = new AprovacaoChamado(
            cenario.Chamado.Id,
            TipoOrigemAprovacaoChamado.Categoria,
            cenario.Administrador.Id,
            CriadoPorTeste,
            solicitanteId: cenario.Solicitante.Id,
            aprovadorId: cenario.Responsavel.Id);

        var instancia = new InstanciaAprovacaoChamado(
            chamadoId: cenario.Chamado.Id,
            solicitanteId: cenario.Solicitante.Id,
            origem: OrigemInstanciaAprovacaoChamado.RegraMotor,
            tipoFluxoAprovacao: TipoFluxoAprovacao.Simples,
            efeitoOperacional: EfeitoOperacionalRegraAprovacao.ExigirAprovacaoEBloquearAvanco,
            escopoRegra: EscopoRegraAprovacao.AtendimentoChamado,
            tipoRegra: TipoRegraAprovacao.Combinada,
            exigeAprovacao: true,
            bloqueante: true,
            tipoResolucaoAprovador: TipoResolucaoAprovadorRegraAprovacao.AprovadorEspecifico,
            criadoPorUsuarioId: cenario.Administrador.Id,
            criadoPor: CriadoPorTeste,
            categoriaId: cenario.Categoria.Id,
            aprovadorEspecificoUsuarioId: cenario.Responsavel.Id,
            aprovadorResolvidoUsuarioId: cenario.Responsavel.Id,
            regraNomeSnapshot: "Regra relacional",
            regraVersaoSnapshot: 1,
            regraCriterioSnapshot: "Criterio");

        context.AprovacoesChamado.Add(aprovacao);
        context.InstanciasAprovacaoChamado.Add(instancia);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context);
        var response = await useCase.ExecutarAsync(new ResolverDestinatariosNotificacaoRequest(
            CriarEvento(cenario.Chamado.Id),
            [
                TipoParticipacaoDestinatarioNotificacao.AprovadorLegado,
                TipoParticipacaoDestinatarioNotificacao.AprovadorInstancia,
                TipoParticipacaoDestinatarioNotificacao.PerfilAcesso
            ],
            AprovacaoChamadoId: aprovacao.Id,
            InstanciaAprovacaoChamadoId: instancia.Id,
            PerfilAcessoId: perfilNotificacao.Id));

        var destinatario = Assert.Single(response.Destinatarios);
        Assert.Equal(cenario.Responsavel.Id, destinatario.UsuarioId);
        Assert.Equal(0, await context.Notificacoes.CountAsync());
    }

    [Fact]
    public async Task DeveManterBancoSemAlteracoesQuandoNaoHouverElegiveis()
    {
        await ResetAsync();
        await using var context = _fixture.CreateContext();
        var cenario = await CriarCenarioChamadoAsync(context, "sem-elegivel");
        cenario.Chamado.AtribuirResponsavel(null, CriadoPorTeste);
        await context.SaveChangesAsync();
        var useCase = CriarUseCase(context);

        var antesNotificacoes = await context.Notificacoes.CountAsync();
        var antesUsuarios = await context.Usuarios.CountAsync(x => x.CriadoPor == CriadoPorTeste);

        var response = await useCase.ExecutarAsync(new ResolverDestinatariosNotificacaoRequest(
            CriarEvento(cenario.Chamado.Id),
            [TipoParticipacaoDestinatarioNotificacao.ResponsavelAtual]));

        Assert.Empty(response.Destinatarios);
        Assert.NotEmpty(response.Avisos);
        Assert.Equal(antesNotificacoes, await context.Notificacoes.CountAsync());
        Assert.Equal(antesUsuarios, await context.Usuarios.CountAsync(x => x.CriadoPor == CriadoPorTeste));
    }

    private ResolverDestinatariosNotificacaoUseCase CriarUseCase(SGXSistemaChamadoDbContext context)
        => new(
            new Infrastructure.Repositories.Repository<Chamado>(context),
            new Infrastructure.Repositories.Repository<Usuario>(context),
            new Infrastructure.Repositories.Repository<AprovacaoChamado>(context),
            new Infrastructure.Repositories.Repository<InstanciaAprovacaoChamado>(context),
            new Infrastructure.Repositories.Repository<MembroGrupoTecnico>(context),
            new Infrastructure.Repositories.Repository<PerfilAcesso>(context),
            new Infrastructure.Repositories.Repository<UsuarioPerfilAcesso>(context));

    private async Task ResetAsync()
    {
        await using var context = _fixture.CreateContext();
        await context.Notificacoes.ExecuteDeleteAsync();
        await context.InstanciasAprovacaoChamado.Where(x => x.CriadoPor == CriadoPorTeste).ExecuteDeleteAsync();
        await context.AprovacoesChamado.Where(x => x.CriadoPor == CriadoPorTeste).ExecuteDeleteAsync();
        await context.MembrosGruposTecnicos.Where(x => x.CriadoPor == CriadoPorTeste).ExecuteDeleteAsync();
        await context.UsuariosPerfisAcesso.Where(x => x.CriadoPor == CriadoPorTeste).ExecuteDeleteAsync();
        await context.PerfisAcessoPermissoes.Where(x => x.CriadoPor == CriadoPorTeste).ExecuteDeleteAsync();
        await context.PerfisAcesso.Where(x => x.CriadoPor == CriadoPorTeste).ExecuteDeleteAsync();
        await context.Chamados.Where(x => x.CriadoPor == CriadoPorTeste).ExecuteDeleteAsync();
        await context.GruposTecnicos.Where(x => x.CriadoPor == CriadoPorTeste).ExecuteDeleteAsync();
        await context.CategoriasChamado.Where(x => x.CriadoPor == CriadoPorTeste).ExecuteDeleteAsync();
        await context.Usuarios.Where(x => x.CriadoPor == CriadoPorTeste).ExecuteDeleteAsync();
    }

    private async Task<(Usuario Administrador, Usuario Solicitante, Usuario Responsavel, CategoriaChamado Categoria, Chamado Chamado)> CriarCenarioChamadoAsync(
        SGXSistemaChamadoDbContext context,
        string sufixo)
    {
        var administrador = await CriarUsuarioAsync(context, $"admin.{sufixo}", TipoPerfil.Administrador);
        var solicitante = await CriarUsuarioAsync(context, $"sol.{sufixo}", TipoPerfil.Solicitante);
        var responsavel = await CriarUsuarioAsync(context, $"resp.{sufixo}", TipoPerfil.Atendente);
        var categoria = new CategoriaChamado($"Categoria {sufixo}", null, null, CriadoPorTeste);
        context.CategoriasChamado.Add(categoria);
        await context.SaveChangesAsync();

        var prioridade = await context.PrioridadesChamado.OrderBy(x => x.CriadoEm).FirstAsync();
        var status = await context.StatusChamado.FirstAsync(x => x.Codigo == StatusChamadoEnum.EmAtendimento);
        var chamado = new Chamado(
            $"DST-{Guid.NewGuid():N}"[..12],
            $"Chamado {sufixo}",
            "Chamado relacional para resolucao de destinatarios.",
            solicitante.Id,
            categoria.Id,
            prioridade.Id,
            status.Id,
            OrigemChamado.Portal,
            CriadoPorTeste);

        context.Chamados.Add(chamado);
        await context.SaveChangesAsync();
        return (administrador, solicitante, responsavel, categoria, chamado);
    }

    private async Task<Usuario> CriarUsuarioAsync(SGXSistemaChamadoDbContext context, string prefixo, TipoPerfil? perfil = null)
    {
        var usuario = new Usuario(
            $"Usuario {prefixo}"[..Math.Min(30, $"Usuario {prefixo}".Length)],
            $"{prefixo}.{Guid.NewGuid():N}@teste.local",
            $"{prefixo}.{Guid.NewGuid():N}"[..20],
            CriadoPorTeste);
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        if (perfil.HasValue)
        {
            var perfilAcesso = await context.PerfisAcesso.FirstAsync(x => x.TipoPerfil == perfil.Value);
            context.UsuariosPerfisAcesso.Add(new UsuarioPerfilAcesso(usuario.Id, perfilAcesso.Id, CriadoPorTeste));
            await context.SaveChangesAsync();
        }

        return usuario;
    }

    private static EventoCandidatoNotificacao CriarEvento(Guid chamadoId)
        => new(
            TipoEventoNotificacao.EventoChamado,
            chamadoId,
            null,
            new DateTime(2026, 6, 21, 13, 0, 0, DateTimeKind.Utc),
            "corr-resolucao-relacional",
            $"idem-resolucao-{Guid.NewGuid():N}",
            new Dictionary<string, string>());
}
