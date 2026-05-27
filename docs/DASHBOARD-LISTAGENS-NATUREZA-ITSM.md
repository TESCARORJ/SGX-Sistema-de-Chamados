# DASHBOARD-LISTAGENS-NATUREZA-ITSM

## 1. Objetivo
Adicionar suporte a `NaturezaChamado` na experiencia administrativa para:
- filtrar listagem de chamados;
- filtrar dashboard/indicadores;
- exibir contadores por natureza ITSM.

## 2. Filtro por Natureza na listagem admin
- endpoint reutilizado: `GET /api/admin/chamados`
- request evoluido com campo opcional `naturezaChamado`
- combina com filtros existentes (status, prioridade, categoria, periodo, SLA, responsavel etc.)
- no frontend, filtro inclui:
  - Todos
  - Incidente
  - Requisicao
  - Mudanca
  - Problema
  - Evento/Alerta
  - Tarefa operacional

## 3. Indicadores por Natureza no dashboard admin
- endpoint reutilizado: `GET /api/admin/dashboard`
- response evoluida com `chamadosPorNatureza`:
  - `codigo`
  - `natureza`
  - `total`
- contadores retornam sempre as 6 naturezas ITSM, inclusive com total `0` quando nao houver ocorrencia no recorte.

## 4. Calculo dos contadores
- fonte: chamados ativos retornados pelo recorte atual do dashboard;
- agregacao: `GroupBy(NaturezaChamado)` no backend via LINQ;
- o dashboard aplica os filtros informados e depois consolida os totais por natureza.

## 5. Relacao com filtros existentes
- filtros de periodo, departamento, categoria e responsavel permanecem ativos;
- `naturezaChamado` foi adicionada como filtro opcional adicional;
- quando informada, restringe listagem e indicadores ao tipo selecionado.

## 6. Backend como autoridade final
- frontend faz filtro visual e envio de parametro;
- backend executa filtro real nos use cases e retorna dados consolidados corretos.

## 7. Limitacoes atuais
- a secao por natureza no dashboard e agregada (contagem), sem detalhamento de subdimensionais;
- nao houve alteracao de regras de SLA, status por natureza ou prioridade nesta sprint.

## 8. Pendencias futuras
- consolidar natureza em relatorios avancados com recortes mais analiticos;
- exportacoes dedicadas por natureza (tendencia por periodo, categoria e responsavel);
- visualizacoes graficas adicionais por natureza quando houver demanda de gestao.
