# AGENTS.md - SGX.SistemaChamado

## Objetivo
Manter a evolucao do Sistema de Chamados com foco em arquitetura DDD, separacao de camadas e manutenibilidade de longo prazo.

## Stack obrigatoria
- Backend: .NET 9 (ASP.NET Core + Worker Service)
- Frontend: Vue 3 + Quasar (projeto `src/SGX.SistemaChamado.Web`)
- Banco: PostgreSQL
- Persistencia: Entity Framework Core
- Migracoes: EF Core Migrations

## Fronteiras de modulo
- `src/SGX.SistemaChamado.Domain`: regras e modelos de dominio
- `src/SGX.SistemaChamado.Application`: casos de uso, contratos e DTOs
- `src/SGX.SistemaChamado.Infrastructure`: adaptadores tecnicos, EF Core e integracoes
- `src/SGX.SistemaChamado.Api`: borda HTTP e autenticacao/autorizacao
- `src/SGX.SistemaChamado.Worker.Email`: processamento IMAP e correlacao de mensagens
- `src/SGX.SistemaChamado.Web`: portal solicitante e area administrativa em `/admin`

## Regras de implementacao
1. Nunca colocar regra de dominio em controller.
2. Nunca colocar detalhes de infraestrutura dentro do dominio.
3. Sempre gerar migration incremental de EF Core em mudancas estruturais de banco.
4. Nomear classes, entidades, enums, repositorios e casos de uso com linguagem de dominio em pt-BR.
5. Preservar separacao entre Dominio, Aplicacao, Infraestrutura, Api, Worker e UI.
6. Priorizar implementacao completa e coesa, evitando placeholders desnecessarios.
7. Garantir build backend/frontend ao fim de cada sprint.

## Padrao de entrega esperado
- Listar arquivos alterados/criados
- Registrar decisoes tecnicas
- Informar comandos executados
- Explicar pendencias
- Sugerir proximos passos
