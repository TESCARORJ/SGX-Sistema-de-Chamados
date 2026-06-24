using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConcluirTemplatesMaterializacaoConteudoSprint6Roadmap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000904"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777725"),
                columns: new[] { "evidencia_implementacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual" },
                values: new object[] { "Diagnostico consolidado; modelagem de dominio da Notificacao; contrato EventoCandidatoNotificacao; configuracao EF explicita; DbSet no contexto; migration estrutural CriarEstruturaNotificacaoSprint6; servico GerarNotificacaoUseCase com request, response e validator; idempotencia por chave unica global; resolucao de destinatarios por participacao e perfil com DTOs, validator, use case, elegibilidade, deduplicacao e avisos; TemplateNotificacao persistente com configuracao EF e migration estrutural CriarEstruturaTemplateNotificacaoSprint6; materializacao de assunto e conteudo com placeholders seguros, validacao de variaveis e vigencia; testes de dominio, configuracao, persistencia relacional, validator e use case; documentacao em docs/roadmap/sprint-6-templates-materializacao-conteudo.md; sem envio funcional de notificacao.", "Validar cenarios reais de recebimento por solicitante, responsavel, aprovador, grupo tecnico e perfil apos existirem preferencias, integracao aos eventos ITSM e entrega por canal.", "Implementar preferencias de notificacao por usuario e evento, depois processamento, entrega por canal e API de consulta sem misturar responsabilidades nem antecipar envio.", 50, "Implementar preferencias de notificacao por usuario e evento.", "Templates persistentes e materializacao de conteudo implementados com versao, vigencia, variaveis permitidas, renderizacao segura e testes de dominio/aplicacao/persistencia, ainda sem preferencias nem entrega por canal." });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000904"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777725"),
                columns: new[] { "evidencia_implementacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual" },
                values: new object[] { "Diagnostico consolidado; modelagem de dominio da Notificacao; contrato EventoCandidatoNotificacao; configuracao EF explicita; DbSet no contexto; migration estrutural CriarEstruturaNotificacaoSprint6; servico GerarNotificacaoUseCase com request, response e validator; idempotencia por chave unica global; resolucao de destinatarios por participacao e perfil com DTOs, validator, use case, elegibilidade, deduplicacao e avisos; testes unitarios, validator e persistencia relacional; documentacao em docs/roadmap/sprint-6-resolucao-destinatarios-participacao-perfil.md; sem envio funcional de notificacao.", "Validar cenarios reais de recebimento por solicitante, responsavel, aprovador, grupo tecnico e perfil apos existir materializacao de conteudo e integracao aos eventos ITSM.", "Modelar templates e materializacao de conteudo, depois preferencias, processamento, entrega por canal e API de consulta sem misturar responsabilidades nem antecipar envio.", 44, "Modelar templates e materializacao de conteudo.", "Resolucao de destinatarios internos implementada com participacoes reais, elegibilidade, deduplicacao, avisos e testes unitarios/relacionais, ainda sem templates nem entrega por canal." });
        }
    }
}
