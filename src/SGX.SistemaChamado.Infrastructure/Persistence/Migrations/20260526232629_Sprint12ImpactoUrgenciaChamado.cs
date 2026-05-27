using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Sprint12ImpactoUrgenciaChamado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "impacto_chamado",
                table: "chamados",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "urgencia_chamado",
                table: "chamados",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.Sql(
                """
                UPDATE chamados AS c
                SET prioridade_id = pb.id
                FROM prioridades_chamado AS pb
                WHERE pb.nivel = 1
                  AND (
                    c.prioridade_id IS NULL
                    OR NOT EXISTS (
                        SELECT 1
                        FROM prioridades_chamado AS pa
                        WHERE pa.id = c.prioridade_id
                    )
                    OR EXISTS (
                        SELECT 1
                        FROM prioridades_chamado AS pa
                        WHERE pa.id = c.prioridade_id
                          AND pa.nivel <= 1
                    )
                  );
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "impacto_chamado",
                table: "chamados");

            migrationBuilder.DropColumn(
                name: "urgencia_chamado",
                table: "chamados");
        }
    }
}
