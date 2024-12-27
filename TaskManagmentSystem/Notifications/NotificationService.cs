using Microsoft.AspNetCore.SignalR;
using SignalRHub;

namespace TaskManagment.Consumer.Notifications
{
    public class NotificationService
    {
        private readonly IHubContext<SignalRhub> _hubContext;

        public NotificationService(IHubContext<SignalRhub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyClients(string user, string message)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
