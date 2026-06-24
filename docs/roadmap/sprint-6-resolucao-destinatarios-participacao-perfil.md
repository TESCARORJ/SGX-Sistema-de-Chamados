# Sprint 6 - Resolucao de destinatarios por participacao e perfil

## 1. Objetivo

Implementar a regra de aplicacao que resolve destinatarios internos elegiveis para notificacoes a partir de participacoes reais e perfis persistidos no SGX, sem criar notificacoes nem executar envio.

## 2. Estado anterior

A Sprint 6 ja possuia:
- entidade `Notificacao` persistida;
- contrato `EventoCandidatoNotificacao`;
- servico `IGerarNotificacaoUseCase` com idempotencia e concorrencia validadas;
- ausencia deliberada de entrega por canal.

## 3. Participacoes analisadas

Foram analisadas as seguintes origens potenciais:
- solicitante do chamado;
- responsavel atual do chamado;
- usuario originador do evento;
- aprovacao legada;
- instancia do motor de aprovacao;
- grupo tecnico;
- perfil de acesso;
- fila de atendimento;
- observadores;
- grupo aprovador futuro.

## 4. Participacoes suportadas

Foram suportadas somente as origens com relacao persistida e consulta confiavel:
- `Solicitante`;
- `ResponsavelAtual`;
- `UsuarioOriginador`;
- `AprovadorLegado`;
- `AprovadorInstancia`;
- `MembroGrupoTecnico`;
- `PerfilAcesso`.

## 5. Participacoes nao suportadas

Permaneceram fora do escopo por nao haver estrutura funcional segura de expansao:
- observadores;
- fila como lista de usuarios;
- grupo aprovador futuro;
- quoruns;
- delegacao;
- expansao generica por permissao global.

## 6. Contrato de entrada

Foi criado `ResolverDestinatariosNotificacaoRequest` com:
- `Evento`;
- `Participacoes`;
- `AprovacaoChamadoId`;
- `InstanciaAprovacaoChamadoId`;
- `GrupoTecnicoId`;
- `PerfilAcessoId`;
- `ExcluirUsuarioOriginador`.

## 7. Contrato de saida

Foram criados:
- `DestinatarioNotificacaoResolvido`, com `UsuarioId`, `Nome`, `Email` e `Origens`;
- `ResolverDestinatariosNotificacaoResponse`, com `Destinatarios` e `Avisos`.

## 8. Elegibilidade

Um usuario interno so entra no resultado quando:
- `Ativo = true`;
- `Situacao = Ativo`;
- `BloqueadoAte` esta nulo ou vencido.

Nao ha exigencia de e-mail preenchido para a resolucao interna.

## 9. Solicitante

O solicitante e resolvido a partir de `Chamado.SolicitanteId` quando o chamado do evento existe.

## 10. Responsavel

O responsavel atual e resolvido somente a partir de `Chamado.ResponsavelId`. Responsavel historico nao e expandido.

## 11. Usuario originador

O originador so e incluido quando `UsuarioOriginador` e explicitamente solicitado no request.

## 12. Grupo tecnico

Grupo tecnico foi suportado somente por vinculo real em `MembroGrupoTecnico`, exigindo:
- grupo ativo;
- membro ativo;
- usuario elegivel.

Se o request nao informar `GrupoTecnicoId`, a resolucao pode usar `Chamado.GrupoTecnicoId`.

## 13. Perfil

Perfil foi suportado por `PerfilAcessoId` e `UsuarioPerfilAcesso`. Nao foi usado codigo textual generico nem permissao global como broadcast.

## 14. Aprovacao legada

O aprovador legado e resolvido exclusivamente por `AprovacaoChamado.AprovadorId`.

## 15. Motor de aprovacao

O motor novo resolve somente `InstanciaAprovacaoChamado.AprovadorResolvidoUsuarioId`. Nao expande grupo aprovador futuro, estrategia abstrata ou delegacao.

## 16. Grupo aprovador futuro

Permanece documentado como capacidade futura, sem ser tratado como origem funcional neste item.

## 17. Observadores

Nao existe entidade persistente funcional de observadores utilizada para esta resolucao.

## 18. Deduplicacao

Destinatarios sao deduplicados por `UsuarioId`.

## 19. Multiplas origens

Quando o mesmo usuario aparece por mais de uma participacao, a resposta agrega todas as origens no mesmo destinatario.

## 20. Exclusao do originador

Quando `ExcluirUsuarioOriginador = true`, o usuario originador e removido integralmente do resultado final.

## 21. Ordenacao

O retorno e deterministico, ordenado por `Nome` e depois por `UsuarioId`.

## 22. Avisos

Avisos sao retornados para cenarios como:
- aprovador nao resolvido;
- grupo sem membros elegiveis;
- perfil sem usuarios elegiveis;
- participacao sem usuario resolvido;
- chamado ausente para participacao dependente.

Ausencia legitima de destinatario nao gera excecao.

## 23. Persistencia somente leitura

O use case nao cria, nao atualiza e nao remove notificacoes. A resolucao e estritamente de leitura.

## 24. Integracao futura com geracao

O desenho preserva a separacao:
1. resolver destinatarios;
2. materializar canal e conteudo;
3. chamar `GerarNotificacaoUseCase` para cada notificacao concreta.

## 25. Compatibilidade com Worker.Email

Nenhuma integracao foi criada com `Worker.Email`. O worker inbound continua isolado.

## 26. Impacto em abertura

Nenhum evento de abertura passou a gerar notificacoes automaticamente.

## 27. Impacto em atendimento

Nenhum fluxo de atendimento foi integrado automaticamente a resolucao.

## 28. Impacto em aprovacao

O item passou a resolver aprovador real de aprovacao legada e da instancia, mas sem criar notificacao.

## 29. Impacto em SLA

Nenhum evento de SLA foi integrado automaticamente.

## 30. Impacto em fechamento e reabertura

Nao houve integracao automatica com fechamento, aceite ou reabertura.

## 31. Testes

Foram criados testes:
- unitarios do use case;
- unitarios do validator;
- relacionais com PostgreSQL;
- regressao do use case de geracao;
- regressao de aprovacao e grupo tecnico.

## 32. Riscos

Os principais riscos remanescentes sao:
- expandir perfil sem escopo adicional em cenarios futuros;
- acoplar resolucao a envio;
- considerar grupo aprovador futuro como funcional antes da modelagem real.

## 33. Decisoes adiadas

Permanecem adiados:
- observadores;
- preferencias por canal/evento;
- templates;
- fila e outbox;
- retry;
- integracao automatica com eventos ITSM;
- regras avancadas de grupo aprovador.

## 34. Criterios de aceite

Atendidos neste item:
- interface, DTOs e validator criados;
- participacoes funcionais suportadas;
- usuarios inativos excluidos;
- deduplicacao por usuario;
- multiplas origens preservadas;
- nenhuma persistencia de notificacao;
- nenhum envio executado;
- testes unitarios e relacionais aprovados.

## 35. Proxima etapa

Modelar templates e materializacao de conteudo.
