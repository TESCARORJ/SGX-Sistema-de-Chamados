# Sprint 7 - Diagnostico de Gerenciamento de Requisicoes

## Objetivo

Registrar o estado real da Sprint 7 antes da troca do checklist generico por um checklist rastreavel e compativel com a implementacao existente.

## Cenario identificado

- `Cenario C - fluxo existe parcialmente`.

## Evidencias confirmadas

- existe abertura por catalogo reutilizando `Chamado`;
- `Chamado` ja possui `CatalogoServicoId`;
- o portal expoe `GET /api/portal/catalogo-servicos/{slug}/preparar-chamado`;
- `AbrirChamadoUseCase` ja resolve o servico de catalogo e aplica categoria, subcategoria e prioridade oficiais do catalogo;
- o fluxo ja registra historico de abertura por catalogo;
- o fluxo ja pode gerar aprovacao automatica quando `RequerAprovacao = true`;
- o portal ja possui tela de catalogo, detalhe do servico e inicio de abertura por catalogo.

## Lacunas confirmadas

- nao existe `NaturezaChamadoEnum.RequisicaoServico`; a base atual reutiliza `NaturezaChamadoEnum.Requisicao`;
- nao existe fluxo separado de requisicao de servico com use case e formulario dinamico dedicados;
- nao existe formulario dinamico por servico;
- nao existe persistencia de respostas de formulario por servico;
- nao existe aplicacao comprovada de grupo tecnico responsavel por servico;
- nao existe aplicacao comprovada de SLA por servico nesse fluxo;
- nao existe trilha final de homologacao funcional/visual e aceite.

## Atualizacao deste item

- foi introduzido contrato dedicado para abertura guiada por catalogo;
- foi confirmado que o validator dedicado deve permanecer restrito a validacoes de contrato, sem absorver regras de negocio, catalogo ou seguranca do use case;
- o validator dedicado cobre `CatalogoServicoId` obrigatorio, `Titulo` obrigatorio, limites de tamanho e consistencia basica do request;
- a semantica explicita de requisicao ficou no endpoint `POST /api/portal/catalogo-servicos/requisicoes`;
- o endpoint mapeia para o fluxo atual e fixa `NaturezaChamadoEnum.Requisicao` no backend;
- o frontend passou a enviar apenas `catalogoServicoId`, `titulo` e `descricao` nesse caminho guiado;
- o fluxo legado de `POST /api/portal/chamados` permaneceu preservado.

## Inconsistencia do roadmap anterior

O checklist anterior marcava como concluido o item generico `Implementar entregas centrais da sprint`, mas o proprio roadmap informava que:

- ainda nao existia fluxo separado de requisicao;
- ainda faltava vincular o fluxo ao catalogo no backend e frontend;
- ainda havia pendencias centrais de aprovacao por servico, status proprios, servicos relacionados e conclusao com aceite.

Conclusao:

- o checklist anterior nao era rastreavel;
- o percentual anterior nao podia ser justificado pelo item generico concluido;
- a Sprint 7 precisava ser decomposta em entregas verificaveis.

## Menor escopo seguro definido

Tratar a Sprint 7 como evolucao sobre a abertura atual por catalogo, sem criar um segundo sistema de chamados e sem duplicar regras existentes:

- reutilizar `Chamado` como agregado principal;
- reutilizar `NaturezaChamadoEnum.Requisicao`;
- manter o backend como fonte de verdade das regras de catalogo;
- preservar incidentes e aberturas sem catalogo;
- preservar o motor de aprovacao existente;
- adiar qualquer extensao estrutural que nao seja comprovadamente necessaria.
