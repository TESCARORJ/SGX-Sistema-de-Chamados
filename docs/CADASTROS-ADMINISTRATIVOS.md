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

## Consolidado documental do modulo (planejamento tecnico)

### Visao geral do modulo

O modulo de Cadastros Administrativos organiza classificacao, priorizacao, triagem, filtros administrativos e preservacao historica dos chamados, servindo como base operacional para evolucoes ITSM.

### Cadastros previstos

- Departamentos
- Categorias de chamado
- Subcategorias de chamado
- Prioridades
- Tipos de solicitacao
- Locais / Unidades

### Regras de negocio

- subcategoria deve pertencer a uma categoria valida;
- subcategoria escolhida deve ser compativel com a categoria selecionada;
- prioridade deve conter nome e peso, com cor opcional;
- operacoes de exclusao devem priorizar inativacao logica;
- novos fluxos devem considerar apenas registros ativos;
- historico de chamados deve ser preservado independentemente de inativacao.

### Endpoints administrativos

Departamentos:
- `GET /api/admin/departamentos`
- `GET /api/admin/departamentos/{id}`
- `POST /api/admin/departamentos`
- `PUT /api/admin/departamentos/{id}`
- `DELETE /api/admin/departamentos/{id}`
- `PATCH /api/admin/departamentos/{id}/ativar`
- `PATCH /api/admin/departamentos/{id}/inativar`

Categorias:
- `GET /api/admin/categorias`
- `GET /api/admin/categorias/{id}`
- `POST /api/admin/categorias`
- `PUT /api/admin/categorias/{id}`
- `DELETE /api/admin/categorias/{id}`
- `PATCH /api/admin/categorias/{id}/ativar`
- `PATCH /api/admin/categorias/{id}/inativar`

Subcategorias:
- `GET /api/admin/subcategorias`
- `GET /api/admin/subcategorias/{id}`
- `GET /api/admin/categorias/{categoriaId}/subcategorias`
- `POST /api/admin/subcategorias`
- `PUT /api/admin/subcategorias/{id}`
- `DELETE /api/admin/subcategorias/{id}`
- `PATCH /api/admin/subcategorias/{id}/ativar`
- `PATCH /api/admin/subcategorias/{id}/inativar`

Prioridades:
- `GET /api/admin/prioridades`
- `GET /api/admin/prioridades/{id}`
- `POST /api/admin/prioridades`
- `PUT /api/admin/prioridades/{id}`
- `DELETE /api/admin/prioridades/{id}`
- `PATCH /api/admin/prioridades/{id}/ativar`
- `PATCH /api/admin/prioridades/{id}/inativar`

Tipos de solicitacao:
- `GET /api/admin/tipos-solicitacao`
- `GET /api/admin/tipos-solicitacao/{id}`
- `POST /api/admin/tipos-solicitacao`
- `PUT /api/admin/tipos-solicitacao/{id}`
- `DELETE /api/admin/tipos-solicitacao/{id}`
- `PATCH /api/admin/tipos-solicitacao/{id}/ativar`
- `PATCH /api/admin/tipos-solicitacao/{id}/inativar`

Locais / Unidades:
- `GET /api/admin/locais`
- `GET /api/admin/locais/{id}`
- `POST /api/admin/locais`
- `PUT /api/admin/locais/{id}`
- `DELETE /api/admin/locais/{id}`
- `PATCH /api/admin/locais/{id}/ativar`
- `PATCH /api/admin/locais/{id}/inativar`

### Endpoints operacionais

- `GET /api/cadastros/departamentos/ativos`
- `GET /api/cadastros/categorias/ativas`
- `GET /api/cadastros/categorias/{categoriaId}/subcategorias/ativas`
- `GET /api/cadastros/prioridades/ativas`
- `GET /api/cadastros/tipos-solicitacao/ativos`
- `GET /api/cadastros/locais/ativos`

### Telas administrativas

Menu esperado:

```text
Admin
 └── Cadastros
      ├── Departamentos
      ├── Categorias
      ├── Subcategorias
      ├── Prioridades
      ├── Tipos de Solicitacao
      └── Locais / Unidades
```

Cada tela deve disponibilizar listagem, busca por nome, filtro por status, criar, editar, ativar, inativar, mensagens de sucesso/erro, tratamento de carregamento e estado de lista vazia.

### Integracao esperada com chamados

- abertura de chamado deve consumir somente cadastros ativos;
- validacao de coerencia categoria/subcategoria deve ocorrer na entrada;
- atendimento administrativo deve usar os campos de cadastro para triagem;
- filtros de chamados devem suportar categoria, subcategoria, prioridade, tipo e local/unidade.

### Seed inicial sugerido

Prioridades:
- Baixa - Peso 1 - Cor #22C55E
- Media - Peso 2 - Cor #EAB308
- Alta - Peso 3 - Cor #F97316
- Critica - Peso 4 - Cor #EF4444

Tipos de solicitacao:
- Incidente
- Solicitacao de Servico
- Duvida
- Melhoria
- Problema Recorrente

Categorias:
- Hardware
- Software
- Rede
- Sistema
- Acesso
- E-mail
- Impressora
- Telefonia
- Solicitacao Administrativa

Departamentos:
- Tecnologia da Informacao
- Recursos Humanos
- Financeiro
- Juridico
- Atendimento
- Infraestrutura

Locais / Unidades:
- Sede
- Filial
- Inspetoria
- Datacenter
- Almoxarifado
- Atendimento Externo

### Regras de ativo/inativo

- apenas ativos participam de novas operacoes;
- inativos nao aparecem em seletores de abertura e edicao;
- inativacao nao remove historico existente;
- reativacao torna o cadastro elegivel novamente para novas operacoes.

### Preservacao historica

Chamados antigos devem continuar exibindo nomes dos cadastros vinculados, mesmo que esses cadastros tenham sido inativados posteriormente.

### Checklist de validacao

Checklist funcional detalhado publicado em `docs/CHECKLIST-HOMOLOGACAO-CADASTROS.md`.

### Pendencias evolutivas

- homologacao manual institucional com evidencias formais;
- evolucao para catalogo de servicos;
- evolucao de SLA avancado por prioridade/categoria/tipo;
- ampliacao de testes E2E frontend.

## Item 3 - Implementar backend dos cadastros

Status da implementacao: Backend implementado  
Status tecnico: Backend validado

Objetivo da etapa:
Concluir o backend dos cadastros administrativos estruturais do SGX, com CRUD administrativo, validacoes de negocio, inativacao logica e cobertura de testes.

Escopo backend confirmado:
- entidades de `Departamento`, `CategoriaChamado`, `SubcategoriaChamado`, `PrioridadeChamado`, `TipoSolicitacao` e `LocalUnidade` disponiveis;
- configuracoes EF Core e `DbSet` no `SGXSistemaChamadoDbContext` disponiveis;
- migrations incrementais de cadastros disponiveis;
- DTOs/requests/responses de cadastros administrativos disponiveis;
- validators de criacao/atualizacao/filtro disponiveis;
- use cases administrativos de listagem, detalhe, criacao, atualizacao, inativacao e reativacao disponiveis;
- controller administrativo com endpoints REST de cadastros disponivel;
- validacoes de duplicidade aplicadas por cadastro;
- regra de subcategoria por categoria aplicada;
- regra de prioridade com peso > 0 e cor no formato `#RRGGBB` aplicada;
- `DELETE` tratado como inativacao logica para preservacao historica;
- listagem com busca textual e filtro por status (`Ativo`, `Inativo`, `Todos`) disponivel.

Endpoints administrativos implementados nesta etapa:
- Departamentos: `GET/POST/PUT/DELETE/PATCH ativar/inativar` em `/api/admin/departamentos`
- Categorias: `GET/POST/PUT/DELETE/PATCH ativar/inativar` em `/api/admin/categorias`
- Subcategorias: `GET/POST/PUT/DELETE/PATCH ativar/inativar` em `/api/admin/subcategorias`
- Subcategorias por categoria: `GET /api/admin/categorias/{categoriaId}/subcategorias`
- Prioridades: `GET/POST/PUT/DELETE/PATCH ativar/inativar` em `/api/admin/prioridades`
- Tipos de Solicitacao: `GET/POST/PUT/DELETE/PATCH ativar/inativar` em `/api/admin/tipos-solicitacao`
- Locais / Unidades: `GET/POST/PUT/DELETE/PATCH ativar/inativar` em `/api/admin/locais`

Validacao tecnica executada:
- `dotnet build SGX.SistemaChamado.sln -c Release`
- `dotnet test tests/SGX.SistemaChamado.Tests/SGX.SistemaChamado.Tests.csproj -c Release`

Resultado da validacao:
- build concluido com sucesso;
- testes concluidos com sucesso (`420` aprovados, `0` falhas).

Pendencias apos conclusao do backend:
- implementar/fechar etapa frontend administrativo dos cadastros (concluido no Item 4);
- concluir integracao ponta a ponta no fluxo operacional com homologacao institucional.

## Item 4 - Frontend administrativo dos cadastros

Status da implementacao: Frontend administrativo implementado  
Status tecnico: Frontend validado

Objetivo da etapa:
Validar e consolidar o frontend administrativo de cadastros no menu `Admin > Cadastros`, mantendo padrao visual, contratos de API e regras de validacao.

Escopo frontend validado:
- menu `Admin > Cadastros` com entradas para Departamentos, Categorias, Subcategorias, Prioridades, Tipos de Solicitacao e Locais / Unidades;
- rotas de listagem e detalhe para os seis cadastros administrativos;
- services centralizados em `cadastrosAdminService` consumindo endpoints administrativos;
- tabelas com busca por nome, filtro de status (`Ativos`, `Inativos`, `Todos`) e paginacao;
- acoes de criar, editar, inativar e reativar com confirmacao;
- feedback de sucesso/erro, estados de carregamento e lista vazia;
- validacoes de formulario:
  - nome obrigatorio em todos os cadastros;
  - subcategoria exige categoria;
  - prioridade exige peso > 0;
  - cor de prioridade opcional com validacao `#RRGGBB`;
  - endereco e descricao opcionais quando aplicavel;
- indicacao visual de ativo/inativo nas listagens e detalhes.

Pendencias apos conclusao do frontend administrativo:
- integrar cadastros com abertura e gestao de chamados na etapa operacional completa (concluido no Item 5);
- executar homologacao institucional com evidencias visuais formais;
- consolidar cobertura frontend E2E da trilha de cadastros.

## Item 5 - Integrar cadastros com abertura de chamados

Status da implementacao: Integrado ao fluxo de chamados  
Status tecnico: Integracao validada

Objetivo da etapa:
Conectar os cadastros administrativos ao fluxo real de abertura, detalhe, triagem e filtros administrativos de chamados, sem quebrar compatibilidade historica.

Integracao encontrada e validada:
- entidade `Chamado` com vinculos `DepartamentoId`, `CategoriaId`, `SubcategoriaId`, `PrioridadeId`, `TipoSolicitacaoId` e `LocalUnidadeId`;
- FKs opcionais para `Subcategoria`, `TipoSolicitacao` e `LocalUnidade` ja adicionadas por migration incremental `Sprint5IntegracaoCadastrosChamados`;
- use case de abertura (`AbrirChamadoUseCase`) validando cadastro ativo e coerencia `categoria x subcategoria`;
- detalhe/listagem portal e admin retornando ids e nomes dos cadastros vinculados;
- alteracao de classificacao administrativa (`AlterarCategoriaChamadoUseCase`) validando ativos e compatibilidade;
- filtros administrativos por categoria, subcategoria, prioridade, tipo, local e departamento aplicados no backend;
- frontend de abertura (`NovoChamadoView`) carregando apenas ativos e limpando subcategoria invalida ao trocar categoria;
- detalhes (`DetalheChamadoView` e `AdminDetalheChamadoView`) exibindo nomes de subcategoria, tipo, local e departamento;
- filtros administrativos (`FiltrosChamadoAdmin`) integrados com categoria/subcategoria/tipo/local/departamento.

Endpoints operacionais validados:
- `GET /api/cadastros/departamentos/ativos`
- `GET /api/cadastros/categorias/ativas`
- `GET /api/cadastros/categorias/{categoriaId}/subcategorias/ativas`
- `GET /api/cadastros/prioridades/ativas`
- `GET /api/cadastros/tipos-solicitacao/ativos`
- `GET /api/cadastros/locais/ativos`

Regras validadas:
- novas operacoes usam apenas cadastros ativos;
- subcategoria precisa pertencer a categoria selecionada;
- cadastro inativo nao e aceito na abertura de novo chamado;
- chamados antigos preservam leitura historica dos nomes vinculados.

Cobertura de testes validada:
- testes de `AbrirChamadoUseCase` para categoria/subcategoria/prioridade/tipo/local ativos e bloqueios de inativos/inconsistencia;
- testes de `ObterPortalContextoUseCase` para retorno de ativos;
- testes de `DetalharMeuChamadoUseCase` e `DetalharChamadoAdminUseCase` com exibicao de nomes;
- testes de `ListarChamadosAdminUseCase` com filtros por subcategoria/tipo/local;
- testes HTTP de endpoints `/api/cadastros/*` ativos e subcategorias por categoria.

Pendencias apos conclusao do Item 5:
- homologacao funcional institucional completa com evidencias formais;
- consolidacao de suite frontend E2E para fluxos visuais ponta a ponta.

## Item 6 - Criar seed inicial dos cadastros administrativos

Status da implementacao: Seed inicial criado  
Status tecnico: Seed validado

Objetivo da etapa:
Consolidar a massa inicial dos cadastros administrativos com comportamento idempotente, preservacao de dados existentes e compatibilidade historica.

Seed encontrado e validado:
- `DevelopmentSeedService` ja consolida prioridades, tipos de solicitacao, categorias, subcategorias, departamentos e locais/unidades padrao;
- prioridades padrao mantidas com `Peso` e `Cor` (`Baixa`, `Media`, `Alta`, `Critica`);
- subcategorias padrao vinculadas corretamente as categorias correspondentes;
- seed cria somente registros ausentes e preserva dados existentes.

Regras de seed validadas:
- idempotencia em reexecucoes (sem duplicidade);
- normalizacao de nomes para evitar duplicidade com variacoes de acentuacao;
- nao reativa automaticamente cadastros inativados manualmente;
- nao sobrescreve registros existentes de forma destrutiva.

Cobertura de testes validada:
- testes do `DevelopmentSeedService` cobrindo criacao dos cadastros padrao;
- validacao de `Peso` e `Cor` das prioridades padrao;
- validacao de vinculo `categoria -> subcategoria`;
- validacao de nao duplicidade em multiplas execucoes;
- validacao de preservacao de dados e de inativacoes manuais.

Pendencias apos conclusao do Item 6:
- executar homologacao funcional institucional completa com evidencias formais;
- consolidar suite frontend E2E para fluxos visuais ponta a ponta;
- avaliar evolucao futura para seed institucional configuravel por ambiente.

## Item 7 - Validar fluxo funcional dos cadastros administrativos

Status da implementacao: Fluxo funcional validado  
Status tecnico: Validacao funcional concluida

Objetivo da etapa:
Executar a validacao funcional tecnica do modulo completo de cadastros administrativos, sem introduzir novas funcionalidades, apenas confirmando comportamento esperado e criterios de aceite.

Validacao encontrada e executada:
- fluxo administrativo dos seis cadastros validado (listagem, busca, filtro por status, criar, editar, inativar e reativar) via suite de use cases e integracao HTTP;
- validacoes de negocio confirmadas (nome obrigatorio, bloqueio de duplicidade, categoria obrigatoria para subcategoria, peso > 0 e cor `#RRGGBB` para prioridade);
- fluxo de abertura de chamado validado com carregamento de cadastros ativos, coerencia `categoria x subcategoria` e envio de identificadores;
- regras de ativo/inativo confirmadas para novas operacoes;
- preservacao historica confirmada para chamados antigos com cadastros posteriormente inativados;
- filtros administrativos por categoria, subcategoria, prioridade, tipo de solicitacao e local/unidade confirmados;
- seed inicial mantido idempotente e sem reativacao automatica de registros inativados manualmente.

Cobertura de testes revisada:
- `AbrirChamadoUseCaseTests` para regras de abertura e bloqueios de inativos/inconsistencia;
- `DevelopmentSeedServiceTests` para seed inicial, idempotencia, variacao de acento e preservacao de inativos;
- suites administrativas de cadastros (`Departamentos`, `Categorias`, `Subcategorias`, `Prioridades`, `TiposSolicitacao`, `LocaisUnidades`) para CRUD, duplicidade e filtros;
- `ApiHttpIntegrationTests` para endpoints administrativos e endpoints operacionais `/api/cadastros/*` retornando apenas ativos.

Pendencias apos conclusao do Item 7:
- nao ha pendencias tecnicas bloqueantes identificadas para o modulo;
- manter como evolucao futura a cobertura frontend E2E completa;
- avaliar futuramente se status de chamado continuara como fluxo controlado ou se sera parametrizado em cadastro proprio.

## Item 8 - Homologar cadastros administrativos em ambiente institucional

Status da implementacao: Fluxo funcional validado  
Status tecnico: Aguardando homologacao institucional

Objetivo da etapa:
Preparar e registrar a homologacao institucional/manual do modulo, sem alteracao de codigo, mantendo coerencia entre checklist, evidencias e roadmap.

Resultado desta etapa documental:
- roteiro formal de homologacao manual consolidado no checklist;
- evidencias obrigatorias listadas para coleta institucional;
- campos de responsavel, data, ambiente e resultado mantidos para preenchimento real;
- status do roadmap mantido em `90%` e `7/8`, pois nao ha evidencia institucional anexada nesta etapa.

Situacao atual do roadmap (ordem 8):
Modulo de Cadastros Administrativos implementado e validado funcionalmente em nivel tecnico. Backend, frontend administrativo, integracao com abertura/gestao de chamados, seed inicial e validacao funcional foram concluidos. A homologacao institucional/manual com evidencias formais permanece pendente.

Roteiro minimo consolidado:
- validacao completa das telas `Admin > Cadastros` (departamentos, categorias, subcategorias, prioridades, tipos e locais/unidades);
- operacoes de criar, editar, inativar e reativar em todos os cadastros;
- validacao de busca/filtros (`Ativos`, `Inativos`, `Todos`);
- abertura de chamado com cadastros ativos e coerencia categoria/subcategoria;
- validacao de bloqueio de inativos em novas operacoes;
- validacao de detalhe/filtros administrativos e preservacao historica.

Evidencias institucionais esperadas:
- prints do menu e listagens de cadastros;
- print de criacao/edicao e de registro inativo com destaque visual;
- prints da abertura de chamado, selecao categoria/subcategoria e detalhe;
- print dos filtros administrativos;
- registro de responsavel, data, ambiente e resultado final.

Pendencias apos conclusao do Item 8 (documental):
- executar homologacao institucional/manual com usuario responsavel;
- anexar evidencias formais do checklist;
- atualizar status para `Homologado institucionalmente` somente apos aprovacao real.
