using Bislerium.server.SignalR.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Bislerium.server.SignalR.Hubs
{
    public class ReactionHub : Hub<IReactionHubClient>
    {
        public async Task SendReactionNotification(string userId, Guid postId, string message)
        {
            await Clients.User(userId).ReceiveReactionNotification(postId, message);
        }
    }
}
