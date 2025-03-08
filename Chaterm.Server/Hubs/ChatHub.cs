using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Chaterm.Server.Hubs
{
    public class ChatHub : Hub<IChatClient>, IChatHub
    {
        public async Task SendMessage(Message message)
        {
            await Clients.OthersInGroup(message.GroupSecret).ReceiveMessage(message);
        }
        public async Task JoinGroup(string groupSecret)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupSecret);
        }
        public async Task LeaveGroup(string groupSecret)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupSecret);
        }
    }
}
