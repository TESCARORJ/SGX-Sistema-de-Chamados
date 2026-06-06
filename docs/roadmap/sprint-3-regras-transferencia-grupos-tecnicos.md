# Regras de transferencia entre grupos tecnicos

## Contexto

Esta etapa da Sprint 3 define a regra inicial de transferencia de chamados entre grupos tecnicos. A implementacao prepara a camada de dominio/aplicacao, sem criar endpoint, tela, dashboard, relatorio, SLA, roteamento automatico ou regra de assumir chamado da fila.

## Conceito

Transferir entre grupos tecnicos significa alterar o grupo corporativo responsavel pelo atendimento do chamado.

Conceitos separados:

- `GrupoTecnicoId`: grupo corporativo responsavel pelo atendimento.
- `FilaAtendimentoId`: fila operacional onde o chamado esta posicionado.
- `ResponsavelId`: tecnico individual responsavel pelo chamado.

Transferir grupo nao e o mesmo que atribuir responsavel individual e nao e o mesmo que transferir apenas entre filas.

## Regra para GrupoTecnicoId

`GrupoTecnicoId` passa a receber o grupo tecnico de destino.

Regras aplicadas:

- o grupo de destino deve existir;
- o grupo de destino deve estar ativo;
- chamado sem grupo anterior pode ser direcionado a um grupo;
- chamado com grupo anterior pode ser transferido para outro grupo;
- transferencia para o mesmo grupo com a mesma fila nao gera alteracao nem historico;
- transferencia para o mesmo grupo tentando alterar fila foi bloqueada nesta etapa, para nao antecipar regra de transferencia entre filas.

## Regra para ResponsavelId

Foi adotada a regra simples e segura: transferencia de grupo tecnico limpa `ResponsavelId`.

Motivo:

- o tecnico anterior pode nao pertencer ao novo grupo;
- preservar responsavel sem validar membro ativo do grupo destino poderia gerar ambiguidade operacional;
- a validacao de preservacao condicionada a membro ativo pode ser avaliada em etapa futura.

Assim, depois da transferencia, o chamado fica sem responsavel individual ate ser assumido ou atribuido novamente.

## Regra para FilaAtendimentoId

Como `FilaAtendimento` pertence a um `GrupoTecnico`, a fila antiga nao pode ser mantida automaticamente quando o grupo muda.

Regras aplicadas:

- se `FilaAtendimentoId` de destino nao for informado, a fila do chamado e limpa;
- se `FilaAtendimentoId` de destino for informado, a fila deve existir, estar ativa e pertencer ao grupo tecnico de destino;
- fila de outro grupo e rejeitada.

## Validacao de consistencia

A regra de aplicacao garante que a fila de destino pertence ao grupo tecnico de destino antes de alterar o chamado.

Isto evita a combinacao inconsistente:

`Chamado.GrupoTecnicoId != FilaAtendimento.GrupoTecnicoId`.

## Fluxos analisados

- `Chamado`
- `GrupoTecnico`
- `FilaAtendimento`
- `MembroGrupoTecnico`
- `HistoricoChamado`
- `TipoHistoricoChamado`
- `AssumirChamadoUseCase`
- `AtribuirChamadoUseCase`
- `AcoesChamadoService`
- testes de assumir, atribuir e dominio do chamado

## Alteracoes feitas

- Criado metodo de dominio `Chamado.TransferirGrupoTecnico`.
- Criado request interno `TransferirGrupoTecnicoChamadoRequest`.
- Criado contrato de aplicacao `ITransferirGrupoTecnicoChamadoUseCase`.
- Criado use case `TransferirGrupoTecnicoChamadoUseCase`.
- Registrado o use case no DI.
- Criado tipo de historico `GrupoTecnicoTransferido`.
- Atualizado mapeamento da linha do tempo para o novo tipo textual.
- Criados testes de aplicacao para a regra de transferencia.
- Atualizado roadmap para marcar o item da Sprint 3 como concluido.

## Historico textual

A transferencia registra `HistoricoChamado` com `TipoHistoricoChamado.GrupoTecnicoTransferido`.

A descricao informa grupo origem, grupo destino, fila destino quando houver, e se o responsavel individual foi removido.

Auditoria estruturada de movimentacao de grupo/fila nao foi criada nesta etapa porque existe item futuro especifico para auditoria.

## Testes criados

- transferir chamado sem grupo para grupo tecnico;
- transferir chamado de um grupo para outro;
- limpar `ResponsavelId` na transferencia;
- limpar `FilaAtendimentoId` quando nenhuma fila de destino e informada;
- definir `FilaAtendimentoId` quando fila valida do grupo destino e informada;
- rejeitar fila de outro grupo;
- rejeitar grupo inativo;
- registrar historico textual.

## O que nao foi implementado nesta etapa

- Endpoint publico.
- Tela Vue.
- Dashboard.
- Relatorios.
- SLA.
- Roteamento automatico.
- Regra de assumir chamado da fila.
- Transferencia completa entre filas.
- Auditoria estruturada de movimentacao.
- Regra de preservar responsavel quando ele tambem for membro ativo do grupo destino.

## Riscos tecnicos restantes

- A transferencia ainda nao possui UI ou endpoint exposto; o use case esta preparado para etapa futura.
- A auditoria estruturada de entrada, saida e transferencia de fila/grupo ainda precisa ser modelada.
- A regra de preservar responsavel por pertinencia ao grupo destino pode ser considerada depois, usando `MembroGrupoTecnico`.
- Transferencias futuras devem coordenar grupo, fila, responsavel e historico para nao duplicar eventos.

## Roadmap

Com a regra de aplicacao, historico textual, testes, build e validacao concluidos, o item `Definir regras de transferencia entre grupos tecnicos` pode ser marcado como concluido.

Percentual esperado da Sprint 3 apos esta etapa:

- 9 itens concluidos.
- 54 itens ativos.
- Percentual esperado: 17%.

## Proxima etapa recomendada

Definir regras de auditoria para entrada, saida e transferencia de fila, aproveitando o historico textual atual como base e evoluindo para rastreabilidade estruturada.
