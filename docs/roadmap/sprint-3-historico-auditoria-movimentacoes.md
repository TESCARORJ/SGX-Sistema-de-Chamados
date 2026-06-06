# Sprint 3 - Historico e auditoria das movimentacoes

## Objetivo

Consolidar a auditoria de movimentacoes de grupo tecnico, fila de atendimento e responsavel individual usando o mecanismo existente de `HistoricoChamado`.

## Mecanismo adotado

O `HistoricoChamado` permanece como mecanismo oficial de auditoria da Sprint 3. Nenhuma entidade nova de auditoria foi criada.

Cada movimentacao registra:

- chamado;
- tipo de historico;
- descricao textual;
- usuario responsavel pela movimentacao;
- data/hora de criacao herdada da entidade auditavel.

Origem e destino ainda nao existem como campos estruturados no historico. Quando a movimentacao exige rastreabilidade de origem/destino, essas informacoes sao registradas no texto.

## Tipos validados

- `GrupoTecnicoDefinido`
- `GrupoTecnicoTransferido`
- `FilaAtendimentoDefinida`
- `FilaAtendimentoRemovida`
- `FilaAtendimentoTransferida`
- `ResponsavelRemovidoPorTransferenciaGrupo`
- `ChamadoAssumidoDaFila`
- `ResponsavelAlterado`

Nenhum valor existente do enum `TipoHistoricoChamado` foi reordenado e nenhum tipo novo foi necessario.

## Movimentacoes cobertas

- Direcionamento inicial para grupo tecnico: registra o grupo definido.
- Entrada em fila: registra a fila definida.
- Remocao de fila: registra a fila removida.
- Transferencia entre filas: registra fila de origem e fila de destino.
- Transferencia entre grupos: registra grupo de origem e grupo de destino.
- Remocao de responsavel por transferencia de grupo: registra o responsavel removido.
- Assuncao de chamado da fila: registra fila e tecnico que assumiu.
- Atribuicao manual a tecnico: registra tecnico destino.
- Reatribuicao manual: registra responsavel anterior e responsavel destino.

## Ajustes realizados

A linha do tempo passou a mapear explicitamente `ChamadoAssumidoDaFila`, evitando exibicao generica como apenas "Atualizacao do chamado".

O historico de reatribuicao manual em `AtribuirChamadoUseCase` passou a registrar origem e destino quando ja havia responsavel anterior.

## Linha do tempo

Os novos tipos de grupo/fila/responsavel sao exibidos na linha do tempo como eventos internos, com categorias:

- `grupo-tecnico`
- `fila-atendimento`
- `responsavel`

Os tipos antigos continuam mapeados pelo fluxo existente.

## Testes

Foram ajustados testes de:

- `AtribuirChamadoUseCaseTests`, cobrindo texto de atribuicao e reatribuicao;
- `LinhaTempoChamadoUseCasesTests`, cobrindo exibicao dos tipos de grupo, fila, responsavel removido e chamado assumido da fila.

Os testes existentes dos use cases de direcionamento, transferencia e assuncao ja validam os historicos gerados por cada operacao.

## Fora do escopo

Nao foram criados controller, endpoint publico, tela Vue, service frontend, dashboard, relatorio, regra de SLA, roteamento automatico, entidade nova de auditoria ou migration estrutural.

## Roadmap

O checklist da Sprint 3 foi atualizado marcando somente o item "Criar historico/auditoria das movimentacoes" como concluido.

Com 23 itens concluidos de 54 ativos, o percentual esperado da Sprint 3 passa para aproximadamente 43%.

## Proxima etapa recomendada

Ajustar consultas de chamados para considerar grupo tecnico e fila, garantindo filtros e retornos administrativos coerentes com a nova estrutura.
