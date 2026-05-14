# Configuração Azure AD / Microsoft Entra ID

## Princípio

- Microsoft Entra ID autentica identidade corporativa.
- SGX Sistema de Chamados autoriza por perfis e permissões internos.

## Modo Single Tenant

- O SGX usa Microsoft Entra ID em modo Single Tenant.
- Somente contas do tenant configurado são aceitas.
- Contas pessoais Microsoft, como Outlook, Hotmail e Live, não são aceitas.
- Contas de tenants externos não são aceitas por padrão.
- O tenant é validado por `AzureAd__TenantId`, `AzureAd__Issuer` e claim `tid`.
- A API também valida `audience`, assinatura e expiração do token.
- `Authentication__DominiosPermitidos` pode restringir domínio de e-mail adicionalmente.
- `roles/groups` do Azure não concedem permissões administrativas automaticamente no SGX.

## Contas permitidas

Permitido:
- usuário corporativo do tenant configurado
- domínio permitido, quando configurado
- usuário interno ativo no SGX

Bloqueado:
- conta pessoal Microsoft
- usuário de outro tenant
- domínio fora da lista permitida
- usuário interno inativo
- token com `issuer`/`audience`/`tid` inválidos

## Provedores de autenticação no SGX

Configuração principal no backend:

- `Authentication__ProvedorPrincipal`: `MicrosoftEntraId` | `Local` | `Hibrido`
- `Authentication__LoginLocalHabilitado`: habilita login local SGX
- `Authentication__ModoLocalHabilitado`: habilita apenas o login local Development

Regras:

- `MicrosoftEntraId`: Microsoft como principal.
- `Local`: login local SGX como principal.
- `Hibrido`: Microsoft + login local SGX.

## Variáveis do backend (API)

### Microsoft Entra ID (`AzureAd__*`)

Obrigatórias quando `ProvedorPrincipal` for `MicrosoftEntraId` ou `Hibrido`:

- `AzureAd__Instance`
- `AzureAd__TenantId`
- `AzureAd__ClientId`
- `AzureAd__Audience`
- `AzureAd__Issuer`
- `AzureAd__MetadataAddress` (opcional)

### Autenticação SGX (`Authentication__*`)

- `Authentication__ProvedorPrincipal`
- `Authentication__LoginLocalHabilitado`
- `Authentication__ModoLocalHabilitado`
- `Authentication__DominiosPermitidos`
- `Authentication__CriarUsuarioAutomaticamente`
- `Authentication__PerfilPadraoUsuarioMicrosoft`
- `Authentication__AdminLocalEmail` (apoio ao Development)
- `Authentication__AdminLocalNome` (apoio ao Development)
- `Authentication__AdminLocalSenha` (opcional para seed Development)
- `Authentication__JwtLocalIssuer`
- `Authentication__JwtLocalAudience`
- `Authentication__JwtLocalChaveAssinatura` (mínimo 32 caracteres)
- `Authentication__JwtLocalExpiracaoMinutos`
- `SGX_ADMIN_INICIAL_EMAIL`
- `SGX_ADMIN_INICIAL_SENHA`
- `SGX_ADMIN_INICIAL_NOME`

## Variáveis do frontend (Web)

- `VITE_API_BASE_URL`
- `VITE_AZURE_CLIENT_ID`
- `VITE_AZURE_TENANT_ID`
- `VITE_AZURE_AUTHORITY`
- `VITE_AZURE_REDIRECT_URI`
- `VITE_AZURE_API_SCOPE`
- `VITE_AUTH_MODO_LOCAL` (apenas desenvolvimento)

## Login local SGX x login local Development

### Login local SGX

- Usado em `Local` ou `Hibrido` quando `Authentication__LoginLocalHabilitado=true`.
- Endpoint: `POST /api/auth/local/login`.
- Senha armazenada com hash seguro.
- `/api/me` retorna `autenticadoPor=LocalSgx`.

### Login local Development

- Só aparece em `Development`.
- Depende de `Authentication__ModoLocalHabilitado=true`.
- Não substitui Microsoft Entra ID nem login local SGX de produção.
- `/api/me` retorna `autenticadoPor=LocalDevelopment`.

## Exemplo de cenários

### Cenário 1 - Apenas Microsoft

- `Authentication__ProvedorPrincipal=MicrosoftEntraId`
- `Authentication__LoginLocalHabilitado=false`
- `Authentication__ModoLocalHabilitado=false`

### Cenário 2 - Apenas local SGX

- `Authentication__ProvedorPrincipal=Local`
- `Authentication__LoginLocalHabilitado=true`
- `Authentication__ModoLocalHabilitado=false`

### Cenário 3 - Híbrido

- `Authentication__ProvedorPrincipal=Hibrido`
- `Authentication__LoginLocalHabilitado=true`
- `Authentication__ModoLocalHabilitado=false`

### Cenário 4 - Desenvolvimento técnico

- `ASPNETCORE_ENVIRONMENT=Development`
- `Authentication__ModoLocalHabilitado=true`
- (opcional) `VITE_AUTH_MODO_LOCAL=true`

## Segurança

- Não usar senha em texto puro.
- Não versionar secrets reais.
- Não usar `ModoLocalHabilitado=true` fora de `Development`.
- MFA e Conditional Access continuam responsabilidade do Microsoft Entra ID.
- `roles/groups` do Azure não concedem permissões automaticamente no SGX.

### Administrador inicial seguro

- `SGX_ADMIN_INICIAL_*` é um mecanismo de bootstrap inicial.
- Não deve ser mantido indefinidamente após a criação do primeiro Administrador.
- Não usar `Admin@123456` em produção.
- Nunca versionar senha real nem registrar senha em log.

## Validação rápida

- `GET /api/auth/provedores` retorna provedores disponíveis.
- `POST /api/auth/local/login` retorna JWT quando credenciais locais são válidas.
- `GET /api/me` retorna:
  - `MicrosoftEntraId` no fluxo Microsoft;
  - `LocalSgx` no fluxo local SGX;
  - `LocalDevelopment` apenas no Development local.

## Gestão administrativa da integração Microsoft Entra ID

Tela administrativa:
- `/admin/integracoes/microsoft-entra-id`

Endpoints:
- `GET /api/admin/integracoes/microsoft-entra-id`
- `PUT /api/admin/integracoes/microsoft-entra-id`

Permissões:
- `IntegracoesMicrosoft.Visualizar`
- `IntegracoesMicrosoft.Gerenciar`

Observações:
- A tela não expõe client secret.
- Em arquiteturas baseadas em `appsettings`/variáveis de ambiente, pode ser necessário reiniciar a API após alterações.
- A configuração evita estado sem provedor ativo (Microsoft e login local SGX desabilitados simultaneamente).

## Regras por modo e obrigatoriedade de campos

Tela administrativa: `/admin/integracoes/microsoft-entra-id`.

Quando `Integração habilitada=true` e o modo for `MicrosoftEntraId` ou `Hibrido`, os campos obrigatórios são:

- `Tenant ID`
- `Client ID`
- `Audience`
- `Issuer`
- `Authority`
- `API Scope`
- `Redirect URI`

Mensagens de validação por campo:

- Tenant ID é obrigatório quando a integração Microsoft está habilitada.
- Client ID é obrigatório quando a integração Microsoft está habilitada.
- Audience é obrigatória quando a integração Microsoft está habilitada.
- Issuer é obrigatório quando a integração Microsoft está habilitada.
- Authority é obrigatória quando a integração Microsoft está habilitada.
- API Scope é obrigatório quando a integração Microsoft está habilitada.
- Redirect URI é obrigatório quando a integração Microsoft está habilitada.

Regras adicionais:

- `Local`: `LoginLocalHabilitado` deve permanecer `true`.
- Se `Local` for salvo com `LoginLocalHabilitado=false`, a API rejeita com mensagem clara de validação.
- `Hibrido`: login Microsoft e login local SGX podem coexistir.
- O sistema não permite salvar sem ao menos um provedor ativo.
