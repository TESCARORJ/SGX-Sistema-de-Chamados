# Rejeitar Chamado pela Interface (Sprint 4 - Item 50)

## 1. Objetivo da ação de rejeição pela interface
Permitir que um aprovador rejeite (reprove) uma pendência de aprovação gerenciada pelo novo motor ITSM. A ação deve estar disponível na interface administrativa (listagem de pendências e detalhe do chamado), com confirmação, justificativa obrigatória e envio do respectivo contrato.

## 2. Limites desta etapa
- Implementada apenas a ação de **Rejeição/Reprovação**.
- **Não implementado** neste item: cancelamento, expiração, reavaliação ou geração manual de pendências.
- Focado apenas em consumir o endpoint de reprovação já existente no backend (`/api/admin/aprovacoes-motor/reprovar`).
- Não altera status do chamado diretamente pelo frontend, delegando isso ao backend conforme o efeito da regra configurada.

## 3. Telas analisadas
- `AdminAprovacoesPendentesView.vue`: Listagem global de pendências do aprovador.
- `AdminDetalheChamadoView.vue`: Detalhe do chamado com seção do Motor ITSM.

## 4. Componente/tela alterado
- `src/SGX.SistemaChamado.Web/src/views/AdminAprovacoesPendentesView.vue`
- `src/SGX.SistemaChamado.Web/src/views/AdminDetalheChamadoView.vue`

## 5. Service frontend alterado
- `src/SGX.SistemaChamado.Web/src/services/aprovacoesMotorService.ts`:
  - Adicionado o método `reprovarAprovacao(request: ReprovarAprovacaoChamadoRequest)`.

## 6. Types/interfaces criadas ou reutilizadas
Foram adicionados os tipos ao arquivo `src/SGX.SistemaChamado.Web/src/types/aprovacoesMotor.ts`:
- `ReprovarAprovacaoChamadoRequest`
- `ReprovarAprovacaoChamadoResponse`

## 7. Endpoint consumido
`POST /api/admin/aprovacoes-motor/reprovar`

## 8. Contrato enviado
```json
{
  "instanciaAprovacaoChamadoId": "<id-da-instancia>",
  "decisaoFinal": true,
  "justificativa": "<texto obrigatório>",
  "observacao": "<texto opcional>"
}
```

## 9. Fluxo visual de rejeição
1. O usuário visualiza o botão "Rejeitar" nas pendências em status `Pendente` ou `EmReavaliacao`.
2. Ao clicar, um modal "Rejeitar pendência" é aberto.
3. O usuário é obrigado a preencher a justificativa.
4. A observação é exibida como opcional.
5. Um indicador de loading trava os botões durante o processamento.
6. A listagem é recarregada para mostrar o status `Reprovado`.
7. O detalhe do chamado recarrega a si mesmo e a seção de pendências.
8. Mensagem de sucesso ou erro (Notificação) é exibida.

## 10. Confirmação antes da rejeição
Implementado via modal (`q-dialog`) com campo para justificar e observar.

## 11. Justificativa obrigatória
O frontend valida se o campo justificativa foi preenchido e exibe alerta visual (regras do q-input). O botão também verifica em javascript o `.trim()` e alerta antes do request.

## 12. Tratamento de reprovação por instância
O ID da instância é submetido como `instanciaAprovacaoChamadoId`. Por padrão no frontend atual, estamos reprovando a instância diretamente (aprovação simples), marcando `decisaoFinal: true`.

## 13. Tratamento de reprovação por etapa
Não foi submetido `etapaAprovacaoChamadoId` explicitamente, pois o frontend atua na instância atual da aprovação.

## 14. Tratamento de decisão parcial/final
Foi enviado `decisaoFinal: true` para manter alinhamento ao que foi feito no botão Aprovar (item 49).

## 15. Tratamento de loading
Gerenciado por variáveis `processingReprovar` / `processingReprovarMotor`.

## 16. Tratamento de sucesso
Via Quasar Notify com a mensagem: "Rejeição registrada com sucesso.". O componente chama `carregarAprovacoes()` ou `carregarAprovacoesMotor()` em seguida.

## 17. Tratamento de erro
Quasar Notify exibe "Não foi possível registrar a rejeição." com a mensagem original de erro se houver falha de rede ou de API (ex: 400 Bad Request).

## 18. Atualização da listagem após reprovação
Refetch da tabela chamando a função da listagem após fechar o modal.

## 19. Atualização do detalhe do chamado após reprovação
Refetch do chamado e da lista de pendências daquela aba de contexto, forçando o Vue a repintar e alertar (banners de bloqueio/sucesso) conforme a resposta do servidor.

## 20. Relação com endpoint do item 44
Consome os endpoints gerados e estruturados a partir daquela base de rotas administrativas.

## 21. Relação com pendências do item 47
Complementa o Item 47, garantindo que o status modificado seja exibido na listagem de Minhas Pendências.

## 22. Relação com detalhe do chamado do item 46
Modifica as opções visuais dentro da seção criada no item 46.

## 23. Relação com aprovação pela interface do item 49
Os modais de Aprovar e Rejeitar foram separados, com o de Rejeitar contendo uma validação extra de "Justificativa *" como obrigatório, e o Aprovar não contendo justificativa obrigatória.

## 24. Relação com configuração de regras do item 48
Ação final em decorrência da avaliação de uma regra originada pelo Motor. 

## 25. O que a interface não faz
- Não provê botões ou lógica de expiração, cancelamento e reavaliação.
- Não atualiza o estado primário do chamado via API de chamado, usando estritamente o endpoint de `/reprovar`.

## 26. Testes criados ou executados
- Validação efetuada com o teste de Roadmap (`RoadmapSprint4MotorAprovacoesChecklistTests`), ajustando o checklist e verificando percentuais (74% e 50 itens).
- O backend compila e o frontend compila.

## 27. Riscos de UX/governança
Mesmo cenário da Aprovação, os caracteres máximos são 500 no frontend, garantidos pelos rules do `q-input` e passíveis de validação pelo backend.

## 28. Decisões adiadas para próximos itens
Filtros e listagens para pendências canceladas, expiradas e abas distintas não foram tratadas, sendo endereçadas ao Item 51 e seguintes.

## 29. Conclusão técnica
A funcionalidade de reprovação atende à restrição de segurança e tracking exigidas pelas regras de aprovação. O modal garante coleta de insumo e evita clicks não intencionais.

## 30. Próxima etapa recomendada
Avançar para o **Item 51: Ajustar listagem/filtros para aprovação pendente**.
