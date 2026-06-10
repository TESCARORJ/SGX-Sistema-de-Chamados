# Sprint 4 - Conceito de Aprovacao Multinivel
## 1. Objetivo da definicao
Definir conceitualmente o que sera aprovacao multinivel no futuro motor de aprovacao ITSM reutilizavel do SGX Sistema de Chamados, separando niveis sequenciais, niveis paralelos, participacao de grupos e aprovadores especificos, fallback por nivel e efeito operacional das decisoes.
## 2. Limites desta etapa
- Esta etapa registra apenas definicao conceitual, documentacao e atualizacao do roadmap/checklist.
- Nao houve implementacao funcional de aprovacao multinivel.
- Nao foram criadas entidades novas.
- Nao foram criadas migrations estruturais.
- Nao houve alteracao no modelo de dominio.
- Nao foi criada tabela de niveis de aprovacao nem relacao entre aprovacao, etapa e usuario.
- Nao houve alteracao em `AprovacaoChamado`, `BloqueiaAvancoAtendimento` ou no fluxo atual de aprovacao.
- Nao houve homologacao nem aceite final.
## 3. Contexto atual de aprovacao simples no sistema
- O fluxo atual do SGX trabalha com aprovacao simples associada ao chamado.
- `AprovacaoChamado` representa hoje uma instancia unica de aprovacao com uma decisao formal principal.
- O fluxo atual registra status, solicitacao, decisao, justificativa, aprovador e auditoria.
- Nao existe hoje divisao formal de uma mesma aprovacao em etapas, niveis ou dependencias entre decisoes.
## 4. Suporte atual, ou ausencia dele, para multiplos niveis
- Nao foi identificado suporte atual a multiplos niveis em `AprovacaoChamado`.
- Nao foi identificada estrutura formal de etapa, ordem de nivel, dependencia entre niveis ou regras de paralelismo.
- O sistema atual suporta aprovacao simples e, conceitualmente, futura associacao com grupo aprovador, mas ainda nao suporta fluxo multinivel.
## 5. Conceito de aprovacao multinivel
Aprovacao multinivel e o mecanismo conceitual que permite ao motor exigir duas ou mais decisoes formais antes de liberar uma acao sensivel, especialmente em cenarios que combinam risco, custo, mudanca, seguranca, compliance, acesso privilegiado, servico critico ou decisao gerencial.
Cada nivel representa uma etapa logica da aprovacao, com responsaveis, quorum, status, ordem e efeito operacional proprios.
## 6. Diferenca entre aprovacao simples, aprovacao por grupo e aprovacao multinivel
- Aprovacao simples: uma unica decisao formal libera ou reprova a acao.
- Grupo aprovador: varios possiveis decisores dentro de uma mesma etapa.
- Aprovacao multinivel: duas ou mais etapas formais, cada uma com responsaveis, quorum e efeito proprios.
- Grupo aprovador define quem pode decidir em um nivel; multinivel define quantos niveis existem e como eles se relacionam.
## 7. Conceito de nivel de aprovacao
Nivel de aprovacao e a unidade logica de decisao dentro de uma aprovacao multinivel.
Cada nivel pode ter:
- ordem;
- nome;
- objetivo;
- aprovador especifico;
- grupo aprovador;
- aprovador padrao como fallback;
- quorum;
- prazo;
- status;
- justificativa;
- efeito em caso de aprovacao;
- efeito em caso de reprovacao;
- regra de expiracao;
- regra de escalonamento.
## 8. Quando usar aprovacao multinivel
- Mudanca em ambiente produtivo com risco.
- Servico sensivel que exige aprovacao tecnica e gerencial.
- Solicitacao com custo relevante e validacao financeira.
- Liberacao de acesso privilegiado com aprovacao do gestor e da seguranca.
- Excecao de processo normal.
- Acao relacionada a compliance ou auditoria obrigatoria.
- Alteracao em servico critico.
- Cenario que exige segregacao de funcao.
- Decisao que exige mais de uma competencia ou autoridade.
## 9. Quando nao usar aprovacao multinivel
- Solicitacao simples sem risco, custo, acesso ou servico sensivel.
- Aprovacao que pode ser decidida por um aprovador especifico unico.
- Aprovacao que pode ser decidida por um grupo em uma unica etapa.
- Casos em que a multinivel adicionaria burocracia sem ganho de governanca.
- Chamados cuja urgencia operacional exija procedimento simplificado previamente definido.
- Incidentes simples com correcao padrao.
## 10. Niveis sequenciais
- Em aprovacao sequencial, o nivel seguinte so e iniciado apos aprovacao do nivel anterior.
- Exemplos conceituais:
  - aprovacao tecnica antes da aprovacao gerencial;
  - aprovacao gerencial antes da financeira;
  - aprovacao financeira antes da execucao;
  - aprovacao de seguranca apos validacao tecnica.
## 11. Niveis paralelos
- Em aprovacao paralela, dois ou mais niveis independentes podem ser avaliados ao mesmo tempo.
- Exemplos conceituais:
  - seguranca e compliance avaliando simultaneamente;
  - financeiro e dono do servico avaliando em paralelo;
  - infraestrutura e sistemas avaliando impactos distintos.
## 12. Fluxo misto com niveis sequenciais e paralelos
- Aprovacao mista combina etapas sequenciais e paralelas.
- Exemplo conceitual:
  - nivel 1 sequencial: triagem tecnica;
  - nivel 2 paralelo: financeiro e seguranca;
  - nivel 3 sequencial: gestor final.
- O motor futuro devera saber quais niveis liberam quais proximos niveis.
## 13. Participacao de grupo aprovador em um nivel
- Um nivel pode ser resolvido por grupo aprovador quando a competencia exigida pertencer a uma area ou conjunto de responsaveis.
- O grupo define quem pode decidir dentro daquele nivel.
- O nivel continua sendo a unidade de governanca; o grupo define a autoridade coletiva interna da etapa.
## 14. Participacao de aprovador especifico em um nivel
- Um nivel pode ser resolvido por aprovador especifico quando a decisao exigir autoridade nominal.
- O aprovador especifico pode prevalecer sobre grupo ou fallback, conforme a regra daquele nivel.
- Isso e especialmente relevante em decisoes financeiras, gerenciais, de dono do servico ou aceite formal individual.
## 15. Papel do aprovador padrao como fallback de nivel
- Se um nivel nao conseguir resolver aprovador especifico, deve tentar grupo aprovador.
- Se nao houver grupo valido, pode tentar delegacao valida.
- Se a regra do nivel permitir, o aprovador padrao atua como fallback para impedir que a etapa fique sem responsavel.
- O aprovador padrao nao deve substituir permanentemente niveis que exigem competencia especifica.
## 16. Regra conceitual de avanco entre niveis
- Em fluxo sequencial, o proximo nivel so inicia quando o nivel atual for aprovado.
- Em fluxo paralelo, os niveis independentes podem iniciar juntos.
- Em fluxo misto, a regra deve definir quais niveis liberam quais proximos niveis.
- O chamado so deve ser liberado quando todos os niveis obrigatorios forem concluidos conforme a regra.
## 17. Regra conceitual de reprovacao em um nivel
- Uma reprovacao pode encerrar toda a aprovacao.
- Uma reprovacao pode encerrar apenas um nivel, dependendo da regra.
- Uma reprovacao pode permitir reenvio apos ajuste.
- Uma reprovacao pode exigir justificativa obrigatoria.
- Uma reprovacao de nivel critico pode bloquear imediatamente a acao sensivel.
## 18. Regra conceitual de expiracao em um nivel
- Expiracao de nivel pode manter aprovacao pendente.
- Expiracao pode bloquear a acao.
- Expiracao pode escalar para outro aprovador.
- Expiracao pode acionar aprovador padrao.
- Expiracao pode cancelar a aprovacao.
- Expiracao deve ser auditavel.
## 19. Regra conceitual de ausencia de aprovador em um nivel
- Se um nivel nao conseguir resolver aprovador especifico, deve tentar grupo aprovador.
- Se nao houver grupo, deve tentar delegacao valida.
- Se nao houver delegacao, pode usar aprovador padrao como fallback.
- Se nem aprovador padrao existir, o nivel deve ficar em erro de configuracao e nao gerar aprovacao silenciosa sem responsavel.
## 20. Quorum por nivel
- Cada nivel pode ter regra propria de quorum:
  - aprovacao por um membro;
  - maioria simples;
  - unanimidade;
  - papel obrigatorio;
  - dono do servico obrigatorio;
  - aprovacao por gestor;
  - combinacao de papeis;
  - aprovacao por responsavel financeiro;
  - aprovacao por seguranca ou compliance.
## 21. Relacao com AprovacaoChamado
- `AprovacaoChamado` continua sendo a instancia persistente atual da aprovacao simples.
- No futuro, a aprovacao multinivel podera exigir estrutura derivada ou complementar para representar niveis, etapas, responsaveis, quorum e decisoes por nivel.
- Nesta etapa, `AprovacaoChamado` nao foi alterado.
## 22. Relacao com historico e auditoria
- A aprovacao multinivel deve registrar:
  - regra que gerou a aprovacao;
  - niveis exigidos;
  - ordem dos niveis;
  - niveis paralelos e sequenciais;
  - responsaveis resolvidos por nivel;
  - grupo ou aprovador especifico de cada nivel;
  - fallback usado por nivel, se houver;
  - quorum exigido por nivel;
  - decisoes por nivel;
  - quem decidiu;
  - quando decidiu;
  - justificativa;
  - expiracao;
  - escalonamento;
  - efeito final no chamado.
## 23. Relacao com bloqueio operacional do chamado
- Enquanto houver nivel obrigatorio pendente, o motor pode manter acoes sensiveis bloqueadas.
- A liberacao so deve ocorrer quando a regra de todos os niveis obrigatorios for satisfeita.
- A reprovacao em nivel impeditivo deve bloquear acoes dependentes daquela aprovacao.
## 24. Riscos de seguranca e governanca
- Criar niveis demais e travar a operacao.
- Nao definir claramente a ordem dos niveis.
- Misturar grupo aprovador com multinivel.
- Permitir avanco sem todos os niveis obrigatorios.
- Nao auditar decisoes por nivel.
- Usar aprovador padrao em excesso.
- Nao tratar ausencia de aprovador.
- Nao tratar expiracao.
- Nao tratar conflito de interesse por nivel.
- Criar fluxo multinivel para casos simples.
## 25. Compatibilidade com fluxo atual
- O conceito preserva o modulo atual de aprovacao.
- Nao exige mudanca imediata em `AprovacaoChamado`.
- O fluxo atual de solicitacao, aprovacao, reprovacao e cancelamento nao foi alterado.
- Esta etapa apenas define a base conceitual para modelagem futura.
## 26. Lacunas encontradas
- Nao existe suporte atual a multiplos niveis em `AprovacaoChamado`.
- Nao existe estrutura de etapa, ordem, dependencia ou paralelismo.
- Nao existe persistencia de quorum ou decisao por nivel.
- Nao existe politica operacional estruturada para expiracao, ausencia de aprovador ou conflito de interesse por nivel.
## 27. Decisoes adiadas para proximos itens
- Como modelar niveis.
- Se a aprovacao multinivel sera entidade propria ou extensao de `AprovacaoChamado`.
- Como armazenar ordem dos niveis.
- Como armazenar dependencia entre niveis.
- Como armazenar aprovacoes paralelas.
- Como associar grupos a niveis.
- Como associar aprovadores especificos a niveis.
- Como calcular quorum por nivel.
- Como tratar expiracao por nivel.
- Como escalar ausencia de decisao.
- Como aplicar delegacao em nivel.
- Como tratar conflito de interesse em nivel.
- Como exibir fluxo multinivel na interface.
- Como migrar aprovacoes atuais.
- Como manter compatibilidade com aprovacoes simples.
## 28. Conclusao tecnica
Aprovacao multinivel deve ser definida como fluxo de duas ou mais etapas formais de decisao, cada uma com responsaveis, quorum, status, ordem e efeito operacional proprios. O conceito amplia a governanca sem confundir grupo aprovador com nivel de aprovacao, preservando a base atual do modulo e preparando o sistema para sequenciamento, paralelismo e segregacao de funcao.
## 29. Proxima etapa recomendada
Executar o item 14 do checklist da Sprint 4: definir comportamento de aprovacao sequencial.
