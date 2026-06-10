# Sprint 4 - Contratos de configuracao de aprovacao

## 1. Objetivo da criacao dos contratos
Definir a fronteira de entrada e saida da camada de aplicacao para administracao futura de regras de aprovacao, usando `ConfiguracaoRegraAprovacao` como base persistente sem implementar servico, API ou frontend neste item.

## 2. Limites desta etapa
- Criar apenas contratos/DTOs e validacoes simples de formato.
- Nao implementar motor de avaliacao.
- Nao implementar endpoints, controllers, frontend ou service frontend.
- Nao implementar o servico de aplicacao completo, que fica para o item 36.
- Nao criar migration estrutural.

## 3. Contexto estrutural ja existente
O nucleo do motor ja possui:
- `ConfiguracaoRegraAprovacao` para politica/regra.
- `InstanciaAprovacaoChamado` para processo concreto.
- `EtapaAprovacaoChamado` para nivel/ramo interno.
- `DecisaoAprovacaoChamado` para o ato formal de decisao.

## 4. Entidade base usada pelos contratos
Os contratos desta etapa espelham os campos persistidos em `ConfiguracaoRegraAprovacao`, preservando a separacao entre entidade de dominio e DTOs administrativos.

## 5. Enums usados pelos contratos
- `TipoRegraAprovacao`
- `EscopoRegraAprovacao`
- `EfeitoOperacionalRegraAprovacao`
- `TipoFluxoAprovacao`
- `TipoResolucaoAprovadorRegraAprovacao`
- `NaturezaChamadoEnum`
- `ImpactoChamadoEnum`
- `UrgenciaChamadoEnum`
- `PrioridadeChamadoEnum`

## 6. Padrao de contratos encontrado no projeto
O projeto concentra contratos administrativos em arquivos `Admin*Dtos.cs` dentro de `src/SGX.SistemaChamado.Application/DTOs/Admin`, usando classes para requests, records para responses resumidos/detalhados e validators separados em `src/SGX.SistemaChamado.Application/Validators`.

## 7. Contratos criados
- `ListarConfiguracoesRegrasAprovacaoRequest`
- `CriarConfiguracaoRegraAprovacaoRequest`
- `AtualizarConfiguracaoRegraAprovacaoRequest`
- `AlterarStatusConfiguracaoRegraAprovacaoRequest`
- `ValidarConfiguracaoRegraAprovacaoRequest`
- `ValidarConfiguracaoRegraAprovacaoResponse`
- `ConfiguracaoRegraAprovacaoResumoResponse`
- `ConfiguracaoRegraAprovacaoResponse`

## 8. Contrato de criacao
`CriarConfiguracaoRegraAprovacaoRequest` cobre nome, descricao, classificacao da regra, criterios de aplicacao, efeito operacional, fluxo, resolucao de aprovador, vigencia, prioridade, versao, flags operacionais e aprovadores especifico/padrao.

## 9. Contrato de atualizacao
`AtualizarConfiguracaoRegraAprovacaoRequest` replica o shape administrativo da criacao e inclui `Ativo`, permitindo edicao completa sem introduzir comportamento de versionamento automatico neste item.

## 10. Contrato de detalhe/resposta
`ConfiguracaoRegraAprovacaoResponse` expoe a visao administrativa completa da regra, com ids, nomes descritivos opcionais, enums, criterios, aprovadores, vigencia e auditoria simples (`CriadoEm`, `AtualizadoEm`, `CriadoPorUsuarioId`, `AtualizadoPorUsuarioId`).

## 11. Contrato de listagem resumida
`ConfiguracaoRegraAprovacaoResumoResponse` oferece shape reduzido para grids administrativas com classificacao, efeito, fluxo, vigencia, prioridade, versao e situacao.

## 12. Contrato de filtro/pesquisa
`ListarConfiguracoesRegrasAprovacaoRequest` contempla termo textual, situacao, tipo, escopo, natureza, catalogo, categoria, subcategoria, efeito, fluxo, estrategia de aprovador, vigencia e paginacao/ordenacao.

## 13. Contrato de ativacao/inativacao
`AlterarStatusConfiguracaoRegraAprovacaoRequest` foi criado porque o projeto ja usa requests especificos de alteracao de status em modulos administrativos.

## 14. Contrato de validacao/simulacao conceitual
`ValidarConfiguracaoRegraAprovacaoRequest` e `ValidarConfiguracaoRegraAprovacaoResponse` foram criados apenas como fronteira futura para pre-validacao administrativa. Nao existe simulacao funcional implementada nesta etapa.

## 15. Campos contemplados nos contratos
Os contratos cobrem:
- identificacao: nome, descricao, versao, ordem, prioridade;
- criterios: natureza, tipo de solicitacao, catalogo, categoria, subcategoria, impacto, urgencia, prioridade minima, custo minimo, nivel de risco minimo;
- comportamento: exige aprovacao, bloqueante, permite reenvio, permite fallback, efeito operacional;
- aprovacao: tipo de fluxo, tipo de resolucao de aprovador, aprovador especifico, aprovador padrao, prazo de decisao;
- vigencia e situacao: vigente de, vigente ate, ativo;
- auditoria de resposta: criado/atualizado e usuario criador/atualizador.

## 16. Criterios de aplicacao contemplados
Os DTOs cobrem criterios por natureza ITSM, tipo de solicitacao, catalogo/servico, categoria, subcategoria, impacto, urgencia, prioridade, custo e risco, em linha com a entidade existente.

## 17. Efeito operacional contemplado
Os contratos usam `EfeitoOperacionalRegraAprovacao` diretamente, sem criar uma abstracao paralela.

## 18. Bloqueante versus informativa nos contratos
Os requests expoem `Bloqueante`, `ExigeAprovacao` e `EfeitoOperacional`, permitindo diferenciar regra informativa, regra que exige aprovacao sem bloqueio e regra que exige aprovacao com bloqueio.

## 19. Tipo de fluxo nos contratos
Os requests e responses expoem `TipoFluxoAprovacao`, preservando compatibilidade futura com fluxo simples, sequencial, paralelo e multi-nivel.

## 20. Estrategia de aprovador nos contratos
Os DTOs usam `TipoResolucaoAprovadorRegraAprovacao` e suportam `AprovadorEspecificoUsuarioId` e `AprovadorPadraoUsuarioId`.

## 21. Vigencia, prioridade e versao nos contratos
Todos os contratos administrativos principais contemplam prioridade, versao e vigencia, preparando o modulo para gestao futura de publicacao/ativacao sem implementar workflow agora.

## 22. Compatibilidade com custo e risco futuros
`CustoMinimo` e `NivelRiscoMinimo` foram mantidos como opcionais, sem criar modelagem financeira ou de risco adicional.

## 23. Compatibilidade com grupo aprovador futuro
Nao foi criado `GrupoAprovadorId` porque o dominio ainda nao possui entidade real para grupo aprovador no motor. A compatibilidade futura fica documentada para item posterior, evitando contrato que exponha capacidade nao implementada.

## 24. Relacao com `ConfiguracaoRegraAprovacao`
Os DTOs refletem a persistencia atual da entidade, mas permanecem na camada de aplicacao, evitando expor diretamente o tipo de dominio em bordas futuras.

## 25. Relacao futura com servico de aplicacao
O item 36 devera consumir esses contratos como entrada/saida dos casos de uso administrativos de configuracao de aprovacao.

## 26. Relacao futura com endpoints administrativos
O item 43 podera usar esses contratos como payload de API administrativa sem necessidade de redefinir o shape basico.

## 27. Relacao futura com frontend
Os contratos foram desenhados para suportar uma tela administrativa futura com listagem, formulario de criacao/edicao, filtros e validacao preliminar.

## 28. Validacoes simples previstas
Foram adicionadas validacoes simples de contrato para:
- nome obrigatorio e tamanho maximo;
- versao positiva;
- ordem e prioridade nao negativas;
- subcategoria dependente de categoria;
- custo nao negativo;
- risco e prazo positivos quando informados;
- vigencia final nao anterior a inicial;
- regra informativa nao bloqueante;
- regra bloqueante exigindo aprovacao;
- coerencia entre estrategia de aprovador e aprovador especifico/padrao.

## 29. Riscos de seguranca e governanca
- Acoplamento excessivo dos DTOs ao estado atual da entidade.
- Exposicao prematura de campos que podem ser interpretados como comportamento funcional pronto.
- Duplicacao indevida de regra de negocio fora do dominio se os validators crescerem alem do papel de saneamento de entrada.
- Expectativa de suporte a grupo aprovador real antes da modelagem adequada.

## 30. Decisoes adiadas para proximos itens
- Servico de aplicacao completo para regras de aprovacao.
- Validacoes funcionais de negocio e simulacao real.
- Resolucao real de aprovador.
- Grupo aprovador real.
- Versionamento automatico.
- Workflow de publicacao/ativacao.
- Endpoints administrativos.
- Frontend administrativo.
- Auditoria operacional detalhada de alteracao de regra.

## 31. Conclusao tecnica
O item 34 foi concluido com contratos administrativos suficientes para criacao, edicao, consulta, listagem, alteracao de status e validacao conceitual de regras de aprovacao, sem alterar o modelo persistente e sem antecipar a implementacao funcional do motor.

## 32. Proxima etapa recomendada
Executar o item 35: criar contratos de decisao de aprovacao.
