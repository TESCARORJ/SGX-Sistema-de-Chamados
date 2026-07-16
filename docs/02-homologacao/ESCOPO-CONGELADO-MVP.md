# Escopo Congelado da V1 Homologável — SGX Sistema de Chamados

## 1. Objetivo deste documento

Registrar formalmente qual conjunto de módulos e sprints ITSM compõe a **V1 homologável** do SGX Sistema de Chamados, para evitar que o roadmap aberto (21 sprints estratégicas) seja confundido com o escopo mínimo necessário para a primeira homologação institucional.

## 2. Fonte de verdade

Os percentuais vêm de `SeedData.cs` (`RoadmapItsmItem*`, campo `PercentualImplementacao`), validados por testes comportamentais (use case/integração) — não pelos testes de checklist isoladamente, que provaram ser tautológicos em pelo menos um caso (Sprint 7, corrigido em 15/07/2026). Em caso de divergência entre este documento, `docs/ROADMAP.md` e o código, **o código com teste comportamental prevalece**.

## 3. Critério de congelamento

Entram na V1 os itens que fazem parte do ciclo de vida essencial do chamado, ou que são o próprio processo de homologação/implantação da V1, com `PercentualImplementacao >= 90%` no momento do congelamento, ou já entregues como base funcional fora da numeração de sprint ITSM.

## 4. Escopo IN — V1 homologável

### 4.1 Núcleo já entregue
Autenticação (Entra ID / AD / LocalSgx / LocalDevelopment), perfis e permissões, portal do solicitante, atendimento administrativo, SLA base, dashboard administrativo, cadastros administrativos, auditoria/governança, worker de e-mail (IMAP).

### 4.2 Sprints ITSM numeradas
| Sprint | Área | % real | Pendência para 100% |
|---|---|---|---|
| 1 | Fundação ITSM do chamado | 100% | — |
| 3 | Grupos técnicos, filas e atribuição | 90% | Roteiro de homologação (produtividade por grupo, visibilidade por fila, aceite final) — nenhum artefato criado ainda |
| 4 | Motor de Aprovações ITSM | 94% | Roteiro de homologação (4 itens) |
| 5 | Fechamento, aceite e reabertura | 100% | Homologação formal com evidências |
| 6 | Notificações ITSM | 94% | Homologação visual manual (telas em 320px/375px/768px/desktop) — suíte automatizada 100% verde, falta navegação manual |
| 7 | Gerenciamento de Requisições | **100%** | — (corrigido em 15/07/2026: itens de grupo responsável, formulário por serviço e persistência de respostas já estavam implementados desde a Sprint 8; era divergência de documentação, não código faltante) |
| 8 | Catálogo de Serviços 2.0 | 96% | Homologação (3 itens) + documento de homologação funcional pendente de preenchimento |
| 20 | Homologação institucional ITSM | 75% | É o próprio processo de aceite |
| 21 | Produto, implantação e operação | 25% | Escopo reduzido — só o necessário para publicar o ambiente de homologação |

## 5. Escopo OUT — roadmap pós-MVP (adiado, não descartado)

Sprints 2, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 — ver detalhamento completo no roadmap Notion do projeto.

## 6. Ordem de execução recomendada (revisada em 15/07/2026)

1. ~~Sprint 7~~ — **concluída em 15/07/2026.** Não tinha débito de desenvolvimento real; era documentação/seed desatualizados.
2. Execução dos roteiros de homologação das Sprints 3, 4, 5, 6, 8 — nenhuma tem código pendente, todas confirmadas por auditoria direta do código (não apenas do seed) em 15/07/2026.
3. Sprint 21 com escopo reduzido — publicação do ambiente de homologação.
4. Sprint 20 — consolidação e aceite formal da V1.

## 7. Governança de processo (aprendizado da Sprint 7)

- Scripts de edição de seed via regex foram descontinuados e arquivados em `/tools/legacy-scripts/` (15/07/2026) — causaram a divergência da Sprint 7 e a corrupção de `docs/ROADMAP.md`.
- Testes de checklist de roadmap (`Roadmap*ChecklistTests.cs`) não são prova de comportamento sozinhos — devem referenciar/exigir os testes comportamentais correspondentes (ajustado no teste da Sprint 7 em 15/07/2026, replicar para as demais sprints ao longo do Marco 2).
- Toda alteração de item de roadmap deve citar o teste comportamental que a comprova, não só a flag do seed.

## 8. Histórico de revisão

| Data | Alteração |
|---|---|
| 14/07/2026 | Criação do documento. Congelamento inicial de escopo. |
| 15/07/2026 | Corrigida a Sprint 7: não tinha débito de desenvolvimento, era divergência de documentação/seed. Confirmado via auditoria de código real pelo Claude Code. Registrada descontinuação dos scripts de seed via regex. |
