using System.Threading.Tasks;

namespace Bislerium.server.SignalR.Hubs
{
    public interface ICommentHubClient
    {
        Task ReceiveCommentNotification(Guid postId, string message);
    }
}
