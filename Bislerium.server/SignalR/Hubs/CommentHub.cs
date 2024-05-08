using Microsoft.AspNetCore.SignalR;

namespace Bislerium.server.SignalR.Hubs
{
    public class CommentHub : Hub<ICommentHubClient>
    {
        public async Task SendCommentNotification(string userId, int postId, string message)
        {
            await Clients.User(userId).ReceiveCommentNotification(postId, message);
        }
    }
}
