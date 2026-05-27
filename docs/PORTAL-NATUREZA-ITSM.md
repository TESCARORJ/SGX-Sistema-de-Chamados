# PORTAL NATUREZA ITSM

## Objetivo
Adaptar a abertura de chamado do portal para selecao explicita de `NaturezaChamado`, `ImpactoChamado` e `UrgenciaChamado`, mantendo o backend como autoridade final da validacao.

## Tela e rota utilizadas
- Tela: `src/SGX.SistemaChamado.Web/src/views/NovoChamadoView.vue`
- Rota ativa: `/portal/chamados/novo` em `src/SGX.SistemaChamado.Web/src/router/index.ts`
- Service reutilizado: `src/SGX.SistemaChamado.Web/src/services/portalService.ts`
- Types reutilizados: `src/SGX.SistemaChamado.Web/src/types/portal.ts`

## Como o portal usa NaturezaChamado
- O usuario seleciona explicitamente a natureza no formulario.
- Naturezas disponiveis:
  - Incidente
  - Requisicao
  - Mudanca
  - Problema
  - EventoAlerta
  - TarefaOperacional
- O formulario exibe orientacao contextual por natureza (texto de apoio e campos minimos esperados).

## Campos exibidos no formulario
- Classificacao ITSM:
  - `NaturezaChamado`
  - `ImpactoChamado`
  - `UrgenciaChamado`
- Dados do chamado:
  - `Titulo`
  - `Descricao`
- Classificacao operacional (fluxo existente):
  - `Categoria`
  - `Subcategoria`
  - `Prioridade` (fluxo legado atual do portal)
  - `TipoSolicitacao`
  - `LocalUnidade`
  - `Departamento` (quando aplicavel)
- Catalogo (fluxo existente):
  - `CatalogoServicoId`
  - `CatalogoServicoSlug`

## Campos obrigatorios por natureza (visao de UX)
- Incidente: titulo, descricao, impacto, urgencia.
- Requisicao: titulo, descricao e classificacao disponivel (categoria/tipo/catalogo).
- Mudanca: titulo, descricao detalhada, impacto, urgencia.
- Problema: titulo, descricao com evidencias/recorrencia, impacto, urgencia.
- EventoAlerta: titulo, descricao, impacto, urgencia.
- TarefaOperacional: titulo, descricao, impacto, urgencia.

## Impacto e urgencia no fluxo
- `ImpactoChamado` e `UrgenciaChamado` sao selecoes obrigatorias de formulario.
- O payload enviado para `POST /api/portal/chamados` inclui:
  - `naturezaChamado`
  - `impactoChamado`
  - `urgenciaChamado`
  - demais campos de abertura ja existentes.
- A prioridade final permanece responsabilidade do backend.

## Frontend x Backend
- Frontend: orienta preenchimento, aplica validacoes de UX e envia payload completo.
- Backend: valida regra de negocio definitiva e bloqueia combinacoes invalidas.

## Limitacoes atuais
- Nao ha formulario dinamico com campos estruturais especificos por natureza nesta sprint.
- Nao foi implementado calculo visual de prioridade no frontend.
- Fluxos de e-mail e regras de backend/ITSM existentes nao foram alterados.

## Pendencias futuras
- Campos especificos de Mudanca:
  - `JustificativaMudanca`
  - `JanelaMudanca`
  - `PlanoRollback`
- Campos especificos de Problema:
  - `CausaRaizProblema`
  - `EvidenciaProblema`
- Campos especificos de Evento/Alerta:
  - `OrigemAlerta`
  - `SeveridadeAlerta`
- Evolucao de formularios dinamicos avancados por natureza.
- Exibicao opcional de prioridade calculada em tempo real (sem duplicar regra de backend).
