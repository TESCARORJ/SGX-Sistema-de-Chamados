# Checklist de Homologacao - Aprovacao de Chamados

Objetivo:
Validar funcionalmente o modulo Aprovacao de Chamados no SGX antes da homologacao institucional final.

Legenda sugerida:
- [ ] Nao executado
- [x] Executado e aprovado
- [!] Executado com ressalva

## Fluxo administrativo
- [ ] Administrador solicita aprovacao manual para um chamado.
- [ ] Sistema impede aprovacao pendente duplicada para o mesmo chamado.
- [ ] Aprovador aprova chamado pendente.
- [ ] Aprovador reprova chamado pendente com justificativa.
- [ ] Sistema bloqueia reprovacao sem justificativa.
- [ ] Aprovador cancela aprovacao pendente com justificativa.
- [ ] Sistema bloqueia cancelamento sem justificativa.
- [ ] Sistema impede aprovar aprovacao ja aprovada.
- [ ] Sistema impede reprovar aprovacao ja aprovada.
- [ ] Sistema impede cancelar aprovacao ja decidida.
- [ ] Historico do chamado registra aprovacao solicitada.
- [ ] Historico do chamado registra chamado aprovado.
- [ ] Historico do chamado registra chamado reprovado.
- [ ] Historico do chamado registra aprovacao cancelada.
- [ ] Auditoria registra solicitacao, aprovacao, reprovacao e cancelamento.

## Fluxo por catalogo
- [ ] Servico do catalogo com RequerAprovacao = true cria aprovacao pendente na abertura do chamado.
- [ ] Servico do catalogo com RequerAprovacao = false nao cria aprovacao.
- [ ] Chamado aberto sem catalogo continua funcionando normalmente.
- [ ] Aprovacao automatica usa TipoOrigem = CatalogoServico.
- [ ] OrigemDescricao registra nome do servico.
- [ ] Historico registra aprovacao solicitada automaticamente.

## Bloqueios operacionais
- [ ] Chamado pendente de aprovacao nao pode ser assumido.
- [ ] Chamado pendente de aprovacao nao pode avancar status indevidamente.
- [ ] Chamado pendente de aprovacao nao pode ser encerrado.
- [ ] Chamado reprovado permanece bloqueado para avanco.
- [ ] Chamado aprovado fica liberado para atendimento.
- [ ] Aprovacao cancelada remove bloqueio se nao houver outra pendencia ativa.
- [ ] Consulta, comentarios e visualizacao permanecem permitidos quando aplicavel.

## Frontend administrativo
- [ ] Tela de listagem de aprovacoes carrega corretamente.
- [ ] Filtros de aprovacao funcionam.
- [ ] Tela de detalhe de aprovacao carrega corretamente.
- [ ] Botoes de aprovar/reprovar/cancelar aparecem apenas para aprovacao pendente.
- [ ] Reprovacao exige justificativa.
- [ ] Cancelamento exige justificativa.
- [ ] Detalhe administrativo do chamado exibe secao Aprovacao.
- [ ] Detalhe administrativo do chamado permite solicitacao manual, se usuario tiver permissao.
- [ ] Mensagens de erro sao amigaveis.

## Portal do solicitante
- [ ] Listagem do portal indica chamado aguardando aprovacao.
- [ ] Detalhe do chamado no portal exibe secao Aprovacao.
- [ ] Portal mostra mensagem de aguardando aprovacao.
- [ ] Portal mostra mensagem de aprovado.
- [ ] Portal mostra mensagem de reprovado com justificativa, quando aplicavel.
- [ ] Portal mostra aprovacao cancelada de forma coerente.
- [ ] Portal nao expoe dados administrativos sensiveis, como aprovador ou regras internas.
- [ ] Solicitante nao acessa aprovacao de chamado de outro usuario.

## Permissoes
- [ ] AprovacaoChamados.Visualizar controla listagem/detalhe administrativo.
- [ ] AprovacaoChamados.Gerenciar controla solicitacao manual.
- [ ] AprovacaoChamados.Aprovar controla aprovacao.
- [ ] AprovacaoChamados.Reprovar controla reprovacao.
- [ ] AprovacaoChamados.Cancelar controla cancelamento.
- [ ] Usuario sem permissao nao executa acoes restritas.

## Observacoes de Sprint 6
- Testes E2E completos permanecem como pendencia evolutiva (framework E2E nao instalado nesta sprint).
- Evidencias formais com prints reais devem ser registradas em `docs/evidencias/aprovacao-chamados/` durante homologacao institucional.
