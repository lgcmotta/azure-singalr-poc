using Azure.SignalR.HubApi.Extensions;
using Azure.SignalR.HubApi.Features.Broadcasting;
using Azure.SignalR.HubApi.Hubs;
using Keycloak.AuthServices.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddContainerHealthChecks();
builder.Services.AddAzureSignalR(builder.Configuration);
builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration);
builder.Services.AddKeycloakAuthorization(builder.Configuration);
builder.Services.AddOpenApiDocument(builder.Configuration);
builder.Services.AddCors();

var app = builder.Build();

app.MapReadinessHealthCheck();
app.MapLivenessHealthCheck();
app.UsePermissiveCors();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseOpenApiInterface();
app.MapHub<NotificationHub>("/hub").RequireAuthorization();
app.MapPostNotify();

await app.RunAsync();
