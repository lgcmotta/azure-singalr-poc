import { Button } from "@/components/ui/button"
import { useAuth } from "react-oidc-context";


export function Landing() {
  const auth = useAuth()
  return (
    <div className="min-h-screen flex items-center justify-center">
      <Button onClick={async () => await auth.signinRedirect()}>Login</Button>
    </div>
  )
}