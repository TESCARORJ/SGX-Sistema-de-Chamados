# Checklist de Homologacao - Relatorios Avancados

> Objetivo: validar o fechamento funcional da Sprint 6 do modulo Relatorios Avancados antes da homologacao institucional.

## Acesso e permissoes

- [ ] Administrador acessa dashboard de relatorios avancados.
- [ ] Usuario com `RelatoriosAvancados.Visualizar` acessa relatorios permitidos.
- [ ] Usuario sem `RelatoriosAvancados.Visualizar` nao acessa relatorios.
- [ ] Relatorio de auditoria exige `RelatoriosAvancados.Auditoria`.
- [ ] Botao de exportacao aparece apenas para usuario com `RelatoriosAvancados.Exportar`.
- [ ] Relatorios operacionais respeitam `RelatoriosAvancados.Operacional`.
- [ ] Relatorios gerenciais respeitam `RelatoriosAvancados.Gerencial`.

## Dashboard

- [ ] Dashboard geral carrega metadados.
- [ ] Dashboard exibe cards de acesso rapido.
- [ ] Dashboard trata loading.
- [ ] Dashboard trata erro.
- [ ] Dashboard respeita permissao de auditoria.

## Relatorios de chamados

- [ ] Resumo de chamados carrega por periodo.
- [ ] Filtro por departamento funciona.
- [ ] Filtro por categoria funciona.
- [ ] Filtro por prioridade funciona.
- [ ] Distribuicao por status aparece corretamente.
- [ ] Serie temporal por dia/mes funciona.
- [ ] Produtividade por atendente carrega com permissao operacional.

## Relatorios de SLA

- [ ] Resumo de SLA carrega.
- [ ] Violacoes de SLA carregam.
- [ ] SLA por departamento carrega.
- [ ] SLA por prioridade carrega.
- [ ] Indicadores nao confiaveis aparecem como nulos/ausentes, sem numero inventado.

## Relatorios de aprovacoes

- [ ] Resumo de aprovacoes carrega.
- [ ] Tempo medio de decisao carrega quando ha dados.
- [ ] Aprovacoes por origem carrega.
- [ ] Filtros por status/tipo de origem funcionam.

## Relatorios de catalogo

- [ ] Resumo de catalogo carrega.
- [ ] Servicos mais solicitados carregam.
- [ ] Catalogo por departamento carrega.

## Relatorios de inventario

- [ ] Resumo de inventario carrega.
- [ ] Ativos por status carregam.
- [ ] Chamados recorrentes por ativo carregam.
- [ ] Inventario por departamento carrega.

## Relatorios de base de conhecimento

- [ ] Resumo da base carrega.
- [ ] Artigos por status carregam.
- [ ] Vinculos com chamados carregam.
- [ ] Sistema nao inventa metrica de leitura/acesso se nao houver dado.

## Relatorios de auditoria

- [ ] Resumo de auditoria carrega apenas com permissao correta.
- [ ] Auditoria por usuario carrega.
- [ ] Auditoria por entidade carrega.
- [ ] Usuario sem permissao de auditoria nao acessa dados sensiveis.

## Exportacao

- [ ] Exportacao CSV simples funciona para dados carregados.
- [ ] Exportacao nao aparece para usuario sem permissao.
- [ ] Exportacao nao tenta fazer processamento massivo.
- [ ] Nome do arquivo exportado e coerente.

## UX

- [ ] Todas as telas principais tratam loading.
- [ ] Todas as telas principais tratam erro.
- [ ] Todas as telas principais tratam vazio.
- [ ] Filtros sao claros.
- [ ] Mensagens sao amigaveis.
- [ ] Tabelas sao legiveis.
- [ ] Responsividade basica validada.

## Validacoes tecnicas de fechamento

- [ ] `dotnet build` em Release executado com sucesso.
- [ ] Testes backend executados com sucesso.
- [ ] `npm run test:unit` executado com sucesso.
- [ ] `npm run build` executado com sucesso.

## E2E

- [ ] Verificado framework E2E no projeto.
- [ ] Caso exista framework, cenarios minimos de dashboard/chamados/SLA/auditoria/exportacao executados.
- [ ] Caso nao exista framework, pendencia E2E registrada na documentacao.
