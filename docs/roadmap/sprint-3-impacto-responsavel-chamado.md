# Mapeamento do modelo atual de responsável por chamado

## Contexto

A Sprint 3 do roadmap ITIL/ITSM tem como objetivo preparar grupos tecnicos, filas e atribuicao de chamados. Antes de criar novas entidades ou fluxos, foi mapeado como o sistema atual trata o responsavel individual do chamado para preservar o comportamento existente.

Esta analise nao implementa GrupoTecnico, MembroGrupoTecnico, FilaAtendimento, migrations funcionais, endpoints, telas ou regras novas. O foco e documentar dependencias e riscos.

## Objetivo da análise

Identificar campos, relacionamentos, fluxos, DTOs, endpoints, telas, filtros, historico, auditoria, permissoes, SLA e relatorios que dependem do responsavel atual do chamado, orientando a futura modelagem de GrupoTecnico sem regressao no fluxo de atribuicao individual.

## Resumo técnico do modelo atual

O modelo atual usa `Chamado.ResponsavelId` como referencia opcional para `Usuario`. Chamados podem nascer sem responsavel e, na interface administrativa, essa ausencia e tratada como fila operacional implicita.

A atribuicao individual acontece por dois fluxos principais:

- Assumir chamado: o usuario atual passa a ser o responsavel individual.
- Atribuir chamado: um administrador seleciona um usuario com perfil de atendimento.

Nao existe conceito formal de grupo tecnico, fila de atendimento ou transferencia entre grupos. A auditoria atual registra alteracao de responsavel em historico textual e em auditoria generica, sem guardar IDs estruturados de origem/destino da movimentacao.

## Arquivos encontrados e função de cada um

### Dominio

- `src/SGX.SistemaChamado.Domain/Entities/Chamado.cs`: entidade principal; contem `ResponsavelId`, navegacao `Responsavel` e metodo `AtribuirResponsavel`.
- `src/SGX.SistemaChamado.Domain/Entities/HistoricoChamado.cs`: historico textual por chamado, com `Tipo`, `Descricao` e `UsuarioId`.
- `src/SGX.SistemaChamado.Domain/Enums/TipoHistoricoChamado.cs`: define `ResponsavelAlterado = 5`.
- `src/SGX.SistemaChamado.Domain/Enums/AcaoChamadoEnum.cs`: define acoes administrativas como `Assumir` e `Atribuir`.
- `src/SGX.SistemaChamado.Domain/Enums/StatusChamadoEnum.cs`: estados usados para bloquear ou liberar acoes.

### Infraestrutura

- `src/SGX.SistemaChamado.Infrastructure/Persistence/Configurations/ChamadoConfiguration.cs`: mapeia `responsavel_id`, FK para `Usuario` e delete restrito.
- `src/SGX.SistemaChamado.Infrastructure/Persistence/Migrations/20260506024418_InitialCreate.cs`: criou `responsavel_id` nullable, FK `FK_chamados_usuarios_responsavel_id` e indice `IX_chamados_responsavel_id`.
- `src/SGX.SistemaChamado.Infrastructure/Persistence/Migrations/SGXSistemaChamadoDbContextModelSnapshot.cs`: snapshot atual ainda possui `ResponsavelId`, indice e FK.

### Aplicacao/backend

- `src/SGX.SistemaChamado.Application/UseCases/Admin/AssumirChamadoUseCase.cs`: assume chamado, valida permissao e registra primeira resposta de SLA.
- `src/SGX.SistemaChamado.Application/UseCases/Admin/AtribuirChamadoUseCase.cs`: atribui responsavel por administrador, valida usuario ativo e perfil de atendimento.
- `src/SGX.SistemaChamado.Application/UseCases/Admin/ListarChamadosAdminUseCase.cs`: lista chamados com `Include(x => x.Responsavel)` e filtro por `ResponsavelId`.
- `src/SGX.SistemaChamado.Application/UseCases/Admin/AdminChamadoLoader.cs`: centraliza carregamento detalhado com responsavel, historicos, comentarios, anexos, aprovacoes e SLA.
- `src/SGX.SistemaChamado.Application/UseCases/Admin/AdminUseCaseHelpers.cs`: projeta `ResponsavelNome` no resumo e `ResponsavelAdminResponse` no detalhe.
- `src/SGX.SistemaChamado.Application/Services/AcoesChamadoService.cs`: calcula acoes disponiveis com base no responsavel atual.
- `src/SGX.SistemaChamado.Application/UseCases/Admin/AdminIndicadoresUseCases.cs`: calcula `TotalSemResponsavel` e produtividade por atendente.
- `src/SGX.SistemaChamado.Application/UseCases/Admin/RelatoriosAvancadosAdminUseCases.cs`: agrupa e filtra por atendente/responsavel em relatorios.
- `src/SGX.SistemaChamado.Application/UseCases/Chamados/LinhaTempoChamadoUseCases.cs`: transforma `ResponsavelAlterado` em evento interno de linha do tempo.
- `src/SGX.SistemaChamado.Application/DTOs/Admin/AdminRequests.cs`: possui `FiltroChamadosAdminRequest.ResponsavelId` e `AtribuirChamadoRequest.ResponsavelId`.
- `src/SGX.SistemaChamado.Application/DTOs/Admin/AdminResponses.cs`: expoe `ResponsavelNome` e `Responsavel`.
- `src/SGX.SistemaChamado.Application/DTOs/Admin/AdminDashboardDtos.cs`: expoe indicadores por responsavel e total sem responsavel.
- `src/SGX.SistemaChamado.Application/DTOs/Admin/RelatoriosAvancadosDtos.cs`: expoe `ResponsavelId`, `AtendenteId` e agrupamento por responsavel/atendente.
- `src/SGX.SistemaChamado.Application/DTOs/Portal/ChamadoDetalheResponse.cs`: expoe responsavel como texto para o solicitante.
- `src/SGX.SistemaChamado.Application/DTOs/Chamados/LinhaTempoChamadoDtos.cs`: possui campo textual de responsavel em eventos.
- `src/SGX.SistemaChamado.Application/Validators/AtribuirChamadoRequestValidator.cs`: exige `ResponsavelId` no fluxo de atribuicao individual.

### API

- `src/SGX.SistemaChamado.Api/Controllers/AdminChamadosController.cs`: endpoints administrativos de listar, detalhar, assumir e atribuir.
- `src/SGX.SistemaChamado.Api/Controllers/ChamadosController.cs`: endpoints basicos retornam `ResponsavelId` em listagem, detalhe e criacao.
- `src/SGX.SistemaChamado.Api/Authorization/PermissoesConstants.cs`: permissoes `Chamados.Assumir` e `Chamados.Atribuir`.
- `src/SGX.SistemaChamado.Api/Authorization/PermissionPolicies.cs`: policies para assumir e atribuir.
- `src/SGX.SistemaChamado.Api/Services/DevelopmentSeedService.cs`: dados locais criam chamados ja atribuidos para cenarios de desenvolvimento.

### Frontend

- `src/SGX.SistemaChamado.Web/src/types/admin.ts`: tipos de chamado admin, filtros, responsavel e payload de atribuicao.
- `src/SGX.SistemaChamado.Web/src/types/chamado.ts`: tipo legado/basico com `responsavelId`.
- `src/SGX.SistemaChamado.Web/src/types/dashboard.ts`: indicadores com total sem responsavel e produtividade.
- `src/SGX.SistemaChamado.Web/src/types/indicadores.ts`: filtros e respostas por responsavel.
- `src/SGX.SistemaChamado.Web/src/types/portal.ts`: detalhe do portal com responsavel textual.
- `src/SGX.SistemaChamado.Web/src/types/linhaTempo.ts`: evento de linha do tempo com responsavel textual.
- `src/SGX.SistemaChamado.Web/src/types/relatoriosAvancados.ts`: filtros `atendenteId`/`responsavelId` e rankings por atendente.
- `src/SGX.SistemaChamado.Web/src/services/adminService.ts`: envia `responsavelId`, chama `/assumir` e `/atribuir`.
- `src/SGX.SistemaChamado.Web/src/services/dashboardAdminService.ts`: envia filtro `responsavelId`.
- `src/SGX.SistemaChamado.Web/src/services/relatoriosAvancadosAdminService.ts`: envia filtros por atendente/responsavel.
- `src/SGX.SistemaChamado.Web/src/components/admin/TabelaChamados.vue`: exibe responsavel e habilita assumir quando nao ha responsavel.
- `src/SGX.SistemaChamado.Web/src/components/admin/FiltrosChamadoAdmin.vue`: filtro por responsavel usando lista de atendentes.
- `src/SGX.SistemaChamado.Web/src/components/admin/ModalAtribuirResponsavel.vue`: modal de atribuicao individual obrigatoria.
- `src/SGX.SistemaChamado.Web/src/components/admin/PainelAtendimento.vue`: botoes de assumir e atribuir.
- `src/SGX.SistemaChamado.Web/src/components/admin/FiltrosDashboardAdmin.vue`: filtro do dashboard por responsavel.
- `src/SGX.SistemaChamado.Web/src/views/AdminChamadosView.vue`: cards e contadores de chamados sem responsavel.
- `src/SGX.SistemaChamado.Web/src/views/AdminDetalheChamadoView.vue`: detalhe, acoes e modal de atribuicao.
- `src/SGX.SistemaChamado.Web/src/views/AdminDashboardView.vue`: total sem responsavel, produtividade por atendente e acao de assumir.
- `src/SGX.SistemaChamado.Web/src/views/DetalheChamadoView.vue`: exibe responsavel para o portal/visao de detalhe.
- `src/SGX.SistemaChamado.Web/src/views/RelatoriosChamadosPage.vue`: ranking de produtividade por atendente.

## Campos e propriedades relacionados ao responsável

- `Chamado.ResponsavelId`: `Guid?`, opcional.
- `Chamado.Responsavel`: navegacao opcional para `Usuario`.
- `FiltroChamadosAdminRequest.ResponsavelId`: filtro opcional.
- `AtribuirChamadoRequest.ResponsavelId`: obrigatorio na atribuicao individual.
- `ChamadoAdminResumoResponse.ResponsavelNome`: nome opcional exibido em listas.
- `ChamadoAdminDetalheResponse.Responsavel`: objeto opcional com id, nome e email.
- `DashboardAdminResponse.TotalSemResponsavel`: contador de chamados sem responsavel.
- `ProdutividadeAtendenteResponse.ResponsavelId/ResponsavelNome`: agrupamento de produtividade por usuario responsavel.
- `RelatoriosAvancadosDtos`: usa `AtendenteId`, `ResponsavelId` e `UsuarioResponsavelId` em contextos distintos; para chamados, `AtendenteId` normalmente representa `Chamado.ResponsavelId`.
- `HistoricoChamado.UsuarioId`: usuario que executou a acao, nao necessariamente responsavel anterior ou novo.

## Fluxos atuais que dependem do responsável

- Criacao de chamado: chamado nasce sem responsavel individual; o payload de retorno expoe `ResponsavelId`.
- Listagem administrativa: carrega responsavel, permite filtrar por `ResponsavelId` e exibe `ResponsavelNome`.
- Detalhe administrativo: carrega responsavel, historico e acoes disponiveis.
- Assumir chamado: usuario atual vira responsavel; atendente nao administrador so assume chamado sem responsavel ou ja atribuido a ele.
- Atribuir chamado: administrador escolhe usuario ativo com perfil Administrador ou Atendente.
- SLA: assumir/atribuir registra primeira resposta.
- Historico: assumir/atribuir geram `TipoHistoricoChamado.ResponsavelAlterado`.
- Auditoria: assumir/atribuir registram diff textual com nome do responsavel anterior e novo.
- Dashboard: calcula fila implicita por `ResponsavelId == null` e produtividade por responsavel.
- Relatorios: agrupam chamados por atendente/responsavel e ignoram sem responsavel em produtividade.
- Frontend: habilita/desabilita "Assumir" com base em `responsavelNome` e permissao.

## Impactos no domínio

O dominio ja aceita ausencia de responsavel individual. Isso e favoravel para a futura fila de atendimento, mas o metodo `AtribuirResponsavel(Guid? responsavelId, ...)` tambem aceita `null`; a camada de aplicacao atualmente impede esse caso no endpoint de atribuicao. A futura modelagem deve manter essa separacao: chamado pode estar sem responsavel individual, mas a atribuicao individual por endpoint continua exigindo usuario valido.

Nao ha entidade ou value object para movimentacao de fila, grupo tecnico ou transferencia. O historico atual e textual e nao guarda IDs estruturados de responsavel anterior, novo responsavel, grupo origem ou grupo destino.

## Impactos na aplicação/backend

`AssumirChamadoUseCase`, `AtribuirChamadoUseCase`, `AcoesChamadoService`, dashboards e relatorios tratam responsavel individual como eixo operacional. A futura introducao de grupo/fila deve evitar substituir `ResponsavelId`; novos campos/entidades devem complementar o modelo.

A permissao de assumir considera:

- chamado nao finalizado;
- aprovacao nao bloqueando atendimento;
- permissao `Chamados.Assumir`;
- administrador, chamado sem responsavel ou chamado ja atribuido ao proprio usuario.

A permissao de atribuir e restrita a administrador e permissao `Chamados.Atribuir`.

## Impactos na API

Os contratos atuais esperam:

- `GET /api/admin/chamados` com query `responsavelId`.
- `GET /api/admin/chamados/{id}` retornando `responsavel`.
- `POST /api/admin/chamados/{id}/assumir` sem payload.
- `POST /api/admin/chamados/{id}/atribuir` com `{ responsavelId }`.
- `GET /api/chamados` e `GET /api/chamados/{id}` retornando `ResponsavelId`.

Nao ha endpoint de transferencia entre grupos, direcionamento para grupo, fila por grupo ou assumir a partir de fila formal.

## Impactos no frontend

A UI administrativa usa "sem responsavel" como fila atual. Essa ideia aparece em metricas, filtros, tabela, dashboard e controle do botao "Assumir". O modal de atribuicao so conhece atendentes individuais.

Na futura etapa, a UI precisa diferenciar:

- chamado em fila de grupo sem responsavel individual;
- chamado atribuido a responsavel individual;
- grupo tecnico atual para rastreabilidade e filtro;
- transferencia de grupo sem confundir com troca de responsavel individual.

## Impactos em banco de dados/migrations

A migration inicial criou `chamados.responsavel_id` nullable com FK para `usuarios` e indice convencional. O relacionamento usa delete restrito, preservando integridade historica quando um usuario tem chamados associados.

Nao existe tabela de grupo tecnico, membros de grupo, fila ou movimentacao estruturada de fila. Tambem nao ha coluna em `chamados` para grupo tecnico atual ou fila atual.

## Riscos identificados

- A fila atual e inferida por `ResponsavelId == null`; se grupo tecnico for adicionado sem cuidado, chamados em fila de grupo podem continuar aparecendo apenas como "sem responsavel".
- Relatorios de produtividade ignoram chamados sem responsavel; filas por grupo exigirao metricas proprias para volume, tempo parado em fila e transferencia.
- `AtribuirChamadoRequest.ResponsavelId` e obrigatorio; nao deve ser reutilizado para direcionar chamado a grupo.
- Auditoria atual de responsavel e textual; nao permite rastrear origem/destino estruturados de fila e grupo.
- `HistoricoChamado.UsuarioId` representa executor da acao, nao responsavel destino.
- O frontend decide disponibilidade visual de assumir usando `responsavelNome`; com grupos, esse criterio fica insuficiente.
- Nomes `AtendenteId` e `ResponsavelId` sao usados como equivalentes em relatorios; grupo tecnico exigira nomenclatura clara para evitar ambiguidades.
- O SLA de primeira resposta e acionado em assumir/atribuir; direcionamento inicial para grupo nao deve disparar primeira resposta automaticamente sem regra explicita.
- Filtros por responsavel nao devem passar a filtrar grupo tecnico implicitamente.
- Qualquer mudanca que torne `ResponsavelId` obrigatorio quebrara criacao de chamados e fila atual.

## Pontos que não devem ser alterados agora

- Nao criar entidades `GrupoTecnico`, `MembroGrupoTecnico` ou `FilaAtendimento`.
- Nao alterar `Chamado.ResponsavelId`.
- Nao alterar regras de assumir, atribuir, SLA ou permissao.
- Nao criar endpoints, telas, services ou migrations funcionais da Sprint 3.
- Nao executar homologacao, aceite final ou validacao com usuario.
- Nao renomear contratos atuais de responsavel/atendente.

## Recomendações para a próxima etapa da Sprint 3

A proxima etapa deve modelar `GrupoTecnico` como conceito independente do responsavel individual. O chamado deve poder apontar para um grupo tecnico atual e continuar permitindo `ResponsavelId` nulo enquanto estiver em fila.

Recomendacoes objetivas:

- Manter `ResponsavelId` opcional.
- Criar relacionamento separado para grupo tecnico atual do chamado.
- Definir estrutura de fila ou estado de enfileiramento sem reaproveitar `ResponsavelId`.
- Criar historico/auditoria estruturada para movimentacoes de grupo/fila.
- Separar filtros de responsavel individual e grupo tecnico.
- Definir se direcionar para grupo dispara ou nao SLA de primeira resposta.
- Preservar endpoints atuais e adicionar novos contratos apenas na etapa de implementacao.

## Diretriz proposta para convivência entre Grupo técnico, Fila, Responsável individual e Histórico/auditoria

O chamado deve conviver com quatro dimensoes distintas:

- Grupo tecnico: equipe responsavel pelo atendimento ou ownership operacional do chamado.
- Fila: estado em que o chamado aguarda atendimento dentro de um grupo tecnico.
- Responsavel individual: usuario que assumiu ou recebeu atribuicao nominal do chamado.
- Historico/auditoria: trilha estruturada de entrada em fila, saida de fila, transferencia de grupo e atribuicao individual.

Diretriz:

- Chamado pode continuar tendo responsavel individual, como hoje.
- Chamado tambem podera estar associado a um grupo tecnico atual.
- Chamado podera ficar em fila de grupo sem responsavel individual.
- Ao assumir chamado da fila, o tecnico passa a ser `ResponsavelId`.
- O grupo tecnico deve permanecer registrado mesmo apos assumir, para rastreabilidade, produtividade e filtros.
- Transferencia entre grupos deve alterar grupo/fila, mas nao deve ser confundida com troca de responsavel individual.
- Atribuicao individual deve continuar registrando responsavel e primeira resposta conforme regra atual.
- Auditoria futura deve armazenar IDs e nomes de grupo origem, grupo destino, responsavel anterior, responsavel novo, executor, data/hora e motivo quando aplicavel.

## Checklist técnico de conclusão deste mapeamento

- [x] Entidade principal de chamado localizada.
- [x] Campos e navegacoes de responsavel identificados.
- [x] Metodo de dominio de atribuicao mapeado.
- [x] Historico e auditoria atuais avaliados.
- [x] Mapeamento EF Core, FK, indice e migration inicial localizados.
- [x] Use cases de assumir e atribuir analisados.
- [x] DTOs, requests, responses e validators analisados.
- [x] Endpoints atuais de responsavel mapeados.
- [x] Filtros, dashboards e relatorios com responsavel mapeados.
- [x] Telas e componentes Vue dependentes de responsavel mapeados.
- [x] Riscos reais de regressao registrados.
- [x] Diretriz para convivencia com grupo tecnico e fila registrada.
- [x] Proxima etapa orientada para modelar `GrupoTecnico`.
