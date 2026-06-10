# Aprovar Chamado pela Interface (Sprint 4 - Item 49)

## 1. Objetivo da ação de aprovação pela interface
Permitir que um aprovador (usuário do sistema com permissão de aprovação) consiga registrar sua decisão de "Aprovar" em uma pendência de aprovação gerenciada pelo novo motor ITSM, diretamente pelo frontend administrativo (listagem de pendências e detalhe do chamado), com registro de justificativa opcional.

## 2. Limites desta etapa
- Implementada apenas a ação de **Aprovação**.
- **Não implementado** neste item: rejeição (reprovar), cancelamento, reavaliação ou geração manual de pendências.
- Focado apenas em consumir o endpoint já existente no backend (`/api/admin/aprovacoes-motor/aprovar`).

## 3. Telas analisadas
- `AdminAprovacoesPendentesView.vue`: Listagem global de pendências do aprovador logado.
- `AdminDetalheChamadoView.vue`: Detalhe do chamado, exibindo pendências específicas (seção Motor ITSM).

## 4. Componente/tela alterado
- `src/SGX.SistemaChamado.Web/src/views/AdminAprovacoesPendentesView.vue`
- `src/SGX.SistemaChamado.Web/src/views/AdminDetalheChamadoView.vue`

## 5. Service frontend alterado
- `src/SGX.SistemaChamado.Web/src/services/aprovacoesMotorService.ts`:
  - Adicionado o método `aprovarAprovacao(request: AprovarAprovacaoChamadoRequest)`

## 6. Types/interfaces criadas ou reutilizadas
Foram adicionados ao arquivo `src/SGX.SistemaChamado.Web/src/types/aprovacoesMotor.ts`:
- `AprovarAprovacaoChamadoRequest`
- `AprovarAprovacaoChamadoResponse`

## 7. Endpoint consumido
`POST /api/admin/aprovacoes-motor/aprovar`

## 8. Contrato enviado
```json
{
  "instanciaAprovacaoChamadoId": "<id-da-instancia>",
  "decisaoFinal": true,
  "justificativa": "<texto opcional>",
  "observacao": "<texto opcional>"
}
```

## 9. Fluxo visual de aprovação
1. O usuário visualiza o botão "Aprovar" na linha da pendência.
2. O botão é exibido apenas para itens com status `Pendente` ou `EmReavaliacao`.
3. Ao clicar, abre-se um modal (Dialog).
4. O usuário preenche a observação opcional e confirma.
5. Um indicador de "Loading" é exibido durante a requisição.
6. Notificação de sucesso é renderizada.
7. O componente reflete as mudanças visualmente após recarregar.

## 10. Confirmação antes da aprovação
Implementado em ambas as telas via `q-dialog` com textarea para capturar `justificativa/observacao` e botões de "Cancelar" ou "Confirmar aprovação".

## 11. Tratamento de aprovação por instância
Neste momento a aprovação atua sobre o Id principal da Instância (`instanciaAprovacaoChamadoId`). O backend delega e aplica o estado corretamente para o fluxo primário.

## 12. Tratamento de aprovação por etapa
Não foi enviado o `etapaAprovacaoChamadoId` explicitamente, pois o frontend atua na aprovação simples da instância no momento, com `decisaoFinal: true`, alinhado com o esperado para a interface primária.

## 13. Tratamento de decisão parcial/final
Foi convencionado fixar `decisaoFinal: true` nos requests partindo desta UI para simplificar a governança nesta etapa do Roadmap, deixando workflows complexos para evoluções futuras.

## 14. Tratamento de loading
Gerenciado via variáveis reativas `processingAprovar` / `processingAprovarMotor`, travando os botões da UI para prevenir múltiplos disparos.

## 15. Tratamento de sucesso
Via `Quasar Notify` (mensagem "Aprovação registrada com sucesso.") e recarregamento da lista/seção.

## 16. Tratamento de erro
Via `Quasar Notify`, exibindo a mensagem retornada pelo servidor ou uma fallback de erro de rede.

## 17. Atualização da listagem após aprovação
A função `carregarAprovacoes()` é chamada novamente no modal on-close, garantindo refetch completo dos dados da listagem de pendências.

## 18. Atualização do detalhe do chamado após aprovação
A função `carregarAprovacoesMotor()` é chamada novamente. Além disso, `recarregarDetalhe()` também é disparada para atualizar o status e flags do chamado (caso o bloqueio tenha sido retirado).

## 19. Relação com endpoint do item 44
O botão conecta diretamente ao endpoint de aprovação concebido no Item 44.

## 20. Relação com pendências do item 47
Mantém compatibilidade com a listagem criada no item 47, adicionando nela o botão de ação.

## 21. Relação com detalhe do chamado do item 46
Mantém compatibilidade com a seção renderizada no item 46, agregando nela a capacidade acionável, em vez de visualização passiva.

## 22. Relação com configuração de regras do item 48
Funciona downstream da configuração: a regra foi configurada no item 48, gerou pendência, e o item 49 aprova.

## 23. O que a interface não faz
- Não possui botão de "Reprovar".
- Não bloqueia nem desbloqueia o chamado no frontend de forma isolada do backend.
- Não avalia regras dinamicamente.

## 24. Testes criados ou executados
- Tests backend: Teste de roadmap consistido (`RoadmapSprint4MotorAprovacoesChecklistTests`). Valida a progressão do projeto (49/68 - 72%).
- Build frontend validado no Vite sem erros no modo produção.
- Banco atualizado via EF Migration de verificação do checklist do Roadmap.

## 25. Riscos de UX/governança
A injeção de justificada (textarea livre) não possui limite semântico rígido pelo frontend além de `500 chars`. Requer verificação rígida no backend (já implementada em sprints anteriores).

## 26. Decisões adiadas para próximos itens
A funcionalidade de "Reprovar" foi inteiramente isolada e designada para o Item 50.

## 27. Conclusão técnica
A funcionalidade de aprovação simples atende aos requisitos de UX e segurança exigidos no motor do Sistema de Chamados para este item do checklist da Sprint 4.

## 28. Próxima etapa recomendada
O roadmap agora avança para o **Item 50: Permitir rejeitar chamado pela interface**.
