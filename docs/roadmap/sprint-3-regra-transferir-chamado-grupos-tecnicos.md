# Regra para transferir chamado entre grupos tecnicos

## Contexto

Esta etapa conciliou a regra existente `TransferirGrupoTecnicoChamadoUseCase` com a separacao funcional definida na Sprint 3. Nao foi criado use case duplicado.

## Use case validado

Use case existente validado:

- `TransferirGrupoTecnicoChamadoUseCase`

Contrato existente validado:

- `TransferirGrupoTecnicoChamadoRequest`

Interface e DI validados:

- `ITransferirGrupoTecnicoChamadoUseCase`
- registro `Scoped` em `DependencyInjection`

## Direcionar x transferir

Direcionar chamado para grupo tecnico e a definicao inicial do grupo corporativo responsavel.

Transferir entre grupos tecnicos e a mudanca de um grupo tecnico ja existente para outro grupo tecnico. Por isso, a regra foi conciliada para rejeitar chamado sem grupo tecnico anterior e orientar o uso do direcionamento inicial.

## Regras confirmadas para grupo tecnico

- Grupo destino e obrigatorio.
- Grupo destino deve existir e estar ativo.
- Chamado precisa possuir grupo tecnico de origem.
- Grupo destino diferente do grupo atual muda `Chamado.GrupoTecnicoId`.
- Transferencia para o mesmo grupo nao deve ser usada para alterar fila nesta etapa.

## Regras confirmadas para fila de atendimento

- `FilaAtendimentoId` de destino e opcional.
- Sem fila destino, `Chamado.FilaAtendimentoId` e limpo.
- Com fila destino valida, `Chamado.FilaAtendimentoId` passa a ser a fila informada.
- Fila destino deve existir, estar ativa e pertencer ao grupo tecnico destino.
- Fila de outro grupo e rejeitada.

## Regras confirmadas para responsavel

- Transferencia entre grupos limpa `ResponsavelId`.
- A regra nao atribui tecnico automaticamente.
- A limpeza do responsavel e registrada em historico quando havia responsavel anterior.

## Historicos gerados

Sao usados os tipos ja existentes:

- `GrupoTecnicoTransferido`
- `FilaAtendimentoDefinida`
- `FilaAtendimentoRemovida`
- `FilaAtendimentoTransferida`
- `ResponsavelRemovidoPorTransferenciaGrupo`

## Ajustes feitos

- O use case passou a rejeitar chamado sem grupo tecnico anterior, mantendo coerencia com `DirecionarChamadoGrupoTecnicoAdminUseCase`.
- Testes de transferencia foram ajustados para partir de um grupo origem quando o caso e de transferencia real.
- Foram adicionadas validacoes de grupo inexistente, fila inativa e SLA inalterado.

## Testes validados

Arquivo validado/alterado:

- `tests/SGX.SistemaChamado.Tests/TransferirGrupoTecnicoChamadoUseCaseTests.cs`

Cenarios cobertos:

- Rejeitar chamado sem grupo tecnico.
- Transferir de um grupo para outro.
- Limpar responsavel individual.
- Limpar fila quando nao ha fila destino.
- Definir fila valida do grupo destino.
- Rejeitar fila de outro grupo.
- Rejeitar grupo inativo.
- Rejeitar grupo inexistente.
- Rejeitar fila inativa.
- Registrar historico de grupo com origem e destino.
- Registrar entrada, saida e transferencia de fila.
- Registrar remocao de responsavel.
- Nao alterar SLA.

## O que nao foi implementado

- Nenhum use case duplicado.
- Nenhum controller.
- Nenhum endpoint publico.
- Nenhuma tela Vue.
- Nenhum service frontend.
- Nenhuma transferencia autonoma entre filas.
- Nenhum roteamento automatico.
- Nenhuma alteracao em SLA, dashboard ou relatorios.
- Nenhuma migration estrutural.

## Roadmap

O item `Criar regra para transferir chamado entre grupos tecnicos` foi marcado como concluido. Com 21 itens concluidos em 54 itens ativos, o percentual esperado da Sprint 3 passa a ser aproximadamente 39%.

## Proxima etapa recomendada

Criar regra para atribuir chamado a tecnico especifico, mantendo a distincao entre assumir fila, atribuicao manual e transferencia entre grupos.
