using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConcluirConfiguracaoEfMigrationEstruturalNotificacoesSprint6Roadmap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000140"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777725"),
                columns: new[]
                {
                    "atencao_tecnica",
                    "evidencia_implementacao",
                    "pendencias_tecnicas",
                    "percentual_implementacao",
                    "proxima_acao",
                    "situacao_atual"
                },
                values: new object[]
                {
                    "Validar testes de dominio e metadados persistentes antes de avancar para servicos de geracao e processamento.",
                    "Diagnostico consolidado; modelagem de dominio da Notificacao; contrato EventoCandidatoNotificacao; configuracao EF explicita; DbSet no contexto; migration estrutural CriarEstruturaNotificacaoSprint6; indices e constraints; testes de configuracao EF; documentacao em docs/roadmap/sprint-6-configuracao-ef-migration-notificacoes.md; sem comportamento funcional de notificacao.",
                    "Executar e consolidar testes do dominio e da estrutura persistente da notificacao antes de iniciar servicos de geracao, resolucao de destinatarios, processamento e entrega.",
                    25,
                    "Testar dominio e estrutura persistente de notificacoes.",
                    "Entidade Notificacao persistida no EF Core com tabela propria, FKs opcionais, indices e constraints, ainda sem comportamento funcional de geracao, processamento ou envio."
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000140"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777725"),
                columns: new[]
                {
                    "atencao_tecnica",
                    "evidencia_implementacao",
                    "pendencias_tecnicas",
                    "percentual_implementacao",
                    "proxima_acao",
                    "situacao_atual"
                },
                values: new object[]
                {
                    "Criar mapeamento EF, unicidade de idempotencia e base persistente sem acoplar envio, templates e preferencias antes da hora.",
                    "Diagnostico consolidado em docs/roadmap/sprint-6-diagnostico-notificacoes-itsm.md; entidade de dominio Notificacao; contrato EventoCandidatoNotificacao; testes de dominio/contrato; documentacao em docs/roadmap/sprint-6-modelagem-notificacao-contrato-eventos.md; sem persistencia estrutural nesta etapa.",
                    "Criar configuracao EF e migration estrutural da notificacao, preservando separacao entre dominio, persistencia, processamento, envio, templates, preferencias e resolucao de destinatarios.",
                    19,
                    "Criar configuracao EF e migration estrutural de notificacoes.",
                    "Nucleo de dominio da notificacao e contrato interno de evento modelados, ainda sem persistencia estrutural no EF."
                });
        }
    }
}
