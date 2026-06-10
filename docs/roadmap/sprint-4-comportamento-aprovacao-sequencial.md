# Sprint 4 - Comportamento de Aprovacao Sequencial
## 1. Objetivo da definicao
Definir conceitualmente como deve funcionar a aprovacao sequencial no futuro motor de aprovacao ITSM reutilizavel do SGX Sistema de Chamados, detalhando ordem entre niveis, gatilho de avanco, bloqueio operacional, reprovacao intermediaria, expiracao, ausencia de aprovador, fallback e rastreabilidade.
## 2. Limites desta etapa
- Esta etapa registra apenas definicao conceitual, documentacao e atualizacao do roadmap/checklist.
- Nao houve implementacao funcional de aprovacao sequencial.
- Nao foram criadas entidades novas.
- Nao foram criadas migrations estruturais.
- Nao houve alteracao no modelo de dominio.
- Nao foi criada estrutura de niveis, ordem sequencial ou relacionamento entre aprovacao, etapa e usuario.
- Nao houve alteracao em `AprovacaoChamado`, `BloqueiaAvancoAtendimento` ou no fluxo atual de aprovacao.
- Nao houve homologacao nem aceite final.
## 3. Contexto atual do fluxo de aprovacao
- O SGX possui hoje fluxo de aprovacao simples associado ao chamado.
- `AprovacaoChamado` registra solicitacao, aprovador, decisao, justificativa, status e auditoria principal.
- O fluxo atual nao possui encadeamento formal entre niveis de aprovacao.
- Nao existe hoje disparo automatico do proximo nivel apos decisao anterior.
## 4. Suporte atual, ou ausencia dele, para sequencia de niveis
- Nao foi identificado suporte atual a sequencia estruturada de niveis em `AprovacaoChamado`.
- Nao existe persistencia de ordem entre niveis, nivel atual, proximo nivel ou dependencia de aprovacao anterior.
- Nao existe hoje regra estrutural para bloquear inicio do proximo nivel ate conclusao valida do anterior.
## 5. Conceito de aprovacao sequencial
Aprovacao sequencial e o comportamento de aprovacao multinivel em que cada nivel so e iniciado depois que o nivel anterior for aprovado conforme sua propria regra de decisao.
Ela deve ser usada quando a decisao posterior depende da validacao anterior e quando o valor da segunda aprovacao so faz sentido apos o parecer da etapa precedente.
## 6. Diferenca entre aprovacao sequencial, paralela e multinivel generica
- Aprovacao sequencial: os niveis seguem ordem definida e o proximo depende da aprovacao do anterior.
- Aprovacao paralela: dois ou mais niveis independentes podem ser avaliados ao mesmo tempo.
- Aprovacao multinivel: conceito mais amplo que pode conter fluxo sequencial, paralelo ou misto.
- Sequencial e um comportamento especifico dentro da aprovacao multinivel.
## 7. Quando usar aprovacao sequencial
- Quando uma decisao depende de analise anterior.
- Quando e necessario validar tecnicamente antes de aprovar financeiramente.
- Quando o gestor so deve decidir depois do parecer tecnico.
- Quando seguranca ou compliance dependem de evidencia tecnica inicial.
- Quando uma reprovacao inicial torna desnecessaria avaliacao posterior.
- Quando ha hierarquia formal de decisao.
- Quando existe segregacao de funcao entre niveis.
- Quando ha custo ou risco que precisa passar por avaliacao progressiva.
## 8. Quando nao usar aprovacao sequencial
- Quando os pareceres sao independentes e podem ocorrer em paralelo.
- Quando uma decisao simples resolve o caso.
- Quando um grupo aprovador decide em etapa unica.
- Quando a sequencia adiciona burocracia sem ganho de governanca.
- Quando a urgencia exige procedimento simplificado previamente aprovado.
- Quando a ordem dos aprovadores nao altera a qualidade da decisao.
## 9. Inicio do primeiro nivel
- O primeiro nivel deve iniciar quando o motor concluir que existe aprovacao formal obrigatoria e a regra aplicavel definir fluxo sequencial.
- O primeiro nivel pode ser resolvido por:
  - aprovador especifico;
  - grupo aprovador;
  - dono do servico;
  - aprovador tecnico;
  - aprovador gerencial;
  - aprovador financeiro;
  - aprovador de seguranca;
  - aprovador padrao como fallback, se permitido.
## 10. Gatilho de inicio do proximo nivel
- O proximo nivel so deve iniciar quando:
  - o nivel atual estiver aprovado;
  - o quorum exigido do nivel atual for atingido;
  - nao houver reprovacao impeditiva no nivel atual;
  - nao houver erro de configuracao bloqueante;
  - a regra permitir avanco;
  - os dados sensiveis que sustentaram a aprovacao continuarem validos.
## 11. Situacoes em que o proximo nivel nao deve iniciar
- Nivel atual reprovado.
- Nivel atual expirado com regra de bloqueio.
- Nivel atual sem aprovador resolvido.
- Nivel atual sem quorum.
- Conflito de interesse nao resolvido.
- Aprovacao cancelada.
- Chamado cancelado ou encerrado antes da sequencia.
- Alteracao de dados sensiveis que exija reavaliacao do nivel atual.
- Regra de negocio determinar encerramento do fluxo.
## 12. Efeito da aprovacao de nivel intermediario
- A aprovacao de um nivel intermediario nao libera automaticamente o chamado.
- Ela apenas permite iniciar o proximo nivel ou avancar para a proxima etapa da sequencia.
- A liberacao operacional final so ocorre quando todos os niveis obrigatorios forem aprovados.
## 13. Efeito da reprovacao de nivel intermediario
- A reprovacao pode:
  - encerrar toda a sequencia;
  - bloquear acoes sensiveis imediatamente;
  - impedir inicio dos niveis seguintes;
  - exigir justificativa obrigatoria;
  - permitir reenvio apos ajuste;
  - devolver o chamado para correcao;
  - manter historico completo da decisao.
## 14. Efeito da aprovacao do ultimo nivel
- A aprovacao do ultimo nivel obrigatorio deve permitir que o motor considere a regra de aprovacao satisfeita.
- Isso libera acoes compativeis com o escopo aprovado.
- A liberacao final nao dispensa outras regras independentes de aprovacao que possam coexistir.
## 15. Efeito da expiracao de nivel sequencial
- A expiracao pode:
  - manter o nivel pendente;
  - bloquear a sequencia;
  - escalar para outro aprovador;
  - acionar aprovador padrao, se permitido;
  - cancelar a sequencia;
  - exigir nova solicitacao;
  - registrar evento de auditoria;
  - impedir inicio dos niveis seguintes.
## 16. Ausencia de aprovador em nivel sequencial
- Se um nivel nao conseguir resolver aprovador, a sequencia conceitual deve tentar:
  1. aprovador especifico;
  2. dono do servico;
  3. grupo aprovador;
  4. delegacao valida;
  5. aprovador padrao, se permitido;
  6. erro de configuracao, se nenhum responsavel for resolvido.
- O sistema nao deve gerar aprovacao sequencial silenciosa sem responsavel por nivel.
## 17. Relacao com aprovador especifico
- Aprovador especifico pode ser responsavel por um nivel quando a regra exigir autoridade nominal.
- Quando resolvido validamente, ele deve prevalecer sobre fallback generico do nivel.
## 18. Relacao com grupo aprovador
- Grupo aprovador pode decidir dentro de um nivel sequencial.
- O grupo respeita quorum proprio daquele nivel.
- A decisao do grupo conclui o nivel atual, nao necessariamente toda a sequencia.
## 19. Relacao com aprovador padrao
- Aprovador padrao pode atuar como fallback de um nivel quando permitido pela regra.
- Ele nao deve substituir nivel especifico quando houver aprovador, grupo, dono do servico ou delegacao valida.
- Seu uso deve ser auditavel e excepcional.
## 20. Relacao com quorum por nivel
- Cada nivel pode possuir quorum proprio.
- O proximo nivel so deve iniciar quando o quorum do nivel atual estiver satisfeito.
- Quorum insuficiente impede avanco sequencial.
## 21. Relacao com bloqueio operacional do chamado
- Enquanto houver nivel sequencial obrigatorio pendente, o motor pode bloquear acoes sensiveis do chamado.
- O bloqueio pode ser:
  - total de avanco operacional;
  - restrito a acoes sensiveis;
  - limitado ao escopo da aprovacao;
  - mantido ate aprovacao do ultimo nivel;
  - encerrado por reprovacao, cancelamento ou expiracao conforme regra.
## 22. Relacao com `AprovacaoChamado`
- `AprovacaoChamado` continua sendo a instancia persistente atual da aprovacao simples.
- No futuro, a aprovacao sequencial podera exigir estrutura complementar para representar niveis, ordem, status por nivel, responsaveis, quorum e decisoes.
- Nesta etapa, `AprovacaoChamado` nao foi alterado.
## 23. Relacao com historico e auditoria
- A aprovacao sequencial deve registrar:
  - regra que gerou a sequencia;
  - niveis exigidos;
  - ordem dos niveis;
  - nivel atual;
  - quando cada nivel foi iniciado;
  - quando cada nivel foi decidido;
  - quem decidiu;
  - grupo ou aprovador especifico por nivel;
  - fallback usado por nivel;
  - quorum exigido e atingido;
  - reprovacao intermediaria;
  - expiracao;
  - ausencia de aprovador;
  - alteracoes de dados sensiveis durante a sequencia;
  - efeito final no chamado.
## 24. Riscos de seguranca e governanca
- Iniciar proximo nivel sem aprovacao valida do anterior.
- Liberar chamado com niveis pendentes.
- Nao tratar reprovacao intermediaria.
- Nao tratar expiracao.
- Nao tratar ausencia de aprovador.
- Usar aprovador padrao indevidamente.
- Ignorar conflito de interesse.
- Criar sequencia excessiva para casos simples.
- Nao auditar ordem e decisoes por nivel.
- Permitir alteracao sensivel sem reavaliar a sequencia.
## 25. Compatibilidade com fluxo atual
- O conceito preserva o modulo atual de aprovacao.
- Nao exige mudanca imediata em `AprovacaoChamado`.
- O fluxo atual de solicitacao, aprovacao, reprovacao e cancelamento nao foi alterado.
- Esta etapa apenas define o comportamento futuro para orientar modelagem e implementacao posteriores.
## 26. Lacunas encontradas
- Nao existe suporte atual a sequencia estruturada de niveis.
- Nao existe modelo persistente de ordem, nivel atual ou disparo do proximo nivel.
- Nao existe politica operacional implementada para expiracao, ausencia de aprovador ou reenvio apos reprovacao.
## 27. Decisoes adiadas para proximos itens
- Como modelar niveis sequenciais.
- Como armazenar ordem dos niveis.
- Como iniciar automaticamente o proximo nivel.
- Como armazenar status por nivel.
- Como associar grupos a niveis.
- Como calcular quorum por nivel.
- Como tratar reenvio apos reprovacao.
- Como tratar expiracao.
- Como escalar ausencia de decisao.
- Como aplicar delegacao em nivel sequencial.
- Como tratar conflito de interesse.
- Como exibir sequencia na interface.
- Como migrar aprovacoes atuais.
- Como manter compatibilidade com aprovacoes simples.
## 28. Conclusao tecnica
Aprovacao sequencial deve ser definida como comportamento de aprovacao multinivel em que cada nivel depende da aprovacao valida do nivel anterior para iniciar. O conceito reforca governanca progressiva, hierarquia de decisao e segregacao de funcao, preservando a base atual do modulo e preparando o caminho para modelagem futura.
## 29. Proxima etapa recomendada
Executar o item 15 do checklist da Sprint 4: definir comportamento de aprovacao paralela.
