import { Redirect, Route, Switch } from "wouter";
import { Landing } from "@/components/pages/Landing.tsx";
import { Notifications } from "@/components/pages/Notifications.tsx";
import { useAuth } from "react-oidc-context";

export function AppRoutes() {
  const auth = useAuth()

  return (
    <Switch>
      <Route path="/">
        {auth.isAuthenticated ? <Redirect to="/app" /> : <Landing />}
      </Route>
      <Route path="/app">
        {auth.isAuthenticated ? <Notifications /> : <Redirect to="/" />}
      </Route>
    </Switch>
  )
}