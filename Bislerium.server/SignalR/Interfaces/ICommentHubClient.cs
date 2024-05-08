using System.Threading.Tasks;

namespace Bislerium.server.SignalR.Hubs
{
    public interface ICommentHubClient
    {
        Task ReceiveCommentNotification(int postId, string message);
    }
}
