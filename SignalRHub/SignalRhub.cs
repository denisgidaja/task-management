using Microsoft.AspNetCore.SignalR;

namespace SignalRHub
{
    public class SignalRhub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
