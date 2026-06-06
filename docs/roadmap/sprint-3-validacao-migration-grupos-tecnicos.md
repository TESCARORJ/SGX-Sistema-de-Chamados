# Validacao da migration de grupos tecnicos

## Contexto

O item "Criar migration para grupos tecnicos" da Sprint 3 foi tratado como uma conciliacao, pois a entidade `GrupoTecnico` e a tabela `grupos_tecnicos` ja haviam sido criadas anteriormente pela migration estrutural `20260605022142_ModelarGrupoTecnicoSprint3`.

Nenhuma nova tabela estrutural foi criada nesta etapa.

## Migration estrutural validada

Migration:

- `20260605022142_ModelarGrupoTecnicoSprint3`

Arquivo:

- `src/SGX.SistemaChamado.Infrastructure/Persistence/Migrations/20260605022142_ModelarGrupoTecnicoSprint3.cs`

## Estrutura validada da tabela

Tabela criada:

- `grupos_tecnicos`

Colunas validadas:

- `id` (`uuid`, obrigatorio)
- `nome` (`character varying(180)`, obrigatorio)
- `descricao` (`character varying(500)`, opcional)
- `criado_em` (`timestamp with time zone`, obrigatorio)
- `criado_por` (`character varying(120)`, obrigatorio)
- `atualizado_em` (`timestamp with time zone`, opcional)
- `atualizado_por` (`character varying(120)`, opcional)
- `ativo` (`boolean`, obrigatorio)

## Indices validados

- `ux_grupos_tecnicos_nome`: indice unico por `nome`
- `ix_grupos_tecnicos_ativo`: indice por `ativo`

## Seeds validados

A migration inclui seed inicial minimo para:

- `Service Desk`
- `Suporte Tecnico`
- `Infraestrutura`
- `Sistemas`

Os mesmos seeds estao refletidos em `SeedData.GruposTecnicos`.

## Configuracao EF Core validada

Arquivo:

- `src/SGX.SistemaChamado.Infrastructure/Persistence/Configurations/GrupoTecnicoConfiguration.cs`

Validacoes:

- `builder.ToTable("grupos_tecnicos")`
- propriedades mapeadas para nomes de coluna em snake_case
- `nome` obrigatorio com tamanho maximo 180
- `descricao` opcional com tamanho maximo 500
- campos de auditoria conforme padrao do projeto
- indice unico `ux_grupos_tecnicos_nome`
- indice `ix_grupos_tecnicos_ativo`
- seed via `SeedData.GruposTecnicos`

## DbContext validado

Arquivo:

- `src/SGX.SistemaChamado.Infrastructure/Persistence/SGXSistemaChamadoDbContext.cs`

DbSet validado:

- `public DbSet<GrupoTecnico> GruposTecnicos => Set<GrupoTecnico>();`

## Snapshot EF Core

O snapshot `SGXSistemaChamadoDbContextModelSnapshot.cs` contem:

- entidade `SGX.SistemaChamado.Domain.Entities.GrupoTecnico`
- tabela `grupos_tecnicos`
- indice `ux_grupos_tecnicos_nome`
- indice `ix_grupos_tecnicos_ativo`
- seeds iniciais dos grupos tecnicos

## Rollback

O metodo `Down` da migration estrutural remove a tabela `grupos_tecnicos` e desfaz os dados de roadmap alterados naquela etapa. O rollback e coerente com a criacao estrutural original.

## Migration desta etapa

Esta etapa nao criou migration estrutural duplicada.

Foi criada apenas uma migration de dados de roadmap/checklist para marcar o item "Criar migration para grupos tecnicos" como concluido e ajustar o percentual da Sprint 3 para 20%.

## Comandos executados

- `rg "grupos_tecnicos|GrupoTecnico|ux_grupos_tecnicos_nome|ix_grupos_tecnicos_ativo" src`
- `dotnet ef migrations list --configuration Release --project src\SGX.SistemaChamado.Infrastructure --startup-project src\SGX.SistemaChamado.Api --context SGXSistemaChamadoDbContext`
- `rg "SGX\.SistemaChamado\.Domain\.Entities\.GrupoTecnico|grupos_tecnicos|ux_grupos_tecnicos_nome|ix_grupos_tecnicos_ativo|Service Desk|Suporte Tecnico|Infraestrutura|Sistemas" src\SGX.SistemaChamado.Infrastructure\Persistence\Migrations\SGXSistemaChamadoDbContextModelSnapshot.cs`
- `dotnet ef migrations has-pending-model-changes --configuration Release --project src\SGX.SistemaChamado.Infrastructure --startup-project src\SGX.SistemaChamado.Api --context SGXSistemaChamadoDbContext`

## Conclusao

A migration estrutural existente atende ao item do checklist. A tabela, os indices, os campos, o seed, o snapshot, a configuracao EF Core e o `DbSet` estao coerentes.

## Proxima etapa recomendada

Conciliar o item "Criar migration para membros de grupo tecnico", validando a migration estrutural de `MembroGrupoTecnico` sem criar tabela duplicada.
