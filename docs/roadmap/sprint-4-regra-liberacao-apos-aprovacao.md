# Sprint 4 - Regra de Liberacao Apos Aprovacao
## 1. Objetivo da definicao
Definir conceitualmente quando e como uma aprovacao concedida deve liberar acoes do chamado, separando liberacao total, parcial, por escopo, por acao, por validade e por satisfacao de niveis ou ramos obrigatorios.
## 2. Limites desta etapa
- Esta etapa registra apenas definicao conceitual, documentacao e atualizacao do roadmap/checklist.
- Nao houve implementacao funcional da regra de liberacao apos aprovacao.
- Nao foram criadas entidades novas.
- Nao foram criadas migrations estruturais.
- Nao houve alteracao no modelo de dominio.
- Nao houve alteracao em `AprovacaoChamado`, `BloqueiaAvancoAtendimento`, `StatusAprovacaoChamado` ou no fluxo atual de atendimento.
- Nao houve homologacao nem aceite final.
## 3. Contexto atual da aprovacao concedida
- O SGX ja possui aprovacao simples com decisao `Aprovado`, `Reprovado`, `Pendente` e `Cancelado`.
- O comportamento atual trata aprovacao concedida como remocao do bloqueio simples ligado a aprovacao pendente bloqueante.
- O item 16 da sprint ja definiu que pendencia nao e sinônimo de bloqueio total; por consequencia, aprovacao concedida tambem nao deve ser tratada como liberacao irrestrita automatica.
- O motor futuro precisa separar a decisao formal de aprovacao do efeito operacional que essa decisao realmente libera.
## 4. Comportamento atual apos aprovacao
- `AprovacaoChamadoHelper` deixa `AprovacaoPendente = false` quando a ultima decisao e `Aprovado`.
- O helper devolve mensagem de orientacao de chamado aprovado e `BloqueiaAvancoAtendimento = false`.
- Os testes atuais mostram que, apos aprovacao, o chamado volta a permitir avanco operacional dentro da logica simples hoje existente, como alteracao para status intermediario e encerramento.
- O fluxo atual ainda nao diferencia se a aprovacao libera o chamado inteiro, uma acao especifica ou apenas parte do escopo.
## 5. Conceito de liberacao apos aprovacao
Liberacao apos aprovacao e o efeito operacional aplicado pelo motor quando uma decisao formal aprovada satisfaz a regra de aprovacao exigida para determinada acao, escopo ou etapa do chamado.
Ela nao deve ser confundida com autorizacao irrestrita de tudo o que existe no chamado.
## 6. Diferenca entre aprovacao concedida e liberacao operacional
- Aprovacao concedida: decisao formal positiva registrada por aprovador, grupo ou nivel autorizado.
- Liberacao operacional: permissao para executar uma acao do chamado que antes estava bloqueada por exigencia de aprovacao.
- Uma aprovacao pode estar concedida e mesmo assim liberar apenas parte do fluxo, apenas uma acao, apenas um servico ou apenas uma etapa especifica.
## 7. Liberacao total
- Liberacao total deve ser usada somente quando a regra de aprovacao for simples, unica e cobrir todo o escopo operacional exigido.
- Exemplo conceitual: aprovacao unica que cobre integralmente a execucao exigida, sem outros niveis, ramos ou regras obrigatorias pendentes.
## 8. Liberacao parcial
- Liberacao parcial deve ser usada quando a aprovacao cobre apenas parte do chamado ou parte da acao sensivel.
- Exemplo conceitual: aprovacao que libera apenas execucao tecnica, mas nao encerramento, custo adicional ou mudanca de escopo.
## 9. Liberacao por escopo
- A aprovacao deve liberar somente o escopo aprovado.
- O escopo pode ser, por exemplo:
  - servico;
  - custo;
  - risco;
  - acesso;
  - mudanca;
  - ambiente;
  - categoria operacional;
  - execucao especifica.
## 10. Liberacao por acao
- A aprovacao pode liberar apenas acoes especificas.
- Exemplos conceituais:
  - assumir;
  - executar;
  - alterar status;
  - encerrar;
  - liberar acesso;
  - aplicar mudanca;
  - prosseguir com custo aprovado.
## 11. Liberacao por validade da decisao
- A aprovacao deve liberar enquanto os dados que motivaram a decisao permanecerem validos.
- A liberacao nao deve persistir automaticamente se o contexto sensivel mudar depois da aprovacao.
- O motor futuro deve considerar validade logica da decisao, nao apenas o fato historico de ela ter existido.
## 12. Quando a aprovacao libera avanco operacional
- Liberar avanco operacional quando:
  - a aprovacao obrigatoria foi concedida;
  - a aprovacao pertence ao mesmo escopo da acao;
  - a decisao esta valida;
  - nao ha reprovacao ativa relacionada;
  - nao ha nivel obrigatorio pendente;
  - nao ha ramo obrigatorio pendente;
  - nao houve alteracao sensivel que exija reavaliacao;
  - outras regras obrigatorias tambem estao satisfeitas.
## 13. Quando a aprovacao libera apenas acoes especificas
- Liberar apenas acoes especificas quando:
  - a aprovacao foi concedida para um servico especifico;
  - a aprovacao cobre apenas custo, mas nao risco;
  - a aprovacao cobre acesso, mas nao mudanca;
  - a aprovacao cobre execucao tecnica, mas nao encerramento;
  - a aprovacao cobre um ramo ou nivel, mas nao o fluxo completo;
  - a aprovacao tem escopo limitado pela regra.
## 14. Quando a aprovacao nao deve liberar tudo
- Nao liberar tudo quando:
  - a aprovacao for de nivel intermediario;
  - apenas um ramo paralelo foi aprovado;
  - a aprovacao cobrir somente um servico;
  - a aprovacao cobrir somente custo, mas nao risco;
  - a aprovacao cobrir somente parecer tecnico;
  - houver outra regra obrigatoria pendente;
  - os dados sensiveis tiverem mudado apos a aprovacao;
  - a decisao estiver expirada, cancelada ou invalidada em outro escopo;
  - a aprovacao tiver sido concedida por fallback em cenario que exige validacao adicional.
## 15. Regra conceitual para aprovacao simples
- Em aprovacao simples, a liberacao pode ocorrer quando a aprovacao concedida satisfizer integralmente a regra exigida.
- Se a aprovacao simples tiver escopo limitado, a liberacao tambem deve ser limitada ao mesmo escopo.
- Aprovacao simples nao deve ser automaticamente traduzida como liberacao total se a propria regra for restrita.
## 16. Regra conceitual para aprovacao sequencial
- Em aprovacao sequencial, a aprovacao de um nivel intermediario deve liberar apenas o inicio do proximo nivel.
- A liberacao operacional final so deve ocorrer apos aprovacao de todos os niveis obrigatorios.
- A aprovacao do ultimo nivel nao dispensa outras regras independentes eventualmente coexistentes.
## 17. Regra conceitual para aprovacao paralela
- Em aprovacao paralela, a aprovacao de um ramo libera apenas aquele ramo.
- A liberacao operacional final so deve ocorrer apos a consolidacao positiva de todos os ramos obrigatorios.
- Ramos aprovados isoladamente podem remover parte de uma restricao de escopo, mas nao a aprovacao inteira se ainda restarem ramos obrigatorios.
## 18. Regra conceitual para aprovacao multinivel
- Em aprovacao multinivel, a liberacao depende da satisfacao de todos os niveis ou ramos obrigatorios, conforme a regra de sequencia, paralelismo ou fluxo misto.
- O motor deve distinguir liberacao de etapa, liberacao parcial de escopo e liberacao operacional final.
## 19. Reavaliacao apos alteracao de dados sensiveis
- A aprovacao concedida deve ser reavaliada quando, apos a decisao, houver alteracao relevante em:
  - natureza ITSM;
  - tipo de chamado;
  - servico solicitado;
  - impacto;
  - urgencia;
  - custo;
  - risco;
  - ambiente;
  - acesso;
  - escopo da execucao;
  - dados que fundamentaram a decisao.
- Exemplos conceituais:
  - servico comum alterado para servico sensivel;
  - custo inicialmente baixo que aumenta;
  - risco inicialmente controlado que se torna alto;
  - execucao antes prevista para homologacao que muda para producao;
  - parecer tecnico aprovado antes de alteracao relevante do escopo tecnico.
## 20. Relacao com natureza ITSM
- A liberacao deve respeitar o escopo da natureza que gerou a aprovacao.
- Uma `Mudanca` aprovada deve liberar a mudanca aprovada, nao outras acoes sensiveis fora daquele escopo.
- Mudanca de natureza apos aprovacao pode invalidar a liberacao anterior e exigir reavaliacao.
## 21. Relacao com tipo de chamado
- Se o tipo de chamado gerou a exigencia, a liberacao deve se limitar ao tipo e a acao aprovada.
- Mudanca de tipo apos aprovacao pode exigir reavaliacao.
- O tipo aprovado nao deve ser usado como permissao implicita para outro tipo de execucao sensivel.
## 22. Relacao com servico sensivel
- Servico sensivel aprovado libera somente a execucao daquele servico ou das acoes explicitamente cobertas.
- Alteracao para outro servico sensivel exige nova avaliacao.
- Aprovacao de um servico nao deve ser herdada automaticamente por outro servico apenas por pertencer ao mesmo chamado.
## 23. Relacao com impacto e urgencia
- A aprovacao baseada em impacto e urgencia so permanece valida enquanto a combinacao avaliada continuar compatível com a decisao.
- Aumento relevante de impacto ou urgencia pode exigir reavaliacao.
- O motor deve evitar que uma aprovacao concedida para contexto brando seja reutilizada em contexto agravado.
## 24. Relacao com custo e risco
- A aprovacao por custo ou risco deve ser estritamente limitada ao custo ou risco aprovado.
- Aumento de custo, mudanca de risco ou ampliacao de escopo deve exigir reavaliacao.
- Aprovacao financeira nao deve liberar risco tecnico nao avaliado, e aprovacao tecnica nao deve liberar compromisso financeiro alem do que foi aprovado.
## 25. Relacao com `AprovacaoChamado`
- `AprovacaoChamado` continua sendo a fonte atual da decisao de aprovacao.
- O motor futuro deve avaliar `Status`, origem, escopo, decisao e validade para concluir se a aprovacao concedida libera ou nao a acao solicitada.
- Nesta etapa, `AprovacaoChamado` nao foi alterado.
## 26. Relacao com `BloqueiaAvancoAtendimento`
- Quando a aprovacao concedida satisfizer a regra de bloqueio simples atual, o bloqueio representado por `BloqueiaAvancoAtendimento` pode ser considerado resolvido para aquele escopo simples.
- O motor futuro, porem, deve validar tambem acao, escopo, niveis, ramos e validade da decisao.
- Ou seja, resolver o booleano atual nao deve significar liberar irrestritamente tudo.
## 27. Relacao com `AguardandoAprovacao`
- Se o chamado estiver em `AguardandoAprovacao`, a aprovacao concedida pode permitir saida desse estado.
- Isso so deve ocorrer quando todas as regras obrigatorias estiverem satisfeitas.
- Sair de `AguardandoAprovacao` nao deve significar liberacao irrestrita se houver outras aprovacoes pendentes ou se a aprovacao concedida tiver escopo parcial.
## 28. Relacao com historico e auditoria
- Toda liberacao apos aprovacao deve ser rastreavel.
- O motor futuro deve registrar:
  - qual aprovacao liberou;
  - qual regra foi satisfeita;
  - qual escopo foi liberado;
  - quais acoes foram liberadas;
  - quem aprovou;
  - quando aprovou;
  - se houve niveis ou ramos envolvidos;
  - se houve fallback;
  - se houve reavaliacao;
  - se a liberacao foi total, parcial, por escopo ou por acao.
## 29. Compatibilidade com fluxo atual
- O conceito preserva o modulo atual de aprovacao.
- Nao exige mudanca imediata em `AprovacaoChamado`, `BloqueiaAvancoAtendimento`, status ou fluxo atual.
- Esta etapa apenas define a regra conceitual para orientar implementacao futura.
## 30. Lacunas encontradas
- O fluxo atual nao diferencia claramente liberacao total, parcial, por escopo e por acao.
- `BloqueiaAvancoAtendimento` resolve bloqueio simples, mas nao expressa escopo liberado.
- Nao ha estrutura para validade da aprovacao por escopo.
- Nao ha estrutura para reavaliacao automatica apos mudanca sensivel.
- Fluxos sequencial, paralelo e multinivel ainda nao existem estruturalmente.
- Falta auditoria especifica da acao liberada por aprovacao.
- Falta regra clara para expiracao ou cancelamento apos aprovacao.
## 31. Riscos de seguranca e governanca
- Aprovacao de um ponto liberar indevidamente todo o chamado.
- Aprovacao intermediaria liberar execucao final.
- Aprovacao de um ramo paralelo liberar consolidacao inteira.
- Aprovacao de custo liberar risco nao avaliado.
- Aprovacao de servico liberar outro servico.
- Mudanca de dados sensiveis apos aprovacao sem reavaliacao.
- Falta de rastreabilidade sobre o que foi liberado.
- Uso indevido de fallback como liberacao ampla.
- Encerramento do chamado com aprovacoes parciais ainda insuficientes.
## 32. Decisoes adiadas para proximos itens
- Como implementar liberacao por escopo.
- Como armazenar escopo aprovado.
- Como armazenar acoes liberadas.
- Como validar validade da decisao.
- Como registrar reavaliacao apos alteracao sensivel.
- Como refletir liberacao parcial na interface.
- Como tratar multiplas aprovacoes aprovadas e conflitantes.
- Como tratar expiracao apos aprovacao.
- Como tratar cancelamento de aprovacao ja concedida.
- Como testar regressao do fluxo atual.
- Como migrar aprovacoes existentes.
## 33. Conclusao tecnica
Liberacao apos aprovacao deve ser definida como o efeito operacional aplicado pelo motor quando uma decisao formal positiva satisfaz a regra exigida para determinada acao, escopo ou etapa do chamado. O conceito precisa separar aprovacao concedida, liberacao operacional, escopo liberado, acoes liberadas, validade da decisao e necessidade de reavaliacao, evitando que uma aprovacao pontual seja interpretada como permissao irrestrita.
## 34. Proxima etapa recomendada
Executar o item 18 do checklist da Sprint 4: definir regra de rejeicao e encerramento ou retorno do chamado.
