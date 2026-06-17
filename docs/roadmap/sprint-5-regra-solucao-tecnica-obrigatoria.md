# Regra de Solução Técnica Obrigatória (Sprint 5)

## 1. Objetivo da Regra
Garantir que nenhum chamado possa ser resolvido tecnicamente sem o registro formal de uma solução. Esta regra solidifica a governança de ITSM, impedindo que a equipe técnica avance o ciclo de vida do chamado para "Resolvido" utilizando apenas comentários comuns vazios ou descrições incompletas.

## 2. Diferença entre Solução Técnica e Comentário Comum
- **Comentário Comum:** Interações diárias de acompanhamento, dúvidas, registro de passos e trocas de informações com o usuário. Não possui valor definitivo para fechamento de auditoria e não altera o status por si só.
- **Solução Técnica:** O registro definitivo (root cause / fix applied) da ação que restabeleceu o serviço ou atendeu a requisição do usuário. É armazenado estruturalmente em conjunto com a ação que aciona a pausa de SLAs pendentes e habilita o chamado para futuro Aceite e Fechamento.

## 3. Onde a Regra foi Aplicada
A regra foi implementada em multicamadas, garantindo que não haja dependência apenas de restrições de interface ou API:
- **Camada de Domínio (`Chamado.cs`):** O método `Resolver(Guid statusResolvidoId, string solucaoTecnica, string atualizadoPor)` foi atualizado para lançar uma exceção de domínio `ArgumentException` se a `solucaoTecnica` for nula ou composta apenas de espaços vazios.
- **Camada de Validação (`ResolverChamadoRequestValidator.cs`):** Validação antecipada (Fail Fast) na fronteira de API usando FluentValidation (não permite campo nulo, vazio, e impõe limites de caracteres).
- **Camada de Aplicação (`ResolverChamadoUseCase.cs`):** O UseCase foi integrado para receber o objeto validado e repassá-lo ao Domínio, convertendo simultaneamente a solução em um comentário formal e registrando-o no Histórico.

## 4. Comportamento Esperado em Caso Válido
- A validação flui, o método de domínio é acionado.
- O campo interno `ResolvidoEm` da entidade `Chamado` é atualizado.
- O SLA pendente referente ao atendimento do chamado é pausado.
- Um registro em `ComentarioChamado` e `HistoricoChamado` (do tipo `Resolvido`) são criados vinculando a solução e o técnico responsável.

## 5. Comportamento Esperado em Caso Inválido
- **API (Requisição HTTP):** Retornará bad request genérico pelo `FluentValidation` capturado no pipeline.
- **Uso Direto do UseCase:** Lançará uma exceção do tipo `ArgumentException` pela validação do domínio ou da API.
- O campo `ResolvidoEm` **NÃO** é preenchido.
- O SLA **NÃO** é pausado.
- Histórico do ciclo resolvido **NÃO** é gerado.

## 6. Integração com Status Resolvido e Fechamento Definitivo Futuro
Conforme as premissas iniciadas no Item 6 do Roadmap, a resolução é apenas a indicação técnica de finalização. A regra criada aqui é o "pedágio" necessário para entrar neste estado de resolução.
O chamado continuará pendente de "Fechamento Definitivo" (que pode ocorrer por meio de ação do Aceite pelo solicitante, ou por prazo através de um Worker de auto-fechamento), etapas ainda a serem implementadas nas próximas sprints.

## 7. Integração com SLA e Aprovação Pendente Bloqueante
- A resolução não impacta no fluxo das aprovações pendentes (se o chamado precisar de aprovação, ele lançará a exceção garantida pelo método de bloqueio `ValidarBloqueioMovimentacaoAprovacaoPendenteUseCase` implementado na Sprint 4).
- Apenas mediante aprovação e solução preenchida é que o SLA será pausado na fase "Resolvido".

## 8. Testes Criados / Ajustados
Novos testes unitários adicionados e validados na suíte `ResolverChamadoUseCaseTests`:
- `NaoDeveResolverChamadoComSolucaoTecnicaNula`
- `NaoDeveResolverChamadoComSolucaoTecnicaVazia`
- `NaoDeveResolverChamadoComSolucaoTecnicaSomenteEspacos`
- `NaoDeveAlterarStatusQuandoSolucaoTecnicaInvalida`
- `NaoDevePreencherResolvidoEmQuandoSolucaoTecnicaInvalida`
- `DeveResolverChamadoComSolucaoTecnicaValida` (Já Existente / Mantido)
- `DeveRegistrarHistoricoDeResolucao` (Já Existente / Mantido)

## 9. O Que Não Foi Alterado
Nenhuma alteração indevida nos fluxos legados de *Fechamento/Aceite/Rejeição* foi realizada nesta fase para preservar o funcionamento do ambiente em produção. 

## 10. Riscos e Decisões Adiadas
Atualmente a solução técnica está sendo gravada apenas na tabela de `comentarios_chamados` sob responsabilidade do técnico, sem possuir uma tabela/coluna isolada no EF Core `Chamado`. O domínio protegeu a ação, mas do ponto de vista do repositório, continua existindo na relação 1-N. Decidiu-se por manter esta arquitetura para não gerar grandes quebras no front-end, contudo, é recomendado avaliar se seria vantajoso desacoplar "Comentários" de "Solução Oficial" futuramente, para exibi-los em blocos visuais separados.
