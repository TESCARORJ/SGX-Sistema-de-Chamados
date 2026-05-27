# RELATORIOS-NATUREZA-ITSM

## Objetivo
Evoluir os relatórios administrativos/avançados para permitir filtro, exibição e consolidação por `NaturezaChamado`, reaproveitando os endpoints e contratos existentes.

## Relatórios impactados
- Relatório de chamados:
  - `GET /api/admin/relatorios-avancados/chamados/resumo`
  - `GET /api/admin/relatorios-avancados/chamados/serie-temporal`
  - `GET /api/admin/relatorios-avancados/chamados/distribuicao`
- Relatórios de SLA (filtro opcional por natureza, sem alterar cálculo):
  - `GET /api/admin/relatorios-avancados/sla/resumo`
  - `GET /api/admin/relatorios-avancados/sla/violacoes`
  - `GET /api/admin/relatorios-avancados/sla/por-departamento`
  - `GET /api/admin/relatorios-avancados/sla/por-prioridade`

## Filtro por NaturezaChamado
- Campo opcional adicionado aos filtros:
  - `FiltroRelatorioChamadosRequest.NaturezaChamado`
  - `FiltroRelatorioSlaRequest.NaturezaChamado`
- O filtro combina com os filtros já existentes (período, status, prioridade, categoria, departamento etc).

## Colunas e exibição
- O frontend de relatórios de chamados passou a exibir:
  - filtro visual de natureza ITSM;
  - consolidado por natureza no resumo;
  - opção de distribuição por natureza.
- Labels amigáveis aplicadas:
  - Incidente
  - Requisição
  - Mudança
  - Problema
  - Evento/Alerta
  - Tarefa operacional

## Agregações por NaturezaChamado
- Novo agregado no resumo de chamados: `TotalPorNatureza`.
- Distribuição de chamados aceita `AgruparPor = Natureza`.
- O consolidado retorna as 6 naturezas, incluindo quantidade zero quando não há ocorrências no período/filtro.

## Relação com SLA
- Relatórios de SLA recebem filtro opcional por natureza.
- Não houve alteração nas regras/cálculo de SLA, pausas ou semântica de cumprimento/violação.

## Limitações atuais
- Não foi criado relatório novo específico por natureza; a evolução foi feita sobre os relatórios existentes.
- Não houve alteração em exportações avançadas (XLSX/PDF) além do uso do resumo/distribuição já disponíveis na tela.

## Pendências futuras
- Relatório dedicado de mudanças (aprovação/janela/rollback).
- Relatório de problema com causa raiz/recorrência.
- Relatório de evento/alerta com correlação e origem técnica.
- Exportações avançadas por natureza com layouts especializados.
