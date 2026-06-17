# Sprint 5 - Configuracao administrativa do prazo de auto-fechamento

## Objetivo
Concluir o Item 13 com a menor mudanca segura para governar o prazo usado pela politica de fechamento automatico de chamados resolvidos sem manifestacao do solicitante.

## Decisao de arquitetura
- O projeto ja possui a entidade generica `ParametroSistema` para configuracoes administrativas persistidas.
- O prazo passou a ser governado pela chave `chamados.fechamento_automatico.prazo_aceite_horas`.
- Nao foi criada tabela nova, campo novo no chamado, scheduler nem configuracao frontend.
- O valor padrao inicial foi seedado em `72` horas.

## O que foi entregue
1. DTOs e validator dedicados para obter e atualizar a configuracao administrativa.
2. Use cases administrativos para leitura e atualizacao do prazo com validacao de faixa segura.
3. Endpoint administrativo protegido:
   - `GET /api/admin/chamados/configuracoes/auto-fechamento`
   - `PUT /api/admin/chamados/configuracoes/auto-fechamento`
4. Auditoria da alteracao com antes/depois, usuario e observacao opcional.
5. Integracao da politica do Item 12 para usar o parametro governado quando o request nao informa `PrazoAceiteHoras`.
6. Preservacao do request explicito para testes deterministas e execucoes tecnicas.

## Regras aplicadas
- Prazo obrigatoriamente entre `1` e `720` horas.
- Prazo zero, negativo ou acima do limite e rejeitado.
- Alterar a configuracao nao fecha chamados, nao altera status e nao recalcula SLA.
- A nova configuracao so afeta execucoes futuras da politica de auto-fechamento.
- O fluxo de aprovacao pendente bloqueante da Sprint 4 continua sendo respeitado pela politica de fechamento.

## Rastreabilidade
- Persistencia governada em `ParametroSistema`.
- Seed inicial do parametro para evitar dependencia de valor hardcoded solto.
- Checklist da Sprint 5 atualizado para `13/32` itens concluidos (`41%`).
- Proxima acao da sprint atualizada para `Criar regra de reabertura controlada por prazo/politica.`

## Fora de escopo
- Scheduler definitivo ou job recorrente.
- Tela/frontend administrativo.
- Politica de reabertura do Item 14.
- Alteracoes de SLA operacional.
