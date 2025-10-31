using Azure.SignalR.HubApi.Extensions;
using Azure.SignalR.HubApi.Features.Broadcasting;
using Azure.SignalR.HubApi.Hubs;
using Keycloak.AuthServices.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration, options =>
{
    options.RequireHttpsMetadata = false;
});
builder.Services.AddKeycloakAuthorization(builder.Configuration);
builder.Services.AddOpenApiDocument(builder.Configuration);
builder.Services.AddCors();

var app = builder.Build();

app.UsePermissiveCors();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseOpenApiInterface();
app.MapHub<NotificationHub>("/hub").RequireAuthorization();
app.MapPostNotify();

await app.RunAsync();
