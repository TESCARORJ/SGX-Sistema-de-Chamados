# AGENTS.md - GETI.SistemaChamado

## Objetivo
Manter a evolucao do Sistema de Chamados com foco em arquitetura DDD, separacao de camadas e manutenibilidade de longo prazo.

## Stack obrigatoria
- Backend: Java 21 + Spring Boot
- Frontend: Vue 3 + Quasar (projeto `GETI.SistemaChamado.UI`)
- Banco: PostgreSQL
- Persistencia: Spring Data JPA + Hibernate
- Migracoes: Flyway

## Fronteiras de modulo
- `GETI.SistemaChamado.Dominio`: regras e modelos de dominio (sem dependencias de infraestrutura)
- `GETI.SistemaChamado.Aplicacao`: casos de uso, contratos e DTOs
- `GETI.SistemaChamado.Infraestrutura`: adaptadores tecnicos, JPA/Hibernate, Flyway, integracoes
- `GETI.SistemaChamado.Api`: controladores REST e configuracoes de borda HTTP
- `GETI.SistemaChamado.Worker.Email`: processamento IMAP e correlacao de mensagens
- `GETI.SistemaChamado.UI`: portal solicitante e area administrativa em `/admin`

## Regras de implementacao
1. Nunca colocar regra de dominio em controller.
2. Nunca colocar detalhes de infraestrutura dentro do dominio.
3. Sempre gerar migration Flyway em mudancas estruturais de banco.
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