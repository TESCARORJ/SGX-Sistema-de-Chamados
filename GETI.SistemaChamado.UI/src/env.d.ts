/// <reference types="vite/client" />

interface ImportMetaEnv {
  readonly VITE_API_BASE_URL?: string;
  readonly VITE_AUTH_LOCAL_LOGIN?: string;
  readonly VITE_AUTH_LOCAL_NOME?: string;
  readonly VITE_AUTH_LOCAL_EMAIL?: string;
  readonly VITE_AUTH_LOCAL_SENHA?: string;
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}
