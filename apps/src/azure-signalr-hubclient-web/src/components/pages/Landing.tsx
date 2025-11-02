import { useAuth } from "react-oidc-context";
import { Button } from "@/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle
} from "@/components/ui/card";

export function Landing() {
  const auth = useAuth();

  return (
    <div className="fixed inset-0 bg-background text-foreground">
      <div className="h-full w-full grid place-items-center p-4">
        <Card className="w-full max-w-sm sm:max-w-md md:max-w-lg bg-card/80 backdrop-blur-md rounded-2xl shadow-xl">
          <CardHeader>
            <CardTitle>SignalR WebClient POC</CardTitle>
            <CardDescription>Sign in to check real time notifications.</CardDescription>
          </CardHeader>
          <CardContent />
          <CardFooter>
            <Button className="w-full" variant="secondary" onClick={async () => await auth.signinRedirect()}>Sign
              In</Button>
          </CardFooter>
        </Card>
      </div>
    </div>
  );
}