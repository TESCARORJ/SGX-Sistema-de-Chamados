# Roadmap ITSM - SGX Sistema de Chamados

## Objetivo do Roadmap ITSM

O Roadmap ITSM organiza a evolucao funcional e tecnica do SGX Sistema de Chamados com foco em governanca, previsibilidade e rastreabilidade de entrega.

## Campos principais do item de roadmap

- Area
- Categoria (legado)
- RoadmapCategoriaId (referencia principal)
- Ordem
- SituacaoAtual
- AtencaoTecnica
- Status (geral/legado)
- Prioridade
- Impacto
- Decisao
- Responsavel
- PrazoAlvo
- Ativo
- Observacao

## Categoria como cadastro controlado

A categoria do Roadmap ITSM agora e controlada por cadastro em tabela propria (`RoadmapCategoria`).

Regras aplicadas:
- nome obrigatorio e unico;
- inativacao logica (sem exclusao fisica);
- dropdown de criacao/edicao usa somente categorias ativas;
- itens antigos continuam exibindo categoria legada quando necessario.

Campos da categoria:
- Nome
- Descricao
- Cor
- Icone
- Ordem
- Ativo

## Status real da implementacao

O campo `Status` foi mantido para compatibilidade e leitura geral.

Para status real, a referencia principal e `StatusImplementacao`, complementada por `StatusTecnico` e checklist.

Campos da secao:
- StatusImplementacao
- StatusTecnico
- PercentualImplementacao
- PendenciasTecnicas
- PendenciasHomologacao
- EvidenciaImplementacao
- DataConclusaoTecnica
- DataHomologacao
- CriterioAceite
- ProximaAcao

### Significado de "Implementado funcionalmente"

`Implementado funcionalmente` indica entrega tecnica da funcionalidade, sem implicar automaticamente homologacao final ou producao.

### Pendencias evolutivas

Quando `StatusTecnico = Completo com pendencias evolutivas`, registrar obrigatoriamente:
- o que foi concluido;
- o que falta para homologacao/producao;
- proxima acao priorizada.

## Percentual por checklist

O percentual deixou de depender de digitacao manual quando ha checklist ativo.

Regra de calculo:
- `PercentualImplementacao = itens ativos concluidos / itens ativos * 100`

Comportamento:
- se existir checklist ativo, a UI mostra percentual calculado e bloqueia edicao manual;
- se nao existir checklist ativo, o valor legado pode ser usado como fallback.

## Checklist da implementacao

Cada item de roadmap pode ter varios itens em `RoadmapChecklistItem`.

Campos principais:
- Titulo
- Descricao
- Grupo
- Ordem
- Concluido
- Obrigatorio
- Ativo

Grupos sugeridos:
- Planejamento
- Desenvolvimento
- Testes
- Documentacao
- Homologacao
- Producao

## CRUD de futuras implementacoes

Cada item de roadmap pode ter N evolucoes em `RoadmapImplementacaoFutura`.

Campos:
- Titulo
- Descricao
- Tipo
- Prioridade
- Status
- Responsavel
- PrazoAlvo
- DataConclusao
- Observacao
- Ativo

Regras:
- vinculo obrigatorio ao item de roadmap;
- inativacao logica;
- concluir/inativar/reativar;
- filtros por status, tipo, prioridade, responsavel e ativo.

## Labels amigaveis na UI

A interface nao deve exibir enums crus.

Exemplos esperados:
- `EmValidacao` -> `Em validacao`
- `NaoIniciado` -> `Nao iniciado`
- `NaoAvaliado` -> `Nao avaliado`
- `CompletoComPendenciasEvolutivas` -> `Completo com pendencias evolutivas`

Aplicacao:
- `QSelect` mostra label amigavel e salva valor tecnico;
- `QTable` e `QBadge` mostram label amigavel;
- contratos de API preservam valor tecnico para integracao.

## Exemplo - Perfis de acesso

Preenchimento recomendado:
- Categoria: `Seguranca` (via `RoadmapCategoriaId`)
- Status (legado): `Implementado`
- StatusImplementacao: `Implementado funcionalmente`
- StatusTecnico: `Completo com pendencias evolutivas`
- Checklist ativo: 10 itens, 9 concluidos e 1 pendente
- Percentual calculado: `90%`
- PendenciasTecnicas: auditoria detalhada de alteracoes, testes frontend/e2e e validacao fina em homologacao
- PendenciasHomologacao: validacao com usuarios reais (Administrador, Atendente, Solicitante)
- EvidenciaImplementacao: `docs/SEGURANCA-PERFIS-PERMISSOES.md`, `docs/ROADMAP.md`, testes backend e matriz frontend
- CriterioAceite: admin gerencia permissoes; atendente ve acoes permitidas; solicitante nao acessa admin; backend bloqueia sem permissao
- ProximaAcao: executar homologacao real e priorizar auditoria detalhada

Futuras implementacoes sugeridas:
- Auditoria detalhada de alteracoes de permissoes
- Testes frontend/e2e da matriz de permissoes
- Validacao com usuarios reais
- Relatorio de permissoes por perfil
- Exportacao da matriz de permissoes

## Observacao de permissao

Nesta iteracao, endpoints de categoria/checklist reutilizam `Roadmap.Visualizar` e `Roadmap.Gerenciar`.

Pendencia real para evolucao futura:
- avaliar criacao de permissao granular dedicada para categorias/checklist (`RoadmapCategorias.*`, `RoadmapChecklist.*`).
