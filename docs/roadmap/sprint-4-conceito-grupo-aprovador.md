# Sprint 4 - Conceito de Grupo Aprovador
## 1. Objetivo da definicao
Definir conceitualmente o que sera um grupo aprovador no futuro motor de aprovacao ITSM reutilizavel do SGX Sistema de Chamados, separando composicao do grupo, autoridade para decidir, quorum minimo, modalidades de decisao e relacao com aprovador padrao e aprovacao multinivel.
## 2. Limites desta etapa
- Esta etapa registra apenas definicao conceitual, documentacao e atualizacao do roadmap/checklist.
- Nao houve implementacao funcional de grupo aprovador.
- Nao foram criadas entidades novas.
- Nao foram criadas migrations estruturais.
- Nao houve alteracao no modelo de dominio.
- Nao foi criada tabela de grupo aprovador nem relacao entre grupo e usuario.
- Nao houve alteracao em `AprovacaoChamado`, `BloqueiaAvancoAtendimento` ou no fluxo atual de aprovacao.
- Nao houve homologacao nem aceite final.
## 3. Contexto atual de aprovacao individual no sistema
- O fluxo atual de aprovacao do SGX trabalha com aprovacao pendente associada a um chamado.
- A decisao atual e individual, registrada sobre uma instancia de `AprovacaoChamado`.
- O projeto ja possui conceitos de usuario, perfil, grupo tecnico, fila e area responsavel em outros contextos do sistema.
- Ainda assim, esses conceitos nao formam hoje uma estrutura formal de grupo aprovador com autoridade coletiva para decidir aprovacoes.
## 4. Representacao atual de grupo aprovador, se existir
- Nao foi identificado conceito estruturado de grupo aprovador no fluxo atual de `AprovacaoChamado`.
- `AprovacaoChamado` trabalha hoje com `AprovadorId` individual.
- Existem estruturas de apoio no sistema, como perfis, grupos tecnicos, departamentos e responsaveis, mas elas nao foram modeladas como autoridade coletiva de aprovacao neste fluxo.
## 5. Lacuna caso grupo aprovador ainda nao exista como conceito estruturado
- O sistema atual nao define como uma aprovacao deve ser decidida por area, competencia coletiva ou conjunto de responsaveis.
- Tambem nao define quorum, votos, empate, conflito de interesse ou precedencia entre grupo e fallback individual.
- Essa lacuna precisa ser resolvida conceitualmente para evitar que aprovacoes mais complexas dependam sempre de uma unica pessoa ou do aprovador padrao.
## 6. Conceito de grupo aprovador
Grupo aprovador e a estrutura conceitual que reune pessoas, papeis, perfis ou responsaveis autorizados a decidir uma aprovacao quando a regra exigir decisao coletiva, decisao por area responsavel, decisao por competencia tecnica ou decisao por responsavel funcional.
Ele nao deve ser tratado como simples lista de notificacao. O grupo aprovador precisa representar autoridade real de decisao.
## 7. Quando usar grupo aprovador
- Quando um servico sensivel tiver uma area responsavel.
- Quando a aprovacao exigir competencia tecnica de uma equipe.
- Quando a decisao pertencer a financeiro, seguranca, compliance, infraestrutura, sistemas ou gestao.
- Quando custo ou risco exigirem avaliacao por mais de uma pessoa ou papel.
- Quando o dono do servico for uma equipe, e nao uma pessoa unica.
- Quando houver necessidade de reduzir dependencia do aprovador padrao.
- Quando a regra permitir que qualquer membro autorizado decida.
- Quando a regra exigir quorum minimo.
## 8. Quando nao usar grupo aprovador
- Quando houver aprovador especifico unico definido pela regra.
- Quando a decisao exigir autoridade individual nominal.
- Quando houver delegacao valida de um aprovador especifico.
- Quando a aprovacao exigir sequencia de niveis diferentes.
- Quando a regra exigir segregacao explicita entre etapas.
- Quando o grupo nao tiver autoridade formal sobre servico, custo, risco ou mudanca.
- Quando todos os membros estiverem em conflito de interesse.
## 9. Diferenca entre grupo aprovador e aprovador padrao
- Grupo aprovador: conjunto de usuarios, papeis ou perfis com autoridade para decidir uma aprovacao.
- Aprovador padrao: fallback geral quando nenhuma regra especifica resolver o responsavel.
- Grupo aprovador tem precedencia sobre aprovador padrao quando houver configuracao valida.
## 10. Diferenca entre grupo aprovador e aprovador especifico
- Grupo aprovador: decisao coletiva ou por conjunto elegivel de responsaveis.
- Aprovador especifico: usuario individual definido diretamente pela regra ou contexto.
- Grupo resolve aprovacao por autoridade compartilhada; aprovador especifico resolve aprovacao por autoridade nominal.
## 11. Diferenca entre grupo aprovador e dono do servico
- Grupo aprovador: estrutura coletiva de decisao.
- Dono do servico: responsavel funcional ou tecnico pelo servico, que pode ser pessoa ou area.
- Quando o dono do servico for uma area, essa area pode futuramente ser representada por grupo aprovador.
## 12. Diferenca entre grupo aprovador e delegacao
- Grupo aprovador: conjunto estavel ou configurado de elegiveis para decidir.
- Delegacao: autorizacao temporaria ou formal para outra pessoa decidir em nome de um aprovador.
- Delegacao substitui ou amplia uma autoridade existente; grupo define um conjunto originario de autoridade.
## 13. Diferenca entre grupo aprovador e aprovacao multinivel
- Grupo aprovador: define quem pode decidir dentro de um mesmo nivel de aprovacao.
- Aprovacao multinivel: define duas ou mais decisoes em sequencia ou em paralelo.
- Um grupo aprovador pode participar de um nivel, mas nao e o mesmo conceito que multinivel.
## 14. Composicao conceitual do grupo
O grupo aprovador pode ser composto por:
- usuarios individuais;
- papeis;
- perfis;
- responsaveis por area;
- responsaveis por servico;
- responsaveis tecnicos;
- responsaveis financeiros;
- responsaveis de seguranca;
- responsaveis de compliance;
- gestores;
- donos de processo.
## 15. Autoridade de decisao do grupo
- O grupo so deve aprovar quando tiver autoridade formal sobre o assunto.
- Exemplos conceituais:
  - grupo financeiro aprova custo;
  - grupo de seguranca aprova acesso privilegiado ou dados sensiveis;
  - grupo de infraestrutura aprova mudanca em ambiente produtivo;
  - grupo dono do servico aprova alteracao em servico critico;
  - grupo de compliance aprova excecao regulatoria ou auditoria obrigatoria.
## 16. Quorum minimo conceitual
- O quorum define quantas decisoes sao necessarias para concluir a aprovacao.
- Possibilidades futuras:
  - um membro decide;
  - maioria simples decide;
  - unanimidade decide;
  - um papel obrigatorio decide;
  - dono do servico precisa decidir;
  - pelo menos um gestor precisa decidir;
  - aprovacao de combinacao de papeis.
## 17. Modalidades conceituais de decisao
1. Qualquer membro aprova:
   A primeira decisao valida de qualquer membro autorizado conclui a aprovacao.
2. Maioria simples:
   A aprovacao depende de mais votos favoraveis do que contrarios.
3. Unanimidade:
   Todos os membros exigidos precisam aprovar.
4. Papel obrigatorio:
   Pelo menos um membro com papel especifico precisa aprovar.
5. Dono do servico obrigatorio:
   Um membro identificado como dono do servico precisa aprovar.
6. Reprovacao impeditiva:
   Uma reprovacao de membro com autoridade suficiente pode encerrar negativamente a aprovacao.
## 18. Tratamento conceitual de aprovacao
- A aprovacao em grupo deve respeitar a modalidade definida pela regra.
- O motor deve registrar quem decidiu, em nome de qual grupo e sob qual criterio de quorum.
- A aprovacao so deve ser concluida quando o quorum aplicavel for atingido.
## 19. Tratamento conceitual de reprovacao
- A reprovacao pode:
  - encerrar imediatamente a aprovacao;
  - depender de quorum contrario;
  - exigir justificativa obrigatoria;
  - permitir reabertura ou nova solicitacao futura;
  - bloquear acoes sensiveis relacionadas.
- A regra detalhada fica para etapas futuras, mas a governanca deve prever reprovação como evento formal e rastreavel.
## 20. Tratamento conceitual de empate ou ausencia de decisao
- Empate, ausencia de quorum ou expiracao devem ser tratados como lacunas de decisao.
- A regra futura podera decidir se:
  - escala para aprovador padrao;
  - escala para gestor;
  - expira a aprovacao;
  - bloqueia a acao;
  - mantem pendente;
  - exige nova solicitacao.
## 21. Tratamento conceitual de conflito de interesse
- Solicitante, executor direto ou beneficiario da aprovacao pode estar impedido de decidir quando a regra exigir segregacao de funcao.
- O grupo aprovador nao deve ser considerado valido se todos os membros elegiveis estiverem em conflito de interesse.
- A governanca futura deve prever exclusao de membros impedidos e rastreabilidade da restricao aplicada.
## 22. Relacao com aprovador padrao
- O grupo aprovador deve ter precedencia sobre o aprovador padrao quando a regra especifica resolver um grupo competente.
- O aprovador padrao so deve atuar se:
  - nao houver grupo configurado;
  - o grupo estiver invalido;
  - o grupo nao tiver membros ativos;
  - o grupo nao tiver autoridade suficiente;
  - a regra futura permitir fallback.
## 23. Relacao com AprovacaoChamado
- `AprovacaoChamado` continua sendo a instancia persistente atual.
- No futuro, o grupo aprovador podera estar associado a aprovacao ou a uma etapa de aprovacao.
- A decisao final do grupo devera preservar `ChamadoId`, `Status`, `TipoOrigem`, decisao, justificativa, historico e auditoria.
- O conceito atual nao exige alteracao imediata em `AprovacaoChamado`.
## 24. Relacao com historico e auditoria
- Toda decisao em grupo deve registrar:
  - grupo acionado;
  - membros elegiveis no momento da solicitacao;
  - quem decidiu;
  - quando decidiu;
  - papel ou perfil usado na decisao;
  - quorum exigido;
  - quorum atingido;
  - votos ou decisoes registradas, se aplicavel;
  - justificativa de aprovacao ou reprovacao;
  - motivo de fallback, se houver.
## 25. Riscos de seguranca e governanca
- Grupo sem autoridade real.
- Grupo sem membros ativos.
- Grupo com membros em conflito de interesse.
- Aprovacao por maioria sem papel obrigatorio quando o caso exigir competencia especifica.
- Ausencia de quorum.
- Falta de rastreabilidade dos membros elegiveis no momento da decisao.
- Uso do grupo como lista de notificacao, sem responsabilidade formal.
- Dependencia excessiva do fallback para aprovador padrao.
## 26. Compatibilidade com fluxo atual
- O conceito preserva o modulo atual de aprovacao.
- Nao exige mudanca imediata em `AprovacaoChamado`.
- O fluxo atual de solicitacao, aprovacao, reprovacao e cancelamento permanece inalterado nesta etapa.
- Esta definicao apenas organiza a modelagem futura do motor reutilizavel.
## 27. Lacunas encontradas
- Nao existe hoje grupo aprovador estruturado no fluxo de aprovacao.
- Nao existe composicao formal de grupo por usuarios, papeis, perfis ou areas para aprovacao.
- Nao existe modelo atual de votos, quorum, empate ou escalonamento.
- Nao existe politica atual de conflito de interesse no contexto de aprovacao coletiva.
## 28. Decisoes adiadas para proximos itens
- Como modelar grupo aprovador.
- Se o grupo sera composto por usuarios, perfis, papeis ou areas.
- Como associar grupo a servico, natureza, tipo, custo, risco ou impacto.
- Como armazenar membros elegiveis no momento da solicitacao.
- Como registrar votos individuais.
- Como calcular quorum.
- Como tratar expiracao.
- Como escalar decisao sem quorum.
- Como aplicar delegacao dentro do grupo.
- Como tratar conflito de interesse.
- Como exibir grupo aprovador na interface.
- Como integrar grupo aprovador com aprovacao multinivel.
- Como migrar aprovacoes atuais para o novo modelo.
## 29. Conclusao tecnica
Grupo aprovador deve ser definido como estrutura coletiva de autoridade para decisao de aprovacao, e nao como simples lista de notificacao. O conceito distribui responsabilidade, reduz dependencia do aprovador padrao e prepara o motor para cenarios de area, competencia tecnica, quorum e futura aprovacao multinivel, preservando compatibilidade com o fluxo atual.
## 30. Proxima etapa recomendada
Executar o item 13 do checklist da Sprint 4: definir conceito de aprovacao multinivel.
