using Microsoft.AspNetCore.SignalR;

namespace RetailXMVC.SignalR
{
    public class NotificationHub : Hub
    {
        public async Task JoinTenantGroup(string tenantId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Tenant_{tenantId}");
        }
    }
}
