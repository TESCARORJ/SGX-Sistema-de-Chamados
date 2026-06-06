# Modelagem da entidade GrupoTecnico

## Contexto

Esta etapa da Sprint 3 introduz a entidade `GrupoTecnico` como unidade corporativa de atendimento. A modelagem segue a analise anterior registrada em `docs/roadmap/sprint-3-impacto-responsavel-chamado.md`, que concluiu que o responsavel individual do chamado deve continuar separado dos futuros conceitos de grupo tecnico e fila.

## Decisao de modelagem

`GrupoTecnico` foi criado como entidade independente de `Chamado.ResponsavelId`.

Motivos:

- `ResponsavelId` representa um usuario/tecnico individual.
- Chamados devem poder permanecer sem responsavel individual enquanto estiverem em uma futura fila de grupo.
- O grupo tecnico precisa existir para roteamento, filtros, produtividade e auditoria futura, sem alterar o fluxo atual de atribuicao individual.
- A Sprint 3 ainda nao deve vincular chamado a grupo tecnico; essa relacao sera definida em item posterior do checklist.

## Estrutura da entidade

Entidade: `GrupoTecnico`

Campos:

- `Id`: `Guid`, herdado de `EntityBase`.
- `Nome`: obrigatorio.
- `Descricao`: opcional.
- `Ativo`: herdado de `AuditableEntity`, inicia ativo por padrao.
- `CriadoEm`: herdado de `AuditableEntity`.
- `CriadoPor`: herdado de `AuditableEntity`.
- `AtualizadoEm`: herdado de `AuditableEntity`.
- `AtualizadoPor`: herdado de `AuditableEntity`.

Metodos de dominio:

- Construtor publico com `nome`, `descricao` e `criadoPor`.
- Construtor privado para EF Core.
- `AlterarDados`: atualiza nome e descricao com auditoria.
- `Inativar`: inativa o grupo com auditoria.
- `Reativar`: reativa o grupo com auditoria.

Validacoes:

- Nome nao pode ser vazio.
- Atualizacao nao permite nome vazio.
- Descricao vazia e normalizada para `null`.

## Estrutura de banco

Tabela: `grupos_tecnicos`

Colunas:

- `id`
- `nome`
- `descricao`
- `criado_em`
- `criado_por`
- `atualizado_em`
- `atualizado_por`
- `ativo`

Indices:

- `ux_grupos_tecnicos_nome`: indice unico por `nome`.
- `ix_grupos_tecnicos_ativo`: indice simples por `ativo`.

## Decisao sobre indice unico de nome

Foi adotado indice unico para `nome`. O projeto ja usa unicidade em cadastros de referencia com nome operacional estavel, como `LocalUnidade`, `TipoSolicitacao`, `PerfilAcesso`, `TipoAtivoInventario` e `RoadmapCategoria`.

Para grupo tecnico, nomes duplicados poderiam gerar ambiguidade em filtros, produtividade, roteamento e auditoria. Por isso, a unicidade foi definida desde a tabela inicial.

## Seed inicial

Foi incluido seed minimo com quatro grupos tecnicos:

- Service Desk
- Suporte Tecnico
- Infraestrutura
- Sistemas

A decisao foi manter o seed pequeno para permitir testes e evolucao futura sem poluir o banco com grupos especulativos. Nenhum chamado, usuario ou permissao foi vinculado a esses grupos nesta etapa.

## O que nao foi implementado nesta etapa

- Entidade `MembroGrupoTecnico`.
- Entidade `FilaAtendimento`.
- Vinculo entre chamado e grupo tecnico.
- Vinculo entre chamado e fila.
- Regras de roteamento.
- Regras de transferencia.
- Regra de assumir chamado pela fila.
- Endpoints, controllers ou services de aplicacao.
- Telas ou componentes Vue.
- Alteracoes em `ResponsavelId`.
- Alteracoes em SLA.
- Homologacao ou aceite com usuario.

## Impacto no modelo atual de responsavel

Nenhum comportamento de `ResponsavelId` foi alterado. O campo continua opcional e representando responsavel individual.

A futura convivencia esperada permanece:

- Chamado pode estar sem responsavel individual.
- Chamado podera futuramente estar associado a grupo tecnico.
- Ao assumir chamado, o tecnico podera continuar sendo registrado em `ResponsavelId`.
- Grupo tecnico deve permanecer separado para rastreabilidade, produtividade, filtros e auditoria.

## Roadmap

Com a entidade, configuracao EF, `DbSet`, migration, aplicacao no banco e build concluidos, o item `Modelar entidade GrupoTecnico` pode ser marcado como concluido.

Percentual esperado da Sprint 3 apos esta etapa:

- 3 itens concluidos.
- 54 itens ativos.
- Percentual esperado: 6%.

## Proxima etapa recomendada

Modelar `MembroGrupoTecnico`, mantendo a separacao entre:

- grupo tecnico corporativo;
- usuario membro do grupo;
- responsavel individual do chamado;
- futura fila de atendimento.
