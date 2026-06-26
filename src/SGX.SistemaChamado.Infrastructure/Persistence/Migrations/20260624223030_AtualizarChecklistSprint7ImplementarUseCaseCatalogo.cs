using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AtualizarChecklistSprint7ImplementarUseCaseCatalogo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000916"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777718"),
                columns: new[] { "pendencias_tecnicas", "percentual_implementacao", "proxima_acao" },
                values: new object[] { "Aplicar classificacao vinda do catalogo no backend, aplicar grupo responsavel e SLA por servico, introduzir formulario por servico com persistencia das respostas e concluir a integracao guiada sem romper incidentes e fluxos legados.", 56, "Aplicar classificacao vinda do catalogo no backend." });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000916"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777718"),
                columns: new[] { "pendencias_tecnicas", "percentual_implementacao", "proxima_acao" },
                values: new object[] { "Implementar use case dedicado da jornada guiada quando trouxer ganho real sem duplicar regras, aplicar grupo responsavel e SLA por servico, introduzir formulario por servico com persistencia das respostas e concluir a integracao guiada sem romper incidentes e fluxos legados.", 54, "Implementar use case dedicado de abertura de requisicao de servico via catalogo." });
        }
    }
}
