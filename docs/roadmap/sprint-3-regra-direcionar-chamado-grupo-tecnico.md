# Regra para direcionar chamado a grupo tecnico

## Contexto

Esta etapa cria a regra de aplicacao para direcionar um chamado a um grupo tecnico, opcionalmente informando uma fila de atendimento do mesmo grupo. A regra nao cria endpoint, controller, tela, roteamento automatico ou atribuicao individual.

## Direcionar x transferir

Direcionar para grupo tecnico define o grupo corporativo inicialmente responsavel pelo atendimento do chamado. Transferir entre grupos muda um chamado que ja possui outro grupo tecnico e pode envolver efeitos operacionais proprios, como limpar responsavel individual.

Regra adotada:

- `DirecionarChamadoGrupoTecnicoAdminUseCase` atende chamados sem grupo tecnico e permite ajustar fila quando o chamado ja esta no mesmo grupo.
- `TransferirGrupoTecnicoChamadoUseCase` continua sendo o caminho para mudar de um grupo existente para outro.
- Se o chamado ja possui grupo tecnico diferente do destino, o direcionamento e rejeitado com orientacao para usar transferencia.

## Regra para chamado sem grupo

- Valida se o chamado existe e esta ativo.
- Valida se o grupo tecnico existe e esta ativo.
- Define `Chamado.GrupoTecnicoId`.
- Preserva `Chamado.ResponsavelId`.
- Nao altera SLA, status, prioridade, dashboard ou relatorios.

## Regra para fila

- `FilaAtendimentoId` e opcional.
- Quando informado, a fila deve existir, estar ativa e pertencer ao grupo tecnico informado.
- Quando nao informado, a fila atual e preservada somente se ja pertence ao grupo tecnico informado.
- Quando a fila atual pertence a outro grupo, ela e limpa para evitar ambiguidade operacional.
- Quando nao ha fila atual, `FilaAtendimentoId` permanece nulo.

## Regra para chamado que ja possui grupo

- Mesmo grupo tecnico: permitido ajustar fila, desde que a fila informada seja valida para o grupo.
- Grupo tecnico diferente: rejeitado; a troca deve usar transferencia entre grupos tecnicos.

## Responsavel individual

`ResponsavelId` e preservado porque direcionamento de grupo nao representa assumir chamado, atribuir tecnico individual ou transferir atendimento. A limpeza de responsavel permanece restrita ao fluxo de transferencia entre grupos.

## Historicos gerados

Sao usados os tipos ja existentes de `HistoricoChamado`:

- `GrupoTecnicoDefinido` quando o grupo e definido inicialmente.
- `FilaAtendimentoDefinida` quando uma fila e informada ou passa a ser definida.
- `FilaAtendimentoRemovida` quando a fila atual pertence a outro grupo e e limpa.
- `FilaAtendimentoTransferida` quando o chamado permanece no mesmo grupo e a fila muda.

## Contratos e use case

- Contrato criado: `DirecionarChamadoGrupoTecnicoRequest`.
- Interface criada: `IDirecionarChamadoGrupoTecnicoAdminUseCase`.
- Use case criado: `DirecionarChamadoGrupoTecnicoAdminUseCase`.

## Testes criados

Arquivo criado: `tests/SGX.SistemaChamado.Tests/DirecionarChamadoGrupoTecnicoAdminUseCaseTests.cs`.

Cenarios cobertos:

- Direcionar chamado sem grupo para grupo ativo.
- Direcionar chamado sem grupo para grupo ativo e fila valida.
- Rejeitar grupo inexistente.
- Rejeitar grupo inativo.
- Rejeitar fila inexistente.
- Rejeitar fila inativa.
- Rejeitar fila de outro grupo.
- Preservar `ResponsavelId`.
- Registrar historico de entrada em grupo.
- Registrar historico de entrada em fila.
- Nao alterar SLA.
- Preservar fila atual do mesmo grupo.
- Limpar fila atual de outro grupo.
- Rejeitar chamado com grupo tecnico diferente.

## O que nao foi implementado

- Nenhum controller.
- Nenhum endpoint publico.
- Nenhuma tela Vue.
- Nenhum service frontend.
- Nenhum roteamento automatico.
- Nenhuma regra de assumir chamado da fila.
- Nenhuma alteracao em dashboard, relatorio ou SLA.
- Nenhuma migration estrutural.

## Roadmap

O item `Criar regra para direcionar chamado a grupo tecnico` foi marcado como concluido. Com 19 itens concluidos em 54 itens ativos, o percentual esperado da Sprint 3 passa a ser aproximadamente 35%.

## Proxima etapa recomendada

Criar regra para direcionar chamado para fila de atendimento dentro do grupo tecnico, mantendo a separacao entre fila, responsavel individual e transferencia entre grupos.
