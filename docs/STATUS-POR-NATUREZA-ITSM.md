# STATUS POR NATUREZA ITSM

## Objetivo
Estabelecer uma regra centralizada no backend para validar quais status do chamado sao permitidos conforme a `NaturezaChamado`, reaproveitando os status ja existentes no sistema.

## Decisao tecnica
- Reaproveitar `StatusChamado` e `StatusChamadoEnum` existentes.
- Nao criar sistema paralelo de status.
- Aplicar validacao no fluxo administrativo de mudanca de status, encerramento e reabertura.
- Manter a logica atual de SLA (`PausaSla`, `EhStatusFinal`, eventos e calculos) sem alteracoes de semantica.

## Matriz inicial de status permitidos por natureza
Status existentes reutilizados:
- `Aberto`
- `EmAtendimento`
- `AguardandoSolicitante`
- `Resolvido`
- `Encerrado`
- `Cancelado`

Mapeamento:
- `Incidente`: `Aberto`, `EmAtendimento`, `AguardandoSolicitante`, `Resolvido`, `Encerrado`, `Cancelado`
- `Requisicao`: `Aberto`, `EmAtendimento`, `AguardandoSolicitante`, `Resolvido`, `Encerrado`, `Cancelado`
- `Mudanca`: `Aberto`, `EmAtendimento`, `AguardandoSolicitante`, `Resolvido`, `Encerrado`, `Cancelado`
- `Problema`: `Aberto`, `EmAtendimento`, `AguardandoSolicitante`, `Resolvido`, `Encerrado`, `Cancelado`
- `EventoAlerta`: `Aberto`, `EmAtendimento`, `Resolvido`, `Encerrado`, `Cancelado`
- `TarefaOperacional`: `Aberto`, `EmAtendimento`, `Resolvido`, `Encerrado`, `Cancelado`

## Implementacao
- Servico centralizado: `IFluxoStatusChamadoService` e `FluxoStatusChamadoService`.
- Metodos:
  - `ObterStatusPermitidos(NaturezaChamadoEnum natureza)`
  - `StatusEhPermitido(NaturezaChamadoEnum natureza, StatusChamadoEnum status)`
  - `ValidarStatusPermitido(NaturezaChamadoEnum natureza, StatusChamadoEnum status)`
- Validacao aplicada em:
  - `AlterarStatusChamadoUseCase`
  - `EncerrarChamadoUseCase`
  - `ReabrirChamadoUseCase`

## Dados legados
- Nao ha migration obrigatoria nesta sprint (nenhum novo status/coluna).
- Chamados legados com combinacao natureza/status fora da matriz continuam visiveis.
- A regra bloqueia novas transicoes invalidas, sem alterar historico automaticamente.

## Impacto em SLA
- Nenhuma mudanca na semantica de pausa/finalizacao de SLA.
- Nenhum recalculo estrutural de SLA foi introduzido por esta sprint.
- A validacao por natureza atua apenas como regra de autorizacao de transicao de status.

## Limitacoes desta sprint
- Nao inclui status ITIL mais especificos (ex.: `EmAprovacao`, `Aprovada`, `Reprovada`, `Planejada`, `Correlacionado`).
- Nao implementa SLA por natureza.
- Nao implementa fluxo completo de aprovacao de mudanca.
- Nao altera nomes publicos de status existentes.

## Pendencias futuras
- Evoluir matriz de status por natureza com estados ITIL especializados.
- Avaliar exposicao de status disponiveis por chamado no detalhe admin para orientar UI por natureza.
- Planejar estrategia de saneamento opcional para legados fora da matriz, se necessario.
