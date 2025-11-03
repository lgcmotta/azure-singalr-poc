import { HttpTransportType, type HubConnection, HubConnectionBuilder } from "@microsoft/signalr";

export type Notification = {
  message: string
}

const URL = !import.meta.env.VITE_APIM_SUBSCRIPTION_KEY
  ? import.meta.env.VITE_SIGNALR_HUB_URL
  : `${import.meta.env.VITE_SIGNALR_HUB_URL}?subscription-key=${import.meta.env.VITE_APIM_SUBSCRIPTION_KEY}`

type ConnectorOptions = {
  accessTokenFactory: () => Promise<string>,
  skipNegotiation: boolean
  transport: HttpTransportType
  subscriptionKey: string
}


class Connector {
  private connection: HubConnection;

  public events: (onMessageReceived: (notification: Notification) => void) => void;

  static instance: Connector;

  constructor(options: ConnectorOptions) {
    this.connection = new HubConnectionBuilder()
      .withUrl(URL, {
        headers: {
          "Ocp-Apim-Subscription-Key": options.subscriptionKey
        },
        accessTokenFactory: options.accessTokenFactory,
        skipNegotiation: options.skipNegotiation,
        transport: HttpTransportType.WebSockets
      })
      .withAutomaticReconnect()
      .build();

    this.connection.start().catch(err => console.log(err));

    this.events = (onMessageReceived) => {
      this.connection.on("receiveNotification", (payload) => {
        onMessageReceived(payload);
      });

      return () => this.connection.off("receiveNotification");
    };
  }

  public static getInstance(options: ConnectorOptions): Connector {
    if (!Connector.instance)
      Connector.instance = new Connector(options);
    return Connector.instance;
  }
}

export function convertTransportType(transport: keyof typeof HttpTransportType | string): HttpTransportType {
  switch (transport) {
    case "None":
      return HttpTransportType.None
    case "WebSockets":
      return HttpTransportType.WebSockets
    case "ServerSentEvents":
      return HttpTransportType.ServerSentEvents
    case "LongPolling":
      return HttpTransportType.LongPolling
  }

  return HttpTransportType.WebSockets
}

export function convertSkipNegotiation(skip: string) {
  if (skip === "false") return false
  return skip === "true";
}

export default Connector.getInstance;