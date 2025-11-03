using Azure.SignalR.HubApi.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Azure.SignalR.HubApi.Features.Broadcasting;

internal record NotificationRequest(string Message);

internal static class BroadcastEndpoint
{
    internal static WebApplication MapPostNotify(this WebApplication app)
    {
        app.MapPost("/notify", Post)
            .WithName("post-notify")
            .WithDisplayName("Notify Connected Clients")
            .WithDescription("Sends a hello world notification to all clients connected to SignalR")
            .WithTags("notifications")
            .RequireAuthorization();

        return app;
    }

    private static async Task<IResult> Post([FromServices] IHubContext<NotificationHub> hub,
        [FromBody] NotificationRequest request,
        CancellationToken cancellationToken = default)
    {
        await hub.Clients.All.SendAsync(
            method: "receiveNotification",
            arg1: request,
            cancellationToken: cancellationToken
        );

        return Results.Accepted();
    }
}