import { Router } from 'wouter'
import { AuthProvider } from 'react-oidc-context';
import { AppRoutes } from "@/components/routes/AppRoutes.tsx";
import { onSignInCallback, userManager } from "@/config.ts";

function App() {
  return (
    <Router>
      <AuthProvider userManager={userManager} onSigninCallback={onSignInCallback}>
        <AppRoutes />
      </AuthProvider>
    </Router>
  )
}

export default App
