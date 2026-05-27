# Impacto, Urgencia e Prioridade (Fundacao ITSM)

## Decisao arquitetural

- `PrioridadeChamado` existente foi reaproveitada.
- `ImpactoChamado` e `UrgenciaChamado` foram introduzidos como classificadores obrigatorios no chamado.
- A prioridade inicial do chamado passa a ser calculada de forma centralizada por matriz `Impacto x Urgencia`.
- O endpoint administrativo de alteracao manual de prioridade foi mantido por compatibilidade nesta sprint.

## Responsabilidades

- `ImpactoChamado`:
  - representa a abrangencia/efeito no negocio.
- `UrgenciaChamado`:
  - representa a pressao temporal para atendimento.
- `PrioridadeChamado`:
  - continua como referencia operacional e de SLA.
  - agora recebe valor inicial calculado pela matriz.

## Valores oficiais

- `ImpactoChamadoEnum`:
  - `Baixo`
  - `Medio`
  - `Alto`

- `UrgenciaChamadoEnum`:
  - `Baixa`
  - `Media`
  - `Alta`

## Matriz oficial

- `Alto + Alta = Critica`
- `Alto + Media = Alta`
- `Alto + Baixa = Media`
- `Medio + Alta = Alta`
- `Medio + Media = Media`
- `Medio + Baixa = Baixa`
- `Baixo + Alta = Media`
- `Baixo + Media = Baixa`
- `Baixo + Baixa = Baixa`

## Regras de fallback

- Migration (chamados legados):
  - `ImpactoChamado = Baixo`
  - `UrgenciaChamado = Baixa`
  - prioridade legada recebe fallback para `Baixa` quando vier nula/invalida/baixa e preserva valores maiores (`Media`, `Alta`, `Critica`).

- Abertura por e-mail:
  - fallback base: `ImpactoChamado = Baixo`, `UrgenciaChamado = Baixa`;
  - se classificado como `Incidente`, aplica ao menos `UrgenciaChamado = Media`.
  - prioridade e calculada pela matriz; se nao houver cadastro ativo correspondente, usa fallback de prioridade padrao existente.

## Pendencias para proximas sprints

- revisar governanca do endpoint de alteracao manual de prioridade;
- fluxo de status por natureza;
- campos obrigatorios por natureza;
- SLA por natureza;
- filtros e dashboards por impacto/urgencia;
- automatismos adicionais de classificacao.
