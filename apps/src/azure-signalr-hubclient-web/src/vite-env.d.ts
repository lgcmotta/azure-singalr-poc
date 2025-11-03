interface ViteTypeOptions {
  strictImportMetaEnv: unknown
}

interface ImportMetaEnv {
  readonly VITE_APP_TITLE: string
  readonly VITE_KEYCLOAK_URL: string
  readonly VITE_KEYCLOAK_REALM: string
  readonly VITE_KEYCLOAK_CLIENT_ID: string
  readonly VITE_SIGNALR_HUB_URL: string
  readonly VITE_SIGNALR_TRANSPORT: string
  readonly VITE_APIM_SUBSCRIPTION_KEY: string
  readonly VITE_SIGNALR_SKIP_NEGOTIATION: string
}

interface ImportMeta {
  readonly env: ImportMetaEnv
}