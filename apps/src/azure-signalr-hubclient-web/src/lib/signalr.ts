import { HttpTransportType, type HubConnection, HubConnectionBuilder } from "@microsoft/signalr";

export type Notification = {
  message: string
}

const URL = !import.meta.env.VITE_APIM_SUBSCRIPTION_KEY
  ? import.meta.env.VITE_SIGNALR_HUB_URL
  : `${import.meta.env.VITE_SIGNALR_HUB_URL}?subscription-key=${import.meta.env.VITE_APIM_SUBSCRIPTION_KEY}`



class Connector {
  private connection: HubConnection;

  public events: (onMessageReceived: (notification: Notification) => void) => void;

  static instance: Connector;

  constructor(accessTokenFactory: ()=> Promise<string>) {
    this.connection = new HubConnectionBuilder()
      .withUrl(URL, {
        accessTokenFactory: accessTokenFactory,
        transport: HttpTransportType.LongPolling
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

  public static getInstance(accessTokenFactory: ()=> Promise<string>): Connector {
    if (!Connector.instance)
      Connector.instance = new Connector(accessTokenFactory);
    return Connector.instance;
  }
}

export default Connector.getInstance;