using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddObjetivoRoadmapItsm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "objetivo",
                table: "roadmap_itsm_itens",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777701"),
                column: "objetivo",
                value: "Permitir que o solicitante autenticado registre chamados diretamente pelo portal, informando titulo, descricao, categoria, prioridade e anexos opcionais, acompanhando depois o andamento, historico e status do atendimento.");

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777702"),
                columns: new[] { "atencao_tecnica", "objetivo", "situacao_atual" },
                values: new object[] { "Validar com caixa IMAP real, e-mails reais, anexos reais e regras de autenticacao exigidas pelo ambiente.", "Permitir que e-mails recebidos em uma caixa configurada sejam processados pelo Worker IMAP para criar chamados automaticamente, correlacionar respostas com chamados existentes, registrar anexos permitidos e manter logs tecnicos de processamento.", "Fluxo implementado tecnicamente via Worker.Email, com criacao de chamado por e-mail, correlacao de respostas, tratamento de anexos e logs administrativos." });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777703"),
                columns: new[] { "atencao_tecnica", "objetivo", "situacao_atual" },
                values: new object[] { "Validar permissoes finas por tela e acao durante homologacao e evoluir auditoria detalhada de alteracoes de permissoes.", "Controlar o acesso ao sistema por perfis e permissoes granulares, permitindo definir o que Administradores, Atendentes e Solicitantes podem visualizar, executar e administrar sem necessidade de alteracao de codigo.", "Perfis macro, permissoes granulares, matriz de permissoes, /api/me com permissoes efetivas e controle visual por permissao implementados." });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777704"),
                column: "objetivo",
                value: null);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777705"),
                column: "objetivo",
                value: null);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777706"),
                column: "objetivo",
                value: null);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777707"),
                column: "objetivo",
                value: null);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777708"),
                column: "objetivo",
                value: null);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777709"),
                column: "objetivo",
                value: null);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777710"),
                column: "objetivo",
                value: null);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777711"),
                column: "objetivo",
                value: null);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777712"),
                column: "objetivo",
                value: null);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777713"),
                column: "objetivo",
                value: null);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777714"),
                column: "objetivo",
                value: null);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777715"),
                column: "objetivo",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "objetivo",
                table: "roadmap_itsm_itens");

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777702"),
                columns: new[] { "atencao_tecnica", "situacao_atual" },
                values: new object[] { "Validar fluxo completo com caixa IMAP real e e-mails reais antes de homologar", "Worker.Email, abertura por e-mail, correlacao de respostas, anexos e logs administrativos implementados tecnicamente" });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777703"),
                columns: new[] { "atencao_tecnica", "situacao_atual" },
                values: new object[] { "Validar permissoes finas por tela e acao", "Administrador, Atendente e Solicitante" });
        }
    }
}
