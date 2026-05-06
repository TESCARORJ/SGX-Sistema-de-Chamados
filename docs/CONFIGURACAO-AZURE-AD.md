# Configuracao Azure AD / Microsoft Entra ID

## Principio

- Azure AD autentica usuario.
- SGX.SistemaChamado autoriza com perfis internos no banco.

## App Registration (SPA + API)

1. Criar App Registration para frontend SPA.
2. Definir Redirect URI da SPA.
3. Configurar escopo da API para emissao de token.
4. Configurar audiencia e issuer no backend.

## Campos principais

- `TenantId`
- `ClientId`
- `Audience`
- `Issuer`
- `Instance` (`https://login.microsoftonline.com/`)
- `Redirect URI` da SPA (`VITE_AZURE_REDIRECT_URI`)

## Variaveis backend

- `AzureAd__Instance`
- `AzureAd__TenantId`
- `AzureAd__ClientId`
- `AzureAd__Audience`
- `AzureAd__Issuer`

## Variaveis frontend

- `VITE_AZURE_CLIENT_ID`
- `VITE_AZURE_TENANT_ID`
- `VITE_AZURE_AUTHORITY`
- `VITE_AZURE_REDIRECT_URI`
- `VITE_AZURE_API_SCOPE`

## Escopos

- O frontend deve solicitar escopo compatível com a API.
- A API valida issuer e audience configurados.

## Development local

- Com `Authentication__ModoLocalHabilitado=true` em `Development`, o sistema permite autenticacao local por headers `X-Dev-*`.
- Esse modo nao deve ser usado em homologacao/producao.

## Pontos de atencao

- Nao versionar `ClientSecret` em repositorio.
- Nao expor valores sensiveis em logs.
- Testar fluxo real de login apenas com App Registration valido.
