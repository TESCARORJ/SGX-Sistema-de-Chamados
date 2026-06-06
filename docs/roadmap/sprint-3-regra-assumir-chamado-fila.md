# Regra para assumir chamado da fila

## Contexto

Esta etapa cria a regra de aplicacao para que um tecnico assuma um chamado que ja esta direcionado a um grupo tecnico e a uma fila de atendimento. A regra preenche o responsavel individual sem remover o grupo nem a fila do chamado.

## Assumir legado x assumir da fila

O `AssumirChamadoUseCase` legado continua existindo para o fluxo administrativo atual de assumir chamado sem responsavel. Ele preserva grupo e fila quando existirem, mas nao exige membro ativo do grupo tecnico.

O novo `AssumirChamadoFilaAdminUseCase` e especifico para chamados em fila:

- exige grupo tecnico no chamado;
- exige fila de atendimento no chamado;
- exige usuario autenticado como membro ativo do grupo tecnico;
- rejeita chamado que ja possui responsavel individual;
- nao altera SLA.

## Regra de membro ativo do grupo

O usuario que assume deve ser o proprio usuario autenticado informado no request. Isso evita transformar o fluxo de assumir em uma atribuicao manual para outro tecnico.

Para assumir, deve existir `MembroGrupoTecnico` ativo com:

- `GrupoTecnicoId` igual ao grupo do chamado;
- `UsuarioId` igual ao usuario autenticado.

Usuario fora do grupo ou com vinculo inativo e rejeitado.

## Regra de grupo e fila

- `GrupoTecnicoId` deve estar preenchido.
- O grupo tecnico deve existir e estar ativo.
- `FilaAtendimentoId` deve estar preenchido.
- A fila deve existir e estar ativa.
- A fila deve pertencer ao mesmo grupo tecnico do chamado.
- Ao assumir, `GrupoTecnicoId` e `FilaAtendimentoId` sao preservados.

## Regra de responsavel

Chamado da fila precisa estar sem `ResponsavelId`.

Ao assumir:

- `ResponsavelId` recebe o usuario autenticado;
- `GrupoTecnicoId` permanece inalterado;
- `FilaAtendimentoId` permanece inalterado.

Se o chamado ja possui responsavel, a operacao e rejeitada para evitar tomada indevida de atendimento.

## Historico gerado

Foi adicionado o tipo `ChamadoAssumidoDaFila` ao final de `TipoHistoricoChamado`, preservando os valores existentes.

O historico textual registra:

- nome da fila;
- nome do usuario que assumiu;
- observacao opcional, quando informada.

## Contratos e use case

- Contrato criado: `AssumirChamadoFilaRequest`.
- Interface criada: `IAssumirChamadoFilaAdminUseCase`.
- Use case criado: `AssumirChamadoFilaAdminUseCase`.

## Testes criados

Arquivo criado: `tests/SGX.SistemaChamado.Tests/AssumirChamadoFilaAdminUseCaseTests.cs`.

Cenarios cobertos:

- Assumir chamado de fila com usuario membro ativo do grupo.
- Preservar `GrupoTecnicoId`.
- Preservar `FilaAtendimentoId`.
- Definir `ResponsavelId`.
- Rejeitar chamado inexistente.
- Rejeitar chamado sem grupo tecnico.
- Rejeitar chamado sem fila de atendimento.
- Rejeitar chamado ja com responsavel.
- Rejeitar usuario que nao e membro ativo do grupo.
- Rejeitar membro inativo.
- Rejeitar grupo inativo.
- Rejeitar fila inativa.
- Rejeitar fila de outro grupo.
- Rejeitar `UsuarioId` diferente do usuario autenticado.
- Registrar historico textual.
- Validar que o fluxo legado de assumir chamado continua funcionando.

## O que nao foi implementado

- Nenhum controller.
- Nenhum endpoint publico.
- Nenhuma tela Vue.
- Nenhum service frontend.
- Nenhum roteamento automatico.
- Nenhuma transferencia entre grupos.
- Nenhuma transferencia entre filas.
- Nenhuma alteracao em SLA, dashboard ou relatorios.
- Nenhuma migration estrutural.

## Roadmap

O item `Criar regra para assumir chamado da fila` foi marcado como concluido. Com 20 itens concluidos em 54 itens ativos, o percentual esperado da Sprint 3 passa a ser aproximadamente 37%.

## Proxima etapa recomendada

Criar regra para transferir chamado entre grupos tecnicos como fluxo operacional separado, preservando a distincao entre direcionamento inicial, assumir fila e transferencia.
