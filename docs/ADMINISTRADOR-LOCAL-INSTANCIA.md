# Administrador Local da Instancia - SGX Sistema de Chamados

## Objetivo
Garantir que cada instalacao do SGX possua um caminho seguro de primeiro acesso administrativo, independente de MicrosoftEntraId e ActiveDirectory.

## Principio de seguranca
- O provedor autentica.
- O SGX autoriza por perfis e permissoes internas.
- O Administrador Local da Instancia autentica por `LocalSgx`.

## Configuracao obrigatoria de bootstrap
- `SGX_ADMIN_INICIAL_EMAIL`
- `SGX_ADMIN_INICIAL_SENHA`
- `SGX_ADMIN_INICIAL_NOME`

Regras:
- em producao, o administrador inicial so e criado por configuracao explicita;
- se faltar qualquer variavel obrigatoria, nenhum usuario e criado;
- se ja existir Administrador ativo, nao cria duplicidade.

## Regras de criacao e seguranca
- senha local sempre armazenada como hash (`PasswordHasher<Usuario>`);
- senha nunca deve aparecer em log;
- senha fraca e rejeitada pela politica de senha;
- usuario inicial recebe `DeveAlterarSenha=true`;
- perfil administrativo interno do SGX e associado;
- o usuario criado nao e superusuario global de fornecedora, apenas Administrador interno da instancia.

## Fluxo operacional
1. Definir variaveis `SGX_ADMIN_INICIAL_*`.
2. Inicializar API.
3. SGX cria Administrador Local da Instancia se nao houver Administrador ativo.
4. Primeiro login local SGX retorna `deveAlterarSenha=true`.
5. Usuario conclui `POST /api/auth/local/alterar-senha`.
6. Proximos logins retornam `deveAlterarSenha=false`.

## Contingencia administrativa
- O login local SGX deve permanecer como contingencia administrativa.
- O fluxo local continua funcional mesmo com MicrosoftEntraId desabilitado.
- `LocalDevelopment` e apenas suporte tecnico de Development, sem substituir o administrador local de producao/homologacao.

## Endpoints relacionados
- `POST /api/auth/local/login`
- `POST /api/auth/local/alterar-senha`
- `GET /api/me`
- `GET /api/auth/provedores`

## Evidencias automatizadas
Cobertura de testes inclui:
- criacao do administrador inicial com variaveis completas;
- nao criacao com variaveis ausentes/incompletas;
- nao duplicar quando existe Administrador ativo;
- rejeicao de senha fraca;
- hash de senha e ausencia de senha em logs;
- primeiro login com troca obrigatoria;
- login apos troca com `DeveAlterarSenha=false`;
- bloqueio de login para usuario inativo;
- contingencia local com Microsoft desabilitado.
