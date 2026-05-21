# Dashboard / Gestão

## Objetivo

Disponibilizar uma visão gerencial da operação de chamados, permitindo que administradores e atendentes acompanhem em tempo real os principais indicadores do service desk, incluindo volume de chamados abertos, em atendimento, aguardando solicitante, resolvidos no período, chamados sem responsável, riscos de SLA, distribuição por status, prioridade, categoria, produtividade por atendente e situação da integração de e-mail.

## Escopo implementado

### Backend

- `GET /api/admin/dashboard`
- `GET /api/admin/indicadores/chamados-por-status`
- `GET /api/admin/indicadores/chamados-por-prioridade`
- `GET /api/admin/indicadores/chamados-por-categoria`
- `GET /api/admin/indicadores/sla`
- `GET /api/admin/indicadores/produtividade`

Arquivos principais:

- `src/SGX.SistemaChamado.Api/Controllers/AdminDashboardController.cs`
- `src/SGX.SistemaChamado.Application/Interfaces/Admin/IAdminIndicadoresUseCases.cs`
- `src/SGX.SistemaChamado.Application/UseCases/Admin/AdminIndicadoresUseCases.cs`
- `src/SGX.SistemaChamado.Application/DTOs/Admin/AdminDashboardDtos.cs`
- `src/SGX.SistemaChamado.Application/Validators/FiltroIndicadoresRequestValidator.cs`
- `src/SGX.SistemaChamado.Infrastructure/DependencyInjection.cs`

### Frontend

- Tela administrativa em `/admin`
- Filtros por período, departamento, categoria e responsável
- Cards gerenciais
- Indicadores por status, prioridade e categoria
- Indicadores de SLA
- Produtividade por atendente
- Fila resumida de chamados
- Resumo da integração de e-mail

Arquivos principais:

- `src/SGX.SistemaChamado.Web/src/services/dashboardAdminService.ts`
- `src/SGX.SistemaChamado.Web/src/types/dashboard.ts`
- `src/SGX.SistemaChamado.Web/src/types/indicadores.ts`
- `src/SGX.SistemaChamado.Web/src/views/AdminDashboardView.vue`
- `src/SGX.SistemaChamado.Web/src/router/index.ts`
- `src/SGX.SistemaChamado.Web/src/layouts/AdminLayout.vue`
- `src/SGX.SistemaChamado.Web/src/constants/permissoes.ts`

## Autorização e acesso

- O backend está protegido por perfil (`Policies.AdminOuAtendente`) no controller de dashboard.
- A rota frontend `/admin` exige perfis `Administrador` ou `Atendente`.
- O menu frontend controla exibição por permissão `Dashboard.Visualizar`.

Pendência evolutiva mapeada:

- aplicar/validar policy granular `Dashboard.Visualizar` também no backend do dashboard, além da proteção por perfil.

## Testes automatizados existentes

- `tests/SGX.SistemaChamado.Tests/DashboardAdminUseCaseTests.cs`
- `tests/SGX.SistemaChamado.Tests/IndicadoresUseCaseTests.cs`
- `tests/SGX.SistemaChamado.Tests/ApiHttpIntegrationTests.cs` (bloqueio de solicitante para `/api/admin/dashboard`)

## Status consolidado (Roadmap ITSM)

- Status da implementação: `Implementado funcionalmente`
- Status técnico: `Completo com pendências evolutivas`
- Percentual: `85%`
- Checklist ativo: `40 itens` (`34 concluídos`, `6 pendentes`)

## Pendências técnicas

- Aplicar ou validar permissão granular `Dashboard.Visualizar` no backend, além da proteção por perfil.
- Validar performance com volume maior de chamados.
- Criar ou consolidar testes automatizados específicos do dashboard em nível HTTP.
- Criar testes frontend/e2e para `dashboardAdminService` e `AdminDashboardView`, se o projeto já tiver estrutura para isso.
- Avaliar cache ou otimização das consultas agregadas, caso necessário.
- Revisar regras de permissão dos indicadores por perfil.

## Pendências de homologação

- Validar com Administrador.
- Validar com Atendente.
- Conferir números do dashboard contra consultas reais no banco.
- Validar filtros por período, departamento, categoria e responsável.
- Confirmar se os indicadores atendem à necessidade de gestão da operação.
- Registrar evidências formais de homologação.

## Critério de aceite

O usuário autorizado deve conseguir acessar o Dashboard Administrativo e visualizar indicadores consolidados da operação. Os filtros devem alterar os dados apresentados. Os cards principais devem exibir chamados abertos, em atendimento, aguardando solicitante, SLA vencido, próximos do vencimento e resolvidos no período. A tela deve permitir navegação para fila de chamados, gestão de chamados e integração de e-mail. Os dados exibidos devem ser coerentes com os registros persistidos no sistema.

## Próxima ação

Executar validação técnica e homologação funcional do dashboard com dados reais ou massa simulada mais próxima da operação institucional.
