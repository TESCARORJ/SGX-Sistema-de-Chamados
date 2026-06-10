# Sprint 4 - Conceito de Aprovador Padrao
## 1. Objetivo da definicao
Definir conceitualmente quem deve ser considerado aprovador padrao no futuro motor de aprovacao ITSM reutilizavel do SGX Sistema de Chamados quando uma regra exigir aprovacao formal, mas ainda nao houver responsavel especifico resolvido por servico, grupo, nivel, dono do servico ou delegacao.
## 2. Limites desta etapa
- Esta etapa registra apenas definicao conceitual, documentacao e atualizacao do roadmap/checklist.
- Nao houve implementacao funcional do aprovador padrao.
- Nao foram criadas entidades novas.
- Nao foram criadas migrations estruturais.
- Nao houve alteracao no modelo de dominio.
- Nao houve criacao de campo de aprovador, delegacao ou grupo aprovador.
- Nao houve alteracao em `AprovacaoChamado`, `BloqueiaAvancoAtendimento` ou no fluxo atual de aprovacao.
- Nao houve homologacao nem aceite final.
## 3. Contexto atual de aprovadores no fluxo existente
- O fluxo atual de aprovacao do SGX ja permite registrar uma aprovacao pendente e depois decidir por aprovacao, reprovacao ou cancelamento.
- A decisao ja fica vinculada a um usuario aprovador na instancia de aprovacao.
- O fluxo atual tambem preserva solicitante, usuario criador, usuario de cancelamento e auditoria de atualizacao.
- Apesar disso, o sistema ainda nao possui conceito estruturado de aprovador padrao como fallback de governanca quando nenhuma regra especifica resolver o responsavel.
## 4. Representacao atual de aprovador, se existir
- `AprovacaoChamado` ja possui `AprovadorId`.
- `AprovacaoChamado` tambem possui `SolicitanteId`, `CriadoPorUsuarioId`, `AtualizadoPorUsuarioId` e `CanceladoPorUsuarioId`.
- Aprovacao e decisao ja preservam `Status`, `DecididaEm`, `JustificativaDecisao`, `SolicitadaEm`, `OrigemDescricao` e auditoria.
- Isso mostra que a representacao do decisor atual existe, mas ainda sem uma politica formal de fallback para eleicao do aprovador.
## 5. Lacuna caso aprovador padrao ainda nao exista como conceito estruturado
- O sistema atual nao define quem deve assumir a aprovacao quando nao existir aprovador especifico configurado.
- Tambem nao existe hoje diferenca formal entre:
  - aprovador padrao;
  - aprovador especifico;
  - aprovador por delegacao;
  - grupo aprovador;
  - aprovacao multinivel;
  - dono do servico.
- Essa lacuna precisa ser resolvida conceitualmente para evitar aprovacao obrigatoria sem responsavel definido.
## 6. Conceito de aprovador padrao
Aprovador padrao e o usuario, papel, perfil ou responsavel administrativo configurado para receber decisoes de aprovacao quando o motor identificar necessidade de aprovacao formal e nenhuma regra mais especifica resolver quem deve aprovar.
Ele deve ser tratado como fallback de governanca, e nao como unico aprovador definitivo do sistema.
## 7. Quando usar aprovador padrao
- Quando a regra exigir aprovacao e nao houver aprovador especifico.
- Quando o servico sensivel ainda nao tiver dono configurado.
- Quando tipo ou natureza exigirem aprovacao, mas ainda nao houver grupo aprovador definido.
- Quando custo ou risco exigirem decisao formal, mas nao existir responsavel financeiro, gestor, dono do servico ou aprovador por nivel configurado.
- Quando o sistema precisar preservar compatibilidade com o fluxo atual de aprovacao simples.
- Quando houver necessidade de fallback administrativo para impedir aprovacao sem responsavel.
## 8. Quando nao usar aprovador padrao
- Quando houver grupo aprovador especifico configurado.
- Quando houver dono do servico responsavel pela aprovacao.
- Quando houver aprovador por nivel definido.
- Quando houver delegacao ativa e valida.
- Quando a regra exigir aprovacao tecnica, financeira, seguranca, compliance ou gerencial especifica.
- Quando houver conflito de interesse, como solicitante e aprovador coincidindo indevidamente.
- Quando a regra futura exigir segregacao de funcao.
## 9. Diferenca entre aprovador padrao e aprovador especifico
- Aprovador padrao: fallback geral quando nenhuma regra especifica resolver o responsavel.
- Aprovador especifico: usuario diretamente indicado pela regra, servico, tipo, natureza ou contexto da acao.
## 10. Diferenca entre aprovador padrao e aprovador por delegacao
- Aprovador padrao: fallback estrutural do motor.
- Aprovador por delegacao: pessoa autorizada temporaria ou formalmente a aprovar em nome de outro aprovador.
- Delegacao pressupoe relacao de substituicao; aprovador padrao pressupoe ausencia de regra especifica resolvida.
## 11. Diferenca entre aprovador padrao e grupo aprovador
- Aprovador padrao: resolve um responsavel minimo para nao deixar a aprovacao sem destino.
- Grupo aprovador: conjunto de usuarios, papeis ou responsaveis aptos a decidir, podendo exigir um ou mais votos.
- Grupo aprovador e regra especifica; aprovador padrao e fallback.
## 12. Diferenca entre aprovador padrao e aprovacao multinivel
- Aprovador padrao: um fallback para identificar quem decide quando nenhuma composicao mais rica estiver disponivel.
- Aprovacao multinivel: fluxo com mais de uma decisao em sequencia ou em paralelo, como tecnico, gestor, financeiro e seguranca.
- Multinivel distribui responsabilidade; aprovador padrao evita lacuna de responsabilidade.
## 13. Diferenca entre aprovador padrao e dono do servico
- Aprovador padrao: fallback administrativo ou global.
- Dono do servico: responsavel funcional ou tecnico pelo servico solicitado, podendo ser o aprovador preferencial em servicos sensiveis.
- Se houver dono do servico bem definido, ele tende a prevalecer sobre o aprovador padrao.
## 14. Regra conceitual de fallback
1. O motor identifica que a acao exige aprovacao formal.
2. O motor tenta resolver aprovador especifico pela regra mais forte disponivel.
3. Se nao houver aprovador especifico, tenta resolver dono do servico.
4. Se nao houver dono do servico, tenta resolver grupo aprovador futuro.
5. Se nao houver grupo configurado, tenta resolver delegacao valida, quando aplicavel.
6. Se nenhuma regra especifica resolver o responsavel, usa aprovador padrao.
7. Se nao houver aprovador padrao configurado, o motor deve sinalizar lacuna critica de configuracao e impedir geracao silenciosa de aprovacao sem responsavel.
## 15. Relacao com natureza ITSM
- A natureza pode disparar necessidade de aprovacao, mas nao deve sozinha decidir quem e o aprovador final.
- Quando a natureza exigir aprovacao e nao houver responsavel mais especifico, o aprovador padrao pode ser usado como fallback.
- Em `Mudanca`, o aprovador padrao nao deve reduzir futuras exigencias de governanca mais especificas.
## 16. Relacao com tipo de chamado
- O tipo de chamado pode refinar a exigencia de aprovacao.
- Se o tipo trouxer aprovador especifico no futuro, ele deve prevalecer.
- Sem essa definicao, o aprovador padrao cobre o caso como protecao de governanca.
## 17. Relacao com servico sensivel
- Servico sensivel tende a ser o eixo mais forte para futura resolucao de aprovador especifico.
- Quando o servico ainda nao tiver dono ou grupo aprovador, o aprovador padrao impede que a aprovacao fique sem responsavel.
- O aprovador padrao nao substitui a futura governanca por dono do servico.
## 18. Relacao com impacto e urgencia
- Impacto e urgencia podem reforcar a necessidade de aprovacao, mas normalmente nao determinam sozinhos o aprovador.
- Em cenarios criticos sem responsavel especifico resolvido, o fallback pode usar aprovador padrao ate que as regras avancadas sejam definidas.
## 19. Relacao com custo e risco
- Custo e risco reforcam a necessidade de autoridade formal.
- Quando ainda nao houver responsavel financeiro, gestor de risco ou aprovador especializado definido, o aprovador padrao pode atuar como fallback.
- Isso nao elimina a necessidade futura de segregacao e de trilhas especificas por custo, risco, seguranca e compliance.
## 20. Relacao com AprovacaoChamado
- `AprovacaoChamado` continua sendo a instancia persistente da aprovacao.
- No futuro, o aprovador padrao deve ser associado a essa instancia ou a uma futura etapa de aprovacao.
- Devem ser preservados `ChamadoId`, `Status`, `TipoOrigem`, `AprovadorId`, decisao, historico e auditoria.
## 21. Relacao com historico e auditoria
- Toda decisao tomada por aprovador padrao deve registrar:
  - quem decidiu;
  - quando decidiu;
  - se atuou como aprovador padrao;
  - qual regra levou ao fallback;
  - se havia ausencia de regra especifica;
  - justificativa da decisao, quando aplicavel.
- Essa rastreabilidade e essencial para auditoria futura e para evitar uso indevido do fallback como regra permanente.
## 22. Riscos de seguranca e governanca
- Excesso de aprovacoes concentradas em uma unica pessoa.
- Aprovacao por pessoa sem competencia tecnica ou autoridade financeira adequada.
- Conflito de interesse.
- Aprovacao sem segregacao de funcao.
- Falta de rastreabilidade sobre o motivo do fallback.
- Uso indevido do aprovador padrao como substituto permanente de regras especificas.
## 23. Compatibilidade com fluxo atual
- O conceito preserva o modulo atual de aprovacao.
- Nao exige mudanca imediata em como as aprovacoes sao hoje solicitadas, aprovadas, reprovadas ou canceladas.
- O aprovador padrao entra nesta etapa apenas como definicao futura de governanca para organizar o motor reutilizavel.
## 24. Lacunas encontradas
- Nao existe conceito estruturado de aprovador padrao no estado atual.
- Nao existe regra formal de hierarquia entre aprovador especifico, dono do servico, grupo, delegacao e fallback.
- Nao existe validacao formal de conflito de interesse no fluxo atual de aprovacao simples.
- Nao existe politica estruturada de segregacao de funcao para o futuro motor.
## 25. Decisoes adiadas para proximos itens
- Como configurar o aprovador padrao.
- Se ele sera usuario, perfil, papel, grupo ou parametro global.
- Se havera aprovador padrao por tenant, unidade, categoria, servico ou area.
- Como tratar ausencia de aprovador padrao configurado.
- Como validar conflito de interesse.
- Como implementar delegacao.
- Como implementar grupo aprovador.
- Como implementar aprovacao multinivel.
- Como exibir aprovador padrao na interface.
- Como migrar aprovacoes existentes para usar essa nova logica.
## 26. Conclusao tecnica
O aprovador padrao deve ser definido como fallback de governanca do futuro motor de aprovacao ITSM. Sua finalidade nao e substituir regras especificas, mas impedir que uma aprovacao obrigatoria fique sem responsavel. O conceito preserva compatibilidade com o fluxo atual e cria base para evolucao posterior com dono do servico, grupo aprovador, delegacao e multiplos niveis.
## 27. Proxima etapa recomendada
Executar o item 12 do checklist da Sprint 4: definir conceito de grupo aprovador.
