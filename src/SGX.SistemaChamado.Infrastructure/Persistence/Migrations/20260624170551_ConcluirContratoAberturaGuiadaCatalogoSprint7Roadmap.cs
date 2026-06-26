using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConcluirContratoAberturaGuiadaCatalogoSprint7Roadmap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000914"),
                columns: new[] { "atualizado_em", "atualizado_por", "concluido" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777718"),
                columns: new[] { "evidencia_implementacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual" },
                values: new object[] { "CatalogoServicoId em Chamado; GET /api/portal/catalogo-servicos/{slug}/preparar-chamado; POST /api/portal/catalogo-servicos/requisicoes com contrato dedicado; abertura por catalogo no AbrirChamadoUseCase; historico de abertura por catalogo; aprovacao automatica opcional por servico; tela de catalogo e detalhe do servico no portal.", "Validar abertura guiada de requisicao por catalogo em cenarios com e sem aprovacao, comportamento com descricao opcional no contrato, ownership dos endpoints e responsividade no portal.", "Consolidar validator e use case dedicados da jornada guiada, aplicar grupo responsavel e SLA por servico, introduzir formulario por servico com persistencia das respostas e concluir a integracao guiada sem romper incidentes e fluxos legados.", 51, "Criar validator dedicado para abertura guiada por catalogo.", "Abertura por catalogo ja existe no chamado comum, com consulta do servico, associacao CatalogoServicoId, aplicacao backend de classificacao e aprovacao automatica opcional por servico. Agora existe contrato dedicado de abertura guiada por catalogo com semantica explicita de requisicao, ainda reutilizando o fluxo atual de Chamado. Permanecem pendentes validator consolidado da jornada guiada, use case dedicado, formulario dinamico e regras avancadas por servico." });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000914"),
                columns: new[] { "atualizado_em", "atualizado_por", "concluido" },
                values: new object[] { null, null, false });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777718"),
                columns: new[] { "evidencia_implementacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual" },
                values: new object[] { "CatalogoServicoId em Chamado; GET /api/portal/catalogo-servicos/{slug}/preparar-chamado; abertura por catalogo no AbrirChamadoUseCase; historico de abertura por catalogo; aprovacao automatica opcional por servico; tela de catalogo e detalhe do servico no portal.", "Validar abertura guiada de requisicao por catalogo em cenarios com e sem aprovacao, formulario obrigatorio, ownership dos endpoints e responsividade no portal.", "Criar fluxo guiado de requisicao sobre o chamado existente, com contrato e validator dedicados, aplicacao de grupo responsavel e SLA por servico, formulario por servico, persistencia de respostas, endpoints e telas guiadas sem romper incidentes e fluxos legados.", 49, "Implementar ou ajustar contrato de abertura guiada por catalogo com semantica explicita de requisicao.", "Abertura por catalogo ja existe no chamado comum, com consulta do servico, associacao CatalogoServicoId, aplicacao backend de classificacao e aprovacao automatica opcional por servico. Ainda nao existe fluxo separado e guiado de Requisicao de Servico com contrato, validator, use case e formulario dinamico dedicados." });
        }
    }
}
