using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ITHelpDesk.API.Hubs;

[Authorize]
public class NotificationHub : Hub
{
}