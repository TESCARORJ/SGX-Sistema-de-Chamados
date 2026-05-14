# SGX.SistemaChamado

Sistema institucional para abertura, atendimento e acompanhamento de chamados.

## Stack

- Backend: .NET 9 (ASP.NET Core + Worker Service)
- Frontend: Vue 3 + Quasar
- Banco: PostgreSQL
- Persistência: Entity Framework Core

## Arquitetura

```text
src/
  SGX.SistemaChamado.Domain
  SGX.SistemaChamado.Application
  SGX.SistemaChamado.Infrastructure
  SGX.SistemaChamado.Api
  SGX.SistemaChamado.Worker.Email
  SGX.SistemaChamado.Web
tests/
  SGX.SistemaChamado.Tests
```

## Autenticação e autorização

- Microsoft Entra ID autentica identidade corporativa.
- SGX autoriza por perfis e permissões internos (`Administrador`, `Atendente`, `Solicitante`).

### Provedores suportados

Configuração principal no backend:

- `Authentication__ProvedorPrincipal`: `MicrosoftEntraId` | `Local` | `Hibrido`
- `Authentication__LoginLocalHabilitado`: `true`/`false`

Regras:

- `MicrosoftEntraId`: login Microsoft como principal.
- `Local`: login local SGX como principal.
- `Hibrido`: Microsoft + login local SGX.

### Microsoft Entra ID (Single Tenant)

- O SGX opera em modo Single Tenant.
- Somente contas corporativas do tenant configurado podem autenticar.
- Contas pessoais Microsoft (Outlook/Hotmail/Live) são bloqueadas por padrão.
- Contas de tenants externos são bloqueadas por padrão.
- A API valida `TenantId`, `issuer`, `tid`, `audience`, assinatura e expiração.
- O SGX continua autorizando internamente por perfis e permissões.

### Login local SGX (produção/homologação)

- Endpoint: `POST /api/auth/local/login`
- JWT local assinado pela API
- Senha armazenada com hash seguro (`PasswordHasher<Usuario>`)
- `/api/me` retorna `autenticadoPor=LocalSgx`
- Suporte a lockout e registro de `UltimoLoginEm`
- Suporte a troca obrigatória com `deveAlterarSenha`

### Recuperação e troca de senha local SGX

- `POST /api/auth/local/alterar-senha`
- `POST /api/auth/local/recuperar-senha/solicitar`
- `POST /api/auth/local/recuperar-senha/redefinir`
- Solicitação de recuperação retorna mensagem genérica (sem enumeração de usuário).
- Token de recuperação é temporário, hasheado e de uso único.
- Em troca obrigatória, frontend redireciona para `/alterar-senha`.

### Administrador inicial seguro (bootstrap)

- Variáveis obrigatórias:
  - `SGX_ADMIN_INICIAL_EMAIL`
  - `SGX_ADMIN_INICIAL_SENHA`
  - `SGX_ADMIN_INICIAL_NOME`
- Criação ocorre apenas se ainda não existir Administrador ativo.
- Senha é hasheada com `PasswordHasher<Usuario>`.
- Senha não é salva em texto puro e não é logada.
- Após o primeiro bootstrap, remova/rotacione essas variáveis.
- `Admin@123456` não é permitido para bootstrap de produção.

### Login local Development (somente desenvolvimento)

- Depende de `Authentication__ModoLocalHabilitado=true`
- Restrito ao ambiente `Development`
- Não substitui o login local SGX de produção
- `/api/me` retorna `autenticadoPor=LocalDevelopment`

## Variáveis de ambiente principais

### Backend

- `ConnectionStrings__DefaultConnection`
- `Authentication__ProvedorPrincipal`
- `Authentication__LoginLocalHabilitado`
- `Authentication__ModoLocalHabilitado`
- `Authentication__JwtLocalIssuer`
- `Authentication__JwtLocalAudience`
- `Authentication__JwtLocalChaveAssinatura`
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
- `SGX_ADMIN_INICIAL_EMAIL`
- `SGX_ADMIN_INICIAL_SENHA`
- `SGX_ADMIN_INICIAL_NOME`
- `AzureAd__Instance`
- `AzureAd__TenantId`
- `AzureAd__ClientId`
- `AzureAd__Audience`
- `AzureAd__Issuer`

### Frontend

- `VITE_API_BASE_URL`
- `VITE_AZURE_CLIENT_ID`
- `VITE_AZURE_TENANT_ID`
- `VITE_AZURE_AUTHORITY`
- `VITE_AZURE_REDIRECT_URI`
- `VITE_AZURE_API_SCOPE`
- `VITE_AUTH_MODO_LOCAL` (apenas desenvolvimento)

## Execução local

### Backend

```bash
dotnet restore SGX.SistemaChamado.sln
dotnet build SGX.SistemaChamado.sln
dotnet test SGX.SistemaChamado.sln
```

### Frontend

```bash
cd src/SGX.SistemaChamado.Web
npm install
npm run build
```

## Documentação

- `docs/AUTENTICACAO-CORPORATIVA.md`
- `docs/CONFIGURACAO-AZURE-AD.md`
- `docs/ROADMAP.md`
- `docs/ROADMAP-ITSM.md`
- `docs/EXECUCAO-LOCAL.md`
- `docs/ARQUITETURA.md`
- `docs/HOMOLOGACAO-CHECKLIST.md`
- `docs/SLA.md`

No painel administrativo, a consulta gerencial fica em:

- `Admin > Gestão ITSM > Roadmap` (`/admin/gestao-itsm/roadmap`)
- `Admin > Gestão ITSM > Documentação` (`/admin/gestao-itsm/documentacao`)
- A rota legada `/admin/roadmap-itsm` continua funcionando.

## Correções recentes - Integração Microsoft e senha por Administrador

- Menu administrativo de `Integrações` agora exibe:
  - `E-mail`
  - `Microsoft Entra ID`
- Nova tela administrativa:
  - `/admin/integracoes/microsoft-entra-id`
- Endpoints administrativos:
  - `GET /api/admin/integracoes/microsoft-entra-id`
  - `PUT /api/admin/integracoes/microsoft-entra-id`
- Login frontend usa `GET /api/auth/provedores` e oculta Microsoft quando desabilitado.
- Nova ação no detalhe de usuário:
  - `Redefinir senha` (`POST /api/admin/cadastros/usuarios/{id}/redefinir-senha`)
  - senha validada por política e armazenada com hash.
- Seed Development ajustado para manter apenas 2 usuários demonstrativos por perfil:
  - 2 Administradores
  - 2 Atendentes
  - 2 Solicitantes

## Usuários demonstrativos oficiais

Usuários ativos de demonstração permitidos no seed Development:

- Administradores:
  - `admin@sgxdigital.com`
  - `admin2@sgxdigital.com`
- Atendentes:
  - `atendente.demo@sgxdigital.com`
  - `atendente2.demo@sgxdigital.com`
- Solicitantes:
  - `solicitante.demo@sgxdigital.com`
  - `solicitante2.demo@sgxdigital.com`

Regras:

- Usuários demonstrativos antigos (`seed/homol/local`) devem permanecer inativos.
- Qualquer usuário demonstrativo legado no domínio `@sgx.local` deve permanecer inativo.
- Usuários demonstrativos legados não devem ser recriados automaticamente após inativação.
- Não remover fisicamente usuários reais.
- Usuário de administrador inicial seguro (`SGX_ADMIN_INICIAL_*`) não pode ser inativado automaticamente quando for administrador real.

## Configuração Microsoft Entra ID

No modo `MicrosoftEntraId` ou `Hibrido`, com integração habilitada, os campos abaixo são obrigatórios:

- `Tenant ID`
- `Client ID`
- `Audience`
- `Issuer`
- `Authority`
- `API Scope`
- `Redirect URI`

Regras adicionais:

- `Local`: exige `LoginLocalHabilitado=true`.
- Se `Local` estiver selecionado com `LoginLocalHabilitado=false`, a API rejeita com mensagem clara.
- O sistema não permite salvar configuração sem ao menos um provedor ativo.
