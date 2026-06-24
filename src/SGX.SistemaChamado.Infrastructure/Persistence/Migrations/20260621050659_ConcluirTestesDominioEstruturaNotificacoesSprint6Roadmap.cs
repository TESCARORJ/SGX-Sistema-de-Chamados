using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConcluirTestesDominioEstruturaNotificacoesSprint6Roadmap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000901"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777725"),
                columns: new[] { "atencao_tecnica", "evidencia_implementacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual" },
                values: new object[] { "Preservar a separacao entre persistencia validada, geracao idempotente, resolucao de destinatarios, processamento e entrega ao evoluir a Sprint 6.", "Diagnostico consolidado; modelagem de dominio da Notificacao; contrato EventoCandidatoNotificacao; configuracao EF explicita; DbSet no contexto; migration estrutural CriarEstruturaNotificacaoSprint6; testes de dominio; testes do contrato; testes de configuracao EF; testes relacionais PostgreSQL; validacao de migration, indices, constraints, FKs e idempotencia; documentacao em docs/roadmap/sprint-6-testes-dominio-estrutura-notificacoes.md; sem comportamento funcional de notificacao.", "Implementar o servico de geracao idempotente de notificacoes, seguido por resolucao de destinatarios, processamento, entrega por canal e API de consulta sem misturar responsabilidades.", 31, "Criar servico de geracao idempotente de notificacoes.", "Dominio, contrato, configuracao EF e estrutura persistente da notificacao validados tecnicamente, com testes automatizados e sem comportamento funcional de geracao, processamento ou envio." });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000901"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777725"),
                columns: new[] { "atencao_tecnica", "evidencia_implementacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual" },
                values: new object[] { "Validar testes de dominio e metadados persistentes antes de avancar para servicos de geracao e processamento.", "Diagnostico consolidado; modelagem de dominio da Notificacao; contrato EventoCandidatoNotificacao; configuracao EF explicita; DbSet no contexto; migration estrutural CriarEstruturaNotificacaoSprint6; indices e constraints; testes de configuracao EF; documentacao em docs/roadmap/sprint-6-configuracao-ef-migration-notificacoes.md; sem comportamento funcional de notificacao.", "Executar e consolidar testes do dominio e da estrutura persistente da notificacao antes de iniciar servicos de geracao, resolucao de destinatarios, processamento e entrega.", 25, "Testar dominio e estrutura persistente de notificacoes.", "Entidade Notificacao persistida no EF Core com tabela propria, FKs opcionais, indices e constraints, ainda sem comportamento funcional de geracao, processamento ou envio." });
        }
    }
}
