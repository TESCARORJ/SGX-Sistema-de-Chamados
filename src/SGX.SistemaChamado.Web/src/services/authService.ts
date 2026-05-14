import {
  BrowserAuthError,
  InteractionRequiredAuthError,
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
      'Microsoft Entra ID não configurado no frontend. Defina VITE_AZURE_CLIENT_ID e VITE_AZURE_TENANT_ID.'
    )
  }
}

function isMsalUserCancellation(error: unknown): boolean {
  if (
    error instanceof BrowserAuthError &&
    (error.errorCode === 'user_cancelled' || error.errorCode === 'user_cancelled_flow')
  ) {
    return true
  }

  if (typeof error === 'object' && error !== null) {
    const candidate = error as { errorCode?: string; message?: string }
    const code = candidate.errorCode?.toLowerCase() ?? ''
    const message = candidate.message?.toLowerCase() ?? ''

    return code.includes('user_cancelled') || message.includes('user cancelled')
  }

  return false
}

function isInteractionRequired(error: unknown): boolean {
  return error instanceof InteractionRequiredAuthError
}

export const authService = {
  isAzureConfigured(): boolean {
    return Boolean(clientId && tenantId)
  },

  isUserCancellation(error: unknown): boolean {
    return isMsalUserCancellation(error)
  },

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

    let token: AuthenticationResult
    try {
      token = await app.acquireTokenSilent({
        ...loginRequest,
        account: activeAccount,
      })
    } catch (error) {
      if (isInteractionRequired(error)) {
        token = await app.acquireTokenPopup({
          ...loginRequest,
          account: activeAccount,
        })
      } else {
        throw error
      }
    }

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
