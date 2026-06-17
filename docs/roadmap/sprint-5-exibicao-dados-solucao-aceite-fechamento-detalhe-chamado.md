# Sprint 5 - Exibicao de dados de solucao, aceite e fechamento no detalhe do chamado

## Objetivo
Expor no detalhe administrativo e no detalhe do portal os dados ja existentes do ciclo de resolucao, aceite, rejeicao e fechamento do chamado, sem criar fluxo novo de negocio.

## Escopo aplicado
- ampliacao dos DTOs de detalhe admin e portal;
- preenchimento dos novos campos nos use cases de detalhe;
- exibicao read-only no frontend administrativo e no portal;
- manutencao da separacao entre historico funcional e auditoria tecnica.

## Campos exibidos
- `SolucaoTecnica`;
- `ResolvidoEm`;
- `EncerradoEm`;
- `AceitoEm`;
- `AceitoPorUsuarioId`;
- `AceitoPorNome`;
- `ObservacaoAceite`;
- `SolucaoRejeitadaEm`;
- `SolucaoRejeitadaPorUsuarioId`;
- `SolucaoRejeitadaPorNome`;
- `MotivoRejeicaoSolucao`;
- `StatusFechamentoDescricao`.

## Origem dos dados
- os campos de aceite, rejeicao e fechamento sao lidos diretamente da entidade `Chamado`;
- `SolucaoTecnica` nao existe como coluna propria em `Chamado` nesta sprint;
- para preservar o escopo, a solucao tecnica foi derivada da evidencia funcional ja existente no comentario criado junto da resolucao, usando uma janela curta ao redor de `ResolvidoEm`.

## Comportamento de permissao
- o detalhe administrativo pode exibir a solucao derivada mesmo quando o comentario de resolucao foi interno;
- o detalhe do portal so exibe a solucao quando ela estiver disponivel em comentario nao interno, preservando a restricao de visibilidade do solicitante;
- auditoria tecnica bruta nao foi exposta no frontend.

## Limitacoes assumidas
- `MotivoCancelamento` nao foi incluido no contrato de detalhe desta etapa porque nao existe campo persistido equivalente em `Chamado` dentro do escopo atual;
- a origem de fechamento automatico permanece registrada em historico/auditoria, sem card dedicado novo no detalhe.

## Validacoes executadas
- testes de detalhe administrativo e portal cobrindo dados de resolucao, aceite, rejeicao e comportamento nulo;
- testes frontend de presenca da secao read-only de ciclo de encerramento;
- build backend, build frontend e verificacao de `pending model changes`.
