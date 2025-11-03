import { useAuth } from "react-oidc-context";
import { useEffect, useEffectEvent, useMemo, useState } from "react";
import Connector, { convertSkipNegotiation, convertTransportType, type Notification } from "@/lib/signalr";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Badge } from "@/components/ui/badge";
import { Separator } from "@/components/ui/separator";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";


export function Notifications() {
  const auth = useAuth();
  const [ notifications, setNotifications ] = useState<Notification[]>([]);
  const token = auth.user?.access_token ?? "";

  const handler = useMemo(
    () => (n: Notification) => setNotifications(old => [ ...old, n ]),
    []
  );

  const signOut = useEffectEvent(async () => await auth.signoutRedirect());

  useEffect(() => {
    if (!token) return undefined
    const { events } = Connector({
      accessTokenFactory: async () => token,
      subscriptionKey: import.meta.env.VITE_APIM_SUBSCRIPTION_KEY,
      transport: convertTransportType(import.meta.env.VITE_SIGNALR_TRANSPORT),
      skipNegotiation: convertSkipNegotiation(import.meta.env.VITE_SIGNALR_SKIP_NEGOTIATION)
    });

    return events(handler)
  }, [ token, handler ])

  const name = auth.user?.profile?.name ?? "user";
  const initials = name.split(" ").map(s => s[0]).join("").slice(0, 2).toUpperCase();

  return (
    <div className="min-h-screen flex flex-col bg-background text-foreground overflow-hidden">
      <header className="border-b bg-background/80 backdrop-blur">
        <div className="mx-auto max-w-7xl px-8 h-16 flex items-center gap-8">
          <div className="flex items-center gap-4">
            <Badge variant="secondary">Online</Badge>
            <Separator orientation="vertical" className="h-8" />
            <span className="font-semibold tracking-tight text-lg">
              SignalR WebClient POC
            </span>
          </div>

          <div className="ml-auto flex items-center gap-6">
            <Avatar className="h-8 w-8">
              <AvatarFallback>{initials || "U"}</AvatarFallback>
            </Avatar>
            <Button variant="outline" size="sm" onClick={signOut}>
              Logout
            </Button>
          </div>
        </div>
      </header>
      <main className="flex-1 min-h-0 flex items-start justify-center px-6 py-4">
        <Card className="w-full max-w-md flex flex-col h-[70vh]">
          <CardHeader className="pb-2 pt-4">
            <CardTitle className="text-center">Notifications</CardTitle>
          </CardHeader>

          <CardContent className="flex-1 min-h-0 p-0">
            <ScrollArea className="h-full px-6 pb-4">
              <ul className="space-y-3">
                {notifications.length === 0 && (
                  <CardContent className="text-sm text-muted-foreground text-center mt-4">
                    No messages yet.
                  </CardContent>
                )}
                {notifications.map((n, i) => (
                  <CardContent
                    key={i}
                    className="rounded-md border bg-card p-3 text-card-foreground"
                  >
                    {n.message}
                  </CardContent>
                ))}
              </ul>
            </ScrollArea>
          </CardContent>
        </Card>
      </main>
    </div>
  );
}