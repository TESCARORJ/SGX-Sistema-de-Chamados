# Validacao de indices para grupo, fila e responsavel

## Contexto

O item "Criar indices necessarios para consulta por grupo, fila e responsavel" foi tratado como conciliacao tecnica. As migrations anteriores da Sprint 3 ja criaram varios indices relacionados a grupos tecnicos, filas de atendimento, membros de grupo e chamados.

Nesta etapa foram validados os indices existentes antes de decidir qualquer nova migration estrutural.

## Indices existentes em chamados

Arquivo de configuracao:

- `src/SGX.SistemaChamado.Infrastructure/Persistence/Configurations/ChamadoConfiguration.cs`

Indices explicitamente configurados:

- `ux_chamados_codigo`: indice unico por `Codigo`
- `ix_chamados_catalogo_servico_id`: indice por `CatalogoServicoId`
- `ix_chamados_fila_atendimento_id`: indice por `FilaAtendimentoId`
- `ix_chamados_grupo_tecnico_id`: indice por `GrupoTecnicoId`
- `ix_chamados_inventario_ativo_id`: indice por `InventarioAtivoId`

Indice por responsavel:

- `ResponsavelId` possui indice no snapshot EF Core como indice convencional da FK.
- No banco, esse indice atende consultas por responsavel individual, ainda que sem nome customizado `ix_chamados_responsavel_id`.

Usos reais identificados:

- `ListarChamadosAdminUseCase` filtra por `ResponsavelId`.
- `AdminIndicadoresUseCases` filtra e agrupa por `ResponsavelId`.
- `RelatoriosAvancadosAdminUseCases` filtra e agrupa por `ResponsavelId`.
- Frontend administrativo envia `responsavelId` em filtros de chamados, dashboard e relatorios.

Conclusao:

- Ja existe cobertura de indice para grupo tecnico, fila de atendimento e responsavel individual em `chamados`.

## Indices existentes em grupos_tecnicos

Arquivo de configuracao:

- `src/SGX.SistemaChamado.Infrastructure/Persistence/Configurations/GrupoTecnicoConfiguration.cs`

Indices:

- `ux_grupos_tecnicos_nome`: indice unico por `Nome`
- `ix_grupos_tecnicos_ativo`: indice por `Ativo`

Conclusao:

- Os indices atendem consulta por nome e filtros por grupos ativos.

## Indices existentes em filas_atendimento

Arquivo de configuracao:

- `src/SGX.SistemaChamado.Infrastructure/Persistence/Configurations/FilaAtendimentoConfiguration.cs`

Indices:

- `ix_filas_atendimento_grupo_tecnico_id`: indice por `GrupoTecnicoId`
- `ix_filas_atendimento_ativo`: indice por `Ativo`
- `ux_filas_atendimento_grupo_nome`: indice unico composto por `GrupoTecnicoId` + `Nome`

Conclusao:

- Os indices atendem consulta de filas por grupo, filtros por filas ativas e protecao contra duplicidade de nome dentro do grupo.

## Indices existentes em membros_grupos_tecnicos

Arquivo de configuracao:

- `src/SGX.SistemaChamado.Infrastructure/Persistence/Configurations/MembroGrupoTecnicoConfiguration.cs`

Indices:

- `ix_membros_grupos_tecnicos_grupo_tecnico_id`: indice por `GrupoTecnicoId`
- `ix_membros_grupos_tecnicos_usuario_id`: indice por `UsuarioId`
- `ix_membros_grupos_tecnicos_ativo`: indice por `Ativo`
- `ux_membros_grupos_tecnicos_grupo_usuario`: indice unico composto por `GrupoTecnicoId` + `UsuarioId`

Conclusao:

- Os indices atendem consulta de membros por grupo, consulta de grupos por usuario, filtros por membros ativos e bloqueio de associacao duplicada.

## Validacao no snapshot EF Core

O snapshot `SGXSistemaChamadoDbContextModelSnapshot.cs` confirma:

- `HasIndex("FilaAtendimentoId")`
- `HasIndex("GrupoTecnicoId")`
- `HasIndex("ResponsavelId")`
- `HasIndex("GrupoTecnicoId", "Nome")` em `FilaAtendimento`
- `HasIndex("GrupoTecnicoId", "UsuarioId")` em `MembroGrupoTecnico`
- indices por `Ativo` em grupos, filas e membros

## Decisao sobre novos indices

Nao foi criado novo indice estrutural nesta etapa.

Motivos:

- O indice por `ResponsavelId` ja existe por convencao da FK no modelo EF.
- Os indices simples por grupo e fila em `chamados` ja existem com nomes explicitos.
- Os indices de grupo, fila e membros ja cobrem os filtros operacionais basicos da Sprint 3.
- Nao ha consulta real implementada que use combinacoes como `grupo_tecnico_id + fila_atendimento_id`, `grupo_tecnico_id + responsavel_id` ou `fila_atendimento_id + responsavel_id`.
- Criar indices compostos agora poderia antecipar custo de escrita e manutencao sem ganho comprovado.

## Justificativa para nao criar indice composto

Os fluxos atuais e imediatamente planejados filtram por colunas simples:

- chamados por grupo tecnico;
- chamados por fila;
- chamados por responsavel;
- membros por grupo;
- membros por usuario;
- grupos/filas/membros ativos.

Os indices compostos devem ser avaliados quando existirem consultas concretas de fila operacional, listagens ou relatorios usando filtros combinados de forma recorrente.

## Confirmacao de ausencia de alteracao funcional

Esta etapa nao alterou:

- entidades de dominio;
- endpoints;
- telas;
- SLA;
- dashboard;
- relatorios;
- regras de assumir, atribuir ou transferir chamados;
- obrigatoriedade de `GrupoTecnicoId` ou `FilaAtendimentoId`;
- comportamento de `ResponsavelId`.

## Migration desta etapa

Foi criada apenas uma migration de dados de roadmap/checklist para marcar o item "Criar indices necessarios para consulta por grupo, fila e responsavel" como concluido e ajustar o percentual da Sprint 3 para 26%.

Nao houve migration estrutural de indice.

## Comandos executados

- `rg "HasIndex|grupo_tecnico_id|fila_atendimento_id|responsavel_id|ResponsavelId|ix_chamados|ux_" src`
- `rg "ResponsavelId|responsavel|responsavelId" src\SGX.SistemaChamado.Application src\SGX.SistemaChamado.Api src\SGX.SistemaChamado.Web -g "!*Migrations*"`
- `rg -n "ResponsavelId|responsavel_id|GrupoTecnicoId|FilaAtendimentoId" src\SGX.SistemaChamado.Infrastructure\Persistence\Migrations\SGXSistemaChamadoDbContextModelSnapshot.cs`
- `dotnet ef migrations list --configuration Release --project src\SGX.SistemaChamado.Infrastructure --startup-project src\SGX.SistemaChamado.Api --context SGXSistemaChamadoDbContext`
- `dotnet ef migrations has-pending-model-changes --configuration Release --project src\SGX.SistemaChamado.Infrastructure --startup-project src\SGX.SistemaChamado.Api --context SGXSistemaChamadoDbContext`

## Conclusao

Os indices existentes sao suficientes para o escopo atual da Sprint 3. O item foi conciliado sem criar indices duplicados ou especulativos.

## Proxima etapa recomendada

Seguir para "Garantir compatibilidade com chamados existentes", validando dados legados com `GrupoTecnicoId`, `FilaAtendimentoId` e `ResponsavelId` opcionais, sem alterar comportamento funcional.
