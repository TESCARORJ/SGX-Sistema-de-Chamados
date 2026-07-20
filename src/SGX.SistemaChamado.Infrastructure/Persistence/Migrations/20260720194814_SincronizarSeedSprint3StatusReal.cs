using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SincronizarSeedSprint3StatusReal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777720"),
                columns: new[] { "evidencia_implementacao", "proxima_acao", "status", "status_implementacao", "status_tecnico" },
                values: new object[] { "Grupos tecnicos, membros, fila de atendimento e regras de direcionamento, assuncao e transferencia implementados em GrupoTecnico.cs, MembroGrupoTecnico.cs e FilaAtendimento.cs, com use cases de assumir, direcionar e transferir chamado; checklist tecnico consolidado em 51/54 e 129/129 testes comportamentais passando.", "Executar os 3 roteiros de homologacao pendentes (produtividade por grupo, visibilidade por fila, aceite final).", 3, 3, 3 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777720"),
                columns: new[] { "evidencia_implementacao", "proxima_acao", "status", "status_implementacao", "status_tecnico" },
                values: new object[] { "Escopo sprint definido.", "Documentar modelo de grupo tecnico, filas e atribuicao.", 4, 1, 0 });
        }
    }
}
