# Escopo Congelado da V1 Homologável — SGX Sistema de Chamados

## 1. Objetivo deste documento

Registrar formalmente qual conjunto de módulos e sprints ITSM compõe a **V1 homologável** do SGX Sistema de Chamados, para evitar que o roadmap aberto (21 sprints estratégicas) seja confundido com o escopo mínimo necessário para a primeira homologação institucional.

## 2. Fonte de verdade

Os percentuais vêm de `SeedData.cs` (`RoadmapItsmItem*`, campo `PercentualImplementacao`), validados por testes comportamentais (use case/integração) — não pelos testes de checklist isoladamente, que provaram ser tautológicos em pelo menos um caso (Sprint 7, corrigido em 15/07/2026). Em caso de divergência entre este documento, `docs/ROADMAP.md` e o código, **o código com teste comportamental prevalece**.

## 3. Critério de congelamento (revisado em 16/07/2026)

**Mudança de critério:** o escopo da V1 deixou de ser definido por percentual de conclusão e passou a ser definido por **necessidade funcional para a demo/pitch de substituição do GLPI**. Percentual alto não garante entrada no V1 (Sprint 8 está em 96% e é diferencial, não núcleo); percentual baixo não garante exclusão se a funcionalidade for essencial para a demo (nenhum caso assim identificado até agora, mas o critério permanece).

Uma sprint entra na V1 se, e somente se, se enquadrar em um destes três grupos:
- **Núcleo obrigatório**: sem isso a demo não existe — paridade básica esperada de qualquer ferramenta de service desk.
- **Diferencial**: argumento concreto de "isso é melhor que o GLPI", usado para convencer a decisão de troca.
- **Processo**: não é feature, é o próprio ato de homologar/implantar a V1.

Tudo que não se enquadra em nenhum desses três é roadmap pós-MVP — maturidade ITIL avançada que não fecha a decisão de troca agora, independente do percentual técnico.

## 4. Escopo IN — V1 homologável

### 4.1 Núcleo já entregue (base funcional, fora da numeração de sprint ITSM)
Autenticação (Entra ID / AD / LocalSgx / LocalDevelopment), perfis e permissões, portal do solicitante, atendimento administrativo, SLA base, dashboard administrativo, cadastros administrativos, auditoria/governança, worker de e-mail (IMAP).

### 4.2 Sprints ITSM numeradas, por categoria funcional

| Categoria | Sprint | Racional |
|---|---|---|
| Núcleo obrigatório | 1 — Fundação ITSM do chamado | Objeto central do sistema |
| Núcleo obrigatório | 3 — Grupos técnicos, filas e atribuição | Paridade básica com GLPI, roteamento por equipe |
| Núcleo obrigatório | 5 — Fechamento, aceite e reabertura | Ciclo de vida básico esperado |
| Núcleo obrigatório | 6 — Notificações ITSM | Sem isso o sistema "parece quebrado" numa demo — ⚠️ status técnico atual inconsistente, ver seção 8 |
| Diferencial | 4 — Motor de Aprovações ITSM | GLPI não tem fluxo de aprovação estruturado — argumento de venda direto |
| Diferencial | 7 — Gerenciamento de Requisições | Portal guiado por catálogo, visualmente superior ao formulário genérico do GLPI |
| Diferencial | 8 — Catálogo de Serviços 2.0 | Pré-requisito técnico da Sprint 7 |
| Diferencial | 18 — Base de Conhecimento 2.0 | Argumento de ROI para diretoria ("reduz volume de chamado") — entra apesar do percentual baixo |
| Processo | 20 — Homologação institucional ITSM | O próprio ato de validar a V1 |
| Processo | 21 — Produto, implantação e operação | Sem isso o V1 não sai do notebook de ninguém (escopo reduzido — só o necessário para homologação) |

## 5. Escopo OUT — roadmap pós-MVP (adiado, não descartado)

Sprints 2, 9, 10, 11, 12, 13, 14, 15, 16, 17, 19 — nenhuma delas é núcleo, diferencial ou processo de entrega da V1, independente do percentual técnico. Detalhamento completo no roadmap Notion do projeto.

Nota: incidente básico, SLA básico, dashboard básico e Base de Conhecimento (fundação) já fazem parte do núcleo entregue (seção 4.1) — o que fica fora é a evolução "2.0"/avançada dessas capacidades (Sprints 9, 14, 17).

## 6. Ordem de execução recomendada (revisada em 16/07/2026 — critério funcional, não percentual)

Critério de ordenação: risco primeiro, núcleo antes de diferencial, e dentro do diferencial quem precisa de mais trabalho real começa mais cedo.

| Fase | Sprint(s) | Motivo da ordem |
|---|---|---|
| 0 — Destravar risco | 6 | Status técnico "Bloqueado" (não só incompleto). Núcleo obrigatório. Investigar causa raiz antes de alocar qualquer outro esforço com confiança |
| 1 — Fechar núcleo quase pronto | 1, 3, 5 | 94–100%, só falta homologação formal — tração rápida |
| 2 — Diferencial já avançado | 7, 4 | 100% e 94% — fechar homologação e ter a demo mais forte pronta cedo |
| 3 — Diferencial com trabalho real pendente | 8, 18 | Competem por desenvolvimento de verdade, não só homologação — entram cedo, não no fim |
| 4 — Consolidação | 20 | Só faz sentido depois que as fases 0–3 fecharem |
| 5 — Entrega | 21 | Implantação é sempre o último passo (dividir em "preparar ambiente", que pode começar em paralelo à Fase 2/3, e "publicar", que é o passo final) |

## 7. Governança de processo (aprendizado da Sprint 7)

- Scripts de edição de seed via regex foram descontinuados e arquivados em `/tools/legacy-scripts/` (15/07/2026) — causaram a divergência da Sprint 7 e a corrupção de `docs/ROADMAP.md`.
- Testes de checklist de roadmap (`Roadmap*ChecklistTests.cs`) não são prova de comportamento sozinhos — devem referenciar/exigir os testes comportamentais correspondentes.
- Toda alteração de item de roadmap deve citar o teste comportamental que a comprova, não só a flag do seed.

## 8. Divergências sob investigação (16/07/2026) — não confiar nestes números sem reauditoria

- **Sprint 2**: percentual saltou de 25% para 85% sem auditoria de código correspondente registrada. Continua fora do escopo IN pelo critério funcional (não é núcleo, diferencial nem processo), mas o salto merece checagem antes de usar esse número em qualquer outra decisão.

## 9. Histórico de revisão

| Data | Alteração |
|---|---|
| 14/07/2026 | Criação do documento. Congelamento inicial de escopo, critério por percentual. |
| 15/07/2026 | Corrigida a Sprint 7: não tinha débito de desenvolvimento, era divergência de documentação/seed. Confirmado via auditoria de código real pelo Claude Code. Registrada descontinuação dos scripts de seed via regex. |
| 16/07/2026 | Critério de congelamento trocado de percentual para funcional (necessidade para demo de substituição do GLPI). Sprint 18 entra no escopo IN apesar do percentual baixo. Ordem de execução reorganizada em fases (0–5). Registradas divergências sob investigação nas Sprints 2, 6 e 8. |
| 17/07/2026 | Corrigida a Sprint 6: não tinha débito de desenvolvimento, era divergência de documentação/seed (mesma causa raiz da Sprint 7). Seed desatualizado apontava `StatusImplementacao = EmDesenvolvimento` e `StatusTecnico = Bloqueado`; corrigido para `ImplementadoFuncionalmente` e `CompletoComPendenciasEvolutivas`, alinhado a 209/209 testes de lógica pura passando e à integração do `ProcessarEventoCandidatoNotificacaoUseCase` a 5 use cases de chamado. Falta apenas homologação visual/manual e registro de aceite formal. |
| 20/07/2026 | Corrigida a Sprint 8: não houve queda real de 96% para 93% — era erro de redação deste documento (seções 3 e 8), que citou um valor histórico intermediário do log de progresso (item 64 do checklist, 07/07/2026) como se fosse o estado atual. Auditoria completa confirmou 73/76 itens do checklist concluídos (mesmos 3 pendentes de homologação desde 14/07/2026), 277 testes comportamentais relevantes passando (119 de CatálogoServico + 158 de fluxos relacionados), zero falhas. Nenhuma mudança de código, seed ou schema foi necessária. |
