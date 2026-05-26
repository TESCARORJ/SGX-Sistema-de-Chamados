# Metodos de Login - SGX Sistema de Chamados

## Objetivo
Permitir que o administrador da instancia defina, de forma persistida, quais metodos de login estao disponiveis na tela de autenticacao do SGX.

Regra arquitetural:
- o provedor autentica;
- o SGX autoriza por perfis e permissoes internas.

## Provedores suportados
- `MicrosoftEntraId`
- `ActiveDirectory`
- `LocalSgx`
- `LocalDevelopment` (somente em `Development`)

## Conceitos
- Configurado: existe na configuracao tecnica (appsettings/variaveis/opcoes).
- Habilitado: aparece para o usuario final na tela de login.
- Principal: metodo priorizado como opcao padrao na UI.
- Ordem: sequencia de exibicao no login.

## Persistencia administrativa
A Sprint 4 usa persistencia em `ParametroSistema` com chaves `auth.provedores.*` para guardar:
- status habilitado por provedor;
- provedor principal;
- ordem de exibicao;
- nome/rotulo exibido;
- auto provisionamento e perfil padrao (quando aplicavel).

Fallback seguro:
- se nao houver configuracao administrativa salva, o SGX usa a configuracao tecnica (`Authentication__*`, `ActiveDirectory__*` e configuracao efetiva Microsoft).

## Endpoints administrativos

### `GET /api/admin/autenticacao/provedores`
Retorna configuracao administrativa efetiva dos metodos de login.

Permissao:
- `AutenticacaoProvedores.Visualizar`

### `PUT /api/admin/autenticacao/provedores`
Salva configuracao administrativa dos metodos de login.

Permissao:
- `AutenticacaoProvedores.Gerenciar`

## Endpoint publico do login

### `GET /api/auth/provedores`
A tela de login deve consumir este endpoint e renderizar somente os provedores retornados.

Regras:
- provedor habilitado aparece;
- provedor desabilitado nao aparece;
- `LocalDevelopment` nao aparece fora de `Development`;
- ordem de exibicao e provedor principal respeitam configuracao administrativa efetiva.

## Regras obrigatorias de seguranca
- nao permitir salvar configuracao sem nenhum metodo de login viavel;
- nao permitir remover o ultimo metodo de acesso administrativo sem alternativa funcional;
- `LocalSgx` nao pode ser desabilitado quando for a unica contingencia administrativa;
- perfil `Administrador` nao pode ser atribuido automaticamente por auto provisionamento de `MicrosoftEntraId` ou `ActiveDirectory`;
- `LocalDevelopment` permanece exclusivo de `Development`.

## Auto provisionamento
- configuravel por provedor quando aplicavel;
- exige perfil padrao valido;
- perfil `Administrador` e proibido para provisionamento automatico.

## Auditoria administrativa (Sprint 5)
- alteracoes de metodos de login sao auditadas de forma persistida;
- eventos cobertos:
  - habilitar/desabilitar provedor;
  - troca de provedor principal;
  - alteracao de ordem;
  - alteracao de auto provisionamento;
  - alteracao de perfil padrao;
  - alteracao de rotulo de exibicao;
  - bloqueio de configuracao insegura;
  - tentativa negada por falta de permissao.

Consulta dedicada:
- `GET /api/admin/auditoria/autenticacao`
- permissao: `AuditoriaAutenticacao.Visualizar`

## Operacao recomendada
1. Validar configuracao tecnica minima de autenticacao.
2. Acessar `Administracao > Integracoes > Autenticacao > Metodos de login`.
3. Ajustar habilitacao, principal e ordem.
4. Definir auto provisionamento/perfil padrao apenas quando necessario.
5. Salvar e validar em `GET /api/auth/provedores`.
6. Testar login de contingencia local SGX antes de encerrar mudanca.

## Observacao
Esta sprint nao altera o contrato de autenticacao AD ja entregue na Sprint 3; ela adiciona governanca administrativa persistida para os metodos de login.
