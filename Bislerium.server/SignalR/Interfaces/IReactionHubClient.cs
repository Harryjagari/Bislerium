namespace Bislerium.server.SignalR.Interfaces
{
    public interface IReactionHubClient
    {
        Task ReceiveReactionNotification(int postId, string message);
    }
}
