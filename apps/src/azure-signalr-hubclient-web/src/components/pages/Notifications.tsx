import { type AuthContextProps, useAuth } from "react-oidc-context";
import { HttpTransportType, HubConnectionBuilder } from "@microsoft/signalr";
import { Suspense, useEffect, useState } from "react";

type Notification = {
  message: string
}

type SignalRConnectOptions = {
  auth: AuthContextProps
}

function createConnection({ auth }: SignalRConnectOptions) {
  const hubUrl = import.meta.env.VITE_SIGNALR_HUB_URL
  const subscriptionKey = import.meta.env.VITE_APIM_SUBSCRIPTION_KEY

  return new HubConnectionBuilder()
    .withUrl(`${hubUrl}?subscription-key=${subscriptionKey}`, {
      accessTokenFactory: async () => (auth.user?.access_token ?? ""),
      headers: {
        "Ocp-Apim-Subscription-Key": subscriptionKey,
      },

      skipNegotiation: false,
      transport: HttpTransportType.WebSockets
    })
    .withAutomaticReconnect()
    .build()
}

export function Notifications() {
  const auth = useAuth()
  const [ notifications, setNotifications ] = useState<Notification[]>([])

  useEffect(() => {
    if (!auth.isAuthenticated || !auth.user?.access_token) {
      return () => { }
    }

    const connection = createConnection({ auth })

    connection.on("receiveNotification", payload => {
      setNotifications(n => [...n, payload])
    })

    connection.start().catch(r => console.log(r))

    return async () => {
      connection.off("receiveNotification")
      connection.stop().catch(r => console.log(r))
    }
  }, [auth])

  return (
    <Suspense fallback={<div>Connecting...</div>}>
      <div className="min-h-screen p-6 space-y-4">
        <ul className="space-y-2">
          {
            notifications.map((notification, index) => (
              <li key={index} className="border rounded p-3">{notification.message}</li>)
            )
          }
        </ul>
      </div>
    </Suspense>
  )
}