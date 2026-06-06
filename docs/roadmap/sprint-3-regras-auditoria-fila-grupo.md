# Mapeamento das regras de auditoria para fila e grupo tecnico

## Contexto

A Sprint 3 introduziu `GrupoTecnico`, `MembroGrupoTecnico`, `FilaAtendimento` e os vinculos opcionais de `Chamado` com grupo e fila. O sistema ja possuia `HistoricoChamado` como trilha textual dos eventos do chamado.

Esta etapa fortalece a rastreabilidade de entrada, saida e transferencia de grupo/fila usando o historico atual, sem criar uma entidade nova de auditoria estruturada.

## Objetivo

Registrar eventos mais claros quando um chamado entra em grupo tecnico, muda de grupo, entra em fila, sai de fila, troca de fila ou perde o responsavel individual por transferencia de grupo.

## Historico atual do chamado

`HistoricoChamado` registra:

- `ChamadoId`
- `Tipo`
- `Descricao`
- `UsuarioId`
- `CriadoEm`
- dados basicos de auditoria textual

O tipo e representado por `TipoHistoricoChamado`. Como os valores sao persistidos como enum, novos tipos foram adicionados ao final, sem reordenar valores existentes.

## Eventos auditados nesta etapa

- Entrada em grupo tecnico: quando `GrupoTecnicoId` passa de nulo para um grupo destino.
- Transferencia de grupo tecnico: quando `GrupoTecnicoId` muda de um grupo para outro.
- Entrada em fila: quando `FilaAtendimentoId` passa de nulo para uma fila destino.
- Saida de fila: quando havia `FilaAtendimentoId` e a transferencia de grupo nao informa fila destino.
- Transferencia de fila: quando havia fila anterior e uma nova fila valida do grupo destino e informada.
- Remocao de responsavel: quando a transferencia de grupo limpa `ResponsavelId`.

## Tipos de historico

Tipos reaproveitados:

- `GrupoTecnicoTransferido`

Tipos criados:

- `GrupoTecnicoDefinido`
- `FilaAtendimentoDefinida`
- `FilaAtendimentoRemovida`
- `FilaAtendimentoTransferida`
- `ResponsavelRemovidoPorTransferenciaGrupo`

## Diferenca entre grupo, fila e responsavel individual

`GrupoTecnicoId` representa o grupo corporativo associado ao atendimento.

`FilaAtendimentoId` representa a fila operacional onde o chamado esta posicionado.

`ResponsavelId` continua representando o tecnico individual. A transferencia entre grupos limpa o responsavel individual para evitar manter um tecnico que pode nao pertencer ao grupo destino.

## Alteracao no use case de transferencia

`TransferirGrupoTecnicoChamadoUseCase` continua sendo o ponto de aplicacao da transferencia entre grupos. A regra funcional principal foi preservada:

- altera `GrupoTecnicoId`;
- valida fila destino ativa e pertencente ao grupo destino;
- limpa `FilaAtendimentoId` quando nao ha fila destino;
- limpa `ResponsavelId`;
- nao altera SLA, dashboard, relatorios, telas ou endpoints.

A diferenca desta etapa e que o use case agora grava historicos separados para cada evento relevante, em vez de concentrar tudo em uma unica descricao textual.

## Linha do tempo

`LinhaTempoChamadoUseCases` foi atualizado para mapear os novos tipos como eventos internos especificos:

- `grupo-tecnico`
- `fila-atendimento`
- `responsavel`

Isso evita que os novos eventos aparecam como historico generico para atendentes e administradores.

## O que nao foi implementado

- Nenhuma tela Vue.
- Nenhum endpoint publico novo.
- Nenhum dashboard.
- Nenhum relatorio.
- Nenhuma regra de SLA.
- Nenhum roteamento automatico.
- Nenhuma regra de assumir chamado da fila.
- Nenhuma entidade nova de auditoria estruturada.
- Nenhuma mudanca no contrato publico da API.

## Limitacoes da auditoria textual

A auditoria ainda depende de descricoes textuais para nomes de grupo, fila e responsavel. Isso preserva compatibilidade e baixo impacto, mas limita consultas analiticas futuras por origem/destino de movimentacao.

Uma evolucao futura pode criar uma auditoria estruturada de movimentacoes contendo IDs de grupo/fila origem e destino, responsavel removido, motivo, usuario executor e correlacao entre eventos.

## Riscos tecnicos restantes

- Eventos textuais nao sao ideais para relatorios analiticos de produtividade por grupo/fila.
- A consistencia entre `Chamado.GrupoTecnicoId` e `FilaAtendimento.GrupoTecnicoId` segue garantida no use case de transferencia, mas ainda deve ser reforcada em regras futuras de direcionamento e transferencia de fila.
- Como nao ha fluxo completo de roteamento, chamados podem continuar existindo em estados legados sem grupo/fila.

## Testes criados ou ajustados

- Transferencia sem grupo anterior registra `GrupoTecnicoDefinido`.
- Transferencia entre grupos registra origem e destino.
- Transferencia com fila destino registra entrada na fila.
- Transferencia sem fila destino registra saida da fila anterior.
- Transferencia entre filas registra fila anterior e nova fila.
- Transferencia que limpa responsavel registra `ResponsavelRemovidoPorTransferenciaGrupo`.
- Linha do tempo mapeia eventos de grupo, fila e responsavel como tipos especificos.

## Proxima etapa recomendada

Seguir para o proximo item do checklist da Sprint 3, revisando os itens de banco de dados ja gerados para grupos tecnicos e consolidando a rastreabilidade das migrations antes de avancar para contratos e servicos de aplicacao.
