# CAMPOS OBRIGATORIOS POR NATUREZA ITSM

## Objetivo
Centralizar no backend a validacao de campos obrigatorios de abertura de chamado por `NaturezaChamado`, reaproveitando os campos ja existentes e sem criar novos campos nesta sprint.

## Campos reaproveitados
- `NaturezaChamado`
- `Titulo`
- `Descricao`
- `CategoriaId`
- `TipoSolicitacaoId`
- `ImpactoChamado`
- `UrgenciaChamado`
- `CatalogoServicoId` / `CatalogoServicoSlug` (quando a categoria vem do catalogo)

## Regra minima por natureza
- `Incidente`
  - `NaturezaChamado`, `Titulo`, `Descricao`, `ImpactoChamado`, `UrgenciaChamado`
  - `CategoriaId` ou `TipoSolicitacaoId` (ou servico de catalogo informado)
- `Requisicao`
  - `NaturezaChamado`, `Titulo`, `Descricao`
  - `CategoriaId` ou `TipoSolicitacaoId` (ou servico de catalogo informado)
- `Mudanca`
  - `NaturezaChamado`, `Titulo`, `Descricao`, `ImpactoChamado`, `UrgenciaChamado`
  - `CategoriaId` ou `TipoSolicitacaoId` (ou servico de catalogo informado)
  - `Descricao` com detalhamento minimo
- `Problema`
  - `NaturezaChamado`, `Titulo`, `Descricao`, `ImpactoChamado`, `UrgenciaChamado`
  - `Descricao` com evidencias/recorrencia ou detalhamento minimo
- `EventoAlerta`
  - `NaturezaChamado`, `Titulo`, `Descricao`, `ImpactoChamado`, `UrgenciaChamado`
  - `CategoriaId` ou `TipoSolicitacaoId` (ou servico de catalogo informado)
- `TarefaOperacional`
  - `NaturezaChamado`, `Titulo`, `Descricao`, `ImpactoChamado`, `UrgenciaChamado`
  - `CategoriaId` ou `TipoSolicitacaoId` (ou servico de catalogo informado)

## Diferenca Portal x E-mail
- Portal/API: validacao obrigatoria por natureza aplicada no fluxo de criacao.
- E-mail: mantido fallback seguro para abertura automatica.
  - `NaturezaChamado` inferida por conteudo.
  - `ImpactoChamado` e `UrgenciaChamado` preenchidos automaticamente.
  - Nao bloquear por ausencia de campos especificos que nao existem no payload de e-mail.

## Campos especificos ainda nao criados (pendencias)
- `JustificativaMudanca`
- `JanelaMudanca`
- `EvidenciaProblema`
- `OrigemAlerta`
- `SeveridadeAlerta`
- `ResponsavelTarefa`

## Limitacoes atuais
- Abertura por chamado depende de campos hoje existentes; nao foi criado fluxo adicional de aprovacao de mudanca nesta sprint.
- Regra de categoria/tipo considera tambem abertura por catalogo, onde categoria e resolvida pelo servico.
- Para compatibilidade de fluxos legados e internos, o fallback da abertura por e-mail foi preservado.

## Proximos passos sugeridos
- Evoluir formularios condicionais por natureza no frontend (portal/admin).
- Adicionar campos especificos por natureza com migrations controladas.
- Refinar mensagens de validacao por contexto (portal, api legada e email worker).
