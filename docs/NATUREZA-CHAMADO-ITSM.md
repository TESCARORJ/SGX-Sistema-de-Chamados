# Natureza do Chamado (Fundacao ITSM)

## Decisao arquitetural

- `NaturezaChamado` e um conceito proprio e obrigatorio do chamado.
- `NaturezaChamado` **nao substitui** `TipoSolicitacao` nesta sprint.
- `TipoSolicitacao` permanece como classificacao operacional/catalogo/servico solicitado.
- `NaturezaChamado` representa o processo ITSM do ticket.

## Responsabilidades

- `NaturezaChamado`:
  - define o tipo de processo ITSM do chamado;
  - base para futuras regras de fluxo, SLA, campos obrigatorios, permissoes e relatorios.

- `TipoSolicitacao`:
  - classifica o pedido operacional existente;
  - permanece ativo no dominio, APIs e telas atuais;
  - nao deve ser duplicado nem removido nesta sprint.

## Valores oficiais de NaturezaChamado

- `Incidente`
- `Requisicao`
- `Mudanca`
- `Problema`
- `EventoAlerta`
- `TarefaOperacional`

## Regras de fallback desta sprint

- Chamados legados (migration):
  - fallback inicial: `Requisicao`;
  - regra simples para `Incidente`: quando titulo/descricao contem termos como `erro`, `falha`, `indisponibilidade`, `indisponivel`, `travamento`, `travou`, `sem acesso`, `queda`, `fora do ar`.

- Abertura por e-mail:
  - fallback padrao: `Requisicao`;
  - se assunto/corpo indicar falha/incidente pelos mesmos termos, classifica como `Incidente`.

## Pendencias para proximas sprints

- fluxo de status por natureza;
- campos obrigatorios por natureza;
- matriz impacto x urgencia;
- SLA por natureza;
- filtros e visoes no dashboard por natureza;
- permissoes e acoes por natureza.
