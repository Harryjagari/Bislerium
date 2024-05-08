namespace Bislerium.server.SignalR.Interfaces
{
    public interface IReactionHubClient
    {
        Task ReceiveReactionNotification(Guid postId, string message);
    }
}
