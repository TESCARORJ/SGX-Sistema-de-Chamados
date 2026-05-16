# Checklist de Homologacao - Cadastros Administrativos

## Objetivo

Validar em ambiente de homologacao o comportamento funcional do modulo de Cadastros Administrativos no fluxo completo do SGX Sistema de Chamados.

## Escopo

- Cadastros administrativos (CRUD e regras)
- Integracao dos cadastros na abertura e classificacao de chamados
- Leitura historica com cadastros inativos
- Filtros administrativos por cadastro

## Pre-condicoes

- ambiente de homologacao atualizado com a versao da Sprint 8;
- base com usuarios de teste para perfis Administrador, Atendente e Solicitante;
- massa de dados contendo registros ativos e inativos por cadastro.

## Resultado esperado

Cada item deve ser marcado como:
- `[x]` aprovado
- `[ ]` pendente ou reprovado (registrar observacao)

## 1. Cadastros administrativos

- [ ] listar Departamentos, Categorias, Subcategorias, Prioridades, Tipos de Solicitacao e Locais/Unidades
- [ ] criar registro valido em cada cadastro
- [ ] editar registro existente em cada cadastro
- [ ] inativar registro e confirmar ausencia nas selecoes operacionais
- [ ] reativar registro e confirmar retorno nas selecoes operacionais
- [ ] validar busca por nome
- [ ] validar filtro por status (`Ativos`, `Inativos`, `Todos`)

## 2. Validacoes de negocio

- [ ] bloquear duplicidade de Departamentos
- [ ] bloquear duplicidade de Categorias
- [ ] bloquear duplicidade de Subcategorias dentro da mesma Categoria
- [ ] permitir mesmo nome de Subcategoria em Categorias diferentes
- [ ] bloquear Prioridade com peso `0` ou negativo
- [ ] validar formato de cor da Prioridade (`#RRGGBB`)
- [ ] bloquear Subcategoria sem Categoria

## 3. Abertura de chamado com cadastros

- [ ] abrir chamado com Categoria e Subcategoria consistentes
- [ ] abrir chamado com Prioridade, Tipo de Solicitacao, Local/Unidade e Departamento ativos
- [ ] bloquear selecao de cadastro inativo em nova abertura
- [ ] bloquear Subcategoria que nao pertence a Categoria selecionada

## 4. Fluxo administrativo e historico

- [ ] validar filtros administrativos de chamados por cadastro
- [ ] validar detalhe administrativo com nomes de cadastros vinculados
- [ ] validar detalhe do portal com nomes de cadastros vinculados
- [ ] validar leitura historica de chamado antigo com cadastro atualmente inativo

## 5. Evidencias obrigatorias

- [ ] print da listagem administrativa de cada cadastro
- [ ] print da abertura de chamado com todos os campos de cadastro preenchidos
- [ ] print de bloqueio de regra (ex.: subcategoria fora da categoria)
- [ ] print do detalhe de chamado (portal e admin)
- [ ] print de filtro administrativo por cadastro

## 6. Registro final de homologacao

- Responsavel pela execucao:
- Data da execucao:
- Resultado geral (`Aprovado`, `Aprovado com ressalvas`, `Reprovado`):
- Observacoes:
- Plano de acao (se houver itens reprovados):
