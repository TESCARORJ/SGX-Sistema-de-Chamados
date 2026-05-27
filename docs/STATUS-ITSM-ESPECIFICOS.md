# STATUS ITSM ESPECIFICOS

## 1. Objetivo
Expandir os status operacionais do chamado para suportar evolucao de processos ITSM por natureza, sem implementar workflow completo de mudanca, problema, evento/alerta ou automacoes externas nesta etapa.

## 2. Status preservados
Os status ja existentes foram preservados:
- `Aberto`
- `EmAtendimento`
- `AguardandoSolicitante`
- `Resolvido`
- `Encerrado`
- `Cancelado`

## 3. Status adicionados
Foram adicionados os seguintes status especificos:

Mudanca:
- `EmAnalise`
- `AguardandoAprovacao`
- `Aprovada`
- `Reprovada`
- `EmExecucao`
- `Concluida`

Problema:
- `CausaRaizIdentificada`
- `SolucaoDeContorno`

EventoAlerta:
- `Correlacionado`
- `Tratado`

TarefaOperacional:
- `Planejada`

## 4. Valores numericos preservados
Os codigos legados foram mantidos sem alteracao:
- `Aberto = 1`
- `EmAtendimento = 2`
- `AguardandoSolicitante = 3`
- `Resolvido = 4`
- `Encerrado = 5`
- `Cancelado = 6`

Novos codigos adicionados no final:
- `EmAnalise = 7`
- `AguardandoAprovacao = 8`
- `Aprovada = 9`
- `Reprovada = 10`
- `EmExecucao = 11`
- `Concluida = 12`
- `CausaRaizIdentificada = 13`
- `SolucaoDeContorno = 14`
- `Correlacionado = 15`
- `Tratado = 16`
- `Planejada = 17`

## 5. Matriz de status por natureza
Fonte unica: `IFluxoStatusChamadoService` / `FluxoStatusChamadoService`.

- `Incidente`: `Aberto`, `EmAtendimento`, `AguardandoSolicitante`, `Resolvido`, `Encerrado`, `Cancelado`
- `Requisicao`: `Aberto`, `EmAtendimento`, `AguardandoSolicitante`, `Resolvido`, `Encerrado`, `Cancelado`
- `Mudanca`: `Aberto`, `EmAnalise`, `AguardandoAprovacao`, `Aprovada`, `Reprovada`, `EmExecucao`, `Concluida`, `Encerrado`, `Cancelado`
- `Problema`: `Aberto`, `EmAnalise`, `CausaRaizIdentificada`, `SolucaoDeContorno`, `Resolvido`, `Encerrado`, `Cancelado`
- `EventoAlerta`: `Aberto`, `EmAnalise`, `Correlacionado`, `Tratado`, `Encerrado`, `Cancelado`
- `TarefaOperacional`: `Aberto`, `Planejada`, `EmExecucao`, `Concluida`, `Encerrado`, `Cancelado`

## 6. Impacto em SLA
Configuracao conservadora aplicada em seed para `EhStatusFinal` e `PausaSla`:
- status finais: `Encerrado`, `Cancelado`, `Resolvido`, `Concluida`, `Reprovada`, `Tratado`;
- pausa de SLA mantida para `AguardandoSolicitante`;
- `AguardandoAprovacao` configurado com pausa de SLA para manter comportamento conservador de espera.

Nao houve mudanca estrutural no motor de SLA; a regra continua centralizada no backend e coberta por testes.

## 7. Impacto em acoes disponiveis
`IAcoesChamadoService` / `AcoesChamadoService` passou a considerar status final para bloquear acoes operacionais e liberar `Reabrir` quando o perfil/permissao permitir.

Nao foi implementado fluxo funcional completo para:
- aprovacao real de mudanca;
- causa raiz estruturada;
- correlacao real de eventos;
- conclusao especifica de tarefa operacional.

## 8. Limitacoes atuais
- Nao existe CAB nem orquestracao de aprovacao por etapa.
- Nao existe workflow completo de problema com artefatos formais de RCA.
- Nao existe correlacao automatica real para eventos/alertas externos.
- Nao existe automacao externa para transicoes por integracoes.

## 9. Pendencias futuras
- aprovacao real de mudanca;
- workflow completo de mudanca;
- causa raiz estruturada;
- solucao de contorno estruturada;
- correlacao real de eventos;
- conclusao especifica de tarefa operacional.
