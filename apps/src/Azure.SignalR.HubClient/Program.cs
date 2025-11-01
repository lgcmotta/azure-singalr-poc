using Azure.SignalR.HubClient.Clients.Keycloak;
using Azure.SignalR.HubClient.Clients.Keycloak.Options;
using Azure.SignalR.HubClient.Extensions;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddKeycloakClient(builder.Configuration);

var app = builder.Build();

var options = app.Services.GetRequiredService<IOptions<KeycloakOptions>>();

var body = new Dictionary<string, string>
{
    { "client_id", options.Value.ClientId },
    { "client_secret", options.Value.ClientSecret },
    { "grant_type", options.Value.GrantType },
    { "scope", options.Value.Scopes }
};

await using var scope = app.Services.CreateAsyncScope();

var keycloak = scope.ServiceProvider.GetRequiredService<IKeycloakApi>();

var response = await keycloak.PostCredentialsAsync(body);

var hubUrl = app.Configuration.GetValue<string>("SignalR:BaseUrl");

if (string.IsNullOrWhiteSpace(hubUrl))
{
    return;
}

var connection = new HubConnectionBuilder()
    .WithUrl(hubUrl, connection =>
    {
        connection.Headers["Ocp-Apim-Subscription-Key"] = "3968fdfc31004f05bfc260f8eb61e608";
        connection.Transports = HttpTransportType.WebSockets;
        connection.AccessTokenProvider = () => Task.FromResult(response.Content?.AccessToken);
    })
    .WithAutomaticReconnect()
    .Build();

connection.On<string,string>("receiveNotification", (title, message) =>
{
    Console.WriteLine(title);
    Console.WriteLine(message);
});

await connection.StartAsync();