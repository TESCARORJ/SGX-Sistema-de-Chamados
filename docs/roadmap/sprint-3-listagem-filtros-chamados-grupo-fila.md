# Sprint 3 - Listagem e filtros de chamados por grupo e fila

## Tela/listagem alterada

- `src/SGX.SistemaChamado.Web/src/views/AdminChamadosView.vue`
- `src/SGX.SistemaChamado.Web/src/components/admin/TabelaChamados.vue`
- `src/SGX.SistemaChamado.Web/src/components/admin/FiltrosChamadoAdmin.vue`

## Campos exibidos

A listagem administrativa de chamados passou a exibir a coluna compacta `Atendimento`, com:

- grupo tecnico;
- fila de atendimento.

No modo mobile, as mesmas informacoes aparecem no card do chamado.

## Valores nulos

Chamados legados ou ainda nao direcionados exibem:

- `Sem grupo`;
- `Sem fila`.

## Filtros adicionados

Foram adicionados filtros opcionais por:

- `grupoTecnicoId`;
- `filaAtendimentoId`.

Os filtros usam os nomes ja suportados pelo backend em `FiltroChamadosAdminRequest` e pelo service frontend.

## Carregamento de filas

Os grupos tecnicos ativos sao carregados via service administrativo de grupos. Ao selecionar um grupo, a tela carrega as filas ativas daquele grupo usando o endpoint existente:

- `GET /api/admin/grupos-tecnicos/{grupoTecnicoId}/filas`

Ao trocar ou limpar o grupo tecnico, o filtro de fila e limpo.

## Preservacao dos filtros existentes

Foram preservados os filtros atuais de texto, natureza ITSM, status, prioridade, responsavel, classificacao, solicitante, periodo, SLA e ordenacao.

## Testes

Foram ajustados testes frontend para cobrir:

- exibicao de atendimento na tabela;
- fallbacks `Sem grupo` e `Sem fila`;
- filtros de grupo tecnico e fila de atendimento;
- limpeza da fila ao trocar o grupo;
- uso dos services ja existentes de grupos e filas.

## O que nao foi implementado

- Nenhum endpoint novo.
- Nenhuma regra backend nova.
- Nenhuma acao operacional nova na listagem.
- Nenhuma tela nova.
- Nenhuma alteracao de detalhe do chamado.
- Nenhuma alteracao de SLA, dashboard ou relatorio.
- Nenhuma migration estrutural.

## Proxima etapa recomendada

Testar cadastro de grupo tecnico.
