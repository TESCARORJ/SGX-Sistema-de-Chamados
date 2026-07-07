# Sprint 8 - Catalogo de Servicos 2.0

## Atualizacao 2026-07-07 - Documento de homologacao funcional criado

- Foi criado o documento [docs/homologacao/sprint-8-homologacao-funcional.md](/c:/Pessoal/SGX/SGX%20Sistema%20de%20Chamados%20Completo/docs/homologacao/sprint-8-homologacao-funcional.md) para registro formal da homologacao funcional da Sprint 8.
- O documento consolida identificacao, objetivo, cenarios obrigatorios, resultado geral, pendencias e campos de responsabilidade, mas permanece pendente de preenchimento e aprovacao.
- Nenhum status de checklist foi alterado nesta etapa.
- O item `74` continua pendente ate que a evidência funcional real seja registrada e aprovada.

## Atualizacao 2026-07-07 - Item 66 concluido

- Item `66` concluido exclusivamente com foco em consolidar a regressao de seguranca do formulario dinamico e das respostas na abertura guiada.
- A suite cobre rejeicao para campo inexistente, campo de outro servico, campo de outra versao, campo inativo, campo invisivel, opcao inexistente e opcao inativa.
- Tambem ficou consolidado que payload malicioso tentando manipular grupo tecnico, SLA, aprovacao ou classificacao continua rejeitado no endpoint guiado, sem criar chamado nem persistir respostas.
- Os testes de auditoria seguem garantindo que os valores efetivos das respostas nao sao expostos em `EventoAuditoria`.
- Nenhuma funcionalidade nova foi criada nesta etapa; a entrega ficou restrita a testes, checklist e documentacao.
- Status recalculado da Sprint 8 nesta etapa: `73/76` itens concluidos (`96%`).
- Os proximos itens pendentes reais registrados no checklist consolidado passam a ser `74`, `75` e `76`.

## Atualizacao 2026-07-06 - Item 65 concluido

- Item `65` concluido exclusivamente com foco em garantir que o solicitante so envie respostas permitidas para o formulario aplicavel ao servico.
- O backend agora valida, alem da estrutura e do escopo do campo, se `SelecaoUnica` usa apenas `Valor` presente em opcao ativa do campo e se `SelecaoMultipla` usa apenas itens de `Valores` presentes em opcoes ativas do campo.
- Opcoes inexistentes ou inativas passam a ser rejeitadas antes da persistencia, mantendo o chamado e as respostas sem gravacao quando o payload for invalido.
- Respostas para campos fora do formulario aplicavel, de outro servico, inativos ou invisiveis permanecem rejeitadas sem regressao.
- O contrato publico do portal permanece inalterado; a protecao foi concentrada no backend e nos testes de abertura guiada.
- Status recalculado da Sprint 8 nesta etapa: `72/76` itens concluidos (`95%`).
- O proximo item pendente real registrado no checklist consolidado passa a ser o item `66` (`Testar seguranca do formulario e respostas`).

## Atualizacao 2026-07-06 - Item 64 concluido

- Item `64` concluido exclusivamente com foco em impedir que o solicitante sobrescreva grupo tecnico, SLA, aprovacao ou classificacao operacional na abertura guiada.
- O contrato publico dedicado de abertura guiada continua aceitando apenas `CatalogoServicoId`, `Titulo`, `Descricao` e `RespostasFormulario`, sem expor `GrupoTecnicoId`, `SlaId`, campos de aprovacao ou classificacao.
- O backend e o catalogo permanecem como fonte de verdade para `NaturezaChamado`, `CategoriaId`, `SubcategoriaId`, `PrioridadeId`, `GrupoTecnicoId`, `SlaId` e aprovacao no fluxo guiado.
- Foram reforcados testes para payloads maliciosos enviados fora do contrato publico, validando que esses campos sao ignorados e nao alteram o chamado persistido.
- A abertura legada sem catalogo permanece compativel com a regra atual e sem regressao funcional nesta etapa.
- Status recalculado da Sprint 8 nesta etapa: `71/76` itens concluidos (`93%`).
- O proximo item pendente real registrado no checklist consolidado passa a ser o item `65` (`Garantir que solicitante so envie respostas permitidas para o servico`).

## Atualizacao 2026-07-06 - Item 63 concluido

- Item `63` concluido exclusivamente com foco em garantir autorizacao para manutencao administrativa do formulario.
- A leitura administrativa do formulario continua disponivel para `Administrador` e `Atendente`, preservando a consulta operacional ja existente.
- As operacoes de manutencao administrativa de formulario, versao, campo e opcao agora exigem `Administrador` na API e na camada de aplicacao, sem alterar os contratos, endpoints ou o frontend.
- Os testes de use case e API foram reforcados para validar que `Atendente` continua conseguindo consultar e passa a ser bloqueado nas operacoes de escrita e alteracao de status.
- Nenhuma funcionalidade fora desse escopo foi criada nesta etapa; abertura guiada, abertura legada, SLA, aprovacao, grupo tecnico e respostas do formulario permanecem inalterados.
- Status recalculado da Sprint 8 nesta etapa: `70/76` itens concluidos (`92%`).
- O proximo item pendente real registrado no checklist consolidado passa a ser o item `64` (`Garantir que solicitante nao manipule grupo, SLA, aprovacao ou classificacao`).

## Atualizacao 2026-07-06 - Item 55 concluido

- Item `55` concluido exclusivamente com foco em garantir a classificacao herdada do catalogo durante a abertura guiada.
- A abertura via `CatalogoServicoId` agora sempre persiste `NaturezaChamadoEnum.Requisicao` e faz `CategoriaId`, `SubcategoriaId` e `PrioridadeId` do catalogo prevalecerem quando configurados.
- Quando o catalogo nao define categoria, subcategoria ou prioridade, o fluxo preserva o fallback legado ja existente sem alterar SLA, aprovacao ou grupo tecnico.
- A API do portal e os testes de use case foram reforcados para evidenciar a regra sem criar endpoint, DTO ou estrutura nova.
- Nenhuma funcionalidade fora desse escopo foi criada nesta etapa.
- Status recalculado da Sprint 8 nesta etapa: `69/76` itens concluidos (`91%`).
- O proximo item pendente real registrado no checklist consolidado passa a ser o item `63` (`Garantir autorizacao para manutencao administrativa do formulario`), porque os itens `56` a `62` ja constavam como concluidos no seed anterior.

## Atualizacao 2026-07-06 - Item 54 concluido

- Item `54` concluido exclusivamente com foco no reforco dos testes de exibicao das respostas do formulario na area administrativa de atendimento.
- Os testes do detalhe administrativo validam resposta simples, resposta multipla, rotulo, tipo, ordem recebida e compatibilidade do layout quando nao houver respostas.
- Tambem foi validado que a tela continua usando apenas o endpoint existente de detalhe administrativo, sem chamada a endpoint novo para respostas.
- Nenhuma funcionalidade nova foi criada nesta etapa; a entrega ficou restrita a testes, checklist e documentacao.
- O proximo item pendente da Sprint 8 passa a ser o item `55` (`Garantir aplicacao de tipo, categoria, subcategoria e prioridade do catalogo`).
- Status recalculado da Sprint 8 nesta etapa: `68/76` itens concluidos (`89%`).

## Atualizacao 2026-07-06 - Item 53 concluido

- Item `53` concluido exclusivamente com foco no reforco dos testes de exibicao das respostas do formulario no portal do solicitante.
- Os testes do detalhe do chamado no portal validam resposta simples, resposta multipla, rotulo, tipo, ordem recebida e compatibilidade do layout quando nao houver respostas.
- Tambem foi validado que a tela continua usando apenas o endpoint existente de detalhe do chamado, sem chamada a endpoint novo para respostas.
- Nenhuma funcionalidade nova foi criada nesta etapa; a entrega ficou restrita a testes, checklist e documentacao.
- O proximo item pendente da Sprint 8 passa a ser o item `54` (`Testar exibicao das respostas no atendimento administrativo`).
- Status recalculado da Sprint 8 nesta etapa: `67/76` itens concluidos (`88%`).

## Atualizacao 2026-07-06 - Item 52 concluido

- Item `52` concluido exclusivamente com foco no reforco dos testes de persistencia das respostas do formulario na abertura guiada.
- Os testes agora cobrem explicitamente resposta simples em `Valor`, resposta multipla em `ValoresJson`, vinculos com `ChamadoId`, `FormularioServicoVersaoId` e `CampoFormularioServicoId`.
- O payload invalido continua sem persistir chamado nem respostas, e a auditoria tecnica do item `51` permanece sem expor valores sensiveis.
- Nenhuma funcionalidade nova foi criada nesta etapa; a entrega ficou restrita a testes, checklist e documentacao.
- O proximo item pendente da Sprint 8 passa a ser o item `53` (`Testar exibicao das respostas no portal`).
- Status recalculado da Sprint 8 nesta etapa: `66/76` itens concluidos (`87%`).

## Atualizacao 2026-07-06 - Item 51 concluido

- Item `51` concluido exclusivamente com foco no registro de auditoria tecnica das respostas persistidas na abertura guiada.
- Quando houver respostas persistidas, a abertura agora registra `Respostas do formulario persistidas na abertura guiada.` em `EventoAuditoria`.
- A auditoria registra apenas referencias seguras: `ChamadoId`, `FormularioServicoVersaoId`, quantidade de respostas persistidas, origem `AberturaGuiadaCatalogo` e contexto seguro do usuario executor.
- Valores das respostas nao sao registrados em auditoria; o historico funcional do item `50` permanece preservado sem regressao.
- O proximo item pendente da Sprint 8 passa a ser o item `52` (`Testar persistencia das respostas do formulario`).
- Status recalculado da Sprint 8 nesta etapa: `65/76` itens concluidos (`86%`).

## Atualizacao 2026-07-03 - Item 50 concluido

- Item `50` concluido exclusivamente com foco no registro do historico funcional da abertura guiada com formulario preenchido.
- A abertura guiada agora registra a mensagem resumida `Chamado aberto com formulario do servico preenchido.` quando houver respostas persistidas.
- O historico nao expõe valores, rotulos nem conteudo completo das respostas.
- O historico padrao de criacao do chamado e o historico de catalogo permanecem preservados.
- Auditoria tecnica especifica das respostas ainda nao existe.
- O proximo item pendente da Sprint 8 passa a ser o item `51` (`Registrar auditoria tecnica das respostas persistidas`).
- Status recalculado da Sprint 8 nesta etapa: `64/76` itens concluidos (`84%`).

## Atualizacao 2026-07-03 - Item 49 concluido

- Item `49` concluido exclusivamente com foco na exibicao das respostas do formulario na area administrativa de atendimento.
- A tela administrativa de detalhe do chamado agora exibe uma secao dedicada com `Rotulo`, `Tipo`, `Valor` e `Valores`, reutilizando a ordem ja recebida do backend.
- Chamados sem respostas continuam compativeis; a secao e ocultada quando nao ha `RespostasFormulario`.
- Nenhum backend funcional novo foi criado nesta etapa; a area administrativa reutiliza o contrato de detalhe ja existente.
- Auditoria e historico especifico dessas respostas ainda nao existem.
- O proximo item pendente da Sprint 8 passa a ser o item `50` (`Registrar historico da abertura com formulario preenchido`).
- Status recalculado da Sprint 8 nesta etapa: `63/76` itens concluidos (`83%`).

## Atualizacao 2026-07-03 - Item 48 concluido

- Item `48` concluido exclusivamente com foco na exibicao das respostas do formulario no portal do solicitante.
- A tela de detalhe do chamado no portal agora exibe uma secao dedicada com `Rotulo`, `Tipo`, `Valor` e `Valores`, respeitando a ordem recebida do backend.
- Chamados sem respostas continuam compativeis; a secao e ocultada quando nao ha `RespostasFormulario`.
- O backend funcional nao foi alterado nesta etapa; o portal reutiliza o contrato de detalhe ja existente.
- O atendimento administrativo ainda nao tem exibicao dedicada dessas respostas.
- O proximo item pendente da Sprint 8 passa a ser o item `49` (`Exibir respostas do formulario na area administrativa de atendimento`).
- Status recalculado da Sprint 8 nesta etapa: `62/76` itens concluidos (`82%`).

## Atualizacao 2026-07-03 - Item 47 concluido

- Item `47` concluido exclusivamente com foco em expor as respostas persistidas do formulario no detalhe do chamado.
- O backend agora retorna, no detalhamento do chamado, `CampoFormularioServicoId`, `Nome`, `Rotulo`, `Tipo`, `Valor`, `Valores` e `Ordem`.
- As respostas sao retornadas ordenadas por `Ordem` do campo; respostas simples usam `Valor` e respostas multiplas usam `Valores`.
- Chamados sem respostas continuam retornando colecao vazia, preservando compatibilidade dos consumidores.
- O portal do solicitante ainda nao tem exibicao dedicada dessas respostas.
- O atendimento administrativo ainda nao tem exibicao dedicada dessas respostas.
- O proximo item pendente da Sprint 8 passa a ser o item `48` (`Exibir respostas do formulario no portal do solicitante`).
- Status recalculado da Sprint 8 nesta etapa: `61/76` itens concluidos (`80%`).

## Atualizacao 2026-07-02 - Item 46 concluido

- Item `46` concluido exclusivamente com foco na persistencia das respostas do formulario na abertura guiada.
- As respostas validas agora sao persistidas em `RespostaFormularioChamado`, vinculadas ao chamado criado, a versao aplicavel do formulario e ao campo respondido.
- Respostas simples sao gravadas em `Valor` e respostas multiplas sao gravadas em `ValoresJson`.
- Servicos sem formulario continuam sem persistir respostas e continuam rejeitando payload preenchido.
- As respostas ainda nao sao exibidas no detalhe do chamado, no portal ou no atendimento.
- O proximo item pendente da Sprint 8 passa a ser o item `47` (`Exibir respostas do formulario no detalhe do chamado`).
- Status recalculado da Sprint 8 nesta etapa: `60/76` itens concluidos (`79%`).

## Atualizacao 2026-07-02 - Item 45 concluido

- Item `45` concluido exclusivamente com foco na migration estrutural de `RespostaFormularioChamado`.
- Foi criada a migration estrutural da tabela `respostas_formulario_chamado`, com FKs, indices e snapshot consolidado do modelo EF.
- O `pending model changes` deixado no item `44` foi fechado nesta etapa.
- As respostas ainda nao sao persistidas na abertura guiada.
- As respostas ainda nao sao exibidas no chamado, no portal ou no atendimento.
- O proximo item pendente da Sprint 8 passa a ser o item `46` (`Persistir respostas do formulario na abertura guiada`).
- Status recalculado da Sprint 8 nesta etapa: `59/76` itens concluidos (`78%`).

## Atualizacao 2026-07-02 - Item 44 concluido

- Item `44` concluido exclusivamente com foco na configuracao explicita do EF Core para `RespostaFormularioChamado`.
- O EF Core agora reconhece a entidade, a tabela planejada, as FKs para `Chamado`, `FormularioServicoVersao` e `CampoFormularioServico`, os indices e `DeleteBehavior.Restrict`.
- A migration estrutural da tabela continua reservada para o item `45`; ela nao foi criada nesta etapa.
- As respostas ainda nao sao persistidas na abertura guiada.
- As respostas ainda nao sao exibidas no chamado, no portal ou no atendimento.
- O proximo item pendente da Sprint 8 passa a ser o item `45` (`Criar migration estrutural para respostas do formulario`).
- Status recalculado da Sprint 8 nesta etapa: `58/76` itens concluidos (`76%`).

## Atualizacao 2026-07-02 - Item 43 concluido

- Item `43` concluido exclusivamente com foco em modelagem estrutural de dominio para respostas do formulario no chamado.
- Foi modelada a entidade `RespostaFormularioChamado`, com vinculos de dominio para `Chamado`, `FormularioServicoVersao` e `CampoFormularioServico`, suporte a valor unico ou multiplo e auditoria minima.
- A entidade foi mantida fora do EF Core nesta etapa; configuracao EF e migration estrutural continuam reservadas para os itens `44` e `45`.
- As respostas ainda nao sao persistidas na abertura guiada.
- As respostas ainda nao sao exibidas no chamado, no portal ou no atendimento.
- O proximo item pendente da Sprint 8 passa a ser o item `44` (`Configurar EF Core para respostas do formulario`).
- Status recalculado da Sprint 8 nesta etapa: `57/76` itens concluidos (`75%`).

## Atualizacao 2026-07-02 - Item 42 concluido

- Item `42` concluido exclusivamente com foco em testes e regressao da abertura guiada sem formulario configurado.
- A cobertura automatizada reforca backend use case, endpoint `GET /api/portal/catalogo-servicos/{slug}/preparar-chamado`, endpoint `POST /api/portal/catalogo-servicos/requisicoes`, frontend do portal e checklist da Sprint 8.
- O fluxo sem formulario continua retornando `Formulario = null`, continua aceitando abertura sem `RespostasFormulario` ou com `RespostasFormulario = []` e continua rejeitando respostas preenchidas.
- O frontend continua sem renderizar secao dinamica para servicos sem formulario, continua limpando respostas anteriores ao trocar de servico e continua propagando erro do backend se payload invalido ocorrer.
- Respostas do formulario continuam sem persistencia; nao foi criada tabela, entidade, endpoint, DTO ou migration estrutural para respostas.
- O proximo item pendente da Sprint 8 passa a ser o item `43` (`Modelar persistencia das respostas do formulario no chamado`).
- Status recalculado da Sprint 8 nesta etapa: `56/76` itens concluidos (`74%`).

## Atualizacao 2026-07-01 - Item 41 concluido

- Item `41` concluido exclusivamente com foco em testes e regressao da abertura guiada com respostas invalidas.
- A cobertura automatizada reforca backend use case, endpoint `POST /api/portal/catalogo-servicos/requisicoes`, propagacao do erro no frontend e rastreabilidade do checklist da Sprint 8.
- Durante a consolidacao do item `41`, os itens `61` e `62` tambem foram sincronizados no checklist por ja possuirem evidencia automatizada preexistente de regressao em SLA, grupo, aprovacao, abertura legada e incidente; nao houve funcionalidade nova associada a essa sincronizacao.
- Respostas do formulario continuam sem persistencia; nao foi criada tabela, entidade, endpoint, DTO ou migration estrutural para respostas.
- O proximo item pendente da Sprint 8 passa a ser o item `43` (`Modelar persistencia das respostas do formulario no chamado`).
- Status recalculado da Sprint 8 nesta etapa: `56/76` itens concluidos (`74%`).

## Objetivo desta atualizacao
Concluir o item 42 da Sprint 8 com testes de regressao da abertura guiada sem formulario configurado, preservando o restante do backlog funcional ainda pendente.

Esta etapa implementa apenas testes e pequenos ajustes de rastreabilidade. Nao implementa persistencia de respostas, exibicao das respostas, novas entidades, novos endpoints ou migrations estruturais de dominio.

## Atualizacao tecnica posterior
- Os itens `5`, `6`, `7`, `8`, `9` e `10` ja estavam tecnicamente implementados no codigo e nos testes; nesta atualizacao, o roadmap foi sincronizado para refletir essa evidencia real.
- O item `11` passou a ter evidencia dedicada por meio de testes de consulta administrativa `GET`, fechando a lacuna de rastreabilidade restante para o grupo tecnico no catalogo.
- O item `16` agora esta concluido com a modelagem estrutural da entidade `FormularioServico`, vinculada opcionalmente a `CatalogoServico`.
- O item `17` agora esta concluido com a modelagem estrutural da entidade `CampoFormularioServico`, vinculada a `FormularioServico`.
- O item `18` agora esta concluido com a modelagem dos tipos permitidos para `CampoFormularioServico`.
- O item `19` agora esta concluido com a modelagem de obrigatoriedade, ordem, texto de ajuda e visibilidade como metadados do campo.
- O item `20` agora esta concluido com a modelagem estrutural de `OpcaoCampoFormularioServico`, vinculada a `CampoFormularioServico`, incluindo valor, rotulo, ordem, ativacao e indices unicos por campo.
- O item `21` agora esta concluido com a modelagem estrutural de `FormularioServicoVersao`, adotando versionamento separado do cabecalho do formulario e vinculando `CampoFormularioServico` a uma versao.
- O item `22` agora esta concluido com a auditoria e consolidacao das configuracoes EF Core de `FormularioServico`, `FormularioServicoVersao`, `CampoFormularioServico` e `OpcaoCampoFormularioServico`, incluindo metadata, FKs, `DeleteBehavior.Restrict`, limites, indices unicos e nomes seguros para PostgreSQL.
- O item `23` agora esta concluido como etapa de governanca estrutural de banco: as migrations dos itens `16` a `21` foram auditadas, a separacao entre migrations estruturais e migrations de checklist foi comprovada por teste e o pacote estrutural do formulario dinamico ficou formalmente consolidado.
- O item `24` agora esta concluido com a criacao dos contratos administrativos de leitura e request para formulario, versao, campo e opcao de campo, mantendo a etapa estritamente contratual.
- O item `25` agora esta concluido com validators administrativos para formulario, versao, campo e opcao de campo, limitados a validacoes contratuais dos requests.
- O item `26` agora esta concluido com use cases administrativos de aplicacao para formulario, versao, campo e opcao, incluindo validacoes dependentes de banco, mapeamento para DTO e operacoes de criacao, atualizacao, consulta e inativacao/reativacao sem expor endpoints.
- O item `27` agora esta concluido com endpoints administrativos para formulario, versao, campo e opcao, expondo os use cases ja criados e preservando a camada HTTP apenas como orquestracao de rota, autorizacao, validacao e retorno.
- Nenhuma nova tabela foi criada nesta etapa, porque o modelo e as migrations estruturais incrementais ja estavam consistentes.
- Esta etapa nao adicionou endpoints nem alterou o fluxo de abertura.
- O frontend administrativo de manutencao do formulario agora existe somente na area administrativa do catalogo.
- Validacoes de existencia, duplicidade e regras relacionais basicas agora estao cobertas na camada de aplicacao; publicacao funcional, clonagem e validacao dinamica continuam adiadas.
- O item `Aplicar grupo tecnico responsavel na abertura guiada por catalogo` agora esta funcionalmente conectado ao fluxo real de abertura por catalogo.
- Quando `CatalogoServico.GrupoTecnicoId` estiver configurado e o grupo estiver ativo, o chamado aberto a partir do catalogo nasce com esse grupo tecnico como responsavel inicial.
- Quando o servico nao possuir grupo configurado, ou o grupo configurado estiver indisponivel, o fluxo preserva o fallback legado sem alterar SLA, status, aprovacao ou abertura sem catalogo.
- O checklist da Sprint 8 agora reflete a conexao funcional completa deste item: modelagem, EF, migration estrutural, contratos, validators, use cases, consulta administrativa, aplicacao, fallback e testes obrigatorios de grupo tecnico por catalogo.
- Decisao arquitetural desta etapa: foi adotada a alternativa B, com a entidade separada `FormularioServicoVersao`. O cabecalho `FormularioServico` permanece vinculado ao `CatalogoServico`, enquanto os campos passam a pertencer a uma versao especifica. Esse desenho prepara historico futuro sem conflitar com o vinculo `1:1` atual entre catalogo e formulario.
- O versionamento continua apenas estrutural: ainda nao ha publicacao funcional, clonagem de versao, bloqueio operacional de edicao, respostas versionadas ou uso da versao na abertura guiada.
- A auditoria EF desta etapa nao introduziu endpoint administrativo, frontend, validacao dinamica nem alteracao do fluxo de abertura; ela consolidou apenas o mapeamento relacional e os testes de metadata.
- O percentual persistido do roadmap foi sincronizado para `62%`, acompanhando `47` itens concluidos de `76`.

## Diagnostico obrigatorio

### 1. Estado atual de `CatalogoServico`
- Encontrado em `src/SGX.SistemaChamado.Domain/Entities/CatalogoServico.cs`.
- A entidade ja possui `DepartamentoResponsavelId`, `CategoriaId`, `SubcategoriaId`, `PrioridadePadraoId`, `SlaPadraoId`, `ArtigoBaseConhecimentoId`, `GrupoTecnicoId`, `PermiteAberturaChamado` e `RequerAprovacao`.
- O catalogo atual ja sustenta consulta administrativa, consulta no portal e abertura guiada basica.

### 2. `GrupoTecnicoId` ou equivalente no catalogo
- Encontrado em `CatalogoServico` como vinculo opcional.
- Encontrado em `CatalogoServicoConfiguration` com relacionamento restritivo.
- Encontrado nos DTOs administrativos do catalogo.
- A divergencia residual estava apenas na conexao com a abertura guiada, agora coberta no `AbrirChamadoUseCase`.

### 3. Estrutura de formulario dinamico
- Encontrada no dominio e na infraestrutura, com as entidades `FormularioServico`, `FormularioServicoVersao`, `CampoFormularioServico` e `OpcaoCampoFormularioServico`.
- O banco ja possui estrutura consolidada para `formularios_servico`, `formularios_servico_versoes`, `campos_formulario_servico` e `opcoes_campos_formulario_servico`.
- A governanca do item `23` validou que as migrations estruturais dos itens `16` a `21` cobrem integralmente esse pacote, sem mistura com dados de checklist.

### 4. Estrutura de resposta de formulario
- Nao encontrada no dominio, aplicacao, infraestrutura, API ou frontend.
- Nao ha persistencia estruturada de respostas vinculadas ao chamado.

### 5. Versionamento de campos
- Encontrado para formulario de servico por meio da entidade `FormularioServicoVersao`.
- `CampoFormularioServico` pertence a uma versao especifica do formulario, preservando a base estrutural para historico futuro sem ativar ainda publicacao, clonagem ou respostas versionadas.

### 6. Validacao dinamica
- Nao encontrada para campos dinamicos de servico.
- O validator atual de abertura guiada (`AbrirRequisicaoServicoCatalogoRequestValidator`) valida apenas `CatalogoServicoId`, `Titulo` e `Descricao`.

### 7. Frontend para renderizacao dinamica
- Nao encontrado.
- `NovoChamadoView.vue` exibe formulario estatico.
- `CatalogoServicoDetalhePage.vue` apenas prepara a abertura e redireciona para a tela estatica.

### 8. Endpoint de consulta dos campos do servico
- Nao encontrado endpoint dedicado para campos dinamicos do servico.
- Existe apenas `GET /api/portal/catalogo-servicos/{slug}/preparar-chamado`, que retorna dados fixos do servico.

### 9. Endpoint de persistencia de respostas
- Nao encontrado endpoint dedicado para envio/persistencia de respostas dinamicas.
- Existe apenas `POST /api/portal/catalogo-servicos/requisicoes`, com payload fixo de abertura.

### 10. Pendencias da Sprint 7 documentadas como transferidas
- A Sprint 7 ja registrava, no checklist, as lacunas:
  - item 10: `Aplicar grupo responsavel configurado no catalogo`;
  - item 13: `Implementar ou reutilizar formulario por servico`;
  - item 14: `Validar e persistir respostas do formulario`.
- Antes desta atualizacao, a Sprint 8 ainda nao explicitava essa transferencia no proprio checklist.

## Decisao de governanca
- Os itens 10, 13 e 14 da Sprint 7 passam a ser tratados como pendencias estruturais rastreadas da Sprint 8.
- O roadmap persistido continua usando apenas os macrogrupos suportados por `GrupoRoadmapChecklist`.
- Neste documento, os grupos tecnicos detalhados solicitados pelo roadmap funcional sao preservados textualmente.

## Recalculo do status da Sprint 8
- Total de itens ativos no novo checklist: `76`
- Itens concluidos com evidencia: `54`
- Itens pendentes: `22`
- Percentual recalculado: `71%`

Decisao registrada:
- o percentual anterior de `50%` nao foi preservado;
- o status foi rebaixado para refletir a evidencia real da Sprint 8;
- a base funcional do catalogo existe, mas o objetivo especifico do Catalogo 2.0 permanece majoritariamente pendente.

Status recomendado apos diagnostico:
- `Status`: `Em desenvolvimento`
- `Status da implementacao`: `Em desenvolvimento`
- `Status tecnico`: `Parcial`
- `Proxima acao`: `Testar abertura guiada com respostas invalidas`

## Itens concluidos com evidencia
- `1` Diagnosticar estado atual do Catalogo 2.0 e pendencias transferidas da Sprint 7.
- `2` Confirmar escopo estrutural do Catalogo 2.0.
- `3` Definir criterios de aceite para motor de abertura guiada por servico.
- `4` Documentar decisao de transferencia dos itens 10, 13 e 14 da Sprint 7.
- `5` Modelar vinculo opcional entre Catalogo de Servico e Grupo Tecnico responsavel.
- `6` Configurar EF Core para vinculo entre catalogo e grupo tecnico.
- `7` Criar migration estrutural para grupo tecnico no catalogo.
- `8` Ajustar contratos administrativos do catalogo para grupo tecnico responsavel.
- `9` Ajustar validators administrativos do catalogo para grupo tecnico responsavel.
- `10` Ajustar use cases administrativos do catalogo para grupo tecnico responsavel.
- `11` Expor grupo tecnico responsavel na consulta administrativa do catalogo.
- `12` Aplicar grupo tecnico responsavel na abertura guiada por catalogo.
- `13` Preservar fallback de grupo quando servico nao possuir grupo configurado.
- `14` Testar aplicacao de grupo tecnico configurado no catalogo.
- `15` Testar fallback de grupo sem configuracao no catalogo.
- `16` Modelar entidade de formulario por servico.
- `17` Modelar campos do formulario por servico.
- `18` Modelar tipos de campo permitidos.
- `19` Modelar obrigatoriedade, ordem, ajuda e visibilidade dos campos.
- `20` Modelar opcoes de campos enumerados, se aplicavel.
- `21` Modelar versionamento de formulario por servico.
- `22` Configurar EF Core para formulario e campos.
- `23` Criar migration estrutural para formulario dinamico.
- `24` Ajustar contratos administrativos para manutencao de formulario do servico.
- `25` Criar validators administrativos para formulario do servico.
- `26` Criar use cases administrativos para configurar formulario do servico.
- `27` Criar endpoints administrativos para formulario do servico.
- `30` Expor campos do formulario no endpoint de preparacao da abertura.
- `29` Testar configuracao administrativa de formulario por servico.
- `31` Ajustar contrato de abertura guiada para receber respostas do formulario.
- `32` Criar validator de respostas do formulario na abertura guiada.
- `35` Impedir respostas de campos inexistentes ou de outro servico.
- `36` Preservar abertura guiada sem formulario configurado.
- `37` Ajustar frontend do portal para renderizar formulario dinamico.
- `41` Testar abertura guiada com respostas invalidas.
- `56` Garantir aplicacao de SLA padrao do catalogo.
- `57` Garantir aplicacao de aprovacao por servico.
- `58` Garantir compatibilidade com abertura legada sem catalogo.
- `59` Garantir compatibilidade com incidentes.
- `60` Garantir compatibilidade com aprovacao legada e motor novo.
- `67` Atualizar documentacao tecnica da Sprint 8.
- `68` Atualizar `docs/ROADMAP.md` e `docs/ROADMAP-ITSM.md`.
- `69` Atualizar `SeedData` e testes de checklist da Sprint 8.
- `70` Criar migration de checklist da Sprint 8.
- `71` Executar build backend e testes direcionados.
- `72` Executar build frontend e validacao TypeScript.
- `73` Verificar EF pending model changes.

## Itens com cobertura parcial, mas mantidos pendentes
- `64` O backend ja protege parte da classificacao oficial do catalogo, mas o item tambem menciona grupo, SLA e manipulacao indevida em um escopo que ainda nao esta completo; por isso permanece pendente.

## Checklist tecnico completo

### Diagnostico e governanca
- [x] 1. Diagnosticar estado atual do Catalogo 2.0 e pendencias transferidas da Sprint 7
  Grupo solicitado: Planejamento
- [x] 2. Confirmar escopo estrutural do Catalogo 2.0
  Grupo solicitado: Planejamento
- [x] 3. Definir criterios de aceite para motor de abertura guiada por servico
  Grupo solicitado: Planejamento
- [x] 4. Documentar decisao de transferencia dos itens 10, 13 e 14 da Sprint 7
  Grupo solicitado: Governanca

### Grupo responsavel por catalogo
- [x] 5. Modelar vinculo opcional entre Catalogo de Servico e Grupo Tecnico responsavel
  Grupo solicitado: Modelagem estrutural
- [x] 6. Configurar EF Core para vinculo entre catalogo e grupo tecnico
  Grupo solicitado: Infraestrutura
- [x] 7. Criar migration estrutural para grupo tecnico no catalogo
  Grupo solicitado: Migration estrutural
- [x] 8. Ajustar contratos administrativos do catalogo para grupo tecnico responsavel
  Grupo solicitado: Contrato/DTO
- [x] 9. Ajustar validators administrativos do catalogo para grupo tecnico responsavel
  Grupo solicitado: Validator
- [x] 10. Ajustar use cases administrativos do catalogo para grupo tecnico responsavel
  Grupo solicitado: Aplicacao
- [x] 11. Expor grupo tecnico responsavel na consulta administrativa do catalogo
  Grupo solicitado: API
- [x] 12. Aplicar grupo tecnico responsavel na abertura guiada por catalogo
  Grupo solicitado: Regra de aplicacao
- [x] 13. Preservar fallback de grupo quando servico nao possuir grupo configurado
  Grupo solicitado: Compatibilidade
- [x] 14. Testar aplicacao de grupo tecnico configurado no catalogo
  Grupo solicitado: Testes
- [x] 15. Testar fallback de grupo sem configuracao no catalogo
  Grupo solicitado: Testes

### Formulario dinamico por servico
- [x] 16. Modelar entidade de formulario por servico
  Grupo solicitado: Modelagem estrutural
- [x] 17. Modelar campos do formulario por servico
  Grupo solicitado: Modelagem estrutural
- [x] 18. Modelar tipos de campo permitidos
  Grupo solicitado: Modelagem estrutural
- [x] 19. Modelar obrigatoriedade, ordem, ajuda e visibilidade dos campos
  Grupo solicitado: Modelagem estrutural
- [x] 20. Modelar opcoes de campos enumerados, se aplicavel
  Grupo solicitado: Modelagem estrutural
- [x] 21. Modelar versionamento de formulario por servico
  Grupo solicitado: Modelagem estrutural
- [x] 22. Configurar EF Core para formulario e campos
  Grupo solicitado: Infraestrutura
- [x] 23. Criar migration estrutural para formulario dinamico
  Grupo solicitado: Migration estrutural
- [x] 24. Ajustar contratos administrativos para manutencao de formulario do servico
  Grupo solicitado: Contrato/DTO
- [x] 25. Criar validators administrativos para formulario do servico
  Grupo solicitado: Validator
- [x] 26. Criar use cases administrativos para configurar formulario do servico
  Grupo solicitado: Aplicacao
- [x] 27. Criar endpoints administrativos para formulario do servico
  Grupo solicitado: API
- [x] 28. Ajustar frontend administrativo do catalogo para configurar formulario
  Grupo solicitado: Frontend
- [x] 29. Testar configuracao administrativa de formulario por servico
  Grupo solicitado: Testes

## Atualizacao do item 41

- O backend agora possui cobertura automatizada consolidada para rejeitar texto curto acima do limite, numero invalido, data invalida, booleano invalido, uso indevido de `Valor`/`Valores`, campo inexistente, campo de outro servico, campo de outra versao nao aplicavel, campo inativo, campo invisivel, duplicidade por campo e resposta sem conteudo.
- A API agora possui evidencia explicita de erro `400` tanto para payload contratualmente invalido quanto para resposta invalida rejeitada no use case, preservando o padrao de resposta ja adotado pelo endpoint.
- O frontend agora possui evidencia adicional de que o service propaga o erro bruto do backend e de que a view nao deve simular sucesso quando a abertura guiada falha.
- O checklist tambem foi sincronizado para refletir regressao ja coberta dos itens `61` e `62`, sem ampliar escopo funcional da sprint.
- As respostas do formulario continuam sem persistencia, sem historico especifico e sem exibicao no chamado ou no portal.
- A validacao de opcoes permitidas permanece como pendencia separada, pois nao apareceu implementada de forma completa no fluxo atual e nao foi expandida nesta etapa.
- As respostas ainda nao sao persistidas.
- O frontend do portal ainda nao renderiza formulario dinamico.
- O proximo item pendente passa a ser o item `32`.

## Atualizacao do item 32

- O item `32` agora esta concluido com a validacao contratual de `RespostasFormulario` na abertura guiada.
- Cada resposta agora exige `CampoFormularioServicoId` valido e exatamente uma forma de conteudo: `Valor` ou `Valores`.
- A colecao `RespostasFormulario` continua opcional e aceita lista vazia.
- O validator tambem rejeita itens vazios em `Valores`, excesso de tamanho e respostas duplicadas para o mesmo campo no mesmo request.
- Esta etapa ainda nao valida obrigatoriedade.
- Esta etapa ainda nao valida tipo ou formato.
- Esta etapa ainda nao valida se o campo existe ou pertence ao servico.
- Esta etapa ainda nao persiste respostas.
- O proximo item pendente passa a ser o item `33`.

## Atualizacao do item 33

- O item `33` agora esta concluido com a validacao de obrigatoriedade dos campos do formulario no backend, durante a abertura guiada por catalogo.
- A validacao considera somente campos `Ativo`, `Visivel` e `Obrigatorio` da versao aplicavel do formulario.
- A mesma regra do item `30` foi preservada para selecionar a versao aplicavel: priorizar a versao ativa publicada mais recente; sem publicacao aplicavel, usar a versao ativa de maior numero.
- Campo obrigatorio respondido com `Valor` ou `Valores` permite a abertura normalmente.
- Campo opcional, campo inativo e campo invisivel nao sao exigidos.
- Esta etapa ainda nao valida tipo ou formato das respostas.
- Esta etapa ainda nao valida opcoes permitidas nem se o campo pertence ao servico alem do minimo necessario para obrigatoriedade.
- Esta etapa ainda nao persiste respostas.
- O proximo item pendente passa a ser o item `35`.

## Atualizacao do item 34

- O item `34` agora esta concluido com a validacao de tipos e formatos das respostas no backend.
- `TextoCurto` exige `Valor` e respeita limite seguro de `180` caracteres.
- `TextoLongo` exige `Valor` e respeita limite seguro de `4000` caracteres.
- `Numero` exige `Valor` conversivel para decimal invariante.
- `Data` exige `Valor` no formato ISO `yyyy-MM-dd`.
- `Booleano` exige `Valor` com `true` ou `false`, case-insensitive.
- `SelecaoUnica` exige `Valor`.
- `SelecaoMultipla` exige `Valores` e nao aceita `Valor`.
- Campo opcional sem resposta continua permitido.
- Campo inativo ou invisivel continua sem exigencia nem validacao.
- Esta etapa ainda nao persiste respostas.
- Esta etapa ainda nao valida completamente campo inexistente/de outro servico.
- Esta etapa ainda nao valida se o valor pertence as opcoes permitidas.
- O proximo item pendente passa a ser o item `35`.

## Atualizacao do item 35

- O item `35` agora esta concluido com a rejeicao backend de respostas fora do escopo do formulario aplicavel.
- Cada `CampoFormularioServicoId` respondido agora precisa pertencer a versao aplicavel do formulario do servico.
- Respostas para campo inexistente, de outro formulario, de outra versao nao aplicavel ou de outro servico sao rejeitadas.
- Respostas para campo inativo ou invisivel tambem sao rejeitadas.
- Campos opcionais validos continuam aceitos.
- Obrigatoriedade continua validada pelo item `33`.
- Tipo e formato continuam validados pelo item `34`.
- Esta etapa ainda nao persiste respostas.
- Esta etapa ainda nao valida pertencimento do valor as opcoes permitidas.
- O proximo item pendente passa a ser o item `36`.

## Atualizacao do item 36

- O item `36` agora esta concluido com regressao explicita para abertura guiada sem formulario configurado.
- `GET /api/portal/catalogo-servicos/{slug}/preparar-chamado` continua retornando `Formulario = null` para servicos sem formulario.
- `POST /api/portal/catalogo-servicos/requisicoes` continua aceitando `RespostasFormulario = null` e `RespostasFormulario = []` quando o servico nao possui formulario.
- Quando o servico nao possui formulario, respostas preenchidas agora sao rejeitadas como payload incompatível.
- SLA, grupo tecnico e aprovacao continuam preservados no fluxo do catalogo sem formulario.
- As respostas ainda nao sao persistidas.
- O proximo item pendente passa a ser o item `37`.

## Atualizacao do item 37

- O item `37` agora esta concluido com a renderizacao dinamica do formulario no portal do solicitante.
- O frontend passa a exibir `TextoCurto`, `TextoLongo`, `Numero`, `Data`, `Booleano`, `SelecaoUnica` e `SelecaoMultipla`.
- Rotulo, obrigatoriedade, texto de ajuda e ordenacao por `Ordem` passam a ser respeitados na tela.
- Campos invisiveis/inativos e opcoes inativas continuam filtrados por seguranca no frontend.
- Servicos sem formulario continuam funcionando sem erro e sem alterar a abertura atual.
- As respostas ainda nao sao enviadas no POST.
- As respostas ainda nao sao persistidas.
- O proximo item pendente passa a ser o item `38`.

## Atualizacao do item 38

- Item `38` concluido.
- O portal agora coleta e envia `RespostasFormulario` no request de abertura guiada.
- Campos de valor unico sao serializados em `Valor`.
- Campos `SelecaoMultipla` sao serializados em `Valores`.
- Campos sem resposta nao sao enviados.
- Trocar o servico limpa respostas anteriores do formulario.
- Servico sem formulario continua sem enviar respostas preenchidas.
- As respostas ainda nao sao persistidas.
- A exibicao das respostas no chamado ainda nao existe.
- O proximo item pendente passa a ser o item `39`.

## Atualizacao do item 39

- Item `39` concluido.
- A abertura guiada com formulario valido esta coberta por testes de use case, integracao de endpoint e frontend.
- O fluxo valido agora possui evidencia automatizada para todos os tipos principais: `TextoCurto`, `TextoLongo`, `Numero`, `Data`, `Booleano`, `SelecaoUnica` e `SelecaoMultipla`.
- Os testes confirmam preservacao de grupo tecnico, SLA e aprovacao do catalogo.
- Os testes confirmam que as respostas ainda nao sao persistidas.
- O proximo item pendente passa a ser o item `40`.

## Atualizacao do item 40

- Item `40` concluido.
- A abertura guiada com campos obrigatorios ausentes esta coberta por testes de use case, integracao de endpoint e frontend.
- Os testes cobrem ausencia total de resposta, `Valor` vazio, `Valores` vazios e mensagem de erro coerente para payload incompleto.
- Os testes confirmam que campo opcional ausente continua permitido e que campos obrigatorios inativos ou invisiveis continuam nao exigidos.
- As respostas ainda nao sao persistidas.
- O proximo item pendente passa a ser o item `41`.

### Consulta e abertura guiada
- [x] 30. Expor campos do formulario no endpoint de preparacao da abertura
  Grupo solicitado: API
- [x] 31. Ajustar contrato de abertura guiada para receber respostas do formulario
  Grupo solicitado: Contrato/DTO
- [x] 32. Criar validator de respostas do formulario na abertura guiada
  Grupo solicitado: Validator
- [x] 33. Validar obrigatoriedade dos campos no backend
  Grupo solicitado: Regra de aplicacao
- [x] 34. Validar tipos e formatos das respostas no backend
  Grupo solicitado: Regra de aplicacao
- [x] 35. Impedir respostas de campos inexistentes ou de outro servico
  Grupo solicitado: Seguranca
- [x] 36. Preservar abertura guiada sem formulario configurado
  Grupo solicitado: Compatibilidade
- [x] 37. Ajustar frontend do portal para renderizar formulario dinamico
  Grupo solicitado: Frontend
- [x] 38. Ajustar frontend do portal para enviar respostas do formulario
  Grupo solicitado: Frontend
- [x] 39. Testar abertura guiada com formulario valido
  Grupo solicitado: Testes
- [x] 40. Testar abertura guiada com campos obrigatorios ausentes
  Grupo solicitado: Testes
- [x] 41. Testar abertura guiada com respostas invalidas
  Grupo solicitado: Testes
- [x] 42. Testar abertura guiada sem formulario configurado
  Grupo solicitado: Testes

### Persistencia e rastreabilidade das respostas
- [x] 43. Modelar persistencia das respostas do formulario no chamado
  Grupo solicitado: Modelagem estrutural
- [x] 44. Configurar EF Core para respostas do formulario
  Grupo solicitado: Infraestrutura
- [x] 45. Criar migration estrutural para respostas do formulario
  Grupo solicitado: Migration estrutural
- [x] 46. Persistir respostas do formulario na abertura guiada
  Grupo solicitado: Aplicacao
- [x] 47. Exibir respostas do formulario no detalhe do chamado
  Grupo solicitado: API
- [x] 48. Exibir respostas do formulario no portal do solicitante
  Grupo solicitado: Frontend
- [x] 49. Exibir respostas do formulario na area administrativa de atendimento
  Grupo solicitado: Frontend
- [x] 50. Registrar historico da abertura com formulario preenchido
  Grupo solicitado: Governanca
- [x] 51. Registrar auditoria tecnica das respostas persistidas
  Grupo solicitado: Auditoria
- [x] 52. Testar persistencia das respostas do formulario
  Grupo solicitado: Testes
- [x] 53. Testar exibicao das respostas no portal
  Grupo solicitado: Testes
- [x] 54. Testar exibicao das respostas no atendimento administrativo
  Grupo solicitado: Testes

### Integracoes com regras operacionais
- [x] 55. Garantir aplicacao de tipo, categoria, subcategoria e prioridade do catalogo
  Grupo solicitado: Regra de aplicacao
- [x] 56. Garantir aplicacao de SLA padrao do catalogo
  Grupo solicitado: Regra de aplicacao
- [x] 57. Garantir aplicacao de aprovacao por servico
  Grupo solicitado: Regra de aplicacao
- [x] 58. Garantir compatibilidade com abertura legada sem catalogo
  Grupo solicitado: Compatibilidade
- [x] 59. Garantir compatibilidade com incidentes
  Grupo solicitado: Compatibilidade
- [x] 60. Garantir compatibilidade com aprovacao legada e motor novo
  Grupo solicitado: Compatibilidade
- [x] 61. Testar regressao de abertura guiada com SLA, grupo e aprovacao
  Grupo solicitado: Testes
- [x] 62. Testar regressao de abertura legada e incidente
  Grupo solicitado: Testes

### Seguranca e governanca
- [x] 63. Garantir autorizacao para manutencao administrativa do formulario
  Grupo solicitado: Seguranca
- [ ] 64. Garantir que solicitante nao manipule grupo, SLA, aprovacao ou classificacao
  Grupo solicitado: Seguranca
- [ ] 65. Garantir que solicitante so envie respostas permitidas para o servico
  Grupo solicitado: Seguranca
- [ ] 66. Testar seguranca do formulario e respostas
  Grupo solicitado: Testes
- [x] 67. Atualizar documentacao tecnica da Sprint 8
  Grupo solicitado: Documentacao
- [x] 68. Atualizar docs/ROADMAP.md e docs/ROADMAP-ITSM.md
  Grupo solicitado: Documentacao
- [x] 69. Atualizar SeedData e testes de checklist da Sprint 8
  Grupo solicitado: Governanca
- [x] 70. Criar migration de checklist da Sprint 8
  Grupo solicitado: Migration de dados/checklist
- [x] 71. Executar build backend e testes direcionados
  Grupo solicitado: Validacao tecnica
- [x] 72. Executar build frontend e validacao TypeScript
  Grupo solicitado: Validacao tecnica
- [x] 73. Verificar EF pending model changes
  Grupo solicitado: Validacao tecnica
- [ ] 74. Registrar homologacao funcional
  Grupo solicitado: Homologacao
- [ ] 75. Registrar homologacao visual responsiva
  Grupo solicitado: Homologacao
- [ ] 76. Registrar aceite formal somente com evidencia
  Grupo solicitado: Homologacao

## Comandos de validacao desta atualizacao
- `dotnet test tests/SGX.SistemaChamado.Tests /p:UseSharedCompilation=false -m:1 --filter "FullyQualifiedName~FormularioDinamicoMigrationsTests|FullyQualifiedName~FormularioServicoEfCoreConfigurationTests|FullyQualifiedName~RoadmapSprint8CatalogoServicosChecklistTests"`
- `dotnet test tests/SGX.SistemaChamado.Tests /p:UseSharedCompilation=false -m:1 --filter "RoadmapSprint8CatalogoServicosChecklistTests"`
- `dotnet build SGX.SistemaChamado.sln /p:UseSharedCompilation=false`
- `npm run build` em `src/SGX.SistemaChamado.Web`
- `dotnet ef migrations add SincronizarChecklistSprint8MigrationEstruturalFormularioDinamico -p src/SGX.SistemaChamado.Infrastructure -s src/SGX.SistemaChamado.Api`
- `dotnet ef migrations has-pending-model-changes -p src/SGX.SistemaChamado.Infrastructure -s src/SGX.SistemaChamado.Api --no-build`
