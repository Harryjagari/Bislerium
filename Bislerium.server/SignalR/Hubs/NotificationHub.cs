//using Microsoft.AspNetCore.SignalR;
//using System.Threading.Tasks;

//namespace Bislerium.server.SignalR.Hubs
//{
//    public class NotificationHub : Hub<ICommentHubClient>
//    {
//        public async Task SendNotification(string userId, string message)
//        {
//            await Clients.User(userId).ReceiveCommentNotification(0, message);
//        }
//    }
//}
