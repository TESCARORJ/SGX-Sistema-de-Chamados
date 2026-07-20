using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SincronizarSeedSprint6NotificacoesStatusReal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777725"),
                columns: new[] { "evidencia_implementacao", "pendencias_tecnicas", "proxima_acao", "status_implementacao", "status_tecnico" },
                values: new object[] { "Eventos priorizados integrados ao pipeline de notificacoes via orquestrador interno, com pontos estaveis em abertura, atribuicao/assuncao, status relevante e encerramento; idempotencia por evento/destinatario/canal; 209/209 testes de logica pura passando; ProcessarEventoCandidatoNotificacaoUseCase integrado a 5 use cases de chamado; compatibilidade com frontend, processamento e canais Sistema/Email; sem SignalR, sem fila externa, sem outbox improvisada e sem alterar Worker.Email.", "Nenhum debito tecnico pendente; falta apenas homologacao visual/manual e registro de aceite formal (item 16 do checklist).", "Executar homologacao visual/manual (320px/375px/768px/desktop) e registrar aceite formal da Sprint 6.", 3, 3 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777725"),
                columns: new[] { "evidencia_implementacao", "pendencias_tecnicas", "proxima_acao", "status_implementacao", "status_tecnico" },
                values: new object[] { "Eventos priorizados integrados ao pipeline de notificacoes via orquestrador interno, com pontos estaveis em abertura, atribuicao/assuncao, status relevante e encerramento; idempotencia por evento/destinatario/canal; testes unitarios, integracao e regressao; compatibilidade com frontend, processamento e canais Sistema/Email; sem SignalR, sem fila externa, sem outbox improvisada e sem alterar Worker.Email.", "Executar homologacao funcional/manual da Sprint 6 com templates ativos no ambiente, cenarios reais por perfil e evidencias formais, sem antecipar item 16 nem ampliar escopo para todos os eventos, aprovacao completa ou SLA.", "Documentar, homologar e registrar aceite da Sprint 6", 2, 6 });
        }
    }
}
