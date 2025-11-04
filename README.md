# Azure SignalR + API Management POC

This repository is a POC exploring how Azure SignalR works behind Azure API Management (APIM), with authentication/authorization via Keycloak. 
It includes IaC with OpenTofu, a .NET 9 Hub API hosted on Azure Container Apps, a .NET 9 console client, 
and a React web client hosted on Azure Static Web Apps (SWA).

## Architecture

![Alt text for the image](assets/azure-diagram.svg)

1. User accesses the **React** web client through the **Azure Static Web Apps** exposed URL.
2. User signs in to the application using **Keycloak** with OAuth2 (Authorization Code with PKCE).
3. **React** web client uses **Microsoft's SignalR** package to request a hub connection with the **ASP.NET Core** 
Web Application using the exposed HTTPS endpoints in **Azure API Management**.
4. Using the **Azure API Management** subscription key and the **Keycloak** JWT Bearer token, the request is received by the 
**ASP.NET Core** Web Application, which validates the JWT Bearer token against **Keycloak**.
5. If the JWT Bearer token is accepted, the backend will reply with the **Azure SignalR** negotiation payload.
6. The **React** web client will automatically initiate the negotiation payload with **Azure SignalR** through 
**Microsoft's SignalR** package using the exposed HTTPS endpoints in **Azure API Management**.
7. The **React** web client uses the negotiation payload to connect with **Azure SignalR** using the exposed WebSockets API 
in **Azure API Management**.


## Environment

- Azure CLI and AWS CLI (`az` and `aws`)
- OpenTofu (or Terraform)
- .NET 9 SDK
- Node 22
- Docker (only required for local development).

> Note: If it fits your scenario, you may switch AWS S3 with Azure Blob Storage as the backend for OpenTofu. 
> Since I have all of my `tfstate` files hosted on S3, I have opted to use the same bucket for this POC.

## Provisioning Azure Infrastructure

Replace the Keycloak realm placeholders in the `iac/modules/realm/sample.realm.json` file and rename it to `realm.json`.

> Note: if you want to run this environment locally with Docker, rename it to `realm.local.json`.

Create the `terraform.tfvars` into the `iac` directory and run the commands `tofu plan -out out/tfplan` and `tofu apply out/tfplan`.

> Note: Excluding the SignalR API module is required for the first plan to avoid the following error:
> * `service_url` is required when `api_type` is `websocket`

 
Collect the output variables and provide them as environment variables to the `Azure.SignalR.HubApi` (backend) 
and `azure-signalr-hubclient-web` (frontend):

> Note: For the .NET 9 project (`Azure.SignalR.HubApi`) feel free to store the variables using .NET User Secrets (`secrets.json`).


## Hub API Environment Variables

| Name                               | Description                                                                                  |
|------------------------------------|----------------------------------------------------------------------------------------------|
| `Azure__SignalR__ConnectionString` | SignalR connection string with APIM gateway client endpoint override.                        |
| `Keycloak__AdminUrl`               | Base URL for Keycloak Admin API behind APIM.                                                 |
| `Keycloak__Realm`                  | Keycloak realm name used for authentication.                                                 |
| `Keycloak__AuthServerUrl`          | Keycloak authentication base URL.                                                            |
| `Keycloak__SslRequired`            | Defines SSL enforcement for Keycloak.                                                        |
| `Keycloak__Resource`               | Keycloak client ID for the backend service.                                                  |
| `Keycloak__PublicClient`           | Marks the client as public.                                                                  |
| `Keycloak__ConfidentialPort`       | Port used for confidential Keycloak clients (0 = none).                                      |
| `Keycloak__VerifyTokenAudience`    | Whether to verify the audience in access tokens.                                             |
| `Keycloak__RequireHttpsMetadata`   | Whether Keycloak metadata must be retrieved over HTTPS. (Use `false` for Keycloak in Docker) |
| `Keycloak__Scopes__0`              | OAuth2 scope 1.                                                                              |
| `Keycloak__Scopes__1`              | OAuth2 scope 2.                                                                              |
| `Keycloak__Scopes__2`              | OAuth2 scope 3.                                                                              |
| `OpenApi__Title`                   | OpenAPI document title.                                                                      |
| `OpenApi__Server`                  | Base API server URL (behind APIM).                                                           |
| `OpenApi__SubscriptionRequired`    | Whether a subscription key is required to access the API.                                    |
| `OpenApi__DocumentEndpoint`        | Internal OpenAPI document endpoint.                                                          |
| `OpenApi__SwaggerEndpoint`         | Swagger UI endpoint with APIM subscription placeholder.                                      |
| `OpenApi__Contact__Name`           | Contact name in the OpenAPI document.                                                        |
| `OpenApi__Contact__Email`          | Contact email in the OpenAPI document.                                                       |
| `CorsOrigins__0`                   | CORS allowed origin (APIM Gateway).                                                          |
| `CorsOrigins__1`                   | CORS allowed origin for local web client on port 5281.                                       |
| `CorsOrigins__2`                   | CORS allowed origin for the Azure Static Web App.                                            |
| `CorsOrigins__3`                   | CORS allowed origin for local Vite dev server.                                               |

## Hub Web Client Environment Variables

## Frontend Environment Variables

| Name                            | Description                                                                |
|---------------------------------|----------------------------------------------------------------------------|
| `VITE_APP_TITLE`                | Application title displayed in the browser tab and metadata.               |
| `VITE_KEYCLOAK_URL`             | Base URL of the Keycloak server used for authentication.                   |
| `VITE_KEYCLOAK_REALM`           | Keycloak realm used by the frontend client.                                |
| `VITE_KEYCLOAK_CLIENT_ID`       | Keycloak public client ID registered for the frontend.                     |
| `VITE_SIGNALR_HUB_URL`          | URL for connecting to the SignalR hub (through APIM).                      |
| `VITE_SIGNALR_TRANSPORT`        | SignalR transport type (`WebSockets`, `LongPolling`, etc.).                |
| `VITE_APIM_SUBSCRIPTION_KEY`    | Subscription key for authenticating with APIM gateway.                     |
| `VITE_SIGNALR_SKIP_NEGOTIATION` | Whether to skip the negotiation phase (set to `false` for APIM scenarios). |
