# Inventario/Ativos - SGX Sistema de Chamados

## Visao geral
O modulo Inventario/Ativos gerencia ativos de infraestrutura e operacao do SGX com rastreabilidade completa.

Escopo funcional entregue ate a Sprint 6:
- cadastro administrativo de ativos;
- tipos de ativo;
- inativacao logica sem exclusao fisica;
- historico operacional e movimentacoes;
- vinculo de ativo com chamados;
- consulta de chamados relacionados por ativo;
- integracao com detalhe administrativo do chamado;
- auditoria complementar aos registros de historico;
- frontend administrativo completo.

Status consolidado Sprint 6:
- Area: Inventario/Ativos
- Categoria: Infraestrutura
- Status da implementacao: Implementado funcionalmente
- Status tecnico: Homologacao funcional preparada
- Percentual: 90%

## Conceito de ativo
Um ativo representa um item rastreavel de infraestrutura ou operacao, por exemplo:
- notebook;
- desktop;
- impressora;
- monitor;
- servidor;
- switch;
- roteador;
- celular corporativo;
- tablet;
- nobreak;
- periferico;
- outro.

## Codigo x Patrimonio x Serie
- Codigo: identificador institucional do SGX para o ativo. Obrigatorio e unico.
- NumeroPatrimonio: identificador patrimonial corporativo. Opcional e unico quando preenchido.
- NumeroSerie: identificador de fabrica. Opcional e unico quando preenchido.

## Tipos de ativo
Entidade: `TipoAtivoInventario`.

Uso:
- classificar ativos por tipo;
- apoiar filtros e relatarios;
- permitir evolucao do inventario sem restringir o modulo a computadores.

## Status operacional
Enum: `StatusOperacionalAtivo`.

Valores:
- Operacional
- EmManutencao
- ComDefeito
- Reservado
- Baixado

## Status patrimonial
Enum: `StatusPatrimonialAtivo`.

Valores:
- EmUso
- EmEstoque
- Emprestado
- EmTransferencia
- Descartado
- Extraviado

## Criticidade
Enum: `CriticidadeAtivo`.

Valores:
- Baixa
- Media
- Alta
- Critica

## Permissoes do modulo
- `InventarioAtivos.Visualizar`
- `InventarioAtivos.Gerenciar`
- `InventarioAtivos.Inativar`
- `InventarioAtivos.Movimentar`
- `InventarioAtivos.VincularChamado`

## Fluxo administrativo
Fluxo principal:
1. listar ativos e aplicar filtros;
2. criar ativo com validacoes de unicidade;
3. editar ativo ativo;
4. inativar/reativar sem exclusao fisica;
5. movimentar ativo com registro de historico;
6. consultar historico e chamados relacionados.

## Inativacao e reativacao
- inativacao: `Ativo = false`, `InativadoEm`, `InativadoPorUsuarioId`.
- reativacao: `Ativo = true`, preservando rastreio de inativacao anterior.
- exclusao fisica: nao existe no modulo.

## Movimentacao
Endpoint dedicado: `POST /api/admin/inventario-ativos/{id}/movimentar`.

Permite alterar:
- departamento;
- local/unidade;
- usuario responsavel;
- status operacional;
- status patrimonial;
- observacao.

Regras:
- bloqueia ativo inativo;
- bloqueia movimentacao sem alteracao efetiva;
- grava historico com origem/destino e valores anteriores/novos;
- atualiza auditoria administrativa.

## Historico operacional
Entidade: `HistoricoInventarioAtivo`.

Cobertura automatica:
- criacao;
- edicao relevante;
- inativacao;
- reativacao;
- movimentacao;
- vinculo/remocao de vinculo com chamado.

## Vinculo com chamados
- abertura de chamado aceita `InventarioAtivoId` opcional;
- abertura sem ativo continua suportada;
- ativo inativo e bloqueado para nova abertura e novo vinculo.

Endpoints administrativos de vinculo:
- `POST /api/admin/chamados/{chamadoId}/ativo/{ativoId}`;
- `DELETE /api/admin/chamados/{chamadoId}/ativo`.

## Chamados relacionados ao ativo
Endpoint:
- `GET /api/admin/inventario-ativos/{id}/chamados`.

Comportamento:
- lista chamados vinculados ao ativo;
- permanece disponivel mesmo com ativo inativo, para preservar historico.

## Integracao com detalhe administrativo do chamado
Tela:
- `AdminDetalheChamadoView.vue`.

Recursos:
- secao "Ativo vinculado" com codigo/nome/patrimonio/serie/status operacional;
- link para detalhe do ativo;
- busca e vinculacao de ativo;
- remocao de vinculo com confirmacao;
- controle por permissao `InventarioAtivos.VincularChamado`.

## Endpoints administrativos do modulo
Inventario/Ativos:
- `GET /api/admin/inventario-ativos`
- `GET /api/admin/inventario-ativos/{id}`
- `POST /api/admin/inventario-ativos`
- `PUT /api/admin/inventario-ativos/{id}`
- `POST /api/admin/inventario-ativos/{id}/inativar`
- `POST /api/admin/inventario-ativos/{id}/reativar`
- `GET /api/admin/inventario-ativos/tipos`
- `GET /api/admin/inventario-ativos/{id}/historico`
- `POST /api/admin/inventario-ativos/{id}/movimentar`
- `GET /api/admin/inventario-ativos/{id}/chamados`

Chamados x ativo:
- `POST /api/admin/chamados/{chamadoId}/ativo/{ativoId}`
- `DELETE /api/admin/chamados/{chamadoId}/ativo`

## Testes existentes
Status de validacao atual:
- backend build Release: OK
- testes backend: 670 aprovados
- frontend unit tests: 65 aprovados
- frontend build: OK

## Revisao UX - Sprint 6
Arquivos revisados:
- `InventarioAtivosListPage.vue`
- `InventarioAtivosFormPage.vue`
- `InventarioAtivosDetalhePage.vue`
- `AdminDetalheChamadoView.vue`

Resultado da revisao:
- estados de loading presentes;
- estados de erro presentes;
- estados vazios presentes;
- mensagens amigaveis para usuario final;
- acoes destrutivas com confirmacao;
- inativacao/reativacao com confirmacao;
- remocao de vinculo com confirmacao;
- movimentacao com validacao visual de alteracao efetiva;
- responsividade basica preservada;
- sem `console.log`, `debugger`, `TODO` ou `FIXME` indevidos nos arquivos revisados.

## Revisao de seguranca - Sprint 6
Confirmacoes:
- endpoints administrativos protegidos por `PermissionPolicies.InventarioAtivos.*`;
- ativo inativo nao pode ser usado em novo chamado;
- ativo inativo nao pode ser vinculado a chamado existente;
- historico de chamados antigos permanece consultavel;
- exclusao fisica nao existe;
- backend valida codigo, patrimonio e serie;
- backend valida movimentacao e bloqueia movimentacao vazia;
- backend valida vinculo com chamado;
- frontend nao e barreira principal de seguranca;
- auditoria e historico coexistem e se complementam.

## Homologacao e evidencias
Arquivos de apoio da Sprint 6:
- `docs/CHECKLIST-HOMOLOGACAO-INVENTARIO-ATIVOS.md`
- `docs/evidencias/inventario-ativos/README.md`

## E2E
Nao foi identificado framework E2E (Playwright/Cypress) instalado no projeto web nesta sprint.

Decisao da Sprint 6:
- nao instalar framework novo nesta sprint;
- registrar E2E completo como pendencia evolutiva.

## Pendencias evolutivas
- homologacao institucional com usuarios reais;
- evidencias com prints reais;
- testes E2E completos;
- seletor de ativo na abertura de chamado do portal/admin;
- importacao em massa de ativos;
- exportacao de inventario;
- leitura por QR Code;
- etiquetas patrimoniais;
- anexos no ativo;
- garantia e alertas de vencimento;
- manutencao preventiva;
- integracao com patrimonio oficial;
- relatorios de ativos por departamento;
- dashboard de ativos criticos;
- indicadores de chamados por ativo;
- regra patrimonial avancada para descartado/extraviado;
- inventario por agente automatico futuramente.
