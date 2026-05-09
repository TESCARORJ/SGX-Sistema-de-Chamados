import {
  PublicClientApplication,
  type AccountInfo,
  type AuthenticationResult,
  type PopupRequest,
} from '@azure/msal-browser'

const clientId = import.meta.env.VITE_AZURE_CLIENT_ID ?? ''
const tenantId = import.meta.env.VITE_AZURE_TENANT_ID ?? ''
const redirectUri = import.meta.env.VITE_AZURE_REDIRECT_URI ?? window.location.origin
const authority =
  import.meta.env.VITE_AZURE_AUTHORITY ??
  `https://login.microsoftonline.com/${tenantId || 'common'}`

const apiScope = (import.meta.env.VITE_AZURE_API_SCOPE as string | undefined) ?? ''
const defaultScopes = apiScope ? [apiScope] : ['openid', 'profile', 'email']

const loginRequest: PopupRequest = {
  scopes: defaultScopes,
}

const app = new PublicClientApplication({
  auth: {
    clientId,
    authority,
    redirectUri,
  },
  cache: {
    cacheLocation: 'sessionStorage',
    storeAuthStateInCookie: false,
  },
})

let initialized = false

async function ensureInitialized(): Promise<void> {
  if (initialized) {
    return
  }

  await app.initialize()
  initialized = true
}

function ensureAzureConfigured(): void {
  if (!clientId || !tenantId) {
    throw new Error(
      'Azure AD não configurado no frontend. Defina VITE_AZURE_CLIENT_ID e VITE_AZURE_TENANT_ID.'
    )
  }
}

export const authService = {
  async loginPopup(): Promise<AuthenticationResult> {
    ensureAzureConfigured()
    await ensureInitialized()
    const result = await app.loginPopup(loginRequest)
    if (result.account) {
      app.setActiveAccount(result.account)
    }
    return result
  },

  async acquireAccessToken(account?: AccountInfo | null): Promise<string | null> {
    ensureAzureConfigured()
    await ensureInitialized()
    const activeAccount = account ?? app.getActiveAccount() ?? app.getAllAccounts()[0] ?? null
    if (!activeAccount) {
      return null
    }

    app.setActiveAccount(activeAccount)

    const token = await app.acquireTokenSilent({
      ...loginRequest,
      account: activeAccount,
    })

    return token.accessToken
  },

  async getAccount(): Promise<AccountInfo | null> {
    await ensureInitialized()
    return app.getActiveAccount() ?? app.getAllAccounts()[0] ?? null
  },

  async logout(): Promise<void> {
    await ensureInitialized()
    const account = app.getActiveAccount() ?? app.getAllAccounts()[0] ?? undefined
    await app.logoutPopup({ account })
  },
}
