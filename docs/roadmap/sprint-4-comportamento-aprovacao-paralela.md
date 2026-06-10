# Sprint 4 - Comportamento de Aprovacao Paralela
## 1. Objetivo da definicao
Definir conceitualmente como deve funcionar a aprovacao paralela no futuro motor de aprovacao ITSM reutilizavel do SGX Sistema de Chamados, detalhando ramos independentes, inicio simultaneo, consolidacao de decisoes, quorum por ramo, reprovacao, expiracao, ausencia de aprovador, fallback e efeito no bloqueio operacional.
## 2. Limites desta etapa
- Esta etapa registra apenas definicao conceitual, documentacao e atualizacao do roadmap/checklist.
- Nao houve implementacao funcional de aprovacao paralela.
- Nao foram criadas entidades novas.
- Nao foram criadas migrations estruturais.
- Nao houve alteracao no modelo de dominio.
- Nao foi criada estrutura de ramos, niveis paralelos ou relacionamento entre aprovacao, ramo e usuario.
- Nao houve alteracao em `AprovacaoChamado`, `BloqueiaAvancoAtendimento` ou no fluxo atual de aprovacao.
- Nao houve homologacao nem aceite final.
## 3. Contexto atual do fluxo de aprovacao
- O SGX possui hoje fluxo de aprovacao simples associado ao chamado.
- `AprovacaoChamado` registra solicitacao, aprovador, decisao, justificativa, status e auditoria principal.
- O fluxo atual nao possui ramos independentes executados simultaneamente.
- Nao existe hoje consolidacao formal de multiplas decisoes paralelas antes da liberacao operacional.
## 4. Suporte atual, ou ausencia dele, para aprovacao paralela
- Nao foi identificado suporte atual a aprovacao paralela estruturada em `AprovacaoChamado`.
- Nao existe persistencia de ramos paralelos, papel por ramo, quorum por ramo ou consolidacao final entre ramos.
- O modulo atual opera com instancia simples de aprovacao, sem coordenacao nativa entre decisoes simultaneas independentes.
## 5. Conceito de aprovacao paralela
Aprovacao paralela e o comportamento de aprovacao multinivel em que dois ou mais ramos de decisao independentes sao iniciados e avaliados ao mesmo tempo, sem depender da conclusao previa um do outro.
Ela deve ser usada quando diferentes areas, papeis ou competencias precisam decidir sobre aspectos independentes da mesma solicitacao e a decisao final depende da consolidacao dessas respostas conforme a regra aplicavel.
## 6. Diferenca entre aprovacao paralela, sequencial e multinivel generica
- Aprovacao paralela: ramos independentes podem ser avaliados ao mesmo tempo.
- Aprovacao sequencial: o proximo nivel depende da aprovacao valida do nivel anterior.
- Aprovacao multinivel: conceito mais amplo que pode conter fluxo sequencial, paralelo ou misto.
- Paralela e um comportamento especifico dentro da aprovacao multinivel.
## 7. Quando usar aprovacao paralela
- Quando decisoes sao independentes entre si.
- Quando financeiro e seguranca podem avaliar ao mesmo tempo.
- Quando dono do servico e infraestrutura possuem competencias distintas.
- Quando compliance e gestao podem decidir sem ordem obrigatoria.
- Quando reduzir tempo de aprovacao sem perder governanca.
- Quando multiplas areas precisam dar aceite sobre aspectos diferentes.
- Quando o fluxo sequencial criaria espera desnecessaria.
- Quando cada ramo possui autoridade propria e quorum proprio.
## 8. Quando nao usar aprovacao paralela
- Quando uma decisao depende da anterior.
- Quando aprovacao tecnica precisa preceder aprovacao financeira.
- Quando seguranca depende de evidencia tecnica ainda nao validada.
- Quando a ordem da decisao afeta o resultado.
- Quando uma unica decisao simples resolve o caso.
- Quando grupo aprovador decide em etapa unica.
- Quando o paralelismo aumenta o risco de decisoes contraditorias.
- Quando a regra exige segregacao em ordem definida.
## 9. Inicio dos ramos paralelos
- Os ramos paralelos devem iniciar quando o motor concluir que existe aprovacao formal obrigatoria e a regra aplicavel definir multiplos ramos independentes.
- Cada ramo pode ser resolvido por:
  - aprovador especifico;
  - grupo aprovador;
  - dono do servico;
  - responsavel tecnico;
  - responsavel financeiro;
  - responsavel de seguranca;
  - compliance;
  - gestor;
  - aprovador padrao como fallback, se permitido.
## 10. Criterios de independencia entre ramos
- Um ramo pode ser paralelo quando:
  - sua decisao nao depende de outro ramo;
  - sua autoridade e propria;
  - sua evidencia necessaria ja esta disponivel;
  - sua reprovacao ou aprovacao pode ser avaliada separadamente;
  - a ordem entre os ramos nao muda a qualidade da decisao;
  - nao ha conflito de competencia;
  - a regra permite consolidacao posterior.
## 11. Consolidacao de decisoes paralelas
- A decisao final da aprovacao paralela deve considerar o resultado de todos os ramos obrigatorios.
- A consolidacao conceitual pode seguir combinacoes como:
  - todos os ramos obrigatorios precisam aprovar;
  - um ramo critico pode reprovar e encerrar todo o fluxo;
  - ramos opcionais apenas sinalizam;
  - quorum global pode ser exigido;
  - quorum por ramo pode ser exigido;
  - aprovacao de papel obrigatorio pode ser necessaria;
  - a liberacao final so ocorre quando todos os ramos bloqueantes estiverem satisfeitos.
## 12. Efeito da aprovacao de um ramo paralelo
- A aprovacao de um ramo isolado nao libera automaticamente o chamado.
- Ela apenas marca aquele ramo como satisfeito.
- O chamado so deve ser liberado quando a regra de consolidacao dos ramos obrigatorios for cumprida.
## 13. Efeito da reprovacao de um ramo paralelo
- A reprovacao pode:
  - encerrar toda a aprovacao;
  - reprovar apenas aquele ramo;
  - bloquear acoes sensiveis relacionadas ao escopo daquele ramo;
  - exigir justificativa obrigatoria;
  - permitir ajuste e reenvio;
  - manter outros ramos em analise, se a regra permitir;
  - impedir consolidacao final positiva.
## 14. Efeito da aprovacao de todos os ramos obrigatorios
- Quando todos os ramos obrigatorios forem aprovados conforme quorum e regra de consolidacao, o motor pode considerar a aprovacao satisfeita para aquela regra.
- Isso libera acoes compativeis com o escopo aprovado.
- A liberacao final nao dispensa outras regras independentes de aprovacao que possam coexistir.
## 15. Efeito da expiracao de ramo paralelo
- A expiracao de um ramo pode:
  - manter o ramo pendente;
  - bloquear a consolidacao final;
  - escalar para outro aprovador;
  - acionar aprovador padrao, se permitido;
  - cancelar apenas o ramo;
  - cancelar toda a aprovacao;
  - exigir nova solicitacao;
  - registrar evento de auditoria.
## 16. Ausencia de aprovador em ramo paralelo
- Se um ramo nao conseguir resolver aprovador, a regra conceitual deve tentar:
  1. aprovador especifico;
  2. dono do servico;
  3. grupo aprovador;
  4. delegacao valida;
  5. aprovador padrao, se permitido;
  6. erro de configuracao, se nenhum responsavel for resolvido.
- O sistema nao deve gerar ramo paralelo silencioso sem responsavel.
## 17. Relacao com aprovador especifico
- Aprovador especifico pode ser responsavel por um ramo quando a regra exigir autoridade nominal.
- Quando resolvido validamente, ele deve prevalecer sobre fallback generico daquele ramo.
## 18. Relacao com grupo aprovador
- Grupo aprovador pode decidir dentro de um ramo paralelo.
- O grupo respeita quorum proprio daquele ramo.
- A decisao do grupo satisfaz aquele ramo, nao necessariamente toda a aprovacao paralela.
## 19. Relacao com aprovador padrao
- Aprovador padrao pode atuar como fallback de um ramo quando permitido pela regra.
- Ele nao deve substituir ramo especifico quando houver aprovador, grupo, dono do servico ou delegacao valida.
- Seu uso deve ser auditavel e excepcional.
## 20. Relacao com quorum por ramo
- Cada ramo pode possuir quorum proprio.
- A consolidacao final so deve considerar o ramo aprovado quando o quorum daquele ramo estiver satisfeito.
- Quorum insuficiente impede consolidacao positiva daquele ramo e, quando obrigatorio, da aprovacao paralela.
## 21. Relacao com bloqueio operacional do chamado
- Enquanto houver ramo obrigatorio pendente, o motor pode bloquear acoes sensiveis do chamado.
- O bloqueio pode ser:
  - total de avanco operacional;
  - restrito a acoes sensiveis;
  - restrito ao escopo do ramo pendente;
  - mantido ate aprovacao de todos os ramos obrigatorios;
  - encerrado por reprovacao, cancelamento ou expiracao conforme regra.
## 22. Relacao com `AprovacaoChamado`
- `AprovacaoChamado` continua sendo a instancia persistente atual da aprovacao simples.
- No futuro, a aprovacao paralela podera exigir estrutura complementar para representar ramos, status por ramo, responsaveis, quorum e decisoes paralelas.
- Nesta etapa, `AprovacaoChamado` nao foi alterado.
## 23. Relacao com historico e auditoria
- A aprovacao paralela deve registrar:
  - regra que gerou os ramos;
  - ramos exigidos;
  - ramos obrigatorios e opcionais;
  - responsaveis resolvidos por ramo;
  - quando cada ramo foi iniciado;
  - quando cada ramo foi decidido;
  - quem decidiu;
  - grupo ou aprovador especifico por ramo;
  - fallback usado por ramo;
  - quorum exigido e atingido;
  - reprovacao em ramo paralelo;
  - expiracao;
  - ausencia de aprovador;
  - consolidacao final;
  - efeito final no chamado.
## 24. Riscos de seguranca e governanca
- Liberar chamado com ramo obrigatorio pendente.
- Tratar aprovacao de um ramo como aprovacao total.
- Nao consolidar corretamente decisoes paralelas.
- Nao diferenciar ramo obrigatorio de ramo informativo.
- Nao tratar reprovacao de ramo critico.
- Nao tratar expiracao.
- Nao tratar ausencia de aprovador.
- Usar aprovador padrao indevidamente.
- Ignorar conflito de interesse.
- Criar ramos paralelos sem autoridade formal.
- Nao auditar decisoes por ramo.
- Permitir decisoes contraditorias sem regra de precedencia.
## 25. Compatibilidade com fluxo atual
- O conceito preserva o modulo atual de aprovacao.
- Nao exige mudanca imediata em `AprovacaoChamado`.
- O fluxo atual de solicitacao, aprovacao, reprovacao e cancelamento nao foi alterado.
- Esta etapa apenas define o comportamento futuro para orientar modelagem e implementacao posteriores.
## 26. Lacunas encontradas
- Nao existe suporte atual a ramos paralelos estruturados.
- Nao existe modelo persistente de ramo, consolidacao de ramos ou quorum por ramo.
- Nao existe politica operacional implementada para expiracao, ausencia de aprovador ou escalonamento entre ramos paralelos.
## 27. Decisoes adiadas para proximos itens
- Como modelar ramos paralelos.
- Como armazenar status por ramo.
- Como iniciar ramos simultaneamente.
- Como associar grupos a ramos.
- Como calcular quorum por ramo.
- Como consolidar decisao final.
- Como tratar ramos opcionais.
- Como tratar reprovacao parcial.
- Como tratar expiracao.
- Como escalar ausencia de decisao.
- Como aplicar delegacao em ramo paralelo.
- Como tratar conflito de interesse.
- Como exibir aprovacao paralela na interface.
- Como migrar aprovacoes atuais.
- Como manter compatibilidade com aprovacoes simples.
## 28. Conclusao tecnica
Aprovacao paralela deve ser definida como comportamento de aprovacao multinivel em que multiplos ramos independentes sao acionados simultaneamente, e a decisao final depende da consolidacao das decisoes desses ramos conforme a regra aplicavel. O conceito reforca governanca por competencia, reduz espera desnecessaria e preserva a base atual do modulo para modelagem futura.
## 29. Proxima etapa recomendada
Executar o item 16 do checklist da Sprint 4: definir regra de bloqueio por decisao pendente.
