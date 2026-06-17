using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Sprint5RoadmapSeedAtualizacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777724"),
                columns: new[] { "atencao_tecnica", "criterio_aceite", "evidencia_implementacao", "objetivo", "pendencias_homologacao", "pendencias_tecnicas", "proxima_acao", "situacao_atual" },
                values: new object[] { "Separar conceitualmente e funcionalmente os estados Resolvido e Fechado. Não tratar resolução como encerramento definitivo. Exigir dados obrigatórios de solução para resolução e motivo obrigatório para cancelamento. Preservar compatibilidade com o fluxo legado, com SLA, com atendimento, com histórico, com permissões administrativas e com o motor de aprovações da Sprint 4. Nenhuma regra de fechamento deve ignorar aprovação pendente bloqueante.", "Fluxo contempla resolução, aceite/rejeição pelo solicitante, fechamento automático por prazo, cancelamento com motivo obrigatório, reabertura auditável e compatibilidade com SLA, histórico, permissões e aprovações pendentes bloqueantes.", "Base de encerramento/reabertura existente reaproveitada. Fluxos atuais de EncerrarChamadoUseCase e ReabrirChamadoUseCase devem ser preservados como base evolutiva.", "Criar governança de encerramento com aceite, fechamento automático e reabertura controlada.", "Validar regras com solicitantes, atendentes e administradores reais, incluindo resolução, aceite, rejeição, fechamento automático, cancelamento e reabertura controlada.", "Aceite do solicitante, rejeição da solução, prazo de auto-fechamento, motivo de cancelamento, campo solução obrigatório, política formal de reabertura, auditoria do ciclo resolvido/fechado/reaberto e integração segura com bloqueios de aprovação pendente.", "Evoluir estados e regras de negócio de ciclo de vida.", "Encerrar e reabrir chamados já existem no sistema, mas o fluxo ainda não possui governança completa de aceite do solicitante, prazo formal de auto-fechamento, rejeição de solução, campos obrigatórios de solução/cancelamento e políticas auditáveis de reabertura." });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777724"),
                columns: new[] { "atencao_tecnica", "criterio_aceite", "evidencia_implementacao", "objetivo", "pendencias_homologacao", "pendencias_tecnicas", "proxima_acao", "situacao_atual" },
                values: new object[] { "Separar resolvido de fechado e exigir dados obrigatorios de solucao/cancelamento.", "Fluxo contempla resolucao, aceite/rejeicao, fechamento automatico e reabertura auditavel.", "Base de encerramento/reabertura existente reaproveitada.", "Criar governanca de encerramento com aceite, fechamento automatico e reabertura controlada.", "Validar regras com solicitantes e atendentes reais.", "Aceite, prazo de auto-fechamento, motivo de cancelamento e campo solucao obrigatorio.", "Evoluir estados e regras de negocio de ciclo de vida.", "Encerrar e reabrir existem, mas faltam aceite do solicitante e politicas formais." });
        }
    }
}
