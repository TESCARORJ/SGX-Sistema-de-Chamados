# Sprint 4 - Item 46: Exibir status de aprovação no detalhe do chamado

## Objetivo
Atender ao item 46 da Sprint 4 (Motor de Aprovações ITSM), adicionando a exibição do status das aprovações pendentes provenientes do novo motor ITSM na tela de detalhe administrativo do chamado (`AdminDetalheChamadoView.vue`).

## Implementação Realizada

### 1. Types
- Criado o arquivo `src/SGX.SistemaChamado.Web/src/types/aprovacoesMotor.ts`.
- Adicionados os enumeradores `StatusInstanciaAprovacaoChamado`, `TipoRegraAprovacao` e `EfeitoOperacionalRegraAprovacao`.
- Criadas as interfaces de resposta `InstanciaAprovacaoChamadoResumoResponse` e `InstanciaAprovacaoChamadoResponse`.

### 2. Services
- Criado o arquivo `src/SGX.SistemaChamado.Web/src/services/aprovacoesMotorService.ts` contendo a função `listarPendenciasPorChamado(chamadoId: string)` que aponta para `GET /api/admin/aprovacoes-motor/chamados/{chamadoId}/pendencias`.

### 3. View (`AdminDetalheChamadoView.vue`)
- **Separação UI:** A exibição legada de "Aprovacao" foi renomeada para "Aprovacao legada" para preservar o comportamento atual do sistema e evitar regressões, conforme diretrizes do projeto.
- **Nova Seção:** Criada uma nova `AppSectionCard` intitulada "Aprovacao do motor ITSM", exibindo todas as pendências daquele chamado.
- **Banners de Contexto:**
  - `bg-orange-1`: Exibido quando existe uma aprovação com efeito de "Bloqueia Atendimento" ou "Bloqueia Encerramento" (Bloqueante) pendente.
  - `bg-info`: Exibido quando existe aprovação puramente informativa pendente.
  - `bg-amber-1`: Exibido quando o status é `EmReavaliacao`.
  - `bg-red-1`: Exibido quando há reprovação registrada no motor.
  - `bg-green-1`: Exibido quando todas as pendências ativas do motor encontram-se aprovadas.
- **Listagem detalhada:** Um `q-list` renderiza:
  - Status (usando badges com cores indicativas);
  - Nome da regra de aprovação;
  - Origem / Tipo da Regra (Ex: Automática, Grupo Aprovador);
  - Efeito Operacional (Bloqueante, Informativo, etc.);
  - Data e Prazo de Vencimento.
- **Integração:** Adicionado método `carregarAprovacoesMotor()` que é chamado de forma sequencial após o load principal e após as ações na seção de aprovação legada para garantir atualização reativa.

### 4. Roadmap e Seed
- `PercentualImplementacao` atualizado para `68%` em `SeedData.cs`.
- `Item 46` atualizado para `Concluido = true`.
- Próxima Ação definida como `"Exibir pendencias de aprovacao para aprovador."` (Item 47).
- Teste unitário de checklist `RoadmapSprint4MotorAprovacoesChecklistTests.cs` atualizado e passando com sucesso.

## Próximos Passos
Prosseguir para o Item 47 da checklist da Sprint 4: "Exibir pendências de aprovação para aprovador", onde implementaremos uma listagem específica e consolidada de chamados aguardando a decisão de um usuário logado.
