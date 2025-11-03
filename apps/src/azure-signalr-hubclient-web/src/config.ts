import { UserManager, WebStorageStateStore } from 'oidc-client-ts';

export const userManager = new UserManager({
  authority: `${import.meta.env.VITE_KEYCLOAK_URL}/realms/${import.meta.env.VITE_KEYCLOAK_REALM}`,
  client_id: import.meta.env.VITE_KEYCLOAK_CLIENT_ID,
  redirect_uri: `${window.location.origin}/`,
  post_logout_redirect_uri: `${window.location.origin}/`,
  userStore: new WebStorageStateStore({ store: window.sessionStorage }),
  monitorSession: true,
});

export const onSignInCallback = () => {
  window.history.replaceState({}, document.title, window.location.pathname);
};