using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Sprint1NaturezaChamadoFundacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "natureza_chamado",
                table: "chamados",
                type: "integer",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.Sql(
                """
                UPDATE chamados
                SET natureza_chamado = 1
                WHERE natureza_chamado = 2
                  AND (
                    LOWER(COALESCE(titulo, '') || ' ' || COALESCE(descricao, '')) LIKE '%erro%'
                    OR LOWER(COALESCE(titulo, '') || ' ' || COALESCE(descricao, '')) LIKE '%falha%'
                    OR LOWER(COALESCE(titulo, '') || ' ' || COALESCE(descricao, '')) LIKE '%indisponibilidade%'
                    OR LOWER(COALESCE(titulo, '') || ' ' || COALESCE(descricao, '')) LIKE '%indisponivel%'
                    OR LOWER(COALESCE(titulo, '') || ' ' || COALESCE(descricao, '')) LIKE '%travamento%'
                    OR LOWER(COALESCE(titulo, '') || ' ' || COALESCE(descricao, '')) LIKE '%travou%'
                    OR LOWER(COALESCE(titulo, '') || ' ' || COALESCE(descricao, '')) LIKE '%sem acesso%'
                    OR LOWER(COALESCE(titulo, '') || ' ' || COALESCE(descricao, '')) LIKE '%queda%'
                    OR LOWER(COALESCE(titulo, '') || ' ' || COALESCE(descricao, '')) LIKE '%fora do ar%'
                  );
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "natureza_chamado",
                table: "chamados");
        }
    }
}
