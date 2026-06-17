# Sprint 5 - Separar status Resolvido e Fechado no fluxo de negócio

## 1. O que foi alterado
- **Domínio (`Chamado.cs`)**: Adicionada a propriedade `ResolvidoEm` (nullable) e o método `Resolver(Guid statusResolvidoId, string atualizadoPor)`. O novo método obriga a injeção do status e data atual (UTC) de forma consistente com a arquitetura DDD.
- **Enums**: Adicionado `Resolvido` no `TipoHistoricoChamado` e `Resolver` no `AcaoChamadoEnum`.
- **Casos de Uso (`ResolverChamadoUseCase.cs`)**: Implementado o fluxo que orquestra a transição de status para `Resolvido`. Este caso de uso inclui as regras para geração do comentário técnico (com flag para interno ou não) que atesta a solução do chamado. O relógio de SLA é pausado/congelado via `RegistrarEncerramentoAsync`.
- **DTO e Validação (`ResolverChamadoRequest`)**: Novo payload com o campo `Solucao` e a flag `ComentarioInterno`. A solução foi tornada obrigatória (não vazia) através do validador.
- **Banco de Dados**: Criada migration estrutural (`SepararStatusResolvidoFechado`) para adicionar a coluna `ResolvidoEm` e migration para o roadmap para marcar o item como concluído na Seed do EF.

## 2. O que não foi alterado
- O fechamento definitivo (`EncerrarChamadoUseCase.cs`) foi mantido intacto. Ele continuará operando o status "Encerrado" até ser complementado pelas futuras tarefas (aceite/rejeição).
- O motor de aprovações da Sprint 4 não foi modificado em seu núcleo, mas foi referenciado como regra impeditiva de avanço, garantindo integração suave.
- Nenhum endpoint existente (como `/encerrar`) teve comportamento regressivo.

## 3. Diferença entre Resolver e Fechar
- **Resolver**: O atendente afirma ter entregue a solução técnica ou paliativa (workaround). O chamado tem o relógio SLA pausado e passa ao status `Resolvido`. Ainda passível de rejeição/reabertura ou aceite futuro pelo usuário.
- **Fechar**: O solicitante (ou o sistema por decurso de prazo) aceitou a solução e o chamado atinge o estado final. Não deverá ser alterado.

## 4. Endpoint Criado
`POST /api/admin/chamados/{id}/resolver`
Recebe:
```json
{
  "solucao": "Servidor reiniciado e RAM estendida.",
  "comentarioInterno": false
}
```

## 5. Campos Adicionados
- `ResolvidoEm` (tabela `Chamados`) - Data/Hora (UTC) do momento da resolução técnica.

## 6. Regras de Negócio
- Não se pode resolver um chamado sem fornecer a `Solucao`.
- Não se pode resolver um chamado se houver alguma `Aprovacao` pendente do tipo bloqueante.
- O ato de resolver registra na linha do tempo um comentário contendo a solução, para transparência.
- A auditoria rastreará a transição para "Resolvido".

## 7. Riscos e Decisões Adiadas
- **Risco**: Impacto visual mínimo até a criação da UI no Vue.js para este status.
- **Adiamento**: O comportamento de "Aceite" e "Rejeição" por parte do solicitante fica postergado para as próximas tarefas da sprint.

## 8. Relação com Aceite, Rejeição e Auto-fechamento (Próximas etapas)
Este item pavimenta a base de dados. Os próximos itens focarão na criação da interface do usuário para consumir este novo endpoint e os gatilhos (trigers/workers) para fechamento automático.
