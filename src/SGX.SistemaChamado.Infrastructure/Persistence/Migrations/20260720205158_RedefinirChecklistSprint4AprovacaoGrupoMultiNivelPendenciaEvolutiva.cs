using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RedefinirChecklistSprint4AprovacaoGrupoMultiNivelPendenciaEvolutiva : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000361"),
                columns: new[] { "descricao", "obrigatorio", "titulo" },
                values: new object[] { "Achado documentado, não pendência de homologação da V1: não existe hoje endpoint ou tela que crie uma InstanciaAprovacaoChamado a partir de um chamado real (AbrirChamadoUseCase só cria AprovacaoChamado legado; AdminAprovacoesMotorController expõe apenas listagem, aprovar e reprovar; CriarManualAsync e PrepararAsync existem na camada de aplicação mas não são expostos por nenhum controller, e PrepararAsync explicitamente não persiste). Sem esse caminho, a aprovação por grupo aprovador aplicada a um chamado real fica fora do escopo homologável da V1 e passa a ser tratada como pendência evolutiva pós-MVP.", false, "Homologação de aprovação por grupo end-to-end (pendência evolutiva pós-MVP)" });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000362"),
                columns: new[] { "descricao", "obrigatorio", "titulo" },
                values: new object[] { "Achado documentado, não pendência de homologação da V1: não existe hoje endpoint ou tela que crie uma InstanciaAprovacaoChamado a partir de um chamado real (AbrirChamadoUseCase só cria AprovacaoChamado legado; AdminAprovacoesMotorController expõe apenas listagem, aprovar e reprovar; CriarManualAsync e PrepararAsync existem na camada de aplicação mas não são expostos por nenhum controller, e PrepararAsync explicitamente não persiste). Sem esse caminho, a aprovação multi-nível aplicada a um chamado real fica fora do escopo homologável da V1 e passa a ser tratada como pendência evolutiva pós-MVP.", false, "Homologação de aprovação multi-nível end-to-end (pendência evolutiva pós-MVP)" });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000363"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Depende do item 65 (aprovação por serviço/critério sensível end-to-end) ser implementado e homologado. Aprovação por grupo (item 66) e multi-nível (item 67) foram redefinidas como pendência evolutiva pós-MVP e não bloqueiam este aceite.", "Registrar aceite formal do escopo real de aprovação automática da Sprint 4 (aprovação por regra simples end-to-end)" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000361"),
                columns: new[] { "descricao", "obrigatorio", "titulo" },
                values: new object[] { "Categoria: Homologacao", true, "Preparar roteiro de homologacao de aprovacao por grupo" });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000362"),
                columns: new[] { "descricao", "obrigatorio", "titulo" },
                values: new object[] { "Categoria: Homologacao", true, "Preparar roteiro de homologacao de aprovacao multi-nivel" });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000363"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Categoria: Homologacao", "Registrar homologacao e aceite final" });
        }
    }
}
