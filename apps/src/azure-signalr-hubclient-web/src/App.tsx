import { Router } from 'wouter'
import { AuthProvider } from 'react-oidc-context';
import { AppRoutes } from "@/components/routes/AppRoutes.tsx";
import { onSignInCallback, userManager } from "@/config.ts";
import { ThemeProvider } from "@/components/theme/ThemeProvider";
import "./App.css"

function App() {
  return (
    <ThemeProvider defaultTheme="dark" storageKey="vite-ui-theme">
      <Router>
        <AuthProvider userManager={userManager} onSigninCallback={onSignInCallback}>
          <AppRoutes />
        </AuthProvider>
      </Router>
    </ThemeProvider>
  )
}

export default App
