# Escopo Congelado da V1 Homologável — SGX Sistema de Chamados

## 1. Objetivo deste documento

Registrar formalmente qual conjunto de módulos e sprints ITSM compõe a **V1 homologável** (MVP V1) do SGX Sistema de Chamados, para evitar que o roadmap aberto (21 sprints estratégicas) seja confundido com o escopo mínimo necessário para a primeira homologação institucional, e para eliminar divergências entre este documento, o roadmap e o código quando elas surgirem.

## 2. Data do congelamento

- **Congelamento inicial**: 14/07/2026.
- **Última revisão de escopo (nesta atualização)**: 22/07/2026 — resolvida a inconsistência da Sprint 18 (Base de Conhecimento 2.0), que estava classificada como IN ("Diferencial") na revisão de 16/07/2026 sem que uma dependência técnica bloqueante tivesse sido demonstrada. Ver seção 7.
- Este documento é vivo apenas quanto à seção 15 (histórico de revisão) e às correções de divergência; o escopo IN/OUT em si só muda seguindo a seção 12 (regras de mudança futura de escopo).

## 3. Hierarquia de evidências

Em caso de divergência entre este documento, `docs/ROADMAP.md`, `docs/ROADMAP-ITSM.md` e o código, a evidência prevalece na seguinte ordem (da mais forte para a mais fraca):

1. Código implementado (Domain/Application/Infrastructure/Api/Web).
2. Testes de regra de negócio, aplicação, API e frontend (comportamentais).
3. Build da solução (backend e frontend).
4. Verificação do modelo EF Core (`dotnet ef migrations has-pending-model-changes`).
5. Evidência de homologação registrada (com responsável, data e ambiente).
6. `SeedData.cs` e testes de checklist de roadmap (`Roadmap*ChecklistTests.cs`) — indicam intenção declarada, não comprovam sozinhos que o comportamento existe.
7. Documentação narrativa (este documento, `docs/ROADMAP.md`, `docs/ROADMAP-ITSM.md`).

Percentual de `SeedData.cs` nunca é usado isoladamente para decidir se algo está pronto — precisa estar amparado por pelo menos os itens 1–4 desta lista.

## 4. Critérios de inclusão e exclusão

**Mudança de critério (16/07/2026, mantida nesta revisão):** o escopo da V1 não é definido por percentual de conclusão, e sim por **necessidade funcional para a demo/pitch de substituição do GLPI**. Percentual alto não garante entrada no V1 (Sprint 8 está em 96% e é diferencial, não núcleo); percentual baixo não garante exclusão automática, mas exige demonstrar dependência técnica real e bloqueante para justificar a entrada (nenhum caso assim identificado até esta revisão — a Sprint 18 foi avaliada explicitamente contra este critério na seção 7 e não se enquadrou).

Uma sprint entra na V1 se, e somente se, se enquadrar em um destes três grupos:

- **Núcleo obrigatório**: sem isso a demo não existe — paridade básica esperada de qualquer ferramenta de service desk.
- **Diferencial**: argumento concreto de "isso é melhor que o GLPI", usado para convencer a decisão de troca — mas só entra se o código já demonstrar o diferencial ou se houver dependência técnica bloqueante comprovada (não intenção declarada em seed ou documentação).
- **Processo**: não é feature, é o próprio ato de homologar/implantar a V1.

Tudo que não se enquadra em nenhum desses três grupos, ou que só se enquadraria por argumento de ROI/estratégia sem lastro técnico demonstrado, é roadmap pós-MVP — maturidade ITIL avançada que não fecha a decisão de troca agora, independente do percentual técnico.

## 5. Escopo IN — V1 homologável

### 5.1 Núcleo já entregue (base funcional, fora da numeração de sprint ITSM)

Autenticação (Entra ID / AD / LocalSgx / LocalDevelopment), perfis e permissões, portal do solicitante, atendimento administrativo, comentários/anexos/histórico, SLA base, dashboard administrativo, cadastros administrativos, auditoria/governança, worker de e-mail (IMAP), **Base de Conhecimento (fundação)** — `RoadmapItsmItem10Id`, categoria "Conhecimento", 90%, artigo + vínculo opcional ao chamado (`ChamadoArtigoConhecimento`) já implementados e testados, independente da numeração de sprint ITSM.

### 5.2 Sprints ITSM numeradas, por categoria funcional

| Categoria | Sprint | Racional |
|---|---|---|
| Núcleo obrigatório | 1 — Fundação ITSM do chamado | Objeto central do sistema |
| Núcleo obrigatório | 3 — Grupos técnicos, filas e atribuição | Paridade básica com GLPI, roteamento por equipe |
| Núcleo obrigatório | 5 — Fechamento, aceite e reabertura | Ciclo de vida básico esperado |
| Núcleo obrigatório | 6 — Notificações ITSM | Sem isso o sistema "parece quebrado" numa demo |
| Diferencial | 4 — Motor de Aprovações ITSM | GLPI não tem fluxo de aprovação estruturado — argumento de venda direto |
| Diferencial | 7 — Gerenciamento de Requisições | Portal guiado por catálogo, visualmente superior ao formulário genérico do GLPI |
| Diferencial | 8 — Catálogo de Serviços 2.0 | Pré-requisito técnico da Sprint 7 |
| Processo | 20 — Homologação institucional | O próprio ato de validar a V1 |
| Processo | 21 — Produto, implantação e operação | Sem isso o V1 não sai do notebook de ninguém (escopo reduzido — só o necessário para disponibilizar e sustentar o ambiente de homologação) |

## 6. Escopo OUT — roadmap pós-MVP (adiado, não descartado)

Sprints 2, 9, 10, 11, 12, 13, 14, 15, 16, 17, **18**, 19 — nenhuma delas é núcleo, diferencial comprovado ou processo de entrega da V1, independente do percentual técnico. Detalhamento completo no roadmap Notion do projeto.

- **Sprint 9** (46%) está explicitamente fora do escopo funcional principal do MVP — Gerenciamento de Incidentes avançado não é pré-requisito de nenhum fluxo essencial (incidente básico já faz parte do núcleo entregue).
- **Sprint 18** (50%) — Base de Conhecimento 2.0 — decisão detalhada na seção 7.

Nota: incidente básico, SLA básico, dashboard básico e Base de Conhecimento (fundação, seção 5.1) já fazem parte do núcleo entregue — o que fica fora é a evolução "2.0"/avançada dessas capacidades (Sprints 9, 14, 17, 18).

## 7. Decisão sobre a Sprint 18 — Base de Conhecimento 2.0

**Decisão: Sprint 18 permanece FORA do MVP V1 (pós-MVP).**

**Verificação realizada:** nenhuma jornada essencial do MVP (abertura de chamado, atendimento, aprovação, fechamento, notificação, SLA, dashboard, catálogo de serviços) depende tecnicamente da Sprint 18 para funcionar. Evidência:

- A Base de Conhecimento **fundação** (artigo, publicação, consulta no portal, vínculo opcional ao chamado) já existe como módulo independente e entregue (`RoadmapItsmItem10Id`, 90%, categoria "Conhecimento") — não faz parte da numeração de sprint ITSM e não é o que a Sprint 18 propõe evoluir.
- Em `Chamado.cs`, o vínculo com artigos (`ArtigosConhecimento`) é uma coleção opcional (`ICollection<ChamadoArtigoConhecimento>`), sem qualquer obrigatoriedade ou acoplamento que bloqueie abrir, atender, aprovar, encerrar ou notificar um chamado.
- Nas buscas e arquivos analisados nesta etapa, não foram identificadas implementações das capacidades específicas da Sprint 18, como sugestão contextual por serviço/erro conhecido, workflow editorial e avaliação de utilidade (busca por `SugestaoContextual`, `WorkflowEditorial`, `ErroConhecido`, `AvaliacaoUtilidade` no código-fonte não retornou nenhum resultado). Não há evidência de dependência bloqueante de uma capacidade cuja implementação não foi localizada nesta análise.
- `RoadmapSprint18` não possui teste de checklist dedicado (`Roadmap*ChecklistTests.cs`); o único teste que referencia a Base de Conhecimento (`RoadmapBaseConhecimentoConsistencyTests.cs`) valida o item de **fundação** (`RoadmapItsmItem10Id`), não a Sprint 18 (`RoadmapItsmItem33Id`).

**Conclusão do tratamento obrigatório:** não há dependência bloqueante. Por isso:

- a Sprint 18 é mantida fora do MVP (roadmap pós-MVP, junto das Sprints 2, 9–17, 19);
- ela pertence à evolução ITIL avançada, não à V1;
- **nenhuma auditoria funcional completa da Sprint 18 foi realizada nesta etapa** — o tratamento se limitou a confirmar a ausência de dependência crítica, conforme instruído;
- **nenhum item novo da Sprint 18 foi implementado** nesta etapa;
- o percentual (50%) e o status (`ImplementadoFuncionalmente` / `CompletoComPendenciasEvolutivas`) em `SeedData.cs` **não foram alterados** — permanecem inalterados;
- esta etapa não encontrou evidência suficiente para confirmar que esses valores representam as capacidades específicas da Sprint 18 — Base de Conhecimento 2.0 (sugestão contextual, workflow editorial, avaliação de utilidade); a divergência permanece registrada na seção 14 para auditoria técnica pós-MVP;
- a decisão de manter a Sprint 18 fora do MVP (acima) independe desse percentual;
- esta análise não foi uma auditoria funcional completa da Sprint 18 e não deve produzir conclusão absoluta além da ausência de dependência bloqueante para o MVP.

Isso corrige a divergência introduzida na revisão de 16/07/2026, que havia classificado a Sprint 18 como "Diferencial" por argumento de ROI ("reduz volume de chamado") sem lastro técnico — em desacordo com o próprio critério de inclusão da seção 4, que exige comprovação, não intenção declarada.

## 8. Ordem de execução recomendada (dentro do escopo IN)

Critério de ordenação: o bloqueador técnico conhecido (Sprint 4, item 65 — seção 10) fecha primeiro, porque nenhuma homologação por jornada integrada é confiável enquanto o motor de aprovações novo não estiver ligado ao chamado real. Só depois disso faz sentido auditar tecnicamente as demais sprints, preparar o ambiente mínimo e homologar por jornadas.

| Fase | Escopo | Ações |
|---|---|---|
| 0 — Fechar bloqueador técnico | Sprint 4 | Implementar a integração end-to-end da aprovação automática por regra simples com o chamado real; adicionar testes comportamentais; preservar a aprovação legada (`AprovacaoChamado`) funcionando durante a transição; homologar o fluxo; concluir legitimamente os itens 65 e 68 do checklist quando houver evidência de código, teste e homologação. |
| 1 — Auditoria técnica das demais sprints IN | Sprints 1, 3, 5, 6, 7 e 8 | Confirmar, sprint a sprint, código implementado, testes comportamentais, frontend e ausência de outros bloqueadores técnicos além do já identificado na Sprint 4 — nenhuma dessas sprints deve ser tratada como tecnicamente fechada só pelo checklist/`SeedData.cs` (seção 9). |
| 2 — Preparar ambiente mínimo | Sprint 21 (recorte mínimo) | Disponibilizar o ambiente de homologação institucional (seção 4 do `PLANO-HOMOLOGACAO-PRODUTO.md`) — pode começar em paralelo à Fase 1. |
| 3 — Homologação por jornadas | Sprints 1, 3, 4, 5, 6, 7 e 8 | Executar os cenários integrados de cada sprint (roteiro da seção 9 do `PLANO-HOMOLOGACAO-PRODUTO.md`) e registrar evidências reais (responsável, data, ambiente, resultado, prints) — não avança para uma sprint sem que a Fase 0 (se for a Sprint 4) e a Fase 1 correspondente já tenham fechado. |
| 4 — Consolidação | Sprint 20 | Reconciliar resultados de homologação, defeitos encontrados, aceite formal e decisão de go/no-go da V1. |
| 5 — Produção | Sprint 21 (publicação) | Executar somente após a homologação (Fase 3) e as validações operacionais (seção 11 do `PLANO-HOMOLOGACAO-PRODUTO.md`) estarem concluídas; o ambiente de homologação não deve ser tratado como produção. |

## 9. Critérios de aceite do MVP V1

O MVP V1 é considerado apto a seguir para homologação institucional quando, para cada sprint do escopo IN (seção 5):

- o código da entrega principal está presente e não é apenas planejamento (`StatusImplementacao` em `ImplementadoFuncionalmente` ou superior);
- existem testes comportamentais (use case/integração) cobrindo as regras centrais da sprint, não apenas testes de checklist;
- o build da solução (backend `dotnet build` e frontend) passa sem erro;
- não há `pending model changes` no EF Core não intencionais;
- as pendências remanescentes são exclusivamente de **homologação formal** (registro de evidência, aceite), não de desenvolvimento.

**Situação por sprint nesta revisão (22/07/2026) — ver ressalva de escopo da validação no início da seção 10:**

- **Sprints 1, 3, 5, 6 e 7**: situação funcional indicada pelo estado atual do roadmap (checklist técnico e status em `SeedData.cs`), ainda sujeita à auditoria técnica específica de cada sprint e à homologação formal — nesta etapa não foi executada a suíte comportamental completa de cada uma individualmente, apenas os testes de checklist de roadmap (ver seção 10).
- **Sprint 8**: o roadmap atual (`SeedData.cs`) informa 73/76 itens do checklist concluídos; os 3 itens conhecidos pendentes (74, 75 e 76) são de homologação (registrar homologação funcional, homologação visual responsiva e aceite formal, respectivamente), não de desenvolvimento. Isso não significa que a sprint esteja tecnicamente completa: ela ainda deve passar pela auditoria técnica específica (código e testes comportamentais) e pelo build/teste do frontend antes de ser considerada pronta para homologação — esta revisão documental não comprovou que inexistem outras lacunas além das 3 conhecidas (ver seção 10).
- **Sprint 4**: **não atende a este critério** — é funcionalmente parcial para o escopo do novo motor de aprovações. O motor novo (`ConfiguracaoRegraAprovacao` + `InstanciaAprovacaoChamado`) ainda não está integrado ao fluxo real de abertura de chamado: `AbrirChamadoUseCase` continua criando somente o motor legado (`AprovacaoChamado`). O item 65 do checklist (aprovação automática por regra simples aplicada ao chamado real) segue pendente de desenvolvimento, não apenas de homologação (detalhe na seção 10).
- **Sprint 20**: processo de homologação institucional pendente de execução.
- **Sprint 21**: ambiente e implantação mínima pendentes de disponibilização.

## 10. Bloqueadores para homologação

**Bloqueador técnico funcional aberto — Sprint 4 (item 65):** o motor de aprovações novo (`ConfiguracaoRegraAprovacao` + `InstanciaAprovacaoChamado`) ainda não está ligado ao fluxo real de criação do chamado. `AbrirChamadoUseCase` cria somente `AprovacaoChamado` (motor legado); não existe caminho end-to-end do novo motor aplicado ao chamado real (confirmado em `AbrirChamadoUseCase.cs` e no commit `30e9738`, 22/07/2026 — sem commit posterior que resolva o item). O aceite final da Sprint 4 (item 68) depende da implementação, dos testes e da homologação desse fluxo. Aprovação por grupo (item 66) e multinível (item 67) continuam pós-MVP (`Obrigatorio = false`); a aprovação legada deve continuar preservada e funcional durante a transição.

**Escopo real das validações desta etapa (22/07/2026):** 36/36 testes executados com filtro `FullyQualifiedName~Roadmap` passaram, build do backend (`dotnet build`) sem erro, ausência de `pending model changes` no EF Core. Esse filtro seleciona os testes cujo nome contém "Roadmap" — inclui os testes de checklist (`Roadmap*ChecklistTests.cs`) e também testes de consistência de roadmap (ex.: `RoadmapBaseConhecimentoConsistencyTests.cs`); não se pode afirmar que os 36 eram exclusivamente testes de checklist. Não foram executados nesta etapa os testes comportamentais completos de cada sprint individualmente, nem build ou testes do frontend. Portanto, além do bloqueador da Sprint 4 acima, não se pode afirmar que não existem outros bloqueadores técnicos no escopo IN — apenas que esta verificação pontual não encontrou nenhum outro.

Bloqueadores de **processo** (não de código), pendentes em todas as sprints do núcleo/diferencial:

- homologação funcional formal não executada (Sprints 1, 3, 4, 6 e 8 — Sprint 5 sem pendência registrada, Sprint 7 com homologação revertida em 20/07/2026 por falta de evidência real);
- homologação visual/responsiva não executada onde aplicável;
- aceite formal (registro de responsável, data, ambiente e resultado) não registrado para nenhuma sprint do escopo IN;
- ambiente de homologação institucional (seção 4 do `PLANO-HOMOLOGACAO-PRODUTO.md`) ainda não disponibilizado — pré-requisito da Sprint 21 mínima.

**Homologação manual não deve ser tratada como executada** só porque um documento ou checklist foi preenchido — precisa de evidência (prints, roteiro executado, responsável, data) conforme seção 8 do `PLANO-HOMOLOGACAO-PRODUTO.md`. O histórico da Sprint 7 (seção 15, entrada de 20/07/2026) é o exemplo registrado do que acontece quando essa regra é violada.

## 11. Definições formais

- **Tecnicamente completo**: o código da entrega principal existe, cobre as regras de negócio da sprint, está coberto por teste comportamental (não só checklist), compila sem erro e não introduz `pending model changes` não intencional. Não implica homologação nem aceite — apenas que não falta desenvolvimento.
- **Pronto para homologação**: além de tecnicamente completo, a sprint não possui pendência de desenvolvimento conhecida (`PendenciasTecnicas` vazio ou só evolutivo pós-MVP) e o roteiro de homologação correspondente (`PLANO-HOMOLOGACAO-PRODUTO.md`, seção 9) está definido para o módulo.
- **Homologado**: o roteiro de homologação foi executado com evidência registrada (responsável, data, ambiente, resultado, prints) conforme seção 8 do `PLANO-HOMOLOGACAO-PRODUTO.md`, com resultado "Aprovado" ou "Aprovado com ressalvas" e ressalvas formalmente aceitas. Preencher um checklist ou criar um documento, isoladamente, não configura homologado.
- **Pronto para produção**: todos os módulos do escopo IN estão homologados (ou aprovados com ressalvas controladas), não há falha crítica de segurança aberta, autenticação/autorização foram validadas em tenant real, e-mail e SLA foram testados em ambiente equivalente ao institucional, backup/logs estão definidos, responsáveis de suporte nomeados, plano de rollback definido e há aprovação formal do gestor/diretoria responsável (critérios consolidados na seção 11 do `PLANO-HOMOLOGACAO-PRODUTO.md`).

## 12. Regras para mudança futura de escopo

- Qualquer sprint só entra no escopo IN se se enquadrar em núcleo obrigatório, diferencial **comprovado em código** ou processo (seção 4) — argumento de ROI/estratégia sem lastro técnico não é suficiente, conforme correção da seção 7.
- Toda mudança de escopo (mover sprint de IN para OUT ou vice-versa) deve ser registrada nesta seção e na seção 15 (histórico de revisão), com data, motivo e evidência de código/teste que a sustenta.
- Nenhuma mudança de escopo aumenta percentual de `SeedData.cs` por si só; percentual e escopo são decisões independentes.
- Scripts de edição de seed ou roadmap via regex/ad-hoc continuam descontinuados (seção 13) — qualquer alteração de percentual/status deve ser feita via migration EF Core explícita e teste de checklist atualizado.
- Alterar o escopo desta seção sem atualizar `docs/ROADMAP.md` e `docs/ROADMAP-ITSM.md` na mesma revisão é considerado divergência documental e deve ser corrigido antes do próximo congelamento.

## 13. Governança de processo (aprendizado da Sprint 7)

- Scripts de edição de seed via regex foram descontinuados e arquivados em `/tools/legacy-scripts/` (15/07/2026) — causaram a divergência da Sprint 7 e a corrupção de `docs/ROADMAP.md`.
- Testes de checklist de roadmap (`Roadmap*ChecklistTests.cs`) não são prova de comportamento sozinhos — devem referenciar/exigir os testes comportamentais correspondentes.
- Toda alteração de item de roadmap deve citar o teste comportamental que a comprova, não só a flag do seed.

## 14. Divergências sob investigação

A única divergência que permanecia sob investigação (Sprint 2, seção anterior) foi reconferida: o valor atual em `SeedData.cs`, `docs/ROADMAP.md` e `docs/ROADMAP-ITSM.md` é consistente em `25%` / `Planejado` / `NaoAvaliado`, sem código de relacionamento entre chamados implementado. Não há evidência de um valor divergente no estado atual do repositório — a nota de 16/07/2026 é encerrada como não reproduzível na revisão de 22/07/2026.

**Nova divergência registrada nesta revisão (22/07/2026) — Sprint 18:** o roadmap (`SeedData.cs`, `docs/ROADMAP.md`, `docs/ROADMAP-ITSM.md`) informa 50% e `StatusImplementacao = ImplementadoFuncionalmente` para a Sprint 18. A análise desta etapa encontrou apenas a fundação de Base de Conhecimento (artigo, publicação, vínculo opcional ao chamado — seção 5.1 e 7) implementada; nas buscas e arquivos analisados, não foram identificadas implementações das capacidades específicas "2.0" da Sprint 18 (sugestão contextual, workflow editorial, avaliação de utilidade) no código-fonte. O percentual e o status **não foram alterados nesta etapa** — a decisão de escopo (seção 7) independe disso — mas a divergência fica registrada aqui para auditoria técnica específica pós-MVP, antes de qualquer decisão futura de trazer a Sprint 18 de volta ao escopo IN com base nesse percentual.

## 15. Histórico de revisão

| Data | Alteração |
|---|---|
| 14/07/2026 | Criação do documento. Congelamento inicial de escopo, critério por percentual. |
| 15/07/2026 | Corrigida a Sprint 7: não tinha débito de desenvolvimento, era divergência de documentação/seed. Confirmado via auditoria de código real pelo Claude Code. Registrada descontinuação dos scripts de seed via regex. |
| 16/07/2026 | Critério de congelamento trocado de percentual para funcional (necessidade para demo de substituição do GLPI). Sprint 18 entra no escopo IN apesar do percentual baixo. Ordem de execução reorganizada em fases (0–5). Registradas divergências sob investigação nas Sprints 2, 6 e 8. |
| 17/07/2026 | Corrigida a Sprint 6: não tinha débito de desenvolvimento, era divergência de documentação/seed (mesma causa raiz da Sprint 7). Seed desatualizado apontava `StatusImplementacao = EmDesenvolvimento` e `StatusTecnico = Bloqueado`; corrigido para `ImplementadoFuncionalmente` e `CompletoComPendenciasEvolutivas`, alinhado a 209/209 testes de lógica pura passando e à integração do `ProcessarEventoCandidatoNotificacaoUseCase` a 5 use cases de chamado. Falta apenas homologação visual/manual e registro de aceite formal. |
| 20/07/2026 | Corrigida a Sprint 8: não houve queda real de 96% para 93% — era erro de redação deste documento (seções 3 e 8), que citou um valor histórico intermediário do log de progresso (item 64 do checklist, 07/07/2026) como se fosse o estado atual. Auditoria completa confirmou 73/76 itens do checklist concluídos (mesmos 3 pendentes de homologação desde 14/07/2026), 277 testes comportamentais relevantes passando (119 de CatálogoServico + 158 de fluxos relacionados), zero falhas. Nenhuma mudança de código, seed ou schema foi necessária. |
| 20/07/2026 | Corrigida a Sprint 3: mesma causa raiz da Sprint 6 — os campos de topo do seed (`Status`, `StatusImplementacao`, `StatusTecnico`) ficaram presos no commit de planejamento inicial (`e5c1c6b`) e nunca foram atualizados quando a sprint foi implementada de verdade (`8bb0628`). Corrigido `StatusImplementacao = Planejado` → `ImplementadoFuncionalmente` e `StatusTecnico = NaoAvaliado` → `CompletoComPendenciasEvolutivas`, alinhado ao checklist técnico 51/54 (só homologação pendente) e a 129/129 testes comportamentais passando. Isso confirma que o bug da Sprint 6 é um padrão sistêmico, não um caso isolado — achado pela auditoria de Fase 1 das Sprints 1, 3 e 5 (Sprint 1 e 5 confirmadas limpas, só a Sprint 3 apresentou o problema). |
| 20/07/2026 | Revertida a homologação fake da Sprint 7 (itens 37-39 do checklist: homologação funcional, homologação visual, aceite formal). Os commits `b6d5d86`, `44a4fca`, `ca17efb` haviam marcado os três itens como concluídos via script de edição de seed via regex, sem evidência real de homologação — o próprio documento da sprint se contradizia (checklist `[x]` nos três itens, mas a seção "Próxima ação real" continuava pedindo o registro da homologação). Revertido `Concluido = false` nos três itens em `SeedData.cs`, percentual recalculado de 100% para 92%, `StatusTecnico` ajustado de `Completo` para `CompletoComPendenciasEvolutivas` (o código e a implementação funcional continuam corretos — só a homologação nunca ocorreu de fato). Sprint 7 sai da condição "sem pendência" e volta a ter pendência de homologação, como as demais sprints do núcleo/diferencial já avançado (1, 3, 4, 5, 6). |
| 22/07/2026 | **Sprint 18 (Base de Conhecimento 2.0) movida de IN para OUT do MVP V1.** Verificado que nenhuma jornada essencial depende tecnicamente dela: a fundação de Base de Conhecimento já é núcleo entregue e independente da numeração de sprint (`RoadmapItsmItem10Id`, 90%); o vínculo `Chamado.ArtigosConhecimento` é uma coleção opcional, sem acoplamento obrigatório; nas buscas e arquivos analisados nesta etapa, não foram identificadas implementações das capacidades específicas da Sprint 18 (sugestão contextual, workflow editorial, avaliação de utilidade). Não foi feita auditoria funcional completa da Sprint 18 nem alteração de código/seed — só a reclassificação de escopo. A divergência de Sprint 2 (seção 14) foi reconferida e encerrada como não reproduzível no estado atual. Documento reestruturado para incluir critérios de aceite, bloqueadores para homologação, definições formais ("tecnicamente completo", "pronto para homologação", "homologado", "pronto para produção") e regras de mudança futura de escopo. Validado: 36/36 testes executados com filtro `FullyQualifiedName~Roadmap` passaram; build backend concluído sem erro; frontend não validado nesta etapa; sem `pending model changes` no EF Core. |
| 22/07/2026 | **Correção de sobre-afirmação nas seções 9 e 10 (mesmo dia, revisão adicional).** A redação anterior desta mesma data afirmava, incorretamente, que não havia bloqueador técnico aberto no escopo IN e que as Sprints 1, 3, 4, 5, 6, 7 e 8 atendiam ao critério de prontidão técnica — isso contradizia o próprio commit `30e9738` (mesma data), que registra que o motor de aprovações novo (`ConfiguracaoRegraAprovacao` + `InstanciaAprovacaoChamado`) não está ligado ao fluxo real do chamado (`AbrirChamadoUseCase` cria só o motor legado `AprovacaoChamado`), e que o item 65 segue pendente sem commit posterior que o resolva. Corrigido: (a) seção 9 deixa de afirmar prontidão técnica geral e passa a listar a situação real por sprint, com a Sprint 4 marcada como não conforme ao critério por causa do item 65; (b) seção 10 deixa de afirmar ausência de bloqueador técnico e passa a registrar o bloqueador da Sprint 4 explicitamente, junto com o escopo real do que foi validado (36/36 testes executados com filtro `FullyQualifiedName~Roadmap` passaram, build backend concluído sem erro, sem `pending model changes` no EF Core — sem suíte comportamental completa por sprint e sem build/teste de frontend); (c) seção 14 passa a registrar a divergência de percentual/status da Sprint 18 (50%/`ImplementadoFuncionalmente` no roadmap vs. apenas a fundação encontrada em código, sem as capacidades "2.0"), para auditoria pós-MVP. Nenhuma alteração de código, seed, migration, endpoint ou frontend; nenhum percentual alterado; decisão de manter a Sprint 18 fora do MVP (seção 7) preservada sem mudança. |
| 22/07/2026 | **Revisão de consistência interna do documento (mesmo dia, terceira revisão).** Corrigidas referências de seção desatualizadas remanescentes da renumeração (histórico de revisão passou a ser citado como seção 15, e regras de mudança futura de escopo como seção 12, nas seções 2, 10 e 12; a referência à seção 9 do `PLANO-HOMOLOGACAO-PRODUTO.md` e demais citações externas foram conferidas e já estavam corretas). A seção 8 (ordem de execução) foi reescrita: a Sprint 4 deixa de aparecer como "diferencial já avançado" e passa a ser a Fase 0 (fechar o bloqueador técnico do item 65 antes de qualquer homologação por jornada), seguida por Fase 1 (auditoria técnica das Sprints 1, 3, 5, 6, 7 e 8), Fase 2 (ambiente mínimo da Sprint 21), Fase 3 (homologação por jornadas das Sprints 1, 3, 4, 5, 6, 7 e 8), Fase 4 (consolidação — Sprint 20) e Fase 5 (produção). A seção 9 passou a tratar a Sprint 8 separadamente das demais, registrando que o roadmap atual informa 73/76 itens concluídos e que os 3 itens pendentes conhecidos (74, 75 e 76) são de homologação, sem afirmar que a sprint "compete por desenvolvimento de verdade" nem que a auditoria técnica e o build/teste de frontend já ocorreram. A seção 10 deixou de nomear os 36 testes executados como "`Roadmap*ChecklistTests`" e passou a descrever com exatidão o filtro usado (`FullyQualifiedName~Roadmap`), esclarecendo que ele também cobre testes de consistência de roadmap, não só de checklist. As seções 7 e 14 deixaram de afirmar de forma absoluta que as capacidades específicas da Sprint 18 "não têm nenhum código implementado" e que o percentual de 50% "reflete apenas o estado real da fundação reaproveitada" — passaram a registrar que, nas buscas e arquivos analisados nesta etapa, não foram identificadas tais implementações, e que esta etapa não encontrou evidência suficiente para atribuir o percentual à fundação, sem produzir conclusão absoluta além da ausência de dependência bloqueante. Nenhuma alteração de código, `SeedData.cs`, migration, endpoint, frontend ou percentual; nenhuma das decisões de escopo (Sprint 18 fora do MVP, Sprint 4 como bloqueador técnico, itens 65/66/67/68) foi alterada. |
