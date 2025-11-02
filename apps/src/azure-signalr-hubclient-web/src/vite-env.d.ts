interface ViteTypeOptions {
  strictImportMetaEnv: unknown
}

interface ImportMetaEnv {
  readonly VITE_APP_TITLE: string
  readonly VITE_KEYCLOAK_URL: string
  readonly VITE_KEYCLOAK_REALM: string
  readonly VITE_KEYCLOAK_CLIENT_ID: string
  readonly VITE_API_BASE: string
  readonly VITE_SIGNALR_HUB_URL: string
  readonly VITE_APIM_SUBSCRIPTION_KEY: string
}

interface ImportMeta {
  readonly env: ImportMetaEnv
}