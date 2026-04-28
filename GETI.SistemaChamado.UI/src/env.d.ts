/// <reference types="vite/client" />

interface ImportMetaEnv {
  readonly VITE_AUTH_LOCAL_LOGIN?: string;
  readonly VITE_AUTH_LOCAL_NOME?: string;
  readonly VITE_AUTH_LOCAL_EMAIL?: string;
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}
