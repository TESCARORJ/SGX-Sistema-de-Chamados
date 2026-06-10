# Sprint 4 - Servico de aplicacao para regras de aprovacao

## 1. Objetivo do servico de aplicacao
Criar a primeira camada de orquestracao administrativa das regras de aprovacao, permitindo listar, consultar, criar, atualizar, ativar/inativar, validar e avaliar conceitualmente configuracoes de regra sem gerar qualquer efeito operacional no chamado.

## 2. Limites desta etapa
- Nao gera `InstanciaAprovacaoChamado`.
- Nao gera `AprovacaoChamado` legado.
- Nao gera `EtapaAprovacaoChamado`.
- Nao gera `DecisaoAprovacaoChamado`.
- Nao altera abertura, atendimento ou SLA.
- Nao executa bloqueio, liberacao, workflow, quorum, delegacao ou endpoints.

## 3. Contexto estrutural e contratual existente
O item 29 consolidou `ConfiguracaoRegraAprovacao`.
O item 34 consolidou os contratos administrativos de configuracao.
O item 35 consolidou os contratos administrativos de decisao.
Este item 36 reutiliza a base estrutural e contratual para inaugurar a camada de aplicacao das regras.

## 4. Entidade base usada pelo servico
- `ConfiguracaoRegraAprovacao`

## 5. Contratos usados pelo servico
- `ListarConfiguracoesRegrasAprovacaoRequest`
- `CriarConfiguracaoRegraAprovacaoRequest`
- `AtualizarConfiguracaoRegraAprovacaoRequest`
- `AlterarStatusConfiguracaoRegraAprovacaoRequest`
- `ValidarConfiguracaoRegraAprovacaoRequest`
- `ValidarConfiguracaoRegraAprovacaoResponse`
- `ConfiguracaoRegraAprovacaoResumoResponse`
- `ConfiguracaoRegraAprovacaoResponse`
- `ContextoAvaliacaoRegraAprovacaoRequest`
- `RegraAprovacaoCandidataResponse`
- `AvaliacaoConfiguracaoRegraAprovacaoResponse`

## 6. Validators usados pelo servico
- `ListarConfiguracoesRegrasAprovacaoRequestValidator`
- `CriarConfiguracaoRegraAprovacaoRequestValidator`
- `AtualizarConfiguracaoRegraAprovacaoRequestValidator`
- `AlterarStatusConfiguracaoRegraAprovacaoRequestValidator`
- `ValidarConfiguracaoRegraAprovacaoRequestValidator`
- `ContextoAvaliacaoRegraAprovacaoRequestValidator`

## 7. Padrao de service/use case identificado no projeto
O projeto privilegia interfaces em `Application.Interfaces.Admin` e implementacoes agrupadas em `Application.UseCases.Admin`, registradas em `Infrastructure/DependencyInjection.cs`. O item 36 seguiu esse mesmo padrao.

## 8. Servico criado
- `ConfiguracaoRegraAprovacaoAdminUseCases`

## 9. Interface criada
- `IAdminConfiguracaoRegraAprovacaoUseCases`

## 10. Operacoes administrativas implementadas
- `ListarAsync`
- `ObterPorIdAsync`
- `CriarAsync`
- `AtualizarAsync`
- `AlterarStatusAsync`
- `ValidarAsync`
- `ListarRegrasCandidatasAsync`
- `AvaliarRegraAsync`

## 11. Listagem de regras
Suporta filtros por termo, ativo, tipo, escopo, natureza, tipo de solicitacao, catalogo, categoria, subcategoria, efeito operacional, fluxo, estrategia de aprovador, bloqueio, exigencia de aprovacao e vigencia em data de referencia. A paginação segue o padrao administrativo do projeto.

## 12. Consulta de detalhe
Retorna a visao administrativa completa da configuracao, incluindo nomes resolvidos de tipo de solicitacao, catalogo, categoria, subcategoria e aprovadores quando houver relacionamento carregado.

## 13. Criacao de regra
A criacao reutiliza o contrato do item 34, aplica o validator, valida relacionamentos simples, valida duplicidade por `Nome + Versao` e instancia `ConfiguracaoRegraAprovacao` sem gerar qualquer artefato operacional.

## 14. Atualizacao de regra
A atualizacao reutiliza o contrato do item 34, reaplica validacoes, impede duplicidade e atualiza a entidade persistida sem criar aprovacao. Como a entidade ainda nao expunha um metodo publico de edicao completa, a camada de aplicacao usou um adaptador interno para copiar estado validado e registrar auditoria de atualizacao sem alterar o modelo persistente.

## 15. Ativacao/inativacao
Foi criada a operacao `AlterarStatusAsync`, usando `AlterarStatusConfiguracaoRegraAprovacaoRequest`, com ativacao e inativacao logicas da regra.

## 16. Validacao de regra
`ValidarAsync` retorna `ValidarConfiguracaoRegraAprovacaoResponse` com erros e alertas. Ela nao grava nada e nao executa workflow. Alem do validator, checa duplicidade e existencia simples de relacionamentos.

## 17. Avaliacao de regra aplicavel
Foi criada uma avaliacao pura:
- `ListarRegrasCandidatasAsync` localiza regras ativas, vigentes e compatíveis com o contexto;
- `AvaliarRegraAsync` escolhe a melhor regra por prioridade, especificidade, ordem e versao.

Nenhuma dessas operacoes cria aprovacao, instancia, etapa ou decisao.

## 18. Tratamento de vigencia
Regras candidatas precisam estar:
- ativas;
- vigentes em `DataReferencia` ou `DateTime.UtcNow`.

O servico respeita `VigenteDe` e `VigenteAte`.

## 19. Tratamento de prioridade e ordem
Na avaliacao pura, as regras candidatas sao ordenadas por:
- `Prioridade` decrescente;
- especificidade decrescente;
- `Ordem` crescente;
- `Versao` decrescente;
- `Nome` crescente como desempate final.

## 20. Tratamento de criterios por natureza, tipo, catalogo, categoria, subcategoria, impacto, urgencia, prioridade, custo e risco
O servico considera:
- correspondencia exata para natureza, tipo de solicitacao, catalogo, categoria e subcategoria;
- comparacao minima para impacto, urgencia, prioridade, custo e nivel de risco.

## 21. Tratamento de efeito operacional
O servico apenas expõe `EfeitoOperacionalRegraAprovacao` como dado de avaliacao. Ele nao aplica o efeito no chamado.

## 22. Tratamento de bloqueante versus informativa
As validacoes administrativas impedem combinacoes incoerentes, como regra informativa bloqueante. O servico apenas devolve essas flags como metadado.

## 23. Tratamento de estrategia de aprovador
O servico respeita `TipoResolucaoAprovador`, `AprovadorEspecificoUsuarioId` e `AprovadorPadraoUsuarioId`, validando coerencia e existencia simples quando os ids forem informados.

## 24. Compatibilidade com aprovador padrao
Mantida. Regras com resolucao por aprovador padrao continuam suportadas e retornadas pela avaliacao pura.

## 25. Compatibilidade futura com grupo aprovador
Mantida apenas conceitualmente. O servico nao cria grupo aprovador real, mas preserva o espaco evolutivo para isso em itens posteriores.

## 26. Compatibilidade com fluxo simples, sequencial, paralelo e multinivel
Mantida no nivel de configuracao e avaliacao conceitual. O servico reconhece `TipoFluxoAprovacao`, mas nao cria ou consolida workflow.

## 27. Compatibilidade com chamados legados
Totalmente preservada. Nenhum chamado legado foi reprocessado. `AprovacaoChamado` legado nao foi alterada.

## 28. Garantias de ausencia de efeitos colaterais operacionais
O servico:
- nao cria aprovacao;
- nao cria instancia;
- nao cria etapa;
- nao cria decisao;
- nao bloqueia andamento;
- nao altera status;
- nao altera SLA.

## 29. Relacao futura com servico de instancia de aprovacao
Este servico prepara o item 37, que passara a consumir a regra escolhida para criar a instancia concreta de aprovacao.

## 30. Relacao futura com geracao de aprovacao obrigatoria
Este servico prepara o item 38, que usara a avaliacao pura para decidir se deve ou nao gerar aprovacao obrigatoria no chamado.

## 31. Relacao futura com endpoints administrativos
Os metodos e DTOs ja estao prontos para futura exposicao por endpoints administrativos, sem criar API neste item.

## 32. Relacao futura com frontend
Os contratos e responses permitem construir telas futuras de cadastro, listagem, validacao e simulacao administrativa de regras.

## 33. Testes criados
- `ServicoAplicacaoRegrasAprovacaoTests`

Cobertura focada:
- criacao de regra valida;
- rejeicao de regra incoerente;
- atualizacao sem efeitos colaterais;
- listagem filtrada por ativo;
- avaliacao pura com regra candidata;
- exclusao de regra inativa da lista de candidatas.

## 34. Riscos de seguranca e governanca
- o adaptador interno de atualizacao depende da forma atual da entidade e deve ser revisitado quando o dominio expuser edicao publica;
- a avaliacao pura pode ser confundida com motor operacional se for usada fora da camada planejada;
- regras muito amplas podem produzir candidatos demais sem uma governanca clara de especificidade.

## 35. Decisoes adiadas para proximos itens
- servico de instancia de aprovacao;
- geracao automatica de aprovacao obrigatoria;
- bloqueio operacional;
- criacao de etapa e decisao;
- quorum, delegacao e grupo aprovador real;
- endpoints administrativos;
- frontend administrativo completo.

## 36. Conclusao tecnica
O item 36 estabeleceu a camada de aplicacao das regras de aprovacao com CRUD administrativo, validacao consistente e avaliacao conceitual pura. A base ficou pronta para os proximos itens sem antecipar o motor operacional.

## 37. Proxima etapa recomendada
Executar o item 37: criar servico de aplicacao para instancia de aprovacao.
