# Sprint 6 - Central de notificacoes no frontend

## 1. Objetivo

Implementar a central de notificacoes do usuario autenticado no frontend Vue/TypeScript, consumindo exclusivamente a API autenticada da caixa propria criada no item 13.

## 2. Estado anterior

Antes desta entrega, a Sprint 6 ja possuia notificacoes persistentes, API autenticada de consulta e leitura, contagem de nao lidas e ownership no backend. O frontend ainda nao tinha uma central real integrada a essa API.

## 3. API consumida

Rotas autenticadas utilizadas:

- `GET /api/notificacoes/minhas`
- `GET /api/notificacoes/minhas/{id}`
- `GET /api/notificacoes/minhas/nao-lidas/contagem`
- `PATCH /api/notificacoes/minhas/{id}/lida`
- `PATCH /api/notificacoes/minhas/{id}/nao-lida`

## 4. Types

Foram criados types TypeScript especificos para resumo, detalhe, listagem paginada, alteracao de leitura e contagem de nao lidas.

## 5. Service

Foi criado um service HTTP dedicado para notificacoes, isolando a montagem de query string e o consumo das rotas autenticadas sem expor detalhes de transporte nas views.

## 6. Store/composable

Foi reutilizado o store de notificacoes para uma responsabilidade minima compartilhada: carregar e sincronizar o contador global de nao lidas entre layout e pagina.

## 7. Rota

Foi criada a rota autenticada da central em `/portal/notificacoes`, e a mesma view passou a atender `/admin/notificacoes` para manter consistencia de experiencia entre layouts autenticados.

## 8. Integracao com layout

O acesso a notificacoes foi integrado ao `PortalLayout` e ao `AdminLayout`, com badge no cabecalho e link de navegacao para a pagina da central.

## 9. Contador

O contador global usa a API autenticada de nao lidas e e recarregado ao montar os layouts, ao abrir a central e apos operacoes de marcar como lida e nao lida.

## 10. Listagem

A central lista somente dados retornados pela API, com pagina atual, total, total de paginas e total de nao lidas sincronizados com o backend.

## 11. Filtros

Os filtros disponiveis sao:

- `Todas`
- `Nao lidas`
- `Lidas`

O frontend traduz essas opcoes para o parametro `lida` sem enviar qualquer identificador de ownership.

## 12. Paginacao

A paginacao e server-side, com pagina inicial 1, navegacao por pagina e recarga da listagem a cada troca de pagina ou filtro.

## 13. Detalhe

O detalhe da notificacao e exibido em dialog proprio, carregado por endpoint explicito e sem efeito colateral de leitura automatica.

## 14. Marcar como lida

A interface oferece acao explicita por item e no detalhe para marcar como lida, atualizando o item, o total local e o contador global.

## 15. Marcar como nao lida

A interface oferece acao explicita por item e no detalhe para marcar como nao lida, preservando a idempotencia retornada pela API.

## 16. Idempotencia visual

Quando a API informa que o estado ja era o esperado, a interface mantem consistencia visual sem criar mutacoes extras nem mensagens enganosas.

## 17. Loading

Foram implementados estados de carregamento para lista, detalhe, contagem e acao por item, evitando duplo clique e bloqueio da pagina inteira.

## 18. Erros

Falhas de listagem, detalhe e alteracao de leitura exibem mensagens objetivas. Respostas `401`, `403`, `404` e `500` recebem tratamento sem expor stack trace ou detalhes internos.

## 19. Estado vazio

O estado vazio varia conforme o filtro ativo, distinguindo:

- ausencia total de notificacoes
- ausencia de notificacoes nao lidas
- ausencia de notificacoes lidas

## 20. Seguranca

O frontend nao decide ownership, nao compoe rotas de outros usuarios e apenas consome endpoints autenticados da caixa propria.

## 21. Ownership

Toda a validacao de ownership permanece no backend. O frontend apenas trata `404` e demais erros sem inferir existencia de notificacoes alheias.

## 22. Ausencia de `UsuarioId`

Nenhuma request da central envia `UsuarioId`, `DestinatarioUsuarioId`, `PerfilId` ou `GrupoId`.

## 23. Conteudo seguro

O conteudo da notificacao e renderizado como texto, sem `v-html`, preservando quebras de linha com CSS (`white-space: pre-wrap`).

## 24. Acessibilidade

Foram aplicados `aria-label`, titulos claros, indicacao textual de leitura e acoes acessiveis por teclado, sem depender apenas de cor para diferenciar leitura.

## 25. Responsividade

A central foi estruturada com cards e dialog responsivos, evitando tabela rigida e scroll horizontal em mobile.

## 26. Canal Sistema

A central foi desenhada para a caixa interna autenticada do canal `Sistema`.

## 27. Canal Email

O frontend nao exibe notificacoes de `Email`, pois consome apenas a API da inbox propria filtrada no backend.

## 28. Compatibilidade com backend

Nao foi necessario alterar o backend funcional da inbox. A integracao consumiu os contratos e rotas ja disponiveis.

## 29. Compatibilidade com Worker.Email

Nao houve alteracao no `Worker.Email`.

## 30. Impacto em abertura

Nenhum impacto funcional direto em abertura de chamados.

## 31. Impacto em atendimento

Nenhum impacto funcional direto em atendimento, alem da nova superficie de consulta das notificacoes internas.

## 32. Impacto em aprovacao

Nenhum impacto funcional direto em aprovacao nesta etapa.

## 33. Impacto em SLA

Nenhum impacto funcional direto em SLA nesta etapa.

## 34. Impacto em fechamento e reabertura

Nenhum impacto funcional direto em fechamento e reabertura nesta etapa.

## 35. Testes

Foram adicionados testes frontend para:

- service de notificacoes
- store de contagem
- integracao estrutural da view, layouts e rotas

Tambem foram executados build de producao, `vue-tsc --noEmit` e testes de roadmap no backend.

## 36. O que nao foi implementado

Nao foram implementados SignalR, WebSocket, push, polling agressivo, notificacoes do navegador, exclusao, arquivamento, agrupamento, categorias personalizadas, preferencias adicionais, administracao da caixa de terceiros ou integracao automatica de eventos ITSM.

## 37. Riscos

O contador global depende de recarga explicita apos acoes locais. Sem atualizacao em tempo real, a caixa pode ficar temporariamente defasada em cenarios de uso simultaneo em varias abas.

## 38. Decisoes adiadas

Ficaram adiadas:

- atualizacao em tempo real
- integracao automatica dos eventos ITSM priorizados
- eventuais testes E2E de navegacao ponta a ponta

## 39. Criterios de aceite

Os criterios desta etapa foram atendidos com central autenticada, contador, filtros, paginacao, detalhe sem mutacao em `GET`, marcacao lida/nao lida, tratamento de erro, responsividade e ausencia de `UsuarioId` nas requests.

## 40. Proxima etapa

`Integrar notificacoes aos eventos ITSM priorizados e executar testes de regressao`
