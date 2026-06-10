# Ajustar listagem/filtros para aprovação pendente (Sprint 4 - Item 51)

## 1. Objetivo dos ajustes de listagem/filtros
Melhorar a listagem de pendências de aprovação do motor ITSM, permitindo que o administrador ou aprovador encontre as solicitações relevantes utilizando diversos critérios, além de destacar visualmente urgências e prazos de expiração.

## 2. Limites desta etapa
- Apenas ajustes visuais de listagem e parâmetros de query string para consumo da API existente.
- Nenhuma nova regra de aprovação criada.
- Nenhuma mutação de dados (cancelamento, reavaliação ou expiração manuais).
- Botões de Aprovação/Rejeição preservados do item 49 e 50.

## 3. Tela analisada
- `AdminAprovacoesPendentesView.vue`

## 4. Componente/tela alterado
- `src/SGX.SistemaChamado.Web/src/views/AdminAprovacoesPendentesView.vue`

## 5. Service frontend alterado
Nenhum service alterado. O método `listarMinhasPendencias` já recebia parâmetros parciais (via `Record<string, any>`) ou tipagem equivalente em `aprovacoesMotorService.ts`. O serviço apenas foi instruído a repassar novos filtros de query.

## 6. Types/interfaces alteradas ou reutilizadas
Foram reutilizados os enumeradores de `StatusInstanciaAprovacaoChamado` do arquivo `aprovacoesMotor.ts`.

## 7. Endpoints consumidos
- `GET /api/admin/aprovacoes-motor/pendencias/minhas` com os parâmetros extras de query suportados pelo `ListarInstanciasAprovacaoChamadoRequest`.

## 8. Filtros criados ou ajustados
- `termo`: Input para busca por ID do chamado ou nome da regra.
- `status`: Select para filtrar por status da instância.
- `apenasPendentes`: Toggle para listar apenas o que está travado (Pendente, EmReavaliacao).
- `apenasBloqueantes`: Toggle para listar apenas instâncias bloqueantes.

## 9. Ordenação criada ou ajustada
- `Mais recentes primeiro` (solicitadaem_desc)
- `Mais antigas primeiro` (solicitadaem_asc)
- `Vencimento mais próximo` (deveexpirarem_asc)

## 10. Paginação
A paginação já presente (através de `PaginacaoTabela`) foi mantida e os filtros agora interagem resetando a página para `1` quando um filtro muda.

## 11. Estados visuais
Foram inseridas labels condicionais de `Bloqueante`, `Informativa`, e cor/status para o vencimento.

## 12. Tratamento de pendência bloqueante
As instâncias bloqueantes possuem a flag `bloqueante: true` no retorno, e foi inserido um `<q-badge color="negative">Bloqueante</q-badge>` para destacá-las na tabela.

## 13. Tratamento de pendência informativa
Para instâncias não bloqueantes, inserimos `<q-badge color="info">Informativa</q-badge>`.

## 14. Tratamento de reavaliação
Status é colorido com `info` e texto `Em reavaliação`. Os botões de Aprovar e Rejeitar continuam habilitados para esse status.

## 15. Tratamento de vencidas
Baseado na comparação de data atual com `deveExpirarEm`, exibe tag `Vencida` na cor vermelha/negativa caso expirada no relógio local, ou exibe o status natural de Expirado vindo do banco.

## 16. Tratamento de próximas do vencimento
Compara `deveExpirarEm` com a data atual. Até 24h gera `Vence hoje` (orange-9), e até 72h gera `Prazo próximo` (warning).

## 17. Tratamento de estado vazio
Ajustado para "Nenhuma pendência encontrada com os filtros selecionados. Tente alterar os termos de busca ou limpar os filtros."

## 18. Tratamento de loading
Mantido o `<LoadingState>` inline.

## 19. Tratamento de erro
Mantido o `<ErrorState>` com botão de tentar novamente (retry chamando a API com os filtros atuais).

## 20. Preservação da aprovação pela interface
Modal mantido intacto conforme Item 49, e acionado apenas se o status estiver apto a aprovação.

## 21. Preservação da rejeição pela interface
Modal mantido intacto conforme Item 50.

## 22. Relação com item 47
Consolida e expande a listagem para aprovador que foi iniciada ali.

## 23. Relação com item 49
Os modais e a lógicas de aprovar (item 49) rodam o `carregarAprovacoes()` que, por sua vez, agora preserva os filtros atuais da view state.

## 24. Relação com item 50
A rejeição também força reload dos mesmos filtros ao finalizar com sucesso.

## 25. O que a tela não faz
- Não cria instâncias manuais de aprovação.
- Não expira ou reavalia forçadamente (não clica em um botão para mandar "expirar agora").

## 26. Testes/builds executados
Backend build (pass).
Frontend `npm run build` (pass).
Testes de roadmap (pass: `RoadmapSprint4MotorAprovacoesChecklistTests`).

## 27. Riscos de UX/governança
Filtros aplicados que não retornem nenhum resultado devem ficar claros ao usuário para que ele não ache que esvaziou a fila, sendo que na verdade há um filtro ativado. O botão "Limpar Filtros" previne esse bloqueio de UX.

## 28. Decisões adiadas para próximos itens
Filtros complexos como buscar por intervalo exato de data, tipo de fluxo, aprovador e regras de SLA ficarão para relatórios estendidos que fogem do escopo inicial de pendências simples.

## 29. Conclusão técnica
A listagem agora fornece visibilidade e filtros suficientes para a operação básica do aprovador dentro da UI Vue/Quasar sem expor complexidades não homologadas.

## 30. Próxima etapa recomendada
Testar regras de aprovação por natureza ITSM (Item 52).
