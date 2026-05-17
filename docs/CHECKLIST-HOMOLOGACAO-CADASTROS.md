# Checklist de Homologação — Cadastros Administrativos

## Identificação da validação

- Responsavel:
- Data:
- Ambiente:
- Versao/branch:
- Observacoes gerais:

## Legenda

- Aprovado
- Reprovado
- Nao aplicavel
- Pendente

## Status da homologacao institucional (Item 8)

- Situacao atual: Pendente.
- Motivo: nao foram anexadas evidencias manuais institucionais nesta etapa documental.
- Regra aplicada: sem evidencias reais, o item permanece em `7/8` e nao pode ser marcado como homologado.
- Proxima acao: executar homologacao institucional/manual com evidencias formais (prints, responsavel, data, ambiente e resultado).

## Roteiro formal de homologacao manual

1. Acessar `Admin > Cadastros`.
2. Validar tela de `Departamentos`.
3. Validar tela de `Categorias`.
4. Validar tela de `Subcategorias`.
5. Validar tela de `Prioridades`.
6. Validar tela de `Tipos de Solicitacao`.
7. Validar tela de `Locais / Unidades`.
8. Criar um registro de teste em cada cadastro.
9. Editar um registro de teste em cada cadastro.
10. Inativar um registro de teste em cada cadastro.
11. Reativar um registro de teste em cada cadastro.
12. Validar busca por nome.
13. Validar filtro `Ativos`.
14. Validar filtro `Inativos`.
15. Validar filtro `Todos`.
16. Abrir chamado com categoria ativa.
17. Abrir chamado com subcategoria vinculada a categoria.
18. Validar que a subcategoria muda ao trocar categoria.
19. Abrir chamado com prioridade ativa.
20. Abrir chamado com tipo de solicitacao ativo.
21. Abrir chamado com local/unidade ativo.
22. Validar que cadastros inativos nao aparecem em novas operacoes.
23. Validar detalhe do chamado exibindo nomes dos cadastros.
24. Validar filtros administrativos por cadastros.
25. Validar que chamado antigo mantem historico mesmo com cadastro inativo.

## Evidencias esperadas na homologacao institucional

- Print do menu `Admin > Cadastros`.
- Print da listagem de cada cadastro.
- Print de criacao ou edicao de pelo menos um cadastro.
- Print de registro inativo com diferenciacao visual.
- Print da tela de novo chamado com cadastros carregados.
- Print da selecao categoria/subcategoria.
- Print do detalhe do chamado com cadastros exibidos.
- Print dos filtros administrativos.
- Print ou log de validacoes tecnicas (build/test), se necessario.
- Registro do responsavel pela homologacao.
- Data da homologacao.
- Ambiente utilizado.
- Resultado final: `Aprovado`, `Aprovado com ressalvas` ou `Reprovado`.

## Registro da execucao institucional

- Responsavel pela execucao:
- Data da execucao:
- Ambiente:
- Versao/branch:
- Resultado:
- Evidencias anexadas (links/caminhos):
- Observacoes:

## Checklist de cadastros administrativos

| Item | Resultado esperado | Status | Observacoes |
|---|---|---|---|
| Acessar menu Admin > Cadastros | Menu administrativo de cadastros acessivel para perfil autorizado |  |  |
| Listar Departamentos | Listagem carregada com dados e estado vazio quando aplicavel |  |  |
| Criar Departamento | Registro salvo com sucesso e exibido na listagem |  |  |
| Editar Departamento | Alteracoes persistidas e refletidas na listagem/detalhe |  |  |
| Inativar Departamento | Registro inativado sem exclusao fisica |  |  |
| Reativar Departamento | Registro volta a estado ativo |  |  |
| Listar Categorias | Listagem carregada com dados e estado vazio quando aplicavel |  |  |
| Criar Categoria | Registro salvo com sucesso e exibido na listagem |  |  |
| Editar Categoria | Alteracoes persistidas e refletidas na listagem/detalhe |  |  |
| Inativar Categoria | Registro inativado sem exclusao fisica |  |  |
| Reativar Categoria | Registro volta a estado ativo |  |  |
| Listar Subcategorias | Listagem carregada com dados e estado vazio quando aplicavel |  |  |
| Criar Subcategoria vinculada a Categoria | Registro salvo com categoria valida associada |  |  |
| Editar Subcategoria | Alteracoes persistidas e refletidas na listagem/detalhe |  |  |
| Inativar Subcategoria | Registro inativado sem exclusao fisica |  |  |
| Reativar Subcategoria | Registro volta a estado ativo |  |  |
| Listar Prioridades | Listagem carregada com dados e estado vazio quando aplicavel |  |  |
| Criar Prioridade com peso e cor | Registro salvo com peso valido e cor valida/opcional |  |  |
| Editar Prioridade | Alteracoes persistidas e refletidas na listagem/detalhe |  |  |
| Inativar Prioridade | Registro inativado sem exclusao fisica |  |  |
| Reativar Prioridade | Registro volta a estado ativo |  |  |
| Listar Tipos de Solicitação | Listagem carregada com dados e estado vazio quando aplicavel |  |  |
| Criar Tipo de Solicitação | Registro salvo com sucesso e exibido na listagem |  |  |
| Editar Tipo de Solicitação | Alteracoes persistidas e refletidas na listagem/detalhe |  |  |
| Inativar Tipo de Solicitação | Registro inativado sem exclusao fisica |  |  |
| Reativar Tipo de Solicitação | Registro volta a estado ativo |  |  |
| Listar Locais / Unidades | Listagem carregada com dados e estado vazio quando aplicavel |  |  |
| Criar Local / Unidade | Registro salvo com sucesso e exibido na listagem |  |  |
| Editar Local / Unidade | Alteracoes persistidas e refletidas na listagem/detalhe |  |  |
| Inativar Local / Unidade | Registro inativado sem exclusao fisica |  |  |
| Reativar Local / Unidade | Registro volta a estado ativo |  |  |
| Validar busca por nome | Busca retorna itens coerentes com termo informado |  |  |
| Validar filtro Ativos | Listagem exibe apenas registros ativos |  |  |
| Validar filtro Inativos | Listagem exibe apenas registros inativos |  |  |
| Validar filtro Todos | Listagem exibe ativos e inativos |  |  |

## Checklist de validacoes

| Item | Resultado esperado | Status | Observacoes |
|---|---|---|---|
| Bloquear nome duplicado de Departamento | Nao permite salvar registro duplicado |  |  |
| Bloquear nome duplicado de Categoria | Nao permite salvar registro duplicado |  |  |
| Bloquear Subcategoria duplicada dentro da mesma Categoria | Nao permite salvar registro duplicado no mesmo contexto |  |  |
| Bloquear Prioridade com peso zero | Nao permite salvar peso invalido |  |  |
| Bloquear Prioridade com peso negativo | Nao permite salvar peso invalido |  |  |
| Validar cor no formato #RRGGBB | Cor invalida deve ser rejeitada |  |  |
| Bloquear Tipo de Solicitação duplicado | Nao permite salvar registro duplicado |  |  |
| Bloquear Local / Unidade duplicado | Nao permite salvar registro duplicado |  |  |
| Exigir nome obrigatorio em todos os cadastros | Nao permite salvar sem nome |  |  |
| Exigir Categoria ao criar Subcategoria | Nao permite salvar sem categoria valida |  |  |

## Checklist de abertura de chamado

| Item | Resultado esperado | Status | Observacoes |
|---|---|---|---|
| Carregar Categorias ativas | Campo exibe apenas categorias ativas |  |  |
| Carregar Subcategorias conforme Categoria selecionada | Subcategoria respeita filtro da categoria |  |  |
| Carregar Prioridades ativas | Campo exibe apenas prioridades ativas |  |  |
| Carregar Tipos de Solicitação ativos | Campo exibe apenas tipos ativos |  |  |
| Carregar Locais / Unidades ativos | Campo exibe apenas locais/unidades ativos |  |  |
| Carregar Departamentos ativos, se aplicavel | Campo exibe apenas departamentos ativos |  |  |
| Criar chamado com Categoria | Chamada salva com categoria valida |  |  |
| Criar chamado com Subcategoria | Chamada salva com subcategoria valida |  |  |
| Criar chamado com Prioridade | Chamada salva com prioridade valida |  |  |
| Criar chamado com Tipo de Solicitação | Chamada salva com tipo valido |  |  |
| Criar chamado com Local / Unidade | Chamada salva com local/unidade valido |  |  |
| Bloquear Subcategoria que nao pertence a Categoria selecionada | Operacao rejeitada com mensagem adequada |  |  |
| Bloquear cadastro inativo em nova abertura | Operacao rejeitada ou opcao indisponivel |  |  |

## Checklist de filtros administrativos

| Item | Resultado esperado | Status | Observacoes |
|---|---|---|---|
| Filtrar chamados por Categoria | Listagem retorna apenas chamados compativeis |  |  |
| Filtrar chamados por Subcategoria | Listagem retorna apenas chamados compativeis |  |  |
| Filtrar chamados por Prioridade | Listagem retorna apenas chamados compativeis |  |  |
| Filtrar chamados por Tipo de Solicitação | Listagem retorna apenas chamados compativeis |  |  |
| Filtrar chamados por Local / Unidade | Listagem retorna apenas chamados compativeis |  |  |
| Validar listagem apos aplicacao dos filtros | Resultado e coerente com filtros ativos |  |  |
| Limpar filtros e restaurar listagem | Listagem retorna ao estado padrao |  |  |

## Checklist de registros ativos e inativos

| Item | Resultado esperado | Status | Observacoes |
|---|---|---|---|
| Registro ativo aparece em novas operacoes | Seletores operacionais mostram item ativo |  |  |
| Registro inativo nao aparece em novas operacoes | Seletores operacionais ocultam item inativo |  |  |
| Registro inativo permanece visivel no historico se ja estiver vinculado a chamado | Historico continua legivel |  |  |
| Inativar cadastro nao apaga chamados antigos | Integridade historica preservada |  |  |
| Reativar cadastro torna o item disponivel novamente para novas operacoes | Item volta aos seletores operacionais |  |  |

## Checklist de historico

| Item | Resultado esperado | Status | Observacoes |
|---|---|---|---|
| Chamado antigo exibe Categoria inativa vinculada | Nome da categoria continua visivel no detalhe |  |  |
| Chamado antigo exibe Subcategoria inativa vinculada | Nome da subcategoria continua visivel no detalhe |  |  |
| Chamado antigo exibe Prioridade inativa vinculada | Nome da prioridade continua visivel no detalhe |  |  |
| Chamado antigo exibe Tipo de Solicitação inativo vinculado | Nome do tipo continua visivel no detalhe |  |  |
| Chamado antigo exibe Local / Unidade inativo vinculado | Nome do local/unidade continua visivel no detalhe |  |  |
| Chamado antigo exibe Departamento inativo vinculado, se aplicavel | Nome do departamento continua visivel no detalhe |  |  |

## Checklist de permissoes

| Item | Resultado esperado | Status | Observacoes |
|---|---|---|---|
| Administrador acessa manutencao de cadastros | Acesso permitido com permissoes adequadas |  |  |
| Usuario sem permissao administrativa nao acessa manutencao de cadastros | Acesso bloqueado |  |  |
| Usuario autenticado acessa dados operacionais necessarios para abertura de chamado | Endpoints operacionais disponiveis conforme regra |  |  |
| Endpoints de mutacao exigem permissao administrativa | Operacoes de escrita bloqueadas sem permissao |  |  |

## Checklist de seed

| Item | Resultado esperado | Status | Observacoes |
|---|---|---|---|
| Seed cria Prioridades padrao | Registros padrao disponiveis apos seed |  |  |
| Seed cria Tipos de Solicitação padrao | Registros padrao disponiveis apos seed |  |  |
| Seed cria Categorias padrao | Registros padrao disponiveis apos seed |  |  |
| Seed cria Subcategorias padrao | Registros padrao disponiveis apos seed |  |  |
| Seed cria Departamentos padrao | Registros padrao disponiveis apos seed |  |  |
| Seed cria Locais / Unidades padrao | Registros padrao disponiveis apos seed |  |  |
| Seed nao duplica registros em nova execucao | Reexecucao mantem consistencia sem duplicidade |  |  |
| Seed preserva cadastros inativados manualmente | Seed nao reativa nem sobrescreve inativos manuais |  |  |

## Checklist de build e testes

| Item | Resultado esperado | Status | Observacoes |
|---|---|---|---|
| Executar dotnet build SGX.SistemaChamado.sln -c Release | Build concluido sem erro |  |  |
| Executar dotnet test tests/SGX.SistemaChamado.Tests/SGX.SistemaChamado.Tests.csproj -c Release | Suite de testes executada com sucesso |  |  |
| Executar npm.cmd run build em src/SGX.SistemaChamado.Web | Build frontend concluido sem erro |  |  |
| Registrar total de testes aprovados | Quantitativo documentado na homologacao |  |  |
| Registrar evidencia da execucao | Evidencia anexada (log, print ou relatorio) |  |  |

## Resultado final da homologacao

- Status final:
- Responsavel pela aprovacao:
- Data da aprovacao:
- Pendencias:
- Observacoes finais:

## Observacao da etapa tecnica (Item 7)

- Integracao de cadastros com abertura/detalhe/triagem/filtros de chamados validada tecnicamente.
- Seed inicial dos cadastros administrativos validado com comportamento idempotente.
- Fluxo funcional tecnico do modulo validado com base em testes de use case e integracao backend.
- A homologacao institucional manual deste checklist permanece pendente ate o preenchimento do roteiro e anexacao das evidencias formais.

## Resultado consolidado da validacao funcional tecnica (Item 7)

- Fluxo administrativo de cadastros: validado tecnicamente.
- Validacoes de negocio (obrigatoriedade, duplicidade, cor `#RRGGBB`, peso > 0): validadas tecnicamente.
- Abertura de chamado com cadastros ativos: validada tecnicamente.
- Regras de ativo/inativo e bloqueio de inativos em novas operacoes: validadas tecnicamente.
- Preservacao historica para chamados antigos com cadastro inativo vinculado: validada tecnicamente.
- Filtros administrativos por cadastros: validados tecnicamente.
- Seed inicial (idempotencia, sem duplicidade, sem reativacao automatica): validado tecnicamente.
- Pendencia evolutiva mantida: consolidar suite frontend E2E para cobertura visual automatizada.


