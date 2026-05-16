# ITSM - Cadastros Administrativos

## Objetivo

Consolidar a visao de governanca ITSM do modulo de Cadastros Administrativos, reunindo status real de implementacao, evidencias documentais e pendencias de homologacao institucional.

## Escopo funcional do modulo

O modulo cobre os seguintes cadastros e seu uso no ciclo de chamados:
- Departamentos
- Categorias
- Subcategorias
- Prioridades
- Tipos de Solicitacao
- Locais/Unidades

## Status consolidado (Sprint 8)

- Status da implementacao: Implementado funcionalmente
- Status tecnico: Completo com pendencias evolutivas
- Situacao geral: trilha tecnica concluida, homologacao manual institucional pendente

## Linha do tempo de entregas

- Sprint 1: base tecnica e modelagem inicial dos cadastros
- Sprint 2: CRUD backend de Departamentos, Categorias e Subcategorias
- Sprint 3: CRUD backend de Prioridades, Tipos de Solicitacao e Locais/Unidades
- Sprint 4: telas administrativas no frontend
- Sprint 5: integracao dos cadastros ao fluxo de abertura/gestao de chamados
- Sprint 6: seed inicial idempotente e reforco de testes
- Sprint 7: checklist funcional tecnico e ajustes finais
- Sprint 8: consolidacao documental ITSM e checklist formal de homologacao

## Regras de negocio consolidadas

- novos chamados usam somente cadastros ativos para selecao;
- subcategoria deve pertencer a categoria selecionada;
- inativacao e logica para preservacao historica;
- chamados antigos mantem leitura de cadastros vinculados, mesmo inativos;
- filtros administrativos aceitam classificacao por cadastro integrado.

## Evidencias documentais da Sprint 8

- `docs/CADASTROS-ADMINISTRATIVOS.md` (consolidacao da trilha e Sprint 8)
- `docs/CHECKLIST-HOMOLOGACAO-CADASTROS.md` (checklist operacional de homologacao)
- `docs/ROADMAP.md` (registro da Sprint 8 no roadmap geral)
- `docs/ROADMAP-ITSM.md` (registro da Sprint 8 no roadmap ITSM)

## Pendencias para encerramento institucional

- executar checklist manual completo em ambiente de homologacao com usuarios reais;
- anexar evidencias visuais por tela/fluxo (prints ou video curto);
- registrar aceite formal dos perfis Administrador, Atendente e Solicitante.

## Proxima acao recomendada

Executar a homologacao guiada pelo documento `docs/CHECKLIST-HOMOLOGACAO-CADASTROS.md` e registrar o resultado por item (aprovado/reprovado), com observacoes de correcao quando aplicavel.
