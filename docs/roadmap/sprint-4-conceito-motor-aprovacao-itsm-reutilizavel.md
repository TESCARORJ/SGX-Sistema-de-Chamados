# Sprint 4 - Conceito do Motor de Aprovação ITSM Reutilizável

## 1. Objetivo da definição conceitual

Definir conceitualmente o motor de aprovacao ITSM reutilizavel do SGX Sistema de Chamados como uma camada central de governanca e decisao, capaz de avaliar quando um chamado exige aprovacao formal antes de permitir avancar, executar, concluir ou encerrar uma acao operacional.

## 2. Limites desta etapa

- Esta etapa registra apenas definicao conceitual, documentacao e atualizacao do roadmap/checklist.
- Nao foram criadas entidades novas.
- Nao foram criadas migrations estruturais.
- Nao houve alteracao no modelo de dominio.
- Nao houve criacao de endpoint, controller, tela ou service frontend novo.
- Nao houve implementacao funcional do motor.
- Nao houve alteracao em `AprovacaoChamado`.
- Nao houve alteracao em `BloqueiaAvancoAtendimento`.
- Nao houve alteracao no status do chamado.
- Nao houve alteracao do fluxo atual de atendimento.
- Nao houve homologacao nem aceite final.

## 3. Contexto atual do modulo de aprovacao

- O sistema ja possui uma base funcional consolidada de aprovacao sobre a entidade `AprovacaoChamado`.
- Existem dois usos principais hoje:
  - aprovacao administrativa principal, com comportamento bloqueante padrao;
  - aprovacoes vinculadas ao detalhe do chamado, com comportamento informativo por padrao.
- O vinculo com o chamado ja existe por `ChamadoId`.
- O estado resumido da aprovacao do chamado ja e consolidado em `AprovacaoChamadoHelper`.
- O campo `BloqueiaAvancoAtendimento` ja representa o mecanismo atual de bloqueio impeditivo simples.
- O status `AguardandoAprovacao` existe no ecossistema de status, mas ainda nao e acionado automaticamente pelo fluxo atual.

## 4. Problema que o motor deve resolver

O modulo atual resolve cenarios lineares de aprovacao, mas ainda nao oferece uma camada central e reutilizavel de governanca para responder de forma consistente:

1. se um chamado exige aprovacao;
2. qual regra disparou essa exigencia;
3. se a aprovacao e impeditiva ou apenas informativa;
4. se a acao pode continuar;
5. se uma nova aprovacao deve ser gerada;
6. se uma aprovacao anterior precisa ser reavaliada.

Sem essa camada central, o comportamento tende a ficar espalhado em use cases especificos, com bloqueios parciais, lacunas de compatibilidade e crescimento dificil de sustentar para regras por natureza ITSM, tipo, servico, risco, custo e niveis multiplos.

## 5. Conceito proposto de motor de aprovacao ITSM reutilizavel

O motor de aprovacao ITSM reutilizavel deve ser definido como uma camada central de decisao responsavel por avaliar se um chamado, em determinado contexto operacional, exige aprovacao formal antes de permitir avancar, executar, concluir ou encerrar uma acao.

Esse motor:

- nao deve ser tratado apenas como tela, endpoint ou CRUD de aprovacao;
- nao deve ser visto apenas como a entidade `AprovacaoChamado`;
- deve ser entendido como mecanismo de governanca reutilizavel;
- deve responder perguntas de negocio e operacao antes que a acao seja executada.

Conceitualmente, o motor deve ser capaz de responder:

1. Este chamado exige aprovacao?
2. Qual regra disparou a exigencia?
3. A aprovacao e impeditiva ou apenas informativa?
4. Existe aprovacao pendente?
5. Existe aprovacao aprovada?
6. Existe aprovacao reprovada?
7. A acao solicitada pode continuar?
8. A acao deve ser bloqueada?
9. A acao pode continuar com sinalizacao?
10. Uma nova aprovacao deve ser gerada apos mudanca de dados sensiveis?

## 6. Responsabilidades do motor

O motor deve concentrar conceitualmente as seguintes responsabilidades:

1. Determinar se o chamado exige aprovacao.
2. Avaliar a acao que o usuario esta tentando executar.
3. Identificar a regra conceitual que fundamenta a exigencia.
4. Definir se a aprovacao e impeditiva, informativa ou dispensavel para aquela acao.
5. Verificar o estado atual da aprovacao aplicavel.
6. Indicar se a acao pode continuar, deve ser bloqueada ou deve continuar com sinalizacao.
7. Indicar se uma aprovacao nova precisa ser gerada.
8. Indicar se uma aprovacao anterior precisa ser reavaliada.
9. Preservar compatibilidade com a base atual de `AprovacaoChamado`, historico e auditoria.

## 7. O que o motor deve avaliar

Conceitualmente, o motor deve avaliar ao menos:

1. Natureza ITSM do chamado.
2. Tipo de chamado.
3. Servico solicitado.
4. Sensibilidade do servico.
5. Impacto e urgencia.
6. Custo estimado.
7. Risco operacional.
8. Contexto da acao solicitada.
9. Existencia de aprovacao anterior.
10. Estado da aprovacao anterior.
11. Necessidade de reaproveitamento ou reavaliacao da aprovacao existente.

## 8. Tipos de decisao conceitual do motor

O motor deve ser concebido para produzir respostas como:

1. `Permitido`
   A acao pode continuar sem exigencia de aprovacao.

2. `PermitidoComSinalizacao`
   A acao pode continuar, mas o sistema deve sinalizar pendencia, risco, contexto de governanca ou aprovacao relacionada.

3. `BloqueadoPorAprovacaoPendente`
   A acao nao pode continuar ate que exista decisao formal.

4. `BloqueadoPorAprovacaoReprovada`
   A acao nao pode continuar porque a decisao formal foi negativa.

5. `RequerGeracaoDeAprovacao`
   A acao ou a alteracao de dados exige abertura de nova aprovacao antes de prosseguir.

6. `RequerReavaliacaoDeAprovacao`
   A acao alterou dados sensiveis e exige verificar se a aprovacao anterior continua valida.

## 9. Diferenca entre bloqueio impeditivo, sinalizacao e acao permitida

### Bloqueio impeditivo

Deve ser usado quando a acao operacional nao pode prosseguir sem aprovacao formal.

Exemplos conceituais:

- executar servico sensivel;
- liberar acesso;
- concluir ou encerrar chamado sensivel;
- prosseguir com mudanca de risco relevante;
- executar acao com custo ou risco que exija decisao formal.

### Sinalizacao

Deve ser usada quando o sistema precisa tornar visivel um contexto de aprovacao, risco ou governanca, mas sem impedir a continuidade da acao.

Exemplos conceituais:

- aprovacao vinculada informativa;
- pendencia administrativa nao impeditiva;
- aprovacao relacionada a contexto que nao trava a acao atual.

### Acao permitida

Deve ser usada quando a aprovacao pendente nao deve impedir a atividade.

Exemplos conceituais:

- consultar chamado;
- listar chamados;
- visualizar historico;
- registrar comentario;
- anexar evidencia;
- consultar SLA;
- consultar status da aprovacao;
- registrar auditoria.

## 10. Relacao com `AprovacaoChamado`

`AprovacaoChamado` deve ser preservada conceitualmente como a base da instancia de aprovacao atual do chamado.

O motor nao substitui a entidade; ele a utiliza como suporte persistente para:

- vinculo com `ChamadoId`;
- status de aprovacao;
- origem da aprovacao;
- solicitacao;
- decisao;
- historico;
- auditoria;
- compatibilidade com fluxos ja existentes.

Conceitualmente:

- `AprovacaoChamado` representa a instancia persistida da aprovacao;
- o motor representa a camada central que decide como interpretar essa instancia para cada acao do chamado.

## 11. Relacao com `BloqueiaAvancoAtendimento`

`BloqueiaAvancoAtendimento` deve ser tratado como o mecanismo atual de bloqueio impeditivo simples.

O motor futuro deve:

- preservar compatibilidade com esse comportamento atual;
- continuar reconhecendo esse flag como indicador valido de bloqueio existente;
- nao ficar limitado apenas a ele.

Conceitualmente, o campo passa a ser entendido como:

- uma expressao atual de bloqueio impeditivo;
- util como ponte de compatibilidade;
- insuficiente, sozinho, para representar todas as regras futuras do motor.

## 12. Relacao com `StatusChamadoEnum.AguardandoAprovacao`

`AguardandoAprovacao` deve ser avaliado como possivel estado operacional de espera, mas nao como a unica forma de bloqueio.

Conceitualmente:

- o motor pode bloquear uma acao mesmo sem mover o chamado para esse status;
- o status pode ser utilizado futuramente como representacao operacional de espera;
- o bloqueio nao deve depender exclusivamente da troca de status.

Isso evita acoplamento excessivo entre:

- decisao de governanca do motor;
- representacao visual ou operacional do ciclo de vida do chamado.

## 13. Compatibilidade com o fluxo atual

O conceito do motor deve nascer com as seguintes premissas de compatibilidade:

1. Reaproveitar `AprovacaoChamado` como base persistente.
2. Preservar o comportamento atual do bloqueio simples ja em producao tecnica.
3. Nao invalidar os endpoints atuais de aprovacao.
4. Nao quebrar o fluxo atual do portal que apenas consulta status.
5. Nao transformar aprovacoes vinculadas informativas em bloqueantes automaticamente.
6. Nao depender de alteracao estrutural imediata para existir conceitualmente.

## 14. Eventos ou momentos onde o motor deve ser consultado futuramente

Conceitualmente, o motor deve ser consultado em eventos como:

1. abertura de chamado;
2. inicio de atendimento;
3. atribuicao ou direcionamento operacional;
4. mudanca de status;
5. conclusao;
6. encerramento;
7. reabertura;
8. cancelamento;
9. reclassificacao de dados sensiveis;
10. alteracao de servico solicitado;
11. alteracao de impacto, urgencia, custo ou risco.

## 15. Acoes que podem ser bloqueadas futuramente

1. Iniciar atendimento.
2. Assumir ou atribuir responsavel para execucao.
3. Avancar status operacional.
4. Concluir chamado.
5. Encerrar chamado.
6. Prosseguir com mudanca emergencial.
7. Liberar acesso.
8. Prosseguir com servico sensivel.
9. Prosseguir com custo ou risco relevante sem decisao formal.

## 16. Acoes que podem apenas gerar sinalizacao

1. Exibir status de aprovacao no detalhe do chamado.
2. Exibir pendencia ou risco de governanca para o atendente.
3. Exibir contexto de aprovacao para o gestor.
4. Sinalizar necessidade de revisao futura sem bloquear a acao corrente.
5. Exibir aprovacao vinculada nao bloqueante.

## 17. Acoes que devem permanecer permitidas

1. Consultar chamado.
2. Listar chamados.
3. Visualizar historico.
4. Registrar comentario.
5. Anexar evidencia.
6. Consultar SLA.
7. Consultar status da aprovacao.
8. Registrar auditoria.
9. Cancelar solicitacao de aprovacao, quando a regra administrativa permitir.

## 18. Premissas para regras futuras

1. A regra de aprovacao deve ser separada da persistencia da instancia de aprovacao.
2. O motor deve avaliar contexto, nao apenas status bruto da aprovacao.
3. Nem toda aprovacao pendente deve significar bloqueio universal.
4. O bloqueio deve ser orientado por regra de negocio e governanca.
5. O desenho deve suportar evolucao para aprovador padrao, grupo aprovador e multiplos niveis.
6. O desenho deve preservar rastreabilidade, historico e auditoria.
7. O desenho deve respeitar compatibilidade com abertura, atendimento e SLA atuais.

## 19. Fora de escopo desta etapa

1. Implementar o motor.
2. Criar entidade nova.
3. Criar endpoint novo.
4. Criar controller novo.
5. Criar tela nova.
6. Implementar regra por natureza ITSM.
7. Implementar regra por tipo de chamado.
8. Implementar regra por servico sensivel.
9. Implementar bloqueio novo.
10. Implementar aprovacao por grupo.
11. Implementar aprovacao multinivel.
12. Alterar `AprovacaoChamado`.
13. Alterar `BloqueiaAvancoAtendimento`.
14. Alterar status do chamado.

## 20. Riscos de compatibilidade

1. Tratar toda aprovacao como bloqueante pode quebrar fluxos hoje informativos.
2. Acoplar o motor exclusivamente ao status do chamado pode gerar rigidez operacional.
3. Pausar SLA automaticamente sem regra clara pode distorcer indicadores existentes.
4. Reavaliar aprovacao apos qualquer mudanca de dados pode gerar excesso de pendencias.
5. Generalizar demais sem separar contexto da acao pode criar bloqueios indevidos.

## 21. Decisoes adiadas para proximos itens

Ficam explicitamente adiadas para os proximos itens do checklist:

1. Regra de aprovacao por natureza ITSM.
2. Regra de aprovacao por tipo de chamado.
3. Regra de aprovacao por servico sensivel.
4. Regra por impacto e urgencia.
5. Regra por custo e risco.
6. Conceito de aprovador padrao.
7. Conceito de grupo aprovador.
8. Conceito de aprovacao multinivel.
9. Regra especifica de bloqueio por decisao pendente.
10. Regra de liberacao apos aprovacao.
11. Regra de rejeicao, cancelamento e expiracao.
12. Regras de historico e auditoria especializadas do motor.

## 22. Conclusao tecnica

O motor de aprovacao ITSM reutilizavel deve ser entendido como uma camada central de decisao e governanca sobre o chamado, e nao apenas como uma tela de aprovacao ou uma instancia isolada de `AprovacaoChamado`. O modulo atual oferece uma base funcional importante, mas o passo conceitual desta etapa e separar a decisao do motor da forma atual de persistencia, permitindo que o sistema evolua com coerencia para regras por natureza, tipo, servico, risco, custo, aprovador e nivel, sem perder compatibilidade com o fluxo vigente.

## 23. Proxima etapa recomendada

Definir a regra de aprovacao por natureza ITSM, usando este conceito como base para separar:

- quando a natureza exige aprovacao;
- quando a aprovacao e impeditiva;
- quando a aprovacao e apenas informativa;
- quais acoes da natureza passam a ser governadas pelo motor.
