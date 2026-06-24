using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConcluirIntegracaoEventosItsmRegressaoSprint6Roadmap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000911"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777725"),
                columns: new[] { "atencao_tecnica", "evidencia_implementacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual" },
                values: new object[] { "Preservar a separacao entre fato de negocio, resolucao de destinatarios, materializacao de conteudo, geracao idempotente, processamento e entrega, mantendo fora do escopo aprovacao/SLA sem ponto estavel de notificacao nesta etapa.", "Eventos priorizados integrados ao pipeline de notificacoes via orquestrador interno, com pontos estaveis em abertura, atribuicao/assuncao, status relevante e encerramento; idempotencia por evento/destinatario/canal; testes unitarios, integracao e regressao; compatibilidade com frontend, processamento e canais Sistema/Email; sem SignalR, sem fila externa, sem outbox improvisada e sem alterar Worker.Email.", "Validar recebimento real por solicitante e responsavel, confirmar templates ativos no ambiente, revisar eventos adiados e registrar aceite institucional da Sprint 6.", "Executar homologacao funcional/manual da Sprint 6 com templates ativos no ambiente, cenarios reais por perfil e evidencias formais, sem antecipar item 16 nem ampliar escopo para todos os eventos, aprovacao completa ou SLA.", 94, "Documentar, homologar e registrar aceite da Sprint 6", "Notificacoes internas persistidas, inbox autenticada e central frontend concluida; eventos ITSM priorizados agora integram o pipeline de geracao idempotente sem entrega sincrona nem impacto indevido em abertura, atribuicao, status, encerramento ou fluxos legados." });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000911"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777725"),
                columns: new[] { "atencao_tecnica", "evidencia_implementacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual" },
                values: new object[] { "Preservar a separacao entre resolucao de destinatarios, materializacao de conteudo, geracao idempotente, processamento e entrega ao evoluir a Sprint 6.", "Central frontend autenticada com service, types, contador de nao lidas, filtros, paginacao, detalhe, marcacao lida/nao lida, testes e responsividade; sem polling agressivo, sem tempo real e sem alterar Worker.Email.", "Validar cenarios reais de recebimento por solicitante, responsavel, aprovador, grupo tecnico e perfil apos existirem processamento, entrega por canal e integracao aos eventos ITSM.", "Integrar notificacoes aos eventos ITSM priorizados e executar testes de regressao sem misturar inbox, transporte, processamento e frontend da caixa propria.", 88, "Integrar notificacoes aos eventos ITSM priorizados e executar testes de regressao", "Notificacoes internas persistidas com API autenticada e central frontend responsiva para consulta propria, detalhe e marcacao lida/nao lida sem alterar entrega, transporte ou processamento." });
        }
    }
}
