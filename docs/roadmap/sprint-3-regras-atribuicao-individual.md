# Regras de atribuicao individual

## Contexto

Esta etapa da Sprint 3 revisa a atribuicao individual de chamados apos a introducao dos vinculos opcionais `GrupoTecnicoId` e `FilaAtendimentoId`.

O objetivo foi preservar o comportamento atual de `ResponsavelId` como tecnico individual, sem implementar roteamento, transferencia, regra de assumir chamado da fila ou auditoria estruturada de grupo/fila.

## Como funciona a atribuicao individual atual

A atribuicao individual continua centralizada em `Chamado.AtribuirResponsavel`.

Fluxos analisados:

- `AssumirChamadoUseCase`
- `AtribuirChamadoUseCase`
- `AcoesChamadoService`
- `Chamado.AtribuirResponsavel`
- `HistoricoChamado`
- `TipoHistoricoChamado.ResponsavelAlterado`
- testes de assumir, atribuir e dominio do chamado

`AssumirChamadoUseCase` carrega o chamado, valida permissao e aprovacao pendente, e chama `chamado.AtribuirResponsavel(usuario.Id, usuario.Login)`.

`AtribuirChamadoUseCase` valida que o operador pode atribuir, valida que o usuario destino possui perfil de atendimento e chama `chamado.AtribuirResponsavel(responsavel.Id, usuario.Login)`.

Ambos preservam o historico textual atual com `TipoHistoricoChamado.ResponsavelAlterado`.

## Convivencia entre responsavel, grupo e fila

- `ResponsavelId`: tecnico individual responsavel pelo chamado.
- `GrupoTecnicoId`: grupo corporativo associado ao atendimento.
- `FilaAtendimentoId`: fila operacional onde o chamado esta posicionado.

A atribuicao individual nao exige `GrupoTecnicoId` e nao exige `FilaAtendimentoId`.

Ao atribuir ou assumir um chamado, `ResponsavelId` e atualizado sem limpar automaticamente `GrupoTecnicoId` ou `FilaAtendimentoId`.

## Alteracoes feitas

Nao foi necessario alterar `AssumirChamadoUseCase`, `AtribuirChamadoUseCase`, `AcoesChamadoService` ou a regra de dominio `AtribuirResponsavel`, pois a regra atual ja modifica somente `ResponsavelId`.

Foram adicionados testes de regressao para garantir que:

- assumir chamado preserva `GrupoTecnicoId` e `FilaAtendimentoId`;
- atribuir chamado preserva `GrupoTecnicoId` e `FilaAtendimentoId`;
- `ResponsavelId` continua sendo preenchido no fluxo atual.

O roadmap foi atualizado para marcar o item `Definir regras de atribuicao individual sem quebrar o responsavel atual` como concluido.

## Comportamentos preservados

- `ResponsavelId` permanece opcional.
- `GrupoTecnicoId` permanece opcional.
- `FilaAtendimentoId` permanece opcional.
- Atribuicao direta para tecnico continua valida sem grupo/fila.
- Assumir chamado continua preenchendo `ResponsavelId`.
- Historico textual de alteracao de responsavel foi preservado.
- SLA nao foi alterado nesta etapa.
- Endpoints e telas existentes nao foram alterados.

## O que nao foi implementado nesta etapa

- Roteamento automatico para grupo.
- Regra de assumir chamado da fila.
- Transferencia entre grupos.
- Transferencia entre filas.
- Auditoria estruturada de movimentacao de grupo/fila.
- Cadastro funcional de grupo ou fila.
- Endpoints, controllers, services novos ou telas Vue.
- Alteracoes em dashboard, relatorios ou SLA.

## Riscos tecnicos restantes

- A regra de consistencia entre `Chamado.GrupoTecnicoId` e `FilaAtendimento.GrupoTecnicoId` ainda precisa ser implementada em etapa futura.
- A auditoria atual registra a mudanca de responsavel em texto, mas ainda nao registra movimentacoes estruturadas de grupo/fila.
- Fluxos futuros de assumir da fila precisam decidir se o chamado permanece ou sai da fila apos assumir, sem alterar implicitamente o comportamento legado.

## Roadmap

Com a revisao tecnica, testes de regressao, build e validacao concluidos, o item `Definir regras de atribuicao individual sem quebrar o responsavel atual` pode ser marcado como concluido.

Percentual esperado da Sprint 3 apos esta etapa:

- 8 itens concluidos.
- 54 itens ativos.
- Percentual esperado: 15%.

## Proxima etapa recomendada

Definir as regras de transferencia entre grupos tecnicos, mantendo `ResponsavelId` como tecnico individual e preservando a separacao entre grupo corporativo, fila operacional e responsavel.
