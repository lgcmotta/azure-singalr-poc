using Microsoft.AspNetCore.SignalR;

namespace Azure.SignalR.HubApi.Hubs;

public class NotificationHub : Hub
{
    public override Task OnConnectedAsync()
    {
        return base.OnConnectedAsync();
    }
}