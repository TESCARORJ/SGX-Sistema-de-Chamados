# Cadastros Administrativos

## Objetivo

Estabelecer a base tecnica dos cadastros administrativos que suportam o ciclo de chamados, preparando o dominio e a persistencia para evolucoes de atendimento, SLA, dashboards e relatorios.

## Sprint 1 - Base tecnica

Status da implementacao: Implementado funcionalmente  
Status tecnico: Completo com pendencias evolutivas

Escopo entregue:
- entidades `Departamento`, `CategoriaChamado` e `PrioridadeChamado` mantidas/evoluidas;
- entidades `SubcategoriaChamado`, `TipoSolicitacao` e `LocalUnidade` criadas;
- relacionamento `CategoriaChamado 1:N SubcategoriaChamado` criado;
- `DbSet` adicionados no `SGXSistemaChamadoDbContext`;
- configuracoes Fluent API criadas/ajustadas;
- migration `AddCadastrosAdministrativosSprint1` criada e aplicada no PostgreSQL.

## Entidades e campos

Observacao de padrao do projeto:
- `DataCadastro` = `CriadoEm`
- `DataAtualizacao` = `AtualizadoEm`

### Departamento
- Id
- Nome
- Sigla
- Descricao
- Ativo
- CriadoEm
- AtualizadoEm

### CategoriaChamado
- Id
- Nome
- Descricao
- DepartamentoId
- Ativo
- CriadoEm
- AtualizadoEm

### SubcategoriaChamado
- Id
- CategoriaChamadoId
- Nome
- Descricao
- Ativo
- CriadoEm
- AtualizadoEm

### PrioridadeChamado
- Id
- Nome
- Descricao
- Nivel
- Peso
- Cor
- PrazoPrimeiraRespostaHoras
- PrazoResolucaoHoras
- Ativo
- CriadoEm
- AtualizadoEm

### TipoSolicitacao
- Id
- Nome
- Descricao
- Ativo
- CriadoEm
- AtualizadoEm

### LocalUnidade
- Id
- Nome
- Descricao
- Endereco
- Ativo
- CriadoEm
- AtualizadoEm

## Banco de dados

Tabelas criadas na sprint:
- `subcategorias_chamado`
- `tipos_solicitacao`
- `locais_unidade`

Tabela evoluida:
- `prioridades_chamado` (novas colunas `peso` e `cor`)

Indices criados:
- `ux_subcategorias_chamado_categoria_nome`
- `ux_tipos_solicitacao_nome`
- `ux_locais_unidade_nome`

## Pendencias evolutivas sugeridas

- expor CRUD administrativo de `TipoSolicitacao` e `LocalUnidade`;
- avaliar vinculacao de `SubcategoriaChamado`, `TipoSolicitacao` e `LocalUnidade` na entidade `Chamado`;
- ampliar testes de integracao HTTP para os endpoints de Subcategorias;
- integrar novos cadastros no frontend administrativo.

## Sprint 2 - Backend CRUD (Departamentos, Categorias e Subcategorias)

Status da implementacao: Implementado funcionalmente  
Status tecnico: Completo com pendencias evolutivas

Escopo entregue:
- CRUD administrativo de Departamentos;
- CRUD administrativo de Categorias de chamado;
- CRUD administrativo de Subcategorias de chamado;
- validacoes de duplicidade e vinculo categoria/subcategoria;
- ativacao e inativacao logica;
- listagem com busca, filtro de status e paginacao;
- testes automatizados de use cases.

### Endpoints administrativos

Base:
- `api/admin/cadastros` (compatibilidade)
- `api/admin` (atalho sem quebrar rotas legadas)

Departamentos:
- `GET /api/admin/departamentos`
- `GET /api/admin/departamentos/{id}`
- `POST /api/admin/departamentos`
- `PUT /api/admin/departamentos/{id}`
- `DELETE /api/admin/departamentos/{id}` (inativacao logica)
- `PATCH /api/admin/departamentos/{id}/ativar`
- `PATCH /api/admin/departamentos/{id}/inativar`

Categorias:
- `GET /api/admin/categorias`
- `GET /api/admin/categorias/{id}`
- `POST /api/admin/categorias`
- `PUT /api/admin/categorias/{id}`
- `DELETE /api/admin/categorias/{id}` (inativacao logica)
- `PATCH /api/admin/categorias/{id}/ativar`
- `PATCH /api/admin/categorias/{id}/inativar`

Subcategorias:
- `GET /api/admin/subcategorias`
- `GET /api/admin/subcategorias/{id}`
- `GET /api/admin/categorias/{categoriaId}/subcategorias`
- `POST /api/admin/subcategorias`
- `PUT /api/admin/subcategorias/{id}`
- `DELETE /api/admin/subcategorias/{id}` (inativacao logica)
- `PATCH /api/admin/subcategorias/{id}/ativar`
- `PATCH /api/admin/subcategorias/{id}/inativar`

### Regras de validacao aplicadas

Departamentos:
- nome obrigatorio;
- bloqueio de duplicidade por nome/sigla;
- descricao opcional;
- preservacao historica por inativacao logica.

Categorias:
- nome obrigatorio;
- descricao opcional;
- validacao de departamento quando informado;
- preservacao historica por inativacao logica.

Subcategorias:
- nome obrigatorio;
- `CategoriaChamadoId` obrigatorio;
- categoria precisa existir;
- bloqueio de duplicidade de nome dentro da mesma categoria;
- permitido mesmo nome em categorias diferentes;
- preservacao historica por inativacao logica.

### Regra de DELETE

Nesta fase, `DELETE` segue politica segura de preservacao historica e executa inativacao logica em vez de remocao fisica.

## Sprint 3 - Backend CRUD (Prioridades, Tipos de Solicitacao e Locais/Unidades)

Status da implementacao: Implementado funcionalmente  
Status tecnico: Completo com pendencias evolutivas

Escopo entregue:
- CRUD administrativo de Prioridades com validacao de nome duplicado, peso obrigatorio (>0), cor opcional e ativacao/inativacao;
- CRUD administrativo de Tipos de Solicitacao com validacao de nome duplicado e ativacao/inativacao;
- CRUD administrativo de Locais/Unidades com validacao de nome duplicado, endereco opcional e ativacao/inativacao;
- listagens com busca por nome, filtro de status e paginacao;
- `DELETE` mantendo inativacao logica;
- aliases legados mantidos em `api/admin/cadastros/*`.

### Endpoints administrativos

Prioridades:
- `GET /api/admin/prioridades`
- `GET /api/admin/prioridades/{id}`
- `POST /api/admin/prioridades`
- `PUT /api/admin/prioridades/{id}`
- `DELETE /api/admin/prioridades/{id}` (inativacao logica)
- `PATCH /api/admin/prioridades/{id}/ativar`
- `PATCH /api/admin/prioridades/{id}/inativar`

Tipos de Solicitacao:
- `GET /api/admin/tipos-solicitacao`
- `GET /api/admin/tipos-solicitacao/{id}`
- `POST /api/admin/tipos-solicitacao`
- `PUT /api/admin/tipos-solicitacao/{id}`
- `DELETE /api/admin/tipos-solicitacao/{id}` (inativacao logica)
- `PATCH /api/admin/tipos-solicitacao/{id}/ativar`
- `PATCH /api/admin/tipos-solicitacao/{id}/inativar`

Locais/Unidades:
- `GET /api/admin/locais`
- `GET /api/admin/locais/{id}`
- `POST /api/admin/locais`
- `PUT /api/admin/locais/{id}`
- `DELETE /api/admin/locais/{id}` (inativacao logica)
- `PATCH /api/admin/locais/{id}/ativar`
- `PATCH /api/admin/locais/{id}/inativar`

### Regras de validacao aplicadas

Prioridades:
- nome obrigatorio e sem duplicidade;
- peso obrigatorio e maior que zero;
- cor opcional, com formato hexadecimal `#RRGGBB` quando informada;
- inativacao logica para preservacao historica.

Tipos de Solicitacao:
- nome obrigatorio e sem duplicidade;
- descricao opcional;
- inativacao logica para preservacao historica.

Locais/Unidades:
- nome obrigatorio e sem duplicidade;
- descricao opcional;
- endereco opcional;
- inativacao logica para preservacao historica.

### Testes da sprint

- testes de use case criados para Prioridades, Tipos de Solicitacao e Locais/Unidades;
- testes HTTP expandidos para os novos endpoints e aliases legados.

## Sprint 4 - Frontend Administrativo dos Cadastros

Status da implementacao: Implementado funcionalmente  
Status tecnico: Completo com pendencias evolutivas

Escopo entregue:
- menu `Admin > Cadastros` expandido com `Subcategorias`, `Prioridades`, `Tipos de Solicitacao` e `Locais / Unidades`;
- telas administrativas de listagem e detalhe para:
  - Departamentos
  - Categorias
  - Subcategorias
  - Prioridades
  - Tipos de Solicitacao
  - Locais / Unidades
- padrao de busca por nome e filtro por status (ativos/inativos/todos);
- acoes de criar, editar, inativar e reativar com confirmacao;
- feedback visual de sucesso/erro, loading e estado de lista vazia;
- validacoes de formulario:
  - nome obrigatorio em todos os cadastros;
  - subcategoria exige categoria;
  - prioridade exige peso > 0;
  - cor de prioridade opcional com validacao `#RRGGBB`.

### Rotas frontend criadas

- `/admin/cadastros/subcategorias`
- `/admin/cadastros/subcategorias/:id`
- `/admin/cadastros/tipos-solicitacao`
- `/admin/cadastros/tipos-solicitacao/:id`
- `/admin/cadastros/locais`
- `/admin/cadastros/locais/:id`

### Services frontend atualizados

- `src/SGX.SistemaChamado.Web/src/services/cadastrosAdminService.ts`
  - consumo preferencial de `api/admin/*`;
  - metodos adicionados para subcategorias, tipos de solicitacao e locais/unidades;
  - prioridade ajustada para `peso` e `cor`.

Pendencias evolutivas:
- integrar esses cadastros no fluxo de abertura/edicao de chamados;
- homologacao funcional final com usuarios administrativos.

## Sprint 5 - Integracao dos Cadastros com Abertura e Gestao de Chamados

Status da implementacao: Implementado funcionalmente  
Status tecnico: Completo com pendencias evolutivas

Objetivo:
Integrar os cadastros administrativos ao fluxo real de chamados (abertura, triagem, detalhe e filtros), mantendo compatibilidade historica.

### Campos integrados no Chamado

Sprint 5 preservou os campos existentes e adicionou somente os vinculos faltantes:
- `SubcategoriaId` (`subcategoria_id`)
- `TipoSolicitacaoId` (`tipo_solicitacao_id`)
- `LocalUnidadeId` (`local_unidade_id`)

Campos ja existentes e mantidos:
- `CategoriaId`
- `PrioridadeId`
- `DepartamentoId`

Migration aplicada:
- `20260515212153_Sprint5IntegracaoCadastrosChamados`

### Regras de negocio aplicadas

- novos chamados aceitam somente cadastros ativos para selecao;
- subcategoria deve pertencer a categoria selecionada;
- chamados antigos continuam exibindo nome de cadastro vinculado mesmo quando cadastro esta inativo;
- registro inativo nao aparece para novas selecoes, mas permanece para leitura historica;
- alteracao administrativa de classificacao permite atualizar categoria, subcategoria, tipo de solicitacao, local/unidade e departamento de forma incremental.

### Endpoints operacionais de selecao

Expostos para consumo sem permissao administrativa de gestao (mantendo autenticacao padrao da API):
- `GET /api/cadastros/departamentos/ativos`
- `GET /api/cadastros/categorias/ativas`
- `GET /api/cadastros/categorias/{categoriaId}/subcategorias/ativas`
- `GET /api/cadastros/prioridades/ativas`
- `GET /api/cadastros/tipos-solicitacao/ativos`
- `GET /api/cadastros/locais/ativos`

### Backend integrado na sprint

- DTOs de criacao e detalhe/listagem de chamado atualizados para novos vinculos;
- `AbrirChamadoUseCase` com validacoes de ativo e regra categoria/subcategoria;
- `ObterPortalContextoUseCase` e `ObterAdminContextoUseCase` retornando subcategorias, tipos e locais ativos;
- filtros administrativos de chamados atualizados para categoria, subcategoria, prioridade, tipo de solicitacao, departamento e local/unidade;
- detalhe administrativo e portal exibindo nomes dos novos cadastros vinculados.

### Frontend integrado na sprint

- abertura de chamado (`NovoChamadoView`) com:
  - categorias ativas;
  - subcategorias ativas filtradas por categoria;
  - prioridades ativas;
  - tipos de solicitacao ativos;
  - locais/unidades ativos;
  - departamentos ativos;
- detalhe portal e detalhe administrativo exibindo subcategoria, tipo de solicitacao e local/unidade;
- filtros administrativos ampliados com subcategoria, tipo de solicitacao e local/unidade;
- modal administrativo de alteracao de classificacao atualizado para os novos campos.

### Testes

Cobertura backend atualizada com cenarios de Sprint 5:
- criacao com subcategoria/tipo/local ativos;
- bloqueio de subcategoria de outra categoria;
- bloqueio de subcategoria/tipo/local inativos;
- contexto portal com apenas ativos;
- filtros administrativos por subcategoria, tipo e local;
- detalhe administrativo preservando exibicao historica de cadastros inativos vinculados.

## Sprint 6 - Testes, Seed Inicial, Refinamento e Fechamento da Documentacao

Status da implementacao: Implementado funcionalmente  
Status tecnico: Completo com pendencias evolutivas

Objetivo:
Consolidar o modulo de cadastros administrativos com seed inicial, reforco de testes e fechamento documental para validacao funcional.

### Seed inicial consolidado

A seed inicial foi centralizada no fluxo de inicializacao em desenvolvimento (`DevelopmentSeedService`) com comportamento idempotente (sem duplicar registros existentes):

- Prioridades consolidadas no seed de estabilizacao: `Baixa` (peso `1`, cor `#22C55E`), `Media` (peso `2`, cor `#EAB308`), `Alta` (peso `3`, cor `#F97316`) e `Critica` (peso `4`, cor `#EF4444`);
- Tipos de Solicitacao: `Incidente`, `Solicitacao de Servico`, `Duvida`, `Melhoria`, `Problema Recorrente`;
- Categorias: `Hardware`, `Software`, `Rede`, `Sistema`, `Acesso`, `E-mail`, `Impressora`, `Telefonia`, `Solicitacao Administrativa` (alem da categoria tecnica `Suporte Tecnico` usada para apoio operacional);
- Subcategorias minimas por categoria (Hardware, Software, Rede, Sistema, Acesso, E-mail, Impressora, Telefonia e Solicitacao Administrativa) com vinculo consistente `categoria -> subcategoria`;
- Departamentos: `Tecnologia da Informacao`, `Recursos Humanos`, `Financeiro`, `Juridico`, `Atendimento`, `Infraestrutura`;
- Locais/Unidades: `Sede`, `Filial`, `Inspetoria`, `Datacenter`, `Almoxarifado`, `Atendimento Externo`.

Regras aplicadas no seed:
- nao duplica por variacoes de acentuacao/normalizacao de nome;
- nao remove nem altera registros historicos existentes;
- nao reativa automaticamente cadastros inativados manualmente.

### Testes consolidados na sprint

Backend:
- testes do `DevelopmentSeedService` ampliados para garantir seed inicial e ausencia de duplicidade;
- testes HTTP adicionados para validar endpoints operacionais `/api/cadastros/*` retornando apenas registros ativos;
- validacao de subcategoria operacional filtrada por categoria via endpoint ativo.

Frontend:
- nao ha suite automatizada padrao de testes frontend no projeto nesta iteracao;
- validacao permaneceu por build e cobertura backend/HTTP.

### Criterios de aceite da Sprint 6

- [x] seed inicial aplicado sem duplicidade;
- [x] testes automatizados backend atualizados e executando com sucesso;
- [x] fluxo operacional com ativos/inativos validado em casos de uso e endpoints;
- [x] documentacao final atualizada;
- [x] roadmap atualizado para fechamento da sprint.

## Sprint 7 - Checklist Funcional, Homologacao e Ajustes Finais

Status da implementacao: Implementado funcionalmente  
Status tecnico: Completo com pendencias evolutivas

Objetivo:
Executar uma rodada de validacao funcional do modulo de cadastros administrativos no fluxo de chamados e aplicar ajustes finos sem alterar arquitetura.

### Checklist de homologacao funcional

Cadastros administrativos:
- [x] listagens administrativas validadas para Departamentos, Categorias, Subcategorias, Prioridades, Tipos de Solicitacao e Locais/Unidades;
- [x] criacao, edicao, inativacao e reativacao validadas nos use cases administrativos;
- [x] busca por nome validada;
- [x] filtro por status `Ativos`, `Inativos` e `Todos` validado por testes.

Validacoes:
- [x] bloqueio de duplicidade para departamentos, categorias, subcategorias (na mesma categoria), tipos e locais;
- [x] bloqueio de peso invalido de prioridade (`0` e negativo);
- [x] validacao de cor de prioridade no formato `#RRGGBB`;
- [x] categoria obrigatoria para criacao de subcategoria.

Fluxo de abertura de chamado:
- [x] abertura com cadastros ativos validada;
- [x] subcategoria filtrada e consistente com categoria;
- [x] bloqueio de subcategoria fora da categoria;
- [x] bloqueio de cadastros inativos em novas operacoes;
- [x] contexto do portal retorna apenas ativos.

Administracao e historico:
- [x] filtros administrativos por categoria, subcategoria, prioridade, tipo de solicitacao e local/unidade validados;
- [x] detalhe administrativo e portal exibem nomes dos cadastros vinculados;
- [x] chamados antigos preservam leitura historica de cadastro inativo vinculado.

### Evidencias registradas

Backend:
- testes de use case administrativos de cadastros;
- testes de abertura/detalhe/listagem de chamados;
- testes de seed de cadastros e seed idempotente;
- testes de validadores de cadastros (formato de cor e categoria obrigatoria).

Documentacao e operacao:
- endpoints operacionais `GET /api/cadastros/*` mantidos para selecao de ativos;
- regras de ativos/inativos e preservacao historica consolidadas nesta documentacao.

### Pendencias evolutivas

- validacao manual com evidencias visuais (prints por tela) em ambiente de homologacao institucional;
- consolidacao futura de suite automatizada frontend/E2E para checklist visual ponta a ponta.

## Sprint 8 - Consolidacao ITSM e Checklist Formal de Homologacao

Status da implementacao: Implementado funcionalmente  
Status tecnico: Completo com pendencias evolutivas

Objetivo:
Consolidar a trilha documental do modulo de Cadastros Administrativos em Gestao ITSM, com checklist formal para homologacao institucional.

### Escopo documental da sprint

- criacao de `docs/ITSM-CADASTROS-ADMINISTRATIVOS.md` com visao consolidada do item em ITSM;
- criacao de `docs/CHECKLIST-HOMOLOGACAO-CADASTROS.md` para execucao guiada de homologacao;
- atualizacao dos roadmaps (`docs/ROADMAP.md` e `docs/ROADMAP-ITSM.md`) com registro da Sprint 8;
- consolidacao do status do modulo como implementado tecnicamente, com pendencia de aceite formal em ambiente institucional.

### Checklist Sprint 8

- [x] documento ITSM de cadastros administrativos criado
- [x] checklist formal de homologacao criado
- [x] roadmap geral atualizado com a sprint
- [x] roadmap ITSM atualizado com a sprint
- [x] trilha de cadastros administrativos atualizada

### Pendencias evolutivas

- executar homologacao manual institucional completa com os perfis Administrador, Atendente e Solicitante;
- anexar evidencias visuais por item do checklist;
- registrar aceite formal de negocio para encerramento do ciclo.
