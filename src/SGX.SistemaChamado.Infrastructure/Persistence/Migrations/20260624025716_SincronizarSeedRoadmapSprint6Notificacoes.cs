using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SGX.SistemaChamado.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SincronizarSeedRoadmapSprint6Notificacoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000901"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000902"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000903"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000904"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000905"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000906"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000907"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000908"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000909"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000910"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000911"));

            migrationBuilder.DeleteData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000912"));

            migrationBuilder.UpdateData(
                table: "calendarios_corporativos",
                keyColumn: "id",
                keyValue: new Guid("56565656-5656-5656-5656-565656565701"),
                columns: new[] { "descricao", "nome" },
                values: new object[] { "Calendário inicial para cálculo de SLA em horário comercial.", "Calendário Corporativo Padrão" });

            migrationBuilder.UpdateData(
                table: "perfis_acesso",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222202"),
                column: "nome",
                value: "Técnico N2");

            migrationBuilder.UpdateData(
                table: "perfis_acesso",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222205"),
                column: "nome",
                value: "Auditor Governança");

            migrationBuilder.UpdateData(
                table: "roadmap_categorias",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666601"),
                columns: new[] { "descricao", "nome" },
                values: new object[] { "Segurança e controle de acesso.", "Segurança" });

            migrationBuilder.UpdateData(
                table: "roadmap_categorias",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666604"),
                columns: new[] { "descricao", "nome" },
                values: new object[] { "Integrações com canais e sistemas.", "Integrações" });

            migrationBuilder.UpdateData(
                table: "roadmap_categorias",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666605"),
                column: "descricao",
                value: "Cadastros e parametrizações administrativas.");

            migrationBuilder.UpdateData(
                table: "roadmap_categorias",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666606"),
                columns: new[] { "descricao", "nome" },
                values: new object[] { "Notificações e comunicação.", "Notificações" });

            migrationBuilder.UpdateData(
                table: "roadmap_categorias",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666607"),
                column: "descricao",
                value: "Infraestrutura e sustentação.");

            migrationBuilder.UpdateData(
                table: "roadmap_categorias",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666608"),
                column: "descricao",
                value: "Experiência de uso e interface.");

            migrationBuilder.UpdateData(
                table: "roadmap_categorias",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666609"),
                columns: new[] { "descricao", "nome" },
                values: new object[] { "Relatórios e exportações.", "Relatórios" });

            migrationBuilder.UpdateData(
                table: "roadmap_categorias",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666610"),
                columns: new[] { "descricao", "nome" },
                values: new object[] { "Validações e aceite com usuários.", "Homologação" });

            migrationBuilder.UpdateData(
                table: "roadmap_categorias",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666611"),
                columns: new[] { "descricao", "nome" },
                values: new object[] { "Documentação técnica e funcional.", "Documentação" });

            migrationBuilder.UpdateData(
                table: "roadmap_categorias",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666612"),
                columns: new[] { "descricao", "nome" },
                values: new object[] { "Indicadores e governança gerencial.", "Gestão" });

            migrationBuilder.UpdateData(
                table: "roadmap_categorias",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666613"),
                columns: new[] { "descricao", "nome" },
                values: new object[] { "Rastreabilidade e governança.", "Governança" });

            migrationBuilder.UpdateData(
                table: "roadmap_categorias",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666614"),
                column: "descricao",
                value: "Base de conhecimento e catálogo.");

            migrationBuilder.UpdateData(
                table: "roadmap_categorias",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666615"),
                column: "descricao",
                value: "Fluxos e experiência do portal.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676703"),
                column: "titulo",
                value: "Permissões granulares criadas");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676706"),
                column: "titulo",
                value: "/api/me com permissões");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676708"),
                column: "titulo",
                value: "Matriz de permissões no frontend");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676709"),
                column: "titulo",
                value: "Controle visual por permissão");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676710"),
                column: "titulo",
                value: "Homologação com usuários reais");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676712"),
                column: "titulo",
                value: "Endpoint de criação de chamado validado");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676713"),
                column: "titulo",
                value: "Validações obrigatórias implementadas");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676714"),
                column: "titulo",
                value: "Solicitante obtido pelo usuário autenticado");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676716"),
                column: "titulo",
                value: "Histórico inicial criado");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676717"),
                column: "titulo",
                value: "Formulário com validação visual implementado");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676721"),
                column: "titulo",
                value: "Redirecionamento para detalhe após abertura implementado");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676724"),
                column: "titulo",
                value: "Chamado visível na fila administrativa");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676729"),
                column: "titulo",
                value: "Homologação manual com usuário real");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676731"),
                column: "titulo",
                value: "Validação real de anexos em ambiente de homologação");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676732"),
                column: "titulo",
                value: "Validação de anexo inválido com mensagem amigável");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676733"),
                column: "titulo",
                value: "Validação completa do fluxo abrir, anexar e acompanhar");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676734"),
                column: "titulo",
                value: "Validação com perfil Solicitante real do Microsoft Entra ID");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676735"),
                column: "titulo",
                value: "Validação com Atendente visualizando o chamado na fila");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696701"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist técnico concluído", "Decisão arquitetural documentada: Azure autentica, SGX autoriza." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696702"),
                column: "descricao",
                value: "Checklist técnico concluído");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696703"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist técnico concluído", "Validação JWT/API revisada." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696704"),
                column: "descricao",
                value: "Checklist técnico concluído");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696705"),
                column: "descricao",
                value: "Checklist técnico concluído");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696706"),
                column: "descricao",
                value: "Checklist técnico concluído");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696707"),
                column: "descricao",
                value: "Checklist técnico concluído");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696708"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist técnico concluído", "Emulação de perfis em Development preservada." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696709"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist técnico concluído", "Documentação técnica consolidada." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696710"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist técnico concluído", "Authority, Issuer, Audience, expiração e assinatura validados." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696711"),
                column: "descricao",
                value: "Checklist técnico concluído");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696712"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist técnico concluído", "Domínios permitidos configuráveis." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696713"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist técnico concluído", "Criação automática de usuário interno configurável." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696714"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist técnico concluído", "Perfil padrão de usuário Microsoft configurável." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696715"),
                column: "descricao",
                value: "Checklist técnico concluído");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696716"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist técnico concluído", "Bloqueio por domínio não permitido." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696717"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist técnico concluído", "Bloqueio de usuário interno inativo." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696718"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist técnico concluído", "Roles/groups do Azure não concedem Administrador automaticamente." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696719"),
                column: "descricao",
                value: "Checklist técnico concluído");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696720"),
                column: "descricao",
                value: "Checklist pendente de homologação/governança");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696721"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist pendente de homologação/governança", "Validar login com usuários corporativos reais." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696722"),
                column: "descricao",
                value: "Checklist pendente de homologação/governança");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696723"),
                column: "descricao",
                value: "Checklist pendente de homologação/governança");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696724"),
                column: "descricao",
                value: "Checklist pendente de homologação/governança");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696725"),
                column: "descricao",
                value: "Checklist pendente de homologação/governança");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696726"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist pendente de homologação/governança", "Revisar configuração com equipe responsável pelo Azure." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696727"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist pendente de homologação/governança", "Registrar evidências formais de homologação." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707701"),
                column: "titulo",
                value: "Entidade de política de SLA criada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707704"),
                column: "titulo",
                value: "Seed inicial de SLA padrão criado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707708"),
                column: "titulo",
                value: "Permissões administrativas de SLA criadas.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707709"),
                column: "titulo",
                value: "Tela administrativa básica criada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707710"),
                column: "titulo",
                value: "Validações de duplicidade e campos obrigatórios criadas.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707713"),
                column: "titulo",
                value: "Documentação técnica inicial criada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707716"),
                column: "titulo",
                value: "Service de cálculo de SLA criado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707717"),
                column: "titulo",
                value: "Política aplicável identificada por prioridade/categoria/departamento.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707718"),
                column: "titulo",
                value: "SLA aplicado na criação do chamado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707720"),
                column: "titulo",
                value: "Prazo de resolução calculado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707722"),
                column: "titulo",
                value: "Resolução registrada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707724"),
                column: "titulo",
                value: "Situação atual do SLA calculada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707730"),
                column: "titulo",
                value: "Documentação atualizada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707731"),
                column: "titulo",
                value: "Configuração de alerta de SLA criada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707732"),
                column: "titulo",
                value: "Tela administrativa de configuração de alerta criada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707733"),
                column: "titulo",
                value: "Endpoints de configuração de alerta criados.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707734"),
                column: "titulo",
                value: "Job de verificação de SLA criado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707735"),
                column: "titulo",
                value: "Periodicidade configurável por appsettings criada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707736"),
                column: "titulo",
                value: "Controle contra notificações/eventos duplicados criado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707737"),
                column: "titulo",
                value: "Histórico de eventos de SLA criado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707738"),
                column: "titulo",
                value: "Eventos integrados ao ciclo de SLA aplicado, primeira resposta, resolução, pausa e retomada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707741"),
                column: "titulo",
                value: "Indicador de SLA próximo do vencimento criado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707743"),
                column: "titulo",
                value: "Métrica de tempo médio de primeira resposta criada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707744"),
                column: "titulo",
                value: "Métrica de tempo médio de resolução criada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707748"),
                column: "titulo",
                value: "Histórico de SLA exibido no detalhe administrativo do chamado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707749"),
                column: "titulo",
                value: "Estrutura preparada para exportação futura.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707750"),
                column: "titulo",
                value: "Documentação atualizada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707755"),
                column: "titulo",
                value: "Migrations de calendário criadas.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707756"),
                column: "titulo",
                value: "Seed do calendário padrão criado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707757"),
                column: "titulo",
                value: "Relacionamento entre Política SLA e Calendário criado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707758"),
                column: "titulo",
                value: "Service administrativo de calendário criado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707759"),
                column: "titulo",
                value: "Service de cálculo de tempo útil criado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707760"),
                column: "titulo",
                value: "Cálculo de prazo de primeira resposta usando horário comercial implementado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707761"),
                column: "titulo",
                value: "Cálculo de prazo de resolução usando horário comercial implementado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707762"),
                column: "titulo",
                value: "Cálculo de minutos úteis de primeira resposta implementado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707763"),
                column: "titulo",
                value: "Cálculo de minutos úteis de resolução implementado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707764"),
                column: "titulo",
                value: "Endpoints administrativos de calendário criados.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707765"),
                column: "titulo",
                value: "Tela Admin > SLA > Calendários criada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707766"),
                column: "titulo",
                value: "Tela de política SLA atualizada com seleção de calendário.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707767"),
                column: "titulo",
                value: "Detalhe do chamado mostra tipo de cálculo e calendário usado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707769"),
                column: "titulo",
                value: "Documentação atualizada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-000000000002"),
                column: "titulo",
                value: "Enum de ação de auditoria criado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-000000000003"),
                column: "titulo",
                value: "Enum de nível de auditoria criado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-000000000005"),
                column: "titulo",
                value: "Índices de consulta criados.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-000000000008"),
                column: "titulo",
                value: "Captura de usuário atual integrada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-00000000000b"),
                column: "titulo",
                value: "Registro de logout avaliado e documentado como não aplicável enquanto não houver fluxo backend controlado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-00000000000c"),
                column: "titulo",
                value: "Registro de criação/edição/inativação de usuário integrado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-00000000000d"),
                column: "titulo",
                value: "Registro de perfis/permissões integrado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-000000000010"),
                column: "titulo",
                value: "Documentação atualizada em Gestão ITSM.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-000000000012"),
                column: "titulo",
                value: "Mascaramento de dados sensíveis implementado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-000000000014"),
                column: "titulo",
                value: "Auditoria de alteração de status implementada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-000000000015"),
                column: "titulo",
                value: "Auditoria de alteração de prioridade implementada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-000000000016"),
                column: "titulo",
                value: "Auditoria de alteração de categoria implementada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-000000000017"),
                column: "titulo",
                value: "Auditoria de atribuição de responsável implementada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-000000000019"),
                column: "titulo",
                value: "Auditoria de comentários administrativos implementada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-00000000001a"),
                column: "titulo",
                value: "Auditoria de encerramento/resolução implementada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-00000000001d"),
                column: "titulo",
                value: "Auditoria de usuários revisada e complementada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-00000000001f"),
                column: "titulo",
                value: "Auditoria de permissões revisada e complementada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-000000000020"),
                column: "titulo",
                value: "Auditoria de políticas de SLA implementada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-000000000022"),
                column: "titulo",
                value: "Auditoria de calendários de SLA implementada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-000000000023"),
                column: "titulo",
                value: "Auditoria de horários de calendário implementada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-000000000024"),
                column: "titulo",
                value: "Auditoria de exceções de calendário implementada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-000000000026"),
                column: "titulo",
                value: "Auditoria de autenticação corporativa implementada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-000000000028"),
                column: "titulo",
                value: "Auditoria de documentação ITSM preparada conforme estrutura atual estática.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-000000000029"),
                column: "titulo",
                value: "Testes automatizados de auditoria dos módulos críticos criados.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-00000000002a"),
                column: "titulo",
                value: "Documentação atualizada em Gestão ITSM.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-00000000002b"),
                column: "titulo",
                value: "Validação no banco confirmando eventos reais em eventos_auditoria preparada/executada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-00000000002f"),
                column: "titulo",
                value: "Paginação de eventos criada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-000000000032"),
                column: "titulo",
                value: "Permissões de auditoria criadas ou integradas.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-000000000033"),
                column: "titulo",
                value: "Menu Governança > Auditoria criado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-000000000037"),
                column: "titulo",
                value: "Visualização de dados antes/depois criada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-000000000038"),
                column: "titulo",
                value: "Indicadores básicos de auditoria criados.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-00000000003b"),
                column: "titulo",
                value: "Link entre Auditoria e Gestão ITSM criado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-00000000003c"),
                column: "titulo",
                value: "Documentação em Gestão ITSM atualizada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-00000000003f"),
                column: "titulo",
                value: "Validação com eventos reais em eventos_auditoria executada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("73727272-7272-7272-7272-000000000001"),
                column: "titulo",
                value: "Criar documentação ITSM.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("73727272-7272-7272-7272-000000000002"),
                column: "titulo",
                value: "Criar checklist de homologação.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000001"),
                column: "descricao",
                value: "Checklist de Dashboard / Gestão");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000002"),
                column: "descricao",
                value: "Checklist de Dashboard / Gestão");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000003"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist de Dashboard / Gestão", "Definir visão para administrador e atendente." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000004"),
                column: "descricao",
                value: "Checklist de Dashboard / Gestão");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000005"),
                column: "descricao",
                value: "Checklist de Dashboard / Gestão");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000006"),
                column: "descricao",
                value: "Checklist de Dashboard / Gestão");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000007"),
                column: "descricao",
                value: "Checklist de Dashboard / Gestão");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000008"),
                column: "descricao",
                value: "Checklist de Dashboard / Gestão");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000009"),
                column: "descricao",
                value: "Checklist de Dashboard / Gestão");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000010"),
                column: "descricao",
                value: "Checklist de Dashboard / Gestão");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000011"),
                column: "descricao",
                value: "Checklist de Dashboard / Gestão");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000012"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist de Dashboard / Gestão", "Validar regras de permissão por perfil." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000013"),
                column: "descricao",
                value: "Checklist de Dashboard / Gestão");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000014"),
                column: "descricao",
                value: "Checklist de Dashboard / Gestão");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000015"),
                column: "descricao",
                value: "Checklist de Dashboard / Gestão");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000016"),
                column: "descricao",
                value: "Checklist de Dashboard / Gestão");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000017"),
                column: "descricao",
                value: "Checklist de Dashboard / Gestão");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000018"),
                column: "descricao",
                value: "Checklist de Dashboard / Gestão");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000019"),
                column: "descricao",
                value: "Checklist de Dashboard / Gestão");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000020"),
                column: "descricao",
                value: "Checklist de Dashboard / Gestão");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000021"),
                column: "descricao",
                value: "Checklist de Dashboard / Gestão");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000022"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist de Dashboard / Gestão", "Exibir resumo da integração de e-mail." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000023"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist de Dashboard / Gestão", "Refinar layout visual para apresentação gerencial." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000024"),
                column: "descricao",
                value: "Checklist de Dashboard / Gestão");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000025"),
                column: "descricao",
                value: "Checklist de Dashboard / Gestão");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000026"),
                column: "descricao",
                value: "Checklist de Dashboard / Gestão");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000027"),
                column: "descricao",
                value: "Checklist de Dashboard / Gestão");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000028"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist de Dashboard / Gestão", "Testar bloqueio por ausência de permissão granular, se a policy for aplicada." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000029"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist de Dashboard / Gestão", "Criar teste frontend/e2e, se aplicável." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000030"),
                column: "descricao",
                value: "Checklist de Dashboard / Gestão");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000031"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist de Dashboard / Gestão", "Criar documentação funcional específica do Dashboard / Gestão." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000032"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist de Dashboard / Gestão", "Registrar evidências de homologação." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000033"),
                column: "descricao",
                value: "Checklist de Dashboard / Gestão");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000034"),
                column: "descricao",
                value: "Checklist de Dashboard / Gestão");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000035"),
                column: "descricao",
                value: "Checklist de Dashboard / Gestão");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000036"),
                column: "descricao",
                value: "Checklist de Dashboard / Gestão");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000037"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist de Dashboard / Gestão", "Cards principais com abertos, atendimento, aguardando solicitante, SLA vencido, próximos do vencimento e resolvidos no período implementados." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000038"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist de Dashboard / Gestão", "Navegação para fila de chamados, gestão de chamados e integração de e-mail implementada." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000039"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist de Dashboard / Gestão", "Filtros por período, departamento, categoria e responsável implementados." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000040"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist de Dashboard / Gestão", "Dados consolidados coerentes com os registros persistidos em cenário funcional base." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000108"),
                column: "concluido",
                value: false);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000137"),
                columns: new[] { "atualizado_em", "atualizado_por", "titulo" },
                values: new object[] { null, null, "Planejar escopo e criterios de aceite" });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000138"),
                columns: new[] { "atualizado_em", "atualizado_por", "concluido", "grupo", "titulo" },
                values: new object[] { null, null, false, 2, "Implementar entregas centrais da sprint" });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000139"),
                columns: new[] { "atualizado_em", "atualizado_por", "concluido", "grupo", "titulo" },
                values: new object[] { null, null, false, 3, "Executar testes funcionais e tecnicos" });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000140"),
                columns: new[] { "atualizado_em", "atualizado_por", "concluido", "grupo", "titulo" },
                values: new object[] { null, null, false, 5, "Registrar homologacao e aceite" });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777701"),
                columns: new[] { "criterio_aceite", "pendencias_homologacao", "pendencias_tecnicas", "proxima_acao" },
                values: new object[] { "Solicitante autenticado consegue abrir chamado pelo portal com título, descrição, categoria e prioridade, anexar arquivo permitido, visualizar o detalhe do chamado, acompanhar o status no portal e o chamado aparece na fila administrativa para atendimento.", "Validar com usuário real o fluxo completo de abrir chamado, anexar arquivo, acompanhar no portal e visualizar na fila administrativa.", "Testes E2E frontend do fluxo de abertura, validação real de anexos em homologação e script lint frontend.", "Executar homologação manual do fluxo completo com usuário real." });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777704"),
                columns: new[] { "area", "atencao_tecnica", "categoria", "criterio_aceite", "objetivo", "pendencias_homologacao", "pendencias_tecnicas", "proxima_acao", "situacao_atual" },
                values: new object[] { "Autenticação corporativa", "Manter a explicação clara: Microsoft Entra ID/Azure AD autentica; SGX autoriza. Não usar roles ou groups do Azure para conceder acesso administrativo automaticamente. Perfis e permissões continuam internos ao SGX. Validar MFA, Conditional Access, tenant real, redirect URI, API scope e ambiente publicado antes de considerar produção.", "Segurança", "O usuário corporativo autentica pelo Microsoft Entra ID/Azure AD no tenant configurado. A API valida token, issuer, audience, tenant, expiração e assinatura. O SGX identifica ou cria o usuário interno conforme configuração permitida, bloqueia usuários inativos ou fora do tenant/domínio permitido, retorna perfis e permissões efetivas em GET /api/me e aplica autorização interna nas rotas e ações. Usuários Solicitante, Atendente e Administrador devem acessar apenas o que seus perfis/permissões internos permitem.", "Permitir que usuários acessem o SGX Sistema de Chamados usando identidade corporativa Microsoft Entra ID/Azure AD, mantendo a autorização interna no SGX por usuários, perfis e permissões. O Azure autentica a identidade; o SGX controla o que cada usuário pode acessar e executar dentro do sistema.", "- Executar homologação ponta a ponta com usuário Administrador real.\n- Executar homologação ponta a ponta com usuário Atendente real.\n- Executar homologação ponta a ponta com usuário Solicitante real.\n- Validar comportamento com usuário interno inativo.\n- Validar bloqueio de domínio/tenant não permitido.\n- Validar mensagens de erro de login.\n- Validar redirecionamento por perfil/permissão após login.\n- Registrar evidências com prints, data, ambiente e usuário de teste.", "- Homologar com tenant institucional real do Microsoft Entra ID.\n- Validar login com usuários corporativos reais.\n- Validar MFA.\n- Validar Conditional Access.\n- Validar logout corporativo.\n- Validar ambiente publicado/VPS.\n- Revisar configuração com a equipe responsável pelo Azure.\n- Registrar evidências formais de homologação.\n- Avaliar persistência opcional de identificadores corporativos oid/tid, se necessário.\n- Definir governança de ciclo de vida do usuário interno: bloqueio, reativação e auditoria.", "Executar homologação com tenant institucional real do Microsoft Entra ID, validar MFA/Conditional Access, revisar configuração com a equipe Azure, testar usuários reais por perfil e anexar evidências formais antes de promoção para produção.", "Fluxo de autenticação corporativa com Microsoft Entra ID/Azure AD implementado funcionalmente, com suporte a validação de token JWT, modo Single Tenant, controle de domínio permitido, integração com GET /api/me, criação/identificação de usuário interno e autorização por perfis/permissões do SGX. Ainda depende de homologação com tenant institucional real." });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777705"),
                columns: new[] { "atencao_tecnica", "criterio_aceite", "objetivo", "pendencias_homologacao", "pendencias_tecnicas", "proxima_acao", "situacao_atual" },
                values: new object[] { "O SLA não deve ser apenas um campo manual no chamado. Deve existir uma regra centralizada e auditável para cálculo de prazo. O sistema deve considerar prioridade, categoria, departamento responsável, horário útil, feriados, pausas/suspensões, reabertura de chamado e mudança de status. Evitar cálculo duplicado no frontend. A regra principal deve ficar no backend, com persistência dos marcos calculados no chamado para rastreabilidade.", "O sistema deve permitir cadastrar políticas de SLA e aplicá-las automaticamente aos chamados conforme as regras configuradas. Ao abrir ou atualizar um chamado, o backend deve calcular e persistir os prazos de primeira resposta, atendimento e/ou resolução, considerando prioridade, categoria, departamento, horário útil e regras de pausa/reabertura quando aplicável. O detalhe do chamado deve exibir o status do SLA de forma clara: dentro do prazo, próximo do vencimento, vencido ou suspenso. Administradores e gestores devem conseguir filtrar e acompanhar chamados por situação de SLA. O cálculo deve ser testável, centralizado no backend e validado por testes automatizados.", "Permitir que o SGX Sistema de Chamados controle acordos de nível de serviço para chamados, definindo prazos de primeira resposta, atendimento e resolução conforme prioridade, categoria, departamento, tipo de solicitação e regras institucionais. O SLA deve apoiar gestão operacional, rastreabilidade, cobrança interna, indicadores e melhoria contínua do atendimento.", "- Homologar cadastro de política de SLA.\n- Homologar abertura de chamado com cálculo automático de SLA.\n- Homologar SLA por prioridade.\n- Homologar SLA por categoria.\n- Homologar SLA por departamento responsável.\n- Homologar cálculo de vencimento com horário útil.\n- Homologar comportamento em chamado pausado ou aguardando solicitante.\n- Homologar comportamento em chamado reaberto.\n- Homologar exibição do SLA para atendente.\n- Homologar exibição do SLA para administrador/gestor.\n- Homologar filtros de chamados atrasados.\n- Homologar indicadores gerenciais.\n- Registrar evidências formais com prints, data, ambiente e usuário de teste.", "- Validar cálculo de horário comercial em cenário real com volume institucional.\n- Evoluir calendário por departamento/time quando a governança estiver definida.\n- Evoluir importação automática de feriados nacionais/municipais.\n- Evoluir regras de reabertura para reaproveitamento de prazo remanescente.\n- Refinar política de proximidade do vencimento por canal/time.\n- Implementar alertas/notificações operacionais por SLA, se aplicável.\n- Consolidar trilha de auditoria e relatórios gerenciais de cumprimento.", "Executar homologação funcional de ponta a ponta com usuários reais e validar regras de SLA em ambiente publicado, incluindo casos de pausa, reabertura e governança operacional.", "Sprints 1, 2, 3 e 4 implementadas e validadas funcionalmente, com políticas/metas, SLA aplicado aos chamados, alertas, eventos, monitoramento, painel gerencial e calendário corporativo para horário comercial." });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777706"),
                columns: new[] { "area", "atencao_tecnica", "categoria", "criterio_aceite", "objetivo", "pendencias_homologacao", "pendencias_tecnicas", "proxima_acao", "situacao_atual" },
                values: new object[] { "Histórico/Auditoria", "Auditoria não é log técnico. ILogger continua para diagnóstico técnico; EventoAuditoria é governança/rastreabilidade. Não registrar senha, token JWT, refresh token, access token, client secret ou connection string. Dados sensíveis devem ser mascarados pelo AuditoriaDiffHelper.", "Governança", "O item Histórico/Auditoria deve exibir checklist ativo completo das Sprints 1, 2 e 3 com cálculo automático de percentual por checklist, status da implementação Implementado funcionalmente e status técnico Completo com pendências evolutivas, sem uso de percentual legado/manual.", "Criar trilha de auditoria para registrar ações relevantes executadas no SGX Sistema de Chamados, permitindo rastreabilidade, governança, análise de alterações, auditoria operacional e apoio à homologação.", "- Executar homologação funcional com eventos reais em eventos_auditoria cobrindo Chamados, Usuários, SLA, Autenticação e Roadmap ITSM.\n- Validar filtros e consulta administrativa em Admin > Governança > Auditoria com evidências formais.", "- Exportação Excel/PDF.\n- Retenção configurável de auditoria.\n- Assinatura/hash da trilha de auditoria.\n- Alertas para eventos críticos.\n- Painel avançado de segurança.\n- Integração SIEM/Log Analytics.\n- Política de anonimização/LGPD para eventos antigos.\n- Fluxo backend controlado de logout, se vier a existir.\n- Auditoria de edição de documentação ITSM, caso a documentação deixe de ser estática.", "Executar homologação funcional com eventos reais em eventos_auditoria, incluindo Chamados, Usuários, SLA, Autenticação e Roadmap ITSM. Validar filtros e consulta administrativa em Admin > Governança > Auditoria.", "Base técnica de auditoria criada, eventos de auditoria aplicados aos módulos críticos e tela administrativa de consulta implementada em Admin > Governança > Auditoria, com filtros, detalhe, indicadores e documentação em Gestão ITSM." });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777709"),
                columns: new[] { "atencao_tecnica", "categoria", "criterio_aceite", "objetivo", "observacao", "pendencias_homologacao", "pendencias_tecnicas", "proxima_acao", "situacao_atual" },
                values: new object[] { "Validar se os indicadores respeitam corretamente as permissões internas do usuário autenticado. Confirmar se administradores visualizam a operação completa e se atendentes visualizam apenas o escopo permitido, caso essa regra seja exigida. Verificar performance das consultas em bases maiores, principalmente filtros por período, produtividade por atendente e agrupamentos por status, prioridade e categoria. Garantir que chamados inativos, registros históricos e dados de SLA sejam tratados corretamente para não distorcer os indicadores.", "Gestão", "O usuário autorizado deve conseguir acessar o Dashboard Administrativo e visualizar indicadores consolidados da operação. Os filtros devem alterar os dados apresentados. Os cards principais devem exibir chamados abertos, em atendimento, aguardando solicitante, SLA vencido, próximos do vencimento e resolvidos no período. A tela deve permitir navegação para fila de chamados, gestão de chamados e integração de e-mail. Os dados exibidos devem ser coerentes com os registros persistidos no sistema.", "Disponibilizar uma visão gerencial da operação de chamados, permitindo que administradores e atendentes acompanhem em tempo real os principais indicadores do service desk, incluindo volume de chamados abertos, em atendimento, aguardando solicitante, resolvidos no período, chamados sem responsável, riscos de SLA, distribuição por status, prioridade, categoria, produtividade por atendente e situação da integração de e-mail.", "Checklist ativo consolidado em 34/40 itens (85%), com pendências concentradas em policy granular, performance, testes HTTP/frontend e homologação.", "- Validar com Administrador.\n- Validar com Atendente.\n- Conferir números do dashboard contra consultas reais no banco.\n- Validar filtros por período, departamento, categoria e responsável.\n- Confirmar se os indicadores atendem à necessidade de gestão da operação.\n- Registrar evidências formais de homologação.", "- Aplicar ou validar permissão granular Dashboard.Visualizar no backend, além da proteção por perfil.\n- Validar performance com volume maior de chamados.\n- Criar ou consolidar testes automatizados específicos do dashboard em nível HTTP.\n- Criar testes frontend/e2e para dashboardAdminService e AdminDashboardView, se o projeto já tiver estrutura para isso.\n- Avaliar cache ou otimização das consultas agregadas, caso necessário.\n- Revisar regras de permissão dos indicadores por perfil.", "Executar validação técnica e homologação funcional do dashboard com dados reais ou massa simulada mais próxima da operação institucional.", "Dashboard administrativo implementado funcionalmente no backend e frontend. A API disponibiliza indicadores consolidados, filtros por período e contexto administrativo. A interface apresenta cards gerenciais, gráficos/listagens por status, prioridade e categoria, indicadores de SLA, produtividade por atendente, fila de chamados e resumo da integração de e-mail. Pendente validação com usuários reais, refinamento visual final, testes frontend/e2e e homologação institucional." });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777721"),
                column: "percentual_implementacao",
                value: 25);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777725"),
                columns: new[] { "atencao_tecnica", "evidencia_implementacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual", "status_implementacao" },
                values: new object[] { "Definir modelo de notificacao sem acoplamento excessivo com canais externos.", "Escopo sprint definido.", "Validar recebimento por perfil, observador, aprovador e grupo tecnico.", "Tabela de notificacoes, API leitura/nao lida, preferencias e regras por evento.", 25, "Modelar entidade Notificacao e pipeline de eventos.", "Notificacoes ainda nao estao consolidadas como modulo persistente por evento ITSM.", 1 });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777730"),
                column: "proxima_acao",
                value: "Definir modelo de Problema e integrações com incidente/mudanca.");

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777736"),
                column: "percentual_implementacao",
                value: 25);

            migrationBuilder.UpdateData(
                table: "sla_politicas",
                keyColumn: "id",
                keyValue: new Guid("56565656-5656-5656-5656-565656565601"),
                columns: new[] { "descricao", "nome" },
                values: new object[] { "Política inicial de SLA do SGX Sistema de Chamados, usada como base para controle de primeira resposta e resolução dos chamados.", "SLA Padrão" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "calendarios_corporativos",
                keyColumn: "id",
                keyValue: new Guid("56565656-5656-5656-5656-565656565701"),
                columns: new[] { "descricao", "nome" },
                values: new object[] { "CalendÃ¡rio inicial para cÃ¡lculo de SLA em horÃ¡rio comercial.", "CalendÃ¡rio Corporativo PadrÃ£o" });

            migrationBuilder.UpdateData(
                table: "perfis_acesso",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222202"),
                column: "nome",
                value: "TÃ©cnico N2");

            migrationBuilder.UpdateData(
                table: "perfis_acesso",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222205"),
                column: "nome",
                value: "Auditor GovernanÃ§a");

            migrationBuilder.UpdateData(
                table: "roadmap_categorias",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666601"),
                columns: new[] { "descricao", "nome" },
                values: new object[] { "SeguranÃ§a e controle de acesso.", "SeguranÃ§a" });

            migrationBuilder.UpdateData(
                table: "roadmap_categorias",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666604"),
                columns: new[] { "descricao", "nome" },
                values: new object[] { "IntegraÃ§Ãµes com canais e sistemas.", "IntegraÃ§Ãµes" });

            migrationBuilder.UpdateData(
                table: "roadmap_categorias",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666605"),
                column: "descricao",
                value: "Cadastros e parametrizaÃ§Ãµes administrativas.");

            migrationBuilder.UpdateData(
                table: "roadmap_categorias",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666606"),
                columns: new[] { "descricao", "nome" },
                values: new object[] { "NotificaÃ§Ãµes e comunicaÃ§Ã£o.", "NotificaÃ§Ãµes" });

            migrationBuilder.UpdateData(
                table: "roadmap_categorias",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666607"),
                column: "descricao",
                value: "Infraestrutura e sustentaÃ§Ã£o.");

            migrationBuilder.UpdateData(
                table: "roadmap_categorias",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666608"),
                column: "descricao",
                value: "ExperiÃªncia de uso e interface.");

            migrationBuilder.UpdateData(
                table: "roadmap_categorias",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666609"),
                columns: new[] { "descricao", "nome" },
                values: new object[] { "RelatÃ³rios e exportaÃ§Ãµes.", "RelatÃ³rios" });

            migrationBuilder.UpdateData(
                table: "roadmap_categorias",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666610"),
                columns: new[] { "descricao", "nome" },
                values: new object[] { "ValidaÃ§Ãµes e aceite com usuÃ¡rios.", "HomologaÃ§Ã£o" });

            migrationBuilder.UpdateData(
                table: "roadmap_categorias",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666611"),
                columns: new[] { "descricao", "nome" },
                values: new object[] { "DocumentaÃ§Ã£o tÃ©cnica e funcional.", "DocumentaÃ§Ã£o" });

            migrationBuilder.UpdateData(
                table: "roadmap_categorias",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666612"),
                columns: new[] { "descricao", "nome" },
                values: new object[] { "Indicadores e governanÃ§a gerencial.", "GestÃ£o" });

            migrationBuilder.UpdateData(
                table: "roadmap_categorias",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666613"),
                columns: new[] { "descricao", "nome" },
                values: new object[] { "Rastreabilidade e governanÃ§a.", "GovernanÃ§a" });

            migrationBuilder.UpdateData(
                table: "roadmap_categorias",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666614"),
                column: "descricao",
                value: "Base de conhecimento e catÃ¡logo.");

            migrationBuilder.UpdateData(
                table: "roadmap_categorias",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666615"),
                column: "descricao",
                value: "Fluxos e experiÃªncia do portal.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676703"),
                column: "titulo",
                value: "PermissÃµes granulares criadas");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676706"),
                column: "titulo",
                value: "/api/me com permissÃµes");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676708"),
                column: "titulo",
                value: "Matriz de permissÃµes no frontend");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676709"),
                column: "titulo",
                value: "Controle visual por permissÃ£o");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676710"),
                column: "titulo",
                value: "HomologaÃ§Ã£o com usuÃ¡rios reais");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676712"),
                column: "titulo",
                value: "Endpoint de criaÃ§Ã£o de chamado validado");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676713"),
                column: "titulo",
                value: "ValidaÃ§Ãµes obrigatÃ³rias implementadas");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676714"),
                column: "titulo",
                value: "Solicitante obtido pelo usuÃ¡rio autenticado");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676716"),
                column: "titulo",
                value: "HistÃ³rico inicial criado");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676717"),
                column: "titulo",
                value: "FormulÃ¡rio com validaÃ§Ã£o visual implementado");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676721"),
                column: "titulo",
                value: "Redirecionamento para detalhe apÃ³s abertura implementado");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676724"),
                column: "titulo",
                value: "Chamado visÃ­vel na fila administrativa");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676729"),
                column: "titulo",
                value: "HomologaÃ§Ã£o manual com usuÃ¡rio real");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676731"),
                column: "titulo",
                value: "ValidaÃ§Ã£o real de anexos em ambiente de homologaÃ§Ã£o");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676732"),
                column: "titulo",
                value: "ValidaÃ§Ã£o de anexo invÃ¡lido com mensagem amigÃ¡vel");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676733"),
                column: "titulo",
                value: "ValidaÃ§Ã£o completa do fluxo abrir, anexar e acompanhar");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676734"),
                column: "titulo",
                value: "ValidaÃ§Ã£o com perfil Solicitante real do Microsoft Entra ID");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("67676767-6767-6767-6767-676767676735"),
                column: "titulo",
                value: "ValidaÃ§Ã£o com Atendente visualizando o chamado na fila");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696701"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist tÃ©cnico concluÃ­do", "DecisÃ£o arquitetural documentada: Azure autentica, SGX autoriza." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696702"),
                column: "descricao",
                value: "Checklist tÃ©cnico concluÃ­do");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696703"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist tÃ©cnico concluÃ­do", "ValidaÃ§Ã£o JWT/API revisada." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696704"),
                column: "descricao",
                value: "Checklist tÃ©cnico concluÃ­do");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696705"),
                column: "descricao",
                value: "Checklist tÃ©cnico concluÃ­do");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696706"),
                column: "descricao",
                value: "Checklist tÃ©cnico concluÃ­do");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696707"),
                column: "descricao",
                value: "Checklist tÃ©cnico concluÃ­do");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696708"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist tÃ©cnico concluÃ­do", "EmulaÃ§Ã£o de perfis em Development preservada." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696709"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist tÃ©cnico concluÃ­do", "DocumentaÃ§Ã£o tÃ©cnica consolidada." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696710"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist tÃ©cnico concluÃ­do", "Authority, Issuer, Audience, expiraÃ§Ã£o e assinatura validados." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696711"),
                column: "descricao",
                value: "Checklist tÃ©cnico concluÃ­do");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696712"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist tÃ©cnico concluÃ­do", "DomÃ­nios permitidos configurÃ¡veis." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696713"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist tÃ©cnico concluÃ­do", "CriaÃ§Ã£o automÃ¡tica de usuÃ¡rio interno configurÃ¡vel." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696714"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist tÃ©cnico concluÃ­do", "Perfil padrÃ£o de usuÃ¡rio Microsoft configurÃ¡vel." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696715"),
                column: "descricao",
                value: "Checklist tÃ©cnico concluÃ­do");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696716"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist tÃ©cnico concluÃ­do", "Bloqueio por domÃ­nio nÃ£o permitido." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696717"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist tÃ©cnico concluÃ­do", "Bloqueio de usuÃ¡rio interno inativo." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696718"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist tÃ©cnico concluÃ­do", "Roles/groups do Azure nÃ£o concedem Administrador automaticamente." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696719"),
                column: "descricao",
                value: "Checklist tÃ©cnico concluÃ­do");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696720"),
                column: "descricao",
                value: "Checklist pendente de homologaÃ§Ã£o/governanÃ§a");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696721"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist pendente de homologaÃ§Ã£o/governanÃ§a", "Validar login com usuÃ¡rios corporativos reais." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696722"),
                column: "descricao",
                value: "Checklist pendente de homologaÃ§Ã£o/governanÃ§a");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696723"),
                column: "descricao",
                value: "Checklist pendente de homologaÃ§Ã£o/governanÃ§a");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696724"),
                column: "descricao",
                value: "Checklist pendente de homologaÃ§Ã£o/governanÃ§a");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696725"),
                column: "descricao",
                value: "Checklist pendente de homologaÃ§Ã£o/governanÃ§a");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696726"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist pendente de homologaÃ§Ã£o/governanÃ§a", "Revisar configuraÃ§Ã£o com equipe responsÃ¡vel pelo Azure." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("69696969-6969-6969-6969-696969696727"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist pendente de homologaÃ§Ã£o/governanÃ§a", "Registrar evidÃªncias formais de homologaÃ§Ã£o." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707701"),
                column: "titulo",
                value: "Entidade de polÃ­tica de SLA criada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707704"),
                column: "titulo",
                value: "Seed inicial de SLA padrÃ£o criado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707708"),
                column: "titulo",
                value: "PermissÃµes administrativas de SLA criadas.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707709"),
                column: "titulo",
                value: "Tela administrativa bÃ¡sica criada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707710"),
                column: "titulo",
                value: "ValidaÃ§Ãµes de duplicidade e campos obrigatÃ³rios criadas.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707713"),
                column: "titulo",
                value: "DocumentaÃ§Ã£o tÃ©cnica inicial criada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707716"),
                column: "titulo",
                value: "Service de cÃ¡lculo de SLA criado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707717"),
                column: "titulo",
                value: "PolÃ­tica aplicÃ¡vel identificada por prioridade/categoria/departamento.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707718"),
                column: "titulo",
                value: "SLA aplicado na criaÃ§Ã£o do chamado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707720"),
                column: "titulo",
                value: "Prazo de resoluÃ§Ã£o calculado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707722"),
                column: "titulo",
                value: "ResoluÃ§Ã£o registrada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707724"),
                column: "titulo",
                value: "SituaÃ§Ã£o atual do SLA calculada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707730"),
                column: "titulo",
                value: "DocumentaÃ§Ã£o atualizada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707731"),
                column: "titulo",
                value: "ConfiguraÃ§Ã£o de alerta de SLA criada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707732"),
                column: "titulo",
                value: "Tela administrativa de configuraÃ§Ã£o de alerta criada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707733"),
                column: "titulo",
                value: "Endpoints de configuraÃ§Ã£o de alerta criados.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707734"),
                column: "titulo",
                value: "Job de verificaÃ§Ã£o de SLA criado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707735"),
                column: "titulo",
                value: "Periodicidade configurÃ¡vel por appsettings criada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707736"),
                column: "titulo",
                value: "Controle contra notificaÃ§Ãµes/eventos duplicados criado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707737"),
                column: "titulo",
                value: "HistÃ³rico de eventos de SLA criado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707738"),
                column: "titulo",
                value: "Eventos integrados ao ciclo de SLA aplicado, primeira resposta, resoluÃ§Ã£o, pausa e retomada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707741"),
                column: "titulo",
                value: "Indicador de SLA prÃ³ximo do vencimento criado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707743"),
                column: "titulo",
                value: "MÃ©trica de tempo mÃ©dio de primeira resposta criada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707744"),
                column: "titulo",
                value: "MÃ©trica de tempo mÃ©dio de resoluÃ§Ã£o criada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707748"),
                column: "titulo",
                value: "HistÃ³rico de SLA exibido no detalhe administrativo do chamado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707749"),
                column: "titulo",
                value: "Estrutura preparada para exportaÃ§Ã£o futura.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707750"),
                column: "titulo",
                value: "DocumentaÃ§Ã£o atualizada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707755"),
                column: "titulo",
                value: "Migrations de calendÃ¡rio criadas.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707756"),
                column: "titulo",
                value: "Seed do calendÃ¡rio padrÃ£o criado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707757"),
                column: "titulo",
                value: "Relacionamento entre PolÃ­tica SLA e CalendÃ¡rio criado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707758"),
                column: "titulo",
                value: "Service administrativo de calendÃ¡rio criado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707759"),
                column: "titulo",
                value: "Service de cÃ¡lculo de tempo Ãºtil criado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707760"),
                column: "titulo",
                value: "CÃ¡lculo de prazo de primeira resposta usando horÃ¡rio comercial implementado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707761"),
                column: "titulo",
                value: "CÃ¡lculo de prazo de resoluÃ§Ã£o usando horÃ¡rio comercial implementado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707762"),
                column: "titulo",
                value: "CÃ¡lculo de minutos Ãºteis de primeira resposta implementado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707763"),
                column: "titulo",
                value: "CÃ¡lculo de minutos Ãºteis de resoluÃ§Ã£o implementado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707764"),
                column: "titulo",
                value: "Endpoints administrativos de calendÃ¡rio criados.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707765"),
                column: "titulo",
                value: "Tela Admin > SLA > CalendÃ¡rios criada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707766"),
                column: "titulo",
                value: "Tela de polÃ­tica SLA atualizada com seleÃ§Ã£o de calendÃ¡rio.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707767"),
                column: "titulo",
                value: "Detalhe do chamado mostra tipo de cÃ¡lculo e calendÃ¡rio usado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("70707070-7070-7070-7070-707070707769"),
                column: "titulo",
                value: "DocumentaÃ§Ã£o atualizada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-000000000002"),
                column: "titulo",
                value: "Enum de aÃ§Ã£o de auditoria criado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-000000000003"),
                column: "titulo",
                value: "Enum de nÃ­vel de auditoria criado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-000000000005"),
                column: "titulo",
                value: "Ãndices de consulta criados.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-000000000008"),
                column: "titulo",
                value: "Captura de usuÃ¡rio atual integrada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-00000000000b"),
                column: "titulo",
                value: "Registro de logout avaliado e documentado como nÃ£o aplicÃ¡vel enquanto nÃ£o houver fluxo backend controlado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-00000000000c"),
                column: "titulo",
                value: "Registro de criaÃ§Ã£o/ediÃ§Ã£o/inativaÃ§Ã£o de usuÃ¡rio integrado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-00000000000d"),
                column: "titulo",
                value: "Registro de perfis/permissÃµes integrado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-000000000010"),
                column: "titulo",
                value: "DocumentaÃ§Ã£o atualizada em GestÃ£o ITSM.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-000000000012"),
                column: "titulo",
                value: "Mascaramento de dados sensÃ­veis implementado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-000000000014"),
                column: "titulo",
                value: "Auditoria de alteraÃ§Ã£o de status implementada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-000000000015"),
                column: "titulo",
                value: "Auditoria de alteraÃ§Ã£o de prioridade implementada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-000000000016"),
                column: "titulo",
                value: "Auditoria de alteraÃ§Ã£o de categoria implementada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-000000000017"),
                column: "titulo",
                value: "Auditoria de atribuiÃ§Ã£o de responsÃ¡vel implementada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-000000000019"),
                column: "titulo",
                value: "Auditoria de comentÃ¡rios administrativos implementada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-00000000001a"),
                column: "titulo",
                value: "Auditoria de encerramento/resoluÃ§Ã£o implementada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-00000000001d"),
                column: "titulo",
                value: "Auditoria de usuÃ¡rios revisada e complementada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-00000000001f"),
                column: "titulo",
                value: "Auditoria de permissÃµes revisada e complementada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-000000000020"),
                column: "titulo",
                value: "Auditoria de polÃ­ticas de SLA implementada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-000000000022"),
                column: "titulo",
                value: "Auditoria de calendÃ¡rios de SLA implementada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-000000000023"),
                column: "titulo",
                value: "Auditoria de horÃ¡rios de calendÃ¡rio implementada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-000000000024"),
                column: "titulo",
                value: "Auditoria de exceÃ§Ãµes de calendÃ¡rio implementada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-000000000026"),
                column: "titulo",
                value: "Auditoria de autenticaÃ§Ã£o corporativa implementada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-000000000028"),
                column: "titulo",
                value: "Auditoria de documentaÃ§Ã£o ITSM preparada conforme estrutura atual estÃ¡tica.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-000000000029"),
                column: "titulo",
                value: "Testes automatizados de auditoria dos mÃ³dulos crÃ­ticos criados.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-00000000002a"),
                column: "titulo",
                value: "DocumentaÃ§Ã£o atualizada em GestÃ£o ITSM.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-00000000002b"),
                column: "titulo",
                value: "ValidaÃ§Ã£o no banco confirmando eventos reais em eventos_auditoria preparada/executada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-00000000002f"),
                column: "titulo",
                value: "PaginaÃ§Ã£o de eventos criada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-000000000032"),
                column: "titulo",
                value: "PermissÃµes de auditoria criadas ou integradas.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-000000000033"),
                column: "titulo",
                value: "Menu GovernanÃ§a > Auditoria criado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-000000000037"),
                column: "titulo",
                value: "VisualizaÃ§Ã£o de dados antes/depois criada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-000000000038"),
                column: "titulo",
                value: "Indicadores bÃ¡sicos de auditoria criados.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-00000000003b"),
                column: "titulo",
                value: "Link entre Auditoria e GestÃ£o ITSM criado.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-00000000003c"),
                column: "titulo",
                value: "DocumentaÃ§Ã£o em GestÃ£o ITSM atualizada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("71717171-7171-7171-7171-00000000003f"),
                column: "titulo",
                value: "ValidaÃ§Ã£o com eventos reais em eventos_auditoria executada.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("73727272-7272-7272-7272-000000000001"),
                column: "titulo",
                value: "Criar documentaÃ§Ã£o ITSM.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("73727272-7272-7272-7272-000000000002"),
                column: "titulo",
                value: "Criar checklist de homologaÃ§Ã£o.");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000001"),
                column: "descricao",
                value: "Checklist de Dashboard / GestÃ£o");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000002"),
                column: "descricao",
                value: "Checklist de Dashboard / GestÃ£o");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000003"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist de Dashboard / GestÃ£o", "Definir visÃ£o para administrador e atendente." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000004"),
                column: "descricao",
                value: "Checklist de Dashboard / GestÃ£o");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000005"),
                column: "descricao",
                value: "Checklist de Dashboard / GestÃ£o");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000006"),
                column: "descricao",
                value: "Checklist de Dashboard / GestÃ£o");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000007"),
                column: "descricao",
                value: "Checklist de Dashboard / GestÃ£o");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000008"),
                column: "descricao",
                value: "Checklist de Dashboard / GestÃ£o");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000009"),
                column: "descricao",
                value: "Checklist de Dashboard / GestÃ£o");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000010"),
                column: "descricao",
                value: "Checklist de Dashboard / GestÃ£o");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000011"),
                column: "descricao",
                value: "Checklist de Dashboard / GestÃ£o");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000012"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist de Dashboard / GestÃ£o", "Validar regras de permissÃ£o por perfil." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000013"),
                column: "descricao",
                value: "Checklist de Dashboard / GestÃ£o");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000014"),
                column: "descricao",
                value: "Checklist de Dashboard / GestÃ£o");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000015"),
                column: "descricao",
                value: "Checklist de Dashboard / GestÃ£o");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000016"),
                column: "descricao",
                value: "Checklist de Dashboard / GestÃ£o");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000017"),
                column: "descricao",
                value: "Checklist de Dashboard / GestÃ£o");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000018"),
                column: "descricao",
                value: "Checklist de Dashboard / GestÃ£o");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000019"),
                column: "descricao",
                value: "Checklist de Dashboard / GestÃ£o");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000020"),
                column: "descricao",
                value: "Checklist de Dashboard / GestÃ£o");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000021"),
                column: "descricao",
                value: "Checklist de Dashboard / GestÃ£o");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000022"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist de Dashboard / GestÃ£o", "Exibir resumo da integraÃ§Ã£o de e-mail." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000023"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist de Dashboard / GestÃ£o", "Refinar layout visual para apresentaÃ§Ã£o gerencial." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000024"),
                column: "descricao",
                value: "Checklist de Dashboard / GestÃ£o");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000025"),
                column: "descricao",
                value: "Checklist de Dashboard / GestÃ£o");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000026"),
                column: "descricao",
                value: "Checklist de Dashboard / GestÃ£o");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000027"),
                column: "descricao",
                value: "Checklist de Dashboard / GestÃ£o");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000028"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist de Dashboard / GestÃ£o", "Testar bloqueio por ausÃªncia de permissÃ£o granular, se a policy for aplicada." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000029"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist de Dashboard / GestÃ£o", "Criar teste frontend/e2e, se aplicÃ¡vel." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000030"),
                column: "descricao",
                value: "Checklist de Dashboard / GestÃ£o");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000031"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist de Dashboard / GestÃ£o", "Criar documentaÃ§Ã£o funcional especÃ­fica do Dashboard / GestÃ£o." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000032"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist de Dashboard / GestÃ£o", "Registrar evidÃªncias de homologaÃ§Ã£o." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000033"),
                column: "descricao",
                value: "Checklist de Dashboard / GestÃ£o");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000034"),
                column: "descricao",
                value: "Checklist de Dashboard / GestÃ£o");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000035"),
                column: "descricao",
                value: "Checklist de Dashboard / GestÃ£o");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000036"),
                column: "descricao",
                value: "Checklist de Dashboard / GestÃ£o");

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000037"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist de Dashboard / GestÃ£o", "Cards principais com abertos, atendimento, aguardando solicitante, SLA vencido, prÃ³ximos do vencimento e resolvidos no perÃ­odo implementados." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000038"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist de Dashboard / GestÃ£o", "NavegaÃ§Ã£o para fila de chamados, gestÃ£o de chamados e integraÃ§Ã£o de e-mail implementada." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000039"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist de Dashboard / GestÃ£o", "Filtros por perÃ­odo, departamento, categoria e responsÃ¡vel implementados." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("74727272-7272-7272-7272-000000000040"),
                columns: new[] { "descricao", "titulo" },
                values: new object[] { "Checklist de Dashboard / GestÃ£o", "Dados consolidados coerentes com os registros persistidos em cenÃ¡rio funcional base." });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000108"),
                column: "concluido",
                value: true);

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000137"),
                columns: new[] { "atualizado_em", "atualizado_por", "titulo" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Planejar escopo e critérios de aceite" });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000138"),
                columns: new[] { "atualizado_em", "atualizado_por", "concluido", "grupo", "titulo" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, 1, "Diagnosticar estruturas existentes de notificações e eventos" });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000139"),
                columns: new[] { "atualizado_em", "atualizado_por", "concluido", "grupo", "titulo" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, 2, "Modelar entidade Notificacao e contrato de eventos" });

            migrationBuilder.UpdateData(
                table: "roadmap_checklist_itens",
                keyColumn: "id",
                keyValue: new Guid("78787878-7878-7878-7878-000000000140"),
                columns: new[] { "atualizado_em", "atualizado_por", "concluido", "grupo", "titulo" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, 2, "Criar configuração EF e migration estrutural de notificações" });

            migrationBuilder.InsertData(
                table: "roadmap_checklist_itens",
                columns: new[] { "id", "ativo", "atualizado_em", "atualizado_por", "concluido", "criado_em", "criado_por", "descricao", "grupo", "obrigatorio", "ordem", "roadmap_item_id", "titulo" },
                values: new object[,]
                {
                    { new Guid("78787878-7878-7878-7878-000000000901"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 6 Notificacoes ITSM", 3, true, 5, new Guid("77777777-7777-7777-7777-777777777725"), "Testar domínio e estrutura persistente de notificações" },
                    { new Guid("78787878-7878-7878-7878-000000000902"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 6 Notificacoes ITSM", 2, true, 6, new Guid("77777777-7777-7777-7777-777777777725"), "Criar serviço de geração idempotente de notificações" },
                    { new Guid("78787878-7878-7878-7878-000000000903"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 6 Notificacoes ITSM", 2, true, 7, new Guid("77777777-7777-7777-7777-777777777725"), "Implementar resolução de destinatários por participação e perfil" },
                    { new Guid("78787878-7878-7878-7878-000000000904"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 6 Notificacoes ITSM", 2, true, 8, new Guid("77777777-7777-7777-7777-777777777725"), "Modelar templates e materialização de conteúdo" },
                    { new Guid("78787878-7878-7878-7878-000000000905"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 6 Notificacoes ITSM", 4, true, 9, new Guid("77777777-7777-7777-7777-777777777725"), "Implementar preferências de notificação por usuário e evento" },
                    { new Guid("78787878-7878-7878-7878-000000000906"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 6 Notificacoes ITSM", 2, true, 10, new Guid("77777777-7777-7777-7777-777777777725"), "Implementar processamento e controle de tentativas de entrega" },
                    { new Guid("78787878-7878-7878-7878-000000000907"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 6 Notificacoes ITSM", 2, true, 11, new Guid("77777777-7777-7777-7777-777777777725"), "Implementar entrega pelo canal Sistema" },
                    { new Guid("78787878-7878-7878-7878-000000000908"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 6 Notificacoes ITSM", 2, true, 12, new Guid("77777777-7777-7777-7777-777777777725"), "Implementar entrega pelo canal E-mail" },
                    { new Guid("78787878-7878-7878-7878-000000000909"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 6 Notificacoes ITSM", 3, true, 13, new Guid("77777777-7777-7777-7777-777777777725"), "Criar API de consulta, leitura e marcação como não lida" },
                    { new Guid("78787878-7878-7878-7878-000000000910"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 6 Notificacoes ITSM", 4, true, 14, new Guid("77777777-7777-7777-7777-777777777725"), "Implementar central de notificações no frontend" },
                    { new Guid("78787878-7878-7878-7878-000000000911"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 6 Notificacoes ITSM", 3, true, 15, new Guid("77777777-7777-7777-7777-777777777725"), "Integrar notificações aos eventos ITSM priorizados e executar testes de regressão" },
                    { new Guid("78787878-7878-7878-7878-000000000912"), true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "seed.sistema", "Sprint 6 Notificacoes ITSM", 5, true, 16, new Guid("77777777-7777-7777-7777-777777777725"), "Documentar, homologar e registrar aceite da Sprint 6" }
                });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777701"),
                columns: new[] { "criterio_aceite", "pendencias_homologacao", "pendencias_tecnicas", "proxima_acao" },
                values: new object[] { "Solicitante autenticado consegue abrir chamado pelo portal com tÃ­tulo, descriÃ§Ã£o, categoria e prioridade, anexar arquivo permitido, visualizar o detalhe do chamado, acompanhar o status no portal e o chamado aparece na fila administrativa para atendimento.", "Validar com usuÃ¡rio real o fluxo completo de abrir chamado, anexar arquivo, acompanhar no portal e visualizar na fila administrativa.", "Testes E2E frontend do fluxo de abertura, validaÃ§Ã£o real de anexos em homologaÃ§Ã£o e script lint frontend.", "Executar homologaÃ§Ã£o manual do fluxo completo com usuÃ¡rio real." });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777704"),
                columns: new[] { "area", "atencao_tecnica", "categoria", "criterio_aceite", "objetivo", "pendencias_homologacao", "pendencias_tecnicas", "proxima_acao", "situacao_atual" },
                values: new object[] { "AutenticaÃ§Ã£o corporativa", "Manter a explicaÃ§Ã£o clara: Microsoft Entra ID/Azure AD autentica; SGX autoriza. NÃ£o usar roles ou groups do Azure para conceder acesso administrativo automaticamente. Perfis e permissÃµes continuam internos ao SGX. Validar MFA, Conditional Access, tenant real, redirect URI, API scope e ambiente publicado antes de considerar produÃ§Ã£o.", "SeguranÃ§a", "O usuÃ¡rio corporativo autentica pelo Microsoft Entra ID/Azure AD no tenant configurado. A API valida token, issuer, audience, tenant, expiraÃ§Ã£o e assinatura. O SGX identifica ou cria o usuÃ¡rio interno conforme configuraÃ§Ã£o permitida, bloqueia usuÃ¡rios inativos ou fora do tenant/domÃ­nio permitido, retorna perfis e permissÃµes efetivas em GET /api/me e aplica autorizaÃ§Ã£o interna nas rotas e aÃ§Ãµes. UsuÃ¡rios Solicitante, Atendente e Administrador devem acessar apenas o que seus perfis/permissÃµes internos permitem.", "Permitir que usuÃ¡rios acessem o SGX Sistema de Chamados usando identidade corporativa Microsoft Entra ID/Azure AD, mantendo a autorizaÃ§Ã£o interna no SGX por usuÃ¡rios, perfis e permissÃµes. O Azure autentica a identidade; o SGX controla o que cada usuÃ¡rio pode acessar e executar dentro do sistema.", "- Executar homologaÃ§Ã£o ponta a ponta com usuÃ¡rio Administrador real.\n- Executar homologaÃ§Ã£o ponta a ponta com usuÃ¡rio Atendente real.\n- Executar homologaÃ§Ã£o ponta a ponta com usuÃ¡rio Solicitante real.\n- Validar comportamento com usuÃ¡rio interno inativo.\n- Validar bloqueio de domÃ­nio/tenant nÃ£o permitido.\n- Validar mensagens de erro de login.\n- Validar redirecionamento por perfil/permissÃ£o apÃ³s login.\n- Registrar evidÃªncias com prints, data, ambiente e usuÃ¡rio de teste.", "- Homologar com tenant institucional real do Microsoft Entra ID.\n- Validar login com usuÃ¡rios corporativos reais.\n- Validar MFA.\n- Validar Conditional Access.\n- Validar logout corporativo.\n- Validar ambiente publicado/VPS.\n- Revisar configuraÃ§Ã£o com a equipe responsÃ¡vel pelo Azure.\n- Registrar evidÃªncias formais de homologaÃ§Ã£o.\n- Avaliar persistÃªncia opcional de identificadores corporativos oid/tid, se necessÃ¡rio.\n- Definir governanÃ§a de ciclo de vida do usuÃ¡rio interno: bloqueio, reativaÃ§Ã£o e auditoria.", "Executar homologaÃ§Ã£o com tenant institucional real do Microsoft Entra ID, validar MFA/Conditional Access, revisar configuraÃ§Ã£o com a equipe Azure, testar usuÃ¡rios reais por perfil e anexar evidÃªncias formais antes de promoÃ§Ã£o para produÃ§Ã£o.", "Fluxo de autenticaÃ§Ã£o corporativa com Microsoft Entra ID/Azure AD implementado funcionalmente, com suporte a validaÃ§Ã£o de token JWT, modo Single Tenant, controle de domÃ­nio permitido, integraÃ§Ã£o com GET /api/me, criaÃ§Ã£o/identificaÃ§Ã£o de usuÃ¡rio interno e autorizaÃ§Ã£o por perfis/permissÃµes do SGX. Ainda depende de homologaÃ§Ã£o com tenant institucional real." });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777705"),
                columns: new[] { "atencao_tecnica", "criterio_aceite", "objetivo", "pendencias_homologacao", "pendencias_tecnicas", "proxima_acao", "situacao_atual" },
                values: new object[] { "O SLA nÃ£o deve ser apenas um campo manual no chamado. Deve existir uma regra centralizada e auditÃ¡vel para cÃ¡lculo de prazo. O sistema deve considerar prioridade, categoria, departamento responsÃ¡vel, horÃ¡rio Ãºtil, feriados, pausas/suspensÃµes, reabertura de chamado e mudanÃ§a de status. Evitar cÃ¡lculo duplicado no frontend. A regra principal deve ficar no backend, com persistÃªncia dos marcos calculados no chamado para rastreabilidade.", "O sistema deve permitir cadastrar polÃ­ticas de SLA e aplicÃ¡-las automaticamente aos chamados conforme as regras configuradas. Ao abrir ou atualizar um chamado, o backend deve calcular e persistir os prazos de primeira resposta, atendimento e/ou resoluÃ§Ã£o, considerando prioridade, categoria, departamento, horÃ¡rio Ãºtil e regras de pausa/reabertura quando aplicÃ¡vel. O detalhe do chamado deve exibir o status do SLA de forma clara: dentro do prazo, prÃ³ximo do vencimento, vencido ou suspenso. Administradores e gestores devem conseguir filtrar e acompanhar chamados por situaÃ§Ã£o de SLA. O cÃ¡lculo deve ser testÃ¡vel, centralizado no backend e validado por testes automatizados.", "Permitir que o SGX Sistema de Chamados controle acordos de nÃ­vel de serviÃ§o para chamados, definindo prazos de primeira resposta, atendimento e resoluÃ§Ã£o conforme prioridade, categoria, departamento, tipo de solicitaÃ§Ã£o e regras institucionais. O SLA deve apoiar gestÃ£o operacional, rastreabilidade, cobranÃ§a interna, indicadores e melhoria contÃ­nua do atendimento.", "- Homologar cadastro de polÃ­tica de SLA.\n- Homologar abertura de chamado com cÃ¡lculo automÃ¡tico de SLA.\n- Homologar SLA por prioridade.\n- Homologar SLA por categoria.\n- Homologar SLA por departamento responsÃ¡vel.\n- Homologar cÃ¡lculo de vencimento com horÃ¡rio Ãºtil.\n- Homologar comportamento em chamado pausado ou aguardando solicitante.\n- Homologar comportamento em chamado reaberto.\n- Homologar exibiÃ§Ã£o do SLA para atendente.\n- Homologar exibiÃ§Ã£o do SLA para administrador/gestor.\n- Homologar filtros de chamados atrasados.\n- Homologar indicadores gerenciais.\n- Registrar evidÃªncias formais com prints, data, ambiente e usuÃ¡rio de teste.", "- Validar cÃ¡lculo de horÃ¡rio comercial em cenÃ¡rio real com volume institucional.\n- Evoluir calendÃ¡rio por departamento/time quando a governanÃ§a estiver definida.\n- Evoluir importaÃ§Ã£o automÃ¡tica de feriados nacionais/municipais.\n- Evoluir regras de reabertura para reaproveitamento de prazo remanescente.\n- Refinar polÃ­tica de proximidade do vencimento por canal/time.\n- Implementar alertas/notificaÃ§Ãµes operacionais por SLA, se aplicÃ¡vel.\n- Consolidar trilha de auditoria e relatÃ³rios gerenciais de cumprimento.", "Executar homologaÃ§Ã£o funcional de ponta a ponta com usuÃ¡rios reais e validar regras de SLA em ambiente publicado, incluindo casos de pausa, reabertura e governanÃ§a operacional.", "Sprints 1, 2, 3 e 4 implementadas e validadas funcionalmente, com polÃ­ticas/metas, SLA aplicado aos chamados, alertas, eventos, monitoramento, painel gerencial e calendÃ¡rio corporativo para horÃ¡rio comercial." });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777706"),
                columns: new[] { "area", "atencao_tecnica", "categoria", "criterio_aceite", "objetivo", "pendencias_homologacao", "pendencias_tecnicas", "proxima_acao", "situacao_atual" },
                values: new object[] { "HistÃ³rico/Auditoria", "Auditoria nÃ£o Ã© log tÃ©cnico. ILogger continua para diagnÃ³stico tÃ©cnico; EventoAuditoria Ã© governanÃ§a/rastreabilidade. NÃ£o registrar senha, token JWT, refresh token, access token, client secret ou connection string. Dados sensÃ­veis devem ser mascarados pelo AuditoriaDiffHelper.", "GovernanÃ§a", "O item HistÃ³rico/Auditoria deve exibir checklist ativo completo das Sprints 1, 2 e 3 com cÃ¡lculo automÃ¡tico de percentual por checklist, status da implementaÃ§Ã£o Implementado funcionalmente e status tÃ©cnico Completo com pendÃªncias evolutivas, sem uso de percentual legado/manual.", "Criar trilha de auditoria para registrar aÃ§Ãµes relevantes executadas no SGX Sistema de Chamados, permitindo rastreabilidade, governanÃ§a, anÃ¡lise de alteraÃ§Ãµes, auditoria operacional e apoio Ã  homologaÃ§Ã£o.", "- Executar homologaÃ§Ã£o funcional com eventos reais em eventos_auditoria cobrindo Chamados, UsuÃ¡rios, SLA, AutenticaÃ§Ã£o e Roadmap ITSM.\n- Validar filtros e consulta administrativa em Admin > GovernanÃ§a > Auditoria com evidÃªncias formais.", "- ExportaÃ§Ã£o Excel/PDF.\n- RetenÃ§Ã£o configurÃ¡vel de auditoria.\n- Assinatura/hash da trilha de auditoria.\n- Alertas para eventos crÃ­ticos.\n- Painel avanÃ§ado de seguranÃ§a.\n- IntegraÃ§Ã£o SIEM/Log Analytics.\n- PolÃ­tica de anonimizaÃ§Ã£o/LGPD para eventos antigos.\n- Fluxo backend controlado de logout, se vier a existir.\n- Auditoria de ediÃ§Ã£o de documentaÃ§Ã£o ITSM, caso a documentaÃ§Ã£o deixe de ser estÃ¡tica.", "Executar homologaÃ§Ã£o funcional com eventos reais em eventos_auditoria, incluindo Chamados, UsuÃ¡rios, SLA, AutenticaÃ§Ã£o e Roadmap ITSM. Validar filtros e consulta administrativa em Admin > GovernanÃ§a > Auditoria.", "Base tÃ©cnica de auditoria criada, eventos de auditoria aplicados aos mÃ³dulos crÃ­ticos e tela administrativa de consulta implementada em Admin > GovernanÃ§a > Auditoria, com filtros, detalhe, indicadores e documentaÃ§Ã£o em GestÃ£o ITSM." });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777709"),
                columns: new[] { "atencao_tecnica", "categoria", "criterio_aceite", "objetivo", "observacao", "pendencias_homologacao", "pendencias_tecnicas", "proxima_acao", "situacao_atual" },
                values: new object[] { "Validar se os indicadores respeitam corretamente as permissÃµes internas do usuÃ¡rio autenticado. Confirmar se administradores visualizam a operaÃ§Ã£o completa e se atendentes visualizam apenas o escopo permitido, caso essa regra seja exigida. Verificar performance das consultas em bases maiores, principalmente filtros por perÃ­odo, produtividade por atendente e agrupamentos por status, prioridade e categoria. Garantir que chamados inativos, registros histÃ³ricos e dados de SLA sejam tratados corretamente para nÃ£o distorcer os indicadores.", "GestÃ£o", "O usuÃ¡rio autorizado deve conseguir acessar o Dashboard Administrativo e visualizar indicadores consolidados da operaÃ§Ã£o. Os filtros devem alterar os dados apresentados. Os cards principais devem exibir chamados abertos, em atendimento, aguardando solicitante, SLA vencido, prÃ³ximos do vencimento e resolvidos no perÃ­odo. A tela deve permitir navegaÃ§Ã£o para fila de chamados, gestÃ£o de chamados e integraÃ§Ã£o de e-mail. Os dados exibidos devem ser coerentes com os registros persistidos no sistema.", "Disponibilizar uma visÃ£o gerencial da operaÃ§Ã£o de chamados, permitindo que administradores e atendentes acompanhem em tempo real os principais indicadores do service desk, incluindo volume de chamados abertos, em atendimento, aguardando solicitante, resolvidos no perÃ­odo, chamados sem responsÃ¡vel, riscos de SLA, distribuiÃ§Ã£o por status, prioridade, categoria, produtividade por atendente e situaÃ§Ã£o da integraÃ§Ã£o de e-mail.", "Checklist ativo consolidado em 34/40 itens (85%), com pendÃªncias concentradas em policy granular, performance, testes HTTP/frontend e homologaÃ§Ã£o.", "- Validar com Administrador.\n- Validar com Atendente.\n- Conferir nÃºmeros do dashboard contra consultas reais no banco.\n- Validar filtros por perÃ­odo, departamento, categoria e responsÃ¡vel.\n- Confirmar se os indicadores atendem Ã  necessidade de gestÃ£o da operaÃ§Ã£o.\n- Registrar evidÃªncias formais de homologaÃ§Ã£o.", "- Aplicar ou validar permissÃ£o granular Dashboard.Visualizar no backend, alÃ©m da proteÃ§Ã£o por perfil.\n- Validar performance com volume maior de chamados.\n- Criar ou consolidar testes automatizados especÃ­ficos do dashboard em nÃ­vel HTTP.\n- Criar testes frontend/e2e para dashboardAdminService e AdminDashboardView, se o projeto jÃ¡ tiver estrutura para isso.\n- Avaliar cache ou otimizaÃ§Ã£o das consultas agregadas, caso necessÃ¡rio.\n- Revisar regras de permissÃ£o dos indicadores por perfil.", "Executar validaÃ§Ã£o tÃ©cnica e homologaÃ§Ã£o funcional do dashboard com dados reais ou massa simulada mais prÃ³xima da operaÃ§Ã£o institucional.", "Dashboard administrativo implementado funcionalmente no backend e frontend. A API disponibiliza indicadores consolidados, filtros por perÃ­odo e contexto administrativo. A interface apresenta cards gerenciais, grÃ¡ficos/listagens por status, prioridade e categoria, indicadores de SLA, produtividade por atendente, fila de chamados e resumo da integraÃ§Ã£o de e-mail. Pendente validaÃ§Ã£o com usuÃ¡rios reais, refinamento visual final, testes frontend/e2e e homologaÃ§Ã£o institucional." });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777721"),
                column: "percentual_implementacao",
                value: 6);

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777725"),
                columns: new[] { "atencao_tecnica", "evidencia_implementacao", "pendencias_homologacao", "pendencias_tecnicas", "percentual_implementacao", "proxima_acao", "situacao_atual", "status_implementacao" },
                values: new object[] { "Preservar a separacao entre fato de negocio, resolucao de destinatarios, materializacao de conteudo, geracao idempotente, processamento e entrega, mantendo fora do escopo aprovacao/SLA sem ponto estavel de notificacao nesta etapa.", "Eventos priorizados integrados ao pipeline de notificacoes via orquestrador interno, com pontos estaveis em abertura, atribuicao/assuncao, status relevante e encerramento; idempotencia por evento/destinatario/canal; testes unitarios, integracao e regressao; compatibilidade com frontend, processamento e canais Sistema/Email; sem SignalR, sem fila externa, sem outbox improvisada e sem alterar Worker.Email.", "Validar recebimento real por solicitante e responsavel, confirmar templates ativos no ambiente, revisar eventos adiados e registrar aceite institucional da Sprint 6.", "Executar homologacao funcional/manual da Sprint 6 com templates ativos no ambiente, cenarios reais por perfil e evidencias formais, sem antecipar item 16 nem ampliar escopo para todos os eventos, aprovacao completa ou SLA.", 94, "Documentar, homologar e registrar aceite da Sprint 6", "Notificacoes internas persistidas, inbox autenticada e central frontend concluida; eventos ITSM priorizados agora integram o pipeline de geracao idempotente sem entrega sincrona nem impacto indevido em abertura, atribuicao, status, encerramento ou fluxos legados.", 2 });

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777730"),
                column: "proxima_acao",
                value: "Definir modelo de Problema e integraÃ§Ãµes com incidente/mudanca.");

            migrationBuilder.UpdateData(
                table: "roadmap_itsm_itens",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777736"),
                column: "percentual_implementacao",
                value: 6);

            migrationBuilder.UpdateData(
                table: "sla_politicas",
                keyColumn: "id",
                keyValue: new Guid("56565656-5656-5656-5656-565656565601"),
                columns: new[] { "descricao", "nome" },
                values: new object[] { "PolÃ­tica inicial de SLA do SGX Sistema de Chamados, usada como base para controle de primeira resposta e resoluÃ§Ã£o dos chamados.", "SLA PadrÃ£o" });
        }
    }
}
