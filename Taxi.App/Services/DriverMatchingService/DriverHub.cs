using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

public sealed class DriverHub : Hub
{
    public const string DriversGroup = "connected-drivers";

    public override async Task OnConnectedAsync()
    {
        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            DriversGroup);

        await base.OnConnectedAsync();
    }
}