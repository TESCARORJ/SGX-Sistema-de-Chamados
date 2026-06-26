using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AtualizarChecklistSprint7EndpointsPortal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000919"),
                columns: new[] { "atualizado_em", "atualizado_por", "concluido" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777718"),
                columns: new[] { "criterio_aceite", "data_conclusao_tecnica", "evidencia_implementacao", "observacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "status", "status_implementacao", "status_tecnico" },
                values: new object[] { "Portal possui endpoints consistentes, controllers nao possuem regra de negocio, frontend utiliza os endpoints corretos.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "PortalCatalogoServicosController reutilizando logica do Application com 4 endpoints consistentes: GET listar, GET detalhe, GET preparar e POST requisicoes. Frontend (NovoChamadoView) utilizando portalService e os endpoints corretos sem enviar regras restritas. 100% testes aprovados.", "Cenario A confirmado: Todos os endpoints ja existiam. Foi adicionado apenas o teste de requisicao invalida para fechar a cobertura. Regras estao nos Use Cases.", "Introduzir formulario por servico com persistencia das respostas e concluir a integracao guiada sem romper incidentes e fluxos legados.", 62, "Implementar ou reutilizar fluxo de notificacao para servicos sem aprovacao.", 1, 3, 2 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000919"),
                columns: new[] { "atualizado_em", "atualizado_por", "concluido" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777718"),
                columns: new[] { "criterio_aceite", "data_conclusao_tecnica", "evidencia_implementacao", "observacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "status", "status_implementacao", "status_tecnico" },
                values: new object[] { "Requisicao nasce do Catalogo e aplica formulario, aprovacao, SLA e grupo responsavel.", null, "CatalogoServicoId em Chamado; GET /api/portal/catalogo-servicos/{slug}/preparar-chamado; POST /api/portal/catalogo-servicos/requisicoes com contrato e validator dedicados; abertura por catalogo no AbrirChamadoUseCase; historico de abertura por catalogo; aprovacao automatica opcional por servico; tela de catalogo e detalhe do servico no portal.", null, "Aplicar classificacao vinda do catalogo no backend, aplicar grupo responsavel e SLA por servico, introduzir formulario por servico com persistencia das respostas e concluir a integracao guiada sem romper incidentes e fluxos legados.", 56, "Aplicar classificacao vinda do catalogo no backend.", 3, 2, 1 });
        }
    }
}
