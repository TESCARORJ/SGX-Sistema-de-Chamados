# Sprint 3 - Permissoes de grupo tecnico e fila

## Modelo encontrado

O sistema ja possui autorizacao em duas camadas:

- API: controllers administrativos usam `[Authorize]` com `Policies.AdminOuAtendente` e, em operacoes especificas de chamados, `PermissionPolicies` baseadas em codigos de `PermissoesConstants`.
- Aplicacao: use cases administrativos usam `IUsuarioContextoAplicacaoService` e validacoes por perfil, principalmente `AdminUseCaseHelpers.PodeOperarAdmin`, `AdminUseCaseHelpers.EhAdministrador`, `AdminCadastrosHelpers.GarantirAdminOuAtendente` e `AdminCadastrosHelpers.GarantirAdministrador`.

Os perfis oficiais encontrados para esta etapa sao `Administrador`, `Atendente` e `Solicitante`. Nao existe papel proprio de gestor tecnico ou coordenador tecnico nesta sprint.

## Protecao atual das operacoes admin

Os controllers administrativos existentes sao protegidos por policy de API. Como a Sprint 3 ainda nao criou endpoints de grupo/fila, a exposicao HTTP futura deve reutilizar esse padrao e, quando a operacao for critica, usar permissao fina equivalente.

Na camada de aplicacao, os use cases de grupo tecnico e membro ja validam perfil antes de operar. As regras de direcionar, transferir, assumir da fila, atribuir tecnico e consultar chamados tambem possuem guarda por perfil ou por regra de membro ativo.

## Matriz de permissao proposta

| Operacao | Perfil/regra atual | Recomendacao para endpoint futuro |
| --- | --- | --- |
| Cadastrar grupo tecnico | Administrador | `AdminOuAtendente` + permissao fina de cadastros/gestao, ou Administrador |
| Editar grupo tecnico | Administrador | `AdminOuAtendente` + permissao fina de cadastros/gestao, ou Administrador |
| Ativar/inativar grupo tecnico | Administrador | `AdminOuAtendente` + permissao fina de cadastros/gestao, ou Administrador |
| Listar grupos tecnicos | Administrador ou Atendente | `AdminOuAtendente` |
| Obter grupo tecnico | Administrador ou Atendente | `AdminOuAtendente` |
| Gerenciar membros de grupo tecnico | Administrador | `AdminOuAtendente` + permissao fina de gestao de membros, ou Administrador |
| Listar membros/grupos de usuario | Administrador ou Atendente | `AdminOuAtendente` |
| Direcionar chamado para grupo | Administrador ou Atendente | `AdminOuAtendente` e permissao operacional de chamados, se criada |
| Transferir chamado entre grupos | Administrador ou Atendente | preferir permissao fina futura para transferencia de grupo |
| Assumir chamado da fila | Administrador ou Atendente, proprio usuario autenticado e membro ativo do grupo | policy de assumir chamado + validacao de membro ativo |
| Atribuir chamado a tecnico especifico | Administrador; tecnico destino deve ser membro ativo se chamado tem grupo | policy de atribuir chamado + validacao de membro ativo |
| Consultar chamados por grupo/fila | Administrador ou Atendente | `AdminOuAtendente`; restringir por grupos do usuario se essa regra for exigida na tela futura |

## Use cases analisados

- `GruposTecnicosAdminUseCases`
- `MembrosGruposTecnicosAdminUseCases`
- `DirecionarChamadoGrupoTecnicoAdminUseCase`
- `TransferirGrupoTecnicoChamadoUseCase`
- `AssumirChamadoFilaAdminUseCase`
- `AtribuirChamadoUseCase`
- `ListarChamadosAdminUseCase`
- `DetalharChamadoAdminUseCase`
- `AdminChamadosController`
- `PermissionPolicies`, `PermissoesConstants`, `PermissionAuthorizationHandler` e `ServiceCollectionExtensions`

## Validacoes confirmadas

- Operacoes de escrita de grupo tecnico exigem Administrador.
- Listagens e obtencao de grupos/membros permitem Administrador ou Atendente.
- Direcionamento e transferencia exigem perfil administrativo operacional (`Administrador` ou `Atendente`).
- Assumir chamado da fila exige perfil administrativo operacional, usuario autenticado igual ao usuario informado e membro ativo do grupo.
- Atribuicao manual a tecnico especifico esta restrita a Administrador nesta sprint e valida membro ativo quando o chamado possui grupo.
- Consultas administrativas de chamados seguem `Administrador` ou `Atendente`.

## Testes adicionados

- Solicitante nao direciona chamado para grupo tecnico.
- Solicitante nao transfere chamado entre grupos tecnicos.
- Usuario sem perfil administrativo nao assume chamado da fila.
- Atendente nao atribui chamado manualmente a tecnico especifico nesta sprint.

## Pontos para endpoints futuros

Como ainda nao ha endpoints da Sprint 3, nao foram criadas policies novas nem constantes de permissao especificas de grupo/fila. A proxima etapa de endpoints deve decidir se usa apenas `AdminOuAtendente` ou se cria permissoes finas para:

- gerenciar grupos tecnicos;
- gerenciar membros de grupos;
- direcionar chamado para grupo;
- transferir chamado entre grupos;
- consultar filas por grupo.

## Riscos restantes

- Atendentes ainda consultam todos os chamados administrativos; a restricao por grupos do usuario deve ser definida na tela/endpoint futuro se fizer parte da regra institucional.
- Nao ha perfil de gestor tecnico/coordenador; qualquer matriz que dependa desse papel exige evolucao propria de perfil/permissao.
- As permissoes finas para grupo/fila ainda nao existem porque nao ha endpoints publicos desta sprint.

## O que nao foi implementado

- Nenhum controller.
- Nenhum endpoint publico.
- Nenhuma tela Vue.
- Nenhum service frontend.
- Nenhum mecanismo paralelo de permissao.
- Nenhuma migration estrutural.
- Nenhuma alteracao em dashboard, relatorio, SLA ou roteamento automatico.

## Proxima etapa recomendada

Criar endpoints administrativos de cadastro de grupos tecnicos aplicando a matriz acima e reaproveitando o modelo existente de `Authorize`, `Policies.AdminOuAtendente` e permissoes finas quando a operacao exigir controle mais granular.
