# Validacao da migration de membros de grupo tecnico

## Contexto

O item "Criar migration para membros de grupo tecnico" da Sprint 3 foi tratado como conciliacao, pois a entidade `MembroGrupoTecnico` e a tabela `membros_grupos_tecnicos` ja haviam sido criadas anteriormente pela migration estrutural `20260605031435_ModelarMembroGrupoTecnicoSprint3`.

Nenhuma nova tabela estrutural foi criada nesta etapa.

## Migration estrutural validada

Migration:

- `20260605031435_ModelarMembroGrupoTecnicoSprint3`

Arquivo:

- `src/SGX.SistemaChamado.Infrastructure/Persistence/Migrations/20260605031435_ModelarMembroGrupoTecnicoSprint3.cs`

## Estrutura validada da tabela

Tabela criada:

- `membros_grupos_tecnicos`

Colunas validadas:

- `id` (`uuid`, obrigatorio)
- `grupo_tecnico_id` (`uuid`, obrigatorio)
- `usuario_id` (`uuid`, obrigatorio)
- `criado_em` (`timestamp with time zone`, obrigatorio)
- `criado_por` (`character varying(120)`, obrigatorio)
- `atualizado_em` (`timestamp with time zone`, opcional)
- `atualizado_por` (`character varying(120)`, opcional)
- `ativo` (`boolean`, obrigatorio)

## Relacionamento com GrupoTecnico

A migration criou a FK:

- `FK_membros_grupos_tecnicos_grupos_tecnicos_grupo_tecnico_id`

Destino:

- tabela `grupos_tecnicos`
- coluna `id`

Regra:

- relacionamento obrigatorio
- `onDelete: ReferentialAction.Restrict`

A configuracao EF Core confirma:

- `HasOne(x => x.GrupoTecnico)`
- `WithMany(x => x.Membros)`
- `HasForeignKey(x => x.GrupoTecnicoId)`
- `OnDelete(DeleteBehavior.Restrict)`

## Relacionamento com Usuario

A migration criou a FK:

- `FK_membros_grupos_tecnicos_usuarios_usuario_id`

Destino:

- tabela `usuarios`
- coluna `id`

Regra:

- relacionamento obrigatorio
- `onDelete: ReferentialAction.Restrict`

A configuracao EF Core confirma:

- `HasOne(x => x.Usuario)`
- `WithMany()`
- `HasForeignKey(x => x.UsuarioId)`
- `OnDelete(DeleteBehavior.Restrict)`

Esse relacionamento segue o padrao real do projeto, usando a entidade de dominio `Usuario` persistida na tabela `usuarios`.

## Indices validados

- `ix_membros_grupos_tecnicos_grupo_tecnico_id`: indice por `grupo_tecnico_id`
- `ix_membros_grupos_tecnicos_usuario_id`: indice por `usuario_id`
- `ix_membros_grupos_tecnicos_ativo`: indice por `ativo`
- `ux_membros_grupos_tecnicos_grupo_usuario`: indice unico composto por `grupo_tecnico_id` + `usuario_id`

O indice unico composto impede duplicidade do mesmo usuario no mesmo grupo tecnico e permite que o mesmo usuario participe de grupos diferentes.

## Configuracao EF Core validada

Arquivo:

- `src/SGX.SistemaChamado.Infrastructure/Persistence/Configurations/MembroGrupoTecnicoConfiguration.cs`

Validacoes:

- `builder.ToTable("membros_grupos_tecnicos")`
- propriedades mapeadas para nomes de coluna em snake_case
- `GrupoTecnicoId` obrigatorio
- `UsuarioId` obrigatorio
- campos de auditoria conforme padrao do projeto
- indices simples por grupo, usuario e ativo
- indice unico composto por grupo e usuario
- FKs restritivas para `GrupoTecnico` e `Usuario`

## DbContext validado

Arquivo:

- `src/SGX.SistemaChamado.Infrastructure/Persistence/SGXSistemaChamadoDbContext.cs`

DbSet validado:

- `public DbSet<MembroGrupoTecnico> MembrosGruposTecnicos => Set<MembroGrupoTecnico>();`

## Snapshot EF Core

O snapshot `SGXSistemaChamadoDbContextModelSnapshot.cs` contem:

- entidade `SGX.SistemaChamado.Domain.Entities.MembroGrupoTecnico`
- tabela `membros_grupos_tecnicos`
- indices `ix_membros_grupos_tecnicos_grupo_tecnico_id`, `ix_membros_grupos_tecnicos_usuario_id`, `ix_membros_grupos_tecnicos_ativo`
- indice unico `ux_membros_grupos_tecnicos_grupo_usuario`
- relacionamentos com `GrupoTecnico` e `Usuario`

## Rollback

O metodo `Down` da migration estrutural remove a tabela `membros_grupos_tecnicos` e desfaz os dados de roadmap alterados naquela etapa. O rollback e coerente com a criacao estrutural original.

## Migration desta etapa

Esta etapa nao criou migration estrutural duplicada.

Foi criada apenas uma migration de dados de roadmap/checklist para marcar o item "Criar migration para membros de grupo tecnico" como concluido e ajustar o percentual da Sprint 3 para 22%.

## Comandos executados

- `rg "membros_grupos_tecnicos|MembroGrupoTecnico|ux_membros_grupos_tecnicos_grupo_usuario|ix_membros_grupos_tecnicos" src`
- `dotnet ef migrations list --configuration Release --project src\SGX.SistemaChamado.Infrastructure --startup-project src\SGX.SistemaChamado.Api --context SGXSistemaChamadoDbContext`
- `rg "SGX\.SistemaChamado\.Domain\.Entities\.MembroGrupoTecnico|membros_grupos_tecnicos|ux_membros_grupos_tecnicos_grupo_usuario|ix_membros_grupos_tecnicos|FK_membros_grupos_tecnicos|usuarios" src\SGX.SistemaChamado.Infrastructure\Persistence\Migrations\SGXSistemaChamadoDbContextModelSnapshot.cs`
- `dotnet ef migrations has-pending-model-changes --configuration Release --project src\SGX.SistemaChamado.Infrastructure --startup-project src\SGX.SistemaChamado.Api --context SGXSistemaChamadoDbContext`

## Conclusao

A migration estrutural existente atende ao item do checklist. A tabela, as FKs, os indices, o indice unico composto, o snapshot, a configuracao EF Core e o `DbSet` estao coerentes.

## Proxima etapa recomendada

Conciliar o item "Criar migration para fila ou vinculo de fila do chamado", validando as migrations estruturais ja existentes para `FilaAtendimento` e para o vinculo `Chamado.FilaAtendimentoId`, sem criar estrutura duplicada.
