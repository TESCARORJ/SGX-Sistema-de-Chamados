using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConcluirRegraRejeitarChamadoSprint4Roadmap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE roadmap_checklist_itens
                SET concluido = TRUE,
                    atualizado_em = TIMESTAMPTZ '2026-01-01 00:00:00+00',
                    atualizado_por = 'seed.sistema'
                WHERE id = '78787878-7878-7878-7878-000000000336';
                """);

            migrationBuilder.Sql(
                """
                UPDATE roadmap_itsm_itens
                SET percentual_implementacao = 60,
                    proxima_acao = 'Criar regra para reavaliar aprovacao apos mudanca de dados sensiveis.'
                WHERE id = '77777777-7777-7777-7777-777777777722';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE roadmap_checklist_itens
                SET concluido = FALSE,
                    atualizado_em = NULL,
                    atualizado_por = NULL
                WHERE id = '78787878-7878-7878-7878-000000000336';
                """);

            migrationBuilder.Sql(
                """
                UPDATE roadmap_itsm_itens
                SET percentual_implementacao = 59,
                    proxima_acao = 'Criar regra para rejeitar chamado.'
                WHERE id = '77777777-7777-7777-7777-777777777722';
                """);
        }
    }
}
