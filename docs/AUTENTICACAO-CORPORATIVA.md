# Autenticação Corporativa - SGX Sistema de Chamados

## Objetivo
Padronizar a autenticação com múltiplos provedores no SGX Sistema de Chamados, mantendo a regra arquitetural principal:

- O provedor de identidade autentica.
- O SGX autoriza por perfis e permissões internos.

## Decisão arquitetural

- `MicrosoftEntraId`: autenticação corporativa via Microsoft Entra ID.
- `Local`: autenticação local SGX (e-mail e senha do próprio SGX).
- `Hibrido`: Microsoft Entra ID + login local SGX.
- `LocalDevelopment`: fluxo técnico separado, exclusivo de `Development`.

O login local de produção (`LocalSgx`) **não** substitui o login local de `Development`.

## Configurações obrigatórias

### Backend (`Authentication__*`)

- `Authentication__ProvedorPrincipal`: `MicrosoftEntraId` | `Local` | `Hibrido`
- `Authentication__LoginLocalHabilitado`: `true`/`false`
- `Authentication__ModoLocalHabilitado`: `true`/`false` (somente `Development`)
- `Authentication__JwtLocalIssuer`
- `Authentication__JwtLocalAudience`
- `Authentication__JwtLocalChaveAssinatura` (mínimo de 32 caracteres)
- `Authentication__JwtLocalExpiracaoMinutos`
- `Authentication__PoliticaSenha__TamanhoMinimo`
- `Authentication__PoliticaSenha__ExigirMaiuscula`
- `Authentication__PoliticaSenha__ExigirMinuscula`
- `Authentication__PoliticaSenha__ExigirNumero`
- `Authentication__PoliticaSenha__ExigirEspecial`
- `Authentication__PoliticaSenha__BloquearSenhaAnterior`
- `Authentication__Lockout__TentativasMaximas`
- `Authentication__Lockout__MinutosBloqueio`
- `Authentication__RecuperacaoSenha__ExpiracaoMinutos`

### Microsoft Entra ID (`AzureAd__*`)

Obrigatório quando `Authentication__ProvedorPrincipal` for `MicrosoftEntraId` ou `Hibrido`.

## Regras de exibição no login

A tela de login consulta `GET /api/auth/provedores`:

- `MicrosoftEntraId`: exibe Microsoft como principal.
- `Local`: exibe login local SGX como principal.
- `Hibrido`: exibe Microsoft e login local SGX.
- `LocalDevelopment`: aparece somente em `Development` quando `Authentication__ModoLocalHabilitado=true`.

Se Microsoft não estiver configurado e o login local SGX estiver habilitado, a tela permanece funcional sem erro técnico bloqueante.

## Microsoft Entra ID - Single Tenant

- Microsoft Entra ID autentica.
- SGX Sistema de Chamados autoriza internamente por perfis e permissões.
- Tenant configurado é obrigatório.
- Contas pessoais Microsoft não são aceitas.
- Tenants externos não são aceitos por padrão.
- `roles/groups` do Azure não viram perfil `Administrador`.
- O SGX decide acesso com base no usuário interno, perfis e permissões.

Mensagem para reunião:
O SGX Sistema de Chamados usa Microsoft Entra ID em modo Single Tenant. Isso significa que somente contas corporativas do tenant configurado podem autenticar. Contas pessoais Microsoft e tenants externos não são aceitos por padrão. A autorização continua sendo controlada dentro do SGX por perfis e permissões.

## Endpoints de autenticação

### `GET /api/auth/provedores`
Retorna os provedores habilitados para a UI.

### `POST /api/auth/local/login`
Autentica localmente no SGX e emite JWT assinado pela API.

Payload:

```json
{
  "email": "usuario@empresa.com",
  "senha": "SenhaForte"
}
```

Resposta:

```json
{
  "accessToken": "<jwt>",
  "tokenType": "Bearer",
  "expiresIn": 7200,
  "autenticadoPor": "LocalSgx",
  "deveAlterarSenha": false
}
```

## `/api/me` e origem da autenticação

`GET /api/me` mantém o contrato e retorna:

- `autenticadoPor=MicrosoftEntraId` no fluxo Microsoft.
- `autenticadoPor=LocalSgx` no fluxo local SGX.
- `autenticadoPor=LocalDevelopment` apenas no modo local de desenvolvimento.
- `deveAlterarSenha=true` quando o usuário local deve concluir troca obrigatória antes de navegar.

## Senhas e segurança

- Senha local SGX é armazenada em hash (`senha_hash_local`).
- Implementação usa `PasswordHasher<Usuario>`.
- Não há senha em texto puro no banco.
- Hash/salt não são retornados pela API.
- Não existe senha fixa de desenvolvimento no frontend para autenticação real.

## Regras de bloqueio

- Usuário inativo no SGX é bloqueado no login local SGX.
- Perfis e permissões continuam internos ao SGX.

## Diferença entre Local SGX e Local Development

### Local SGX (`LocalSgx`)

- Válido para produção/homologação quando habilitado.
- Usa `POST /api/auth/local/login`.
- Exige credenciais locais com hash seguro.
- Emite JWT local da API.

### Local Development (`LocalDevelopment`)

- Apenas em `Development`.
- Não representa autenticação corporativa nem login local de produção.
- Usa headers `X-Dev-*` para suporte técnico e emulação de perfis.

## Sprint Autenticação 7 - Administrador inicial seguro

Objetivo:
- Permitir a criação segura do primeiro Administrador em produção por variáveis de ambiente explícitas.

Variáveis obrigatórias:
- `SGX_ADMIN_INICIAL_EMAIL`
- `SGX_ADMIN_INICIAL_SENHA`
- `SGX_ADMIN_INICIAL_NOME`

Regras:
- Só cria o Administrador inicial quando as três variáveis estiverem preenchidas.
- Só cria se ainda não existir Administrador ativo.
- Senha sempre armazenada com hash (`PasswordHasher<Usuario>`).
- Senha nunca é registrada em log.
- Senhas fracas são rejeitadas (incluindo `Admin@123456`, `123456`, `password`, `senha`, `admin`).

Operação recomendada:
1. Definir as variáveis apenas para o primeiro ciclo de inicialização.
2. Confirmar criação do Administrador inicial.
3. Remover/rotacionar imediatamente as variáveis de ambiente.

Diferenças importantes:
- `LocalSgx`: login local SGX de produção/homologação.
- `LocalDevelopment`: login técnico somente em Development.
- Administrador inicial: mecanismo de bootstrap seguro para o primeiro acesso administrativo.

## Sprint Autenticação 8 - Recuperação de senha e hardening do login local SGX

Objetivo:
- Evoluir o login local SGX para uso de produção com recuperação de senha, troca obrigatória, política de senha, lockout e auditoria básica por logs estruturados.

Entregas:
- `POST /api/auth/local/alterar-senha` para troca de senha autenticada.
- `POST /api/auth/local/recuperar-senha/solicitar` com resposta genérica (sem enumeração de usuário).
- `POST /api/auth/local/recuperar-senha/redefinir` com token temporário, expiração e uso único.
- Política de senha centralizada e reutilizada no bootstrap, troca e redefinição.
- Lockout configurável por tentativas inválidas (`TentativasInvalidas`, `BloqueadoAte`).
- Registro de `UltimoLoginEm` em login local bem-sucedido.
- Suporte de troca obrigatória (`DeveAlterarSenha`) com redirecionamento para `/alterar-senha`.

Regras de segurança:
- Senha e token nunca são armazenados em texto puro.
- Banco armazena apenas `senha_hash_local` e `token_hash`.
- Senha e token não são registrados em logs.
- Resposta de solicitação de recuperação sempre usa mensagem genérica:
  - `Se o e-mail estiver cadastrado, enviaremos as instruções para redefinição de senha.`

Diferença entre os fluxos:
- `LocalSgx`: autenticação local de produção/homologação com hardening de senha.
- `LocalDevelopment`: suporte técnico exclusivo de Development.
- Administrador inicial seguro: bootstrap de primeiro acesso administrativo em produção, com `DeveAlterarSenha=true`.

Pendências para Sprint Autenticação 9:
- Entrega do envio transacional real de e-mail para recuperação de senha.
- Evolução para trilha dedicada de auditoria persistida em banco.
- Homologação formal em ambiente publicado dos fluxos de recuperação e lockout.

## Correções - Integração Microsoft e senha atribuída por Administrador

Entregas:
- Menu administrativo de Integrações com item `Microsoft Entra ID`.
- Tela administrativa em `/admin/integracoes/microsoft-entra-id`.
- Endpoints administrativos:
  - `GET /api/admin/integracoes/microsoft-entra-id`
  - `PUT /api/admin/integracoes/microsoft-entra-id`
- Login (`/api/auth/provedores`) passa a refletir configuração administrativa de provedores.
- Redefinição administrativa de senha:
  - `POST /api/admin/cadastros/usuarios/{id}/redefinir-senha`
  - valida política de senha, aplica hash seguro e permite `deveAlterarSenha`.

Permissões novas:
- `IntegracoesMicrosoft.Visualizar`
- `IntegracoesMicrosoft.Gerenciar`
- `Usuarios.RedefinirSenha`

Regras de segurança:
- Senha nunca é armazenada em texto puro.
- Hash de senha não é retornado pela API.
- Senha não é registrada em log.
- Não é permitido salvar configuração sem nenhum provedor de autenticação ativo.

## Correção urgente - usuários demonstrativos e validação Microsoft Entra ID

### Usuários demonstrativos oficiais (Development)

Manter ativos apenas os seis usuários abaixo:

- `admin@sgxdigital.com`
- `admin2@sgxdigital.com`
- `atendente.demo@sgxdigital.com`
- `atendente2.demo@sgxdigital.com`
- `solicitante.demo@sgxdigital.com`
- `solicitante2.demo@sgxdigital.com`

Regras:

- Usuários antigos de `seed`, `homol` e `local` devem ser inativados.
- Qualquer usuário demonstrativo legado no domínio `@sgx.local` deve ser inativado.
- Usuários demonstrativos legados não devem ser recriados automaticamente após inativação.
- Não excluir fisicamente usuários reais.
- Não inativar automaticamente o administrador inicial seguro quando for usuário real (`SGX_ADMIN_INICIAL_*`).

### Campos obrigatórios da integração Microsoft Entra ID

Quando a integração Microsoft estiver habilitada em `MicrosoftEntraId` ou `Hibrido`, são obrigatórios:

- `Tenant ID`
- `Client ID`
- `Audience`
- `Issuer`
- `Authority`
- `API Scope`
- `Redirect URI`

Validações:

- O backend rejeita `PUT /api/admin/integracoes/microsoft-entra-id` com qualquer campo obrigatório ausente.
- Em `Local`, `LoginLocalHabilitado` deve ser `true`.
- Em `Local` com `LoginLocalHabilitado=false`, a API retorna a mensagem: `Login local SGX deve permanecer habilitado quando o modo Local estiver selecionado.`
- O sistema rejeita qualquer configuração sem provedor de autenticação ativo.
