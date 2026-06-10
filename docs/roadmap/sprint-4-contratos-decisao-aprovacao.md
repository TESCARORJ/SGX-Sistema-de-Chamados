# Sprint 4 - Contratos de decisao de aprovacao

## 1. Objetivo da criacao dos contratos
Definir a fronteira de entrada e saida da camada de aplicacao para registro, consulta e validacao conceitual de decisoes de aprovacao futuras, usando `DecisaoAprovacaoChamado` como entidade central e preservando contexto de `InstanciaAprovacaoChamado` e `EtapaAprovacaoChamado`.

## 2. Limites desta etapa
- Criar apenas contratos/DTOs e validators simples.
- Nao implementar servico de aplicacao completo.
- Nao implementar motor, workflow funcional ou consolidacao.
- Nao criar endpoint, controller, frontend ou service frontend.
- Nao criar migration estrutural.

## 3. Contexto estrutural ja existente
O motor de aprovacao ja possui configuracao, instancia, etapa e decisao formal persistidas no dominio e mapeadas no EF Core. O item 34 ja criou contratos administrativos para configuracao de regra.

## 4. Entidade base usada pelos contratos
`DecisaoAprovacaoChamado` e a base principal dos contratos desta etapa.

## 5. Entidades relacionadas usadas pelos contratos
- `InstanciaAprovacaoChamado`
- `EtapaAprovacaoChamado`
- `AprovacaoChamado` legado, apenas como referencia de compatibilidade futura

## 6. Enums usados pelos contratos
- `TipoDecisaoAprovacaoChamado`
- `ResultadoDecisaoAprovacaoChamado`
- `StatusInstanciaAprovacaoChamado`
- `StatusEtapaAprovacaoChamado`
- `EfeitoOperacionalRegraAprovacao`
- `TipoFluxoAprovacao`

## 7. Padrao de contratos encontrado no projeto
O projeto concentra DTOs administrativos em arquivos `Admin*Dtos.cs` dentro de `Application/DTOs/Admin`, usando requests em classes e responses em records/classes. As validacoes de entrada ficam em `Application/Validators` com `FluentValidation`.

## 8. Contratos criados
- `ListarDecisoesAprovacaoChamadoRequest`
- `RegistrarDecisaoAprovacaoChamadoRequest`
- `AprovarAprovacaoChamadoRequest`
- `ReprovarAprovacaoChamadoRequest`
- `CancelarDecisaoAprovacaoChamadoRequest`
- `RegistrarExpiracaoAprovacaoChamadoRequest`
- `SolicitarReavaliacaoAprovacaoChamadoRequest`
- `ValidarDecisaoAprovacaoChamadoRequest`
- `ValidarDecisaoAprovacaoChamadoResponse`
- `DecisaoAprovacaoChamadoResumoResponse`
- `DecisaoAprovacaoChamadoResponse`

## 9. Contrato de registro de decisao
`RegistrarDecisaoAprovacaoChamadoRequest` concentra o shape mais completo, cobrindo instancia obrigatoria, etapa opcional, tipo, resultado, decisor, autoridade, snapshots, flags operacionais, status anteriores/novos e contexto estrutural da etapa e da regra.

## 10. Contrato de aprovacao
`AprovarAprovacaoChamadoRequest` representa o payload futuro para aprovacao administrativa, com foco em instancia, etapa opcional, decisor, narrativa, escopo decidido e flags de liberacao/bloqueio.

## 11. Contrato de rejeicao/reprovacao
`ReprovarAprovacaoChamadoRequest` representa o payload futuro para reprovar, exigindo justificativa e preservando escopo, observacao, bloqueio, reavaliacao e cancelamento do fluxo.

## 12. Contrato de cancelamento
`CancelarDecisaoAprovacaoChamadoRequest` foi criado para separar o evento de cancelamento de uma decisao de merito, exigindo motivo.

## 13. Contrato de expiracao
`RegistrarExpiracaoAprovacaoChamadoRequest` foi criado para preparar o registro administrativo de expiracao, com data obrigatoria, motivo e contexto do responsavel/componente.

## 14. Contrato de reavaliacao
`SolicitarReavaliacaoAprovacaoChamadoRequest` foi criado para representar solicitacao futura de reavaliacao, sem executar workflow.

## 15. Contrato de detalhe/resposta
`DecisaoAprovacaoChamadoResponse` expoe a visao administrativa completa da decisao, incluindo contexto da instancia, etapa, decisor, narrativa, efeito, flags operacionais, snapshots de status e snapshots estruturais.

## 16. Contrato de listagem/resumo
`DecisaoAprovacaoChamadoResumoResponse` oferece shape reduzido para grids e historicos resumidos.

## 17. Contrato de filtro/pesquisa
`ListarDecisoesAprovacaoChamadoRequest` contempla instancia, etapa, chamado, decisor, tipo, resultado, efeito, flags de decisao, periodo, termo e paginacao/ordenacao.

## 18. Contrato de validacao/simulacao conceitual
`ValidarDecisaoAprovacaoChamadoRequest` e `ValidarDecisaoAprovacaoChamadoResponse` foram criados para permitir pre-validacao administrativa futura sem executar a decisao.

## 19. Campos contemplados nos contratos
Os contratos cobrem:
- identificacao da decisao, instancia, etapa e chamado;
- tipo, resultado e efeito operacional;
- decisor, papel, autoridade e marcadores de origem;
- grupo, quorum e delegacao como snapshots/flags;
- justificativa, observacao e escopo decidido;
- parcial/final, liberacao, bloqueio, reavaliacao, nova solicitacao e cancelamento de fluxo;
- status anterior/novo da instancia e da etapa;
- snapshots de status do chamado;
- snapshots de nivel, ordem, ramo e regra.

## 20. Tipo e resultado da decisao nos contratos
Tipo e resultado foram mantidos separados, como no dominio, para evitar confusao entre natureza do ato e resultado reconhecido.

## 21. Decisor e autoridade nos contratos
Os contratos expoem `DecisorUsuarioId`, `PapelDecisorSnapshot`, `AutoridadeDecisorSnapshot` e flags de aprovador especifico, aprovador padrao, membro de grupo e delegacao futura.

## 22. Justificativa e observacao nos contratos
Todos os contratos relevantes suportam `Justificativa` e `Observacao`, com obrigatoriedade adicional de justificativa para reprovacao e motivo para cancelamento/expiracao.

## 23. Escopo decidido nos contratos
`EscopoDecididoSnapshot` foi mantido nos contratos para registrar o recorte formal da decisao sem executar consolidacao.

## 24. Efeito operacional nos contratos
`RegistrarDecisaoAprovacaoChamadoRequest` e os responses usam `EfeitoOperacionalRegraAprovacao` para refletir o efeito reconhecido, sem aplicar esse efeito operacionalmente.

## 25. Decisao parcial versus final nos contratos
As flags `DecisaoParcial` e `DecisaoFinal` foram preservadas para suportar cenarios simples, sequenciais, paralelos e multinivel.

## 26. Liberacao, bloqueio e reavaliacao nos contratos
As flags `LiberaAvanco`, `MantemBloqueio`, `ExigeReavaliacao`, `PermiteNovaSolicitacao` e `CancelaFluxo` foram mantidas como dados de intencao/auditoria, nao como execucao funcional.

## 27. Compatibilidade com decisao direta na instancia
Os contratos permitem `EtapaAprovacaoChamadoId` nulo, suportando aprovacao simples diretamente na instancia.

## 28. Compatibilidade com decisao por etapa
Os contratos permitem `EtapaAprovacaoChamadoId` preenchido, com snapshots de nivel, ordem e ramo para cenarios por etapa.

## 29. Compatibilidade com aprovacao simples
A aprovacao simples pode usar decisao direta na instancia ou decisao em etapa unica, sem exigir workflow adicional.

## 30. Compatibilidade com aprovacao sequencial
Os contratos carregam dados de nivel, ordem e status de etapa, mas nao executam avancÌ§o sequencial.

## 31. Compatibilidade com aprovacao paralela
Os contratos carregam ramo, quorum e grupo em forma de snapshot, mas nao consolidam ramos.

## 32. Compatibilidade com aprovacao multinivel
Os contratos suportam decisao parcial/final e contexto de etapa, mas nao consolidam niveis.

## 33. Compatibilidade futura com grupo aprovador
Foi mantido `GrupoAprovadorSnapshot` e o marcador `DecisorEhMembroGrupo`, sem criar entidade de grupo aprovador real.

## 34. Compatibilidade futura com quorum
`QuorumEsperado` e `QuorumAtingido` estao presentes apenas como dados de contexto e auditoria.

## 35. Compatibilidade futura com delegacao
`DecisorPorDelegacao` permanece como flag preparada para evolucao futura, sem implementar delegacao.

## 36. Relacao com `DecisaoAprovacaoChamado`
Os contratos espelham a entidade de dominio, mas permanecem na camada de aplicacao como fronteira administrativa.

## 37. Relacao com `InstanciaAprovacaoChamado`
`InstanciaAprovacaoChamadoId` e obrigatorio nos contratos principais porque toda decisao deve apontar para uma instancia.

## 38. Relacao com `EtapaAprovacaoChamado`
`EtapaAprovacaoChamadoId` e opcional nos contratos para suportar tanto decisao direta na instancia quanto decisao por etapa.

## 39. Relacao com `AprovacaoChamado` legado
Nao houve alteracao no legado. Os contratos novos atendem somente a trilha nova do motor e nao migram decisoes antigas.

## 40. Relacao futura com servico de aplicacao
O item 36 e os itens funcionais posteriores deverao consumir esses contratos para registrar decisoes, validar permissao e executar comportamento real.

## 41. Relacao futura com endpoints de aprovacao/rejeicao
Os contratos ja estao preparados para endpoints futuros de aprovacao, reprovacao, cancelamento, expiracao, reavaliacao e historico.

## 42. Relacao futura com frontend
Os contratos ja suportam futuras telas de historico, aprovacao, reprovacao e validacao de decisao sem exigir remodelagem de payload.

## 43. Validacoes simples previstas
Foram adicionadas validacoes simples para:
- instancia obrigatoria;
- ids opcionais validos quando informados;
- compatibilidade entre tipo e resultado;
- justificativa obrigatoria para reprovacao;
- motivo obrigatorio para cancelamento e expiracao;
- quorum coerente;
- versao, nivel e ordem snapshot validos;
- impossibilidade de `LiberaAvanco` com `MantemBloqueio`;
- impossibilidade de `DecisaoParcial` com `DecisaoFinal`;
- snapshots de etapa apenas quando houver etapa.

## 44. Riscos de seguranca e governanca
- Contratos parecerem executar decisao antes do servico existir.
- Flags operacionais serem interpretadas como efeito funcional automatico.
- Antecipacao de quorum, grupo ou delegacao como capacidade real.
- Crescimento indevido das validacoes de contrato para dentro da regra de negocio.

## 45. Decisoes adiadas para proximos itens
- Servico de aplicacao para registrar decisao.
- Aprovar, reprovar, cancelar, expirar e reavaliar de forma funcional.
- Consolidacao de etapa e instancia.
- Calculo de quorum.
- Delegacao real.
- Grupo aprovador real.
- Alteracao funcional de status.
- Bloqueio/liberacao operacional.
- Endpoints administrativos.
- Frontend de decisao.
- Auditoria operacional completa.

## 46. Conclusao tecnica
O item 35 foi concluido com contratos administrativos suficientes para registrar, consultar e validar conceitualmente decisoes de aprovacao futuras, preservando compatibilidade com instancia simples e etapa, sem introduzir workflow funcional nesta etapa.

## 47. Proxima etapa recomendada
Executar o item 36: criar servico de aplicacao para regras de aprovacao.
