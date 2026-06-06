# Garantir compatibilidade com chamados existentes

## Contexto

A Sprint 3 introduziu `GrupoTecnico` e `FilaAtendimento` como estruturas corporativas de atendimento. As colunas opcionais `GrupoTecnicoId` e `FilaAtendimentoId` foram adicionadas em `Chamado`, preservando `ResponsavelId` como responsavel individual opcional.

Esta etapa valida que chamados legados continuam funcionando sem grupo tecnico e sem fila de atendimento.

## Campos opcionais validados

- `Chamado.ResponsavelId`: permanece opcional e continua representando o tecnico individual.
- `Chamado.GrupoTecnicoId`: permanece opcional e representa o grupo tecnico corporativo.
- `Chamado.FilaAtendimentoId`: permanece opcional e representa a fila operacional.

Na configuracao EF Core, `responsavel_id`, `grupo_tecnico_id` e `fila_atendimento_id` nao usam `IsRequired()`.

## Migrations analisadas

- `20260605050754_VincularChamadoGrupoTecnicoSprint3`: adiciona `chamados.grupo_tecnico_id` como `nullable: true`, cria o indice `ix_chamados_grupo_tecnico_id`, cria FK opcional para `grupos_tecnicos` com delete restrito e remove tudo no `Down`.
- `20260605141650_VincularChamadoFilaAtendimentoSprint3`: adiciona `chamados.fila_atendimento_id` como `nullable: true`, cria o indice `ix_chamados_fila_atendimento_id`, cria FK opcional para `filas_atendimento` com delete restrito e remove tudo no `Down`.

As migrations nao definem valor padrao obrigatorio para grupo ou fila, portanto registros antigos continuam validos no banco local.

## Fluxos legados analisados

- Dominio de `Chamado`.
- Configuracao EF Core de `Chamado`.
- Abertura de chamado pelo portal.
- Listagem administrativa.
- Detalhe administrativo.
- Linha do tempo.
- Assumir chamado.
- Atribuir chamado.
- Transferencia de grupo tecnico quando nao ha grupo anterior.
- Types e telas Vue de chamados, que ainda usam apenas `responsavelId`/`responsavelNome` e nao expõem grupo/fila nesta etapa.

## Cenarios de compatibilidade testados

- Chamado sem grupo, sem fila e sem responsavel permanece valido no dominio.
- Chamado com responsavel individual e grupo/fila nulos permanece valido no dominio.
- Abertura de chamado nao exige grupo tecnico, fila nem responsavel.
- Listagem administrativa aceita chamado com grupo/fila nulos.
- Detalhe administrativo aceita chamado com grupo/fila/responsavel nulos.
- Linha do tempo aceita chamado com grupo/fila nulos.
- Assumir chamado legado continua preenchendo `ResponsavelId` e mantendo grupo/fila nulos.
- Atribuir chamado legado continua preenchendo `ResponsavelId` e mantendo grupo/fila nulos.
- Transferir chamado sem grupo anterior para grupo tecnico ja estava coberto por teste da regra de transferencia.

## Ajustes realizados

Nao houve ajuste funcional. Foram adicionadas e reforcadas assercoes de regressao nos testes para explicitar os cenarios legados.

## Confirmacoes de compatibilidade

- Abertura de chamado continua sem exigir grupo/fila.
- Listagem e detalhe continuam nulo-seguros para chamados sem grupo/fila.
- Linha do tempo continua baseada em historicos existentes e nao depende de grupo/fila.
- Assumir e atribuir continuam usando `ResponsavelId` como responsavel individual.
- `GrupoTecnicoId` e `FilaAtendimentoId` continuam opcionais.
- `ResponsavelId` nao foi removido, substituido ou tornado obrigatorio.

## O que nao foi alterado

- Nenhum endpoint novo foi criado.
- Nenhuma tela Vue foi criada ou alterada.
- Nenhum dashboard foi alterado.
- Nenhum relatorio foi alterado.
- Nenhuma regra de SLA foi alterada.
- Nenhuma regra de roteamento automatico foi criada.
- Nenhuma regra nova de assumir chamado da fila foi criada.
- Nenhuma migration estrutural foi criada nesta etapa.

## Riscos tecnicos restantes

- Enquanto a regra funcional de direcionamento para grupo/fila nao for implementada, ainda podem existir chamados sem grupo e sem fila no fluxo operacional.
- Futuras respostas da API que exponham grupo/fila deverao manter campos nullable no frontend.
- A consistencia entre `Chamado.GrupoTecnicoId` e `Chamado.FilaAtendimentoId` continua sendo regra de aplicacao futura quando houver operacoes funcionais de fila.

## Roadmap

O item `Garantir compatibilidade com chamados existentes` foi marcado como concluido. Com 15 itens concluidos em 54 itens ativos, o percentual esperado da Sprint 3 passa a ser aproximadamente 28%.

## Proxima etapa recomendada

Criar contratos de grupo tecnico, mantendo os contratos de chamado nulo-seguros e sem exigir grupo/fila na abertura ou nos fluxos legados.
