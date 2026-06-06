# Sprint 3 - Frontend para transferir chamado entre grupos tecnicos

## Tela/componente alterado

- `src/SGX.SistemaChamado.Web/src/views/AdminDetalheChamadoView.vue`
- `src/SGX.SistemaChamado.Web/src/components/admin/PainelAtendimento.vue`

## Acao criada

A tela de detalhe administrativo do chamado passou a exibir a acao `Transferir grupo` no painel de acoes operacionais.

A acao consome o endpoint existente:

- `POST /api/admin/chamados/{chamadoId}/transferir-grupo-tecnico`

## Condicoes visuais

O botao aparece somente quando:

- o chamado possui grupo tecnico atual;
- o usuario autenticado possui perfil `Administrador` ou `Atendente`.

A tela faz apenas validacoes visuais minimas. As regras de grupo ativo, fila ativa, pertencimento da fila ao grupo e limpeza do responsavel continuam no backend.

## Modal/formulario

Foi criado um modal `Transferir chamado para outro grupo tecnico` com:

- `Grupo tecnico de destino`, obrigatorio;
- `Fila de destino`, opcional e carregada conforme o grupo selecionado.

Ao trocar o grupo de destino, a fila selecionada e limpa e as filas ativas do novo grupo sao carregadas pelo endpoint ja existente de filas por grupo.

O contrato atual `TransferirGrupoTecnicoChamadoRequest` nao possui campo de observacao; por isso a tela nao exibe justificativa nesta etapa.

## Comportamento apos sucesso

Apos a transferencia, o detalhe do chamado e recarregado. A tela nao altera localmente `ResponsavelId`, `GrupoTecnicoId` ou `FilaAtendimentoId`; o estado exibido vem da resposta atualizada do backend.

## Tratamento de erros

Erros retornados pela API sao exibidos pelo tratamento ja existente da tela (`registrarErro`), incluindo rejeicoes de regra de negocio.

## Testes

Foram ajustados testes frontend para cobrir:

- chamada do service para `transferir-grupo-tecnico`;
- presenca da acao no detalhe;
- modal com grupo obrigatorio e fila opcional;
- carregamento de grupos e filas;
- reload/sucesso apos transferencia.

## O que nao foi implementado

- Nenhum endpoint novo.
- Nenhuma regra backend nova.
- Nenhuma tela separada de transferencia.
- Nenhum seletor de tecnico.
- Nenhuma acao de direcionamento inicial.
- Nenhum cadastro, edicao ou inativacao de fila.
- Nenhuma alteracao de SLA, dashboard ou relatorio.
- Nenhuma migration estrutural.

## Proxima etapa recomendada

Ajustar listagem/filtros para grupo tecnico e fila.
