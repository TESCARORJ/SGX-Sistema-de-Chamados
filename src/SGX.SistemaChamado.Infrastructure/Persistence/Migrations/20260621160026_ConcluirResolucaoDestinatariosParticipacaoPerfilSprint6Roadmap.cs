using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConcluirResolucaoDestinatariosParticipacaoPerfilSprint6Roadmap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000903"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777725"),
                columns: new[] { "atencao_tecnica", "evidencia_implementacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual" },
                values: new object[] { "Preservar a separacao entre resolucao de destinatarios, materializacao de conteudo, geracao idempotente, processamento e entrega ao evoluir a Sprint 6.", "Diagnostico consolidado; modelagem de dominio da Notificacao; contrato EventoCandidatoNotificacao; configuracao EF explicita; DbSet no contexto; migration estrutural CriarEstruturaNotificacaoSprint6; servico GerarNotificacaoUseCase com request, response e validator; idempotencia por chave unica global; resolucao de destinatarios por participacao e perfil com DTOs, validator, use case, elegibilidade, deduplicacao e avisos; testes unitarios, validator e persistencia relacional; documentacao em docs/roadmap/sprint-6-resolucao-destinatarios-participacao-perfil.md; sem envio funcional de notificacao.", "Validar cenarios reais de recebimento por solicitante, responsavel, aprovador, grupo tecnico e perfil apos existir materializacao de conteudo e integracao aos eventos ITSM.", "Modelar templates e materializacao de conteudo, depois preferencias, processamento, entrega por canal e API de consulta sem misturar responsabilidades nem antecipar envio.", 44, "Modelar templates e materializacao de conteudo.", "Resolucao de destinatarios internos implementada com participacoes reais, elegibilidade, deduplicacao, avisos e testes unitarios/relacionais, ainda sem templates nem entrega por canal." });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000903"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777725"),
                columns: new[] { "atencao_tecnica", "evidencia_implementacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual" },
                values: new object[] { "Preservar a separacao entre geracao idempotente, resolucao de destinatarios, processamento e entrega ao evoluir a Sprint 6.", "Diagnostico consolidado; modelagem de dominio da Notificacao; contrato EventoCandidatoNotificacao; configuracao EF explicita; DbSet no contexto; migration estrutural CriarEstruturaNotificacaoSprint6; servico GerarNotificacaoUseCase com request, response e validator; idempotencia por chave unica global; tratamento de concorrencia; testes de dominio, validator, use case e persistencia relacional; documentacao em docs/roadmap/sprint-6-servico-geracao-idempotente-notificacoes.md; sem envio funcional de notificacao.", "Validar recebimento por perfil, observador, aprovador e grupo tecnico apos existir modulo persistente e integracao real aos eventos ITSM.", "Implementar a resolucao de destinatarios por participacao e perfil, seguida por templates, preferencias, processamento, entrega por canal e API de consulta sem misturar responsabilidades.", 38, "Implementar resolucao de destinatarios por participacao e perfil.", "Geracao idempotente da notificacao persistente implementada com validator, contratos, persistencia transacional, tratamento de concorrencia e testes automatizados, ainda sem entrega por canal." });
        }
    }
}
