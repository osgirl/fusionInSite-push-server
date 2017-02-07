using FusionInsite.App.Server.Data.Models;

namespace FusionInsite.App.Server.PushNotificationSender
{
    public interface IPushNotificationSender
    {
        PushResult Send(int notificationId, UserMessage message);
    }
}