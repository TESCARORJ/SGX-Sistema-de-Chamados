# Sprint 4 - Regra de Aprovação por Serviço Sensível

## 1. Objetivo da definição

Definir conceitualmente como o serviço solicitado deve influenciar a exigência de aprovação no futuro motor de aprovação ITSM reutilizável do SGX Sistema de Chamados.

## 2. Limites desta etapa

- Esta etapa registra apenas definicao conceitual, documentacao e atualizacao do roadmap/checklist.
- Nao houve implementacao funcional da regra por servico sensivel.
- Nao foram criadas entidades novas.
- Nao foram criadas migrations estruturais.
- Nao houve alteracao no modelo de dominio.
- Nao houve criacao de enum novo nem alteracao de enum existente.
- Nao houve alteracao em `AprovacaoChamado`, `BloqueiaAvancoAtendimento`, `CatalogoServico` ou no status do chamado.
- Nao houve alteracao no fluxo atual de abertura, atendimento ou aprovacao.
- Nao houve homologacao nem aceite final.

## 3. Contexto do servico solicitado no sistema atual

- O sistema ja possui Catalogo de Servicos institucional.
- O chamado pode nascer com vinculo opcional a um servico do catalogo.
- O catalogo e hoje a representacao mais forte do servico solicitado.
- Em cenarios sem catalogo, categoria, subcategoria e tipo de solicitacao continuam ajudando na classificacao operacional, mas nao substituem o servico solicitado quando ele existe formalmente.

## 4. Representacao atual de catalogo de servico e servico solicitado

- `Chamado` possui `CatalogoServicoId` opcional.
- `CriarChamadoRequest` aceita `CatalogoServicoId` e `CatalogoServicoSlug`.
- `CatalogoServico` centraliza o cadastro do servico solicitado com campos como:
  - `Nome`
  - `Slug`
  - `DepartamentoResponsavelId`
  - `CategoriaId`
  - `SubcategoriaId`
  - `PrioridadePadraoId`
  - `SlaPadraoId`
  - `PermiteAberturaChamado`
  - `RequerAprovacao`
  - `Visibilidade`
- Na abertura por catalogo, o backend aplica os dados oficiais do servico ao chamado e ignora divergencias manipuladas pelo frontend.
- Quando `CatalogoServico.RequerAprovacao = true`, o fluxo atual ja cria `AprovacaoChamado` automaticamente.

## 5. Relacao entre servico sensivel e motor de aprovacao

A regra por servico sensivel deve ser tratada como entrada forte de decisao do motor.

O motor deve avaliar o servico como o objeto real da solicitacao, respondendo se aquele servico:

1. exige aprovacao formal antes da execucao;
2. gera apenas sinalizacao de governanca;
3. nao exige aprovacao;
4. exige geracao de nova aprovacao;
5. exige reavaliacao de aprovacao ja existente.

## 6. Relacao entre servico sensivel e natureza ITSM

- A natureza continua definindo o contexto principal do processo ITSM.
- O servico sensivel complementa a natureza com a sensibilidade concreta do objeto solicitado.
- O servico pode elevar uma `Requisicao` ou um `Incidente` para exigencia impeditiva.
- O servico nao deve reduzir uma exigencia impeditiva ja definida por `Mudanca`.

## 7. Relacao entre servico sensivel e tipo de chamado

- O tipo de chamado refina a variante operacional.
- O servico solicitado qualifica o que efetivamente sera executado.
- Mesmo com tipo comum, o servico pode elevar a exigencia para aprovacao impeditiva.
- Quando o tipo ja for impeditivo, o servico pode reforcar, especializar ou justificar melhor a regra de aprovacao.

## 8. Conceito de servico sensivel

Servico sensivel e qualquer servico solicitado que envolva acesso privilegiado, alteracao de ambiente, impacto em servico critico, custo, risco operacional, dados sensiveis, seguranca, compliance, recurso restrito ou decisao formal antes da execucao.

## 9. Criterios que tornam um servico sensivel

Um servico deve ser tratado como sensivel quando envolver um ou mais criterios como:

- acesso administrativo, privilegiado ou segregado;
- alteracao de perfil, permissao ou identidade;
- acesso a dados sensiveis, pessoais ou restritos;
- alteracao em ambiente produtivo;
- mudanca de configuracao critica;
- indisponibilidade potencial ou risco de degradacao em servico critico;
- custo financeiro, compra ou consumo de recurso controlado;
- impacto de seguranca da informacao;
- exigencia de compliance, auditoria ou rastreabilidade formal;
- dependencia de aprovacao institucional antes da execucao.

## 10. Servicos com aprovacao impeditiva

Exemplos conceituais:

- Liberacao de acesso administrativo.
- Alteracao de perfil ou permissao.
- Criacao, alteracao ou exclusao de usuario privilegiado.
- Acesso a dados sensiveis.
- Alteracao em ambiente produtivo.
- Mudanca em configuracao critica.
- Liberacao de recurso restrito.
- Requisicao de compra.
- Requisicao com custo.
- Solicitacao com impacto financeiro.
- Execucao com risco operacional relevante.
- Servico que afeta disponibilidade de sistema critico.
- Servico relacionado a seguranca da informacao.
- Servico relacionado a compliance ou auditoria obrigatoria.

## 11. Servicos com apenas sinalizacao

Exemplos conceituais:

- Servico monitorado.
- Servico com historico de recorrencia.
- Servico com possivel impacto futuro.
- Atendimento consultivo tecnico.
- Analise preliminar de problema.
- Solicitacao que pode exigir validacao posterior.
- Servico associado a ativo critico, mas sem alteracao imediata.
- Servico com dependencia de outro time, mas sem risco imediato.

## 12. Servicos sem exigencia de aprovacao

Exemplos conceituais:

- Duvida operacional.
- Informacao.
- Orientacao simples.
- Suporte comum ao usuario.
- Correcao simples sem alteracao de ambiente.
- Registro de incidente simples.
- Solicitacao sem custo e sem risco.
- Atendimento sem acesso privilegiado.

## 13. Regra conceitual na abertura do chamado

- Se o servico solicitado for sensivel e impeditivo, o motor deve retornar `RequerGeracaoDeAprovacao` quando ainda nao existir aprovacao adequada.
- Se ja existir aprovacao pendente adequada ao servico, a acao operacional dependente deve poder resultar em `BloqueadoPorAprovacaoPendente`.
- Se o servico gerar apenas governanca informativa, a abertura pode seguir com `PermitidoComSinalizacao`.
- Se o servico for comum, a abertura segue a regra herdada da natureza e do tipo.

## 14. Regra conceitual na alteracao do servico solicitado

- De servico comum para servico sensivel: `RequerGeracaoDeAprovacao` ou `RequerReavaliacaoDeAprovacao`.
- De servico sinalizado para servico impeditivo: reavaliacao antes de permitir acoes sensiveis.
- De servico impeditivo para servico comum: a decisao sobre cancelar, manter historico ou apenas desconsiderar o bloqueio fica adiada para itens futuros.
- Alteracao de servico que mude acesso, risco, custo ou criticidade deve disparar reavaliacao mesmo sem troca de natureza ou tipo.

## 15. Regra conceitual para reavaliacao de aprovacao

O motor deve prever `RequerReavaliacaoDeAprovacao` quando:

- o servico passa a envolver acesso privilegiado;
- o servico passa a envolver custo ou impacto financeiro;
- o servico passa a afetar ambiente produtivo ou servico critico;
- o servico passa a expor dado sensivel, seguranca ou compliance;
- a aprovacao anterior deixa de cobrir o novo escopo operacional.

## 16. Relacao com impacto e urgencia

- Impacto e urgencia nao definem sozinhos a sensibilidade do servico.
- Eles podem reforcar a necessidade de aprovacao de um servico ja sensivel.
- Servico com impacto alto e urgencia alta tende a exigir governanca adicional, mas a matriz final fica adiada para o item especifico dessa regra.

## 17. Relacao com custo e risco

- Custo e risco sao amplificadores naturais da regra por servico.
- Um servico inicialmente comum pode se tornar impeditivo quando incorporar custo, risco operacional ou exposicao relevante.
- A definicao detalhada desses gatilhos fica adiada para os itens especificos de custo e risco.

## 18. Relacao com AprovacaoChamado

- A regra por servico sensivel deve gerar ou consultar uma instancia de `AprovacaoChamado`.
- Devem ser preservados `ChamadoId`, status, origem, decisao, historico e auditoria.
- O fluxo atual por catalogo ja prova essa compatibilidade conceitual, pois servicos com `RequerAprovacao = true` ja criam aprovacao automatica com `TipoOrigem = CatalogoServico`.

## 19. Relacao com BloqueiaAvancoAtendimento

- Para servicos impeditivos, a aprovacao futura deve poder usar `BloqueiaAvancoAtendimento` como ponte de compatibilidade.
- O conceito nao deve ficar preso apenas a um bloqueio total.
- O servico sensivel pode resultar em:
  - bloqueio total de avancar atendimento;
  - bloqueio apenas de acoes sensiveis;
  - apenas sinalizacao sem bloqueio.

## 20. Relacao com AguardandoAprovacao

- Servicos sensiveis podem levar o chamado ao estado `AguardandoAprovacao`.
- Isso nao deve ser obrigatorio em todos os cenarios.
- O motor deve conseguir bloquear acoes especificas mesmo sem depender exclusivamente de troca de status.

## 21. Compatibilidade com fluxo atual

- O fluxo atual ja possui compatibilidade parcial por meio de `CatalogoServico.RequerAprovacao`.
- O conceito proposto preserva esse comportamento como base inicial.
- O futuro motor deve ampliar a governanca sem quebrar o comportamento existente de aprovacao automatica por catalogo.
- Servico comum nao deve reduzir exigencia impeditiva ja definida por natureza ou tipo.

## 22. Lacunas encontradas

- O sistema atual usa principalmente o booleano `RequerAprovacao`, sem graduacao entre bloqueio impeditivo e sinalizacao.
- Ainda nao existe classificacao formal de sensibilidade do servico alem da exigencia simples de aprovacao.
- Nao existe reavaliacao automatica quando o servico vinculado ao chamado muda.
- O catalogo ainda nao expressa custo, risco, criticidade ou dados sensiveis de forma estruturada para o futuro motor.

## 23. Riscos de compatibilidade

- Regras futuras mais ricas nao podem invalidar o comportamento atual de abertura por catalogo.
- Servicos catalogados como comuns hoje podem exigir revisao institucional quando a classificacao de sensibilidade ficar mais detalhada.
- A ampliacao de governanca precisa evitar falsos positivos em servicos de baixa criticidade.

## 24. Decisoes adiadas para proximos itens

- Regra de aprovacao por impacto e urgencia.
- Regra de aprovacao por custo ou risco.
- Regra final de bloqueio por decisao pendente.
- Regra de liberacao apos aprovacao.
- Regra de rejeicao, cancelamento e expiracao.
- Tratamento da reducao de sensibilidade quando um servico deixa de ser sensivel.
- Estrutura futura para classificar sensibilidade diretamente no cadastro do catalogo.

## 25. Conclusão técnica

O serviço solicitado deve ser tratado como uma entrada forte de governança no futuro motor de aprovação ITSM. Ele representa o objeto concreto da solicitação e pode elevar a exigência de aprovação mesmo quando natureza e tipo parecem comuns. O conceito proposto preserva compatibilidade com o fluxo atual por `CatalogoServico.RequerAprovacao`, mas prepara o sistema para uma avaliação mais completa de acesso, risco, custo, criticidade e compliance.

## 26. Percentual esperado

- `12%`
- Base: `8 / 68 = 11,76%`, arredondado para `12%`

## 27. Confirmações

- Não houve implementação funcional nesta etapa.
- Não houve criação de entidade, enum, endpoint, controller, tela ou service frontend.
- Não houve migration estrutural.
- A migration criada foi apenas de dados.
- Apenas o item `8` foi marcado como concluído.
- Os itens `1` a `7` permaneceram concluídos.
- Os itens `9` a `68` permaneceram pendentes.
- Nenhum item de homologação foi marcado como concluído.

## 28. Migration de dados

- Migration aplicada nesta etapa: `20260606202402_ConcluirRegraAprovacaoPorServicoSensivelSprint4Roadmap`

## 29. Próxima etapa recomendada

1. Executar o item `9` do checklist: definir regra de aprovação por impacto e urgência.
2. Nessa definição, separar quando impacto e urgência apenas priorizam o atendimento e quando elevam a necessidade de aprovação formal.
