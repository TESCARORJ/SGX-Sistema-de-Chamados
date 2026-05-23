using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SyncRoadmapBaseConhecimentoStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777710"),
                columns: new[] { "atencao_tecnica", "criterio_aceite", "decisao", "evidencia_implementacao", "impacto", "observacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual", "status", "status_implementacao", "status_tecnico" },
                values: new object[] { "Pendencias evolutivas: homologacao institucional com usuarios reais, evidencias com prints reais, testes E2E completos e evolucoes futuras como versionamento, workflow de aprovacao, anexos, avaliacao de utilidade, relatorios e busca semantica/IA.", "A tela do roadmap deve refletir Base de conhecimento como implementado funcionalmente, com homologacao funcional preparada e percentual de 90%.", 4, "- docs/BASE-CONHECIMENTO.md\n- docs/ROADMAP.md\n- docs/ROADMAP-ITSM.md\n- docs/CHECKLIST-HOMOLOGACAO-BASE-CONHECIMENTO.md\n- docs/evidencias/base-conhecimento/README.md", 1, "Sprints 1 a 6 implementadas e validadas tecnicamente em Release.", "- Coletar evidencias com prints reais.\n- Registrar aceite funcional institucional.", "- Homologacao institucional com usuarios reais.\n- Testes E2E completos, quando houver framework institucional.\n- Evolucoes futuras: versionamento, workflow de aprovacao, anexos, avaliacao de utilidade, relatorios e busca semantica/IA.", 90, "Executar homologacao institucional com usuarios reais e anexar evidencias formais.", "Fundacao tecnica, CRUD administrativo, consulta no portal, frontend administrativo, frontend do portal, integracao com chamados, auditoria, historico, testes backend/frontend, documentacao, checklist de homologacao e estrutura de evidencias implementados. Modulo validado em Release, com homologacao funcional preparada.", 2, 3, 4 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777710"),
                columns: new[] { "atencao_tecnica", "criterio_aceite", "decisao", "evidencia_implementacao", "impacto", "observacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual", "status", "status_implementacao", "status_tecnico" },
                values: new object[] { "Pode ser GAP assumido para evolucao", null, 2, null, 2, null, null, null, 0, null, "Nao ha evidencia forte", 4, 0, 0 });
        }
    }
}
