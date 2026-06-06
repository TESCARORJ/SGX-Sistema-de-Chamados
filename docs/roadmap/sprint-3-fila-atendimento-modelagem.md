# Modelagem da entidade FilaAtendimento

## Contexto

Esta etapa da Sprint 3 cria a estrutura inicial para filas corporativas de atendimento vinculadas a grupos tecnicos. A modelagem nao altera chamados, responsavel individual, SLA, filtros, dashboards, relatorios, telas ou endpoints.

## Decisao por entidade propria

Foi criada a entidade `FilaAtendimento` em vez de adicionar campos soltos em `Chamado`.

Motivos:

- fila e um conceito operacional proprio, com identidade, nome, ciclo de vida e governanca;
- um grupo tecnico pode futuramente possuir mais de uma fila;
- a fila precisa ser rastreavel para produtividade, filtros e auditoria futura;
- manter fila separada evita confundir grupo tecnico, responsavel individual e estado atual do chamado.

## Separacao de Chamado nesta etapa

`FilaAtendimento` foi mantida sem relacionamento com `Chamado`.

Motivos:

- o item atual e somente modelagem da fila;
- o vinculo entre chamado e grupo/fila sera definido em itens posteriores do checklist;
- `ResponsavelId` continua representando somente o responsavel individual;
- evitar regressao no fluxo atual, que ainda usa `ResponsavelId == null` como fila implicita.

## Relacao com GrupoTecnico

Cada `FilaAtendimento` pertence obrigatoriamente a um `GrupoTecnico`.

Modelo:

- `FilaAtendimento.GrupoTecnicoId`
- `FilaAtendimento.GrupoTecnico`
- `GrupoTecnico.FilasAtendimento`

O relacionamento usa delete restrito para preservar integridade historica e evitar exclusao acidental de grupos com filas associadas.

## Estrutura da entidade

Entidade: `FilaAtendimento`

Campos:

- `Id`: `Guid`, herdado de `EntityBase`.
- `GrupoTecnicoId`: obrigatorio.
- `Nome`: obrigatorio.
- `Descricao`: opcional.
- `Ativo`: herdado de `AuditableEntity`.
- `CriadoEm`: herdado de `AuditableEntity`.
- `CriadoPor`: herdado de `AuditableEntity`.
- `AtualizadoEm`: herdado de `AuditableEntity`.
- `AtualizadoPor`: herdado de `AuditableEntity`.

Metodos:

- Construtor publico com `grupoTecnicoId`, `nome`, `descricao` e `criadoPor`.
- Construtor privado para EF Core.
- `AlterarDados`.
- `Inativar`.
- `Reativar`.

Validacoes:

- nao permite `GrupoTecnicoId` vazio;
- nao permite nome vazio;
- atualizacao nao permite nome vazio;
- descricao vazia e normalizada para `null`.

## Estrutura da tabela

Tabela: `filas_atendimento`

Colunas:

- `id`
- `grupo_tecnico_id`
- `nome`
- `descricao`
- `criado_em`
- `criado_por`
- `atualizado_em`
- `atualizado_por`
- `ativo`

## Indices e constraints

Indices:

- `ix_filas_atendimento_grupo_tecnico_id`
- `ix_filas_atendimento_ativo`
- `ux_filas_atendimento_grupo_nome`

Unicidade:

- `grupo_tecnico_id` + `nome` e unico.

Essa escolha impede duas filas com o mesmo nome dentro do mesmo grupo tecnico e permite nomes iguais em grupos diferentes, se isso fizer sentido operacionalmente. O contexto do grupo elimina ambiguidade local e preserva flexibilidade para organizacoes com filas padronizadas por grupo.

## Seed inicial

Foi incluido seed minimo com uma fila por grupo tecnico inicial:

- Fila Service Desk -> Service Desk
- Fila Suporte Tecnico -> Suporte Tecnico
- Fila Infraestrutura -> Infraestrutura
- Fila Sistemas -> Sistemas

Nao foram criadas filas adicionais para evitar poluicao do banco e manter a base estrutural enxuta.

## O que nao foi implementado nesta etapa

- Alteracao na entidade `Chamado`.
- Campo `FilaAtendimentoId` em chamado.
- Campo `GrupoTecnicoId` em chamado.
- Alteracao em `ResponsavelId`.
- Entidade ou historico de movimentacao de fila.
- Roteamento.
- Transferencia.
- Regra de assumir chamado.
- Cadastro funcional de filas.
- Endpoints, controllers ou services de aplicacao.
- Telas Vue.
- Alteracoes em SLA, relatorios, dashboard ou filtros de chamados.

## Riscos tecnicos

- Enquanto `Chamado` nao estiver vinculado a grupo/fila, a fila operacional real ainda continua implicita por `ResponsavelId == null`.
- A proxima etapa de vinculo deve evitar disparar SLA de primeira resposta apenas por direcionar chamado para fila.
- Relatorios e dashboards precisarao diferenciar volume em fila, grupo tecnico e responsavel individual em etapas futuras.
- A auditoria de movimentacao ainda nao existe; entrada, saida e transferencia de fila precisam de estrutura propria depois.

## Roadmap

Com entidade, configuracao EF, `DbSet`, migration, seed, aplicacao no banco e build concluidos, o item `Modelar entidade FilaAtendimento ou estrutura equivalente` pode ser marcado como concluido.

Percentual esperado da Sprint 3 apos esta etapa:

- 5 itens concluidos.
- 54 itens ativos.
- Percentual esperado: 9%.

## Proxima etapa recomendada

Definir o vinculo entre chamado e grupo tecnico, mantendo `ResponsavelId` opcional e sem remover o fluxo atual de responsavel individual.
