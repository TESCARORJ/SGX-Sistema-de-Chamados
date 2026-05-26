# Homologacao de Autenticacao - SGX Sistema de Chamados

## Objetivo
Consolidar a homologacao operacional do modulo de autenticacao do SGX para cenarios cloud, on-premises e hibridos, sem alteracao de regra funcional na aplicacao.

Regra arquitetural obrigatoria em toda validacao:
- o provedor autentica;
- o SGX autoriza por perfis e permissoes internos.

## Escopo da Sprint 7
- checklist funcional minimo de autenticacao;
- validacao do Administrador Local da Instancia;
- validacao dos logins `LocalSgx`, `ActiveDirectory` e `MicrosoftEntraId`;
- validacao dos metodos de login configuraveis;
- validacao da auditoria de autenticacao;
- consolidacao de evidencias operacionais.

## Pacote institucional da Sprint 8
- ata institucional: `docs/ATA-HOMOLOGACAO-AUTENTICACAO.md`;
- caderno de evidencias por cenario: `docs/EVIDENCIAS-HOMOLOGACAO-AUTENTICACAO.md`;
- checklist executivo resumido: `docs/CHECKLIST-EXECUTIVO-AUTENTICACAO.md`.

Uso recomendado:
- executar este checklist tecnico;
- preencher evidencias por cenario no caderno de evidencias;
- consolidar resultado executivo no checklist executivo;
- formalizar parecer final na ata.

## Pre-requisitos
- banco atualizado com migrations aplicadas;
- backend/frontend/worker com build valido;
- configuracoes revisadas:
  - `Authentication__*`
  - `AzureAd__*`
  - `ActiveDirectory__*`
  - `SGX_ADMIN_INICIAL_*`
- acesso administrativo ao ambiente homologado;
- massa de teste com:
  - usuario Administrador ativo;
  - usuario sem permissao de auditoria;
  - usuario inativo;
  - usuario de AD valido.

## Checklist funcional minimo (obrigatorio)

| ID | Validacao obrigatoria | Evidencia minima | Resultado |
|---|---|---|---|
| 1 | Administrador Local da Instancia criado por variaveis `SGX_ADMIN_INICIAL_*`. | Log tecnico + usuario administrador criado | [ ] |
| 2 | Primeiro login exige troca de senha. | Redirecionamento para `/alterar-senha` | [ ] |
| 3 | Login local SGX funciona como contingencia. | Login `LocalSgx` com JWT valido | [ ] |
| 4 | Microsoft Entra ID aparece somente quando habilitado. | `/login` + `GET /api/auth/provedores` | [ ] |
| 5 | Active Directory aparece somente quando habilitado. | `/login` + `GET /api/auth/provedores` | [ ] |
| 6 | LocalSgx aparece somente quando habilitado. | `/login` + `GET /api/auth/provedores` | [ ] |
| 7 | LocalDevelopment aparece somente em `Development`. | Ambiente Production/Homolog sem `LocalDevelopment` | [ ] |
| 8 | Nao e possivel deixar o sistema sem metodo viavel de login. | Tentativa bloqueada no admin de metodos | [ ] |
| 9 | Nao e possivel remover ultimo acesso administrativo viavel. | Tentativa bloqueada no admin de metodos | [ ] |
| 10 | Login AD valido autentica e emite JWT SGX. | `POST /api/auth/ad/login` com sucesso | [ ] |
| 11 | Usuario AD inativo no SGX e bloqueado. | Login negado + evento de auditoria | [ ] |
| 12 | Auto provisionamento AD ligado cria usuario com perfil padrao. | Usuario criado conforme perfil padrao | [ ] |
| 13 | Auto provisionamento AD desligado exige usuario previamente cadastrado. | Login negado sem pre-cadastro | [ ] |
| 14 | Auto provisionamento nao concede Administrador automaticamente. | Usuario provisionado sem perfil admin | [ ] |
| 15 | Auditoria registra login bem-sucedido. | Evento de autenticacao sucesso | [ ] |
| 16 | Auditoria registra login negado. | Evento de autenticacao negado | [ ] |
| 17 | Auditoria registra usuario inativo bloqueado. | Evento de bloqueio por usuario inativo | [ ] |
| 18 | Auditoria registra alteracao de metodos de login. | Evento administrativo de alteracao | [ ] |
| 19 | Tela de auditoria exibe eventos apenas para usuario com `AuditoriaAutenticacao.Visualizar`. | Tela carregada para usuario autorizado | [ ] |
| 20 | Usuario sem permissao nao acessa auditoria de autenticacao. | Bloqueio em `/acesso-negado` ou `403` | [ ] |

## Roteiro resumido por trilha operacional

### Trilha A - Administrador Local da Instancia
- Executar bootstrap com `SGX_ADMIN_INICIAL_*`.
- Validar troca obrigatoria de senha no primeiro acesso.
- Validar login apos troca.
- Remover/rotacionar variaveis de bootstrap.

### Trilha B - Login LocalSgx
- Habilitar `LocalSgx` na configuracao administrativa.
- Executar login valido e invalido.
- Confirmar comportamento de contingencia com JWT SGX.

### Trilha C - Login ActiveDirectory
- Validar conectividade LDAP/LDAPS.
- Executar login AD valido e verificar JWT interno SGX.
- Validar usuario inativo e auto provisionamento ligado/desligado.

### Trilha D - Login MicrosoftEntraId
- Validar provedor habilitado no ambiente.
- Executar login com conta corporativa do tenant autorizado.
- Validar bloqueio quando provedor estiver desabilitado.

### Trilha E - Metodos de login configuraveis
- Validar habilitacao/desabilitacao, principal e ordem.
- Validar bloqueios de seguranca para estados inviaveis.
- Validar reflexo em `GET /api/auth/provedores`.

### Trilha F - Auditoria de autenticacao
- Confirmar eventos de sucesso, negado e bloqueio.
- Confirmar evento de alteracao administrativa de metodos.
- Validar controle de acesso por permissao `AuditoriaAutenticacao.Visualizar`.

## Registro de execucao
Preencher ao final da rodada:

| Campo | Valor |
|---|---|
| Ambiente | Cloud / On-Premises / Hibrido |
| Data da execucao |  |
| Executor tecnico |  |
| Responsavel TI |  |
| Resultado geral | Aprovado / Reprovado / Aprovado com ressalvas |
| Pendencias abertas |  |
| Prazo de ajuste |  |

## Controle de execucao por item (preenchimento real)

| ID | Responsavel | Data/hora execucao | Status (Aprovado / Aprovado com ressalvas / Reprovado) | Evidencia vinculada | Observacoes |
|---|---|---|---|---|---|
| 1 |  |  |  |  |  |
| 2 |  |  |  |  |  |
| 3 |  |  |  |  |  |
| 4 |  |  |  |  |  |
| 5 |  |  |  |  |  |
| 6 |  |  |  |  |  |
| 7 |  |  |  |  |  |
| 8 |  |  |  |  |  |
| 9 |  |  |  |  |  |
| 10 |  |  |  |  |  |
| 11 |  |  |  |  |  |
| 12 |  |  |  |  |  |
| 13 |  |  |  |  |  |
| 14 |  |  |  |  |  |
| 15 |  |  |  |  |  |
| 16 |  |  |  |  |  |
| 17 |  |  |  |  |  |
| 18 |  |  |  |  |  |
| 19 |  |  |  |  |  |
| 20 |  |  |  |  |  |

## Consolidacao para apresentacao institucional

| Campo | Valor |
|---|---|
| Total de itens obrigatorios | 20 |
| Itens aprovados |  |
| Itens aprovados com ressalvas |  |
| Itens reprovados |  |
| Resultado sugerido | Aprovado / Aprovado com ressalvas / Reprovado |
| Ata emitida | Sim / Nao |
| Data de emissao da ata |  |

## Criterio de aceite operacional
- 100% dos 20 itens obrigatorios concluidos;
- nenhuma exposicao de senha/token/hash/segredo em logs e telas;
- trilha de auditoria coerente com os testes executados;
- contingencia administrativa com `LocalSgx` validada;
- evidencias anexadas ao pacote de homologacao institucional.
