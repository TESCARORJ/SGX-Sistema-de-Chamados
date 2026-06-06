# Modelagem da entidade MembroGrupoTecnico

## Contexto

Esta etapa da Sprint 3 cria a estrutura de dominio e persistencia para associar usuarios a grupos tecnicos. A implementacao permanece limitada a modelagem; nao altera chamado, fila, roteamento, transferencia, SLA, endpoints ou telas.

## Decisao de modelagem

`MembroGrupoTecnico` foi criado separado de `GrupoTecnico` porque a relacao entre grupos e usuarios e naturalmente muitos-para-muitos:

- um grupo tecnico pode ter varios usuarios;
- um usuario pode participar de varios grupos tecnicos;
- a participacao precisa ter ciclo de vida proprio, com ativacao e inativacao logica.

`GrupoTecnico` permanece como unidade corporativa de atendimento. `MembroGrupoTecnico` representa apenas a participacao de um usuario nessa unidade.

## Entidade de usuario usada

A entidade de usuario do dominio atual e `Usuario`, localizada em `src/SGX.SistemaChamado.Domain/Entities/Usuario.cs`.

O relacionamento usa:

- `MembroGrupoTecnico.UsuarioId`
- navegacao `MembroGrupoTecnico.Usuario`

Essa decisao segue o padrao ja usado por entidades como `ChamadoTarefa`, que referencia `Usuario` por campos `Guid` e relacionamento EF Core com `DeleteBehavior.Restrict`.

## Estrutura da entidade

Entidade: `MembroGrupoTecnico`

Campos:

- `Id`: `Guid`, herdado de `EntityBase`.
- `GrupoTecnicoId`: obrigatorio.
- `UsuarioId`: obrigatorio.
- `Ativo`: herdado de `AuditableEntity`, inicia ativo por padrao.
- `CriadoEm`: herdado de `AuditableEntity`.
- `CriadoPor`: herdado de `AuditableEntity`.
- `AtualizadoEm`: herdado de `AuditableEntity`.
- `AtualizadoPor`: herdado de `AuditableEntity`.

Navegacoes:

- `GrupoTecnico`
- `Usuario`

Metodos:

- Construtor publico com `grupoTecnicoId`, `usuarioId` e `criadoPor`.
- Construtor privado para EF Core.
- `Inativar`.
- `Reativar`.

Validacoes:

- Nao permite `GrupoTecnicoId` vazio.
- Nao permite `UsuarioId` vazio.

## Estrutura da tabela

Tabela: `membros_grupos_tecnicos`

Colunas:

- `id`
- `grupo_tecnico_id`
- `usuario_id`
- `criado_em`
- `criado_por`
- `atualizado_em`
- `atualizado_por`
- `ativo`

Relacionamentos:

- FK obrigatoria para `grupos_tecnicos`.
- FK obrigatoria para `usuarios`.
- Delete restrito nos dois relacionamentos.

## Indices criados

- `ix_membros_grupos_tecnicos_grupo_tecnico_id`
- `ix_membros_grupos_tecnicos_usuario_id`
- `ix_membros_grupos_tecnicos_ativo`
- `ux_membros_grupos_tecnicos_grupo_usuario`

## Decisao sobre unicidade

Foi criado indice unico composto por `grupo_tecnico_id` + `usuario_id`.

Motivo:

- impede duplicidade do mesmo usuario no mesmo grupo;
- permite que o mesmo usuario esteja em grupos diferentes;
- permite que um grupo tenha varios usuarios;
- mantem a inativacao logica como forma de preservar historico da participacao.

Se um membro for removido logicamente e precisar voltar ao mesmo grupo, a operacao futura deve reativar o registro existente em vez de criar um duplicado.

## Ativacao e inativacao

A entidade herda `Ativo`, `CriadoEm`, `CriadoPor`, `AtualizadoEm` e `AtualizadoPor` de `AuditableEntity`.

Foram expostos metodos `Inativar` e `Reativar`, alinhados ao uso de soft delete do projeto.

## O que nao foi implementado nesta etapa

- Entidade `FilaAtendimento`.
- Alteracao em `Chamado`.
- Campo `GrupoTecnicoId` em chamado.
- Alteracao em `ResponsavelId`.
- Endpoints, controllers ou services de aplicacao.
- Telas Vue.
- Cadastro funcional de membros.
- Roteamento, transferencia ou assumir por grupo.
- Alteracoes em SLA.
- Alteracoes em dashboard ou relatorios.
- Seeds de membros de grupo.

## Roadmap

Com a entidade, configuracao EF, `DbSet`, migration, aplicacao no banco e build concluidos, o item `Modelar entidade MembroGrupoTecnico` pode ser marcado como concluido.

Percentual esperado da Sprint 3 apos esta etapa:

- 4 itens concluidos.
- 54 itens ativos.
- Percentual esperado: 7%.

## Proxima etapa recomendada

Modelar `FilaAtendimento` ou estrutura equivalente, ainda sem alterar o comportamento de `ResponsavelId` e sem implementar roteamento funcional.
