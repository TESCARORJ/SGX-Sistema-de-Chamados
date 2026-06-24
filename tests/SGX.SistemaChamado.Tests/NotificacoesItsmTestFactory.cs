using Microsoft.Extensions.Logging.Abstractions;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Notificacoes;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

internal static class NotificacoesItsmTestFactory
{
    public static ProcessarEventoCandidatoNotificacaoUseCase CriarOrquestrador(
        SGXSistemaChamadoDbContext context,
        UsuarioContextoAplicacao usuarioContexto)
    {
        return new ProcessarEventoCandidatoNotificacaoUseCase(
            new ResolverDestinatariosNotificacaoUseCase(
                PortalUseCasesTestFactory.Repo<Chamado>(context),
                PortalUseCasesTestFactory.Repo<Usuario>(context),
                PortalUseCasesTestFactory.Repo<AprovacaoChamado>(context),
                PortalUseCasesTestFactory.Repo<InstanciaAprovacaoChamado>(context),
                PortalUseCasesTestFactory.Repo<MembroGrupoTecnico>(context),
                PortalUseCasesTestFactory.Repo<PerfilAcesso>(context),
                PortalUseCasesTestFactory.Repo<UsuarioPerfilAcesso>(context)),
            new AvaliarPreferenciaNotificacaoUseCase(
                PortalUseCasesTestFactory.Repo<Usuario>(context),
                PortalUseCasesTestFactory.Repo<PreferenciaNotificacaoUsuario>(context)),
            new MaterializarConteudoNotificacaoUseCase(
                PortalUseCasesTestFactory.Repo<TemplateNotificacao>(context)),
            new GerarNotificacaoUseCase(
                PortalUseCasesTestFactory.Repo<Notificacao>(context),
                PortalUseCasesTestFactory.Uow(context),
                new FakeUsuarioContextoAplicacaoService(usuarioContexto)),
            NullLogger<ProcessarEventoCandidatoNotificacaoUseCase>.Instance);
    }

    public static async Task CriarTemplatesPadraoChamadoAsync(
        SGXSistemaChamadoDbContext context,
        Guid usuarioCriadorId,
        string criadoPor = "test.notificacoes")
    {
        context.Set<TemplateNotificacao>().Add(CriarTemplate(
            usuarioCriadorId,
            CanalNotificacao.Sistema,
            criadoPor));
        context.Set<TemplateNotificacao>().Add(CriarTemplate(
            usuarioCriadorId,
            CanalNotificacao.Email,
            criadoPor));

        await context.SaveChangesAsync();
    }

    public static TemplateNotificacao CriarTemplate(
        Guid usuarioCriadorId,
        CanalNotificacao canal,
        string criadoPor = "test.notificacoes")
    {
        var assunto = canal == CanalNotificacao.Email
            ? "[{{evento.nome}}] Chamado {{chamado.codigo}}"
            : null;
        var conteudo = "Evento: {{evento.nome}}\nChamado: {{chamado.codigo}} - {{chamado.titulo}}\nStatus: {{chamado.status}}\nDetalhe: {{evento.descricao}}";

        return new TemplateNotificacao(
            $"template-{canal.ToString().ToLowerInvariant()}-evento-chamado",
            TipoEventoNotificacao.EventoChamado,
            canal,
            1,
            conteudo,
            usuarioCriadorId,
            criadoPor,
            [
                "chamado.codigo",
                "chamado.status",
                "chamado.titulo",
                "evento.descricao",
                "evento.nome",
                "evento.ocorrido_em",
                "responsavel.nome",
                "solicitante.nome",
                "solucao.resumo",
                "status.codigo",
                "status.nome"
            ],
            assunto,
            $"Template generico do canal {canal} para eventos priorizados de chamado.",
            new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            null);
    }
}
