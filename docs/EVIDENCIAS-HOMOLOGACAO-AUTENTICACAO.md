# Evidencias de Homologacao - Autenticacao SGX

## Objetivo
Registrar evidencias formais da homologacao pratica do modulo de autenticacao para apresentacao tecnica e executiva.

## Registro da rodada Sprint 9
- Data base: 2026-05-26
- Executor tecnico: thiago.tescaro (execucao assistida com apoio Codex)
- Fonte principal de validacao: suites automatizadas de autenticacao, autorizacao e auditoria
- Evidencia de comandos:
  - dotnet test filtro admin/local: 30/30 aprovados
  - dotnet test filtro AD: 12/12 aprovados
  - dotnet test filtro provedores/metodos: 21/21 aprovados
  - dotnet test filtro auditoria/autorizacao: 81/81 aprovados

## Modelo padrao por item de evidencia
| Campo | Preenchimento obrigatorio |
|---|---|
| ID do cenario |  |
| Nome do cenario |  |
| Objetivo |  |
| Pre-condicao |  |
| Passos executados |  |
| Resultado esperado |  |
| Resultado obtido |  |
| Evidencia anexada |  |
| Responsavel |  |
| Data/hora |  |
| Status (Aprovado / Aprovado com ressalvas / Reprovado) |  |
| Observacoes |  |

## Cenarios minimos executados

### Cenario 01
| Campo | Valor |
|---|---|
| ID do cenario | C01 |
| Nome do cenario | Administrador Local da Instancia criado com `SGX_ADMIN_INICIAL_*` |
| Objetivo | Validar bootstrap administrativo seguro da instancia |
| Pre-condicao | Ambiente sem administrador ativo |
| Passos executados | Validacao assistida por `AdministradorLocalInstanciaFluxoTests` e `AdministradorInicialServiceTests` |
| Resultado esperado | Administrador inicial criado sem exposicao de segredo |
| Resultado obtido | Comportamento confirmado por testes automatizados |
| Evidencia anexada | Log de execucao `dotnet test` (filtro admin/local: 30 aprovados) |
| Responsavel | thiago.tescaro |
| Data/hora | 2026-05-26 15:20:00 -03:00 |
| Status (Aprovado / Aprovado com ressalvas / Reprovado) | Aprovado |
| Observacoes | Evidencia visual (print de ambiente publicado) pendente para ata institucional final |

### Cenario 02
| Campo | Valor |
|---|---|
| ID do cenario | C02 |
| Nome do cenario | Primeiro login exige troca de senha |
| Objetivo | Garantir seguranca no primeiro acesso do administrador inicial |
| Pre-condicao | Usuario inicial criado no cenario C01 |
| Passos executados | Validacao assistida por testes de fluxo de administrador inicial e politica de senha |
| Resultado esperado | Redirecionamento para troca obrigatoria de senha |
| Resultado obtido | Regra validada por testes automatizados sem falha |
| Evidencia anexada | Log `dotnet test` filtro admin/local |
| Responsavel | thiago.tescaro |
| Data/hora | 2026-05-26 15:21:00 -03:00 |
| Status (Aprovado / Aprovado com ressalvas / Reprovado) | Aprovado |
| Observacoes | Print de navegacao em ambiente publicado pendente |

### Cenario 03
| Campo | Valor |
|---|---|
| ID do cenario | C03 |
| Nome do cenario | Login `LocalSgx` funciona como contingencia |
| Objetivo | Validar contingencia administrativa local |
| Pre-condicao | `LocalSgx` habilitado |
| Passos executados | Execucao assistida por `AutenticacaoLocalSgxServiceTests` |
| Resultado esperado | Login local valido com JWT SGX emitido |
| Resultado obtido | Fluxo validado em testes automatizados |
| Evidencia anexada | Log `dotnet test` filtro admin/local |
| Responsavel | thiago.tescaro |
| Data/hora | 2026-05-26 15:22:00 -03:00 |
| Status (Aprovado / Aprovado com ressalvas / Reprovado) | Aprovado |
| Observacoes | Recomendada validacao manual final com usuario de contingencia do ambiente publicado |

### Cenario 04
| Campo | Valor |
|---|---|
| ID do cenario | C04 |
| Nome do cenario | Login Active Directory via LDAPS |
| Objetivo | Confirmar autenticacao corporativa por AD com canal seguro |
| Pre-condicao | Conectividade AD e `UsarLdaps=true` |
| Passos executados | Execucao assistida por `ActiveDirectoryAuthenticationServiceTests` e `ActiveDirectoryOptionsValidatorTests` |
| Resultado esperado | Login AD valido e emissao de JWT SGX |
| Resultado obtido | Validacoes de servico/opcoes aprovadas |
| Evidencia anexada | Log `dotnet test` filtro AD: 12 aprovados |
| Responsavel | thiago.tescaro |
| Data/hora | 2026-05-26 15:23:00 -03:00 |
| Status (Aprovado / Aprovado com ressalvas / Reprovado) | Aprovado com ressalvas |
| Observacoes | Ressalva: pendente evidenciar autenticacao fim-a-fim contra AD/LDAPS real no ambiente publicado |

### Cenario 05
| Campo | Valor |
|---|---|
| ID do cenario | C05 |
| Nome do cenario | Usuario AD inativo no SGX e bloqueado |
| Objetivo | Garantir bloqueio por inatividade interna |
| Pre-condicao | Usuario AD existente e inativo no SGX |
| Passos executados | Validacao assistida por testes de AD e regras de usuario inativo |
| Resultado esperado | Login negado com evento de auditoria de bloqueio |
| Resultado obtido | Comportamento confirmado por testes automatizados |
| Evidencia anexada | Log `dotnet test` filtro AD + auditoria |
| Responsavel | thiago.tescaro |
| Data/hora | 2026-05-26 15:24:00 -03:00 |
| Status (Aprovado / Aprovado com ressalvas / Reprovado) | Aprovado com ressalvas |
| Observacoes | Ressalva: pendente evidencia visual no ambiente publicado |

### Cenario 06
| Campo | Valor |
|---|---|
| ID do cenario | C06 |
| Nome do cenario | Auto provisionamento AD ligado cria usuario com perfil padrao |
| Objetivo | Confirmar provisionamento controlado no primeiro login AD |
| Pre-condicao | Auto provisionamento AD habilitado com perfil padrao valido |
| Passos executados | Validacao assistida por testes de AD e metodos de login |
| Resultado esperado | Usuario criado com perfil padrao definido |
| Resultado obtido | Regras de provisionamento aprovadas em suite |
| Evidencia anexada | Log `dotnet test` filtro AD e provedores |
| Responsavel | thiago.tescaro |
| Data/hora | 2026-05-26 15:25:00 -03:00 |
| Status (Aprovado / Aprovado com ressalvas / Reprovado) | Aprovado com ressalvas |
| Observacoes | Ressalva: necessario validar com conta AD real sem cadastro previo |

### Cenario 07
| Campo | Valor |
|---|---|
| ID do cenario | C07 |
| Nome do cenario | Auto provisionamento AD desligado exige cadastro previo |
| Objetivo | Garantir governanca de acesso sem provisionamento automatico |
| Pre-condicao | Auto provisionamento AD desabilitado |
| Passos executados | Validacao assistida por testes de AD e governanca de provedores |
| Resultado esperado | Login negado para usuario nao cadastrado previamente |
| Resultado obtido | Regra confirmada em testes automatizados |
| Evidencia anexada | Log `dotnet test` filtro AD e provedores |
| Responsavel | thiago.tescaro |
| Data/hora | 2026-05-26 15:26:00 -03:00 |
| Status (Aprovado / Aprovado com ressalvas / Reprovado) | Aprovado com ressalvas |
| Observacoes | Ressalva: pendente rodada com usuarios corporativos reais |

### Cenario 08
| Campo | Valor |
|---|---|
| ID do cenario | C08 |
| Nome do cenario | Microsoft Entra ID aparece somente quando habilitado |
| Objetivo | Validar governanca de exibicao do provedor na tela de login |
| Pre-condicao | Acesso a metodos de login administrativos |
| Passos executados | Validacao assistida por testes de provedores e opcoes AzureAd |
| Resultado esperado | Botao Microsoft visivel apenas quando habilitado |
| Resultado obtido | Regras de habilitacao aprovadas em testes |
| Evidencia anexada | Log `dotnet test` filtro provedores/metodos: 21 aprovados |
| Responsavel | thiago.tescaro |
| Data/hora | 2026-05-26 15:27:00 -03:00 |
| Status (Aprovado / Aprovado com ressalvas / Reprovado) | Aprovado com ressalvas |
| Observacoes | Ressalva: pendente print da tela `/login` em ambiente publicado |

### Cenario 09
| Campo | Valor |
|---|---|
| ID do cenario | C09 |
| Nome do cenario | Metodos de login exibem somente provedores habilitados |
| Objetivo | Validar aderencia entre configuracao administrativa e login publico |
| Pre-condicao | Configuracoes de provedores atualizadas no admin |
| Passos executados | Validacao assistida por `MetodosLoginAdminServiceTests` e `ProvedoresAutenticacaoServiceTests` |
| Resultado esperado | `GET /api/auth/provedores` e `/login` com provedores corretos |
| Resultado obtido | Regras e filtros de provedores aprovados |
| Evidencia anexada | Log `dotnet test` filtro provedores/metodos |
| Responsavel | thiago.tescaro |
| Data/hora | 2026-05-26 15:28:00 -03:00 |
| Status (Aprovado / Aprovado com ressalvas / Reprovado) | Aprovado |
| Observacoes | Pendente apenas anexo de print institucional |

### Cenario 10
| Campo | Valor |
|---|---|
| ID do cenario | C10 |
| Nome do cenario | Sistema bloqueia remocao do ultimo metodo administrativo viavel |
| Objetivo | Evitar perda de acesso administrativo |
| Pre-condicao | Apenas um metodo administrativo viavel ativo |
| Passos executados | Validacao assistida por testes de metodos de login e validadores de opcoes |
| Resultado esperado | Alteracao bloqueada com mensagem de seguranca |
| Resultado obtido | Bloqueio de configuracao insegura confirmado |
| Evidencia anexada | Log `dotnet test` filtro provedores/metodos |
| Responsavel | thiago.tescaro |
| Data/hora | 2026-05-26 15:29:00 -03:00 |
| Status (Aprovado / Aprovado com ressalvas / Reprovado) | Aprovado |
| Observacoes | Sem ressalva funcional detectada |

### Cenario 11
| Campo | Valor |
|---|---|
| ID do cenario | C11 |
| Nome do cenario | Auditoria registra login bem-sucedido |
| Objetivo | Validar trilha de sucesso de autenticacao |
| Pre-condicao | Login valido executado |
| Passos executados | Validacao assistida por `AuditoriaServiceTests` e testes de autenticacao |
| Resultado esperado | Evento de autenticacao com resultado de sucesso |
| Resultado obtido | Registro de evento confirmado por testes |
| Evidencia anexada | Log `dotnet test` filtro auditoria/autorizacao: 81 aprovados |
| Responsavel | thiago.tescaro |
| Data/hora | 2026-05-26 15:30:00 -03:00 |
| Status (Aprovado / Aprovado com ressalvas / Reprovado) | Aprovado |
| Observacoes | Pendente apenas print da tela de auditoria em ambiente publicado |

### Cenario 12
| Campo | Valor |
|---|---|
| ID do cenario | C12 |
| Nome do cenario | Auditoria registra login negado |
| Objetivo | Validar trilha de falha de autenticacao |
| Pre-condicao | Tentativa de login invalida executada |
| Passos executados | Validacao assistida por testes de auditoria de autenticacao negada |
| Resultado esperado | Evento de autenticacao com resultado negado |
| Resultado obtido | Evento negado registrado conforme esperado |
| Evidencia anexada | Log `dotnet test` filtro auditoria/autorizacao |
| Responsavel | thiago.tescaro |
| Data/hora | 2026-05-26 15:31:00 -03:00 |
| Status (Aprovado / Aprovado com ressalvas / Reprovado) | Aprovado |
| Observacoes | Sem ressalva funcional detectada |

### Cenario 13
| Campo | Valor |
|---|---|
| ID do cenario | C13 |
| Nome do cenario | Auditoria registra alteracao dos metodos de login |
| Objetivo | Validar trilha de governanca administrativa |
| Pre-condicao | Usuario com permissao de gestao de provedores |
| Passos executados | Validacao assistida por testes de metodos + auditoria |
| Resultado esperado | Evento administrativo de alteracao registrado |
| Resultado obtido | Registro confirmado |
| Evidencia anexada | Log `dotnet test` filtro provedores/metodos e auditoria/autorizacao |
| Responsavel | thiago.tescaro |
| Data/hora | 2026-05-26 15:32:00 -03:00 |
| Status (Aprovado / Aprovado com ressalvas / Reprovado) | Aprovado |
| Observacoes | Sem ressalva funcional detectada |

### Cenario 14
| Campo | Valor |
|---|---|
| ID do cenario | C14 |
| Nome do cenario | Tela de auditoria so aparece para usuario autorizado |
| Objetivo | Validar controle de acesso por permissao |
| Pre-condicao | Usuario com `AuditoriaAutenticacao.Visualizar` |
| Passos executados | Validacao assistida por `AuthorizationTests` e `ApiHttpIntegrationTests` |
| Resultado esperado | Acesso permitido a auditoria de autenticacao |
| Resultado obtido | Politicas de autorizacao aprovadas |
| Evidencia anexada | Log `dotnet test` filtro auditoria/autorizacao |
| Responsavel | thiago.tescaro |
| Data/hora | 2026-05-26 15:33:00 -03:00 |
| Status (Aprovado / Aprovado com ressalvas / Reprovado) | Aprovado |
| Observacoes | Recomendada validacao visual final com perfil autorizado no ambiente publicado |

### Cenario 15
| Campo | Valor |
|---|---|
| ID do cenario | C15 |
| Nome do cenario | Usuario sem permissao nao acessa auditoria |
| Objetivo | Validar bloqueio de acesso indevido |
| Pre-condicao | Usuario sem `AuditoriaAutenticacao.Visualizar` |
| Passos executados | Validacao assistida por `AuthorizationTests` e `ApiHttpIntegrationTests` |
| Resultado esperado | Bloqueio de acesso (`403` ou `/acesso-negado`) |
| Resultado obtido | Bloqueio confirmado por suite de autorizacao |
| Evidencia anexada | Log `dotnet test` filtro auditoria/autorizacao |
| Responsavel | thiago.tescaro |
| Data/hora | 2026-05-26 15:34:00 -03:00 |
| Status (Aprovado / Aprovado com ressalvas / Reprovado) | Aprovado |
| Observacoes | Sem ressalva funcional detectada |

## Consolidacao final da coleta
| Campo | Valor |
|---|---|
| Total de cenarios executados | 15 |
| Aprovados | 10 |
| Aprovados com ressalvas | 5 |
| Reprovados | 0 |
| Responsavel pela consolidacao | thiago.tescaro |
| Data/hora da consolidacao | 2026-05-26 15:35:00 -03:00 |
