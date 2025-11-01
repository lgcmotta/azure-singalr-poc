using JetBrains.Annotations;

namespace Azure.SignalR.HubClient.Clients.Hubs.Responses;

[UsedImplicitly]
public record Notification
{
    public required string Message { get; init; }
}