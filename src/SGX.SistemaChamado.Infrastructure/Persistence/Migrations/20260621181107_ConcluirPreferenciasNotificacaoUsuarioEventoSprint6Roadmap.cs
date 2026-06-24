using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConcluirPreferenciasNotificacaoUsuarioEventoSprint6Roadmap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000905"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777725"),
                columns: new[] { "evidencia_implementacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual" },
                values: new object[] { "Notificacao persistida, geracao idempotente, resolucao de destinatarios, templates com materializacao segura e PreferenciaNotificacaoUsuario persistente com definicao, avaliacao, fallback permissivo, elegibilidade e testes de dominio/aplicacao/persistencia; sem envio funcional.", "Validar cenarios reais de recebimento por solicitante, responsavel, aprovador, grupo tecnico e perfil apos existirem processamento, entrega por canal e integracao aos eventos ITSM.", "Implementar processamento e controle de tentativas de entrega, depois entrega por canal e API de consulta sem misturar preferencia, geracao, processamento e envio.", 56, "Implementar processamento e controle de tentativas de entrega", "Preferencias de notificacao por usuario, evento e canal implementadas com avaliacao, fallback permissivo, elegibilidade, definicao explicita e testes de dominio/aplicacao/persistencia, ainda sem entrega por canal." });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000905"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777725"),
                columns: new[] { "evidencia_implementacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual" },
                values: new object[] { "Diagnostico consolidado; modelagem de dominio da Notificacao; contrato EventoCandidatoNotificacao; configuracao EF explicita; DbSet no contexto; migration estrutural CriarEstruturaNotificacaoSprint6; servico GerarNotificacaoUseCase com request, response e validator; idempotencia por chave unica global; resolucao de destinatarios por participacao e perfil com DTOs, validator, use case, elegibilidade, deduplicacao e avisos; TemplateNotificacao persistente com configuracao EF e migration estrutural CriarEstruturaTemplateNotificacaoSprint6; materializacao de assunto e conteudo com placeholders seguros, validacao de variaveis e vigencia; testes de dominio, configuracao, persistencia relacional, validator e use case; documentacao em docs/roadmap/sprint-6-templates-materializacao-conteudo.md; sem envio funcional de notificacao.", "Validar cenarios reais de recebimento por solicitante, responsavel, aprovador, grupo tecnico e perfil apos existirem preferencias, integracao aos eventos ITSM e entrega por canal.", "Implementar preferencias de notificacao por usuario e evento, depois processamento, entrega por canal e API de consulta sem misturar responsabilidades nem antecipar envio.", 50, "Implementar preferencias de notificacao por usuario e evento.", "Templates persistentes e materializacao de conteudo implementados com versao, vigencia, variaveis permitidas, renderizacao segura e testes de dominio/aplicacao/persistencia, ainda sem preferencias nem entrega por canal." });
        }
    }
}
