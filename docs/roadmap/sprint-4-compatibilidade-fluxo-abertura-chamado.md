# Sprint 4 - Compatibilidade com fluxo atual de abertura de chamado

## Objetivo da avaliacao

Avaliacao conceitual de como o futuro motor de aprovacoes ITSM deve se encaixar no fluxo atual de abertura de chamados sem quebrar a criacao pelo portal, sem duplicar aprovacao por catalogo e sem alterar o comportamento funcional antes da implementacao formal do motor.

## Limites desta etapa

Esta etapa nao altera `AbrirChamadoUseCase`, nao altera o portal, nao altera `AprovacaoChamado`, nao altera `BloqueiaAvancoAtendimento`, nao altera `AguardandoAprovacao` e nao implementa motor novo. O objetivo e apenas registrar compatibilidade, riscos e diretrizes.

## Contexto atual do fluxo de abertura

O fluxo atual de abertura cria o chamado no portal com validacao de campos minimos, derivacao de prioridade por matriz impacto x urgencia, inicializacao de SLA e criacao automatica de aprovacao apenas quando o servico de catalogo possui `RequerAprovacao = true`.

## Componentes atuais envolvidos na abertura

- `PortalController` recebe `POST /api/portal/chamados`.
- `AbrirChamadoUseCase` centraliza validacoes, criacao do chamado, historico, aprovacao automatica por catalogo e SLA inicial.
- `CamposObrigatoriosChamadoService` valida obrigatorios conforme natureza e contexto.
- `PrioridadeChamadoMatrizService` tenta derivar prioridade por impacto e urgencia.
- `CatalogoServicosPortalUseCases` prepara abertura por catalogo e resolve defaults do servico.
- `ISlaService` inicializa SLA na abertura.
- `PortalUseCaseHelpers` devolve no response o estado atual de aprovacao para o portal.
- `NovoChamadoView.vue` monta o payload e reaproveita `catalogoServicoId` e `catalogoServicoSlug` quando a abertura parte de um servico.

## Dados informados ou derivados na abertura

O fluxo atual trabalha com titulo, descricao, solicitante, categoria, subcategoria, prioridade, tipo de solicitacao, local/unidade, departamento, inventario/ativo, catalogo ou servico, natureza ITSM, impacto, urgencia, historico inicial e SLA inicial. Quando ha servico de catalogo, o backend tambem pode derivar categoria, subcategoria, prioridade, departamento e `SlaPadraoId`.

## Natureza ITSM na abertura

`NaturezaChamado` ja participa da abertura e influencia obrigatoriedade de impacto e urgencia. Hoje:

- `Incidente`, `Mudanca`, `Problema`, `EventoAlerta` e `TarefaOperacional` exigem impacto e urgencia.
- `Requisicao` pode seguir sem esses campos, desde que haja classificacao suficiente para abrir o chamado.

O motor futuro deve usar a natureza como criterio de aprovacao, mas nao deve quebrar a abertura atual nem substituir essa validacao basica.

## Impacto e urgencia na abertura

Impacto e urgencia ja sao usados para calcular prioridade pela matriz atual. Eles ajudam na criticidade do chamado e no SLA, mas hoje nao criam aprovacao por si so. Conceitualmente, o motor futuro pode usar essa combinacao para sinalizacao ou exigencia de aprovacao, desde que isso nao bloqueie a criacao comum do chamado sem regra explicita.

## Categoria, subcategoria, catalogo e servico na abertura

A abertura atual aceita classificacao direta por categoria/subcategoria/tipo ou indireta por servico de catalogo. Quando ha catalogo:

- o servico precisa estar publicado, ativo e visivel ao perfil do usuario;
- o servico pode substituir categoria, subcategoria, prioridade e departamento informados manualmente;
- o portal usa `prepararAberturaChamado` para preencher o formulario antes do `POST`.

## Aprovacao automatica atual por catalogo ou servico

Hoje a unica aprovacao automatica na abertura nasce de `CatalogoServico.RequerAprovacao`. Nessa situacao, o backend:

- cria `AprovacaoChamado` com `TipoOrigem = CatalogoServico`;
- grava historico `AprovacaoSolicitada`;
- devolve `RequerAprovacao = true`, `AprovacaoPendente = true` e `StatusAprovacao = Pendente`.

Nao existe hoje aprovacao automatica por natureza, impacto, urgencia, custo, risco ou grupo aprovador.

## Relacao atual com `AprovacaoChamado`

`AprovacaoChamado` e criada somente apos a criacao do chamado e apenas quando o catalogo exige aprovacao. O fluxo atual evita duplicidade imediata verificando se ja existe aprovacao pendente ativa para o chamado antes de inserir nova instancia.

## Relacao atual com `BloqueiaAvancoAtendimento`

O fluxo atual de abertura nao decide explicitamente bloqueio por escopo nem seta nova regra de bloqueio conceitual do motor. A compatibilidade futura deve preservar `BloqueiaAvancoAtendimento` como sinal legado de bloqueio simples quando a aprovacao criada ou existente for bloqueante.

## Relacao atual com `AguardandoAprovacao`

O fluxo atual de abertura nao muda o chamado para `AguardandoAprovacao` durante a criacao. A aprovacao pendente pode existir sem esse status inicial. O motor futuro nao deve depender exclusivamente desse status para reconhecer pendencia de aprovacao.

## Relacao atual com SLA inicial

O SLA inicial continua sendo configurado na abertura por `slaService.InicializarNaAberturaAsync`, podendo usar `SlaPadraoId` do catalogo quando houver. A aprovacao automatica atual nao pausa SLA nem cria SLA proprio de aprovacao.

## Cenario de abertura comum sem aprovacao

Chamados comuns, sem catalogo que exige aprovacao e sem regra futura sensivel ativada, devem continuar abrindo normalmente. Conceitualmente, o motor futuro deve retornar `Permitido`.

## Cenario de abertura com servico sensivel

Como a sensibilidade de servico ainda nao esta estruturada no modelo atual da abertura, o motor futuro nao deve presumir bloqueio automatico hoje. A diretriz e:

- permitir a criacao do chamado se os dados minimos existirem;
- gerar aprovacao ou sinalizacao somente quando a regra formal existir;
- bloquear acoes sensiveis posteriores, nao a criacao em si, salvo ausencia de dado obrigatorio real.

## Cenario de abertura de mudanca

`Mudanca` ja exige descricao com detalhamento minimo, impacto e urgencia. Conceitualmente, o motor futuro pode usar essa natureza para exigir aprovacao pendente e bloqueio posterior, mas a abertura em si deve continuar permitida quando os dados obrigatorios forem validos.

## Cenario de abertura de requisicao simples

`Requisicao` deve seguir como o caso mais compativel com abertura simples. Se nao houver servico sensivel, regra de catalogo ou outro gatilho formal, o motor futuro deve preservar abertura sem aprovacao.

## Cenario de abertura de incidente

`Incidente` hoje prioriza rapidez de registro e restauracao. O motor futuro nao deve transformar incidente comum em bloqueio generico de abertura. A aprovacao futura, quando existir, deve recair sobre execucao sensivel, nao sobre o registro inicial do incidente, salvo regra critica explicitamente configurada.

## Cenario de abertura com dados incompletos

O fluxo atual falha quando faltam dados minimos reais, como titulo, descricao, classificacao basica, impacto/urgencia para certas naturezas ou consistencia de catalogo/categoria/subcategoria. Fora isso, a diretriz futura e usar fallback seguro: permitir abertura, sinalizar incompletude e exigir revisao antes de acao sensivel.

## Quando a abertura deve seguir permitida

A abertura deve seguir permitida quando:

- os campos obrigatorios atuais estiverem validos;
- o chamado for comum e nao houver regra formal de aprovacao aplicavel;
- houver risco baixo ou apenas necessidade de sinalizacao;
- houver catalogo valido que nao exige aprovacao;
- existir incompletude nao impeditiva que possa ser tratada por revisao posterior.

## Quando a abertura deve gerar aprovacao

A abertura deve gerar aprovacao quando:

- o catalogo ja exigir aprovacao, preservando o comportamento atual;
- regra futura por natureza, servico, tipo, custo, risco ou combinacao critica for formalmente implementada;
- o escopo da aprovacao estiver claro e auditavel;
- nao houver aprovacao equivalente ja criada para o mesmo escopo.

## Quando a abertura deve gerar bloqueio

O bloqueio futuro deve incidir sobre acoes posteriores sensiveis quando:

- houver aprovacao pendente bloqueante;
- a regra sensivel exigir decisao formal antes de execucao, encerramento, liberacao de acesso ou mudanca critica;
- o escopo aprovado ainda nao cobrir a acao que se pretende executar.

A diretriz e nao bloquear genericamente a criacao do chamado.

## Quando a abertura deve gerar apenas sinalizacao

A abertura deve gerar apenas sinalizacao quando:

- a natureza ou contexto indicarem atencao, mas nao aprovacao obrigatoria;
- houver dado incompleto nao impeditivo;
- houver indicio de risco que exija revisao humana antes de acao sensivel;
- a regra futura pedir observacao, nao bloqueio.

## Quando a abertura deve falhar por dados obrigatorios

A abertura deve falhar apenas quando faltarem ou forem invalidos dados basicos necessarios para criar o chamado, como:

- titulo;
- descricao;
- natureza invalida;
- impacto ou urgencia obrigatorios para a natureza;
- categoria/tipo/catalogo ausente quando nenhum deles foi informado;
- categoria, subcategoria, prioridade, tipo, local ou servico inativo/invalido;
- servico sem visibilidade ou sem permissao de abertura.

## Quando aplicar fallback seguro

Aplicar fallback seguro quando:

- os dados atuais permitirem criar o chamado, mas nao permitirem decidir toda a governanca futura;
- nao houver custo ou risco estruturados;
- o servico sensivel ainda nao estiver formalmente classificado;
- a regra futura depender de informacao que o portal ainda nao fornece.

Nesse caso, a diretriz e permitir abertura, sinalizar revisao e nao criar aprovacao automatica sem escopo claro.

## Risco de aprovacao duplicada na abertura

Os principais riscos sao:

- catalogo gerar aprovacao e o motor gerar outra equivalente para o mesmo escopo;
- regras de natureza e servico criarem instancias paralelas sem consolidacao;
- reprocessamento do payload criar nova aprovacao em cada reenvio;
- ausencia de escopo estruturado levar a duplicidade por interpretacao.

O motor futuro deve detectar aprovacao existente equivalente e preservar a instancia atual de `AprovacaoChamado`.

## Risco de quebra do portal

O motor futuro nao deve:

- exigir campos que `NovoChamadoView.vue` ainda nao envia;
- mudar o contrato de `POST /api/portal/chamados` sem versao;
- bloquear abertura comum por regra futura ainda nao implantada;
- depender de componente novo do frontend para funcionar;
- falhar silenciosamente ao avaliar aprovacao.

## Compatibilidade com auditoria de solicitacao de aprovacao

Quando a abertura gerar aprovacao, a trilha futura de auditoria deve registrar:

- regra que disparou a solicitacao;
- dados do chamado no momento da abertura;
- solicitante;
- catalogo, servico, natureza, tipo, impacto e urgencia;
- aprovador, fallback ou ausencia de resolucao;
- se houve bloqueio, sinalizacao ou apenas pendencia;
- se o SLA inicial foi afetado.

## Compatibilidade com chamados sem custo ou risco estruturado

O modelo atual de abertura nao depende de custo ou risco estruturados. Portanto, o motor futuro nao deve impedir a abertura atual pela ausencia desses dados. Quando esses campos existirem no futuro, devem complementar a decisao sem quebrar compatibilidade retroativa do fluxo atual.

## Compatibilidade com chamados sem servico sensivel

O fluxo atual aceita abertura sem classificacao formal de servico sensivel. A ausencia dessa classificacao nao pode ser tratada automaticamente nem como liberacao irrestrita nem como bloqueio absoluto. A diretriz e sinalizacao ou revisao manual quando houver gatilho sensivel posterior.

## Diretrizes para encaixe futuro do motor

- Chamar o motor em ponto controlado dentro da abertura, apos validacao dos dados minimos e antes da consolidacao final da resposta.
- Avaliar apenas os dados realmente disponiveis na abertura atual.
- Retornar decisao clara: `Permitido`, `PermitidoComSinalizacao`, `RequerAprovacao`, `BloqueioPosterior` ou erro por dado obrigatorio.
- Reaproveitar a aprovacao automatica atual por catalogo como compatibilidade.
- Evitar aprovacao duplicada para o mesmo escopo.
- Preservar SLA inicial atual.
- Registrar auditoria da solicitacao quando houver aprovacao.

## Diretrizes para preservar comportamento atual

- Nao alterar a abertura comum.
- Nao alterar o formulario atual do portal.
- Nao alterar o contrato atual do endpoint.
- Nao alterar o uso atual do catalogo na abertura.
- Nao alterar a geracao atual de aprovacao por catalogo.
- Nao introduzir bloqueio de abertura por regra futura ainda nao implementada.
- Nao reclassificar automaticamente o chamado na abertura.

## Riscos de seguranca e governanca

- bloquear chamados comuns indevidamente;
- permitir servico sensivel sem aprovacao posterior quando a regra futura exigir;
- gerar aprovacao duplicada;
- criar aprovacao sem escopo claro;
- depender de campos inexistentes;
- quebrar o portal;
- quebrar SLA inicial;
- nao auditar a aprovacao criada na abertura;
- ignorar `BloqueiaAvancoAtendimento`;
- tratar `AguardandoAprovacao` como unica forma de bloqueio;
- usar impacto e urgencia isolados como bloqueio indevido da criacao.

## Decisoes adiadas para proximos itens

- onde chamar exatamente o motor no fluxo de abertura;
- como consolidar multiplas regras de aprovacao na abertura;
- como evitar duplicidade tecnicamente por escopo;
- como registrar auditoria estruturada da solicitacao;
- como tratar SLA durante aprovacao pendente;
- como refletir sinalizacao e pendencia no portal;
- como modelar custo e risco na abertura;
- como testar abertura com motor ativo;
- como versionar contrato da API, se necessario.

## Conclusao tecnica

O fluxo atual de abertura ja possui base suficiente para receber o motor futuro sem ruptura, desde que a integracao respeite o principio de que abrir chamado e uma acao simples e confiavel. A aprovacao deve atuar como camada de decisao e bloqueio posterior por escopo, nao como barreira generica de criacao.

## Proxima etapa recomendada

Executar o item 27 da Sprint 4: avaliar compatibilidade com fluxo atual de atendimento.
