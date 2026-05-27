# ADMIN-ATENDIMENTO-NATUREZA-ITSM

## 1. Objetivo
Adaptar a experiencia administrativa de chamados para:
- exibir NaturezaChamado, ImpactoChamado, UrgenciaChamado e PrioridadeChamado;
- limitar visualmente os status disponiveis conforme a natureza ITSM do chamado.

O backend continua como autoridade final das regras.

## 2. Exibicao na tela administrativa
### Listagem (`/admin/chamados`)
- coluna de classificacao ITSM com:
  - natureza;
  - impacto;
  - urgencia.

### Detalhe/atendimento (`/admin/chamados/:id`)
- bloco "Resumo do chamado" com:
  - Natureza ITSM;
  - Impacto;
  - Urgencia;
  - Prioridade (badge existente).

## 3. Como os status disponiveis sao obtidos
- o detalhe admin recebe `statusPermitidosCodigos` no DTO;
- essa lista e montada no backend via `IFluxoStatusChamadoService` (regra centralizada por natureza);
- no frontend, o modal de alterar status recebe apenas os status filtrados por `statusPermitidosCodigos`.

## 4. Backend como autoridade final
- o bloqueio visual no frontend evita selecao invalida na UI;
- a validacao real continua no backend (use cases administrativos de status/encerramento/reabertura);
- se houver tentativa invalida, a API retorna erro e a mensagem e exibida ao atendente.

## 5. Bloqueio visual x bloqueio real
- visual: select de status mostra somente opcoes compativeis;
- real: backend valida e rejeita transicoes incompativeis.

## 6. Limitacoes atuais
- a listagem prioriza exibicao compacta e pode resumir informacoes em telas menores;
- nao foram criadas acoes administrativas especificas por natureza nesta sprint;
- prioridade permanece exibida como valor calculado ja existente.

## 7. Pendencias futuras
- acoes especializadas por natureza ITSM;
- fluxo de aprovacao completo para mudanca;
- campos de causa raiz para problema;
- correlacao dedicada para evento/alerta;
- campos avancados condicionais por natureza no atendimento admin.
