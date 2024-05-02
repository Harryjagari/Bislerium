using Bislerium.shared.Models;

namespace Bislerium.shared.Services
{
    public interface IEmailService
    {
        void SendEmail(Message message);
    }
}
