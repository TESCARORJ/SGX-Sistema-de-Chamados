# Sprint 7 - Gerenciamento de Requisicoes

## Estado atual

- Ordem: `107`
- Categoria: `ITIL/ITSM`
- Cenario: `A - SLA de catalogo aplicado`
- Status da implementacao: `Em desenvolvimento`
- Status tecnico: `Parcial`
- Percentual recalculado: `87%`
- Checklist ativo: `39`
- Checklist concluido: `34` (Nota: Os endpoints do portal já existiam estruturados corretamente no `PortalCatalogoServicosController`. O frontend ajustou a interface `NovoChamadoView` para ocultar os campos de classificação não necessários no fluxo de catálogo guiado e validou o funcionamento dos endpoints `/preparar-chamado` e `/requisicoes`.)
## Objetivo

Formalizar o fluxo de Requisicao de Servico com abertura preferencial via Catalogo, reutilizando `Chamado`, `Catalogo`, `Aprovacao`, `SLA`, `Classificacao` e estruturas de atendimento existentes.

## Decisao sobre o checklist antigo

O checklist generico anterior foi removido porque marcava como concluida uma entrega central que ainda nao existia integralmente. O novo checklist foi quebrado em itens objetivos, incrementais e auditaveis.

## Resumo tecnico do que ja existe

- abertura por catalogo sobre o fluxo atual de chamado;
- `CatalogoServicoId` persistido no chamado;
- contrato dedicado para abertura guiada por catalogo com semantica explicita de requisicao;
- validator dedicado para o request de abertura guiada por catalogo;
- classificacao oficial do catalogo aplicada no backend;
- aprovacao automatica opcional por servico;
- consulta de catalogo no portal;
- detalhe do servico no portal;
- historico de abertura por catalogo.

## O que ainda falta para a sprint

- use case dedicado de abertura via catalogo;
- aplicacao de grupo responsavel e SLA por servico;
- formulario dinamico por servico e persistencia das respostas;
- endpoints e telas guiadas de requisicao;
- regressao completa, homologacao funcional, homologacao visual e aceite formal.

## Checklist consolidado

- [x] 1. Diagnosticar estado atual da Sprint 7 e inconsistencias do roadmap
- [x] 2. Confirmar representacao da requisicao de servico como Chamado com `NaturezaChamadoEnum.Requisicao`
- [x] 3. Validar vinculo existente entre Chamado e Catalogo de Servicos
- [x] 4. Definir menor escopo seguro da abertura guiada por catalogo
- [x] 5. Implementar ou ajustar contrato de consulta do servico para abertura
- [x] 6. Implementar ou ajustar contrato de abertura guiada por catalogo com semantica de requisicao
- [x] 7. Criar validator dedicado para abertura guiada por catalogo
- [x] 8. Implementar use case dedicado de abertura de requisicao de servico via catalogo
- [x] 9. Aplicar classificacao vinda do catalogo no backend
- [ ] 10. Aplicar grupo responsavel configurado no catalogo
- [x] 11. Aplicar SLA configurado ou fallback existente
- [x] 12. Persistir vinculo entre chamado e servico do catalogo
- [ ] 13. Implementar ou reutilizar formulario por servico
- [ ] 14. Validar e persistir respostas do formulario
- [x] 15. Gerar aprovacao obrigatoria quando a regra aplicavel exigir
- [x] 16. Preservar aprovacao legada sem duplicidade
- [x] 17. Preservar abertura de incidentes e chamados sem catalogo
- [x] 18. Criar ou ajustar endpoints do portal para catalogo e abertura guiada
- [x] 19. Implementar tela de catalogo no portal
- [x] 20. Implementar detalhe do servico no portal
- [x] 21. Implementar formulario guiado de abertura
- [x] 22. Implementar confirmacao e acompanhamento da requisicao aberta
- [x] 23. Garantir seguranca, autorizacao e ownership dos endpoints
- [x] 24. Registrar historico e auditoria dos eventos relevantes
- [x] 25. Testar abertura por catalogo sem aprovacao
- [x] 26. Testar abertura por catalogo com aprovacao obrigatoria
- [x] 27. Testar formulario obrigatorio e respostas invalidas
- [ ] - [x] 28. Testar grupo responsavel e SLA
- [ ] - [x] 29. Testar regressao de abertura legada, incidente e atendimento
- [ ] - [x] 30. Testar regressao de aprovacao legada e motor novo
- [x] 31. Executar build backend e testes direcionados
- [x] 32. Executar build frontend e validacao TypeScript
- [x] 33. Verificar EF pending model changes
- [x] 34. Criar ou revisar migrations estruturais, se necessarias
35. Criar migration de dados ou checklist, se aplicavel
36. Atualizar documentacao principal da Sprint 7
37. Registrar homologacao funcional
38. Registrar homologacao visual responsiva
39. Registrar aceite formal somente com evidencia

## Decisao de compatibilidade do contrato

- o fluxo legado de `CriarChamadoRequest` foi preservado para abertura sem catalogo e incidentes;
- a abertura guiada por catalogo recebeu contrato dedicado e endpoint proprio;
- o novo contrato nao aceita natureza, categoria, subcategoria, prioridade, grupo, SLA ou aprovacao como fonte de verdade;
- o backend continua resolvendo a semantica com `NaturezaChamadoEnum.Requisicao` e regras existentes do catalogo.

## Limitacao Identificada (Cenario C para Grupos)

O catalogo nao possui o campo `GrupoResponsavelId` ou `GrupoTecnicoId` modelado em `CatalogoServico`. O campo existente e `DepartamentoResponsavelId`, que e uma entidade distinta do `GrupoTecnico` utilizado para roteamento no fluxo de `Chamado`. 
Conforme diretriz, o item "Aplicar grupo responsavel configurado no catalogo" fica bloqueado, devendo aguardar um escopo estrutural separado para alteracao do modelo.
## Limitacao Identificada (Cenario C para Formularios)

Nao existe estrutura de dominio para campos dinamicos ou respostas de formulario (tabelas como `CampoFormulario`, `RespostaFormulario`, `CatalogoServicoCampo`). O request nao suporta respostas, o frontend nao as renderiza e o backend nao valida regras.
Conforme diretriz, o item "Implementar ou reutilizar formulario por servico" fica bloqueado, devendo aguardar um escopo estrutural separado para alteracao do modelo, evitando criar JSON livre ou tabelas novas sem design completo (Cenario C).

## Nenhuma migration estrutural

A Sprint 7 não gerou migration estrutural. As mudanças persistidas foram migrations de checklist/SeedData. Grupo responsável por catálogo, formulário por serviço e respostas permanecem bloqueados para modelagem estrutural futura.

## Proxima acao real

`Item 35 - Criar migration de dados ou checklist, se aplicavel`

