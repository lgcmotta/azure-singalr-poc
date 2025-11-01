using Azure.SignalR.HubClient.Clients.Hubs.Responses;
using Azure.SignalR.HubClient.Clients.Keycloak;
using Azure.SignalR.HubClient.Clients.Keycloak.Options;
using Azure.SignalR.HubClient.Extensions;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddKeycloakClient(builder.Configuration);

var app = builder.Build();

var options = app.Services.GetRequiredService<IOptions<KeycloakOptions>>();

var keycloak = app.Services.GetRequiredService<IKeycloakApi>();

var hubUrl = app.Configuration.GetValue<string>("SignalR:BaseUrl");

if (string.IsNullOrWhiteSpace(hubUrl))
{
    return;
}

var subscriptionKey = app.Configuration.GetValue("ApiManager:SubscriptionKey", string.Empty);

var connection = new HubConnectionBuilder()
    .WithUrl(hubUrl, connection =>
    {
        connection.Transports = HttpTransportType.WebSockets;
        connection.SkipNegotiation = false;
        connection.Headers["Ocp-Apim-Subscription-Key"] = subscriptionKey;
        connection.AccessTokenProvider = async () =>
        {
            var body = options.Value.ToUrlEncodedBody();

            var response = await keycloak.PostCredentialsAsync(body);

            return response is { IsSuccessStatusCode: true, Content: not null }
                ? response.Content.AccessToken
                : string.Empty;
        };
    })
    .WithAutomaticReconnect()
    .Build();

connection.On<Notification>("receiveNotification",
    notification =>
    {
        app.Logger.LogInformation("Received notification with message {Message}", notification.Message);
    });

await connection.StartAsync();

await app.RunAsync();